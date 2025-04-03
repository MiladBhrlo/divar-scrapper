using System;
using System.Threading.Tasks;
using DivarScraper.Core.Settings;
using DivarScraper.Core.Data;

namespace DivarScraper.Crawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Divar Crawler...");

                var settings = new AppSettings();
                var repository = new SqliteCarAdRepository(settings);
                var crawler = new DivarCrawler(settings, repository);

                await crawler.CrawlAsync();

                Console.WriteLine("Crawling completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
