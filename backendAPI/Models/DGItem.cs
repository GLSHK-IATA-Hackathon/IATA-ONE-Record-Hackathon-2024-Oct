namespace WebAPITemplate.Models
{
    public class DGItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }

        public static DGItem FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            DGItem dgValues = new DGItem();
            dgValues.ID = Convert.ToInt32(values[0]);
            dgValues.Name = Convert.ToString(values[1]);
            dgValues.Remark = Convert.ToString(values[2]);
            return dgValues;
        }
    }
}