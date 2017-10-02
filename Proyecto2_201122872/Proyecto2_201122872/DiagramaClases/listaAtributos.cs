using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.DiagramaClases
{
    class listaAtributos
    {
        private LinkedList<Atributo> lAtributos;

        public listaAtributos()
        {
            this.lAtributos = new LinkedList<Atributo>();
        }

        private Boolean existe(String nombre)
        {
            foreach (Atributo item in lAtributos)
            {
                if (item.getNombre().Equals(nombre))
                {
                    return true;
                }
            }

            return false;
        }


        public void insertar(Atributo nuevo)
        {
            if (!existe(nuevo.getNombre()))
            {
                this.lAtributos.AddLast(nuevo);
            }
            else
            {
                //error el atributo existe
            }
        }






    }
}
