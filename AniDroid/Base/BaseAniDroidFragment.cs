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
using AniDroid.AniList.Interfaces;

namespace AniDroid.Base
{
    public abstract class BaseAniDroidFragment : Android.Support.V4.App.Fragment, IAniDroidView
    {
        protected BaseAniDroidActivity BaseActivity => Activity as BaseAniDroidActivity;
        protected new LayoutInflater LayoutInflater => BaseActivity.LayoutInflater;

        public abstract void OnError(IAniListError error);

        public void DisplaySnackbarMessage(string message, int length) => BaseActivity.DisplaySnackbarMessage(message, length);

        public void DisplayNotYetImplemented() => BaseActivity.DisplayNotYetImplemented();
    }
}