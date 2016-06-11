using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Rejestr
    {
        public string nazwaProgramu;
        public string wydawca;
        public string sciezkaProgramu;
        public string sciezkaDezinstalacji;
        public Rejestr(string nP, string w, string sP, string sD)
        {
            nazwaProgramu = nP;
            wydawca = w;
            sciezkaProgramu = sP;
            sciezkaDezinstalacji = sD;
        }
    }
}
