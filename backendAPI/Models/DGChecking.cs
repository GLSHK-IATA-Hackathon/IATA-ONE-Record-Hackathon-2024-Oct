namespace WebAPITemplate.Models
{
    public class DGCheckingRequest
    {
        public string id {  get; set; }
        public string origin {  get; set; }
        public string destination { get; set; } 
        public string shipperName { get; set; }
        public string consigneeName { get; set; }
        public string SPH { get; set; }
        public string description { get; set; } 
    }

    public class DGCheckingResponse
    {
        public List<DGPrediction> predictions { get; set; }
    }
    
    public class DGPrediction
    {
        public string id { get; set; }
        public string instance { get; set; }
        public string prediction { get; set; }
    }
}
