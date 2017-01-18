using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using Kursach;
using System.Drawing;
using ServerWPF;

namespace Klient_OK
{
    public partial class MainWindow : Window
    {
        List<UserInformations> UI;
        Setting Set;
        Client Client = new Client();
        NotifyIcon trey;
        public MainWindow()
        {
            InitializeComponent();
            UI = new List<UserInformations>();
            Set = new Setting("0.0.0.0", "4005", "Null");
            trey = new NotifyIcon();
            ShowTime();
            ShowInfoAboutSetting();
        }
        public bool AreSettingSet()
        {
            string path = @"" + Environment.CurrentDirectory.ToString() + "Setting.txt";
            FileInfo file = new FileInfo(path);
            if (file.Exists && file.Length > 0) return true;
            else return false;

        }
        private void ExitLi_Click(object sender, RoutedEventArgs e)
        {
            if (Client.IsConnected())
            {
                Client.SendMessage("EXIT");
                Client.FinishStream();
            }
            this.Close();
        }
        public void ShowInfoAboutSetting()
        {
            string currDir = Environment.CurrentDirectory.ToString();
            string path = @"" + currDir + "Setting.txt";
            if (!File.Exists(path)) { UserLabel.Content = "НЕ ЗАДАНО"; }
            else
            {
                FileInfo file = new FileInfo(path);
                if (file.Length == 0)
                {
                    UserLabel.Content = "Не задано налаштування!";
                }
                else
                {
                    string str = File.ReadAllText(path);
                    string[] information = str.Split(':');
                    if (information[0].Length > 8)
                    {
                        string s = "";
                        for (int i = 0; i < 7; i++) s += information[0][i];
                        s += "...";
                        UserLabel.Content = s;
                    }
                    else
                    {
                        UserLabel.Content = information[0];
                    }
                    UserIPLabel.Content = information[1];
                    UserPortLabel.Content = information[2];
                    if (File.Exists(@"" + currDir + "Image.txt"))
                    {
                        string strIm = File.ReadAllText(@"" + currDir + "Image.txt");
                        byte[] arr = Convert.FromBase64String(strIm);
                        MemoryStream ms = new MemoryStream(arr);
                        var dec = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        var img = new ImageBrush();
                        img.ImageSource = new BitmapImage();
                        img.ImageSource = dec.Frames[0];
                        img.Stretch = Stretch.UniformToFill;
                        UserInformation.Background = img;
                    }
                    Set.SetIP(information[1]);
                    Set.SetPort(information[2]);
                    Set.SetUser(information[0]);
                    Client = new Client(Set);
                    Client.GetMessageEvent += GetMessage;
                    Client.NewUserConnectEvent += NewUser;
                    Client.UserDisconnectEvent += DeleteUser;
                    Client.ServerDisconnect += Client_ServerDisconnect;
                }
            }

        }
        void Client_ServerDisconnect(object server, EventArgs e)
        {
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                UI.Clear();
                UserList.Items.Clear();
                this.connectText.Content = "З'ЄДНАТИСЬ";
            });
        }
        private void NewSettingLi_Click(object sender, RoutedEventArgs e)
        {
            SettingVindow window = new SettingVindow();
            window.ButtonClicked += UpdateSettingLi_Click;
            window.ShowDialog();
        }
        private void ShowTime()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o , e) => { ProgTime.Content = DateTime.Now.Hour+":"+DateTime.Now.Minute; };
            timer.Start();
        }
        public void ShowInfoInTrey(string title, string text, ToolTipIcon icon)
        {
            trey.Icon = SystemIcons.Information;
                    trey.BalloonTipTitle = title;
                    trey.BalloonTipText = text;
                    trey.BalloonTipIcon = icon;
                    trey.Visible = true;
                    trey.ShowBalloonTip(50);
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (AreSettingSet()!=true)
            {
                System.Windows.MessageBox.Show("Не заданні налаштування!");
            }
            else
            {
                if (String.Compare("З'ЄДНАТИСЬ", this.connectText.Content.ToString()) == 0)
                {
                    try
                    {
                        string currDir = Environment.CurrentDirectory.ToString();
                        string strIm = File.ReadAllText(@"" + currDir + "Image.txt");
                        Client.Connect();
                        Task Listen = new Task(Client.GetSMS);
                        Listen.Start();
                        Client.SendMessage("CONNECT:" + "IMG:" + strIm);
                        this.connectText.Content = "ВИЙТИ";
                        ShowInfoInTrey("CONNECT", "З'єднання встановлено вдало", ToolTipIcon.Info);
                        SettingButton.Focusable = false;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        Client.SendMessage("EXIT");
                        Client.FinishStream();
                        this.connectText.Content = "З'ЄДНАТИСЬ";
                        UserList.Items.Clear();
                        SettingButton.Focusable = true;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        public void NewUser(object sender, EventArgs e)
        {
            UserEventArgs arg = (UserEventArgs)e;
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                string mh = "";
                UserInformations u = new UserInformations();
                u.MessHistory = mh;
                u.Avatar = arg.Avatar;
                UI.Add(u);
                UserList.Items.Add(arg.User_Name);
            });
        }
        public void DeleteUser(object sender , EventArgs e)
        {
            UserEventArgs arg = (UserEventArgs)e;
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                int ind = 0;
                foreach (var item in UserList.Items)
                {
                    if (item.ToString() == arg.User_Name) { ind = UserList.Items.IndexOf(item); break; }
                }
                UI.RemoveAt(ind);
                UserList.Items.Remove(arg.User_Name);
            });
        }
        public void GetMessage(object sender, EventArgs e)
        {
            MessageEventArgs arg = (MessageEventArgs)e;
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                int ind = 0;
                foreach (var item in UserList.Items)
                {
                    if (item.ToString() == arg.User_Name) { ind = UserList.Items.IndexOf(item); break; }
                }
                UI[ind].MessHistory += DateTime.Now.Hour + ":" + DateTime.Now.Minute + "-" + arg.User_Name + ":" + arg.Message + "\n";
                if (UserList.SelectedIndex != -1)
                {
                    MessageH.Text = UI[UserList.SelectedIndex].MessHistory;
                }
                ShowInfoInTrey("Нове повідомлення від " + arg.User_Name, " ", ToolTipIcon.None);
            });
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SendMessage.Clear();
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (SendMessage.Text == "" | SendMessage.Text == " ")
            {
                System.Windows.MessageBox.Show("Не можна відправити пусте повідомлення!");
            }
            else
            {
                try
                {
                    if (UserList.Items.Count != 0)
                    {
                        if (UserList.SelectedIndex != -1)
                        {
                            Client.SendMessage(SendMessage.Text + ":" + UserList.SelectedValue.ToString());
                            UI[UserList.SelectedIndex].MessHistory += DateTime.Now.Hour + ":" + DateTime.Now.Minute + "-" + "Ви : " + SendMessage.Text + "\n";
                            MessageH.Text = UI[UserList.SelectedIndex].MessHistory;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Оберіть отримувача!");
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Ви не можете відправити повідомлення, оскільки активних користувачів, окрім вас, немає");
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UserList.SelectedIndex != -1)
                {
                    int ind = UserList.SelectedIndex;
                    MessageH.Text = UI[ind].MessHistory;
                    MUser.Content = UserList.SelectedValue.ToString();
                    string strava = UI[ind].Avatar;
                    byte[] arr = Convert.FromBase64String(strava);
                    string masr = Convert.ToBase64String(arr);
                    MemoryStream ms = new MemoryStream(arr);
                    var dec = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    var img = new ImageBrush();
                    img.ImageSource = new BitmapImage();
                    img.ImageSource = dec.Frames[0];
                    img.Stretch = Stretch.UniformToFill;
                    VisitAva.Background = img;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }
        private void UpdateSettingLi_Click(object sender, RoutedEventArgs e)
        {
            ShowInfoAboutSetting();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Client.IsConnected())
            {
                Client.SendMessage("EXIT");
                Client.FinishStream();
            }
            this.Close();
        }
    }
}
