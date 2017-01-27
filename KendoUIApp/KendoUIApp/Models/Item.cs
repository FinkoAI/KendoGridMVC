using System.Collections.Generic;

namespace KendoUIApp.Models
{
    public class Item
    {
        public string Id { get; set; }
        public List<string> ImageUrls { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Brand { get; set; }
        public List<Size> Sizes { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }
    }
}