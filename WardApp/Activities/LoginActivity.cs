using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Java.Interop;
using Android.Support.Design.Widget;
using Android.Graphics;
using Android.Support.V4.Content;
using WardApp.Dialogs;
using Android.Views;

namespace WardApp
{
    [Activity(Label = "WardApp", MainLauncher = false ,Icon = "@drawable/icon")]
    public class LoginActivity : BaseActivity
    {
        private EditText txtUser, txtPass;
        private CheckBox cboxAutoLogin;
        private LinearLayout rootView;
        private Button btnLogin;
        private ProgressBar pbar;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.login;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            
            Database.LoadLocal(this);
            base.OnCreate(bundle);
            
            SupportActionBar.Title = "Ward App - Login";
                       
            rootView = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            cboxAutoLogin = FindViewById<CheckBox>(Resource.Id.cboxAutoLogin);
            txtPass = FindViewById<EditText>(Resource.Id.txtpass);
            txtUser = FindViewById<EditText>(Resource.Id.txtuser);
            pbar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            TextView lblForgotPass = FindViewById<TextView>(Resource.Id.lblForgotPass);


            lblForgotPass.Click += (obj, e) =>
            {
                Dialog dg = new Dialog(this);
                dg.RequestWindowFeature((int)WindowFeatures.NoTitle);
                dg.SetContentView(Resource.Layout.dialog_resetPass);
                //dg.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                Button btnEnviar = dg.FindViewById<Button>(Resource.Id.btnResetPass);
                EditText txtResetPassEmail = dg.FindViewById<EditText>(Resource.Id.txtResetPassEmail);
                btnEnviar.Click += async (o, ev) =>
                {
                    UsuarioTable user = new UsuarioTable();
                    user.user_email = txtResetPassEmail.Text.Trim();
                    dg.Dismiss();
                    if (await Database.PostResetPass(user,this))
                        Toast.MakeText(this, "Email enviado!", ToastLength.Short).Show();
                    
                    else
                        Toast.MakeText(this, "Email inválido!", ToastLength.Short).Show();

                   // dg.Dismiss();
                };
                dg.Show();
                
            };
            btnLogin.Click += delegate { btnLogin_Click(); };


            ISharedPreferences sharedPref = GetSharedPreferences("LOAD",FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPref.Edit();
            editor.PutBoolean("First_Load", true);
            editor.Commit();

            if (Database.local.autoLogin)
            {
                Intent i = new Intent(this, typeof(MainActivity));
                StartActivity(i);
            }
           
        }
      
        [Export("btnLogin_Click")]
        public async void btnLogin_Click()
        {
            btnLogin.Enabled = false;
            txtPass.Enabled = false;
            txtUser.Enabled = false;

            LoadingDialog diag = new LoadingDialog("Autenticando");
            diag.Show(SupportFragmentManager,"");
            //pbar.Visibility = Android.Views.ViewStates.Visible;

            if (! await Database.IsConnectedAsync(this))
            {
                Toast.MakeText(this, "Sem conexão com a internet", ToastLength.Short);
                ResetFields();
                return;
            }           
            UsuarioTable user = new UsuarioTable();
            
            int auth = -1;

            user.user_id = txtUser.Text.Trim();
            user.user_senha = Database.Encrypt(txtPass.Text.Trim());
            auth = await Database.AuthenticateUserAsync(user, this);
            if (auth == 0)
            {
                if (cboxAutoLogin.Checked && !Database.local.autoLogin)
                {
                    Database.local.autoLogin = true;
                    Database.SaveLocal(this);
                }

                Intent i = new Intent(this, typeof(MainActivity));
                StartActivity(i);

            }
            else if (auth == 1)
                Toast.MakeText(this, "Usuário e/ou senha incorretos", ToastLength.Short).Show();
            diag.Dismiss();
            //pbar.Visibility = Android.Views.ViewStates.Invisible;
            ResetFields();
        }

        private void ResetFields()
        {
            btnLogin.Enabled = true;
            txtPass.Enabled = true;
            txtUser.Enabled = true;
            txtPass.Text = "";
            cboxAutoLogin.Checked = false;
        }

        [Export("btnSair_Click")]
        public void btnSair_Click()
        {
            Finish();
        }
      
    }
}

