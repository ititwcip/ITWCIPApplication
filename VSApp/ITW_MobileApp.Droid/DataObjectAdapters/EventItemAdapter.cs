using System;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace ITW_MobileApp.Droid
{
    public class EventItemAdapter : BaseAdapter<EventItem>
    {
        Activity activity;
        int layoutResourceId;
        List<EventItem> items = new List<EventItem>();

        public EventItemAdapter(Activity activity, int layoutResourceId)
        {
            this.activity = activity;
            this.layoutResourceId = layoutResourceId;
        }

        //Returns the view for a specific item on the list
        //TODO: fix view
        public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var row = convertView;
            var currentItem = this[position];
            return row;
        }

        public void Add(EventItem item)
        {
            items.Add(item);
            NotifyDataSetChanged();
        }

        public void Clear()
        {
            items.Clear();
            NotifyDataSetChanged();
        }

        public void Remove(EventItem item)
        {
            items.Remove(item);
            NotifyDataSetChanged();
        }

        #region implemented abstract members of BaseAdapter

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override EventItem this[int position]
        {
            get
            {
                return items[position];
            }
        }

        #endregion
    }
}