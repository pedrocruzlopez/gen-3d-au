using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.DiagramaClases
{
    class Atributo
    {
        private String visibilidad;
        private String nombre;
        private String tipo;
        private Object valor;

        public Atributo(String visibilidad, String nombre, String tipo)
        {
            this.visibilidad = visibilidad;
            this.nombre = nombre;
            this.tipo = tipo;
        }


        public Atributo(String nombre, String tipo)
        {
            this.visibilidad = "publico";
            this.nombre = nombre;
            this.tipo = tipo;
        }

        public String getNombre(){
            return this.nombre;
        }


    }
}
