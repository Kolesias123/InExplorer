using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InExplorer.Kernel
{
    static class Core
    {
        public static string Random(int lenght = 0, bool letters = true, bool numbers = false, bool other = false)
        {
            Random rNum = new Random();
            string data = "";

            string possible = "";
            int i = 0;

            if (letters)
                possible += "abcdefhijkl";

            if (numbers)
                possible += "0123456789";

            if (other)
                possible += "ABCDEFHIJKL@%&^*/(){}-_";

            int pLength = possible.Length;

            while (i < lenght)
            {
                data += possible.Substring(rNum.Next(0, pLength - 1), 1);
                i++;
            }

            return data;
        }

        public static int Time()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
