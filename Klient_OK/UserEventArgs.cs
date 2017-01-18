using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public class UserEventArgs:EventArgs
    {
        private string user_name;
        private string avatar;
        public UserEventArgs(string name , string ava)
        {
            user_name = name;
            avatar = ava;
        }
        public UserEventArgs(string name)
        {
            user_name = name;
            avatar = "";
        }
        public string Avatar
        {
            set
            {
                avatar = value;
            }
            get
            {
                return avatar;
            }
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
    }
}
