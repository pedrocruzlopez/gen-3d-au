﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public class variable:elementoVarArreglo
    {
        public string nombre;
        public string tipo;

        public variable(string nombre, string tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }
        public String getNombreTipoVar()
        {
            return nombre + " -> " + tipo;
        }


        public string getCadenaGraphivz()
        {
            return nombre + ": " + tipo;
        }


       /*traduccion a codigo*/

        public string getCodigoParametroJavaPython()
        {

            return this.tipo + " " + this.nombre;
        }


       
    }
}
