using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ITW_MobileApp
{
    public class CalendarFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            //Tab.axml is a placeholder which will be changed out for something else. 
            var view = inflater.Inflate(Resource.Layout.Tab, container, false);
            var sampleTextView = view.FindViewById<TextView>(Resource.Id.textView);
            sampleTextView.Text = "This is Calender View";
            return view;
        }
    }
}