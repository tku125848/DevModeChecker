using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            var list = await GetListFromWebApi();

            
            await ProcessList(list);
        }

        static async Task<List<Item>> GetListFromWebApi()
        {
            List<Item> list = new List<Item>();
            Item item = new Item();
            item.Url = "{url}";
            item.Key = Item.GetFakeKey();
            item.Content = Item.GetFakeContent();
            list.Add(item);
            return list;
            // using (var client = new HttpClient())
            // {
            //     var response = await client.GetAsync("https://example.com/api/list");
            //     response.EnsureSuccessStatusCode();
            //
            //     var content = await response.Content.ReadAsStringAsync();
            //     var list = JsonConvert.DeserializeObject<List<Item>>(content);
            //     return list;
            // }
        }

        static async Task ProcessList(List<Item> list)
        {
            var tasks = list.Select(async item =>
            {
                var url = item.Url;
                var key = item.Key;
                var content = item.Content;

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();
                    
                    Console.WriteLine($"URL: {url},  Valid: {item.Valid(result)}");
                    Console.WriteLine($"Result: \n{result}");
                    // var exists = content.Contains(hash);
                    //
                    // Console.WriteLine($"URL: {url}, Hash: {hash}, Exists: {exists}");
                }
            });

            await Task.WhenAll(tasks);
        }
    }

    public class Item
    {
        public string Url { get; set; }

        // https://phys.url.tku.edu.tw/
        public string Key { get; set; }
        public string Content { get; set; }

        public bool Valid(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var metas = doc.DocumentNode.SelectNodes("//meta");

            foreach (var meta in metas)
            {
                var name = meta.GetAttributeValue("name", string.Empty);
                var content = meta.GetAttributeValue("content", string.Empty);

                if (name == Key && content == Content)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFakeKey()
        {
            return "{key}";
        }

        public static string GetFakeContent()
        {
            return "Yes";
        }
    }
}