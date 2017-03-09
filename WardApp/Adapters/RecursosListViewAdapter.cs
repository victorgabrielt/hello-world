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

namespace WardApp.Fragments
{
    class RecursosListViewAdapter : BaseAdapter<RecursoTable>
    {
        private List<RecursoTable> recursos;
        private Context ctx;

        public RecursosListViewAdapter(List<RecursoTable> recursos, Context ctx)
        {
            this.recursos = recursos;
            this.ctx = ctx;
        }

        public override RecursoTable this[int position]
        {
            get
            {
                return recursos[position];
            }
        }

        public override int Count
        {
            get
            {
                return recursos.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = LayoutInflater.From(ctx).Inflate(Resource.Layout.item_recurso, parent, false);
            TextView lblRecNome = v.FindViewById<TextView>(Resource.Id.lblRecNome);
            TextView lblRecQtd = v.FindViewById<TextView>(Resource.Id.lblRecQtd);
            TextView lblRecDesc = v.FindViewById<TextView>(Resource.Id.lblRecDesc);

            lblRecNome.Text = recursos[position].rec_nome;
            lblRecQtd.Text = "- " + recursos[position].rec_quantidade + " unidade(s)";
            lblRecDesc.Text = recursos[position].rec_descricao;

            return v;
        }
    }
}