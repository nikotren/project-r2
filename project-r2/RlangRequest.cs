using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_r2
{
    public class RlangRequest
    {
        public string command { get; set; }
        public string delimiter { get; set; }
        public string dataset { get; set; }
        public string formula_x { get; set; }
        public string formula_y { get; set; }
    }
}
