using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string wiadomosc = "<b><h1>RAPORT NIEDOZWOLONYCH GIER Z DNIA " + DateTime.Today.Day + "." + DateTime.Today.Month + "." + DateTime.Today.Year + " roku.</h1></b><br>";
        int licznikZagrozen = 0;
        BackgroundWorker bgw;
        Stopwatch stoper;
        public Form1()
        {
            InitializeComponent();
            bgw = new BackgroundWorker();
            bgw.WorkerReportsProgress = true;

            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            stoper = new Stopwatch();
            stoper.Start();
            bgw.RunWorkerAsync();
        }
        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (listBox1.Items.Count>0)
            {
                listBox1.Items.Clear();
            }
            int licznikZnalezionych = 0;
            var bezDuplikatow = Rejestry();
            int ileZnalezionych = bezDuplikatow.Count;

            string sciezkaBazy = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True";
            //string sciezkaBazy = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\WFtests\WindowsFormsApplication1\WindowsFormsApplication1\bin\Debug\Database1.mdf; Integrated Security = True; Connect Timeout = 30";

            using (SqlConnection polaczenieZBaza = new SqlConnection(sciezkaBazy))
            {
                polaczenieZBaza.Open();
                using (var wszystkoZBazy = new SqlCommand(@"Select * from [dbo].[Table]", polaczenieZBaza))
                {
                    SqlDataReader ExReader = wszystkoZBazy.ExecuteReader();

                        foreach (Rejestr rej in bezDuplikatow)
                        {
                            licznikZnalezionych++;
                            while (ExReader.Read())
                            {
                                if (rej.nazwaProgramu == ExReader[1].ToString())
                                {
                                    licznikZagrozen++;
                                    label1.Text = licznikZagrozen.ToString();
                                    wiadomosc += "<br><p style=\"font-size:14px\"><u>Niedozwolona gra numer " + 
                                        licznikZagrozen + ":" + "</u>" + " " + "Nazwa: " + "<b>" +
                                        rej.nazwaProgramu + "</b>, producent: " + "<b>" + 
                                        rej.wydawca + "</b></p>";
                                    wiadomosc += "<ul><li><u>Ścieżka programu na przeskanowanym komputerze:</u> " + "<b>" + 
                                        rej.sciezkaProgramu + "</b><br></li>";
                                    wiadomosc += "<li><u>Ścieżka dezinstalacji programu na przeskanowanym komputerze:</u> " + "<b>" + 
                                        rej.sciezkaDezinstalacji + "</b><br></li></ul>";                        
                                }
                                else if (rej.wydawca == ExReader[2].ToString())
                                {
                                    licznikZagrozen++;
                                    label1.Text = licznikZagrozen.ToString();
                                    wiadomosc += "<br><p style=\"font-size:14px\"><u>Niedozwolona gra numer " + 
                                        licznikZagrozen + ":" + "</u>" + " " + "Nazwa: " + "<b>" + 
                                        rej.nazwaProgramu + "</b>, producent: " + "<b>" + 
                                        rej.wydawca + "</b></p>";
                                    wiadomosc += "<ul><li><u>Ścieżka programu na przeskanowanym komputerze:</u> " + "<b>" + 
                                        rej.sciezkaProgramu + "</b><br></li>";
                                    wiadomosc += "<li><u>Ścieżka dezinstalacji programu na przeskanowanym komputerze:</u> " + "<b>" + 
                                        rej.sciezkaDezinstalacji + "</b><br></li></ul>";
                                }
                                bgw.ReportProgress(licznikZnalezionych * 100 / ileZnalezionych);
                            }
                            ExReader.Close();
                            ExReader = wszystkoZBazy.ExecuteReader();
                            label2.Text = licznikZnalezionych + "/" + ileZnalezionych;
                            listBox1.Items.Add(rej.nazwaProgramu);
                            listBox1.Update();
                            listBox1.SelectedIndex = listBox1.Items.Count - 1;   
                            label2.Update();
                            label1.Update();
                            Thread.Sleep(15);
                        }
                    ExReader.Close();
                }
                polaczenieZBaza.Close();
            }
            stoper.Stop();
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (licznikZagrozen == 0)
            {
                TimeSpan czasWykonania = stoper.Elapsed;
                string czasW = String.Format("{0:00}:{1:00}",czasWykonania.Minutes,czasWykonania.Seconds);
                MessageBox.Show("Skanowanie zakończone! Nie wykryto niebezpiecznych gier.\n\nCzas skanowania (minuty:sekundy): " + czasW);
            }
               
            else
            {
                TimeSpan czasWykonania = stoper.Elapsed;
                string czasW = String.Format("{0:00}:{1:00}", czasWykonania.Minutes, czasWykonania.Seconds);
                MessageBox.Show("Skanowanie zakończone! Raport jest gotowy do wysłania.\n\nCzas skanowania (minuty:sekundy): " + czasW);
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                label5.Text = "Trwa wysyłanie maila...";
                label5.Update();
                wyslijMaila("mareksyp@gmail.com", wiadomosc);
                label5.Text = "Mail z raportem został wysłany pomyślnie.";
                label5.Update();
            }
            catch (Exception ss)
            {
                MessageBox.Show(ss.ToString());
                label5.Text = "Wystąpił błąd podczas wysłania maila.";
                label5.Update();
            }
        
        }        
    }
}
