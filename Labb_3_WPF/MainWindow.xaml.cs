using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        public List<DateAndTime> datumLista = new List<DateAndTime>();

        public List<string> tider { get; set; }
        public List<string> tables { get; set; }


        public MainWindow()
        {
            // gör en lista med tider här och sen data binda dem

            var watch = Stopwatch.StartNew();
            InitializeComponent();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds.ToString();
            MessageBox.Show(elapsedMs + "milisekunder");
            AddDates(datumLista);
          
            PreMadeBookings(datumLista);
           


            tider = new List<string>() { "16.00", "17.00", "18.00", "19.00", "20.00", "21.00"};
            tables = new List<string>() {"1","2","3","4","5"};

            this.DataContext = this;

            CancelOrder.Visibility = Visibility.Collapsed;
           

           


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
                var bord = tableChoiceBox.Text.ToString();






                if (CheckInputs(förNamn, efterNamn, tid, kön, teleNr, bord) == true)
                {




                    string text = $"Bord: {bord}. Klockan: {tid}. Namn: {namn}. Kön: {kön}. Telefonnummer: {teleNr}. Datum: {kalenderDatum}.*"; // * markerar att det är din bokning


                    Boka(bord, namn, kön, datum, tid, teleNr, text, datumLista);







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


            List<string> bokningar = Filehandler.GetTextsFile();


            var regexDateIdentifier = new Regex(@"Datum: " + DateFromCalendar);
            var regexbordIdentifier = new Regex(@"Bord: [1-5]{1}");


            var queryTexts = from item in bokningar
                             where regexDateIdentifier.IsMatch(item)
                             where regexbordIdentifier.IsMatch(item)
                             orderby item ascending
                             select $"" +
                             $"{item.Substring(0, 7)} bokad {item.Substring(18, 6)} ";

            foreach (var item in queryTexts)
            {
                listBx.Items.Add(item);
            }

        }





        public static void Boka(string bord, string namn, string kön, DateOnly datum, string tid, string nr, string text, List<DateAndTime> datumLista) // bokning för användaren
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

                                Filehandler.WriteFile(text);
                                MessageBox.Show("din bokning har registrerats");


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


        public static async Task Boka(string bord, string namn, string kön, DateOnly datum, string tid, string nr, string text, List<DateAndTime> datumLista, string custom) // bokning för färdiga bokningar.
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

                                await Filehandler.WriteFileAsync(text);


                            }
                            else
                            {

                            }

                            i = item.Tider.Count;
                        }
                    }
                }


            }

     


        }





        public static bool CheckTableAvailable(DateOnly datum, Time tid, string bord)
        {
            bool isAvailalbe = true;
            List<string> bokningar = Filehandler.GetTextsFile();


            var regexDateIdentifier = new Regex(@"Datum: " + datum.ToString());
            var regexTimeIdentifier = new Regex(@"Klockan: " + tid.tid);
            var regexbordIdentifier = new Regex(@"Bord: " + bord);


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


        public static bool CheckInputs(string förNamn, string efterNamn, string tid, string kön, string telefonNr, string bord)
        {
            string missingText = "";
            var regexPhone = new Regex("^0[0-9]{9}"); // 0XX XXX XX XX 10 siffor om tid +46 också

            List<string> missingInputs = new List<string>();

            if (förNamn == "") { missingInputs.Add($"Namn:"); }
            if (efterNamn == "") { missingInputs.Add($"Efternamn:"); }
            if (telefonNr == "") { missingInputs.Add($"Telefon nummer:"); }
            if (tid == "") { missingInputs.Add($"Tid:"); }
            if (kön == "") { missingInputs.Add($"kön:"); }
            if (bord == "") { missingInputs.Add("Bord:"); }

            if (missingInputs.Count > 0)
            {
                for (int i = 0; i < missingInputs.Count; i++)
                {
                    missingText += $"{missingInputs[i]} Saknas\n";
                }
                MessageBox.Show($"Du har ej fyllt i alla rutor!\n{missingText}");
                return false;
            }

            if (regexPhone.IsMatch(telefonNr) != true)
            {
                MessageBox.Show("Inte skrivit ett riktigt mobilnummer. OBS kan ej starta med +46, använd 0 i starten");
                return false;
            }


            else
            {
                return true;
            }




        }


        public static void AddDates(List<DateAndTime> datumLista)
        {
            int daysToFill = 7;
            int startday = 14;
            int month = 11;
            int year = 2022;
            for (int i = 0; i < daysToFill; i++)
            {
                DateOnly date = new DateOnly(year, month, startday + i);
                DateAndTime addDay = new DateAndTime(date);
                datumLista.Add(addDay);
            }
        }
       

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            List<string> bokningar = Filehandler.GetTextsFile();
            listBx.Items.Clear();


            var regexUserIdentifier = new Regex(@"(\*)");

            var queryText = from item in bokningar
                            where regexUserIdentifier.IsMatch(item)
                            select item.ToString();

            foreach (var item in queryText)
            {
                CancelOrder.Visibility = Visibility.Visible;
                listBx.Items.Add(item);

            }
            if (listBx.Items.Count == 0)
            {

                MessageBox.Show("Du har inga registrerade bokningar.");
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {

            string textToRemove = listBx.SelectedItem.ToString();
            var filePath = "bokningar.log";

            List<string> textToKeep = new List<string>();

            using (var sr = new StreamReader(filePath))

            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line != textToRemove)
                    {
                        textToKeep.Add(line);
                    }

                }
            }

            File.Delete(filePath);



            foreach (var item in textToKeep)
            {
                Filehandler.WriteFile(item);
            }

            MessageBox.Show("Din bokning har nu tagits bort");
            CancelOrder.Visibility = Visibility.Collapsed;
            listBx.Items.Clear();


        }

        public static async Task PreMadeBookings(List<DateAndTime> datumLista)
        {
            List<string> bokningar = await Filehandler.GetTextsFileAsync();

            if (bokningar.Count == 1)
            {
                List<Woman> kvinnor = new List<Woman>();
                List<Man> män = new List<Man>();

                // olika namn att använda
                string nameWoman_1 = "Alcicia Eriksson";
                string nameWoman_2 = "Birgitta Svensson";
                string nameWoman_3 = "Anna Asklund";
                string nameWoman_4 = "Magdalena Olofsson";
                string nameWoman_5 = "Jessica Andersson";

                string nameMan_1 = "Olaf Persson";
                string nameMan_2 = "Rickard Svensson";
                string nameMan_3 = "Elias Ingmarsson";
                string nameMan_4 = "Kevin Andersson";
                string nameMan_5 = "Erik Samuelsson";

                DateOnly nov_14 = new DateOnly(2022, 11, 14);
                DateOnly nov_15 = new DateOnly(2022, 11, 15);
                DateOnly nov_16 = new DateOnly(2022, 11, 16);
                DateOnly nov_17 = new DateOnly(2022, 11, 17);
                DateOnly nov_18 = new DateOnly(2022, 11, 18);
                DateOnly nov_19 = new DateOnly(2022, 11, 19);
                DateOnly nov_20 = new DateOnly(2022, 11, 20);

                string time_16 = "16.00";
                string time_17 = "17.00";
                string time_18 = "18.00";
                string time_19 = "19.00";
                string time_20 = "20.00";
                string time_21 = "21.00";

                string phoneNr_1 = "0734058765";
                string phoneNr_2 = "0737659215";
                string phoneNr_3 = "0705678321";
                string phoneNr_4 = "0750982543";
                string phoneNr_5 = "0734078234";

             



                // 14 November 2022
                kvinnor.Add(new Woman(nameWoman_1, nov_14, time_16, phoneNr_1, "1"));
                män.Add(new Man(nameMan_1, nov_14, time_16, phoneNr_5, "3"));
                män.Add(new Man(nameMan_2, nov_14, time_16, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_3, nov_14, time_17, phoneNr_4, "1"));
                män.Add(new Man(nameMan_4, nov_14, time_17, phoneNr_5, "2"));
                män.Add(new Man(nameMan_3, nov_14, time_17, phoneNr_5, "3"));
                män.Add(new Man(nameMan_1, nov_14, time_18, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_14, time_18, phoneNr_3, "2"));
                män.Add(new Man(nameMan_4, nov_14, time_18, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_5, nov_14, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_14, time_19, phoneNr_3, "3"));
                kvinnor.Add(new Woman(nameWoman_2, nov_14, time_19, phoneNr_5, "4"));
                män.Add(new Man(nameMan_4, nov_14, time_20, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_3, nov_14, time_20, phoneNr_3, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_14, time_21, phoneNr_3, "3"));
                män.Add(new Man(nameMan_5, nov_14, time_21, phoneNr_1, "1"));
                män.Add(new Man(nameMan_2, nov_14, time_21, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_14, time_21, phoneNr_3, "5"));
             
               

               






                // 15 November 2022
                kvinnor.Add(new Woman(nameWoman_1, nov_15, time_16, phoneNr_1, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_15, time_17, phoneNr_4, "1"));
                kvinnor.Add(new Woman(nameWoman_2, nov_15, time_17, phoneNr_2, "2"));
                kvinnor.Add(new Woman(nameWoman_4, nov_15, time_17, phoneNr_3, "3"));
                män.Add(new Man(nameMan_3, nov_15, time_17, phoneNr_5, "4"));
                män.Add(new Man(nameMan_1, nov_15, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_15, time_18, phoneNr_3, "1"));
                män.Add(new Man(nameMan_4, nov_15, time_18, phoneNr_5, "2"));
                män.Add(new Man(nameMan_2, nov_15, time_18, phoneNr_2, "3"));
                män.Add(new Man(nameMan_3, nov_15, time_18, phoneNr_3, "4"));
                män.Add(new Man(nameMan_5, nov_15, time_18, phoneNr_1, "5"));
                kvinnor.Add(new Woman(nameWoman_3, nov_15, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_15, time_19, phoneNr_5, "4"));
                män.Add(new Man(nameMan_4, nov_15, time_20, phoneNr_5, "1"));
                män.Add(new Man(nameMan_2, nov_15, time_20, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_15, time_20, phoneNr_3, "4"));
                män.Add(new Man(nameMan_5, nov_15, time_21, phoneNr_1, "1"));
         
               
            
               





                // 16 November 2022
                kvinnor.Add(new Woman(nameWoman_2, nov_16, time_16, phoneNr_2, "5"));
                kvinnor.Add(new Woman(nameWoman_3, nov_16, time_17, phoneNr_4, "1"));
                män.Add(new Man(nameMan_3, nov_16, time_17, phoneNr_5, "3"));
                män.Add(new Man(nameMan_1, nov_16, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_16, time_18, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_3, nov_16, time_18, phoneNr_3, "3"));
                kvinnor.Add(new Woman(nameWoman_3, nov_16, time_18, phoneNr_3, "4"));
                män.Add(new Man(nameMan_3, nov_16, time_18, phoneNr_5, "5"));
                kvinnor.Add(new Woman(nameWoman_5, nov_16, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_16, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_16, time_19, phoneNr_5, "4"));
                män.Add(new Man(nameMan_4, nov_16, time_20, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_1, nov_16, time_21, phoneNr_1, "1"));
                män.Add(new Man(nameMan_5, nov_16, time_21, phoneNr_1, "2"));
                män.Add(new Man(nameMan_2, nov_16, time_21, phoneNr_2, "4"));
                män.Add(new Man(nameMan_3, nov_16, time_21, phoneNr_3, "5"));
              
       
              

     

                // 17 November 2022
                kvinnor.Add(new Woman(nameWoman_1, nov_17, time_16, phoneNr_1, "1"));
                kvinnor.Add(new Woman(nameWoman_2, nov_17, time_16, phoneNr_2, "2"));
                kvinnor.Add(new Woman(nameWoman_5, nov_17, time_16, phoneNr_5, "3"));
                kvinnor.Add(new Woman(nameWoman_3, nov_17, time_16, phoneNr_3, "4"));
                män.Add(new Man(nameMan_1, nov_17, time_16, phoneNr_5, "3"));
                kvinnor.Add(new Woman(nameWoman_3, nov_17, time_17, phoneNr_4, "1"));
                kvinnor.Add(new Woman(nameWoman_1, nov_17, time_17, phoneNr_1, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_17, time_17, phoneNr_2, "3"));
                män.Add(new Man(nameMan_3, nov_17, time_17, phoneNr_5, "4"));
                män.Add(new Man(nameMan_1, nov_17, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_17, time_18, phoneNr_3, "1"));
                män.Add(new Man(nameMan_2, nov_17, time_18, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_17, time_18, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_5, nov_17, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_17, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_17, time_19, phoneNr_5, "4"));
                män.Add(new Man(nameMan_4, nov_17, time_20, phoneNr_5, "4"));
                män.Add(new Man(nameMan_5, nov_17, time_21, phoneNr_1, "1"));
                män.Add(new Man(nameMan_2, nov_17, time_21, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_17, time_21, phoneNr_3, "5"));
              





                // 18 November 2022
                kvinnor.Add(new Woman(nameWoman_1, nov_18, time_16, phoneNr_1, "1"));
                kvinnor.Add(new Woman(nameWoman_2, nov_18, time_16, phoneNr_2, "2"));
                män.Add(new Man(nameMan_1, nov_18, time_16, phoneNr_5, "3"));
                män.Add(new Man(nameMan_2, nov_18, time_16, phoneNr_5, "4"));
                män.Add(new Man(nameMan_3, nov_18, time_16, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_17, phoneNr_4, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_17, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_18, time_17, phoneNr_5, "4"));
                män.Add(new Man(nameMan_3, nov_18, time_17, phoneNr_5, "3"));
                män.Add(new Man(nameMan_1, nov_18, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_18, time_18, phoneNr_3, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_18, phoneNr_1, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_18, time_18, phoneNr_2, "4"));
                kvinnor.Add(new Woman(nameWoman_5, nov_18, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_18, time_19, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_1, nov_18, time_20, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_20, phoneNr_3, "2"));
                män.Add(new Man(nameMan_4, nov_18, time_20, phoneNr_5, "3"));
                män.Add(new Man(nameMan_2, nov_18, time_20, phoneNr_2, "5"));
                kvinnor.Add(new Woman(nameWoman_5, nov_18, time_21, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_18, time_21, phoneNr_3, "2"));
                män.Add(new Man(nameMan_5, nov_18, time_21, phoneNr_1, "3"));
                män.Add(new Man(nameMan_2, nov_18, time_21, phoneNr_2, "4"));
                män.Add(new Man(nameMan_3, nov_18, time_21, phoneNr_3, "5"));

              
               

                // 19 November 2022
                kvinnor.Add(new Woman(nameWoman_1, nov_19, time_16, phoneNr_1, "1"));
                kvinnor.Add(new Woman(nameWoman_2, nov_19, time_16, phoneNr_2, "2"));
                män.Add(new Man(nameMan_1, nov_19, time_16, phoneNr_5, "3"));
                kvinnor.Add(new Woman(nameWoman_3, nov_19, time_17, phoneNr_4, "1"));
                män.Add(new Man(nameMan_2, nov_19, time_17, phoneNr_5, "4"));
                män.Add(new Man(nameMan_3, nov_19, time_17, phoneNr_5, "3"));
                män.Add(new Man(nameMan_1, nov_19, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_4, nov_19, time_18, phoneNr_3, "1"));
                män.Add(new Man(nameMan_2, nov_19, time_18, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_19, time_18, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_5, nov_19, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_19, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_19, time_19, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_1, nov_19, time_20, phoneNr_1, "1"));
                män.Add(new Man(nameMan_4, nov_19, time_20, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_3, nov_19, time_21, phoneNr_3, "3"));
                kvinnor.Add(new Woman(nameWoman_2, nov_19, time_21, phoneNr_5, "4"));
                män.Add(new Man(nameMan_5, nov_19, time_21, phoneNr_1, "1"));
                män.Add(new Man(nameMan_2, nov_19, time_21, phoneNr_2, "2"));
                män.Add(new Man(nameMan_3, nov_19, time_21, phoneNr_3, "5"));


             

                // 20 November 2022
                män.Add(new Man(nameMan_1, nov_20, time_16, phoneNr_5, "3"));
                män.Add(new Man(nameMan_2, nov_20, time_16, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_3, nov_20, time_17, phoneNr_4, "1"));
                män.Add(new Man(nameMan_3, nov_20, time_17, phoneNr_5, "3"));
                män.Add(new Man(nameMan_1, nov_20, time_17, phoneNr_3, "5"));
                kvinnor.Add(new Woman(nameWoman_1, nov_20, time_18, phoneNr_1, "1"));
                kvinnor.Add(new Woman(nameWoman_2, nov_20, time_18, phoneNr_2, "2"));
                kvinnor.Add(new Woman(nameWoman_4, nov_20, time_18, phoneNr_3, "3"));
                kvinnor.Add(new Woman(nameWoman_5, nov_20, time_19, phoneNr_5, "1"));
                kvinnor.Add(new Woman(nameWoman_3, nov_20, time_19, phoneNr_3, "2"));
                kvinnor.Add(new Woman(nameWoman_2, nov_20, time_19, phoneNr_5, "4"));
                män.Add(new Man(nameMan_4, nov_20, time_20, phoneNr_5, "4"));
                kvinnor.Add(new Woman(nameWoman_3, nov_20, time_21, phoneNr_3, "5"));
                män.Add(new Man(nameMan_5, nov_20, time_21, phoneNr_1, "1"));
            

            
             
       









                foreach (var person in kvinnor)
                {
                    await Boka(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, datumLista, "custom");
                }

                foreach (var person in män)
                {
                    await Boka(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, datumLista, "custom");
                }
            }



        }


    }
}

























































































