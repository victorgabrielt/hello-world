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
using WardApp.Fragments;

namespace WardApp
{
    public class NrAulaListViewAdapter : BaseAdapter<AulaTable>
    {

        private List<AulaTable> aulas;
        private Context ctx;
        private NrDialog diag;
        private View selectedView;

        public NrAulaListViewAdapter(NrDialog diag,List<AulaTable> aulas, Context ctx)
        {
            this.diag = diag;
            this.aulas = new List<AulaTable>();
            this.aulas.AddRange(aulas);
            this.ctx = ctx;
        }


        public override AulaTable this[int position]
        {
            get
            {
                return aulas[position];
            }
        }

        public override int Count
        {
            get
            {
                return aulas.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return 0;
        }
       
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = LayoutInflater.From(ctx).Inflate(Resource.Layout.item_aula, null, false);
            TextView lblDisc = v.FindViewById<TextView>(Resource.Id.lblNrAulaDisc);
            TextView lblTurma = v.FindViewById<TextView>(Resource.Id.lblNrAulaTurma);
            TextView lblPeriodo = v.FindViewById<TextView>(Resource.Id.lblNrAulaPeriodo);
            TextView lblDiaSemana = v.FindViewById<TextView>(Resource.Id.lblNrAulaDiaSemana);
            TextView lblHorario = v.FindViewById<TextView>(Resource.Id.lblNrAulaHorario);
            LinearLayout layoutNrAula = v.FindViewById<LinearLayout>(Resource.Id.NrAulaLayout);

            v.Click += (sender, e) => 
            {

                if (selectedView != null && selectedView != v)
                {
                    selectedView.Selected = false;
                    selectedView = v;
                    selectedView.Selected = true;
                }
                else if (selectedView == null)
                {
                    selectedView = v;
                    selectedView.Selected = true;
                }
                else
                {
                    selectedView.Selected = false;
                    selectedView = null;
                    diag.nReserva.res_aula_id = "-1";
                    return;
                }

                diag.nReserva.res_aula_id = aulas[position].aula_id;
            };

            lblDisc.Text = aulas[position].aula_disciplina;
            lblTurma.Text = aulas[position].aula_turma;
            lblDiaSemana.Text = AulaTable.DiaSemana(aulas[position].aula_diasemana);

            lblHorario.Text = AulaTable.GetHorario(aulas[position].aula_horario, aulas[position].aula_duracao);

            return v;
        }
    }
}