using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace AniDroid.Widgets
{
    public class SideScrollingList : RelativeLayout
    {
        private TextView _label;
        private RecyclerView _recyclerView;

        public SideScrollingList(Context context) : base(context)
        {
            Initialize(context, null, null, null);
        }

        public SideScrollingList(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(context, attrs, null, null);
        }

        public SideScrollingList(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Initialize(context, attrs, defStyleAttr, null);
        }

        public SideScrollingList(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Initialize(context, attrs, defStyleAttr, defStyleRes);
        }

        private void Initialize(Context context, IAttributeSet attrs, int? defSyleAttr, int? defStyleRes)
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.Widget_SideScrollingList, this, true);
            _label = FindViewById<TextView>(Resource.Id.SideScrollingList_Label);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.SideScrollingList_Recycler);
        }

        public string LabelText
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public RecyclerView.Adapter RecyclerAdapter
        {
            get => _recyclerView.GetAdapter();
            set => _recyclerView.SetAdapter(value);
        }
    }
}