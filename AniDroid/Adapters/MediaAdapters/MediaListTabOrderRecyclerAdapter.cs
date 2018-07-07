using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaListTabOrderRecyclerAdapter : BaseDraggableRecyclerAdapter<KeyValuePair<string, bool>>
    {
        public MediaListTabOrderRecyclerAdapter(BaseAniDroidActivity context, List<KeyValuePair<string, bool>> items) : base(context, items)
        {
        }
    }
}