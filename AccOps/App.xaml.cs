using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;       //microsoft Excel 14 object in references-> COM tab
using System.IO;
using System.Reflection;

namespace AccOps
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public ObservableCollection<Outstanding> Expenses { get; set; } = new ObservableCollection<Outstanding>();


        private void AppStartup(object sender, StartupEventArgs args)
        {
            getExcelFile();
            LoadData();
        }

        private void LoadData()
        {
            //Expenses.Add(new Person() { Name = "Mike", Department = "Legal" });
            //Expenses.Add(new Person() { Name = "Lisa", Department = "Marketing" });

        }

        public class Person
        {

            public string Name { get; set;  }
            public string Department { get; set; }


        }


        public class Outstanding
        {
            public string Agent { get; set; }

            public Double Amount { get; set; }
        }

        public class Job
        {
            public string Client { get; set; }

            public string Agent { get; set; }

            public Double Amount { get; set; }

            public string Invoice { get; set; }

            public string Draft { get; set; }

            public string Settled { get; set; }

        }


        private void getExcelFile()
        {
            string executableLocation = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            string xslLocation = Path.Combine(executableLocation, @".\jobs.xlsx");

            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(xslLocation);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            List<Job> jobs = new List<Job>();

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            // skip header row
            for (int i = 2; i <= rowCount; i++)
            {
                Job job = new Job();
                for (int j = 1; j <= colCount; j++)
                {
                    ////new line
                    //if (j == 1)
                    //    Console.Write("\r\n");

                    ////write the value to the console
                    if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                    {
                        //    Console.Write(xlRange.Cells[i, j].Value2.ToString() + "\t");
                        switch (j)
                        {
                            case 1:
                                job.Client = xlRange.Cells[i, j].Value2.ToString();
                                break;
                            case 2:
                                job.Agent = xlRange.Cells[i, j].Value2.ToString();
                                break;
                            case 3:
                                job.Amount = xlRange.Cells[i, j].Value2;
                                break;
                            case 4:
                                job.Invoice = xlRange.Cells[i, j].Value2.ToString();
                                break;
                            case 5:
                                job.Draft = xlRange.Cells[i, j].Value2.ToString();
                                break;
                            case 6:
                                job.Settled = xlRange.Cells[i, j].Value2.ToString();
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }
                    }
                }
                jobs.Add(job);
            }

            Console.Write(jobs.Count);

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);


            var result = (from item in jobs
                          where item.Settled == null
                          group item.Amount by item.Agent into g
                          select new Outstanding
                          {
                              Agent = g.Key,
                              Amount = g.Sum()
                          }) ;
            Console.Write(result);
            foreach(var r in result) {
                Expenses.Add(r);
            }
                
        }


    }
}
