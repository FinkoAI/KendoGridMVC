using System.Collections.Generic;

namespace KendoUIApp.Models
{
    public class ParseContentRepository
    {
        private readonly IParseContent _repo;

        public ParseContentRepository(Website website)
        {
            switch (website)
            {
                case Website.Sapato:
                    _repo = new SapatoParsingRepo();
                    break;
                case Website.Bashmag:
                    _repo = new BashmagParsingRepo();
                    break;
                case Website.Ekonika:
                    _repo = new EkonikaParsingRepo();
                    break;
                case Website.Lamoda:
                    _repo = new LamodaParsingRepo();
                    break;
                case Website.Spvtomske:
                    _repo = new SpvtomskeParsingRepo();
                    break;
            }
        }

        public Item ParseItem(string url)
        {
            return _repo.ParseItem(url);
        }

        public List<Item> ParsePage(string url)
        {
            return _repo.ParsePage(url);
        }

        public List<Item> ParseAllPages(string url)
        {
            return _repo.ParseAllPages(url);
        }

    }

    public enum Website
    {
        Sapato,
        Bashmag,
        Ekonika,
        Lamoda,
        Spvtomske
    }
}