using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using ServerWPF;

namespace Server_Console
{
    class ThreadServer
    {
        public class NewConnection
        {
            private TcpClient _Client;
            private NetworkStream _Stream;
            public delegate void _Thread(NewConnection con);
            private IAsyncResult res;
            private string _name;
            private string ava;
            private int _index;
            public NewConnection()
            {
                _Client = null;
                _Stream = null;
                _name = "Unknown";
                _index = 0;
                ava = ""; 
            }
            public IAsyncResult Result
            {
                get
                {
                    return res;
                }
                set
                {
                    res = value;
                }
            }
            public TcpClient Client
            {
                set
                {
                    _Client = value;
                }
                get
                {
                    return _Client;
                }
            }
            public NetworkStream Stream
            {
                set
                {
                    _Stream = value;
                }
                get
                {
                    return _Stream;
                }
            }
            public string Ava
            {
                get
                {
                    return ava;
                }
                set
                {
                    ava = value;
                }
            }
            public string Name
            {
                set
                {
                    _name = value;
                }
                get
                {
                    return _name;
                }
            }
            public void SetTcpClient(TcpClient a)
            {
                _Client = a;
            }
            public void SetNetStream(NetworkStream a)
            {
                _Stream = a;
            }
            public void SetName(string a)
            {
                _name = a;
            }
            public void SetIndex(int i)
            {
                _index = i;
            }
            public string GetName()
            {
                return _name;
            }
            public Socket ReturnClient()
            {
                return _Client.Client;
            }

            public void StopThreading()
            {
                this._Stream.Dispose();
                this._Client.Close();
            }
        }
        public delegate void UserEvent(object sender , UserEventArgs e);
        public delegate void MessageEvent(object sender, MessageEventArgs e);
        public event UserEvent NewUser;
        public event UserEvent UserDisconnect;
        public event MessageEvent SendMessege;
        public event MessageEvent GetMessege;
        private TcpListener _Listener;
        private TcpClient _NewClient;
        private NetworkStream _NewStream;
        private List<NewConnection> GroupOfConnection;
        private List<NewConnection._Thread> ThreadConnextion;
        public ThreadServer()
        {
            _Listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 4005);
            _NewClient = null;
            _NewStream = null;
            GroupOfConnection=new List<NewConnection>();
            ThreadConnextion=new List<NewConnection._Thread>();
        }
        public void AcceptConnection()
        {
            try
            {
                _Listener.Start();
                _NewClient = _Listener.AcceptTcpClient();
                _NewStream = _NewClient.GetStream();
                NewConnection New = new NewConnection();
                New.SetTcpClient(_NewClient);
                New.SetNetStream(_NewStream);
                NewConnection._Thread thr = new NewConnection._Thread(WorkIntoThread);
                GroupOfConnection.Add(New);
                ThreadConnextion.Add(thr);
                New.Result = thr.BeginInvoke(New, new AsyncCallback(FinishStreamWithUser), New);
                Thread.Sleep(500);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void WorkIntoThread( object state)
        {
            NewConnection User = (NewConnection)state;
            NetworkStream _stream = User.Stream;
            while (User.Client.Connected)
            {
                try
                {
                    string get = "";
                    byte[] mass=new byte[1000000];
                    int message_length = 0;
                    message_length = _stream.Read(mass, 0, mass.Length);
                    if (mass.Length != 0)
                    {
                        get = Encoding.Default.GetString(mass,0 , message_length);
                        string toClient = @"^(\w+):(.+)?:(\w+)$";
                        string toServer = @"^(\w+):(\w+)(:IMG:(.+))*$";
                        Regex toC = new Regex(toClient , RegexOptions.Multiline);
                        Regex toS = new Regex(toServer);
                        if (toS.IsMatch(get))// повідомлення типу КЛІЄНТ-СЕРВЕР?
                        {
                            Match matchS = toS.Match(get);
                            if (matchS.Groups[2].Value == "EXIT")
                            {
                                if (UserDisconnect != null)
                                {
                                    UserDisconnect(this , new UserEventArgs(User.GetName()));
                                }
                                GroupOfConnection.Remove(User);
                                foreach (NewConnection con in GroupOfConnection)
                                {
                                    if (con != User)
                                    {
                                        con.Stream.Write(mass, 0, message_length);
                                    }
                                }
                            }
                            if (matchS.Groups[2].Value == "CONNECT")
                            {
                                string[] sm = get.Split(':');
                                User.SetName(sm[0]);
                                User.Ava = sm[3];
                                if (NewUser != null)
                                {
                                    NewUser(this , new UserEventArgs(User.GetName()));
                                }
                                GroupOfConnection[GroupOfConnection.Count - 1] = User;
                                foreach (NewConnection con in GroupOfConnection)
                                {
                                    if (con != User)
                                    {
                                        con.Stream.Write(mass, 0, message_length);
                                    }
                                }
                                foreach (NewConnection con in GroupOfConnection)
                                {
                                    if (con != User)
                                    {
                                        string sms = con.GetName() + ":" + "CONNECT:IMG:" + con.Ava;
                                        byte[] mess = Encoding.Default.GetBytes(sms);
                                        User.Stream.Write(mess, 0, mess.Length);
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                        else if (toC.IsMatch(get))// повідомлення типу КЛІЄНТ-КЛІЄНТ?
                        {
                            Match m = toC.Match(get);
                            if (SendMessege != null)
                            {
                                SendMessege(this , new MessageEventArgs(User.GetName() , ": відправив повідомлення користувачу " + m.Groups[3].Value + "(" + m.Groups[2].Value + ")"+"\n"));
                            }
                            if (GetMessege != null)
                            {
                                GetMessege(this , new MessageEventArgs(m.Groups[3].Value , ": отримав повідомлення від " + User.GetName() + "(" + m.Groups[2].Value + ")" + "\n"));
                            }
                            foreach (NewConnection con in GroupOfConnection)
                            {
                                if (con != User & con.GetName() == m.Groups[3].Value)
                                {
                                    con.Stream.Write(mass, 0, message_length);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    
                }
            }
        }
        public static void FinishStreamWithUser(IAsyncResult resobj)
        {
            NewConnection CONNECTION = (NewConnection)resobj.AsyncState;
            if (CONNECTION.Client.Connected)
            {
                CONNECTION.StopThreading();
            }
        }
        public int HowMachAreConnected()
        {
            return GroupOfConnection.Count;
        }
        public void DisposeUser()
        {
            string message = "SERVERCLOSE";
            byte[] MESS = Encoding.Default.GetBytes(message);
            foreach (NewConnection con in GroupOfConnection)
            {
                    con.Stream.Write(MESS, 0, MESS.Length);
            }
        }
        public void CloseServer()
        {
            for (int i = 0; i < GroupOfConnection.Count; i++)
            {
                GroupOfConnection[i].StopThreading();
            }
            GroupOfConnection.Clear();
            for (int i = 0; i < GroupOfConnection.Count; i++)
            {
                ThreadConnextion[i].EndInvoke(GroupOfConnection[i].Result);
            }
        }
    }
}







