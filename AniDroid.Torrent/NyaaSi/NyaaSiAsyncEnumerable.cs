using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Interfaces;
using OneOf;

namespace AniDroid.Torrent.NyaaSi
{
    public class NyaaSiAsyncEnumerable : IAsyncEnumerable<OneOf<IPagedData<NyaaSiSearchResult>,IAniListError>>
    {
        private readonly NyaaSiSearchRequest _request;

        public NyaaSiAsyncEnumerable(NyaaSiSearchRequest request)
        {
            _request = request;
        }

        public IAsyncEnumerator<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> GetEnumerator() =>
            new Enumerator(this, _request);

        public class Enumerator : IAsyncEnumerator<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>>
        {
            private readonly NyaaSiSearchRequest _request;
            private readonly PageInfo _pageInfo;

            public OneOf<IPagedData<NyaaSiSearchResult>, IAniListError> Current { get; private set; }

            public Enumerator(NyaaSiAsyncEnumerable source, NyaaSiSearchRequest request)
            {
                _request = request;
                _pageInfo = new PageInfo
                {
                    HasNextPage = true
                };
            }

            public async Task<bool> MoveNextAsync(CancellationToken ct = default(CancellationToken))
            {
                if (!_pageInfo.HasNextPage)
                {
                    return false;
                }

                var pageResult = await NyaaSiService.SearchAsync(_request);

                _request.PageNumber += 1;
                _pageInfo.HasNextPage = pageResult.Count >= 75;

                Current = new PagedData<NyaaSiSearchResult>
                {
                    Data = pageResult,
                    PageInfo = _pageInfo
                };

                return true;
            }

            public void Dispose()
            {
                Current = null;
            }
        }
    }
}
