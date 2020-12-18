using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace TagMultiviewerLayoutGetter
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            List<Multiviewer> mvwrList = new List<Multiviewer>();
         //   HashSet<(string, string)> labelAndUid = new
            string file = args[0];
            if (File.Exists(file))
            {
                Console.WriteLine($"Reading from file '{file}'");
                string[] targets = File.ReadAllLines(file);
                Parallel.ForEach(targets, (s) =>
                {
                    string[] ss = s.Split('|');
                    Console.WriteLine($"connecting to {ss[1]} ({ss[0]})");
                    var lay = GetMultiviewerLayouts(ss[1]);
                    Multiviewer mvwr = new Multiviewer();

                    mvwr.Uid = ss[0];
                    mvwr.Address = ss[1];
                    mvwr.Layouts = lay??new  Layout[0];
                    lock(mvwrList)
                    {
                        mvwrList.Add(mvwr);
                        Console.WriteLine($"adding {mvwr.Uid}");
                    }
                });



            }

            FileInfo fi = new FileInfo($"Layout_allocation_{DateTime.Now.ToLocalTime():yyyy_MM_dd_HH-mm-ss}");
            using (ExcelPackage pkg = new ExcelPackage(fi))
            {

                var sheet = pkg.Workbook.Worksheets.Add("data");
                sheet.Cells[1, 1].Value = "Uid";
                sheet.Cells[1, 2].Value = "Label";
                sheet.Cells[1, 3].Value = "uuid";

                int row = 2;
                foreach (var thing in mvwrList.OrderBy(x => x.Uid))
                {
                    foreach (var layout in thing.Layouts)
                    {
                        sheet.Cells[row, 1].Value = thing.Uid;
                        sheet.Cells[row, 2].Value = layout.label;
                        sheet.Cells[row, 3].Value = layout.uuid;
                        row++;
                    }
                }
                pkg.Save();
            }

            var layoutCount = mvwrList.SelectMany(x => x.Layouts).Count();
            Console.WriteLine($"Data for {mvwrList.Count} multiviewers ({layoutCount} layouts) written to '{fi.FullName}'");
            Console.ReadKey();
        }



        private static Layout[] GetMultiviewerLayouts(string ipaddress)
        {
            using (var wc = new WebClientWithTimeout(2000))
            {
                
                string authInfo = "Admin:Admin";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                wc.Headers.Add("Authorization", "Basic " + authInfo);
                wc.Headers.Add("Content-Type", "application/json");

                try
                {
                    string url = $"http://{ipaddress}/api/4.0/layouts/.json";
                    string json = wc.DownloadString(url);
                    if (!string.IsNullOrEmpty(json))
                    {
                        LayoutRootObject[] layout = JsonConvert.DeserializeObject<LayoutRootObject[]>(json);
                        return layout.Select(x=>x.Layout).ToArray();
                    }
                }
                catch (WebException wex)
                {

                }
                catch (Exception ex)
                {

                }
            }
            return null;

        }

        public class WebClientWithTimeout : WebClient
        {
            int Timeout = 0;
            public WebClientWithTimeout(int timeout = 1000)
            {
                Timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = Timeout; // timeout in milliseconds (ms)
                return wr;
            }
        }
    }





}
