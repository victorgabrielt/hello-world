using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.App;

namespace WardApp.Fragments
{
    public class NrSalasRecycleAdapter : RecyclerView.Adapter
    {
        internal List<SalaTable> salas;
        internal View selectedView;
        internal NrDialog nrDiag;
        private FragmentManager fm;

        public NrSalasRecycleAdapter(List<SalaTable> salas, NrDialog nrDiag, FragmentManager fm)
        {
            this.salas = salas;
            this.nrDiag = nrDiag;
            this.fm = fm;
        }

        public override int ItemCount
        {
            get
            {
                return salas.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SalaViewHolder vh = (SalaViewHolder)holder;
            vh.lblSalaNome.Text = salas[position].sala_nome;
            vh.lblSalaNum.Text = salas[position].sala_numero;
            vh.lblSalaPredio.Text = salas[position].sala_predio;
            vh.lblSalaAndar.Text = salas[position].sala_andar > 0 ? salas[position].sala_andar.ToString() + "º andar" : "Térreo";
            vh.lblSalaCapacidade.Text = salas[position].sala_capacidade.ToString() + " pessoas";
            vh.sala_id = salas[position].sala_id;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_sala, parent, false);
            SalaViewHolder vh = new SalaViewHolder(v, this, fm);
            return vh;
        }
    }
    internal class SalaViewHolder : RecyclerView.ViewHolder
    {
        internal TextView lblSalaNome;
        internal TextView lblSalaNum;
        internal TextView lblSalaPredio;
        internal TextView lblSalaAndar;
        internal TextView lblSalaCapacidade;
        internal Button btnShowRecursos;
        //internal TextView lblDataFinal;
        internal string sala_id;

        internal SalaViewHolder(View v, NrSalasRecycleAdapter adpt, FragmentManager fm) : base(v)
        {
            lblSalaNome = v.FindViewById<TextView>(Resource.Id.lblnrSalaNome);
            lblSalaNum = v.FindViewById<TextView>(Resource.Id.lblnrSalaNum);
            lblSalaPredio = v.FindViewById<TextView>(Resource.Id.lblnrSalaPredio);
            lblSalaAndar = v.FindViewById<TextView>(Resource.Id.lblnrSalaAndar);
            lblSalaCapacidade = v.FindViewById<TextView>(Resource.Id.lblnrSalaCapacidade);
            btnShowRecursos = v.FindViewById<Button>(Resource.Id.btnShowRecursos);

            btnShowRecursos.Click += (obj, e) =>
            {
                nrRecursosDialog diag = new nrRecursosDialog(sala_id);
                diag.Show(fm, "");
            };

            v.Click += (obj, e) =>
            {
                if (adpt.selectedView != null && adpt.selectedView != v)
                {
                    adpt.selectedView.Selected = false;
                    adpt.selectedView = v;
                    adpt.selectedView.Selected = true;
                }
                else if (adpt.selectedView == null)
                {
                    adpt.selectedView = v;
                    adpt.selectedView.Selected = true;
                }
                else
                {
                    adpt.selectedView.Selected = false;
                    adpt.selectedView = null;
                    adpt.nrDiag.nReserva.res_sala = "-1";
                    return;
                }

                adpt.nrDiag.nReserva.res_sala = adpt.salas[AdapterPosition].sala_id;
            };

        }
    }
}