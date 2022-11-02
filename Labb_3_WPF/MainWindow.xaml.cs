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
        public List<DateAndTime> dates = new List<DateAndTime>();

        public List<string> times { get; set; }
        public List<string> tables { get; set; }


        public MainWindow()
        {
            // gör en lista med tider här och sen data binda dem

            
            InitializeComponent();
         
            AddDates(dates);
          
            PreMadeBookings(dates);



            CancelOrder.Visibility = Visibility.Collapsed;
            times = new List<string>() { "16.00", "17.00", "18.00", "19.00", "20.00", "21.00"};
            tables = new List<string>() {"1","2","3","4","5"};

            this.DataContext = this;

            
           
        }

           





 

        private void BookingBtn_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                var firstName = firstNameBox.Text;
                var lastName = lastNameBox.Text;
                var fullName = $"{firstName} {lastName}";
                var getPhoneNr = phoneBox.Text;
                var phoneNr = Regex.Replace(getPhoneNr, @"\s+", "");
                var getDate = MainCalendar.SelectedDate.Value.Date.ToShortDateString();
                DateOnly date = DateOnly.Parse(getDate);
                var time = TimeChoiceBox.Text.ToString();
                var gender = genderChoiceBox.Text.ToString();
                var table = tableChoiceBox.Text.ToString();






                if (CheckInputs(firstName, lastName, time, gender, phoneNr, table) == true)
                {




                    string text = $"Bord: {table}. Klockan: {time}. Namn: {fullName}. Kön: {gender}. Telefonnummer: {phoneNr}. Datum: {getDate}.*"; // * markerar att det är din bokning


                    ReservTable(table, fullName, gender, date, time, phoneNr, text, dates);







                    tableChoiceBox.Text = "";
                    TimeChoiceBox.Text = "";
                   
                   

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
            var getDate = BookedDays.SelectedDate.Value.Date.ToShortDateString();

            var regexDateIdentifier = new Regex(@"Datum: " + getDate);
            var regexbordIdentifier = new Regex(@"Bord: [1-5]{1}");

            List<string> bokningar = Filehandler.GetTextsFile();


            var queryMatchedText = from text in bokningar
                             where regexDateIdentifier.IsMatch(text)
                             where regexbordIdentifier.IsMatch(text)
                             orderby text ascending
                             select $"" +
                             $"{text.Substring(0, 7)} bokad {text.Substring(18, 6)} ";

            foreach (var queryText in queryMatchedText)
            {
                listBx.Items.Add(queryText);
            }
           




         
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            List<string> reservations = Filehandler.GetTextsFile();
            listBx.Items.Clear();


            var regexUserIdentifier = new Regex(@"(\*)");

            var queryMatchedText = from text in reservations
                            where regexUserIdentifier.IsMatch(text)
                            select text.ToString();

            foreach (var queryText in queryMatchedText)
            {
                CancelOrder.Visibility = Visibility.Visible;
                listBx.Items.Add(queryText);

            }
            if (listBx.Items.Count == 0)
            {

                MessageBox.Show("Du har inga registrerade bokningar.");
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (listBx.SelectedItem != null)
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

                foreach (var text in textToKeep)
                {
                    Filehandler.WriteFile(text);
                }

                MessageBox.Show("Din bokning har nu tagits bort");
                CancelOrder.Visibility = Visibility.Collapsed;
                listBx.Items.Clear();
            }
            else
            {
                MessageBox.Show("du har inte valt en bokning att avboka");
            }







        }



        public static async Task ReservTable(string table, string fullName, string gender, DateOnly dateToCheck, string time, string nr, string text, List<DateAndTime> dates) // bokning för användaren
        {

            bool checkTime;





            foreach (var date in dates)
            {
                if (date.datum == dateToCheck)
                {
                    for (int i = 0; i < date.Tider.Count; i++)
                    {
                        if (date.Tider[i].tid == time)
                        {
                            checkTime = await CheckTableAvailable(dateToCheck, date.Tider[i], table);

                            if (checkTime == true)
                            {

                                Filehandler.WriteFile(text);
                                MessageBox.Show("din bokning har registrerats");


                            }
                            else
                            {
                                MessageBox.Show("Bordet är redan taget!");
                            }

                            i = date.Tider.Count;
                        }
                    }
                }


            }


        }


        public static async Task ReserveTable(string table, string fullName, string gender, DateOnly dateToCheck, string time, string nr, string text, List<DateAndTime> dates, string custom) // bokning för färdiga bokningar.
        {

            bool checkTime;



         

            foreach (var item in dates)
            {
                if (item.datum == dateToCheck)
                {
                    for (int i = 0; i < item.Tider.Count; i++)
                    {
                        if (item.Tider[i].tid == time)
                        {
                            checkTime = await CheckTableAvailable(dateToCheck, item.Tider[i], table);

                            if (checkTime == true)
                            {

                                Filehandler.WriteFile(text);


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





        public static async Task<bool> CheckTableAvailable(DateOnly date, Time time, string table)
        {
            bool isAvailalbe = true;
            var regexDateIdentifier = new Regex(@"Datum: " + date.ToString());
            var regexTimeIdentifier = new Regex(@"Klockan: " + time.tid);
            var regexbordIdentifier = new Regex(@"Bord: " + table);

            List<string> reservations = await Filehandler.GetTextsFileAsync();



            var queryMatchText = from text in reservations
                            where regexDateIdentifier.IsMatch(text)
                            where regexTimeIdentifier.IsMatch(text)
                            where regexbordIdentifier.IsMatch(text)
                            select text;



            foreach (var queryText in queryMatchText)
            {

                isAvailalbe = false;
            }


            return isAvailalbe;


        }


        public static bool CheckInputs(string firstName, string lastName, string time, string gender, string phoneNr, string table)
        {
            string missingText = "";
            var regexPhone = new Regex("^0[0-9]{9}"); // 0XX XXX XX XX 10 siffor om tid +46 också

            List<string> missingInputs = new List<string>();

            if (firstName == "") { missingInputs.Add($"Namn:"); }
            if (lastName == "") { missingInputs.Add($"Efternamn:"); }
            if (phoneNr == "") { missingInputs.Add($"Telefon nummer:"); }
            if (time == "") { missingInputs.Add($"Tid:"); }
            if (gender == "") { missingInputs.Add($"kön:"); }
            if (table == "") { missingInputs.Add("Bord:"); }

            if (missingInputs.Count > 0)
            {
                for (int i = 0; i < missingInputs.Count; i++)
                {
                    missingText += $"{missingInputs[i]} Saknas\n";
                }
                MessageBox.Show($"Du har ej fyllt i alla rutor!\n{missingText}");
                return false;
            }

            if (regexPhone.IsMatch(phoneNr) != true)
            {
                MessageBox.Show("Inte skrivit ett riktigt mobilnummer. OBS kan ej starta med +46, använd 0 i starten");
                return false;
            }


            else
            {
                return true;
            }




        }


        public static void AddDates(List<DateAndTime> dates)
        {
            int daysToFill = 7;
            int startday = 14;
            int month = 11;
            int year = 2022;
            for (int i = 0; i < daysToFill; i++)
            {
                DateOnly date = new DateOnly(year, month, startday + i);
                DateAndTime addDay = new DateAndTime(date);
                dates.Add(addDay);
            }
        }
       

        public static void PreMadeBookings(List<DateAndTime> dates)
        {
            List<string> reservations = Filehandler.GetTextsFile();

            if (reservations.Count == 1)
            {
                List<Woman> woman = new List<Woman>();
                List<Man> men = new List<Man>();

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

                //16.00
                woman.Add(new Woman(nameWoman_1, nov_14, time_16, phoneNr_1, "1"));
                men.Add(new Man(nameMan_1, nov_14, time_16, phoneNr_5, "3"));
                men.Add(new Man(nameMan_2, nov_14, time_16, phoneNr_5, "4"));
                //17.00
                woman.Add(new Woman(nameWoman_3, nov_14, time_17, phoneNr_4, "1"));
                men.Add(new Man(nameMan_4, nov_14, time_17, phoneNr_5, "2"));
                men.Add(new Man(nameMan_3, nov_14, time_17, phoneNr_5, "3"));
                //18.00
                woman.Add(new Woman(nameWoman_4, nov_14, time_18, phoneNr_3, "2"));
                men.Add(new Man(nameMan_1, nov_14, time_18, phoneNr_3, "5"));
                men.Add(new Man(nameMan_4, nov_14, time_18, phoneNr_5, "4"));
                //19.00
                woman.Add(new Woman(nameWoman_5, nov_14, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_14, time_19, phoneNr_3, "3"));
                woman.Add(new Woman(nameWoman_2, nov_14, time_19, phoneNr_5, "4"));
                //20.00
                men.Add(new Man(nameMan_4, nov_14, time_20, phoneNr_5, "4"));
                woman.Add(new Woman(nameWoman_3, nov_14, time_20, phoneNr_3, "1"));
                //21.00
                woman.Add(new Woman(nameWoman_3, nov_14, time_21, phoneNr_3, "3"));
                men.Add(new Man(nameMan_5, nov_14, time_21, phoneNr_1, "1"));
                men.Add(new Man(nameMan_2, nov_14, time_21, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_14, time_21, phoneNr_3, "5"));
             
               

               






                // 15 November 2022

                //16.00
                woman.Add(new Woman(nameWoman_1, nov_15, time_16, phoneNr_1, "1"));
                //17.00
                woman.Add(new Woman(nameWoman_3, nov_15, time_17, phoneNr_4, "1"));
                woman.Add(new Woman(nameWoman_2, nov_15, time_17, phoneNr_2, "2"));
                woman.Add(new Woman(nameWoman_4, nov_15, time_17, phoneNr_3, "3"));
                men.Add(new Man(nameMan_3, nov_15, time_17, phoneNr_5, "4"));
                men.Add(new Man(nameMan_1, nov_15, time_17, phoneNr_3, "5"));
                //18.00
                woman.Add(new Woman(nameWoman_4, nov_15, time_18, phoneNr_3, "1"));
                men.Add(new Man(nameMan_4, nov_15, time_18, phoneNr_5, "2"));
                men.Add(new Man(nameMan_2, nov_15, time_18, phoneNr_2, "3"));
                men.Add(new Man(nameMan_3, nov_15, time_18, phoneNr_3, "4"));
                men.Add(new Man(nameMan_5, nov_15, time_18, phoneNr_1, "5"));
                //19.00
                woman.Add(new Woman(nameWoman_3, nov_15, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_15, time_19, phoneNr_5, "4"));
                //20.00
                men.Add(new Man(nameMan_4, nov_15, time_20, phoneNr_5, "1"));
                men.Add(new Man(nameMan_2, nov_15, time_20, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_15, time_20, phoneNr_3, "4"));
                //21.00
                men.Add(new Man(nameMan_5, nov_15, time_21, phoneNr_1, "1"));
         
               
            
               





                // 16 November 2022

                // 16.00
                woman.Add(new Woman(nameWoman_2, nov_16, time_16, phoneNr_2, "5"));
                woman.Add(new Woman(nameWoman_2, nov_16, time_16, phoneNr_5, "4"));
                // 17.00
                woman.Add(new Woman(nameWoman_3, nov_16, time_17, phoneNr_4, "1"));
                men.Add(new Man(nameMan_3, nov_16, time_17, phoneNr_5, "3"));
                men.Add(new Man(nameMan_1, nov_16, time_17, phoneNr_3, "5"));
                // 18.00
                woman.Add(new Woman(nameWoman_4, nov_16, time_18, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_3, nov_16, time_18, phoneNr_3, "3"));
                woman.Add(new Woman(nameWoman_3, nov_16, time_18, phoneNr_3, "4"));
                men.Add(new Man(nameMan_3, nov_16, time_18, phoneNr_5, "5"));
                // 19.00
                woman.Add(new Woman(nameWoman_5, nov_16, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_16, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_16, time_19, phoneNr_5, "4"));
                // 20.00
                men.Add(new Man(nameMan_4, nov_16, time_20, phoneNr_5, "4"));
                // 21.00
                woman.Add(new Woman(nameWoman_1, nov_16, time_21, phoneNr_1, "1"));
                men.Add(new Man(nameMan_5, nov_16, time_21, phoneNr_1, "2"));
                men.Add(new Man(nameMan_2, nov_16, time_21, phoneNr_2, "4"));
                men.Add(new Man(nameMan_3, nov_16, time_21, phoneNr_3, "5"));






                // 17 November 2022

                // 16.00
                woman.Add(new Woman(nameWoman_1, nov_17, time_16, phoneNr_1, "1"));
                woman.Add(new Woman(nameWoman_2, nov_17, time_16, phoneNr_2, "2"));
                woman.Add(new Woman(nameWoman_5, nov_17, time_16, phoneNr_5, "3"));
                woman.Add(new Woman(nameWoman_3, nov_17, time_16, phoneNr_3, "4"));
                men.Add(new Man(nameMan_1, nov_17, time_16, phoneNr_5, "3"));
                // 17.00
                woman.Add(new Woman(nameWoman_3, nov_17, time_17, phoneNr_4, "1"));
                woman.Add(new Woman(nameWoman_1, nov_17, time_17, phoneNr_1, "2"));
                woman.Add(new Woman(nameWoman_2, nov_17, time_17, phoneNr_2, "3"));
                men.Add(new Man(nameMan_3, nov_17, time_17, phoneNr_5, "4"));
                men.Add(new Man(nameMan_1, nov_17, time_17, phoneNr_3, "5"));
                // 18.00
                woman.Add(new Woman(nameWoman_4, nov_17, time_18, phoneNr_3, "1"));
                men.Add(new Man(nameMan_2, nov_17, time_18, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_17, time_18, phoneNr_3, "5"));
                // 19.00
                woman.Add(new Woman(nameWoman_5, nov_17, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_17, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_17, time_19, phoneNr_5, "4"));
                // 20.00
                men.Add(new Man(nameMan_4, nov_17, time_20, phoneNr_5, "4"));
                // 21.00
                men.Add(new Man(nameMan_5, nov_17, time_21, phoneNr_1, "1"));
                men.Add(new Man(nameMan_2, nov_17, time_21, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_17, time_21, phoneNr_3, "5"));






                // 18 November 2022

                // 16.00
                woman.Add(new Woman(nameWoman_1, nov_18, time_16, phoneNr_1, "1"));
                woman.Add(new Woman(nameWoman_2, nov_18, time_16, phoneNr_2, "2"));
                men.Add(new Man(nameMan_1, nov_18, time_16, phoneNr_5, "3"));
                men.Add(new Man(nameMan_2, nov_18, time_16, phoneNr_5, "4"));
                men.Add(new Man(nameMan_3, nov_18, time_16, phoneNr_3, "5"));
                // 17.00
                woman.Add(new Woman(nameWoman_3, nov_18, time_17, phoneNr_4, "1"));
                woman.Add(new Woman(nameWoman_3, nov_18, time_17, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_18, time_17, phoneNr_5, "4"));
                men.Add(new Man(nameMan_3, nov_18, time_17, phoneNr_5, "3"));
                men.Add(new Man(nameMan_1, nov_18, time_17, phoneNr_3, "5"));
                // 18.00
                woman.Add(new Woman(nameWoman_4, nov_18, time_18, phoneNr_3, "1"));
                woman.Add(new Woman(nameWoman_3, nov_18, time_18, phoneNr_1, "2"));
                woman.Add(new Woman(nameWoman_2, nov_18, time_18, phoneNr_2, "4"));
                // 19.00
                woman.Add(new Woman(nameWoman_5, nov_18, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_18, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_18, time_19, phoneNr_5, "4"));
                // 20.00
                woman.Add(new Woman(nameWoman_1, nov_18, time_20, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_18, time_20, phoneNr_3, "2"));
                men.Add(new Man(nameMan_4, nov_18, time_20, phoneNr_5, "3"));
                men.Add(new Man(nameMan_2, nov_18, time_20, phoneNr_2, "5"));
                // 21.00
                woman.Add(new Woman(nameWoman_5, nov_18, time_21, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_18, time_21, phoneNr_3, "2"));
                men.Add(new Man(nameMan_5, nov_18, time_21, phoneNr_1, "3"));
                men.Add(new Man(nameMan_2, nov_18, time_21, phoneNr_2, "4"));
                men.Add(new Man(nameMan_3, nov_18, time_21, phoneNr_3, "5"));




                // 19 November 2022

                // 16.00
                woman.Add(new Woman(nameWoman_1, nov_19, time_16, phoneNr_1, "1"));
                woman.Add(new Woman(nameWoman_2, nov_19, time_16, phoneNr_2, "2"));
                men.Add(new Man(nameMan_1, nov_19, time_16, phoneNr_5, "3"));
                // 17.00
                woman.Add(new Woman(nameWoman_3, nov_19, time_17, phoneNr_4, "1"));
                men.Add(new Man(nameMan_2, nov_19, time_17, phoneNr_5, "4"));
                men.Add(new Man(nameMan_3, nov_19, time_17, phoneNr_5, "3"));
                men.Add(new Man(nameMan_1, nov_19, time_17, phoneNr_3, "5"));
                // 18.00
                woman.Add(new Woman(nameWoman_4, nov_19, time_18, phoneNr_3, "1"));
                men.Add(new Man(nameMan_2, nov_19, time_18, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_19, time_18, phoneNr_3, "5"));
                // 19.00
                woman.Add(new Woman(nameWoman_5, nov_19, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_19, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_19, time_19, phoneNr_5, "4"));
                // 20.00
                woman.Add(new Woman(nameWoman_1, nov_19, time_20, phoneNr_1, "1"));
                men.Add(new Man(nameMan_4, nov_19, time_20, phoneNr_5, "4"));
                // 21.00
                woman.Add(new Woman(nameWoman_3, nov_19, time_21, phoneNr_3, "3"));
                woman.Add(new Woman(nameWoman_2, nov_19, time_21, phoneNr_5, "4"));
                men.Add(new Man(nameMan_5, nov_19, time_21, phoneNr_1, "1"));
                men.Add(new Man(nameMan_2, nov_19, time_21, phoneNr_2, "2"));
                men.Add(new Man(nameMan_3, nov_19, time_21, phoneNr_3, "5"));




                // 20 November 2022

                // 16.00
                men.Add(new Man(nameMan_1, nov_20, time_16, phoneNr_5, "3"));
                men.Add(new Man(nameMan_2, nov_20, time_16, phoneNr_5, "4"));
                // 17.00
                woman.Add(new Woman(nameWoman_3, nov_20, time_17, phoneNr_4, "1"));
                men.Add(new Man(nameMan_3, nov_20, time_17, phoneNr_5, "3"));
                men.Add(new Man(nameMan_1, nov_20, time_17, phoneNr_3, "5"));
                // 18.00
                woman.Add(new Woman(nameWoman_1, nov_20, time_18, phoneNr_1, "1"));
                woman.Add(new Woman(nameWoman_2, nov_20, time_18, phoneNr_2, "2"));
                woman.Add(new Woman(nameWoman_4, nov_20, time_18, phoneNr_3, "3"));
                // 19.00
                woman.Add(new Woman(nameWoman_5, nov_20, time_19, phoneNr_5, "1"));
                woman.Add(new Woman(nameWoman_3, nov_20, time_19, phoneNr_3, "2"));
                woman.Add(new Woman(nameWoman_2, nov_20, time_19, phoneNr_5, "4"));
                // 20.00
                men.Add(new Man(nameMan_4, nov_20, time_20, phoneNr_5, "4"));
                // 21.00
                woman.Add(new Woman(nameWoman_3, nov_20, time_21, phoneNr_3, "5"));
                men.Add(new Man(nameMan_5, nov_20, time_21, phoneNr_1, "1"));
            

            
             
       









                foreach (var person in woman)
                {
                    ReserveTable(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, dates, "custom");
                }

                foreach (var person in men)
                {
                    ReserveTable(person.table, person.name, person.gender, person.date, person.time, person.phoneNr, person.text, dates, "custom");
                }
            }



        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string MenuText = "~~~~Dagens meny~~~~";
            string food_1 = "Sushi";
            string food_2 = "Räkor";
            string food_3 = "Pizza";
            string food_4 = "Bläckfisk";

            MessageBox.Show($"{MenuText}\n" +
                $"1. {food_1}\n" +
                $"2. {food_2}\n" +
                $"3. {food_3}\n" +
                $"4. {food_4}");


            
        }
    }
}




























































































