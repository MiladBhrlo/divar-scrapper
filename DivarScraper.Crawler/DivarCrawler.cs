using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;
using DivarScraper.Core.Data;

namespace DivarScraper.Crawler
{
    public class DivarCrawler
    {
        private readonly AppSettings _settings;
        private readonly ICarAdRepository _repository;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public DivarCrawler(AppSettings settings, ICarAdRepository repository)
        {
            _settings = settings;
            _repository = repository;

            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public async Task CrawlAsync()
        {
            try
            {
                foreach (var cityId in _settings.CityIds)
                {
                    Console.WriteLine($"Crawling city: {cityId}");
                    await CrawlCityAsync(cityId);
                }
            }
            finally
            {
                _driver.Quit();
            }
        }

        private async Task CrawlCityAsync(string cityId)
        {
            try
            {
                var ads = new List<CarAd>();
                var url = $"{_settings.DivarBaseUrl}/s/{cityId}/car";

                _driver.Navigate().GoToUrl(url);
                await Task.Delay(2000); // Wait for page to load

                for (int i = 0; i < _settings.MaxScrolls; i++)
                {
                    var postCards = _wait.Until(d => d.FindElements(By.CssSelector("div.post-card-item")));
                    
                    foreach (var card in postCards)
                    {
                        try
                        {
                            var ad = await ExtractAdDataAsync(card);
                            if (ad != null)
                            {
                                ads.Add(ad);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error extracting ad data: {ex.Message}");
                        }
                    }

                    // Scroll to load more ads
                    ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    await Task.Delay(1000);
                }

                await _repository.SaveAdsAsync(ads);
                Console.WriteLine($"Successfully crawled {ads.Count} ads from {cityId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crawling city {cityId}: {ex.Message}");
            }
        }

        private async Task<CarAd> ExtractAdDataAsync(IWebElement card)
        {
            var title = card.FindElement(By.CssSelector("h2.post-card__title")).Text;
            var priceText = card.FindElement(By.CssSelector("div.post-card__price")).Text;
            var token = card.GetAttribute("data-token");
            var link = card.FindElement(By.CssSelector("a.post-card__link")).GetAttribute("href");

            // Extract year and kilometer from title
            var year = ExtractYear(title);
            var kilometer = ExtractKilometer(title);

            // Extract city and district
            var location = card.FindElement(By.CssSelector("span.post-card__location")).Text;
            var (city, district) = ExtractLocation(location);

            // Extract car model
            var carModel = ExtractCarModel(title);

            return new CarAd
            {
                Title = title,
                Price = ParsePrice(priceText),
                Year = year,
                Kilometer = kilometer,
                Token = token,
                Link = link,
                City = city,
                District = district,
                CarModel = carModel
            };
        }

        private int ExtractYear(string title)
        {
            var match = System.Text.RegularExpressions.Regex.Match(title, @"(\d{4})");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private int ExtractKilometer(string title)
        {
            var match = System.Text.RegularExpressions.Regex.Match(title, @"(\d+)\s*کیلومتر");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }

        private (City, District) ExtractLocation(string location)
        {
            var parts = location.Split('،');
            if (parts.Length >= 2)
            {
                return (
                    new City { 
                        Name = parts[0].Trim(), 
                        PersianName = parts[0].Trim(),
                        DivarId = parts[0].Trim().GetHashCode().ToString()
                    },
                    new District { 
                        Name = parts[1].Trim(), 
                        PersianName = parts[1].Trim(),
                        DivarId = parts[1].Trim().GetHashCode().ToString()
                    }
                );
            }
            return (null, null);
        }

        private CarModel ExtractCarModel(string title)
        {
            // This is a simplified version. You might want to use a more sophisticated approach
            var parts = title.Split(' ');
            if (parts.Length >= 2)
            {
                return new CarModel
                {
                    Name = $"{parts[0]} {parts[1]}",
                    PersianName = $"{parts[0]} {parts[1]}",
                    DivarId = $"{parts[0]}_{parts[1]}".GetHashCode().ToString(),
                    Brand = new CarBrand
                    {
                        Name = parts[0],
                        PersianName = parts[0],
                        DivarId = parts[0].GetHashCode().ToString()
                    }
                };
            }
            return null;
        }

        private long ParsePrice(string priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText) || priceText.Contains("توافقی"))
                return 0;

            var cleanPrice = new string(priceText.Where(c => char.IsDigit(c)).ToArray());
            return long.TryParse(cleanPrice, out var price) ? price : 0;
        }
    }
} 