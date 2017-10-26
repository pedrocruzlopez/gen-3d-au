using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Interprete3D.Ejecucion3D
{
    public class ListaTemporales
    {
        public List<Temporal> listaTemporales;

        public ListaTemporales()
        {
            this.listaTemporales = new List<Temporal>();
        }


        public int getSize()
        {
            return this.listaTemporales.Count;
        }


        public object getValorTemp(String nombre)
        {
            foreach (Temporal item in this.listaTemporales)
            {
                if (item.nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return item.valor;
                }
            }
            return null;
        }

        public Boolean exiteTemp(Temporal temp)
        {
            foreach (Temporal item in listaTemporales)
            {
                if (item.nombre.Equals(temp.nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


        public void agregarTemp(Temporal temp)
        {
            if (getSize() > 0)
            {
                if (!exiteTemp(temp))
                {
                    listaTemporales.Add(temp);
                }else{
                    modificarTemp(temp);
                }

            }
            else
            {
                listaTemporales.Add(temp);
            }
        }

        public void modificarTemp(Temporal temp)
        {
            if (exiteTemp(temp))
            {
                foreach (Temporal item in this.listaTemporales)
                {
                    if (item.nombre.Equals(temp.nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        item.valor = temp.valor;
                        break;
                    }
                }
            }
        }


    }
}
