using System.Collections.Generic;

namespace KendoUIApp.Models
{
    internal interface IParseContent
    {
        Item ParseItem(string url);
        List<Item> ParsePage(string url);
        List<Item> ParseAllPages(string url);
    }
}