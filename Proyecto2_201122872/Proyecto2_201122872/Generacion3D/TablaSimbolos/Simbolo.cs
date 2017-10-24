using Proyecto2_201122872.Generacion3D.TablaSimbolos.Arreglos;
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
        public int noDimensiones;
        public listaDimensiones dimenesiones;


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


        public Simbolo(string visibilidad, string nombre, string tipo, string tipoElemento, string ambito, string rol, int apuntado, int tamanho, int noDimen, listaDimensiones dimensiones)
        {

            this.visibilidad = visibilidad;
            this.nombreReal = nombre;
            this.tipo = tipo;
            this.ambito = ambito;
            this.rol = rol;
            this.apuntador = apuntado;
            this.tamanho = tamanho;
            this.tipoElemento = tipoElemento;
            this.noDimensiones = noDimen;
            this.dimenesiones = dimensiones;

        }
      







    }
}
