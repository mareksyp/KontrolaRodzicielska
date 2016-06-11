using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public partial class Form1
    {
        static List<Rejestr> Rejestry()
        {
            string nazwa, prod, scie1, scie2;
            List<Rejestr> listaWydawcow = new List<Rejestr>();

            string[] zainstalowaneProgramy = { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };

            var rej32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                     RegistryView.Registry32);

            var rej64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                     RegistryView.Registry64);

            RegistryKey[] rejB = { rej32, rej64 };
            foreach (RegistryKey rejestr3264 in rejB)
            {
                foreach (string nazw in zainstalowaneProgramy)
                {
                    using (RegistryKey kluczeRej = rejestr3264.OpenSubKey(nazw))
                    {
                        foreach (string nazwaKlucza in kluczeRej.GetSubKeyNames())
                        {
                            using (RegistryKey daneOKluczu = kluczeRej.OpenSubKey(nazwaKlucza))
                            {
                                if (daneOKluczu.GetValue("DisplayName") != null)
                                {
                                    if (daneOKluczu.GetValue("DisplayName") == "")
                                    {
                                        nazwa = "Brak danych";
                                    }
                                    else
                                    {
                                        nazwa = daneOKluczu.GetValue("DisplayName").ToString();
                                    }
                                    if (daneOKluczu.GetValue("Publisher") == "" || daneOKluczu.GetValue("Publisher") == null)
                                    {
                                        prod = "Brak danych";
                                    }
                                    else
                                    {
                                        prod = daneOKluczu.GetValue("Publisher").ToString();
                                    }
                                    if (daneOKluczu.GetValue("InstallLocation") == "" || daneOKluczu.GetValue("InstallLocation") == null)
                                    {
                                        scie1 = "Brak danych";
                                    }
                                    else
                                    {
                                        scie1 = daneOKluczu.GetValue("InstallLocation").ToString();
                                    }
                                    if (daneOKluczu.GetValue("UninstallString") == "" || daneOKluczu.GetValue("UninstallString") == null)
                                    {
                                        scie2 = "Brak danych";
                                    }
                                    else
                                    {
                                        scie2 = daneOKluczu.GetValue("UninstallString").ToString();
                                        if (!scie2.Contains(":\\"))
                                        {
                                            scie2 = "Brak danych";
                                        }
                                    }

                                    listaWydawcow.Add(new Rejestr(nazwa, prod, scie1, scie2));
                                }
                            }
                        }
                    }
                }
            }
            var bezDuplikoatow = listaWydawcow.GroupBy(x => x.nazwaProgramu).Select(y => y.FirstOrDefault()).ToList();
            return bezDuplikoatow;
        }

        public static void wyslijMaila(string doKogoWysyla, string trescMaila)
        {
            var wiadomosc = new MailMessage();
            wiadomosc.From = new MailAddress("kontrola@rodzicielska.noip.me", "Kontrola Rodzicielska");
            wiadomosc.To.Add(new MailAddress(doKogoWysyla));
            wiadomosc.IsBodyHtml = true;
            wiadomosc.Subject = "Raport dzienny wykrytych zagrożeń";
            wiadomosc.Body = trescMaila;

            var smpt = new SmtpClient();
            smpt.EnableSsl = false;
            smpt.UseDefaultCredentials = false;
            smpt.Credentials = new NetworkCredential("kontrola@rodzicielska.noip.me", "password");
            smpt.Host = "rodzicielska.noip.me";
            smpt.Port = 25;
            smpt.DeliveryMethod = SmtpDeliveryMethod.Network;
            smpt.Send(wiadomosc);
        }
    }
    }

