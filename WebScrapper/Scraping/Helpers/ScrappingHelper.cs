using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using WebScrapper.Scraping.DTO;
using WebScrapper.Scraping.ScrappingFluggerDk.DB;
using WebScrapper.Scraping.ScrappingFluggerDk.Enums;

namespace WebScrapper.Scraping.Helpers
{
    public class ScrappingHelper
    {
        public static List<ProductType> _allProductTypes = new List<ProductType>()
        {
            new ProductType() {ProductTypeId = 1, Type = "Indoor", Products = new List<Product>()},
            new ProductType() {ProductTypeId = 2, Type = "Outdoor", Products = new List<Product>()},
            new ProductType() {ProductTypeId = 3, Type = "Other", Products = new List<Product>()}
        };

        public static List<MalingHalvprisProduct> allProductsMalingHalvpris;
        public static List<FluggerDkProduct> allProductsFluggerDk;
        public static List<FluggerHelsingorProduct> allProductsFluggerHelsingor;
        public static List<FluggerHorsensProduct> allProductsFluggerHorsens;
        public static List<Product> productsAddedDuringThisSession = new List<Product>();

        public static readonly Regex InvalidCharacter = new Regex(@"&#[0-9]+[;]|&[A-Za-z]+[;]");


        private static async Task<Product> MalingHalvprisExistsAlreadyInTheDatabase(DBContext dbContext, String hash)
        {
            Product websiteProduct =
                await dbContext.MalingHalvprisProducts.FirstOrDefaultAsync(node => node.Hash.Equals(hash));
            return websiteProduct;
        }

        private static async Task<Product> FluggerDkExistsAlreadyInTheDatabase(DBContext dbContext, String hash)
        {
            Product websiteProduct =
                await dbContext.FluggerDkProducts.FirstOrDefaultAsync(node => node.Hash.Equals(hash));
            return websiteProduct;
        }

        private static async Task<Product> FluggerHelsingorExistsAlreadyInTheDatabase(DBContext dbContext, String hash)
        {
            Product websiteProduct =
                await dbContext.FluggerHelsingorProducts.FirstOrDefaultAsync(node => node.Hash.Equals(hash));
            return websiteProduct;
        }

        private static async Task<Product> FluggerFluggerHorsensExistsAlreadyInTheDatabase(DBContext dbContext,
            String hash)
        {
            Product websiteProduct =
                await dbContext.FluggerHelsingorProducts.FirstOrDefaultAsync(node => node.Hash.Equals(hash));
            return websiteProduct;
        }


        private static async void AddToProductType(DBContext _dbContext, Product product)
        {
            ProductType productType =
                await _dbContext.ProductType.FirstOrDefaultAsync(node =>
                    node.ProductTypeId == product.ProductTypeId);
            if (productType == null)
            {
                throw new RuntimeWrappedException("The product type must be added");
            }

            if (productType.Products == null)
            {
                productType.Products = new List<Product>();
            }

            productType.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }


        public static async void SaveOrUpdateMalligHalvpris(DBContext dbContext, Product product, WebSite webSite)
        {
            String hash = hashData(product.Name + product.Size + webSite);
            product.Hash = hash;
            product.Name = product.Name.Trim();
            productsAddedDuringThisSession.Add(product);
            Product similarProduct = await MalingHalvprisExistsAlreadyInTheDatabase(dbContext, hash);
            if (similarProduct != null)
            {
                if (!similarProduct.CurrentPrice.Equals(product.CurrentPrice))
                {
                    similarProduct.CurrentPrice = product.CurrentPrice;
                    await dbContext.SaveChangesAsync();
                }
            }
            else
            {
                IEnumerable<MalingHalvprisProduct> products = await dbContext.MalingHalvprisProducts.Where(p =>
                        p.Name.Equals(product.Name) &&
                        p.CurrentPrice.Equals(product.CurrentPrice) && p.ProductTypeId.Equals(product.ProductTypeId))
                    .ToListAsync();


                dbContext.MalingHalvprisProducts.Add((MalingHalvprisProduct) product);
                await dbContext.SaveChangesAsync();
                AddToProductType(dbContext, product);
            }
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

        public static string RemoveDiacritics(string text)
        {
            return text.Replace("Ü", "U").Replace("ü", "u")
                .Replace("Æ", "Ae").Replace("æ", "ae").Replace("Ø", "O").Replace("ø", "o").Replace("Å", "A")
                .Replace("å", "a").Replace("ä", "a").Replace("Ä", "A")
                .Replace("ß", "B");
        }


        public static async void LoadAllMalligHalvPrisProducts(DBContext dbContext)
        {
            allProductsMalingHalvpris = dbContext.MalingHalvprisProducts.ToList();
        }

        public static async void LoadAllFluggerDk(DBContext dbContext)
        {
            allProductsFluggerDk = dbContext.FluggerDkProducts.ToList();
        }

        public static async void LoadAllFluggerHelsingorDk(DBContext dbContext)
        {
            allProductsFluggerHelsingor = dbContext.FluggerHelsingorProducts.ToList();
        }

        public static async void LoadAllFluggerHorsensDk(DBContext dbContext)
        {
            allProductsFluggerHorsens = dbContext.FluggerHorsensProducts.ToList();
        }


        [SuppressMessage("ReSharper.DPA", "DPA0004: Closure object allocation")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void removeDeletedProductsFromDB(DBContext dbContext)
        {
            var a = allProducts;
            var b = productsAddedDuringThisSession;
            bool isFound = false;
            for (int i = 0; i < allProducts.Count; i++)
            {
                for (int j = 0; j < productsAddedDuringThisSession.Count; j++)
                {
                    if (allProducts[i].Hash.Equals(productsAddedDuringThisSession[j].Hash))
                    {
                        Console.WriteLine(allProducts[i].Name + "corresponds");
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    Console.WriteLine(allProducts[i].Name + "doesn't correspond");
                    dbContext.Product.Remove(allProducts[i]);
                    dbContext.SaveChanges();
                    Console.WriteLine(dbContext.Product.ToList().Count);
                }

                isFound = false;
            }
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