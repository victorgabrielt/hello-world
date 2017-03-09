using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using WardApp.Dialogs;

namespace WardApp.Fragments
{
    public class ReservaFragment : Fragment
    {
        private RecyclerView rcyView;
        private RecyclerView.LayoutManager rcyViewLayoutManager;
        private ReservaRecyclerAdapter adapter;
        private Enums.ReservaStatus status;
        private FloatingActionButton fbtnAddReserva;
        private List<ReservaTable> reservas;
        private Button btnFiltrar;
        public SwipeRefreshLayout layoutSwipeRefresh;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            reservas = new List<ReservaTable>();
            if (status == Enums.ReservaStatus.Ativa)
            {
                reservas.AddRange(Database.local.reservasAtivas);
                adapter = new ReservaRecyclerAdapter(reservas, status, ChildFragmentManager);
            }
            else if (status == Enums.ReservaStatus.Arquivada)
            {
                reservas.AddRange(Database.local.reservasArquivadas);
                adapter = new ReservaRecyclerAdapter(reservas, status, ChildFragmentManager);
            }

        }
        public static ReservaFragment NewInstance(Enums.ReservaStatus status)
        {

            ReservaFragment frag = new ReservaFragment { Arguments = new Bundle() };
            frag.status = status;
            return frag;

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignored = base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.reservas, null);

            fbtnAddReserva = view.FindViewById<FloatingActionButton>(Resource.Id.fbtnAddReserva);
            btnFiltrar = view.FindViewById<Button>(Resource.Id.btnFiltro);
            rcyView = view.FindViewById<RecyclerView>(Resource.Id.reservasList);
            layoutSwipeRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.layoutSwipeRefresh);
            
            rcyView.SetAdapter(adapter);
            rcyViewLayoutManager = new LinearLayoutManager(Activity.ApplicationContext);
            rcyView.SetLayoutManager(rcyViewLayoutManager);

            if (status == Enums.ReservaStatus.Ativa)
                btnFiltrar.Visibility = ViewStates.Gone;
            else
                btnFiltrar.Visibility = ViewStates.Visible;


            fbtnAddReserva.Click += (sender, e) =>
                {
                    NrDialog rdiag = NrDialog.newInstance(this);
                    rdiag.Show(ChildFragmentManager, "");
                };

            btnFiltrar.Click += (sender, e) =>
            {
                FilterDialog diag = new FilterDialog(reservas, rcyView);
                diag.Show(ChildFragmentManager, "");
            };

            layoutSwipeRefresh.Refresh += (o, e) => { LoadData(); };

            if (status == Enums.ReservaStatus.Ativa)
                fbtnAddReserva.Visibility = ViewStates.Visible;

            else
                fbtnAddReserva.Visibility = ViewStates.Gone;

            ISharedPreferences sharedPref = Activity.GetSharedPreferences("LOAD", FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPref.Edit();

            if (sharedPref.GetBoolean("First_Load", true))
            {
                LoadData();
                editor.PutBoolean("First_Load", false);
                editor.Commit();
            }

            return view;
        }

        public async void LoadData()
        {
            if (!layoutSwipeRefresh.Refreshing)
                layoutSwipeRefresh.Post(() => { layoutSwipeRefresh.Refreshing = true; });
            if (await Database.IsConnectedAsync(Context))
            {              
                string user_id = Database.local.activeUser.user_id;
                var lstRes = await Database.GetReservasByUserIdAsync(user_id, (int)Enums.ReservaStatus.Ativa, Context);
                if (lstRes != null)
                {
                    var lstHist = await Database.GetReservasByUserIdAsync(user_id, (int)Enums.ReservaStatus.Arquivada, Context);
                    if (lstHist != null)
                    {
                        var lstAulas = await Database.GetAulasByUserIdAsync(user_id, Context);
                        if (lstAulas != null)
                        {
                            var lstSalas = await Database.GetSalasByUserIdAsync(user_id, Context);
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
                                Database.SaveLocal(Context);
                                reservas.Clear();
                                if (status == Enums.ReservaStatus.Ativa)
                                    reservas.AddRange(Database.local.reservasAtivas);
                                else
                                    reservas.AddRange(Database.local.reservasArquivadas);

                                rcyView.GetAdapter().NotifyDataSetChanged();
                                
                            }
                        }
                    }
                }

            }
            else
                Toast.MakeText(Context, "Sem conexão com a internet", ToastLength.Short).Show();

            layoutSwipeRefresh.Refreshing = false;
        }

        //private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        //{
        //    Spinner spinner = (Spinner)sender;
        //    switch (e.Position)
        //    {
                
        //        case 0:
        //            txtFilter.Visibility = ViewStates.Gone;
        //            reservas.Clear();
        //            reservas.AddRange(Database.local.reservasArquivadas.OrderBy(x => Math.Abs((x.res_data_final - DateTime.Today).Days)).ToList());
        //            rcyView.GetAdapter().NotifyDataSetChanged();
        //            break;
        //        case 1:
        //            txtFilter.Visibility = ViewStates.Gone;
        //            reservas.Clear();
        //            reservas.AddRange(Database.local.reservasArquivadas.OrderByDescending(x => Math.Abs((x.res_data_final - DateTime.Today).Days)).ToList());
        //            rcyView.GetAdapter().NotifyDataSetChanged();
        //            break;
        //        case 2:
        //            txtFilter.Visibility = ViewStates.Visible;
        //            txtFilter.Focusable = true;
        //            txtFilter.LongClickable = true;
        //            txtFilter.Text = "";
        //            txtFilter.Hint = "Digite o nome da sala";
                    
        //            break;
        //        case 3:
        //            txtFilter.Visibility = ViewStates.Visible;
        //            txtFilter.Hint = "Coloque a data de criação da reserva ";
        //            txtFilter.Focusable = false;
        //            txtFilter.Clickable = true;
        //            txtFilter.LongClickable = false;
        //            break;
        //        default:
        //            break;
        //    }

        //}
    }
}