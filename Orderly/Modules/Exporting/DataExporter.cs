using Orderly.Database.Entities;
using Orderly.DaVault;
using Orderly.Extensions;
using Orderly.Helpers;
using Orderly.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Modules.Exporting
{
    public class DataExporter
    {
        public static void Export(IEnumerable<Credential> credentials, string path, ExportFormats format)
        {
            switch (format) {
                case ExportFormats.CSV:
                    ExportCSV(credentials, path);
                    break;
                case ExportFormats.TXT: 
                    ExportTXT(credentials, path); 
                    break;
                case ExportFormats.HTML:
                    ExportHTML(credentials, path);
                        break;
            }
        }

        private static void ExportCSV(IEnumerable<Credential> credentials, string path)
        {
            List<string> lines = ["Category, ServiceName, Username, Password"];

            Vault v = App.GetService<Vault>()!;  

            credentials.ForEach(x => {
                lines.Add($"{x.Category.Name}, {x.ServiceName}, {x.Username}, {EncryptionHelper.DecryptString(x.Password, v.PasswordEncryptionKey)}");
            });

            File.WriteAllLines($"{path}.csv", lines);
        }

        private static void ExportTXT(IEnumerable<Credential> credentials, string path)
        {
            List<string> lines = new();

            Vault v = App.GetService<Vault>()!;

            credentials.ForEach(x => {
                lines.Add($"{x.Category.Name}, {x.ServiceName}, {x.Username}, {EncryptionHelper.DecryptString(x.Password, v.PasswordEncryptionKey)}");
            });

            File.WriteAllLines($"{path}.txt", lines);
        }

        private static void ExportHTML(IEnumerable<Credential> credentials, string path)
        {
            List<string> lines = new();
            lines.Add("<!DOCTYPE html> <html> <head><style>body{  font-family: 'Helvetica', 'Arial', sans-serif;  } #passwords { font-family: 'Helvetica', 'Arial', sans-serif;    border-collapse: collapse;    width: 100%;  }    #passwords td, #passwords th {    border: 1px solid #ddd;    padding: 8px;  }    #passwords tr:nth-child(even){background-color: #f2f2f2;}    #passwords tr:hover {background-color: #ddd;}    #passwords th {    padding-top: 12px;    padding-bottom: 12px;    text-align: left;    background-color: #fc9038;    color: white;  }  </style>  </head>  <body>    <h1>Orderly - George's passwords</h1>    <table id=\"passwords\">    <tr>      <th>Category</th>      <th>Service Name</th>      <th>Username</th>  <th>Password</th>  </tr>");

            Vault v = App.GetService<Vault>()!;

            credentials.ForEach(x => {
                lines.Add($"<tr><td>{x.Category.Name}</td><td>{x.ServiceName}</td><td>{x.Username}</td><td>{EncryptionHelper.DecryptString(x.Password, v.PasswordEncryptionKey)}</td></tr>");
            });
            lines.Add("</table></body></html>");
            File.WriteAllLines($"{path}.html", lines);
        }

        private static void ExportPDF(IEnumerable<Credential> credentials, string path)
        {

        }
    }
}
