﻿using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class Item
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> ImageUrls { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Brand { get; set; }
        public List<Size> Sizes { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }
        public Website WebsiteName { get; set; }

        public string SizeString
        {
            get
            {
                var result = string.Empty;
                Sizes?.AsParallel().ForEach(sz =>
                {
                    result = result + (sz.IsAvailable
                        ? string.Format("{0},", sz.SizeText)
                        : string.Format("[{0}],", sz.SizeText));
                });
                return string.IsNullOrEmpty(result) ? result : result.TrimEnd(',');
            }
        }

        public string PropertiesString
        {
            get
            {
                var result = string.Empty;
                Properties?.AsParallel().ForEach(prop =>
                {
                    result = result + string.Format("{0}:{1},", prop.Key, prop.Value);
                });
                return string.IsNullOrEmpty(result) ? result : result.TrimEnd(',');
            }
        }

        public override string ToString()
        {
            const string seperator = ",";
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"", 
                WebsiteName.ToString("G"), Url, Id,Title,
                ImageUrls != null ? String.Join(seperator, ImageUrls) : string.Empty,
                Price, Discount, Type, SubType, Brand,
                SizeString, PropertiesString
                );
        }
    }
}