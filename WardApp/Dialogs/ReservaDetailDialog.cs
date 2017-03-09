using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

namespace WardApp.Fragments
{
    public class ReservaDetailDialog : DialogFragment
    {
        private ViewPager vpager;
        private ReservaViewerPageAdapter adapter;
        private ReservaTable reserva;
        private SalaTable sala;
        private AulaTable aula;
        private ReservaRecyclerAdapter resAdapter;

        public  ReservaDetailDialog()
        { }

        public static ReservaDetailDialog newInstance(ReservaTable res, ReservaRecyclerAdapter resAdapter)
        {
            ReservaDetailDialog f = new ReservaDetailDialog();
            f.reserva = res;
            f.aula = Database.local.aulas.Find(x => x.aula_id == res.res_aula_id);
            f.sala = Database.local.salasReservadas.Find(s => s.sala_nome == res.res_sala);
            f.resAdapter = resAdapter;
            return f;
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.reserva_dialog, container, false);
            vpager = v.FindViewById<ViewPager>(Resource.Id.viewPager);
            adapter = new ReservaViewerPageAdapter(ChildFragmentManager, reserva, sala, aula);           
            Button btnResCancel = v.FindViewById<Button>(Resource.Id.btnResCancel);
            Button btnFechar = v.FindViewById<Button>(Resource.Id.btnFechar);

            vpager.Adapter = adapter;

            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            if (reserva.res_status != (int)Enums.ReservaStatus.Ativa)
                btnResCancel.Visibility = ViewStates.Gone;

            btnResCancel.Click += (o, e) => 
            {
                Android.Support.V7.App.AlertDialog.Builder diagBuilder = new Android.Support.V7.App.AlertDialog.Builder(Context);
                diagBuilder.SetTitle("Confirmação de Cancelamento");
                diagBuilder.SetMessage("Deseja realmente cancelar a reserva da sala " + sala.sala_nome + "?");
                diagBuilder.SetNegativeButton("Não", (obj, args) => { diagBuilder.Dispose(); });
                diagBuilder.SetPositiveButton("Sim", async (obj, args) =>
                {
                    btnResCancel.Activated = false;
                    diagBuilder.Dispose();
                    
                    if (await Database.UpdtResStatusByUserIdAsync((int)Enums.ReservaStatus.Arquivada, reserva.res_id, Context))
                    {
                        reserva.res_status = (int)Enums.ReservaStatus.Arquivada;
                        Database.local.reservasArquivadas.Add(reserva);
                        Database.SaveLocal(Context);                      
                        resAdapter.reservas.Remove(reserva);
                        resAdapter.NotifyItemRemoved(resAdapter.GetItemPosition(reserva));
                        Toast.MakeText(Context, "Reserva Cancelada!", ToastLength.Short).Show();
                        Dismiss();
                    }
                    else
                        Toast.MakeText(Context, "Não foi possivel cancelar a reserva", ToastLength.Short).Show();                                          
                });

                diagBuilder.Show();
            };
            btnFechar.Click += (o, e) => { Dismiss(); };
            
            return v;
        }
    }

    internal class ReservaViewerPageAdapter : FragmentPagerAdapter
    {
        const int PAGE_COUNT = 2;
        private ReservaTable reserva;
        private SalaTable sala;
        private AulaTable aula;

        public ReservaViewerPageAdapter(FragmentManager fm, ReservaTable res, SalaTable sala, AulaTable aula) : base(fm)
        {
            reserva = res;
            this.sala = sala;
            this.aula = aula;
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
            if (position == 0)
                return ReservaPage.NewInstance(position, reserva, sala, aula);
            else
                return new RecursoPage(sala.sala_id);
            
            
        }
       
    }


    internal class ReservaPage : Fragment
    {
        private int page;
        private ReservaTable reserva;
        private SalaTable sala;
        private AulaTable aula;

        public static ReservaPage NewInstance(int pos, ReservaTable res, SalaTable sala, AulaTable aula)
        {           
            var args = new Bundle();
            args.PutInt("Page", pos);
            ReservaPage pf = new ReservaPage();
            pf.Arguments = args;
            pf.reserva = res;
            pf.sala = sala;
            pf.aula = aula;
            return pf;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            page = Arguments.GetInt("Page");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.dialog_resDetail_page1, container, false);
            LinearLayout layoutResRecursos = v.FindViewById<LinearLayout>(Resource.Id.layoutResRecursos);
            LinearLayout layoutResInfo = v.FindViewById<LinearLayout>(Resource.Id.layoutResInfo);
            TextView lblResStatus = v.FindViewById<TextView>(Resource.Id.lblStatus);
            TextView lblResCriacao = v.FindViewById<TextView>(Resource.Id.lblDtCriacao);
            TextView lblResDataInicial = v.FindViewById<TextView>(Resource.Id.lblDtinicial);
            TextView lblResDataFinal = v.FindViewById<TextView>(Resource.Id.lblDtFinal);
            TextView lblSalaNome = v.FindViewById<TextView>(Resource.Id.lblSalaNome);
            TextView lblSalaNum = v.FindViewById<TextView>(Resource.Id.lblSalaNum);
            TextView lblSalaPredio = v.FindViewById<TextView>(Resource.Id.lblSalaPredio);
            TextView lblSalaAndar = v.FindViewById<TextView>(Resource.Id.lblSalaAndar);
            TextView lblAulaDisc = v.FindViewById<TextView>(Resource.Id.lblAulaDisc);
            TextView lblAulaTurma = v.FindViewById<TextView>(Resource.Id.lblAulaTurma);
            TextView lblAulaCurso = v.FindViewById<TextView>(Resource.Id.lblAulaCurso);
            TextView lblAulaHorario = v.FindViewById<TextView>(Resource.Id.lblAulaHorario);
            //TextView lblAulaDuracao = v.FindViewById<TextView>(Resource.Id.lblAulaDuracao);

            if (reserva.res_status == 1)
                lblResStatus.Text = "Ativo";
            
            else if (reserva.res_status == 0)
                lblResStatus.Text = "Cancelado";
            else
                lblResStatus.Text = "Arquivado";

            lblResCriacao.Text = reserva.res_criacao.Date.ToString(@"dd/MM/yy");
            lblResDataInicial.Text = reserva.res_data_ini.Date.ToString(@"dd/MM/yy");
            lblResDataFinal.Text = reserva.res_data_final.Date.ToString(@"dd/MM/yy");
            lblSalaNome.Text = sala.sala_nome;
            lblSalaNum.Text = sala.sala_numero;
            lblSalaPredio.Text = sala.sala_predio;
            lblSalaAndar.Text = sala.sala_andar > 0 ? sala.sala_andar +"º andar" : "Térreo";
            lblAulaDisc.Text = aula.aula_disciplina;
            lblAulaTurma.Text = aula.aula_turma;
            lblAulaCurso.Text = aula.aula_curso;

            //TimeSpan noite = new TimeSpan(0, 0, 0);

            //if (aula.aula_horario > 12)
            //    noite = new TimeSpan(0, 30, 0);

            //TimeSpan inicio = new TimeSpan(7, 30, 0);
            //TimeSpan duracao = new TimeSpan(0, 55, 0);
            //inicio = inicio + TimeSpan.FromTicks(duracao.Ticks * (aula.aula_horario - 1)) + noite;
            //TimeSpan tst = TimeSpan.FromTicks(duracao.Ticks * aula.aula_duracao);
            //TimeSpan final = inicio + TimeSpan.FromTicks(duracao.Ticks * aula.aula_duracao);
            lblAulaHorario.Text = AulaTable.GetHorario(aula.aula_horario, aula.aula_duracao);


            if (page == 0)
            {
                layoutResInfo.Visibility = ViewStates.Visible;
                layoutResRecursos.Visibility = ViewStates.Gone;
            }
            else
            {
                layoutResInfo.Visibility = ViewStates.Gone;
                layoutResRecursos.Visibility = ViewStates.Visible;
            }
            

            return v;
        }
    }

    internal class RecursoPage : Fragment
    {
        private ListView lstviewRec;
        private string sala_id;

        internal RecursoPage(string sala_id)
        {
            this.sala_id = sala_id;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.dialog_resDetail_page2, container, false);
            lstviewRec = v.FindViewById<ListView>(Resource.Id.lstviewRecursos);
            List<RecursoTable> recs = Database.local.recursosReservados.FindAll((x) => x.rec_sala_id == sala_id);
            RecursosListViewAdapter adapter = new RecursosListViewAdapter(recs, Context);
            lstviewRec.Adapter = adapter;

            return v;
        }
    }
}