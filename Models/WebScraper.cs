using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSeller.Models
{
    public class WebScraper
    {
        public static List<string> GetBookTypes()
        {
            //Gets all books genres.
            string pageToLoad = "https://books.toscrape.com/";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(pageToLoad);
            HtmlNode[] bookNodes = null;

            bookNodes = document.DocumentNode.SelectNodes("//div[contains(@class,'side_categories')]//ul//li//a").ToArray();

            List<string> listOfBookTypes = new List<string>();
            listOfBookTypes.Add("All");

            foreach (HtmlNode node in bookNodes)
            {
                string bookType = node.InnerText;
                bookType = bookType.Trim();
                listOfBookTypes.Add(bookType);
            }
            listOfBookTypes.Remove("Books");

            return listOfBookTypes;
        }

        public List<BookModel> GetAllBooksOfaNonSpecificGenre(int pageNumber , IProgress<ProgressReportModel> progress)
        {
            ProgressReportModel report = new();

            HtmlWeb web = new HtmlWeb();
            HtmlNode[] referenceNodes = null;
            HtmlDocument document = null;

            List<BookModel> books = new();
            List<string> listOfBookReferences = new();
            string pageToLoad = $"http://books.toscrape.com/catalogue/page-{pageNumber}.html";

            try
            {
                document = web.Load(pageToLoad);
            }
            catch (Exception ex)
            {
                MiscFunctions.ErrorBox(ex.Message);
            }

            try
            {
                referenceNodes = document.DocumentNode.SelectNodes("//li//article//h3//a[@href]").ToArray();
            }
            catch (Exception ex)
            {
                MiscFunctions.ErrorBox(ex.Message);
            }

            foreach (HtmlNode node in referenceNodes)
            {
                //listOfBookReferences will hold the links to the books pages
                listOfBookReferences.Add(node.GetAttributeValue("href", string.Empty));
            }

            foreach (string boofRef in listOfBookReferences)
            {
                //Will go to the book specific page and get the book data.
                HtmlDocument specificBookPage = web.Load($"https://books.toscrape.com/catalogue/{boofRef}");

                string bookTitle = specificBookPage.DocumentNode.SelectSingleNode("//div//h1").InnerText;
                string bookPrice = specificBookPage.DocumentNode.SelectSingleNode("//p[contains(@class,'price_color')]").InnerText;
                var table = specificBookPage.DocumentNode.SelectNodes("//table[contains(@class,'table table-striped')]/tr/td").ToArray();
                string upc = table[0].InnerText;
                string bookStock = table[5].InnerText;
                books.Add(new BookModel() { BookTitle = bookTitle, BookPrice = bookPrice, BookStock = bookStock, BookUPC = upc });

                report.BooksProcessed = books;
                report.PercentageComplete = (books.Count * 100)/ listOfBookReferences.Count;
                progress.Report(report);

            }
            return books;
        }

        public List<BookModel> GetBooksOfaSpecificGenre(string genre , int pageNumber , IProgress<ProgressReportModel> progress)
        {
            ProgressReportModel report = new();

            HtmlWeb web = new HtmlWeb();
            HtmlNode[] referenceNodes = null;
            HtmlDocument document = null;

            string genreToGetLink = GetLinkToGenre(genre);
            string pageToLoad = "";
            if (pageNumber > 1)
            {
                //If we want to load any other pager other than page 1 we will use page number.
                pageToLoad = $"https://books.toscrape.com/{genreToGetLink}/page-{pageNumber}.html";
            }
            else
            {
                //If we want to load the first number, we will use index page.
                pageToLoad = $"https://books.toscrape.com/{genreToGetLink}/index.html";
            }
            
            List<BookModel> books = new();
            List<string> listOfBookReferences = new();

            try
            {
                document = web.Load(pageToLoad);
            }
            catch (Exception ex)
            {
                MiscFunctions.ErrorBox(ex.Message);
            }

            try
            {
                referenceNodes = document.DocumentNode.SelectNodes("//li//article//h3//a[@href]").ToArray();
            }
            catch (Exception ex)
            {
                MiscFunctions.ErrorBox($"{ex.Message}\n No pages to load");
            }

            if(referenceNodes != null)
            {
                for (int i = 0; i < referenceNodes.Length; i++)
                {
                    HtmlNode node = referenceNodes[i];
                    listOfBookReferences.Add(node.GetAttributeValue("href", string.Empty));
                }
            }

            foreach (string boofRef in listOfBookReferences)
            {
                

                string aux = boofRef.Remove(0, 9);
                //Example:
                //We get a data like this -> bookRef = ../../../sharp-objects_997/index.html
                //We remove the firts 9 chars so that remains only 'sharp-objects_997/index.html'

                HtmlDocument specificBookPage = web.Load($"https://books.toscrape.com/catalogue/{aux}");

                string bookTitle = specificBookPage.DocumentNode.SelectSingleNode("//div//h1").InnerText;
                string bookPrice = specificBookPage.DocumentNode.SelectSingleNode("//p[contains(@class,'price_color')]").InnerText;
                var table = specificBookPage.DocumentNode.SelectNodes("//table[contains(@class,'table table-striped')]/tr/td").ToArray();
                string upc = table[0].InnerText;
                string bookStock = table[5].InnerText;
                books.Add(new BookModel() { BookTitle = bookTitle, BookPrice = bookPrice, BookStock = bookStock, BookUPC = upc });

                report.BooksProcessed = books;
                report.PercentageComplete = (books.Count * 100) / listOfBookReferences.Count;
                progress.Report(report);
            }
            return books;
        }

        public string GetLinkToGenre(string genre)
        {
            //This class will return the link to the page of a specific book genre.

            string lintToPage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlNode[] bookNodes = null;
            HtmlNode[] refToBooks = null;
            string pageToLoad = "https://books.toscrape.com/";
            HtmlDocument document = web.Load(pageToLoad);

            try
            {
                bookNodes = document.DocumentNode.SelectNodes("//div[contains(@class,'side_categories')]//ul//li//a").ToArray();
                refToBooks = document.DocumentNode.SelectNodes("//div[contains(@class,'side_categories')]//ul//li//a[@href]").ToArray();
            }
            catch (Exception ex)
            {
                MiscFunctions.ErrorBox(ex.Message);
            }

            int aux = 0;

            foreach (HtmlNode node in bookNodes)
            {
                string bookyType = node.InnerText;
                string bookRef = refToBooks[aux].GetAttributeValue("href", string.Empty);
                bookRef = bookRef.Trim();
                bookyType = bookyType.Trim();
                aux++;

                if(bookyType == genre)
                {
                    lintToPage = bookRef;
                    int start = lintToPage.LastIndexOf('/');
                    int end = lintToPage.Length;
                    string newLink = lintToPage.Remove(start, end-start); //Removes unnaccessary chars from the link.
                    lintToPage = newLink;
                    break;
                }
            }
            return lintToPage;
        }
    }
}
