using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Authorization
    {
        internal static readonly string PASSWORDFILE = "Passwords.txt";

        public static bool Authorized(string userName, string password)
        {
            foreach (string line in File.ReadLines(DataManager.appDataHealthCareDirectory +@"\Passwords\" + PASSWORDFILE))
            {
                if (line.Substring(0, line.IndexOf(" ")).Equals(userName))
                {
                    string p = line.Substring(userName.Length + 1);
                    return p.Equals(password);
                }
            }

            return false;
        }
    }
}
