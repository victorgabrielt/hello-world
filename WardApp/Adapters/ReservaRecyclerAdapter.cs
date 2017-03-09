using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Support.V4.App;

namespace WardApp.Fragments
{
    public class ReservaRecyclerAdapter : RecyclerView.Adapter
    {       
        public List<ReservaTable> reservas;
        private Enums.ReservaStatus status;
        private FragmentManager fm;

        public ReservaRecyclerAdapter(List<ReservaTable> reservas, Enums.ReservaStatus status, FragmentManager fm)
        {
            this.reservas = reservas;
            this.status = status;
            this.fm = fm;
            
        }

        public override int ItemCount
        {
            get
            {
                return reservas.Count;
            }
        }

        public int GetItemPosition(ReservaTable res)
        {
            return reservas.IndexOf(res);
        }

        public override long GetItemId(int position)
        {
            throw new NotImplementedException();
            //return long.Parse(reservas[position].id);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ReservaViewHolder vh = (ReservaViewHolder)holder;

            ReservaTable reserva = reservas[position];
            AulaTable aula = Database.local.aulas.Find(x => x.aula_id == reserva.res_aula_id);
            SalaTable sala = Database.local.salasReservadas.Find(s => s.sala_nome == reserva.res_sala);
            
            //string dias = "Hoje!";
            int hoje = (int)DateTime.Today.DayOfWeek;
            if (status == Enums.ReservaStatus.Ativa)
            {
                int i = DateTime.Compare(DateTime.Today, reserva.res_data_final);
                vh.lblDataFinalTitle.Text = "Ativa até:";
                vh.layoutDataIni.Visibility = ViewStates.Gone;
                vh.lblDiasRest.Visibility = ViewStates.Visible;
                vh.lblDiasRest.Text = (reserva.res_data_final.Date - DateTime.Today.Date).TotalDays + " dia(s) restante(s)";
            }
            else
            {               
               // vh.lblTitle.Text = "Dia da semana: ";
                vh.layoutDataIni.Visibility = ViewStates.Visible;
                vh.lblDataIni.Text = reserva.res_data_ini.Date.ToShortDateString();
                vh.lblDataFinalTitle.Text = "Data final: ";
                vh.lblDiasRest.Visibility = ViewStates.Gone;
            }

            vh.lblAulaHorario.Text = AulaTable.GetHorario(aula.aula_horario, aula.aula_duracao);
            vh.lblDataFinal.Text = reserva.res_data_final.Date.ToString("dd/MM/yyyy");
            vh.lblSalaNum.Text = reserva.res_sala;
            vh.lblTurmaId.Text = aula.aula_turma;
            vh.lblDisc.Text = aula.aula_disciplina;
            vh.lblDiaSemana.Text = "(" + AulaTable.DiaSemana(aula.aula_diasemana) + ")";    

        }       

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_reserva, parent, false);
            ReservaViewHolder holder = new ReservaViewHolder(item, fm, reservas, this);
            return holder;
        }
    }

    internal class ReservaViewHolder : RecyclerView.ViewHolder
    {
        //public TextView lblTitle { get; private set; }
        //public TextView lblDataAula { get; private set; }
        public TextView lblDisc { get; private set; }
        public TextView lblTurmaId { get; private set; }
        
        public TextView lblDataFinal { get; private set; }
        public TextView lblDataFinalTitle { get; private set; }
        public TextView lblSalaNum { get; private set; }
        public TextView lblAulaHorario { get; private set; }
        public TextView lblDiaSemana { get; private set; }
        public LinearLayout layoutDataIni { get; private set; }
        public TextView lblDataIni { get; private set; }
        public TextView lblDiasRest { get; private set; }

        public ReservaViewHolder(View v, FragmentManager fm, List<ReservaTable> reservas, ReservaRecyclerAdapter adapter) : base (v)
        {
            lblDataFinalTitle = v.FindViewById<TextView>(Resource.Id.lblDataFinalTitle);
            lblDataIni = v.FindViewById<TextView>(Resource.Id.lblDataIni);
            layoutDataIni = v.FindViewById<LinearLayout>(Resource.Id.layoutDataIni);
            //lblTitle = v.FindViewById<TextView>(Resource.Id.lblTitle);
            //lblDataAula = v.FindViewById<TextView>(Resource.Id.lblDataAula);
            lblDisc = v.FindViewById<TextView>(Resource.Id.lblDisc);
            lblTurmaId = v.FindViewById<TextView>(Resource.Id.lblTurmaId);
            lblDataFinal = v.FindViewById<TextView>(Resource.Id.lblDataFinal);
            lblSalaNum = v.FindViewById<TextView>(Resource.Id.lblSalaNum);
            lblAulaHorario = v.FindViewById<TextView>(Resource.Id.lblAulaHorario);
            lblDiaSemana = v.FindViewById<TextView>(Resource.Id.lblDiaSemana);
            lblDiasRest = v.FindViewById<TextView>(Resource.Id.lblDiasRest);

            v.Click += (sender, e) =>
            {
                ReservaTable res = reservas[AdapterPosition];              
                ReservaDetailDialog diag = ReservaDetailDialog.newInstance(res, adapter);
                diag.Show(fm, "");
            };

        }
    }
}