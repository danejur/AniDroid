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

        public IAsyncEnumerator<OneOf<IPagedData<NyaaSiSearchResult>, IAniListError>> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
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

            public async ValueTask<bool> MoveNextAsync()
            {
                if (!_pageInfo.HasNextPage)
                {
                    return false;
                }

                Current = await NyaaSiService.SearchAsync(_request);

                if (Current.Match((IAniListError error) => true)
                    .Match(data => data.PageInfo?.HasNextPage != true))
                {
                    return false;
                }

                _request.PageNumber += 1;
                Current.Switch(result => _pageInfo.HasNextPage = result.PageInfo.HasNextPage = result.Data.Count >= 75);

                return true;
            }

            public ValueTask DisposeAsync()
            {
                Current = null;

                return new ValueTask(Task.CompletedTask);
            }
        }
    }
}
