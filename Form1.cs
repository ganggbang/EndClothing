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

namespace EndClothing
{
    public partial class Form1 : Form
    {
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
                httpWebRequest.AllowAutoRedirect = true;
                httpWebRequest.Method = "GET";

                httpWebRequest.Referer = refer;

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {

                    using (var stream = httpWebResponse.GetResponseStream())
                    {

                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
                        {
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

        private void bn_start_Click(object sender, EventArgs e)
        {
            var url = "http://endclothing.com/";
            var html = _getRequest(url, "");

            //var t,r,a,f, WOlLQxg={"xcWIr":

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
    }
}
