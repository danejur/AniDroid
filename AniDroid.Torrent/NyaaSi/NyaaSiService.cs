using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AniDroid.Torrent.NyaaSi
{
    public class NyaaSiService
    {
        private static string NyaaPantsuBaseAddress = "https://nyaa.pantsu.cat";
        private static string NyaaSiBaseAddress = "https://nyaa.si";

        public static async Task<List<NyaaSiSearchResult>> SearchAsync(NyaaSiSearchRequest searchReq)
        {
            var searchString = $"{NyaaSiBaseAddress}/?f={searchReq.Filter}&c={searchReq.Category}&q={(searchReq.SearchTerm ?? "").Replace(" ", "+")}&p={searchReq.PageNumber}";
            var webReq = WebRequest.Create(searchString);
            var webResp = await webReq.GetResponseAsync();
            var webRespStream = webResp.GetResponseStream();

            var retList = new List<NyaaSiSearchResult>();
            var doc = new HtmlDocument();
            doc.Load(webRespStream);
            var torrentTable = doc.DocumentNode.Descendants().FirstOrDefault(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("torrent-list")).Descendants("tbody").First();
            var torrentRows = torrentTable.Descendants("tr");

            foreach (var element in torrentRows)
            {
                try
                {
                    var category = element.Descendants("td").First().Descendants("a").First().Attributes["href"].Value.Replace("/?c=", "");
                    var title = element.Descendants("td").ElementAt(1).Descendants("a").First(x => !x.Attributes.Contains("class") || !x.Attributes["class"].Value.Contains("comments")).InnerText.Trim(new[] { '\n' });
                    var link = element.Descendants("td").ElementAt(2).Descendants("a").FirstOrDefault(x => x.Attributes["href"].Value.Contains("magnet"))?.Attributes["href"].Value;
                    var siteLink = $"{NyaaSiBaseAddress}{element.Descendants("td").ElementAt(1).Descendants("a").First().Attributes["href"].Value}";
                    var date = element.Descendants("td").ElementAt(4).InnerText;
                    var size = element.Descendants("td").ElementAt(3).InnerText;
                    var description = string.Empty;

                    switch (element.Attributes["class"].Value)
                    {
                        case "success":
                            description = "trusted";
                            break;
                        case "danger":
                            description = "remake";
                            break;
                    }

                    var result = new NyaaSiSearchResult
                    {
                        Title = title,
                        Link = link,
                        PublishDate = DateTime.Parse(date),
                        Description = description,
                        Category = NyaaSiConstants.TorrentCategories.GetDisplayCategory(category),
                        Guid = $"{NyaaSiBaseAddress}{siteLink}",
                        Size = size

                    };

                    retList.Add(result);
                }
                catch
                {
                    //something went wrong, but we're not concerned about it
                }
            }

            return retList;
        }
    }
}
