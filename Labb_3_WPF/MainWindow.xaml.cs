using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Labb_3_WPF
{

    public partial class MainWindow : Window
    {
        public List<Booking> bookingList = new List<Booking>();
        public List<CheckDateAndTime> datumLista = new List<CheckDateAndTime>();



        public MainWindow()
        {

            // gör en lista med tider här och sen data binda dem
            AddDates(datumLista);
            InitializeComponent();

        }


        private void BookingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var förNamn = firstNameBox.Text;
                var efterNamn = lastNameBox.Text;
                var namn = $"{förNamn} {efterNamn}";
                var InputTeleNr = phoneBox.Text;
                var teleNr = Regex.Replace(InputTeleNr, @"\s+", "");
                var kalenderDatum = MainCalendar.SelectedDate.Value.Date.ToShortDateString();
                DateOnly datum = DateOnly.Parse(kalenderDatum);
                var tid = TimeChoiceBox.Text.ToString();
                var kön = genderChoiceBox.Text.ToString();
                int bord = int.Parse(tableChoiceBox.Text.ToString());




                if (CheckInputs(förNamn, efterNamn, tid, kön, teleNr) == true)
                {




                    string text = $"Bord: {bord}. Klockan: {tid}. Namn: {namn}. Kön: {kön}. Telefonnummer: {teleNr}. Datum: {kalenderDatum}.";


                    Boka(bord, namn, kön, datum, tid, int.Parse(teleNr), text, datumLista, bookingList);
                    // gör en metod som kan lägga in variabler och gör det till en print text för listboxen

                   




                    firstNameBox.Text = "";
                    lastNameBox.Text = "";
                    TimeChoiceBox.Text = "";
                    genderChoiceBox.Text = "";
                    phoneBox.Text = " ";

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Välj ett tillgängligt Datum");

            }
        }

        private void BookedDays_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            listBx.Items.Clear();
            var DateFromCalendar = BookedDays.SelectedDate.Value.Date.ToShortDateString();
            

            List<string> bokningar = GetTextsFile();


            var regexDateIdentifier = new Regex(@"Datum: " + DateFromCalendar);
            var regexbordIdentifier = new Regex(@"Bord: [1-5]{1}");
         

            var queryTexts = from item in bokningar
                            where regexDateIdentifier.IsMatch(item)
                            where regexbordIdentifier.IsMatch(item)
                            orderby item ascending
                            select $"" +
                            $"{item.Substring(18,6)} {item.Substring(0, 7)} bokad" ;

            foreach (var item in queryTexts)
            {
                listBx.Items.Add(item);
            }

        }
                           

               
        public static void WriteFile(string text)
        {

            using (StreamWriter writeOrder = new StreamWriter("bokningar.log", true))
            {

                writeOrder.WriteLine(text);
            }
        }

        public static List<string> GetTextsFile()
        {
            List<string> texts = new List<string>();


            string line = "";
            using (StreamReader stream = new StreamReader("bokningar.log"))
            {
                while ((line = stream.ReadLine()) != null)
                {
                    texts.Add(line);

                }
            }




            return texts;
        }







        public static void Boka(int bord, string namn, string kön, DateOnly datum, string tid, int nr, string text, List<CheckDateAndTime> datumLista, List<Booking> bookingList)
        {

            bool checkTime;





            foreach (var item in datumLista)
            {
                if (item.datum == datum)
                {
                    for (int i = 0; i < item.Tider.Count; i++)
                    {
                        if (item.Tider[i].tid == tid)
                        {
                            checkTime = CheckTableAvailable(datum, item.Tider[i], bord);

                            if (checkTime == true)
                            {
                                Booking bokning = new Booking(namn, kön, datum, tid, nr, text);
                                bookingList.Add(bokning);
                                WriteFile(text);
                                MessageBox.Show("Din bokning har nu registerats");
                            }
                            else
                            {
                                MessageBox.Show("Bordet är redan taget!");
                            }

                            i = item.Tider.Count;
                        }
                    }
                }

            }
        }




        public static bool CheckTableAvailable(DateOnly datum, Time tid, int bord)
        {
            bool isAvailalbe = true;
            List<string> bokningar = GetTextsFile();
           
       
            //var regexDateIdentifier = new Regex(@$"({datum})");
            //var regexTimeIdentifier = new Regex(@$"({tid.tid})");
            //var regexbordIdentifier = new Regex(@$"({bord})");


            var regexDateIdentifier = new Regex(@"Datum: " + datum.ToString());
            var regexTimeIdentifier = new Regex(@"Klockan: " + tid.tid);
            var regexbordIdentifier = new Regex(@"Bord: " + bord.ToString());
          

            var queryText = from item in bokningar
                            where regexDateIdentifier.IsMatch(item)
                            where regexTimeIdentifier.IsMatch(item)
                            where regexbordIdentifier.IsMatch(item)
                            select item;



            foreach (var item in queryText)
            {

                isAvailalbe = false;
            }

            
            return isAvailalbe;

           
        }


        public static bool CheckInputs(string förNamn, string efterNamn, string tid, string kön, string telefonNr)
        {
            string missingText = "";
            var regexPhone = new Regex("^0[0-9]{9}"); // 0XX XXX XX XX 10 siffor om tid +46 också

            List<string> missingInputs = new List<string>();

            if (förNamn == "") { missingInputs.Add($"Namn:"); }
            if (efterNamn == "") { missingInputs.Add($"Efternamn:"); }
            if (telefonNr == "") { missingInputs.Add($"Telefon nummer:"); }
            if (tid == "") { missingInputs.Add($"Tid:"); }
            if (kön == "") { missingInputs.Add($"kön:"); }

            if (regexPhone.IsMatch(telefonNr) != true)
            {
                MessageBox.Show("Inte skrivit ett riktigt mobilnummer. OBS kan ej starta med +46, använd 0 i starten");
                return false;
            }

            if (missingInputs.Count > 0)
            {
                for (int i = 0; i < missingInputs.Count; i++)
                {
                    missingText += $"{missingInputs[i]} Saknas\n";
                }
                MessageBox.Show($"Du har ej fyllt i alla rutor!\n{missingText}");
                return false;
            }
            else
            {
                return true;
            }




        }


        public static void AddDates(List<CheckDateAndTime> datumLista)
        {
            int daysToFill = 7;
            int startday = 14;
            int month = 11;
            int year = 2022;
            for (int i = 0; i < daysToFill; i++)
            {
                DateOnly date = new DateOnly(year, month, startday + i);
                CheckDateAndTime addDay = new CheckDateAndTime(date);
                datumLista.Add(addDay);
            }
        }




    }


}










  









            
          









            
            
           



































