using HtmlAgilityPack;

namespace CryptoScrape
{
    public class ScrapeData
    {
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
        public string? Change { get; set; }
        public string? ChangeInProcentige { get; set; }
        public string? MarketCap { get; set; }
        public string? VolumeInCurrency { get; set; }
        public string? VolumeOutCurrency24Hr { get; set; }
        public string? TotalVolumeAllCurrencies24Hr { get; set; }
        public string? CirculatingSupply { get; set; }
    }

    public class WebScraper
    {
        public List<ScrapeData> GetScrapeDatas(string url)
        {
            var uri = new Uri(url);
            List<ScrapeData> scrapeDatas = new List<ScrapeData>();
            HtmlWeb web = new HtmlWeb();
            HtmlNode nextButton = null;
            do
            {
                var doc = web.Load(url);
                var nodes = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div/div[1]/div/div[2]/div/div/div[6]/div/div/section/div/div[2]/div[1]/table/tbody/tr[position()>1]");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        var cryptoRow = new ScrapeData
                        {
                            Symbol = node.SelectSingleNode("td[1]")?.InnerText,
                            Name = node.SelectSingleNode("td[2]")?.InnerText,
                            Price = node.SelectSingleNode("td[3]")?.InnerText,
                            Change = node.SelectSingleNode("td[4]")?.InnerText,
                            ChangeInProcentige = node.SelectSingleNode("td[5]")?.InnerText,
                            MarketCap = node.SelectSingleNode("td[6]")?.InnerText,
                            VolumeInCurrency = node.SelectSingleNode("td[7]")?.InnerText,
                            VolumeOutCurrency24Hr = node.SelectSingleNode("td[8]")?.InnerText,
                            TotalVolumeAllCurrencies24Hr = node.SelectSingleNode("td[9]")?.InnerText,
                            CirculatingSupply = node.SelectSingleNode("td[10]")?.InnerText,
                        };
                        scrapeDatas.Add(cryptoRow);
                    }
                }

                nextButton = doc.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div/div[1]/div/div[2]/div/div/div[6]/div/div/section/div/div[2]/div[2]/button[3]");
                if (nextButton != null)
                {
                    var hrefValue = nextButton.GetAttributeValue("href", null);
                    if (!string.IsNullOrEmpty(hrefValue))
                    {
                        url = uri.Scheme + "://" + uri.Authority + hrefValue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            } while (nextButton != null && !nextButton.GetAttributeValue("class", "").Contains("disabled"));

            return scrapeDatas;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var scraper = new WebScraper();
            var url = "https://finance.yahoo.com/crypto/?count=100&offset=0";

            while (true)
            {
                var cryptoData = scraper.GetScrapeDatas(url);
                Console.WriteLine($"Scraping complete. Retrieved data for {cryptoData.Count} cryptocurrencies.");
                foreach (var data in cryptoData)
                {
                    Console.WriteLine($"Symbol: {data.Symbol}, Name: {data.Name}, Price: {data.Price}, Change: {data.Change}, Change in Percentage: {data.ChangeInProcentige}, Market Cap: {data.MarketCap}, Volume in Currency: {data.VolumeInCurrency}, Volume Out Currency 24Hr: {data.VolumeOutCurrency24Hr}, Total Volume All Currencies 24Hr: {data.TotalVolumeAllCurrencies24Hr}, Circulating Supply: {data.CirculatingSupply}");
                }
                await Task.Delay(TimeSpan.FromMinutes(0.10));
            }
        }
    }
}