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
        public UserEventArgs(string name)
        {
            user_name = name;
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
