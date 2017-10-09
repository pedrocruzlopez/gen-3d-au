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
        public listaAtributos atributos;
        public listaFunciones funciones;


        public Boolean esNula()
        {
            return (this.nombre==null);

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





        /*------------ Graphviz---------------------*/

        public string getCadenaGraphivz()
        {
            //[shape=record,label="{ Estudiante |{+ atri1: int \n# atri2:string }| metodo1 \n metodo2 }"];

            return "[shape=record,label=\"{ "+ nombre+"|{"+atributos.getCadenaGraphivz()+"}|{"+funciones.getCadenaGrapvhiz()+"}}\"];";

        }

    }
}
