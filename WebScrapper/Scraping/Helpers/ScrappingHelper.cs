using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");
        public static readonly Regex IpRegex = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b");
        public static HtmlDocument GetHtmlDocument(String url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDocument = web.Load(url);
            return htmlDocument;
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