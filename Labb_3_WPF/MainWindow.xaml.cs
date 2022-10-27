using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Labb_3_WPF
{

    public partial class MainWindow : Window
    {
        public List<Booking> bookingList = new List<Booking>();
      

        public MainWindow()
        {
            
            InitializeComponent();
          
           
        }


        public static bool CheckInputs(string förNamn, string efterNamn, string tid, string bord)
        {
            string missingText = "";
           
            List<string> missingInputs = new List<string>();
      
            if (förNamn == "") { missingInputs.Add($"Namn: {förNamn}"); }
            if (efterNamn == "") { missingInputs.Add($"Efternamn: {efterNamn}"); }
            if (tid == "") { missingInputs.Add($"Tid: {tid}"); }
            if (bord == "") { missingInputs.Add($"Bord: {bord}"); }
                   
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

        private void BookingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var förNamn = firstNameBox.Text;
                var efterNamn = lastNameBox.Text;
                var namn = $"{förNamn} {efterNamn}";
                var kalenderDatum = MainCalendar.SelectedDate.Value.Date.ToShortDateString();
                DateOnly datum = DateOnly.Parse(kalenderDatum); 
                var tid = TimeChoiceBox.Text.ToString();
                var bord = TableChoiceBox.Text.ToString();


              

                if (CheckInputs(förNamn, efterNamn, tid, bord) == true)
                {
                    string text = $"" +
                          $"------------------------------------\n" +
                          $"Namn: {namn}\n" +
                          $"Datum : {kalenderDatum}\n" +
                          $"Klockan: {tid}\n" +
                          $"Bord: {bord}\n" +
                          $"-----------------------------------";


                    // sparar första siffrar i bord boxen
                    string bordSiffra = bord.Remove(1, bord.Length - 1);
                    string newTid = tid.Replace('.', ',');
                    double RightTid = double.Parse(newTid);



                    listBx.Items.Add(text);
                    Booking bokning = new Booking(namn, datum, RightTid, int.Parse(bordSiffra));
                    bookingList.Add(bokning);

                    firstNameBox.Text = "";
                    lastNameBox.Text = "";
                    TimeChoiceBox.Text = "";
                    TableChoiceBox.Text = "";


                }
                

               
            






               

            

            }
            catch (Exception)
            {
                MessageBox.Show("Välj ett Datum");
                
            }
         
          
        }




        //public static List<Booking> PrebookedList()
        //{


        //    Booking booking1 = new Booking("Alex", new DateOnly(2022, 09, 25), 22.00, 5);
        //    Booking booking2 = new Booking("Wilma", new DateOnly(2022, 09, 24), 21.00, 5);

        //    bookingList.Add(booking1);
        //    return bookingList;
        //}
    }
}
