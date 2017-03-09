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
using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace WardApp.Dialogs
{
    public class FilterDialog : DialogFragment
    {
        private List<SalaTable> salas;
        private List<ReservaTable> reservas;
        private RecyclerView rcyView;
        private static List<SalaTable> allSalas = new List<SalaTable>();
        private List<string> recs;
        private ArrayAdapter<string> adpt;
        private LinearLayout lnLayoutRecursos;
        private int spinNum = 1;

        public FilterDialog(List<SalaTable> salas, RecyclerView rcyView)
        {
            this.salas = salas;
            allSalas.Clear();
            allSalas.AddRange(Database.local.salasDisponiveis);                      
            this.rcyView = rcyView;
        }
        public FilterDialog(List<ReservaTable> reservas, RecyclerView rcyView)
        {
            this.reservas = reservas;
            this.rcyView = rcyView;
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v;
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetStyle(DialogFragment.StyleNoTitle, Resource.Style.Theme2Light);
            if (salas != null)
            {
               
                v = inflater.Inflate(Resource.Layout.dialog_filtro_salas, container, false);
                Button btnAplicar = v.FindViewById<Button>(Resource.Id.btnFilterSalasAplicar);
                EditText txtNomeSala = v.FindViewById<EditText>(Resource.Id.txtFilterNrSalaNome);
                EditText txtPredio = v.FindViewById<EditText>(Resource.Id.txtFilterNrPredio);
                EditText txtCapacidade = v.FindViewById<EditText>(Resource.Id.txtFilterNrCapacidade);
                EditText txtRecQtd = v.FindViewById<EditText>(Resource.Id.txtFilterNrRecQtd);
                Spinner spin = v.FindViewById<Spinner>(Resource.Id.spinFilterNrRecursos);
                lnLayoutRecursos = v.FindViewById<LinearLayout>(Resource.Id.lnLayoutRecursos);

                var lst = Database.local.recursosDisponiveis;
                List<string> recs = new List<string>();
                recs.Add("");

                for (int i = 0; i < lst.Count; i++)
                    if (!recs.Contains(lst[i].rec_nome))
                        recs.Add(lst[i].rec_nome);
                    
                
                adpt = new ArrayAdapter<string>(Context, Resource.Layout.spinner_item, recs.ToArray());
                adpt.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spin.Adapter = adpt;

                btnAplicar.Click += (obj, e) =>
                {
                    if (txtNomeSala.Text.Trim() == "")
                    {
                        salas.Clear();
                        salas.AddRange(allSalas);
                    }
                    else
                    {
                        List<SalaTable> _salas = allSalas.FindAll(x => x.sala_nome.Contains(txtNomeSala.Text.Trim()));
                        salas.Clear();
                        salas.AddRange(_salas);
                    }

                    if (txtPredio.Text.Trim() != "")
                    {
                        List<SalaTable> _salas = salas.FindAll(x => x.sala_predio.Contains(txtPredio.Text.Trim()));
                        salas.Clear();
                        salas.AddRange(_salas);
                    }

                    if (txtCapacidade.Text.Trim() != "")
                    {
                        List<SalaTable> _salas = salas.FindAll(x => x.sala_capacidade == int.Parse(txtCapacidade.Text.Trim()));
                        salas.Clear();
                        salas.AddRange(_salas);
                    }

                    List<RecursoTable> frecs = new List<RecursoTable>();

                    for (int i = 0; i < lnLayoutRecursos.ChildCount; i++)
                    {
                        LinearLayout layout = (LinearLayout)lnLayoutRecursos.GetChildAt(i);
                        string rNome = ((Spinner)layout.GetChildAt(0)).SelectedItem.ToString();
                        int rQtd = int.Parse(((EditText)layout.GetChildAt(1)).Text.Trim());
                        if (rNome != "" && rQtd > 0)
                        {
                            RecursoTable r = new RecursoTable();
                            r.rec_sala_id = Database.local.recursosDisponiveis.Find(rc => rc.rec_nome == rNome && int.Parse(rc.rec_quantidade) >= rQtd).rec_sala_id;
                            r.rec_nome = rNome;
                            r.rec_quantidade = rQtd.ToString();
                            frecs.Add(r);
                        }

                    }

                    if (frecs.Count > 0)
                    {
                        List<SalaTable> _salas = frecs.Join(salas, r => r.rec_sala_id, s => s.sala_id, (r, s) => s).ToList();
                        salas.Clear();
                        salas.AddRange(_salas);
                    }

                    rcyView.GetAdapter().NotifyDataSetChanged();
                    Dismiss();
                };

                spin.ItemSelected += (o, ev) =>
                {                   
                    if (lnLayoutRecursos.ChildCount == int.Parse(spin.Tag.ToString()) && ev.Position > 0)
                    {
                        ItemSelect();
                    }
                    
                };
            }
            else
            {
                v = inflater.Inflate(Resource.Layout.dialog_filtro_reserva, container, false);

                RadioGroup rgroup = v.FindViewById<RadioGroup>(Resource.Id.rgroupFilterRes);
                EditText txtNomeSala = v.FindViewById<EditText>(Resource.Id.txtFilterNomeSala);
                TextView lblDataCriacao = v.FindViewById<TextView>(Resource.Id.lblFilterDataCriacao);
                Button btnAplicar = v.FindViewById<Button>(Resource.Id.btnFilterResAplicar);

                lblDataCriacao.Click += (obj, e) =>
                {
                    Android.App.Dialog dg = new Android.App.Dialog(Context);
                    dg.RequestWindowFeature((int)WindowFeatures.NoTitle);
                    dg.SetContentView(Resource.Layout.dialog_datepicker);
                    Button btnOk = dg.FindViewById<Button>(Resource.Id.btnDialogDtpOk);
                    DatePicker dtpicker = dg.FindViewById<DatePicker>(Resource.Id.dtpFilterReserva);
                    btnOk.Click += (o, ev) =>
                    {
                        lblDataCriacao.Text = dtpicker.DateTime.Date.ToString("dd/MM/yyyy");
                        dg.Dismiss();
                    };
                    dg.Show();
                    
                };

                btnAplicar.Click += (obj, e) =>
                {
                    //Pega as reservas pelo nome
                    List<ReservaTable> rlst = Database.local.reservasArquivadas.Join(Database.local.salasReservadas.Where(s => s.sala_nome.Contains(txtNomeSala.Text)), r => r.res_sala, s => s.sala_nome, (r, s) => r).ToList();
                    reservas.Clear();
                    reservas.AddRange(rlst);
                    if (lblDataCriacao.Text.Trim() != "" && lblDataCriacao.Text.Trim() != "Clique para configurar a data")
                    {
                        List<ReservaTable> lst = reservas.Where(r => r.res_criacao.Equals(DateTime.ParseExact(lblDataCriacao.Text.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))).ToList();
                        reservas.Clear();
                        reservas.AddRange(lst);
                    }


                    if (rgroup.CheckedRadioButtonId == Resource.Id.rbtnFilterRecentes)
                    {
                        
                        List<ReservaTable> res = reservas.OrderBy(x => Math.Abs((x.res_data_final - DateTime.Today).Days)).ToList();
                        reservas.Clear();
                        reservas.AddRange(res);
                    }
                    else
                    {
                        
                        List<ReservaTable> res = reservas.OrderByDescending(x => Math.Abs((x.res_criacao - DateTime.Today).Days)).ToList();
                        reservas.Clear();
                        reservas.AddRange(res);
                    }

                    
                    rcyView.GetAdapter().NotifyDataSetChanged();
                    Dismiss();
                };
            }
            return v;
        }

        private void ItemSelect()
        {
            LayoutInflater.From(Context).Inflate(Resource.Layout.item_filter_recurso, lnLayoutRecursos, true);
            Spinner spinner = (Spinner)((LinearLayout)lnLayoutRecursos.GetChildAt(spinNum)).GetChildAt(0);
            spinner.Tag = ++spinNum;
            spinner.Adapter = adpt;
            spinner.ItemSelected += (o, ev) =>
            {
                if (lnLayoutRecursos.ChildCount == int.Parse(spinner.Tag.ToString()) && ev.Position > 0)
                {
                    ItemSelect();
                }
            };
           
        }
    }
}