using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteHealthCare
{
    class GUI
    {

        public GUI ()
        {
            Console.CursorVisible = false;
        }

        public void write (string text)
        {
            Console.Clear();
            Console.Write(text);
        }
    }
}
