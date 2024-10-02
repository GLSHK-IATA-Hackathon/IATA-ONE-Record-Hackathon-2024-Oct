namespace WebAPITemplate.Models
{
    public class DGAIRequest
    {
        public List<DGAIInstance> instances { get; set; }
    }

    public class DGAIInstance
    {
        public string id { get; set; }
        public string instance { get; set; }

        //public DGAIInstance(DGCheckingRequest i) {
        //    id = i.id;
        //    instance = (i.origin ?? "") 
        //        + "," + (i.destination ?? "")
        //        + "," + (i.shipperName ?? "")
        //        + "," + (i.consigneeName ?? "")
        //        + "," + (i.SPH ?? "")
        //        + "," + (i.description ?? "");
        //}
    }
}
