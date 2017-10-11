using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Errores
{
    public class ErrorA
    {
        public string tipoError;
        public int fila;
        public int posicion;
        public int columna;
        public string mensaje;


        public ErrorA(string tipo, int fila, int pos, int col, string mensaje)
        {
            this.tipoError = tipo;
            this.fila = fila;
            this.posicion = pos;
            this.columna = col;
            this.mensaje = mensaje;
        }



    }
}
