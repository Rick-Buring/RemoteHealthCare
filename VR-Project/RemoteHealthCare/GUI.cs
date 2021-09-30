using System;
using System.Collections.Generic;
using System.Text;

namespace Vr_Project.RemoteHealthcare
{
    public class GUI
    {

        public GUI ()
        {
            Console.CursorVisible = false;
        }

        /// <summary>
        /// Deze methode wordt gebruikt om te schrijven in een console gui.
        /// </summary>
        /// <param name="text">De tekst die moet worden opgeschreven.</param>
        public void write (string text)
        {
            //Console.Clear();
            Console.Write(text + "__________________\n");
        }
    }
}
