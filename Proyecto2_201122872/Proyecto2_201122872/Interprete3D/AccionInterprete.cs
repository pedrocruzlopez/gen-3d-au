using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;

namespace Proyecto2_201122872.Interprete3D
{
    class AccionInterprete
    {
        public static double[] Pila = new double[1000];         
        public static double[] Heap = new double[1000];
        public static String Imprimir = "";

        private void imprimir(String cadena){
            Imprimir += cadena + "\n";
        }




        public void Evaluar(ParseTreeNode raiz, String nombreMain){



        }


    }
}
