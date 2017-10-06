using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public  class Atributo
    {
        private string visibilidad;
        private string nombre;
        private string tipo;


        public Atributo(string visibiliad, string nombre, string tipo)
        {
            this.visibilidad = visibiliad;
            this.nombre = nombre;
            this.tipo = tipo;
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


    }
}
