using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D
{
    public class Codigo
    {
        public string codigo3D;
        public int contTemporales;
        public int contEtiquetas;




        public Codigo()
        {
            this.codigo3D = "";
            this.contEtiquetas = 0;
            this.contTemporales = 0;
        }


        public string getEtiqueta()
        {
            contEtiquetas++;
            return "L" + contEtiquetas;
        }

        public string getTemporal()
        {
            contTemporales++;
            return "t" + contTemporales;
        }

        public void addCodigo(string cod)
        {
            codigo3D += cod + "\n";
        }

    }
}
