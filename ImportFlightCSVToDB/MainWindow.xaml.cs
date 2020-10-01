using ImportFlightCSVToDB.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ImportFlightCSVToDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private FlightDetailEntities db = new FlightDetailEntities();
        private DispatcherTimer timer1 = new DispatcherTimer();
        private ExcelHelper excel = new ExcelHelper();
        private List<ObjectFull> listObject = new List<ObjectFull>();
        private int count = 0;
        public MainWindow()
        {
            InitializeComponent();

            timer1.Tick += timer1_Tick;
        }

        //public void InsertOrUpdateSquawk(string icao, string squawk)
        //{
        //    var temp = db.FlightIdentities.Where(m => m.ICAO == icao).FirstOrDefault();
        //    if (temp != null)
        //    {
        //        if (!String.IsNullOrEmpty(temp.Callsign))
        //        {
        //            temp.Squawk = squawk;
        //            db.SaveChanges();
        //        }

        //    }
        //    else
        //    {
        //        var ident = new FlightIdentity();
        //        ident.Squawk = squawk;
        //        ident.ICAO = icao;
        //        db.FlightIdentities.Add(ident);
        //        db.SaveChanges();
        //    }
        //}
        public void InsertOrUpdateCallsign(string icao, string callsign)
        {
            using (var db = new FlightDetailEntities())
            {
                var temp = db.FlightIdentities.Where(m => m.ICAO == icao).FirstOrDefault();
                if (temp != null)
                {
                    if (!String.IsNullOrEmpty(temp.Callsign))
                    {
                        temp.Callsign = callsign;
                        db.SaveChanges();
                    }

                }
                else
                {
                    var ident = new FlightIdentity();
                    ident.Callsign = callsign;
                    ident.ICAO = icao;
                    db.FlightIdentities.Add(ident);
                    db.SaveChanges();
                }
            }
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            // code goes here
            //var listTransmissionType = new List<String>();
            FlightPos currentObject = null;
            
            var fileNameInput = Directory.GetFiles(txtSourceFolder.Text).Select(System.IO.Path.GetFullPath).OrderBy(d => new FileInfo(d).CreationTime).ToList();
            if (fileNameInput != null && fileNameInput.Count > 0)
            {
                foreach (var currentItem in fileNameInput)
                {
                    if (count == fileNameInput.Count - 1)
                    {
                        count = 0;
                        break;
                    }
                        
                    listObject = excel.ProcessTxtFile(currentItem);
                    var listPosObject = new List<FlightPos>();
                    if (listObject != null)
                    {
                        foreach (var item in listObject)
                        {
                            try
                            {
                                var trans = item.transmissionType;
                                if (trans == "1")
                                {
                                    currentObject = null;
                                    if (!String.IsNullOrEmpty(item.callsign))
                                        InsertOrUpdateCallsign(item.ICAO, item.callsign);
                                }
                                else if (trans == "3")
                                {
                                    currentObject = new FlightPos();
                                    currentObject.MessageType = item.messageType;
                                    currentObject.TransmissionType = item.transmissionType;
                                    currentObject.ICAO = item.ICAO;
                                    currentObject.DateGenerate = item.dategenerate;
                                    currentObject.DateLog = item.datelog;
                                    currentObject.Altitude = item.altitude;
                                    currentObject.Latitude = item.latitude;
                                    currentObject.Longitude = item.longitude;
                                    //currentTransmissionType = trans;
                                }
                                else if (trans == "4")
                                {
                                    if (currentObject != null)
                                    {
                                        currentObject.Speed = item.speed;
                                        currentObject.Track = item.track;
                                        currentObject.VerticalRate = item.verticalrate;

                                        if (!String.IsNullOrEmpty(currentObject.Altitude))
                                        {
                                            double altitude;
                                            altitude = Convert.ToDouble(currentObject.Altitude) * 0.3048;
                                            currentObject.Altitude = Math.Round(altitude, 2).ToString();
                                        }

                                        if (!String.IsNullOrEmpty(currentObject.Altitude))
                                        {
                                            double speed;
                                            speed = Convert.ToDouble(currentObject.Speed) * 1.6093;
                                            currentObject.Speed = Math.Round(speed, 2).ToString();
                                        }

                                        if (!String.IsNullOrEmpty(currentObject.Altitude))
                                        {
                                            double verticalrate;
                                            verticalrate = Convert.ToDouble(currentObject.VerticalRate) * 0.3048;
                                            currentObject.VerticalRate = Math.Round(verticalrate, 2).ToString();
                                        }

                                        listPosObject.Add(currentObject);

                                        currentObject = null;
                                    }
                                }
                                // transmissionType = 5
                                else
                                {
                                    currentObject = null;
                                    if (!String.IsNullOrEmpty(item.callsign))
                                        InsertOrUpdateCallsign(item.ICAO, item.callsign);
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }
                    }

                    if (listPosObject.Count > 0)
                    {
                        var tempList = listPosObject.OrderByDescending(m => m.DateLog).GroupBy(x => x.ICAO).Select(x => x.First()).ToList();
                        using (var db = new FlightDetailEntities())
                        {
                            db.FlightPos.AddRange(tempList);
                            db.SaveChanges();
                        }    
                    }
                    if (System.IO.File.Exists(currentItem))
                    {
                        //var fileName = System.IO.Path.GetFileName(currentItem);
                        //var desFile = txtDestinationFolder.Text + "\\" + fileName;
                        try
                        {
                            System.IO.File.Delete(currentItem);
                            //System.IO.File.Move(currentItem, desFile);
                        }
                        catch
                        {

                        }
                    }
                    count++;
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        public void ExecuteNonQuery(string SPName)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }

        private string getCnnString()
        {
            string _connStr = String.Format("Data Source={0};Initial Catalog=FlightDetail;Persist Security Info=True;User ID={1};Password=123456", txtServerIP.Text, txtUserID.Text);
            var connDecoded = HttpUtility.HtmlDecode(_connStr);
            return connDecoded;
        }

        public void ExecuteNonQuery(string SPName, List<SqlParameter> list)
        {
            SqlConnection sqlConn = null;
            try
            {
                sqlConn = new SqlConnection(getCnnString());
                sqlConn.Open();
                var sqlCommand = new SqlCommand(SPName, sqlConn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter param in list)
                {
                    sqlCommand.Parameters.Add(param);
                }
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (sqlConn != null)
                {
                    sqlConn.Close();
                }
            }
        }

        private void txtStart_Click(object sender, RoutedEventArgs e)
        {
            var spanTime = Convert.ToInt32(txtCount.Text);
            timer1.Interval = new TimeSpan(0, spanTime, 0);

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            timer1.Start();
        }

        private void txtStop_Click(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            //var temp = "";
            CommonOpenFileDialog dia = new CommonOpenFileDialog();
            dia.IsFolderPicker = true;
            dia.Title = "+++Select Folder+++";
            if (dia.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtSourceFolder.Text = dia.FileName;
            }
        }

        private void btnDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dia = new CommonOpenFileDialog();
            dia.IsFolderPicker = true;
            dia.Title = "+++Select Folder+++";
            if (dia.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtDestinationFolder.Text = dia.FileName;
            }
        }
        private string GetConnectionString()
        {
            string ip = txtServerIP.Text;
            string account = txtUserID.Text;
            string password = txtPassword.Password;
            string _connStr = String.Format("metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source={0};initial catalog=FlightDetail;user id={1};password={2};MultipleActiveResultSets=True;App=EntityFramework&quot;", ip, account, password);
            var connDecoded = HttpUtility.HtmlDecode(_connStr);
            //var tempdb = new AdventureWorks2008R2Entities(connDecoded);
            return connDecoded;
        }
    }
}
