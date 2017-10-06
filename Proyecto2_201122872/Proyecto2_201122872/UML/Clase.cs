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



        private void iniciarValores()
        {
            this.atributos = new listaAtributos();
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




        /*------------- RElaciones ----------------------*/


    }
}
