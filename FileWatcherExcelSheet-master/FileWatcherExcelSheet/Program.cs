using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

using System.Text.Encodings;
using System.Linq;
using System.Collections.Generic;
using FileWatcherExcelSheet.Model;

using FileWatcherExcelSheet.Conext;

namespace FileWatcherExcelSheet
{
    class Program
    {
        static List<Customer> customers = new List<Customer>();
        static DBContext db = new DBContext();


        static void Main(string[] args)
        {
            Console.WriteLine("Execution started!");
            var watcher = new FileSystemWatcher(@"D:\AUB\TestFolder");

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "Customers.xlsx";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
           
            Console.WriteLine("Done");
            Console.ReadLine();
        }// Main 
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            ReadDataFromExcel();
            AddCusotmersIntoDB();
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }


        private static void ReadDataFromExcel()
        {

            try
            {

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                FileStream stream = File.Open(@"D:\AUB\TestFolder\Customers.xlsx", FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                while (excelReader.Read())
                    FillCustomerList(excelReader[0].ToString(), excelReader[1].ToString(), excelReader[2].ToString());

                excelReader.Close();


            } // try
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } // catch

        }

        private static void FillCustomerList(string name, string email, string status)
        {
            customers.Add(new Customer
            {
                Email = email,
                Name = name,
                Status = status
            });
        }

        private static void AddCusotmersIntoDB()
        {
            foreach (var Cus in customers)
            {
                db.customer.Add(Cus);
            }
            db.SaveChanges();
        }
    }
}