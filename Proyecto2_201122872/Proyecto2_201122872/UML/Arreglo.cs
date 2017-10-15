using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public class Arreglo
    {

       public string nombre;
       public string tipo;
       public int noDimensiones;
       public int[] sizeDimensiones;



       public Arreglo(string nombre, string tipo, int dimensiones)
       {
           this.nombre = nombre;
           this.tipo = tipo;
           this.noDimensiones = dimensiones;
           this.sizeDimensiones = new int[dimensiones];

       }

       


    }
}
