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
        private LinkedList<Funcion> funciones;
        private LinkedList<Atributo> atributos;
        
        


        public Clase(String nombre)
        {
            this.nombre = nombre;
        }



        public String getNombre()
        {
            return this.nombre;
        }



    }
}
