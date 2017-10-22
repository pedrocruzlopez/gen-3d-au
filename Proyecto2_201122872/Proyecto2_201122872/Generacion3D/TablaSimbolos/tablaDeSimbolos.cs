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



        public int getPosicionDeClase(string id, Ambitos ambito)
        {
            for (int i = 0; i < ambito.ambitos.Count; i++)
            {
                if (i == ambito.ambitos.Count - 1)
                {
                    string amb = ambito.ambitos.ElementAt(i);
                    foreach (Simbolo simb in this.tabla)
                    {
                        if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                            simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            return simb.apuntador;
                        }
                    }

                }

            }



            return -1;

        }
        public int getPosicion(string id, Ambitos ambito)
        {
            for (int i = 0; i < ambito.ambitos.Count; i++)
            {
                if (i != ambito.ambitos.Count - 1)
                {
                    string amb = ambito.ambitos.ElementAt(i);
                    foreach (Simbolo simb in this.tabla)
                    {
                        if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                            simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            return simb.apuntador;
                        }
                    }

                }
                    
            }
            
            
           
            return -1;
        }

        public string getTipo(string id, Ambitos ambito)
        {
            foreach (String amb in ambito.ambitos)
            {
                foreach (Simbolo simb in this.tabla)
                {
                    if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                        simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        return simb.tipo;
                    }
                }

            }
            return "";
        }



    }
}
