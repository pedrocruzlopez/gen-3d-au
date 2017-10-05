using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public  class clasesDiagrama
    {
        public LinkedList<Clase> listaClases;


        public clasesDiagrama()
        {
            this.listaClases = new LinkedList<Clase>();
        }


        private Boolean existe(String nombre)
        {
            foreach (Clase item in this.listaClases)
            {
                if (item.getNombre().ToUpper().Equals(nombre.ToUpper()))
                    return true;
            }

            return false;
        }


        public Boolean insertar(Clase nueva)
        {
            if(!existe(nueva.getNombre()))
            {
                this.listaClases.AddLast(nueva);
                return true;
            }
            return false;
        }


    }
}
