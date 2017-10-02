using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.DiagramaClases
{
    class Parametro
    {
        private string tipo;
        private String nombre;


        public Parametro(String tipo, String nombre)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }

        public String getNombre()
        {
            return this.nombre;
        }

    }
}
