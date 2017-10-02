using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.DiagramaClases
{
    class listaParametros
    {

        private LinkedList<Parametro> lParametros;

         public listaParametros()
        {
            this.lParametros = new LinkedList<Parametro>();
        }

        private Boolean existe(String nombre)
        {
            foreach (Parametro item in lParametros)
            {
                if (item.getNombre().Equals(nombre))
                {
                    return true;
                }
            }

            return false;
        }


        public void insertar(Parametro nuevo)
        {
            if (!existe(nuevo.getNombre()))
            {
                this.lParametros.AddLast(nuevo);
            }
            else
            {
                //error el atributo existe
            }
        }


    }
}
