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
using Android.Support.V4.App;

namespace WardApp.Fragments
{
    class nrRecursosDialog : DialogFragment
    {
        private ListView lstview;
        private Button btnFechar;
        private string sala_id;

        public nrRecursosDialog(string sala_id)
        {
            this.sala_id = sala_id;
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.SetTitle("Recursos");
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.MatchParent);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            View v = inflater.Inflate(Resource.Layout.dialog_nrRecursos, container, false);
            lstview = v.FindViewById<ListView>(Resource.Id.lstviewNrRecursos);
            btnFechar = v.FindViewById<Button>(Resource.Id.btnFecharRecDialog);
            List<RecursoTable> recs = Database.local.recursosDisponiveis.FindAll((x) => x.rec_sala_id == sala_id);
            RecursosListViewAdapter adapter = new RecursosListViewAdapter(recs, Context);
            lstview.Adapter = adapter;

            btnFechar.Click += (obj, e) =>
            {
                Dismiss();
            };
            return v;
        }
    }
}