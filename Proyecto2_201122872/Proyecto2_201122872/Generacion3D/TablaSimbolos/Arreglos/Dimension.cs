using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D.TablaSimbolos.Arreglos
{
    public class Dimenision
    {
        public int inf;
        public int sup;
        public int n;


        public Dimenision(int n)
        {
            this.n = n;
            this.inf = 0;
            this.sup= n-1;
        }
    }


}
