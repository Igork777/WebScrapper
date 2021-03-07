using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static readonly List<ProductType> _allProductTypes = new List<ProductType>()
        {
            new ProductType() {ProductTypeId = 1, Type = "Indoor"},
            new ProductType() {ProductTypeId = 2, Type = "Outdoor"},
            new ProductType() {ProductTypeId = 3, Type = "Tool"},
            new ProductType() {ProductTypeId = 4, Type = "Other"}
        };

        public static List<String> proxies;
        public static List<int> ports;

        public static readonly List<Website> _allWebsites = new List<Website>()
        {
            new Website() {WebsiteId = 1, Name = "flugger.dk"},
            new Website() {WebsiteId = 2, Name = "flugger-helsingor.dk"},
            new Website() {WebsiteId = 3, Name = "www.maling-halvpris.dk"},
            new Website() {WebsiteId = 4, Name = "www.flugger-horsens.dk"}
        };
        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");
        public static readonly Regex IpRegex = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b");
        public static HtmlDocument GetHtmlDocument(String url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDocument = web.Load(url);
            return htmlDocument;
        }
        
        public static void RenewIpAndPorts()
        {
            Dictionary<String, int> proxiesAndPorts = GetProxyAndPort();
            proxies = proxiesAndPorts.Keys.ToList();
            ports = proxiesAndPorts.Values.ToList();
        }

        public static String hashData(String content)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(content));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }
        
      
        
        public static HtmlDocument GetHtmlDocument(String url, String proxy, int port)
        {
            WebProxy prox = new WebProxy(proxy, port);
            prox.UseDefaultCredentials = true;
            prox.Credentials = CredentialCache.DefaultCredentials;
          
            WebClient client = new WebClient();
            client.Proxy = prox;

            String baseHtml = "";
            byte[] pageContent = client.DownloadData(url);
            UTF8Encoding utf = new UTF8Encoding();
            baseHtml = utf.GetString(pageContent);
            HtmlDocument pageHtml = new HtmlDocument();
            pageHtml.LoadHtml(baseHtml);

            return pageHtml;
        }
        public static Dictionary<String, int> GetProxyAndPort()
        {
            Dictionary<String, int> proxyAndPort = new Dictionary<String, int>();
            HtmlDocument htmlDocument = GetHtmlDocument("https://free-proxy-list.net/uk-proxy.html");
            HtmlNodeCollection ips = htmlDocument.DocumentNode.SelectNodes("//tbody/tr/td[1]");
            HtmlNodeCollection ports = htmlDocument.DocumentNode.SelectNodes("//tbody/tr/td[2]");
            Console.WriteLine();
            for (int i = 0; i < ips.Count; i++)
            {
               proxyAndPort.Add(ips[i].InnerText, Int32.Parse(ports[i].InnerText));
            }

            return proxyAndPort;


        }



        public static String FixInvalidCharacter(String name, Regex invalidCharacter)
        {
            String fixedName = name;
            MatchCollection incorrectCharacters = invalidCharacter.Matches(name);
            foreach (Match incorrectCharacter in incorrectCharacters)
            {
                fixedName = fixedName.Replace(incorrectCharacter.Value,
                    HttpUtility.HtmlDecode(incorrectCharacter.Value));
            }
            return fixedName;
        }
        
        
        
        public static bool CheckIfInvalidCharacter(String name, Regex regex)
        {
            return regex.IsMatch(name);
        }
        
        
         
    }
}