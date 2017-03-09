using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Android.Support.Design.Widget;
using Java.Lang;

namespace WardApp
{
    public static class Database
    {        
        public static Wrapper local;    
        //Test
        private static async Task<string> RestConnectionAsync<T>(Uri uri, string method, Context ctx ,T data = default(T))
        {

            string response = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method;
            request.ContentType = "application/json";
            request.Timeout = 10000;

            try
            {
                await Task.Run(async () =>
                {

                    if (method != "GET")
                    {
                        //MemoryStream mstream = new MemoryStream();
                        //DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                        //jsonSerializer.WriteObject(mstream, data);
                        //request.ContentLength = mstream.Length;

                        //================= Método 1 Para Enviar os Dados em JSON ==================
                        //using (Stream requestStream = request.GetRequestStream())
                        //{
                        //    // Send the data.
                        //    requestStream.Write(mstream.ToArray(), 0, (int)mstream.Length);
                        //}

                        //========================== Método 2 ============================
                        using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                        using (JsonWriter jsonw = new JsonTextWriter(sw))
                        {
                            JsonSerializer jsonser = new JsonSerializer();
                            jsonser.Serialize(jsonw, data);
                        }
                    }

                    //================== Esperando Resposta ===========================
                    using (WebResponse resp = await request.GetResponseAsync())
                    {
                        using (StreamReader reader = new StreamReader(resp.GetResponseStream(), ASCIIEncoding.ASCII))
                        {
                            response = await reader.ReadToEndAsync();
                        }
                    }

                    //request.BeginGetResponse((x) =>
                    //{
                    //    using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                    //    {
                    //        using (var reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
                    //        {
                    //            Resp = reader.ReadToEnd();
                    //        }
                    //    }
                    //}, null);
                });
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.NameResolutionFailure)
                    Toast.MakeText(ctx, "Sem conexão com o servidor\n" + e.Message, ToastLength.Long).Show();
                else if (e.Status == WebExceptionStatus.Timeout)
                    Toast.MakeText(ctx, "Sem resposta do servidor", ToastLength.Short).Show();
                else if (e.Status == WebExceptionStatus.ReceiveFailure)
                    Toast.MakeText(ctx, "Erro ao receber resposta do servidor", ToastLength.Long).Show();
                
                else
                    Toast.MakeText(ctx, e.Message, ToastLength.Long).Show();
            }
            return response;

        }

        public static async Task<int> AuthenticateUserAsync(UsuarioTable user, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/user_authentication.php");
            string response = await RestConnectionAsync<UsuarioTable>(adress, "POST", ctx, user);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null)
            {
                
                bool logAuth = responseObj.result;
                if (logAuth)
                {
                    JObject userObj = responseObj.usuario;
                    local.activeUser = userObj.ToObject<UsuarioTable>();
                    return 0;
                }
                else
                    return 1;
            }
            else
                return -1;
            


        }

        public static async Task<List<ReservaTable>> GetReservasByUserIdAsync(string user_id, int res_status, Context ctx)
        {           
            Uri adress = new Uri("http://ward.freeoda.com/getReservaByUserId.php?user_id="+ user_id+"&res_status="+res_status);
            string response = await RestConnectionAsync<string>(adress, "GET", ctx);           
            dynamic responseObj = JsonConvert.DeserializeObject(response);

            if (responseObj != null && responseObj.Count > 0)
            {
                JArray jArray = responseObj;
                return jArray.ToObject<List<ReservaTable>>();
            }
            else if (responseObj != null && responseObj.Count == 0)
                return new List<ReservaTable>();
            else
                return null;
                       
        }
        public static async Task<List<AulaTable>> GetAulasByUserIdAsync(string user_id, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/getAulasByUserId.php?user_id=" + user_id);
            string response = await RestConnectionAsync<string>(adress, "GET", ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null && responseObj.Count > 0)
            {
                JArray jArray = responseObj;
                return jArray.ToObject<List<AulaTable>>();
            }
            else if (responseObj != null && responseObj.Count == 0)
                return new List<AulaTable>();
            else
                return null;
        }
        public static async Task<List<SalaTable>> GetSalasByUserIdAsync(string user_id, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/getSalasByUserId.php?user_id=" + user_id);
            string response = await RestConnectionAsync<string>(adress, "GET" , ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null && responseObj.salas.Count > 0)
            {
                JArray jSalas = responseObj.salas;
                JArray jRecursos = responseObj.recursos;
                local.recursosReservados.Clear();
                local.recursosReservados.AddRange(jRecursos.ToObject<List<RecursoTable>>());
                return jSalas.ToObject<List<SalaTable>>();
            }
            else if (responseObj != null && responseObj.salas.Count == 0)
                return new List<SalaTable>();
            
            else
                return null;
            

        }
        public static async Task<List<SalaTable>> GetSalasByAulaIdAsync(string aula_id, string dt_inicio, string dt_final, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/getSalasByAulaId.php?aula_id=" +aula_id+"&dt_inicio="+dt_inicio+"&dt_final="+dt_final );
            string response = await RestConnectionAsync<string>(adress, "GET", ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null && responseObj.salas.Count > 0)
            {
                JArray jSalas = responseObj.salas;
                JArray jRecursos = responseObj.recursos;
                local.recursosDisponiveis.Clear();
                local.recursosDisponiveis.AddRange(jRecursos.ToObject<List<RecursoTable>>());
                return jSalas.ToObject<List<SalaTable>>();
            }
            else if (responseObj != null && responseObj.Count == 0)
                return new List<SalaTable>();
            else
                return null;


        }
        public static async Task<List<DisciplinaTable>> GetDisciplinasByUserIdAsync(string user_id, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/getDisciplinasByUserId.php?user_id=" + user_id);
            string response = await RestConnectionAsync<string>(adress, "GET", ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);

            JArray jArray = responseObj;
            return jArray.ToObject<List<DisciplinaTable>>();

        }
        public static async Task<List<TurmaTable>> GetTurmasByUserIdAsync(string user_id, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/getTurmasByUserId.php?user_id=" + user_id);
            string response = await RestConnectionAsync<string>(adress, "GET", ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);

            JArray jArray = responseObj;
            return jArray.ToObject<List<TurmaTable>>();

        }

        public static async Task<bool> UpdtUserSenhaByUserIdAsync(string senha, string user_id, Context ctx)
        {
            UsuarioTable u = new UsuarioTable();
            u.user_senha = senha;
            Uri adress = new Uri("http://ward.freeoda.com/updtUsuarioSenhaByUserId.php?user_id=" + user_id);
            string response = await RestConnectionAsync(adress,"PUT", ctx, u);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null)
            {
                bool isUpdated = responseObj;
                return isUpdated;
            }
            else
                return false;           
           
        }
        public static async Task<bool> UpdtResStatusByUserIdAsync(int status, string res_id, Context ctx)
        {
            string stat = status.ToString();
            Uri adress = new Uri("http://ward.freeoda.com/updtReservaStatus.php?res_id=" + res_id +"&res_status="+ stat);
            string response = await RestConnectionAsync<string>(adress, "PUT", ctx);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null)
            {
                bool isUpdated = responseObj;
                return isUpdated;
            }
            else
                return false;
            
        }

        public static async Task<bool> PostNewReserva(ReservaTable reserva, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/postNewReserva.php");
            string response = await RestConnectionAsync(adress, "POST", ctx, reserva);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null)
            {
                bool reservaAdded = responseObj;
                return reservaAdded;
            }
            else
                return false;
        }
        public static async Task<bool> PostResetPass(UsuarioTable user, Context ctx)
        {
            Uri adress = new Uri("http://ward.freeoda.com/postResetPass.php");
            string response = await RestConnectionAsync(adress, "POST", ctx, user);
            dynamic responseObj = JsonConvert.DeserializeObject(response);
            if (responseObj != null)
            {
                bool result = responseObj.result;
                return result;
            }
            else
                return false;
        }

        public static void SaveLocal(Context ctx)
        {
            using (StreamWriter sw = new StreamWriter(ctx.OpenFileOutput("database.json", FileCreationMode.Private)))
            using (JsonWriter jsonw = new JsonTextWriter(sw))
            {
                JsonSerializer jsonser = new JsonSerializer();
                jsonser.Serialize(jsonw, local);
            }
        }

        public static void LoadLocal(Context ctx)
        {
            try
            {
                using (StreamReader sr = new StreamReader(ctx.OpenFileInput("database.json")))
                using (JsonReader jsonRd = new JsonTextReader(sr))
                {
                    JsonSerializer jsonser = new JsonSerializer();
                    local = jsonser.Deserialize<Wrapper>(jsonRd);
                }
            }
            catch (System.Exception)
            {
                local = new Wrapper();
            }
        }

        public static async Task<bool> IsConnectedAsync(Context ctx)
        {
            Runtime rt = Runtime.GetRuntime();
            try
            {
                //Google
                Java.Lang.Process p = rt.Exec("/system/bin/ping -c 1 8.8.8.8");
                int exitValue = await p.WaitForAsync();
                //ward.freeoda.com
                p = rt.Exec("/system/bin/ping -c 1 144.76.145.166");
                int exitValue2 = await p.WaitForAsync();
                return (exitValue2 == 0 || exitValue == 0);
            }
            catch (IOException e) {  }
            catch (InterruptedException e) {  }

            return false;
            //Android.Net.ConnectivityManager connectivityManager = (Android.Net.ConnectivityManager)ctx.GetSystemService(Context.ConnectivityService);
            //Android.Net.NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            //return (activeConnection != null) && activeConnection.IsConnected;
        }

        public static string Encrypt(string data)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] array = hash.ComputeHash(enc.GetBytes(data));

                for (int i = 0; i < array.Length; i++)
                {
                    sb.Append(array[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }

    }

    public class Wrapper
    {
        public bool autoLogin = false;
        public int appTheme = 0;
        public UsuarioTable activeUser = new UsuarioTable();
        public List<ReservaTable> reservasAtivas = new List<ReservaTable>();
        public List<ReservaTable> reservasArquivadas = new List<ReservaTable>();
        public List<AulaTable> aulas = new List<AulaTable>();
        public List<SalaTable> salasReservadas = new List<SalaTable>();
        public List<RecursoTable> recursosReservados = new List<RecursoTable>();
        [JsonIgnore]
        public List<SalaTable> salasDisponiveis = new List<SalaTable>();
        [JsonIgnore]
        public List<RecursoTable> recursosDisponiveis = new List<RecursoTable>();
        

        public Wrapper()
        { }
    }


    [DataContract]
    public class UsuarioTable
    {
        [JsonProperty("user_id")]
        [DataMember(Name = "user_id")]
        public string user_id { get; set; }
        [JsonProperty("user_nome")]
        [DataMember(Name = "user_nome")]
        public string user_nome { get; set; }
        [JsonProperty("user_senha")]
        [DataMember(Name = "user_senha")]
        public string user_senha { get; set; }
        [JsonProperty("user_email")]
        [DataMember(Name = "user_email")]
        public string user_email { get; set; }
    }

    public class AulaTable
    {
        [JsonProperty]
        public string aula_id { get; set; }
        [JsonProperty]
        public string aula_usuario { get; set; }
        [JsonProperty]
        public string aula_turma { get; set; }
        [JsonProperty]
        public string aula_disciplina { get; set; }
        [JsonProperty]
        public int aula_horario { get; set; }
        [JsonProperty]
        public int aula_duracao { get; set; }
        [JsonProperty]
        public int aula_diasemana { get; set; }
        [JsonProperty]
        public string aula_curso { get; set; }

        public static string DiaSemana(int i)
        {
            switch (i)
            {
                case 0:
                    return "Domingo";
                case 1:
                    return "Segunda-Feira";
                case 2:
                    return "Terça-Feira";
                case 3:
                    return "Quarta-Feira";
                case 4:
                    return "Quinta-Feira";
                case 5:
                    return "Sexta-Feira";
                case 6:
                    return "Sábado";
                default:
                    return "Número inválido";
            }
        }
        public static string GetHorario(int aula_horario, int aula_duracao)
        {
            TimeSpan noite = new TimeSpan(0, 0, 0);

            if (aula_horario > 12)
                noite = new TimeSpan(0, 30, 0);

            TimeSpan inicio = new TimeSpan(7, 30, 0);
            TimeSpan duracao = new TimeSpan(0, 55, 0);
            inicio = inicio + TimeSpan.FromTicks(duracao.Ticks * (aula_horario - 1)) + noite;
            TimeSpan tst = TimeSpan.FromTicks(duracao.Ticks * aula_duracao);
            TimeSpan final = inicio + TimeSpan.FromTicks(duracao.Ticks * aula_duracao);
           return inicio.ToString(@"hh\:mm") + " - " + final.ToString(@"hh\:mm");
        }

    }

    [DataContract]
    public class SalaTable
    {
        [JsonProperty]
        public string sala_id { get; set; }
        [JsonProperty]
        public string sala_nome { get; set; }
        [JsonProperty]
        public string sala_numero { get; set; }
        [JsonProperty]
        public int sala_andar { get; set; }
        [JsonProperty]
        public int sala_capacidade { get; set; }
        [JsonProperty]
        public string sala_predio { get; set; }
        [JsonProperty]
        public int sala_status { get; set; }
    }

    [DataContract]
    public class DisciplinaTable
    {
        [JsonProperty]
        public string disc_id { get; set; }
        [JsonProperty]
        public string disc_nome { get; set; }
        [JsonProperty]
        public int disc_semestre { get; set; }
    }

    [DataContract]
    public class TurmaTable
    {
        [JsonProperty]
        public string tur_id { get; set; }
        [JsonProperty]
        public string tur_curso { get; set; }
        [JsonProperty]
        public int tur_quantidade { get; set; }
        [JsonProperty]
        public string tur_periodo { get; set; }
    }

    [DataContract]
    public class ReservaTable
    {
        [JsonProperty]
        [DataMember(Name = "res_id")]
        public string res_id { get; set; }
        [JsonProperty]
        [DataMember(Name = "res_aula_id")]
        public string res_aula_id { get; set; }
        [JsonProperty]
        public string res_sala { get; set; }
        [JsonProperty]
        [DataMember(Name = "res_data_ini")]
        public DateTime res_data_ini { get; set; }
        [JsonProperty]
        [DataMember(Name = "res_data_final")]
        public DateTime res_data_final { get; set; }
        [JsonProperty]
        [DataMember(Name = "res_criacao")]
        public DateTime res_criacao { get; set; }
        [JsonProperty]
        [DataMember(Name = "res_status")]
        public int res_status { get; set; } 
        
    }

    public class RecursoTable
    {
        [JsonProperty]
        public string rec_id { get; set; }
        [JsonProperty]
        public string rec_nome { get; set; }
        [JsonProperty]
        public string rec_descricao { get; set; }
        [JsonProperty]
        public int rec_status { get; set; }
        [JsonProperty("sala_rec_quantidade")]
        public string rec_quantidade { get; set; }
        [JsonProperty("sala_id")]
        public string rec_sala_id { get; set; }
    }
}