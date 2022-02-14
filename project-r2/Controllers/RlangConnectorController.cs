using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace project_r2.Controllers
{
    [ApiController]
    public class RlangConnectorController : ControllerBase
    {
        private readonly ILogger<RlangConnectorController> _logger;
        private REngine engine;
        const int THREAD_STACK_SIZE = 2500000;

        public RlangConnectorController(ILogger<RlangConnectorController> logger)
        {
            _logger = logger;

            //COPY THE \bin\x64\Rlapack.dll to \library\stats\libs\x64\
            REngine.SetEnvironmentVariables("C:\\Program Files\\R\\R-3.6.3\\bin\\x64", "C:\\Program Files\\R\\R-3.6.3");

            StartupParameter rinit = new StartupParameter
            {
                Quiet = false,
                RHome = "C:\\Program Files\\R\\R-3.6.3",
                Interactive = true,
            };

            engine = REngine.GetInstance("C:\\Program Files\\R\\R-3.6.3\\bin\\x64\\R.dll", true, rinit);
        }

        [HttpPost]
        [Route("api/v1/r/live")]
        public RlangConnector Live(RlangRequest r)
        {
            string command_lined = SpliceText(r.command, 400);

            string[] result = null;
            int status = 200;
            string message = "ok";
            string finalr = "";

            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = engine.Evaluate(command_lined).AsCharacter().ToArray();
                    }
                    catch (RDotNet.EvaluationException e)
                    {
                        status = 400;
                        message = e.Message;
                    }
                }, THREAD_STACK_SIZE);

            thread.Start();
            thread.Join();

            if (message == "ok")
            {
                StringBuilder builder = new StringBuilder();
                foreach (string v in result)
                {
                    builder.Append(v);
                }

                finalr = builder.ToString();
            }

            return new RlangConnector
            {
                StatusCode = status,
                Message = message,
                Result = finalr,
            };
        }

        [HttpPost]
        [Route("api/v1/r/dataset/text")]
        public RlangConnector DatasetText(RlangRequest r)
        {
            //byte[] b64 = Convert.FromBase64String(r.Dataset);
            //string decoded = Encoding.UTF8.GetString(b64);
            /* Implement this
             * https://stackoverflow.com/questions/49623768/an-unhandled-exception-of-type-system-stackoverflowexception-occurred-in-rdotn
             */
            string b64_lined = SpliceText(r.dataset, 400);
            string command_lined = SpliceText(r.command, 400);

            string cmd = string.Format(@"
            b64data = ""{0}""
            decoded = rawToChar(base64enc::base64decode(b64data))
            data = read.csv(text = decoded, sep = {1})
            
            {2}
            ", b64_lined, r.delimiter, command_lined);

            string[] result = null;

            var thread = new Thread(
                () =>
                {
                    result = engine.Evaluate(cmd).AsCharacter().ToArray();
                }, THREAD_STACK_SIZE);

            thread.Start();
            thread.Join();
            //engine.Evaluate(cmd);
            //string[] result = engine.Evaluate("encoded").AsCharacter().ToArray();

            StringBuilder builder = new StringBuilder();
            foreach (string v in result)
            {
                builder.Append(v);
            }

            string finalr = builder.ToString();

            return new RlangConnector
            {
                StatusCode = 200,
                Result = finalr,
            };
        }

        [HttpPost]
        [Route("api/v1/r/dataset/image")]
        public RlangConnector DatasetImage(RlangRequest r)
        {
            string b64_lined = SpliceText(r.dataset, 400);
            string command_lined = SpliceText(r.command, 400);

            string cmd = string.Format(@"
            b64data = ""{0}""
            decoded = rawToChar(base64enc::base64decode(b64data))
            data = read.csv(text = decoded, sep = {1})
            fn = tempfile(fileext = '.png')
            png(fn)
            {2}
            dev.off()
            encoded = base64enc::base64encode(fn)
            ", b64_lined, r.delimiter, command_lined);

            string[] result = null;

            var thread = new Thread(
                () =>
                {
                    engine.Evaluate(cmd);
                    result = engine.Evaluate("encoded").AsCharacter().ToArray();
                }, THREAD_STACK_SIZE);

            thread.Start();
            thread.Join();

            StringBuilder builder = new StringBuilder();
            foreach (string v in result)
            {
                builder.Append(v);
            }

            string finalr = builder.ToString();

            return new RlangConnector
            {
                StatusCode = 200,
                Result = finalr,
            };
        }
        public static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }

        ~RlangConnectorController() 
        {
            engine.Dispose();
        }
    }
}
