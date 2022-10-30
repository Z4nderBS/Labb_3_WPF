using System;
using System.Collections.Generic;
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




        public MainWindow()
        {

            

            // gör en lista med tider här och sen data binda dem
            AddDates(datumLista);
            PreMadeBookings(datumLista);
            InitializeComponent();
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


            List<string> bokningar = GetTextsFile();


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

                                WriteFile(text);
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


        public static void Boka(string bord, string namn, string kön, DateOnly datum, string tid, string nr, string text, List<DateAndTime> datumLista, string custom) // bokning för färdiga bokningar.
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

                                WriteFile(text);


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
            List<string> bokningar = GetTextsFile();


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
        public static void PreMadeBookings(List<DateAndTime> datumLista)
        {
            List<string> bokningar = GetTextsFile();

            if (bokningar.Count == 0)
            {
                List<Woman> kvinnor = new List<Woman>();
                List<Man> män = new List<Man>();


                Woman kvinna = new Woman("Alcicia Eriksson", new DateOnly(2022, 11, 15), "21.00", "0734058765", "1");
                Woman kvinna2 = new Woman("Birgitta Svensson", new DateOnly(2022, 11, 15), "21.00", "0734058765", "2");
                Woman kvinna3 = new Woman("Anna Asklund", new DateOnly(2022, 11, 15), "21.00", "0734058765", "3");
                Woman kvinna4 = new Woman("Magdalena Olofsson", new DateOnly(2022, 11, 15), "21.00", "0734058765", "4");
                Woman kvinna5 = new Woman("Jessica Andersson", new DateOnly(2022, 11, 15), "21.00", "0734058765", "5");

                kvinnor.Add(kvinna);
                kvinnor.Add(kvinna2);
                kvinnor.Add(kvinna3);
                kvinnor.Add(kvinna4);
                kvinnor.Add(kvinna5);

                Man man = new Man("Olaf Persson", new DateOnly(2022, 11, 15), "19.00", "0734058765", "1");
                Man man2 = new Man("Olaf Persson", new DateOnly(2022, 11, 15), "19.00", "0734058765", "2");
                Man man3 = new Man("Olaf Persson", new DateOnly(2022, 11, 15), "19.00", "0734058765", "3");
                Man man4 = new Man("Olaf Persson", new DateOnly(2022, 11, 15), "19.00", "0734058765", "4");
                Man man5 = new Man("Olaf Persson", new DateOnly(2022, 11, 15), "19.00", "0734058765", "5");

                män.Add(man);
                män.Add(man2);
                män.Add(man3);
                män.Add(man4);
                män.Add(man);


                foreach (var person in kvinnor)
                {
                    Boka(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, datumLista, "custom");
                }

                foreach (var person in män)
                {
                    Boka(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, datumLista, "custom");
                }
            }



        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            List<string> bokningar = GetTextsFile();
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
                WriteFile(item);
            }

            MessageBox.Show("Din bokning har nu tagits bort");
            listBx.Items.Clear();
            
        }
    }
}
 



            

            





            












































































