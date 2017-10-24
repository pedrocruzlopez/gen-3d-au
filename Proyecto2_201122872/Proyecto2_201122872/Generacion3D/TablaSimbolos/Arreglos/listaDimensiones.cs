using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D.TablaSimbolos.Arreglos
{
    public class listaDimensiones
    {
        public List<Dimenision> dimensiones;


        public listaDimensiones(List<int> posiciones)
        {
            this.dimensiones = new List<Dimenision>();
            Dimenision temporal;
            foreach (int item in posiciones)
            {
                temporal = new Dimenision(item);
                dimensiones.Add(temporal);
            }

       
        }

        public int getLinealizado()
        {
            int val = 0;
            Dimenision temp;
            for (int i = 0; i < dimensiones.Count; i++)
            {
                temp= dimensiones.ElementAt(i);
                if (i == 0)
                {//la primera vez
                    val = val + (temp.sup - temp.inf);
                }
                else
                {
                    val = val * temp.n + (temp.sup - temp.inf);
                }

            }

            return val;
        }


    }
}
