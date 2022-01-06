using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Service;
using HtmlAgilityPack;
using OneOf;

namespace AniDroid.Torrent.NyaaSi
{
    public class NyaaSiService
    {
        private const string BaseAddress = "https://nyaa.si";

        public static IAsyncEnumerable<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> GetSearchEnumerable(
            NyaaSiSearchRequest searchReq)
        {
            return new NyaaSiAsyncEnumerable(searchReq);
        }

        public static async Task<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> SearchAsync(
            NyaaSiSearchRequest searchReq)
        {
            try
            {
                var searchTerms = (searchReq.SearchTerm ?? string.Empty).Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                var searchString =
                    $"{BaseAddress}/?f={searchReq.Filter}&c={searchReq.Category}&q={string.Join("+", searchTerms)}&p={searchReq.PageNumber}";

                using var httpClient = new HttpClient();
                var resp = await httpClient.GetAsync(searchString);
                await using var respStream = await resp.Content.ReadAsStreamAsync();

                var retList = new List<NyaaSiSearchResult>();
                var doc = new HtmlDocument();
                doc.Load(respStream);
                var torrentTable = doc.DocumentNode.Descendants()
                    .FirstOrDefault(x =>
                        x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("torrent-list"))
                    ?.Descendants("tbody").First();
                var torrentRows = torrentTable?.Descendants("tr") ?? new List<HtmlNode>();

                foreach (var element in torrentRows)
                {
                    var category = element.Descendants("td").First().Descendants("a").First().Attributes["href"].Value
                        .Replace("/?c=", "");
                    var title = element.Descendants("td").ElementAt(1).Descendants("a").First(x =>
                            !x.Attributes.Contains("class"))
                        .InnerText
                        .Trim('\n');
                    var link = element.Descendants("td").ElementAt(2).Descendants("a")
                        .FirstOrDefault(x => x.Attributes["href"].Value.Contains(".torrent"))?.Attributes["href"].Value;
                    var siteLink = element.Descendants("td").ElementAt(1).Descendants("a").First(x =>
                            !x.Attributes.Contains("class")).Attributes["href"]
                        .Value;
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
                        Link = $"{BaseAddress}{link}",
                        PublishDate = DateTime.Parse(date),
                        Description = description,
                        Category = NyaaSiConstants.TorrentCategories.GetDisplayCategory(category),
                        Guid = $"{BaseAddress}{siteLink}",
                        Size = size

                    };

                    retList.Add(result);
                }

                return new PagedData<NyaaSiSearchResult>
                {
                    Data = retList,
                    PageInfo = new PageInfo
                    {
                        CurrentPage = searchReq.PageNumber,
                        HasNextPage = retList.Count > 0
                    }
                };
            }

            catch (Exception e)
            {
                return new AniListError(500, "An error occurred while searching torrents", e, null);
            }
        }
    }
}
