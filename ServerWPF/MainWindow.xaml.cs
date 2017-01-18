using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server_Console;

namespace ServerWPF
{
    public partial class MainWindow : Window
    {
        ThreadServer Server = new ThreadServer();
        public delegate void _ThreadServer();
        IAsyncResult resultAsync;
        _ThreadServer operatThread;
        List<string> UserSessionInfo;
        public MainWindow()
        {
            UserSessionInfo = new List<string>();
            InitializeComponent();
            Server.NewUser += AddUsers;
            Server.UserDisconnect += DeleteUser;
            Server.GetMessege += AddInfoToSession;
            Server.SendMessege += AddInfoToSession;
        }
        private void StartAccepting_Click(object sender, RoutedEventArgs e)
        {
            operatThread = new _ThreadServer(ServerTurnOn);
            resultAsync = operatThread.BeginInvoke(new AsyncCallback(FinishServerStream) , Server);
            ServerConsition.Text = "Server is running";
            ServerConsition.Foreground = System.Windows.Media.Brushes.Green;
            
        }
        public static void FinishServerStream(IAsyncResult resobj)
        {
            ThreadServer server = (ThreadServer)resobj.AsyncState;
            server.CloseServer();
        }
        public void ServerTurnOn()
        {
            while (true)
            {
                try
                {
                    Server.AcceptConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void AddUsers(object sender , EventArgs e)
        {
            UserEventArgs arg = (UserEventArgs)e;
                this.Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        ActiveUsers.Items.Add(arg.User_Name);
                        UserSessionInfo.Add(Shifr.Coder(arg.User_Name + ": приєднався. " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "\n"));
                    });
        }
        public void DeleteUser(object sender, EventArgs e)
        {
            UserEventArgs arg = (UserEventArgs)e;
            this.Dispatcher.BeginInvoke((ThreadStart)delegate()
                {
                    int index = ActiveUsers.Items.IndexOf(arg.User_Name);
                    ActiveUsers.Items.Remove(arg.User_Name);
                    UserSessionInfo.RemoveAt(index);
                });
        }
        public void AddInfoToSession(object sender, EventArgs e)
        {
            MessageEventArgs arg = (MessageEventArgs)e;
                this.Dispatcher.BeginInvoke((ThreadStart)delegate()
                {
                    string[] str = arg.Message.Split(':');
                    int index = ActiveUsers.Items.IndexOf(arg.User_Name);
                    if (index != -1)
                    {
                        UserSessionInfo[index] += Shifr.Coder(arg.Message) + "\n";
                    }
                    if (ActiveUsers.SelectedIndex!=-1) UserSession.Text = UserSessionInfo[ActiveUsers.SelectedIndex];
                    });
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Server.DisposeUser();
            Server.CloseServer();
            UserSessionInfo.Clear();
            ActiveUsers.Items.Clear();
        }
        private void ActiveUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ActiveUsers.SelectedIndex;
            if (index != -1)
            {
                if (PasswordDec.Text.Length == 0)
                {
                    UserSession.Text = Shifr.DeCoder(UserSessionInfo[index], " ");
                }
                else
                {
                    UserSession.Text = Shifr.DeCoder(UserSessionInfo[index], PasswordDec.Text);
                }
            }
        }
    }
}
