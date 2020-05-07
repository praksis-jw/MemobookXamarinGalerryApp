using System;
using System.Collections.Generic;
using System.Text;

namespace Memobook
{
    class Encryption
    {

        public void SecretEncryptor(string EventId, string onedriveID, out string encrypted)
        {
            string encrypt = EventId + onedriveID;
            string ecrypted = "";

            for (int k = 0; k < encrypt.Length; k++)
            {
                if (k % 2 == 0)
                {
                    ecrypted += Convert.ToChar((int)encrypt[k] + 2);
                }
                if (k % 2 == 1)
                {
                    ecrypted += Convert.ToChar((int)encrypt[k] + 1);
                }
            }
            encrypted = Reverse(ecrypted);
        }


    public void SecretDecryptor(string secretpassword, out string EventId, out string onedriveID)
        {
            secretpassword = Reverse(secretpassword);
            string wynik = "";

            for (int k = 0; k < secretpassword.Length; k++)
            {
                if (k % 2 == 0)
                {
                    wynik += Convert.ToChar((int)secretpassword[k] - 2);
                }
                if (k % 2 == 1)
                {
                    wynik += Convert.ToChar((int)secretpassword[k] - 1);
                }
            }


            string x1 = "";
            string y1 = "";
            x1 = wynik.Substring(0, 36);
            y1 = wynik.Substring(36, wynik.Length - 36);
            int x = 0;
            int y = 0;

            EventId = x1;
            onedriveID = y1;
        }


        string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
