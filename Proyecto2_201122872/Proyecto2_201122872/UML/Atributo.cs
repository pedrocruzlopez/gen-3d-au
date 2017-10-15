using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public  class Atributo
    {
        public string visibilidad;
        public string nombre;
        public string tipo;
        








        public Atributo(string visibiliad, string nombre, string tipo)
        {
            this.visibilidad = visibiliad;
            this.nombre = nombre;
            this.tipo = tipo;
        }



        public int getSize()
        {
            switch (tipo)
            {

            }

            return 1;
        }



        public string getNombre()
        {
            return this.nombre;
        }

        private string getValVisibilidad(){
            switch (visibilidad.ToUpper())
            {
                case "PUBLICO":
                    return "+";
                case "PRIVADO":
                    return "-";
                case "PROTEGIDO":
                    return "#";

            }
            return "";
        }


        public string getCadenaAtributo()
        {
            return getValVisibilidad() + " " + this.nombre + ":  " + this.tipo;
        }


       /*------------- traduccion a codigo ------------------*/

        public string getCodigoJava()
        {
            return this.visibilidad + " " + this.tipo + " " + this.nombre + " ;";
        }


        public string getCodigoPython()
        {
            return this.visibilidad + " " + this.tipo + " " + this.nombre + "\n";
        }


       



    }
}
