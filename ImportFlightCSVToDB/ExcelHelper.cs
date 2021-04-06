using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ImportFlightCSVToDB.ObjectModel;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace ImportFlightCSVToDB
{
    public class ExcelHelper
    {

        public List<ObjectFull> ProcessTxtFile(string filePath)
        {
            var tempList = new List<ObjectFull>();
            if (File.Exists(filePath))
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!String.IsNullOrEmpty(line))
                        {
                            try
                            {
                                var listValues = line.Split(',');
                               
                                string msgtype = ""; string transmissiontype = "";
                                    
                                msgtype = listValues[0];
                                transmissiontype = listValues[1];
                                if (transmissiontype.Trim() == "2" || transmissiontype.Trim() == "6" || transmissiontype.Trim() == "7" || transmissiontype.Trim() == "8")
                                {
                                    continue;
                                }
                                string icao = "";
                                string tempdategenerate = "";
                                string temptimegenerate = "";
                                string tempdatelog = "";
                                string temptimelog = "";
                                string callsign = "";
                                string tempaltitude = ""; string tempspeed = ""; string track = ""; string latitude = "";
                                string longitude = "";
                                string tempverticalrate = ""; string squawk = "";

                                icao = listValues[4];
                                tempdategenerate = listValues[6];
                                temptimegenerate = listValues[7];

                                tempdatelog = listValues[8];
                                temptimelog = listValues[9];
                                callsign = listValues[10];
                                tempaltitude = listValues[11];
                                tempspeed = listValues[12];
                                track = listValues[13];

                                latitude = listValues[14];
                                longitude = listValues[15];
                                tempverticalrate = listValues[16];
                                squawk = listValues[17];



                                var s = tempdategenerate + " " + temptimegenerate;
                                //DateTime dt = DateTime.ParseExact(s, "yyyy/MM/dd HH:mm:tt.ss", CultureInfo.InvariantCulture);

                                DateTime dateGenerate = DateTime.ParseExact(tempdategenerate + " " + temptimegenerate, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                                DateTime dateLog = DateTime.ParseExact(tempdatelog + " " + temptimelog, "yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);


                                var newTargetObject = new ObjectFull();
                                newTargetObject.ICAO = icao;
                                newTargetObject.callsign = callsign;
                                newTargetObject.altitude = tempaltitude;
                                newTargetObject.speed = tempspeed;
                                newTargetObject.dategenerate = dateGenerate;
                                newTargetObject.datelog = dateLog;
                                newTargetObject.messageType = msgtype;
                                newTargetObject.transmissionType = transmissiontype;

                                newTargetObject.track = track;
                                newTargetObject.latitude = latitude;
                                newTargetObject.longitude = longitude;
                                newTargetObject.verticalrate = tempverticalrate;
                                newTargetObject.squawk = squawk;
                                tempList.Add(newTargetObject);

                                

                            }
                            catch
                            {

                            }

                        }
                    }
                }
            }
            return tempList;
        }
        public List<ObjectFull> ProcessFile(string filePath)
        {
            Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;
            var fileName = Path.GetFileName(filePath);
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            object[,] values = xlRange.Value2;
            if (values == null)
            {
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
                return null;
                
            }
                
          
                var tempList = new List<ObjectFull>();
            for (int i = 1; i <= rowCount; i++)
            {

                try
                {
                   
                    string msgtype = ""; string transmissiontype = "";
                    string icao = "";
                    string tempdategenerate = "";
                    string temptimegenerate = "";
                    string tempdatelog = "";
                    string temptimelog = "";
                    string callsign = "";
                    string altitude = ""; string speed = ""; string track = ""; string latitude = "";
                    string longitude = "";
                    string verticalrate = ""; string squawk = "";
                    var te = xlRange.Cells[i, 1];
                    var xp = xlRange.Cells[i, 1].Value2;

                    msgtype = Convert.ToString(values[i, 1]);
                    transmissiontype = Convert.ToString(values[i, 2]);
                    if (transmissiontype.Trim() == "2" || transmissiontype.Trim() == "7" || transmissiontype.Trim() == "8")
                    {
                        continue;
                    }
                    icao = Convert.ToString(values[i, 5]);
                    tempdategenerate = Convert.ToString(values[i, 7]);
                    temptimegenerate = Convert.ToString(values[i, 8]);

                    tempdatelog = Convert.ToString(values[i, 9]);
                    temptimelog = Convert.ToString(values[i, 10]);
                    callsign = Convert.ToString(values[i, 11]);
                    altitude = Convert.ToString(values[i, 12]);
                    speed = Convert.ToString(values[i, 13]);
                    track = Convert.ToString(values[i, 14]);

                    latitude = Convert.ToString(values[i, 15]);
                    longitude = Convert.ToString(values[i, 16]);
                    verticalrate = Convert.ToString(values[i, 17]);
                    squawk = Convert.ToString(values[i, 18]);

             
                    double d1 = double.Parse(tempdategenerate);
                    DateTime dg = DateTime.FromOADate(d1);

                    double t1 = double.Parse(temptimegenerate);
                    DateTime tg = DateTime.FromOADate(t1);

                    double d2 = double.Parse(tempdatelog);
                    DateTime dl = DateTime.FromOADate(d2);

                    double t2 = double.Parse(temptimelog);
                    DateTime tl = DateTime.FromOADate(t2);

                    DateTime dateGenerate = new DateTime(dg.Year, dg.Month, dg.Day, tg.Hour, tg.Minute, tg.Second);
                    
                    DateTime dateLog = new DateTime(dl.Year, dl.Month, dl.Day, tl.Hour, tl.Minute, tl.Second);


                    var newTargetObject = new ObjectFull();
                    newTargetObject.ICAO = icao;
                    newTargetObject.callsign = callsign;
                    newTargetObject.altitude = altitude;
                    newTargetObject.speed = speed;
                    newTargetObject.dategenerate = dateGenerate;
                    newTargetObject.datelog = dateLog;
                    newTargetObject.messageType = msgtype;
                    newTargetObject.transmissionType = transmissiontype;

                    newTargetObject.track = track;
                    newTargetObject.latitude = latitude;
                    newTargetObject.longitude = longitude;
                    newTargetObject.verticalrate = verticalrate;
                    newTargetObject.squawk = squawk;
                    tempList.Add(newTargetObject);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.InnerException.Message);
                    Console.WriteLine(ex.Message);
                }


            }

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

            return tempList;
        }


    }
}
