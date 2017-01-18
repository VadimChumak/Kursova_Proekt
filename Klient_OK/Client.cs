using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;
using ServerWPF;
namespace Kursach
{
    class Client
    {
        private NetworkStream _stream;
        private TcpClient _client;
        private IPAddress _ip;
        private string _name;
        private int _port;
        public delegate void ServerEvent(object server, EventArgs e);
        public delegate void UserEvent(object sender, UserEventArgs e);
        public delegate void MessageEvent(object sender, MessageEventArgs e);
        public event ServerEvent ServerDisconnect;
        public event UserEvent NewUserConnectEvent;
        public event UserEvent UserDisconnectEvent;
        public event MessageEvent GetMessageEvent;
        public Client()
        {
            _client = null;
            _ip = IPAddress.Parse("1.1.1.1");
            _port = 1111;
            _stream =null;
            _name = "____";
        }
        public Client(Setting person)
        {
            _ip = IPAddress.Parse(person.GetIP());
            _port = int.Parse(person.GetPort());
            _client = new TcpClient();
            _name = person.GetUser();
        }
        public void SendMessage(string message)
        {
            string str = "";
            str =_name+":"+message;
            _stream = _client.GetStream();
            byte[] line = Encoding.Default.GetBytes(str);
            _stream.Write(line, 0, line.Length);
        }
        public string GetMessage()
        {
            _stream = _client.GetStream();
            byte[] line = new byte[1000000];
            string message = "";
            int size=_stream.Read(line, 0, line.Length);
            message += Encoding.Default.GetString(line, 0, size);
            return message;
        }
        public void Connect()
        {
            _client = new TcpClient();
            _client.Connect(_ip, _port);
            _stream = _client.GetStream();   
        }

        public void FinishStream()
        {
            _client.Close();
        }
        public bool IsConnected()
        {
            if (_client.Connected) return true;
            else return false;
        }
        public void GetSMS()
        {
            while (IsConnected())
            {
                try
                {
                    string sms = this.GetMessage();
                    if (sms.Length > 0)
                    {
                        string fromServer = @"^(\w+):(\w+)(:IMG:(.+))*$";
                        string toClient = @"^(\w+):(.+)?:(\w+)$";
                        string closeServer = @"SERVERCLOSE";
                        Regex regClient = new Regex(toClient, RegexOptions.Multiline);
                        Regex regServer = new Regex(fromServer);
                        Regex regServerClose = new Regex(closeServer);
                        if (regServerClose.IsMatch(sms))
                        {
                            if (ServerDisconnect != null)
                            {
                                ServerDisconnect(this, null);
                            }
                        }
                        if (regServer.IsMatch(sms))
                        {
                            Match matchS = regServer.Match(sms);
                            if (matchS.Groups[2].Value == "EXIT")
                            {
                                UserEvent user = UserDisconnectEvent;
                                if (user != null)
                                {
                                    user(this, new UserEventArgs(matchS.Groups[1].Value.ToString()));
                                }
                            }
                            if (matchS.Groups[2].Value == "CONNECT")
                            {
                                string[] msg = sms.Split(':');
                                UserEvent user = NewUserConnectEvent;
                                if (user != null)
                                {
                                    user(this , new UserEventArgs(matchS.Groups[1].Value.ToString() ,msg[3]));
                                }
                            }
                        }
                        else if (regClient.IsMatch(sms))
                        {
                            Match mC = regClient.Match(sms);
                            MessageEvent message = GetMessageEvent;
                            if (message != null)
                            {
                                message(this, new MessageEventArgs(mC.Groups[1].Value.ToString(), mC.Groups[2].Value.ToString()));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
