using System.Collections.Generic;

namespace KendoUIApp.Models
{
    public partial class ParseContentRepository : ParserBase
    {
        public Item ParseItem(string url, Website website)
        {
            var item = new Item();
            switch (website)
            {
                case Website.Sapato:
                    item = ParseItemSaptro(url);
                    break;
                case Website.Bashmag:
                    item = ParseItemBashmag(url);
                    break;
            }
            return item;
        }

        public List<Item> ParsePage(string url, Website website)
        {
            var itemList = new List<Item>();
            switch (website)
            {
                case Website.Sapato:
                    itemList = ParsePageSaptro(url);
                    break;
                case Website.Bashmag:
                    itemList = ParsePageBashmag(url);
                    break;
            }
            return itemList;
        }

        public List<Item> ParseAllPages(string url, Website website)
        {
            var itemList = new List<Item>();
            switch (website)
            {
                case Website.Sapato:
                    itemList = ParseAllPagesSaptro(url);
                    break;
            }
            return itemList;
        }
    }

    public enum Website
    {
        Sapato,
        Bashmag
    }
}