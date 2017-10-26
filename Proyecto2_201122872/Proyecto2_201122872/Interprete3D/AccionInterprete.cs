using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;
using Proyecto2_201122872.Interprete3D.Ejecucion3D;

namespace Proyecto2_201122872.Interprete3D
{
    class AccionInterprete
    {
        public  double[] Pila = new double[1000];         
        public double[] Heap = new double[1000];
        public  String Imprimir = "";
        public ListaTemporales temporales;

        public AccionInterprete()
        {
            this.temporales = new ListaTemporales();
        }

        private void imprimir(String cadena){
            Imprimir += cadena + "\n";
        }

        public void Evaluar(ParseTreeNode raiz, String nombreMain){
            Temporal p = new Temporal("P", 0);
            Temporal H = new Temporal("h", 0);
            //buscamos principal



        }


        private void ejecutarInstruccion(ParseTreeNode nodo)
        {
            switch (nodo.Term.Name)
            {
                case ConstantesInterprete.GLOBAL:
                    {
                        ejecutarInstruccion(nodo.ChildNodes[0]);
                        break;
                    }

                case "asignacionNormal":
                    {
                        break;
                    }

                case "asignacionStack":
                    {
                        break;
                    }

                case "asignacionHeap":
                    {
                        break;
                    }

                case "etiqueta":
                    {
                        break;
                    }

                case "llamada":
                    {
                        break;
                    }

                case ConstantesInterprete.salto:
                    {
                        break;
                    }

                case ConstantesInterprete.IF:
                    {
                        break;
                    }

                case ConstantesInterprete.stack:
                    {
                        break;
                    }
                case ConstantesInterprete.heap:
                    {
                        break;
                    }
                case ConstantesInterprete.print:
                    {
                        break;
                    }



            }
        }


    }
}
