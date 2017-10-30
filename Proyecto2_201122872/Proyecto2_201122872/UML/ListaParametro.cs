using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
    public class ListaParametro
    {
        public List<variable> parametros;



        public ListaParametro()
        {
            this.parametros= new List<variable>();
        }


        public string identificadorParametros()
        {
            string re = "";
            foreach (variable item in parametros)
            {
                re += item.tipo.ToUpper();
                
            }

            return re;
        }

        public Boolean addParametro(variable n)
        {
            variable actual;
            for (int i = 0; i < this.parametros.Count; i++)
            {
                actual = this.parametros.ElementAt(i);
                if (actual.nombre.ToUpper().Equals(n.nombre.ToUpper()))
                {
                    return false;
                }

            }

            this.parametros.Add(n);
            return true;
        }



        public string getFirma()
        {
            string cad = "";
            variable actual;
            for (int i = 0; i < parametros.Count; i++)
            {
                actual = parametros.ElementAt(i);
                if (i == parametros.Count - 1)
                {
                    cad += actual.tipo;
                }
                else
                {
                    cad += actual.tipo + "_";
                }
            }
            return cad;
        }


        public string getCadenaGraphivz()
        {
            string cad = "";
            variable actual;
            for (int i = 0; i <parametros.Count; i++)
            {
                actual = parametros.ElementAt(i);
                if (i == parametros.Count - 1)
                    cad += actual.getCadenaGraphivz();
                else
                    cad += actual.getCadenaGraphivz() + ", ";
                
            }
            return cad;
        }


        /*   traduccion a codigo */


        public string getCodigoJavaPython()
        {
            string cad = "";
            variable actual;
            for (int i = 0; i <this.parametros.Count; i++)
            {
                actual = this.parametros.ElementAt(i);
                if (i == parametros.Count - 1)
                {
                    cad += actual.getCodigoParametroJavaPython();
                }
                else
                {
                    cad += actual.getCodigoParametroJavaPython() + ", ";
                }
            }

            return cad;
        }

    }
}
