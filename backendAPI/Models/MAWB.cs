using System.Collections.Generic;

namespace WebAPITemplate.Models
{
    public class CreateMAWBRequest
    {
        public string awbPrefix {  get; set; }
        public string awbSuffix { get; set; }
        public List<House> houses {  get; set; }
    }

    public class CreateMAWBResponse
    {
        public string result { get; set; }
    }
    
    public class House
    {
        public string waybillNumber { get; set; }
        public string goodsDesc { get; set; }
        public List<string> pieces { get; set; }
    }

}
