namespace Sarjee.SimpleRenamer.Common.Model
{
    public class ShowView
    {
        public string Id { get; set; }
        public string ShowName { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }

        public ShowView(string id, string showname, string year, string description)
        {
            Id = id;
            ShowName = showname;
            Year = year;
            Description = description;
        }
    }
}
