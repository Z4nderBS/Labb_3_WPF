using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      

        public MainWindow()
        {
            
            InitializeComponent();
          
           
        }

        //private void btn_Click(object sender, RoutedEventArgs e)
        //{
        //    List<Booking> bookingList = new List<Booking>();
        //    bookingList.AddRange(PrebookedList());

        //    foreach (var item in bookingList)
        //    {
        //        listBx.Items.Add(item.name);
        //        listBx.Items.Add(item.table);
        //        listBx.Items.Add(item.date);
        //        listBx.Items.Add(item.time);
        //    }
        //}



        public static List<Booking> PrebookedList()
        {
         List<Booking> bookingList = new List<Booking>();

            Booking booking1 = new Booking("Alex", new DateOnly(2022, 09, 25), 22.00, 5);
            Booking booking2 = new Booking("Wilma", new DateOnly(2022, 09, 24), 21.00, 5);

            bookingList.Add(booking1);
            return bookingList;
        }

        private void BookingBtn_Click(object sender, RoutedEventArgs e)
        {
            var namn = $"{firstNameBox.Text} {lastNameBox.Text}";
            var kalenderDatum = MainCalendar.SelectedDate.Value;
            var datum = kalenderDatum.ToShortDateString();
            var tid = TimeChoiceBox.Text.ToString();
            var bord = TableChoiceBox.Text.ToString(); 

            string text = $"" +
                $"------------------------------------\n" +
                $"Namn: {namn}\n" +
                $"Datum : {datum}\n" +
                $"Klockan: {tid}\n" +
                $"Bord: {bord}\n" +
                $"-----------------------------------";

            listBx.Items.Add(text);
        }
    }
}
