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
using Evernote.AndroidJob;

namespace AniDroid.Jobs
{
    public class AniDroidJobCreator : Java.Lang.Object, IJobCreator
    {
        private readonly Context _context;

        public AniDroidJobCreator(Context context)
        {
            _context = context;
        }

        public Job Create(string tag)
        {
            switch (tag)
            {
                case AniListNotificationJob.Tag:
                    return new AniListNotificationJob(_context);
            }

            return null;
        }
    }
}