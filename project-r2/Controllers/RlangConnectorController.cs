using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace project_r2.Controllers
{
    [ApiController]
    public class RlangConnectorController : ControllerBase
    {
        private readonly ILogger<RlangConnectorController> _logger;
        private REngine engine;

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

        [HttpGet]
        [Route("api/rlang/{cmd}")]
        public RlangConnector Get(String cmd)
        {
            string[] result = engine.Evaluate(cmd).AsCharacter().ToArray();

            StringBuilder builder = new StringBuilder();
            foreach (string v in result)
            {
                builder.Append(v);
            }

            string finalr = builder.ToString();

            return new RlangConnector 
            {
                //Check status code later
                StatusCode = 200,
                Result = finalr,
            };
        }

        [HttpPost]
        [Route("api/rlang/test1")]
        public RlangConnector Test1(RlangRequest r)
        {
            //byte[] b64 = Convert.FromBase64String(r.Dataset);
            //string decoded = Encoding.UTF8.GetString(b64);
            string b64 = r.Dataset;
            string b64_lined = SpliceText(b64, 400);

            string cmd = string.Format(@"b64data = ""{0}""
            decoded = rawToChar(base64enc::base64decode(b64data))
            data = read.csv(text = decoded)
            fn = tempfile(fileext = '.png')
            png(fn)
            boxplot(sepal.length~variety, data)
            dev.off()
            encoded = base64enc::base64encode(fn)
            ", b64_lined);

            //Console.WriteLine(cmd);
            engine.Evaluate(cmd);
            string[] result = engine.Evaluate("encoded").AsCharacter().ToArray();
            
            StringBuilder builder = new StringBuilder();
            foreach (string v in result)
            {
                builder.Append(v);
            }

            string finalr = builder.ToString();

            return new RlangConnector
            {
                //Check status code later
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
