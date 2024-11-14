using System.Collections.Generic;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Interfaces;
using Newtonsoft.Json;

namespace AniDroid.AniList.Models
{
    public class Connection<TEdgeType, TNodeType> : IPagedData<TEdgeType> where TEdgeType : ConnectionEdge<TNodeType>
    {
        [JsonProperty("Edges")]
        public ICollection<TEdgeType> Data { get; set; }
        public ICollection<TNodeType> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
