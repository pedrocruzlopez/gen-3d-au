using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Generacion3D.TablaSimbolos
{
    public class Simbolo
    {

        public string visibilidad;
        public string nombreReal;
        public string tipoElemento;
        public string tipo;
        public string ambito;
        public string rol;
        public int apuntador;
        public int tamanho;


        public Simbolo(string visibilidad, string nombre, string tipo,string tipoElemento, string ambito, string rol, int apuntado, int tamanho )
        {

            this.visibilidad = visibilidad;
            this.nombreReal = nombre;
            this.tipo = tipo;
            this.ambito = ambito;
            this.rol = rol;
            this.apuntador = apuntado;
            this.tamanho = tamanho;
            this.tipoElemento = tipoElemento;
        }



      







    }
}
