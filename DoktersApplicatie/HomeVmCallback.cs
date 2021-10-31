using System;
using System.Collections.Generic;
using System.Text;

namespace DoktersApplicatie
{
    public interface HomeVmCallback
    {
        void openHistoryWindow(string[] clients);
    }
}
