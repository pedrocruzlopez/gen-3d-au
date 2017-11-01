using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter;

namespace Proyecto2_201122872.UML
{
   public  class Atributo
    {
        public ParseTreeNode expresionAtributo = null; 
        public string visibilidad;
        public string nombre;
        public string tipo;
        public string tipoAtributo;
        public int valLinealizado;
        public int noDimensiones;
        public ParseTreeNode[] expresionesDimensiones;
        public Boolean esHeredado = false;

        public Atributo(string visibiliad, string nombre, string tipo, string tipoAtributo)
        {
            this.visibilidad = visibiliad;
            this.nombre = nombre;
            this.tipo = tipo;
            this.tipoAtributo = tipoAtributo;
        }

        public Atributo(string visibilidad, string nombre, string tipo, string tipoAtributo, int noDimensiones, ParseTreeNode[] valores)
        {
            this.visibilidad = visibilidad;
            this.nombre = nombre;
            this.tipo = tipo;
            this.tipoAtributo = tipoAtributo;
            this.noDimensiones = noDimensiones;
            this.expresionesDimensiones = valores;
            this.valLinealizado = getLinealizado();
        }


       public void setExpresionAtributo(ParseTreeNode nodo){
           this.expresionAtributo=nodo;
       }
        private int getLinealizado()
        {
            return 0;
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
