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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using Kursach;

namespace Klient_OK
{
    public partial class SettingVindow : Window
    {
        public SettingVindow()
        {
            InitializeComponent();
        }
        private bool AreSetImg=false;
        public event Action<object, RoutedEventArgs> ButtonClicked;
        string impath = "";
        private void GetFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialogF = new OpenFileDialog();
            dialogF.ShowDialog();
            impath = dialogF.FileName;
            Avatar.Source = new BitmapImage(new Uri(impath, UriKind.RelativeOrAbsolute));
            Avatar.Stretch = Stretch.UniformToFill;
            AreSetImg = true;
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.TName.Clear();
            this.TIP.Clear();
            this.TPort.Clear();
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (AreSetImg == false)
            {
                impath = @"" + Environment.CurrentDirectory.ToString() + "\\YourSet.png";
            }
            Setting st = new Setting("0.0.0.0", "0000", "aaaa" , System.Drawing.Image.FromFile(impath));
            if (TIP.GetLineLength(0) == 0 | TPort.GetLineLength(0) == 0 | TName.GetLineLength(0) == 0)
            {
                MessageBox.Show("Всі поля мусять бути заповнені!");
            }
            else
            {
                if (!st.IsIP(TIP.Text))
                {
                    MessageBox.Show("Невірний формат ІР адреси!");
                }
                else if (!st.IsPort(TPort.Text))
                {
                    MessageBox.Show("Введено неіснуючий номер порту!");
                }
                else
                {
                    st = new Setting(TIP.Text, TPort.Text, TName.Text, System.Drawing.Image.FromFile(impath));
                    st.SaveSetting();
                    this.Close();
                    if (ButtonClicked != null)
                    {
                        ButtonClicked(this, e);
                    }
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
