using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using SDataExt;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DataLoader Dloader { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Dloader = new DataLoader();
            Dloader.GetList();
            var sTemp = Dloader.SecList;
            cmbSec.ItemsSource = sTemp;
            this.dgSec.ItemsSource = sTemp;

        }

        private void CldFrom_OnDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            txtFrom.Text = cldFrom.DisplayDate.ToString();
        }


        private void CldTo_OnDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            txtTo.Text = cldTo.DisplayDate.ToString();
        }

        private void BtnGetdata_OnClick(object sender, RoutedEventArgs e)
        {
            TimeSpan myTimeFrame;

            if (cmbSec.SelectedItem == null || cmbSec.SelectedItem == "") return;
            if (txtFrom.Text == null || txtFrom.Text == "") return;
            if (txtTo.Text == null || txtTo.Text == "") return;
            if (cbTF.SelectedIndex == -1) return;

            string tPath = @"D:\SH\Hydra";

            int iTF = cbTF.SelectedIndex;

            switch (iTF)
            {
                case 0:
                    myTimeFrame = TimeSpan.FromMinutes(1);
                    break;
                case 1:
                    myTimeFrame = TimeSpan.FromMinutes(5);
                    break;
                case 2:
                    myTimeFrame = TimeSpan.FromHours(1);
                    break;
                case 3:
                    myTimeFrame = TimeSpan.FromDays(1);
                    break;
                default:
                    myTimeFrame = TimeSpan.FromMinutes(1);
                    break;
            }


            string SecTest = cmbSec.SelectedValue.ToString();
            Dloader = new DataLoader(SecTest, myTimeFrame, cldFrom.DisplayDate, cldTo.DisplayDate, "All",tPath);
            var result = Dloader.GetData();

            if (result == 1)
            {
                var sdates = Dloader.dlInfo;
                txtCnt.Text = sdates.Length.ToString();
                Debug.WriteLine("Candles count {0}", sdates.Length.ToString());
            }
            else
            {
                txtCnt.Text = 0.ToString();
                Debug.WriteLine("No candles count retrived");
            }
            

        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var res = this.dgSec.SelectedValue;
            //txtSec.Text = res.ToString();
        }
    }
}
