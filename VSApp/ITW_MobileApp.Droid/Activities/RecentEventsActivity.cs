using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.View;
using System.Linq;
using System;
using Android.Gms.Common;

namespace ITW_MobileApp.Droid
{
    [Activity(Theme = "@style/MyTheme")]
    public class RecentEventsActivity : AppCompatActivity
    {

        Toolbar _supporttoolbar;
        DrawerLayout _drawer;
        NavigationView _navigationview;

        //this section starts off objects for the recycler view
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        //This should eventually be the list inside of the EventItemAdapter
        List<EventItem> myEventList;
        //This is different from the EventItemAdapter, as this how to deal with the RecyclerView
        EventListAdapter myEventListAdapter;

        EventItemAdapter eventItemAdapter;
        RecipientListItemAdapter recipientListItemAdapter;

        ErrorHandler error; 

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            await IoC.UserInfo.setEmployee();
            // Set our view from the "main" layout resource
            await IoC.UserInfo.setEmployee();
            switch (IoC.UserInfo.Employee.PrivledgeLevel)
            {
                case "Admin":
                    {
                        SetContentView(Resource.Layout.RecentEvents_Admin);
                        eventItemAdapter = new EventItemAdapter(this, Resource.Layout.RecentEvents_Admin);
                        recipientListItemAdapter = new RecipientListItemAdapter(this, Resource.Layout.RecentEvents_Admin);
                        break;
                    }                    
                case "Moderator":
                    {
                        SetContentView(Resource.Layout.RecentEvents_Moderator);
                        eventItemAdapter = new EventItemAdapter(this, Resource.Layout.RecentEvents_Moderator);
                        recipientListItemAdapter = new RecipientListItemAdapter(this, Resource.Layout.RecentEvents_Moderator);
                        break;
                    }                    
                default:
                    {
                        SetContentView(Resource.Layout.RecentEvents_User);
                        eventItemAdapter = new EventItemAdapter(this, Resource.Layout.RecentEvents_User);
                        recipientListItemAdapter = new RecipientListItemAdapter(this, Resource.Layout.RecentEvents_User);
                        break;
                    }
            }

                


            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);


            _supporttoolbar = FindViewById<Toolbar>(Resource.Id.ToolBar);
            _drawer = FindViewById<DrawerLayout>(Resource.Id.DrawerLayout);
            _navigationview = FindViewById<NavigationView>(Resource.Id.nav_view);
            ToolbarCreator toolbarCreator = new ToolbarCreator();
            toolbarCreator.setupToolbar(_supporttoolbar, _drawer, _navigationview, Resource.String.recent_events, this);

            error = new ErrorHandler(this);

            if (IsPlayServicesAvailable())
            {
                var intentRegistration = new Intent(this, typeof(RegistrationIntentService));
                StartService(intentRegistration);
            }

            await RefreshView();
            FindViewById(Resource.Id.loadingPanel).Visibility = ViewStates.Gone;


            myEventList = recipientListItemAdapter.getEventsByEmployeeID(IoC.UserInfo.EmployeeID, eventItemAdapter);
            myEventList = filterEvents();

            sortByDate(myEventList);
            //Plug in the linear layout manager
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //Plug in my adapter
            myEventListAdapter = new EventListAdapter(myEventList);
            myEventListAdapter.ItemClick += OnItemClick;
            mRecyclerView.SetAdapter(myEventListAdapter);

        }

        private List<EventItem> filterEvents()
        {
            List<EventItem> filteredList = new List<EventItem>();
            foreach (EventItem eventitem in myEventList)
            {
                if (eventitem.EventDate >= DateTime.Now)
                {
                    foreach (string filter in IoC.ViewRefresher.FilterStringList)
                    {
                        if (eventitem.Category == filter)
                        {
                            filteredList.Add(eventitem);
                        }
                    }
                    if (eventitem.Category == "Emergency")
                    {
                        filteredList.Add(eventitem);
                    }
                }
            }
            return filteredList;
        }

        public void sortByDate(List<EventItem> eventList)
        {
            eventList.Sort((x, y) => DateTime.Compare(x.EventDate, y.EventDate));
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawer.OpenDrawer(GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public async Task RefreshView()
        {
            await IoC.Dbconnect.SyncAsync(pullData: true);
            await IoC.ViewRefresher.RefreshItemsFromTableAsync(eventItemAdapter);
            await IoC.ViewRefresher.RefreshItemsFromTableAsync(recipientListItemAdapter);
        }
        public override void OnBackPressed()
        {
            if (_drawer.IsDrawerOpen(GravityCompat.Start))
            {
                _drawer.CloseDrawer(GravityCompat.Start);
            }
            else {
                MoveTaskToBack(true);
            }
        }
        void OnItemClick(object sender, int position)
        {
            //int eventNum = position + 1;
            //Toast.MakeText(this, "This is event number " + eventNum, ToastLength.Short).Show();
            var intent = new Intent(this, typeof(EventDetailsActivity));
            intent.PutExtra("Name", myEventList.ElementAt(position).Name);
            intent.PutExtra("Date", myEventList.ElementAt(position).EventDate.ToString("MMMM dd, yyyy"));
            intent.PutExtra("Time", myEventList.ElementAt(position).EventDate.ToString("h:mm tt"));
            intent.PutExtra("Location", myEventList.ElementAt(position).Location);
            intent.PutExtra("Category", myEventList.ElementAt(position).Category);
            intent.PutExtra("Description", myEventList.ElementAt(position).EventDescription);
            StartActivity(intent);
        }
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}


