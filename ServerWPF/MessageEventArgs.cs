using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class MessageEventArgs:EventArgs
    {
        private string user_name;
        private string message;
        public MessageEventArgs(string name, string sms)
        {
            user_name = name;
            message = sms;
        }
        public string User_Name
        {
            set
            {
                user_name = value;
            }
            get
            {
                return user_name;
            }
        }
        public string Message
        {
            set
            {
                message = value;
            }
            get
            {
                return message;
            }
        }
    }
}
