using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
    public class Clase
    {
        private string nombre;
        private string herencia;
        public listaAtributos atributos;
        public listaFunciones funciones;
        public string lenguaje;


        public Boolean esNula()
        {
            return (this.nombre==null);

        }


        public void setLenguaje(string l)
        {
            this.lenguaje = l;
        }


        private void iniciarValores()
        {
           
            this.atributos = new listaAtributos();
            funciones = new listaFunciones();
        }


        public Clase()
        {
            iniciarValores();
            
        }

        public Clase(String nombre)
        {
            this.nombre = nombre;
            iniciarValores();
        }

        public Clase(String nombre, String herencia)
        {
            this.nombre = nombre;
            this.herencia = herencia;
            iniciarValores();
        }


        public String getNombre()
        {
            return this.nombre;
        }



        /*------------ Atibutos ----------------------*/

        public Boolean addAtributo(Atributo atr)
        {
            return this.atributos.addAtributo(atr);
        }


        public int sizeAtributos()
        {
            return this.atributos.atributos.Count;
        }
        /*------------- Funciones --------------------*/

        public Boolean addFuncion(Funcion nueva)
        {
            return this.funciones.addFuncion(nueva);
        }




        /*------------- RElaciones ----------------------*/






        /*------- Traduccion a codigo ---------------*/

        public string getCodigoJava()
        {
            string cad = "Clase " + this.nombre;
            if (this.herencia != null || this.herencia != "")
                cad += " " + this.herencia;
            cad += " {\n"+ this.atributos.getCodigoJava()+"\n"+this.funciones.getCodigoJava()+"\n}";
            return cad;

        }



        public string getCodigoPython()
        {
            string cad = "Clase "+ this.nombre+" [";
            if (this.herencia != null || this.herencia != "")
                cad += " " + this.herencia;
            cad += " ]:\n"+ this.atributos.getCodigoPython()+"\n"+this.funciones.getCodigoPython();
            return cad;
        }



        /*------------ Graphviz---------------------*/

        public string getCadenaGraphivz()
        {
           
            return "[shape=record,label=\"{ "+ nombre+"|{"+atributos.getCadenaGraphivz()+"}|{"+funciones.getCadenaGrapvhiz()+"}}\"];";

        }




    }
}
