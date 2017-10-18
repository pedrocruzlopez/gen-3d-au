using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D.TablaSimbolos
{
    public class tablaDeSimbolos
    {

       public List<Simbolo> tabla;


        public tablaDeSimbolos()
        {
            this.tabla = new List<Simbolo>();
        }

        public Boolean addSimbolo(Simbolo nuevo)
        {

            return false;

        }


        public void addLista(List<Simbolo> lista)
        {
            foreach (Simbolo item in lista)
            {
                tabla.Add(item);
            }
        }





    }
}
