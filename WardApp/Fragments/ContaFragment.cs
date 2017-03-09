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
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Content.Res;

namespace WardApp.Fragments
{
    public class ContaFragment : Fragment
    {
        private View parentView;
        private Button btnSalvar;
        private EditText txtNewPass;
        private EditText txtConfNewPass;
        private EditText txtCurrentPass;
        private CheckBox cboxLogin;
        private RadioGroup rgroupTema;
        private TextView lblNomeUser;
        private TextView lblUserEmail;
        private ProgressBar pbar;
        private View rootView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);          
        }

        public static ContaFragment NewInstance(View parentView)
        {
            ContaFragment frag = new ContaFragment { Arguments = new Bundle() };
            frag.parentView = parentView;
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View v = inflater.Inflate(Resource.Layout.conta, null);
            btnSalvar = v.FindViewById<Button>(Resource.Id.btnSalvar);
            txtNewPass = v.FindViewById<EditText>(Resource.Id.txtNewPass);
            txtConfNewPass = v.FindViewById<EditText>(Resource.Id.txtConfNewPass);
            txtCurrentPass = v.FindViewById<EditText>(Resource.Id.txtCurrentPass);
            cboxLogin = v.FindViewById<CheckBox>(Resource.Id.cboxLogin);
            lblNomeUser = v.FindViewById<TextView>(Resource.Id.lblNomeUser);
            lblUserEmail = v.FindViewById<TextView>(Resource.Id.lblUserEmail);
            pbar = v.FindViewById<ProgressBar>(Resource.Id.contaPBar);
            rgroupTema = v.FindViewById<RadioGroup>(Resource.Id.rgroupTema);
            rootView = v.RootView;

            lblNomeUser.Text = Database.local.activeUser.user_nome;
            lblUserEmail.Text = Database.local.activeUser.user_email;
            rgroupTema.Check(Database.local.appTheme == 0 ? Resource.Id.rbtnClaro : Resource.Id.rbtnEscuro);
            cboxLogin.Checked = Database.local.autoLogin;

            btnSalvar.Click += delegate { btnSalvar_Click(); };
            return v;
        }

        private async void btnSalvar_Click()
        {
            if (txtNewPass.Text.Trim() != "")
            {
                pbar.Visibility = ViewStates.Visible;
                btnSalvar.Enabled = false;
                if (await Database.IsConnectedAsync(Context))
                {
                    UsuarioTable user = new UsuarioTable();
                    user.user_id = Database.local.activeUser.user_id;
                    user.user_senha = Database.Encrypt(txtCurrentPass.Text.Trim());
                    if (await Database.AuthenticateUserAsync(user, Context) == 0)
                    {
                        if (txtNewPass.Text.Trim() == txtConfNewPass.Text.Trim())
                        {
                            if (!await Database.UpdtUserSenhaByUserIdAsync(Database.Encrypt(txtNewPass.Text.Trim()), Database.local.activeUser.user_id, Context))
                            {
                                Toast.MakeText(Context, "Não foi possivel atualizar a senha no servidor", ToastLength.Short).Show();
                                txtNewPass.Text = "";
                                txtConfNewPass.Text = "";
                                pbar.Visibility = ViewStates.Gone;
                                btnSalvar.Enabled = true;
                                return;
                            }
                        }
                        else
                        {
                            Toast.MakeText(Context, "Senhas não coincidem", ToastLength.Short).Show();
                            pbar.Visibility = ViewStates.Gone;
                            btnSalvar.Enabled = true;
                            return;
                        }
                    }
                    else
                    {
                        Toast.MakeText(Context, "Senha atual incorreta", ToastLength.Short).Show();
                        pbar.Visibility = ViewStates.Gone;
                        btnSalvar.Enabled = true;
                        return;
                    }
                }
                else
                {
                    Toast.MakeText(Context, "Sem conexão com a internet", ToastLength.Short).Show();
                    pbar.Visibility = ViewStates.Gone;
                    btnSalvar.Enabled = true;
                    return;
                }
                Toast.MakeText(Context, "Senha atualizada!", ToastLength.Short).Show();
            }


            int theme = rgroupTema.CheckedRadioButtonId == Resource.Id.rbtnEscuro ? 1 : 0;

            if (Database.local.autoLogin != cboxLogin.Checked)
            {
                Database.local.autoLogin = cboxLogin.Checked;
            }

            if (theme != Database.local.appTheme)
            {
                Database.local.appTheme = theme;
                
                Activity.Recreate();
            }

            Database.SaveLocal(Activity.ApplicationContext);
            txtConfNewPass.Text = "";
            txtNewPass.Text = "";

            pbar.Visibility = ViewStates.Gone;
            btnSalvar.Enabled = true;
            
        }

    }
}