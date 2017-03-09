using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Java.Lang;
using WardApp.Dialogs;

namespace WardApp.Fragments
{
    public class NrDialog : DialogFragment
    {
        private Button btnNext;
        public ReservaTable nReserva;
        private Button btnPrevious;
        private CustomViewPager vpager;
        private ReservaFragment reservaFrag;
        public NrDialog()
        { }

        public static NrDialog newInstance(ReservaFragment reservaFrag)
        {
            NrDialog diag = new NrDialog();
            diag.reservaFrag = reservaFrag;
            diag.nReserva = new ReservaTable();
            diag.nReserva.res_aula_id = "-1";
            diag.nReserva.res_sala = "-1";
            diag.nReserva.res_data_ini = DateTime.Today;
            diag.nReserva.res_data_final = DateTime.Today;
            diag.nReserva.res_criacao = DateTime.Today;
            diag.nReserva.res_status = 1;    
            return diag;
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
        }       

       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            View v = inflater.Inflate(Resource.Layout.new_reserva_dialog, container, false);
            vpager = v.FindViewById<CustomViewPager>(Resource.Id.newResViewPager);
            Button btnCancel = v.FindViewById<Button>(Resource.Id.btnCancel);
            btnPrevious = v.FindViewById<Button>(Resource.Id.btnPrevious);
            btnNext = v.FindViewById<Button>(Resource.Id.btnNext);
            NrViewPagerAdapter adapter = new NrViewPagerAdapter(ChildFragmentManager, this);

            Android.Graphics.Color c;
            if (Database.local.appTheme == (int)Enums.AppTheme.DarkTheme)
                c = Android.Graphics.Color.White;
            
            else
                c = Android.Graphics.Color.Black;

            
            btnNext.GetCompoundDrawables()[2].SetColorFilter(c , Android.Graphics.PorterDuff.Mode.Multiply);
            btnPrevious.GetCompoundDrawables()[0].SetColorFilter(c, Android.Graphics.PorterDuff.Mode.Multiply);

            btnCancel.Click += (sender, e) => { Dismiss(); };
            btnNext.Click += (sender, e) =>
            {
                int pageNum = vpager.CurrentItem + 1;
                if (pageNum >= 3)
                {
                    if (nReserva.res_sala == "-1")
                    {
                        Toast.MakeText(Context, "Sala não selecionada", ToastLength.Short).Show();
                        return;
                    }
                    AddReserva();                           
                    
                }
                else
                    ChangePage(pageNum);

            };
            btnPrevious.Click += (sender, e) =>
            {
                int pageNum = vpager.CurrentItem - 1;
                ChangePage(pageNum);

            };

            ChangePage(0);
            vpager.PageSwipe = false;
            vpager.Adapter = adapter;
            return v;
        }

        private async void AddReserva()
        {
            bool reservaAdded = await Database.PostNewReserva(nReserva, Context);
            if (reservaAdded)
            {
                reservaFrag.LoadData();
                Toast.MakeText(Context, "Reserva Criada!", ToastLength.Short).Show();
                Dismiss();
            }
            else
                Toast.MakeText(Context, "Não foi possivel criar a reserva", ToastLength.Short).Show();
        }

        private async void LoadSalas()
        {
            NrPage pg = ((NrViewPagerAdapter)vpager.Adapter).GetPage(2);
            LoadingDialog diag = new LoadingDialog("Carregando salas");
            diag.Show(ChildFragmentManager, "");
            List<SalaTable> s = await Database.GetSalasByAulaIdAsync(nReserva.res_aula_id, nReserva.res_data_ini.Date.ToString("yyyy-MM-dd"), nReserva.res_data_final.Date.ToString("yyyy-MM-dd"), Context);
            if (s != null)
            {
                Database.local.salasDisponiveis.Clear();
                Database.local.salasDisponiveis.AddRange(s);                
                pg.RefreshRecycler();
            }
            diag.Dismiss();
        }

        private bool ChangePage(int p)
        {
            switch (p)
            {
                case 0:
                    //lblnrTitle.Text = "Nova Reserva - Selecione o periodo";
                    //Dialog.SetTitle("Selecione o periodo");
                    btnPrevious.Visibility = ViewStates.Invisible;
                    nReserva.res_aula_id = "-1";
                    nReserva.res_sala = "-1";
                    break;
                case 1:
                    
                    nReserva.res_data_ini = ((NrViewPagerAdapter)vpager.Adapter).GetPage(0).dtpInicio.DateTime.Date;
                    nReserva.res_data_final = ((NrViewPagerAdapter)vpager.Adapter).GetPage(0).dtpFinal.DateTime.Date;
                    if (nReserva.res_data_ini < DateTime.Today || nReserva.res_data_final < DateTime.Today || nReserva.res_data_ini > nReserva.res_data_final)
                    {
                        Toast.MakeText(Context, "Período inválido", ToastLength.Short).Show();
                        return false;
                    }
                    //lblnrTitle.Text = "Nova Reserva - Selecione a aula";
                    //Dialog.SetTitle("Selecione a aula");
                    btnNext.Text = "Próximo";
                    btnPrevious.Visibility = ViewStates.Visible;
                    nReserva.res_sala = "-1";
                    break;
                case 2:
                    if (nReserva.res_aula_id == "-1")
                    {
                        Toast.MakeText(Context, "Aula não selecionada", ToastLength.Short).Show();
                        return false;
                    }
                    LoadSalas();
                    //lblnrTitle.Text = "Nova Reserva - Selecione a sala";
                    //Dialog.SetTitle("Selecione a sala");                   
                    btnNext.Text = "Concluir";
                    break;
                default:
                    return false;
            }

            vpager.SetCurrentItem(p, true);
            return true;
        }     

    }

    public class NrViewPagerAdapter : FragmentPagerAdapter
    {
        private const int PAGE_COUNT = 3;
        private NrDialog diag;
        private NrPage[] page = new NrPage[3];
        private FragmentManager fm;

        public NrViewPagerAdapter(FragmentManager fm, NrDialog diag) : base(fm)
        {
            this.diag = diag;
            this.fm = fm;
        }

        public override int Count
        {          
            get
            {
                return PAGE_COUNT;
            }
        }

        public override Fragment GetItem(int position)
        {
            page[position] = new NrPage(position, diag, fm);
            return page[position];
        }

        public NrPage GetPage(int position)
        {
            return page[position];
        }
    }

    public class NrPage : Fragment
    {
        private List<SalaTable> salas = new List<SalaTable>();
        public int page_num;
        private NrDialog diag;
        private RecyclerView rcyView;
        internal DatePicker dtpInicio;
        internal DatePicker dtpFinal;
        private FragmentManager fm;
        //internal ProgressBar pbar;

        public NrPage(int page, NrDialog diag, FragmentManager fm)
        {
            page_num = page;
            this.diag = diag;
            this.fm = fm;
        }

        public void RefreshRecycler()
        {
            salas.Clear();
            salas.AddRange(Database.local.salasDisponiveis);
            rcyView.GetAdapter().NotifyDataSetChanged();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v;

            if (page_num == 0)
            {
                v = inflater.Inflate(Resource.Layout.new_reserva_page1, container, false);
                dtpInicio = v.FindViewById<DatePicker>(Resource.Id.dtpInicio);
                dtpFinal = v.FindViewById<DatePicker>(Resource.Id.dtpFinal);
            }

            else if (page_num == 1)
            {
                v = inflater.Inflate(Resource.Layout.new_reserva_page2, container, false);
                ListView lstview = v.FindViewById<ListView>(Resource.Id.lstAulas);
                lstview.Adapter = new NrAulaListViewAdapter(diag, Database.local.aulas, Context);
            }

            else
            {
                v = inflater.Inflate(Resource.Layout.new_reserva_page3, container, false);
                Button btnFiltro = v.FindViewById<Button>(Resource.Id.btnNrSalasFiltro);
                rcyView = v.FindViewById<RecyclerView>(Resource.Id.recyclerSalas);

                NrSalasRecycleAdapter adapter = new NrSalasRecycleAdapter(salas, diag, fm);
                RecyclerView.LayoutManager rcyLayoutManager = new LinearLayoutManager(Activity.ApplicationContext);
                rcyView.SetAdapter(adapter);
                rcyView.SetLayoutManager(rcyLayoutManager);

                btnFiltro.Click += (obj, e) =>
                {
                    FilterDialog fdiag = new FilterDialog(salas, rcyView);
                    fdiag.Show(ChildFragmentManager, "");
                };
                
            }
            return v;
        }
    }
}