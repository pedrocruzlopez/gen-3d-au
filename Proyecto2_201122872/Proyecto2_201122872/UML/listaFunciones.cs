using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
    public class listaFunciones
    {
        public List<Funcion> funciones;

        public listaFunciones()
        {
            this.funciones = new List<Funcion>();
        }


        public Boolean addFuncion(Funcion nueva)
        {
            foreach (Funcion item in funciones)
            {
                if (item.firma.ToUpper().Equals(nueva.firma.ToUpper()))
                {
                    return false;
                }

            }

            this.funciones.Add(nueva);
            return true;
        }



        public string getCadenaGrapvhiz()
        {

            string cad = "";
            Funcion actual;
            for (int i = 0; i < funciones.Count; i++)
            {
                actual = funciones.ElementAt(i);
                if (i == funciones.Count - 1)
                    cad += actual.getCadenaGraphivz();
                else
                    cad += actual.getCadenaGraphivz() + "\\n";
            }

            return cad;

        }


        public Boolean hayPrincipal()
        {
            foreach (Funcion item in this.funciones)
            {
                if (item.esPrincipal)
                    return true;
            }
            return false;
        }



        /*traduccion de codigo */


        public string getCodigoJava()
        {
            string cad = "";
            foreach (Funcion item in this.funciones)
            {
                cad += item.getCodigoJava() + "\n\n";
            }

            return cad;
        }

        public string getCodigoPython()
        {
            string cad = "";
            foreach (Funcion item in this.funciones)
            {
                cad += "\t"+item.getCodigoPython();
            }

            return cad;
        }

    }
}
