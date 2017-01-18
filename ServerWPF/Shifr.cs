using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWPF
{
    public static class Shifr
    {
        private const string password = "1976111SZ";
        public static string Coder(string line)
        {
            byte[]mass=Encoding.Default.GetBytes(line);
            byte[] key = Encoding.Default.GetBytes(password);
            for(int i=0 ; i<mass.Length ; i++)
                for(int j=0 ; j<key.Length; j++)
                {
                    mass[i]^=key[j];
                }
            string code_str = Encoding.Default.GetString(mass);
            return code_str;
        }
        public static string DeCoder(string line , string pass)
        {
            byte[] mass = Encoding.Default.GetBytes(line);
            byte[] key = Encoding.Default.GetBytes(pass);
            for (int i = 0; i < mass.Length; i++)
                for (int j = 0; j < key.Length; j++)
                {
                    mass[i] ^= key[j];
                }
            string code_str = Encoding.Default.GetString(mass);
            return code_str;
        }
    }
}
