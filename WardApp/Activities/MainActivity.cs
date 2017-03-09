using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

//using AndroidApp1.Fragments;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using WardApp.Fragments;
using Android.Graphics;
using Android.Content;

namespace WardApp
{
    [Activity(Label = "Home", MainLauncher = false, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/Icon")]
    public class MainActivity : BaseActivity
    {
        private DrawerLayout drawerLayout;
        private NavigationView navigationView;
        private LinearLayout layoutPbar;
        private Android.Support.V4.App.Fragment currentFrag;
        int oldPosition = -1;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.main;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            layoutPbar = FindViewById<LinearLayout>(Resource.Id.linearProgressBar);
            View v = navigationView.GetHeaderView(0);
            TextView tv = v.FindViewById<TextView>(Resource.Id.lblNavHeader);
            tv.Text = Database.local.activeUser.user_nome;
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                try
                {
                    if (((ReservaFragment)currentFrag).layoutSwipeRefresh.Refreshing)
                        return;
                }
                catch (System.Exception)
                { }

                if (e.MenuItem.ItemId != Resource.Id.navItem_sair)
                    e.MenuItem.SetChecked(true);         
                      
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_reservas:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_hist:
                        ListItemClicked(1);
                        break;
                    case Resource.Id.nav_conta:
                        ListItemClicked(2);
                        break;                                    
                    case Resource.Id.navItem_sair:
                        ListItemClicked(3);
                        break;
                }

                drawerLayout.CloseDrawers();
            };
            
            if (savedInstanceState == null)
            {
               // LoadData();
                //SupportActionBar.Title = "Reservas Ativas";
                ListItemClicked(0);
                navigationView.SetCheckedItem(Resource.Id.nav_reservas);
            }
        }

        public override void OnBackPressed()
        {
            Android.Support.V7.App.AlertDialog.Builder diagBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            diagBuilder.SetTitle("Sair");
            diagBuilder.SetMessage("Tem certeza que deseja sair de sua conta?");
            diagBuilder.SetPositiveButton("Sim", (o, e) =>
            {
                Database.local.autoLogin = false;
                Database.SaveLocal(ApplicationContext);
                base.OnBackPressed();
                //Finish();
            });
            diagBuilder.SetNegativeButton("Não", (o, e) => { diagBuilder.Dispose(); });
            diagBuilder.Show();
            
        }

        private async void LoadData()
        {           
            if (await Database.IsConnectedAsync(this))
            {
                layoutPbar.Visibility = ViewStates.Visible;
                
                string user_id = Database.local.activeUser.user_id;
                var lstRes = await Database.GetReservasByUserIdAsync(user_id, (int)Enums.ReservaStatus.Ativa, this);
                if (lstRes != null)
                {
                    var lstHist = await Database.GetReservasByUserIdAsync(user_id, (int)Enums.ReservaStatus.Arquivada, this);
                    if (lstHist != null)
                    {
                        var lstAulas = await Database.GetAulasByUserIdAsync(user_id, this);
                        if (lstAulas != null)
                        {
                            var lstSalas = await Database.GetSalasByUserIdAsync(user_id, this);
                            if (lstSalas != null)
                            {
                                Database.local.reservasAtivas.Clear();
                                Database.local.reservasAtivas.AddRange(lstRes);
                                Database.local.reservasArquivadas.Clear();
                                Database.local.reservasArquivadas.AddRange(lstHist);
                                Database.local.aulas.Clear();
                                Database.local.aulas.AddRange(lstAulas);
                                Database.local.salasReservadas.Clear();
                                Database.local.salasReservadas.AddRange(lstSalas);

                                currentFrag = ReservaFragment.NewInstance(Enums.ReservaStatus.Ativa);
                                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, currentFrag).Commit();
                            }
                        }
                    }
                }

                layoutPbar.Visibility = ViewStates.Gone;
               
            }
            else
                Toast.MakeText(this, "Sem conexão com a internet", ToastLength.Short).Show();
            
        }

        private void ListItemClicked(int position)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (position == oldPosition)
                return;

            oldPosition = position;

            currentFrag = null;
            
            switch (position)
            {
                case 0:
                    SupportActionBar.Title = "Reservas Ativas";
                    currentFrag = ReservaFragment.NewInstance(Enums.ReservaStatus.Ativa);
                    break;
                case 1:
                    SupportActionBar.Title = "Histórico de Reservas";
                    currentFrag = ReservaFragment.NewInstance(Enums.ReservaStatus.Arquivada);
                    break;                    
                case 2:
                    SupportActionBar.Title = "Conta";
                    currentFrag = ContaFragment.NewInstance(drawerLayout);
                    break;
                case 3:
                    Android.Support.V7.App.AlertDialog.Builder diagBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    diagBuilder.SetTitle("Sair");
                    diagBuilder.SetMessage("Deseja realmente sair de sua conta?");
                    diagBuilder.SetNegativeButton("Não", (o, e) => { diagBuilder.Dispose(); });
                    diagBuilder.SetPositiveButton("Sim", (o, e) =>
                    {
                        Database.local.autoLogin = false;
                        Database.SaveLocal(ApplicationContext);

                        ISharedPreferences sharedPref = GetSharedPreferences("LOAD", FileCreationMode.Private);
                        ISharedPreferencesEditor editor = sharedPref.Edit();
                        editor.PutBoolean("First_Load", false);
                        editor.Commit();
                        
                        Finish();
                    });
                    
                    diagBuilder.Show();
                    oldPosition = -1;                   
                    break;
            }

            if (currentFrag != null)
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, currentFrag).Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
                //case Resource.Id.item1:
                //    //Snackbar.Make(drawerLayout, "", Snackbar.LengthLong).Show();
                //    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}