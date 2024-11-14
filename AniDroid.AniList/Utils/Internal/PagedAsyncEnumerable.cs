using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using OneOf;

namespace AniDroid.AniList.Utils.Internal 
{
    internal class PagedAsyncEnumerable<T> : IAsyncEnumerable<OneOf<IPagedData<T>, IAniListError>>
    {
        private readonly Func<PagingInfo, CancellationToken, Task<OneOf<IPagedData<T>, IAniListError>>> _getPage;
        private readonly Func<PagingInfo, OneOf<IPagedData<T>, IAniListError>, bool> _nextPage;

        public int PageSize { get; }

        public PagedAsyncEnumerable(int pageSize,
            Func<PagingInfo, CancellationToken, Task<OneOf<IPagedData<T>, IAniListError>>> getPage,
            Func<PagingInfo, OneOf<IPagedData<T>, IAniListError>, bool> nextPage)
        {
            if (pageSize <= 0) throw new ArgumentException($"Value cannot be less than or equal to zero (0)", nameof(pageSize));
            PageSize = pageSize;
            _getPage = getPage ?? throw new ArgumentNullException(nameof(getPage));
            _nextPage = nextPage ?? throw new ArgumentNullException(nameof(nextPage));
        }

        public IAsyncEnumerator<OneOf<IPagedData<T>, IAniListError>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new Enumerator(this);

        public class Enumerator : IAsyncEnumerator<OneOf<IPagedData<T>, IAniListError>>
        {
            private readonly PagedAsyncEnumerable<T> _source;
            private readonly PagingInfo _info;

            public OneOf<IPagedData<T>, IAniListError> Current { get; private set; }

            public Enumerator(PagedAsyncEnumerable<T> source)
            {
                _source = source;
                _info = new PagingInfo(source.PageSize);
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                if (_info.Remaining == false)
                    return false;

                var pageResult = await _source._getPage(_info, default);

                Current = pageResult;

                if (Current.Match((IAniListError error) => true)
                    .Match(data => (data.PageInfo?.CurrentPage ?? 0) == 0))
                {
                    return false;
                }

                _info.Page++;
                _info.Remaining = _source._nextPage(_info, Current);

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
