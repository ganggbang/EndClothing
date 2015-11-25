using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet.Net;


namespace EndClothing
{
    public partial class Form1 : Form
    {
        CookieDictionary cookies;

        public Form1()
        {
            InitializeComponent();
        }



        string GetFirstByRegex(string pattern, string text)
        {
            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            var matches = rgx.Matches(text);

            foreach (Match match in matches)
            {
                return match.Value.ToString();
            }
            return string.Empty;
        }


        List<string> GetListByRegex(string pattern, string text)
        {
            List<string> items = new List<string>();

            var rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            var matches = rgx.Matches(text);

            foreach (Match match in matches)
            {
                items.Add(match.Value.ToString());   
            }
            return items;
        }

        string _getRequest(string url, string refer)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.UserAgent = HttpHelper.ChromeUserAgent();
                httpWebRequest.AllowAutoRedirect = true;
                //if (cookies.Count > 0)
                //    httpWebRequest.CookieContainer = cookies;
                httpWebRequest.Method = "GET";

                httpWebRequest.Referer = refer;

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {

                    using (var stream = httpWebResponse.GetResponseStream())
                    {

                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
                        {

                            string[] cookieVal = null;
                            if (httpWebResponse.Headers["Set-Cookie"] != null)
                                cookieVal = httpWebResponse.Headers["Set-Cookie"].Split(new char[] { ',' });
                            GetCookies(cookieVal);
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                    return sr.ReadToEnd();
            }
        }

        

        string parseIntList2Str(List<string> list_expression)
        {

            int v_exp = 0;
            string parse_to_int = "";
            foreach (String exp in list_expression)
            {
                if (exp.Length > 1)
                {
                    v_exp = 0;
                    v_exp += GetListByRegex(@"\!\!\[\]", exp).Count();
                    v_exp += GetListByRegex(@"\!{1}\+", exp).Count();
                    parse_to_int += v_exp.ToString();
                }
            }

            return parse_to_int;
        }

        string Minus(string v1,string v2)
        {

            var i = Int32.Parse(v1) - Int32.Parse(v2);
            return i.ToString();
        }

        string Plus(string v1, string v2)
        {

            var i = Int32.Parse(v1) + Int32.Parse(v2);
            return i.ToString();
        }


        string Multiply(string v1, string v2)
        {

            var i = Int32.Parse(v1) * Int32.Parse(v2);
            return i.ToString();
        }

        CookieContainer GetCookies(string [] cookieVal)
        {
            CookieContainer cookie = new CookieContainer();

            try
            {
                foreach (string cook in cookieVal)
                {
                    string[] cookie1 = cook.Split(new char[] { ';' });
                    if (cookie1.Length < 2)
                        continue;

                    if (cookie1[0].IndexOf("=") < 0 || cookie1[1].IndexOf("=") < 0)
                        continue;

                    Cookie c = null;
                    if (cookie1.Length >= 3)
                        c = new Cookie(cookie1[0].Split(new char[] { '=' })[0], cookie1[0].Split(new char[] { '=' })[1], cookie1[1].Split(new char[] { '=' })[1]);
                    if (cookie1.Length == 2)
                        c = new Cookie(cookie1[0].Split(new char[] { '=' })[0], cookie1[0].Split(new char[] { '=' })[1]);

                    cookie.Add(new Uri("https://bestsecret.com"), c);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("\nMessage ---\n{0}", e.Message);
            }
            return cookie;
        }



        void Get503Exception(string url)
        {
            //var t,r,a,f, WOlLQxg={"xcWIr":
            
            var html = _getRequest(url, "");

            var t = GetFirstByRegex(@"[0-9\.]+", url).Length;

            string var = GetFirstByRegex(@"parseInt\([a-zA-Z0-9.]+", html).Substring(9);

            string var_without_obj = GetFirstByRegex(@"parseInt\([a-zA-Z0-9]+", html).Substring(9);

            string obj = var.Substring(var_without_obj.Length+1);

            string parent = @"[\-\*\+][\+\!\[\]\(\)]+};";
            
            var tmp1 = @"var\st,r,a,f,\s" + var_without_obj + "={\"" + obj + "\":";

            //выражение
            var expression = GetFirstByRegex(tmp1 + parent + "", html);

            expression = expression.Substring(expression.IndexOf(":") + 1, expression.Length - 4 - expression.IndexOf(":"));

            List<string> list_expression = GetListByRegex(@"[\[\]\+\!]+", expression);

            var p2i = parseIntList2Str(list_expression);
            
            string source = expression;

            string tmp = GetFirstByRegex(@"\);\s+;" + var + ".*;a.value", html);

            string v = tmp.Substring(tmp.IndexOf(var), tmp.Length - tmp.IndexOf(var) - "a.value".Length);
            //WOlLQxg.xcWIr-=+((!+[]+!![]+[])+(!+[]+!![]+!![]+!![]+!![]+!![]));

            List<string> list_exp = GetListByRegex(var + @"[\-\*\+]=[\+\!\[\]\(\)]+", html);

            foreach (string exp in list_exp)
            {
                var znak = GetFirstByRegex(@"[\-\*\+]=", exp).Substring(0,1);
                list_expression = GetListByRegex(@"[\[\]\+\!]+", exp);
                var p2i_new = parseIntList2Str(list_expression);

                if(znak == "-")
                    p2i = Minus(p2i, p2i_new);
                else if(znak == "+")
                    p2i = Plus(p2i, p2i_new);
                else if(znak == "*")
                    p2i = Multiply(p2i, p2i_new);
            }
            p2i = Plus(p2i,t.ToString());
        }

        private string getRequest(string url, string refer)
        {
            try
            {
               
                using (var request = new HttpRequest())
                {
                    string content;
                    request.UserAgent = HttpHelper.ChromeUserAgent();
                    request.Referer = refer;
                    request.AllowAutoRedirect = true;
                    request.Cookies = cookies;

                    HttpResponse response = request.Get(url);


                    var status = response.StatusCode.ToString();
                    using (var responseStream = response.ToStream())
                    {
                        cookies = response.Cookies;
                    }

                    content = response.ToString();

                    return content;

                }
            }
            catch (WebException we)
            {
                var wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode;
                return wRespStatusCode.ToString();
            }
        }

        private string postRequest(string url, string refer, string form_data)
        {
            try
            {
                using (var request = new HttpRequest())
                {
                    //form_key=uS3HD9QaXL8srTob&login%5Busername%5D=hasanqusay%40yahoo.com&login%5Bpassword%5D=3124681122Bh&send=
                    //var multipartContent = new MultipartContent() {
                    //    {new StringContent("form_key"), form_data},
                    //    {new StringContent("login%5Busername%5D"), "hasanqusay%40yahoo.com"},
                    //    {new StringContent("login%5Bpassword%5D"), "3124681122Bh"},
                    //    {new StringContent("send"), " "}
                    //};

                    string content;
                    request.UserAgent = HttpHelper.ChromeUserAgent();
                    request.Referer = refer;

                    request.AddParam("form_key", form_data)
                        .AddParam("login%5Busername%5D", "hasanqusay@yahoo.com")
                        .AddParam("login%5Bpassword%5D", "3124681122Bh")
                        .AddParam("send", "");
                    
                    request.AllowAutoRedirect = true;
                    request.Cookies = cookies;

                    HttpResponse response = request.Post(url);


                    var status = response.StatusCode.ToString();
                    using (var responseStream = response.ToStream())
                    {
                        cookies = response.Cookies;
                    }

                    content = response.ToString();

                    return content;

                }
            }
            catch (WebException we)
            {
                var wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode;
                return wRespStatusCode.ToString();
            }
        }

        string GetFormKey(string html, string xpath)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            
            doc.LoadHtml(html);

            var NodeCollections = doc.DocumentNode.SelectNodes(xpath);
            if (NodeCollections != null)
            {
                foreach (HtmlNode node in NodeCollections)
                {
                    return GetFirstByRegex("\".*\"", GetFirstByRegex("value=\".*\"", node.OuterHtml)).Replace("\"", "");
                }
            }

            return null;
        }

        List<String> GetWishList(string html, string xpath)
        {
            List<string> items = new List<string>();

            var doc = new HtmlAgilityPack.HtmlDocument();
            
            doc.LoadHtml(html);

            var NodeCollections = doc.DocumentNode.SelectNodes(xpath);
            if (NodeCollections != null)
            {
                foreach (HtmlNode node in NodeCollections)
                {
                }
            }

            return items;
        }


        private void bn_start_Click(object sender, EventArgs e)
        {
            var url = "http://endclothing.com/us/";
            //Get503Exception(url);

            var html = getRequest("http://www.endclothing.com/us/customer/account/login/", "");

            var form_key = GetFormKey(html, "//input[@name='form_key']");
            var content = postRequest("http://www.endclothing.com/us/customer/account/loginPost/", "http://www.endclothing.com/us/customer/account/login/", form_key );
            
            html = getRequest("http://www.endclothing.com/us/customer/account/", "http://www.endclothing.com/us/customer/account/loginPost/");

            //string ID = "316082";
            //var new_url = url + "wishlist/index/add/product/" + ID + "/" + form_key;


            //GetWishList(getRequest("https://www.endclothing.com/us/wishlist/", url), "//table[@class='data-table']/tbody/tr/td/a");
            GetWishList(getRequest("https://www.endclothing.com/us/wishlist/", url), "//table[@class='data-table']/tbody/tr/td/div/div[@class='add-to-cart-alt']/p[@class='availability out-of-stock']");

            //getRequest("https://www.endclothing.com/us/wishlist/", url);


            //html = getRequest("http://www.endclothing.com/us/customer/account/", "http://www.endclothing.com/us/customer/account/loginPost/");
            //316082
            //wishlist/index/add/product/ID/form_key/
        }
    }
}
