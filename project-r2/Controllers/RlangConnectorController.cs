using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RDotNet;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using project_r2.Models;
using Microsoft.Extensions.Configuration;

namespace project_r2.Controllers
{
    [ApiController]
    public class RlangConnectorController : ControllerBase
    {
        /* Global variable of REngine,
         * which will hold state over the app runtime.
         * Also, we declare new thread stack size of 4MB,
         * which will be using in custom REngine evaluate thread below.
         */
        private readonly ILogger<RlangConnectorController> _logger;
        private readonly IConfiguration _config;
        private REngine engine;
        const int THREAD_STACK_SIZE = 4 * 1024 * 1024;

        public RlangConnectorController(ILogger<RlangConnectorController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            /* !! COPY THE \bin\x64\Rlapack.dll to \library\stats\libs\x64\ !!
             * There is an error (or bug?) in RDotNet, where REngine won't start
             * without Rlapack.dll in \library\stats\libs\x64\ (in RHome path).
             * Also, we explicitly set rPath and rHome variables 
             * from appsettings.json config file.
             * RDotNet allows for automatic rPath/rHome detection, but there
             * were some problems with it (maybe just on my device).
             * Solution found here: https://stackoverflow.com/a/48211343
             */
            REngine.SetEnvironmentVariables(_config.GetValue<string>("R:rPath"), _config.GetValue<string>("R:rHome"));

            /* We set default parameters for REngine. 
             * Quiet mode is by default turned off so we can catch errors.
             * We can change the value in appsettings.json.
             * Also, there are two R modes, batch or interactive.
             * We use batch in our case.
             * At last, we set bigger maximal memory limit, for
             * running large scripts.
             */
            ulong maxMemSize = 128 * 1024 * 1024;
            StartupParameter rinit = new StartupParameter
            {
                Quiet = _config.GetValue<bool>("R:quiet"),
                Interactive = false,
                MaxMemorySize = maxMemSize,
                StackSize = THREAD_STACK_SIZE,
            };

            /* Finally, create REngine global instance, which
             * will serve all of our R scripts.
             * There is also a 'weird' behaviour, where we cannot
             * create another instance of REngine when one is disposed.
             * This REngine has to be running the whole time when
             * app is running.
             */
            if (engine == null)
            {
                engine = REngine.GetInstance(null, true, rinit);
            }
        }

        [HttpPost]
        [Route("api/v1/r/compute")]
        public RlangConnector Compute(RlangRequest r)
        {
            string cmd = "";

            if(!String.IsNullOrEmpty(r.dataset))
            {
                if (r.image_output)
                {
                    cmd = PrepareImgDataset(r);
                }
                else
                {
                    cmd = PrepareCmdDataset(r);
                }
            }
            else
            {
                if (r.image_output)
                {
                    cmd = PrepareImg(r);
                }
                else
                {
                    cmd = SpliceText(r.command, 400);
                }
            }

            /* We declare default responses and message. 
             * If nothing wrong happens during execution, 
             * message and status stay unchanged.
             */

            string[] result = null;
            int status = 200;
            string message = "ok";
            string finalr = "";

            /* When running intensive scripts, we had to wrap REngine evaluation
             * into custom thread with bigger thread stack size. 
             * Default stack size is 256K on 32-bit and 512 or 64-bit, which
             * is not enough in some cases.
             * Solution taken from here: https://stackoverflow.com/a/50381186
             */
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = engine.Evaluate(cmd).AsCharacter().ToArray();
                    }
                    catch (Exception e)
                    {
                        status = 400;
                        message = e.Message;
                    }
                }, THREAD_STACK_SIZE);

            thread.Start();
            thread.Join();

            /* Result from REngine is ok, so we parse the engine's output (returned as array). 
             * Also we insert new lines where they should be (except the last line).
             */
            if (message == "ok")
            {
                StringBuilder builder = new StringBuilder();
                var count = result.Length;

                foreach (string v in result)
                {
                    builder.Append(v);
                    if (--count > 0)
                    {
                        builder.Append(Environment.NewLine);
                    }
                }

                finalr = builder.ToString();
            }

            /* Finally, return our result struct, 
             * which will be automatically parsed as JSON response by AspNetCore.
             */
            return new RlangConnector
            {
                StatusCode = status,
                Message = message,
                Result = finalr,
            };
        }

        /* Helper function for creating raw R script,
         * which parses dataset from request in base64 format,
         * then read it as CSV into 'data' variable.
         * Also, we have to split long lines using SpliceText() function,
         * beacuse REngine will panic if any script's line is too long
         */
        private string PrepareCmdDataset(RlangRequest r)
        {
            string command_lined = SpliceText(r.command, 400);
            string b64_lined = SpliceText(r.dataset, 400);
            string params_formatted = PrepareParams(r);
            return string.Format(@"b64data = ""{0}""
                decoded = rawToChar(base64enc::base64decode(b64data))
                data = read.csv(text = decoded, sep = ',')
                {1}
                {2}
                ", b64_lined, params_formatted, command_lined);
        }

        /* Almost the same function as above, but here we
         * also add script which catches graphical output from
         * REngine and converts it into base64 format
         */
        private string PrepareImgDataset(RlangRequest r)
        {
            string command_lined = SpliceText(r.command, 400);
            string b64_lined = SpliceText(r.dataset, 400);
            string params_formatted = PrepareParams(r);
            return string.Format(@"b64data = ""{0}""
                decoded = rawToChar(base64enc::base64decode(b64data))
                data = read.csv(text = decoded, sep = ',')
                {1}
                fn = tempfile(fileext = '.svg')
                if (!require('svglite')) install.packages('svglite', repos = 'http://cran.us.r-project.org');
                require(svglite)
                library(svglite)
                svglite(fn, width = 5, height = 5)
                {2}
                dev.off()
                encoded = base64enc::base64encode(fn)
                encoded", b64_lined, params_formatted, command_lined);
        }

        /* Same as above, but here we are not using dataset.
         */
        private string PrepareImg(RlangRequest r)
        {
            string command_lined = SpliceText(r.command, 400);
            return string.Format(@"fn = tempfile(fileext = '.svg')
                if (!require('svglite')) install.packages('svglite', repos = 'http://cran.us.r-project.org');
                library(svglite)
                svglite(fn, width = 5, height = 5)
                {0}
                dev.off()
                encoded = base64enc::base64encode(fn)
                encoded", command_lined);
        }

        /* Helper function, which inserts new variables and values 
         * in R format by 'parameters' structure from request.
         */
        private string PrepareParams(RlangRequest r)
        {
            string result = "";

            if (r.parameters != null)
            {
                foreach (RlangParams p in r.parameters)
                {
                    if (p.type == "number") {
                        result += string.Format(@"{0} = {1} {2}", p.name, p.value, Environment.NewLine);
                    }
                    else
                    {
                        result += string.Format(@"{0} = '{1}' {2}", p.name, p.value, Environment.NewLine);
                    }
                }
            }

            return result;
        }

        /* Helper function, which inserts new lines into one long string.
         * Needed especially for placing big base64 data into R scripts.
         */
        private static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }

        /* We should clean up REngine when exiting the app.
         */
        ~RlangConnectorController() 
        {
            if (engine != null)
            {
                engine.Dispose();
            }
        }
    }
}
