using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klient_OK
{
    public class UserInformations
    {
        private string mh;
        private string ava;
        public UserInformations()
        {
            mh = "";
            ava = "";
        }
        public string Avatar
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
        public string MessHistory
        {
            get
            {
                return mh;
            }
            set
            {
                mh = value;
            }
        }
    }
}
