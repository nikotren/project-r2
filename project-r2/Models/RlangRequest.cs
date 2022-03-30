using System.Text.Json.Serialization;

namespace project_r2.Models
{
    public class RlangRequest
    {
        public string command { get; set; }
        public string dataset { get; set; }
        public bool image_output { get; set; }

        [JsonPropertyName("params")]
        public RlangParams[] parameters { get; set; }
    }

    public class RlangParams
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
