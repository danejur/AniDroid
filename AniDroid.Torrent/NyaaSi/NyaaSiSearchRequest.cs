using System;
using System.Collections.Generic;
using System.Text;

namespace AniDroid.Torrent.NyaaSi
{
    public class NyaaSiSearchRequest
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public string Category { get; set; }
        public string Filter { get; set; }

    }
}
