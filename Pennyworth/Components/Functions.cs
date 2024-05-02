using Pennyworth.Properties;
using Newtonsoft.Json;
using Sap.Data.Hana;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Threading;
using static Pennyworth.Bodys;
using System.Net;
using System.Windows.Forms;
using System.ServiceModel;
using System.EnterpriseServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pennyworth.Components
{
    public class Functions
    {
        
        public static Bodys.Response GetPL(Bodys.Ubications PL)
        {
            Bodys.Response resp = new Bodys.Response();
            HanaConnection conn = new HanaConnection("Server=Hanab1:30013;UserID=SYSTEM;Password=B1AdminH2;Database=NDB");
            var request = PL.ubication;

            conn.Open();
            //request = request.Remove(0, 1);
            try
            {
                string query = "SELECT DISTINCT " +
                                "J.\"BcdCode\" AS \"Codigo Barras\"," +
                                "A.\"ItemCode\" AS \"Articulo\"," +
                                "B.\"ItemName\" AS \"Descripción\"," +
                                "CASE WHEN B.\"ManBtchNum\" = 'Y' THEN H.\"DistNumber\" " +
                                "ELSE CASE WHEN B.\"ManSerNum\" = 'Y' THEN F.\"DistNumber\" ELSE 'N/A' END END \"Serie/Lote\"," +
                                "D.\"BinCode\" AS \"Ubicación\"," +
                                "IFNULL(TO_VARCHAR (TO_DATE(H.\"ExpDate\"), 'yyyy/MM/dd'),'N/A') AS \"Fecha Caducidad\" " +
                                "FROM "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OITW A " +
                                "INNER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OITM B ON B.\"ItemCode\" = A.\"ItemCode\" AND B.\"InvntItem\" = 'Y' AND B.\"SellItem\" = 'Y' AND B.\"PrchseItem\" = 'Y' " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OIBQ C ON C.\"ItemCode\" = A.\"ItemCode\" AND C.\"WhsCode\" = A.\"WhsCode\" " +
                                "INNER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OBIN D ON D.\"AbsEntry\" = C.\"BinAbs\" AND C.\"OnHandQty\">0 " +
                                "LEFT OUTER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OSBQ E ON E.\"BinAbs\" = C.\"BinAbs\" AND E.\"ItemCode\" = C.\"ItemCode\" AND E.\"OnHandQty\">0 " +
                                "LEFT OUTER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OSRN F ON F.\"AbsEntry\" = E.\"SnBMDAbs\" AND F.\"ItemCode\" = E.\"ItemCode\" " +
                                "LEFT OUTER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OBBQ G ON G.\"BinAbs\" = C.\"BinAbs\" AND G.\"ItemCode\" = C.\"ItemCode\" AND G.\"OnHandQty\">0 " +
                                "LEFT OUTER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OBTN H ON H.\"AbsEntry\" = G.\"SnBMDAbs\" AND H.\"ItemCode\" = G.\"ItemCode\" " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OITB I ON I.\"ItmsGrpCod\" = B.\"ItmsGrpCod\" " +
                                "INNER JOIN "+ Pennyworth.Properties.Settings.Default.CompanyDB+".OBCD J ON J.\"ItemCode\" = A.\"ItemCode\" " +
                                "WHERE A.\"ItemCode\" LIKE '%%' " +
                                "AND D.\"BinCode\" LIKE '%" + request + "%' " +
                                "AND A.\"OnHand\" > 0 " +
                                "ORDER BY D.\"BinCode\" ";


                HanaCommand cmd = new HanaCommand(query, conn);
                HanaDataReader reader = cmd.ExecuteReader();
                int x = 0;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        x++;
                    }
                }
                var json = "";

                cmd = new HanaCommand(query, conn);
                reader = cmd.ExecuteReader();

                List<Bodys.Products> poo = new List<Bodys.Products>();
                Bodys.Products lista = new Bodys.Products();

                if (reader.HasRows)
                {
                    string documento = "";
                    int y = 0;
                    while (reader.Read())
                    {
                        //if (documento != "" && documento != reader.GetString(0))
                        //{
                        //    poo.Add(lista);

                        //    lista = new Bodys.Products();
                        //}
                        documento = reader.GetString(0);

                        lista.CodeBar = reader.GetString(0);
                        lista.ItemCode = reader.GetString(1);
                        lista.ItemName = reader.GetString(2);
                        lista.BatchNum = reader.GetString(3);
                        lista.Ubications = reader.GetString(4);
                        lista.ExpDate = reader.GetDateTime(5).ToString("yyyy-MM-dd");


                        poo.Add(lista);

                        lista = new Bodys.Products();
                        

                    }
                }

                resp.products = poo;
                return resp;
            }
            catch (Exception ex)
            {
                Bodys.Conflicts c = new Bodys.Conflicts();
                List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                resp.conflicts = new List<Bodys.Conflicts>();
                c.Problems = true;
                c.Description = ex.Message;

                //string path = Application.CommonAppDataPath + "\\Log.txt";
                //string texto = c.Description;
                //File.AppendAllLines(path, new String[] { texto });

                conflicts.Add(c);
                resp.conflicts = conflicts;
                conn.Close();
                return resp;
            }

        }

        public static Bodys.Response GetPBS(Bodys.GetItem PBS)
        {
            Bodys.Response resp = new Bodys.Response();
            HanaConnection conn = new HanaConnection("Server=Hanab1:30013;UserID=SYSTEM;Password=B1AdminH2;Database=NDB");
            var whsCode = PBS.whsCode;
            var sku = PBS.sku;

            conn.Open();
            //request = request.Remove(0, 1);
            try
            {
                string query = "SELECT DISTINCT " +
                                "A.\"ItemCode\" AS \"Articulo\"," +
                                "B.\"ItemName\" AS \"Descripción\"," +
                                "CASE WHEN B.\"ManBtchNum\" = 'Y' THEN H.\"DistNumber\" " +
                                "ELSE CASE WHEN B.\"ManSerNum\" = 'Y' THEN F.\"DistNumber\" ELSE 'N/A' END END \"Serie/Lote\"," +
                                "D.\"BinCode\" AS \"Ubicación\"," +
                                "IFNULL(TO_VARCHAR (TO_DATE(H.\"ExpDate\"), 'yyyy/MM/dd'),'1987/01/01') AS \"Fecha Caducidad\" " +
                                "FROM " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OITW A " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OITM B ON B.\"ItemCode\" = A.\"ItemCode\" AND B.\"InvntItem\" = 'Y' AND B.\"SellItem\" = 'Y' AND B.\"PrchseItem\" = 'Y' " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OIBQ C ON C.\"ItemCode\" = A.\"ItemCode\" AND C.\"WhsCode\" = A.\"WhsCode\" " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OBIN D ON D.\"AbsEntry\" = C.\"BinAbs\" AND C.\"OnHandQty\">=0 " +
                                "LEFT OUTER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OSBQ E ON E.\"BinAbs\" = C.\"BinAbs\" AND E.\"ItemCode\" = C.\"ItemCode\" AND E.\"OnHandQty\">=0 " +
                                "LEFT OUTER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OSRN F ON F.\"AbsEntry\" = E.\"SnBMDAbs\" AND F.\"ItemCode\" = E.\"ItemCode\" " +
                                "LEFT OUTER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OBBQ G ON G.\"BinAbs\" = C.\"BinAbs\" AND G.\"ItemCode\" = C.\"ItemCode\" AND G.\"OnHandQty\">=0 " +
                                "LEFT OUTER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OBTN H ON H.\"AbsEntry\" = G.\"SnBMDAbs\" AND H.\"ItemCode\" = G.\"ItemCode\" " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OITB I ON I.\"ItmsGrpCod\" = B.\"ItmsGrpCod\" " +
                                "INNER JOIN " + Pennyworth.Properties.Settings.Default.CompanyDB + ".OBCD J ON J.\"ItemCode\" = A.\"ItemCode\" " +
                                "WHERE A.\"WhsCode\" LIKE '%" + whsCode +"%' " +
                                "AND J.\"BcdCode\" LIKE '%"+ sku +"%' " +
                                "AND D.\"BinCode\" LIKE '%%' " +
                                "AND A.\"OnHand\" >= 0 " +
                                "ORDER BY D.\"BinCode\" ";


                HanaCommand cmd = new HanaCommand(query, conn);
                HanaDataReader reader = cmd.ExecuteReader();
                int x = 0;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        x++;
                    }
                }
                var json = "";

                cmd = new HanaCommand(query, conn);
                reader = cmd.ExecuteReader();

                List<Bodys.Products> poo = new List<Bodys.Products>();
                Bodys.Products lista = new Bodys.Products();

                if (reader.HasRows)
                {
                    string documento = "";
                    int y = 0;
                    while (reader.Read())
                    {
                        //if (documento != "" && documento != reader.GetString(0))
                        //{
                        //    poo.Add(lista);

                        //    lista = new Bodys.Products();
                        //}
                        documento = reader.GetString(1);

                        
                        lista.ItemCode = reader.GetString(0);
                        lista.ItemName = reader.GetString(1);
                        lista.BatchNum = reader.GetString(2);
                        lista.ExpDate = reader.GetDateTime(4).ToString("yyyy-MM-dd");
                        lista.Ubications = reader.GetString(3);


                        poo.Add(lista);

                        lista = new Bodys.Products();


                    }
                }

                resp.products = poo;
                return resp;
            }
            catch (Exception ex)
            {
                Bodys.Conflicts c = new Bodys.Conflicts();
                List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                resp.conflicts = new List<Bodys.Conflicts>();
                c.Problems = true;
                c.Description = ex.Message;

                //string path = Application.CommonAppDataPath + "\\Log.txt";
                //string texto = c.Description;
                //File.AppendAllLines(path, new String[] { texto });

                conflicts.Add(c);
                resp.conflicts = conflicts;
                conn.Close();
                return resp;
            }

        }

        public static Bodys.Response PostIC(Bodys.InventoryCountings IC)
        {
            Bodys.Response resp = new Bodys.Response();
            var jsonObject = JsonConvert.SerializeObject(IC);
            string data = string.Empty;
            string url = string.Empty;
            string json = string.Empty;
            var result = string.Empty;
            string DocNum = string.Empty;
            var request = 0;
            string IDss = string.Empty;
            string otra = string.Empty;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpResponse;
            HttpStatusCode STATUS;
            CookieContainer cookies = new CookieContainer();
            HanaConnection conn = new HanaConnection("Server=Hanab1:30013;UserID=SYSTEM;Password=B1AdminH2;Database=NDB");

            HttpClient httpClient = new HttpClient();

            var urlhttpclient = "https://hanab1:50000/b1s/v1/Login";
            var parameters = new Dictionary<string, string> { { "CompanyDB", "TESTREP" }, { "Password", "123456" }, { "UserName", "Desarrollo" } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            //var response = httpClient.PostAsync(url, encodedContent).ConfigureAwait(false);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    // Do something with response. Example get content:
            //    // var responseContent = await response.Content.ReadAsStringAsync ().ConfigureAwait (false);
            //}

            try
            {
                try
                {
                    data = "{    \"CompanyDB\": \"" + Settings.Default.CompanyDB + "\",    \"UserName\": \"" + Settings.Default.UserName + "\",       \"Password\": \"" + Settings.Default.Password + "\"}";
                    url = "https://hanab1:50000/b1s/v1/Login";
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
                    httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
                    httpWebRequest.Accept = "*/*";
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    { streamWriter.Write(data); }

                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //B1Session obj = null;            

                    result = string.Empty;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();


                        var sesion = JsonConvert.DeserializeObject<dynamic>(result);
                        IDss = sesion["SessionId"];

                    }
                }
                catch (Exception ex)
                {
                    Bodys.Conflicts c = new Bodys.Conflicts();
                    List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                    resp.conflicts = new List<Bodys.Conflicts>();
                    c.Problems = true;
                    c.Description = ex.ToString();

                    string path = Application.CommonAppDataPath + "\\Log.txt";
                    string texto = c.Description;
                    File.AppendAllLines(path, new String[] { texto });

                    conflicts.Add(c);
                    resp.conflicts = conflicts;

                    return resp;

                }
                try
                {
                    jsonObject = JsonConvert.SerializeObject(IC);
                    data = jsonObject;
                    url = "https://hanab1:50000/b1s/v1/InventoryCountings";
                    json = string.Empty;

                    httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
                    httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
                    httpWebRequest.Accept = "*/*";
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                    cookies = new CookieContainer();
                    cookies.Add(new Cookie("B1SESSION", IDss) { Domain = "hanab1" });
                    cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = "hanab1" });
                    httpWebRequest.CookieContainer = cookies;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    { streamWriter.Write(data); }

                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                    result = string.Empty;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                        var sesion = JsonConvert.DeserializeObject<dynamic>(result);
                        IDss = sesion["DocumentNumber"];
                        otra = sesion["DocumentEntry"];
                    }
                    resp.DocEntry = otra;
                    resp.DocNum = IDss;
                    return resp;
                }
                catch (Exception ex)
                {
                    Bodys.Conflicts c = new Bodys.Conflicts();
                    List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                    resp.conflicts = new List<Bodys.Conflicts>();
                    c.Problems = true;
                    c.Description = ex.ToString();

                    string path = Application.CommonAppDataPath + "\\Log.txt";
                    string texto = c.Description;
                    File.AppendAllLines(path, new String[] { texto });

                    conflicts.Add(c);
                    resp.conflicts = conflicts;

                    return resp;


                }
            }
            catch (Exception ex)
            {
                Bodys.Conflicts c = new Bodys.Conflicts();
                List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                resp.conflicts = new List<Bodys.Conflicts>();
                c.Problems = true;
                c.Description = ex.ToString();

                string path = Application.CommonAppDataPath + "\\Log.txt";
                string texto = c.Description;
                File.AppendAllLines(path, new String[] { texto });

                conflicts.Add(c);
                resp.conflicts = conflicts;

                return resp;
            }

        }

        public static Bodys.Response PostIP(Bodys.InventoryPostings IP)
        {
            Bodys.Response resp = new Bodys.Response();
            var jsonObject = JsonConvert.SerializeObject(IP);
            string data = string.Empty;
            string url = string.Empty;
            string json = string.Empty;
            var result = string.Empty;
            string DocNum = string.Empty;
            var request = 0;
            string IDss = string.Empty;
            string otra = string.Empty;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpResponse;
            CookieContainer cookies = new CookieContainer();
            HanaConnection conn = new HanaConnection("Server=Hanab1:30013;UserID=SYSTEM;Password=B1AdminH2;Database=NDB");

            try
            {


                try
                {
                    data = "{    \"CompanyDB\": \"" + Settings.Default.CompanyDB + "\",    \"UserName\": \"" + Settings.Default.UserName + "\",       \"Password\": \"" + Settings.Default.Password + "\"}";
                    url = "https://hanab1:50000/b1s/v1/Login";
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
                    httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
                    httpWebRequest.Accept = "*/*";
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    { streamWriter.Write(data); }

                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //B1Session obj = null;            

                    result = string.Empty;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();


                        var sesion = JsonConvert.DeserializeObject<dynamic>(result);
                        IDss = sesion["SessionId"];

                    }
                }
                catch (Exception ex)
                {
                    Bodys.Conflicts c = new Bodys.Conflicts();
                    List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                    resp.conflicts = new List<Bodys.Conflicts>();
                    c.Problems = true;
                    c.Description = ex.ToString();

                    string path = Application.CommonAppDataPath + "\\Log.txt";
                    string texto = c.Description;
                    File.AppendAllLines(path, new String[] { texto });

                    conflicts.Add(c);
                    resp.conflicts = conflicts;

                    return resp;

                }
                try
                {
                    jsonObject = JsonConvert.SerializeObject(IP);
                    data = jsonObject;
                    url = "https://hanab1:50000/b1s/v1/InventoryPostings";
                    json = string.Empty;

                    httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
                    httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
                    httpWebRequest.Accept = "*/*";
                    httpWebRequest.ServicePoint.Expect100Continue = false;
                    httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
                    cookies = new CookieContainer();
                    cookies.Add(new Cookie("B1SESSION", IDss) { Domain = "hanab1" });
                    cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = "hanab1" });
                    httpWebRequest.CookieContainer = cookies;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    { streamWriter.Write(data); }

                    result = string.Empty;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();

                        var sesion = JsonConvert.DeserializeObject<dynamic>(result);
                        IDss = sesion["DocumentNumber"];
                        otra = sesion["DocumentEntry"];
                    }
                    resp.DocEntry = otra;
                    resp.DocNum = IDss;
                    return resp;
                }
                catch (Exception ex)
                {
                    Bodys.Conflicts c = new Bodys.Conflicts();
                    List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                    resp.conflicts = new List<Bodys.Conflicts>();
                    c.Problems = true;
                    c.Description = ex.ToString();

                    string path = Application.CommonAppDataPath + "\\Log.txt";
                    string texto = c.Description;
                    File.AppendAllLines(path, new String[] { texto });

                    conflicts.Add(c);
                    resp.conflicts = conflicts;

                    return resp;

                }
            }
            catch (Exception ex)
            {
                Bodys.Conflicts c = new Bodys.Conflicts();
                List<Bodys.Conflicts> conflicts = new List<Bodys.Conflicts>();
                resp.conflicts = new List<Bodys.Conflicts>();
                c.Problems = true;
                c.Description = ex.ToString();

                string path = Application.CommonAppDataPath + "\\Log.txt";
                string texto = c.Description;
                File.AppendAllLines(path, new String[] { texto });

                conflicts.Add(c);
                resp.conflicts = conflicts;

                return resp;

            }
        }

    }
}