using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Kursach
{
    class Setting
    {
        private string ip;
        private string port;
        private string user_Name;
        private System.Drawing.Image ava;
        public Setting(string _ip, string _port, string _user_name)
        {
            this.ip = _ip;
            this.port = _port;
            this.user_Name = _user_name;
        }
        public Setting(string _ip , string _port , string _user_name , System.Drawing.Image Ava)
        {
            this.ip = _ip;
            this.port = _port;
            this.user_Name = _user_name;
            this.ava = Ava;
        }
        public string GetIP()
        {
            return this.ip;
        }
        public string GetPort()
        {
            return this.port;
        }
        public string GetUser()
        {
            return this.user_Name;
        }
        public System.Drawing.Image GetAva() { return ava; }
        public void SetAva(System.Drawing.Image img) 
        {
            ava = img; 
        }
        public void SetIP(string str)
        {
            this.ip = str;
        }
        public void SetPort(string str)
        {
            this.port = str;
        }
        public void SetUser(string str)
        {
            this.user_Name = str;
        }
        public bool IsIP(string str)
        {
            Regex reg_IP = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])){3}$");
            if (!reg_IP.IsMatch(str))
            {
                return false;
            }
            else return true;
        }
        public bool IsPort(string str)
        {
            Regex reg_Port = new Regex(@"^(([0-9]{1,4})|([1-5][0-9]{4})|(6[0-4][0-9]{3})|(65[0-4][0-9]{2})|(655[0-2][0-9])|(6553[0-5]))$");
            if (!reg_Port.IsMatch(str))
            {
                return false;
            }
            else return true;
        }
        public bool SaveSetting()
        {
            string currDir = Environment.CurrentDirectory.ToString();
            string path = @"" + currDir + "Setting.txt";
            string setting_line = "";
            setting_line = user_Name + ":" + ip + ":" + port;
            File.WriteAllText(path, setting_line);
            MemoryStream ms = new MemoryStream();
            ava.Save(ms, ava.RawFormat);
            byte[] imarr = ms.ToArray();
            string ava_string = Convert.ToBase64String(imarr);
            File.WriteAllText(@"" + currDir + "Image.txt", ava_string);
            ms.Close();
            return true;
        }
    }
}
