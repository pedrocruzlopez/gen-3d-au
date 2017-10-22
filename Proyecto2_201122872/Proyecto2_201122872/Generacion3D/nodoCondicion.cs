using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D
{
   public class nodoCondicion
    {
       public List<String> etiquetasFalsas;
       public List<String> etiquetasVerdaderas;
       public string codigo;



       public nodoCondicion(string codigo)
       {
           this.codigo = codigo;
           this.etiquetasFalsas = new List<string>();
           this.etiquetasVerdaderas = new List<string>();
       }


       public void addEtiquetaFalsa(string et){
           this.etiquetasFalsas.Add(et);
       }

       public void addEtiquetaVerdadera(string et)
       {
           this.etiquetasVerdaderas.Add(et);
       }


       public void addEtiquetasVerdaderas(List<String> lista)
       {
           foreach (string item in lista)
           {
               this.etiquetasVerdaderas.Add(item);
           }
       }


       public void addEtiquetasFalsas(List<String> lista)
       {
           foreach (string item in lista)
           {
               this.etiquetasFalsas.Add(item);
           }
       }


       public string getEtiquetasVerdaderas()
       {
           string cad = "";
           for (int i = 0; i < this.etiquetasVerdaderas.Count; i++)
           {
               cad += etiquetasVerdaderas.ElementAt(i) + ":";
           }
           return cad;
       }

       public string getEtiquetasFalsas()
       {
           string cad = "";
           for (int i = 0; i < this.etiquetasFalsas.Count; i++)
           {
               cad += etiquetasFalsas.ElementAt(i) + ":";
           }
           return cad;
       }



    }
}
