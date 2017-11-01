﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;
using Proyecto2_201122872.Interprete3D.Ejecucion3D;
using Proyecto2_201122872.Errores;
using Proyecto2_201122872.AnalizadorJava;

namespace Proyecto2_201122872.Interprete3D
{
    public class AccionInterprete
    {
        public double[] Pila;
        public double[] Heap ;//= new double[1000];
        public String Imprimir;
        public ListaTemporales temporales;
        private string ir_a;
        private string etiqueta;
        private int bandera;
        public ParseTreeNode raiz3D;
        public string metodoInicio = "";
        public String IMPRIMIR_STACK = "";
        public String IMPRIMIR_HEAP = "";

        public AccionInterprete(ParseTreeNode raiz)
        {
            this.temporales = new ListaTemporales();
            Pila = new double[10000];
            Heap = new double[30000];
            Imprimir = "";
            ir_a = "";
            etiqueta = "";
            bandera = 0;
            this.raiz3D = raiz;

        }

        

        public void setMetodoInicio(String val)
        {
            this.metodoInicio = val;
        }

        /* Pasos de ejecucuin:
         * 1. Buscar el metodo princiapl el cual sera indicado por el usuario
         * 
         
         */



         /*for (int i = 0; i < Raiz.jjtGetNumChildren(); i++) {
                SimpleNode aux = (SimpleNode) Raiz.jjtGetChild(i);
                if (aux.toString().equalsIgnoreCase("MAIN")) {
                    Actual = (SimpleNode) aux.jjtGetChild(0);
                    for (int j = 0; j < Actual.jjtGetNumChildren(); j++) {
                        if (bandera == 1) {
                            break;
                        }
                        Instruccion((SimpleNode) aux.jjtGetChild(0).jjtGetChild(j));
                    }
                }
            }*/

        public void ejecutarCodigo()
        {
            Temporal p = new Temporal("P", 0);
            Temporal H = new Temporal("h", 0);
            temporales.agregarTemp(p);
            temporales.agregarTemp(H);

            ParseTreeNode metodoTemporal, cuerpoTemporal;
            for (int i = 0; i < raiz3D.ChildNodes.Count; i++)
            {
                metodoTemporal = raiz3D.ChildNodes[i];
                if (metodoTemporal.ChildNodes[0].Token.ValueString.Equals(metodoInicio, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("entre");
                    cuerpoTemporal = metodoTemporal.ChildNodes[1];
                    for (int j = 0; j <cuerpoTemporal.ChildNodes.Count; j++)
                    {
                        if (bandera == 1)
                        {
                            break;
                        }
                        ejecutarInstruccion(cuerpoTemporal.ChildNodes[j]);
                        
                    }

                }
                
            }
           
        }



        private void imprimir(String cadena)
        {
            Imprimir += cadena + "\n";
        }


        public string imprimir_pila() {
        for (int i = 50; i >= 0; i--) {
            Console.Write(i + ": " + Pila[i]);
            IMPRIMIR_STACK += (i + ":      " + Pila[i] + "\n");
        }
        return IMPRIMIR_STACK;
    }

        public  string imprimir_heap() {
        for (int i = 50; i >= 0; i--) {
            Console.Write(i + ": " + Heap[i]);
            IMPRIMIR_HEAP += (i + ":      " + Heap[i] + "\n");
        }
        return IMPRIMIR_HEAP;
    }

        public void Evaluar(ParseTreeNode raiz, String nombreMain)
        {
            Temporal p = new Temporal("P", 0);
            Temporal H = new Temporal("h", 0);
            //buscamos principal



        }


        private bool esNula(Object val)
        {
            return val.ToString().Equals("nulo", StringComparison.OrdinalIgnoreCase);
        }

        private void asignacionNormal(ParseTreeNode nodo)
        {
            // asignacionNormal.Rule = identificador + ToTerm("=") + EXPRESION + ";";
            string idTemp = nodo.ChildNodes[0].Token.ValueString;
            Object val = resolverExp(nodo.ChildNodes[1]);
            Temporal nuevo = new Temporal(idTemp, val);
            if (idTemp.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                temporales.modificarTemp(nuevo);
            }
            else
            {
                temporales.agregarTemp(nuevo);
            }

        }

        private void pila(ParseTreeNode nodo, int modo)
        {
            if (modo == 0)
            {// identificador + ToTerm("=") + "STACK" + "[" + EXPRESION + "]" + ";";
                string id = nodo.ChildNodes[0].Token.ValueString;
                Object val = resolverExp(nodo.ChildNodes[1]);
                int aux = (int)val;
                if (!val.ToString().Equals("nulo"))
                    temporales.agregarTemp(new Temporal(id, Pila[aux]));
                else
                {
                    ErrorA er = new ErrorA("Semantico", "Ocurrio un error a realizar la opeacino", nodo.FindToken());
                    Form1.errores.addError(er);
                }
            }
            else if (modo == 1)
            {// stack.Rule = ToTerm("STACK") + "[" + EXPRESION + "]" + "=" + EXPRESION + ";";
                Object indice = resolverExp(nodo.ChildNodes[0]);
                Object valor = resolverExp(nodo.ChildNodes[1]);
                if (!valor.ToString().Equals("nulo") &&
                    !indice.ToString().Equals("nulo"))
                {
                    int ind = (int)indice;
                    Pila[ind] = (double)valor;

                }
                else
                {
                    ErrorA er = new ErrorA("Semantico", "Ocurrio un error a realizar la opeacino", nodo.FindToken());
                    Form1.errores.addError(er);
                }

            }


        }


        private void heap(ParseTreeNode nodo, int modo)
        {
            if (modo == 0)
            {
                //asignacionHeap.Rule = identificador + ToTerm("=") + "HEAP" + "[" + EXPRESION + "]" + ";";
                string id = nodo.ChildNodes[0].Token.ValueString;
                Object val = resolverExp(nodo.ChildNodes[1]);
                if (!esNula(val))
                {
                    int indice = (int)val;
                    temporales.agregarTemp(new Temporal(id, indice));

                }
                else
                {
                    ErrorA er = new ErrorA("Semantico", "Ocurrio un error a realizar la opeacino", nodo.FindToken());
                    Form1.errores.addError(er);

                }



            }
            else if (modo == 1)
            {//heap.Rule = ToTerm("HEAP") + "[" + EXPRESION + "]" + "=" + EXPRESION + ";";
                object objIndice = resolverExp(nodo.ChildNodes[0]);
                object valor = resolverExp(nodo.ChildNodes[1]);
                if (esNula(objIndice) && !esNula(valor))
                {
                    int indice = (int)objIndice;
                    double res = (double)valor;
                    Heap[indice] = res;
                }
                else
                {
                    ErrorA er = new ErrorA("Semantico", "Ocurrio un error a realizar la opeacino", nodo.FindToken());
                    Form1.errores.addError(er);

                }


            }

        }

        private bool esIrAEtiqueta()
        {
            return ir_a.Equals(etiqueta, StringComparison.OrdinalIgnoreCase);
        }



        private void EvaluarCuerpo()
        {

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
                        if (esIrAEtiqueta())
                        {
                            asignacionNormal(nodo);
                            break;
                        }

                        break;
                    }

                case "asignacionStack":
                    {
                        //asignacionStack.Rule = identificador + ToTerm("=") + "STACK" + "[" + EXPRESION + "]" + ";";
                        if (esIrAEtiqueta())
                        {
                            pila(nodo, 0);
                            break;
                        }
                        break;

                    }

                case "asignacionHeap":
                    {
                        //asignacionHeap.Rule = identificador + ToTerm("=") + "HEAP" + "[" + EXPRESION + "]" + ";";
                        if (esIrAEtiqueta())
                        {
                            heap(nodo, 0);
                            break;
                        }
                        
                        break;
                    }

                case "etiqueta":
                    {
                        //etiqueta.Rule = identificador + ToTerm(":");

                        string nombreEtiqueta = nodo.ChildNodes[0].Token.ValueString;
                        if (ir_a.Equals(""))
                        {
                            etiqueta = nombreEtiqueta;
                            ir_a = etiqueta;
                        }
                        else
                        {
                            etiqueta = nombreEtiqueta;
                        }
                        break;
                        
 
                    }

                case "llamada":
                    {

                        break;
                    }

                case ConstantesInterprete.salto:
                    {
                        string salto = nodo.ChildNodes[0].Token.ValueString;
                        if (esIrAEtiqueta())
                        {
                            ir_a = salto;
                            EvaluarCuerpo();
                            bandera = 1;
                        }

                        break;
                    }

                case ConstantesInterprete.IF:
                    {

                        break;
                    }

                case ConstantesInterprete.stack:
                    {
                        if (esIrAEtiqueta())
                        {
                            pila(nodo, 1);
                        }

                        break;
                    }
                case ConstantesInterprete.heap:
                    {
                        if (esIrAEtiqueta())
                        {
                            heap(nodo, 1);
                        }
                        break;
                    }
                case ConstantesInterprete.print:
                    {

                        break;
                    }



            }
        }










        #region resolver Condicion



        #endregion



        #region resolverExp

        private Object resolverExp(ParseTreeNode nodo)
        {
            switch (nodo.Term.Name)
            {
                case "TERMINO":
                    {
                        return resolverExp(nodo.ChildNodes[0]);
                    }
                case ConstantesInterprete.ENTERO:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }
                case ConstantesInterprete.DECIMAL:
                    {
                        return double.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case "ID":
                    {
                        string id = nodo.ChildNodes[0].Token.ValueString;
                        return temporales.getValorTemp(id);

                    }
                case ConstantesInterprete.NEGATIVO:
                    {
                        Object valn = resolverExp(nodo.ChildNodes[1]);
                        if (valn is int)
                        {
                            return int.Parse(valn + "") * -1;
                        }
                        if (valn is Double)
                        {
                            return Double.Parse(valn + "") * -1;
                        }
                        return "";

                    }

                case ConstantesInterprete.OPERACION:
                    {

                        #region operaciones
                        Object val1 = resolverExp(nodo.ChildNodes[0]);
                        Object val2 = resolverExp(nodo.ChildNodes[2]);

                        switch (nodo.ChildNodes[1].ChildNodes[0].Term.Name)
                        {

                            case "+":
                                {
                                    if ((esInt(val1) || esDouble(val1)) &&
                                        (esInt(val2) || esDouble(val2)))
                                    {
                                        if (esInt(val2) && esInt(val2))
                                        {
                                            return (int)int.Parse(val1.ToString()) + int.Parse(val2.ToString());
                                        }
                                        else
                                        {
                                            return (Double)double.Parse(val1.ToString()) + double.Parse(val2.ToString());
                                        }

                                    }
                                    else
                                    {
                                        ErrorA n = new ErrorA("Semantico", "Tipos no validos para una suma", nodo.FindToken());
                                        Form1.errores.addError(n);
                                        return "nulo";
                                    }

                                }

                            case "-":
                                {
                                    if ((esInt(val1) || esDouble(val1)) &&
                                        (esInt(val2) || esDouble(val2)))
                                    {
                                        if (esInt(val2) && esInt(val2))
                                        {
                                            return (int)int.Parse(val1.ToString()) - int.Parse(val2.ToString());
                                        }
                                        else
                                        {
                                            return (Double)double.Parse(val1.ToString()) - double.Parse(val2.ToString());
                                        }

                                    }
                                    else
                                    {
                                        ErrorA n = new ErrorA("Semantico", "Tipos no validos para una resta", nodo.FindToken());
                                        Form1.errores.addError(n);
                                        return "nulo";
                                    }

                                }

                            case "*":
                                {
                                    if ((esInt(val1) || esDouble(val1)) &&
                                        (esInt(val2) || esDouble(val2)))
                                    {
                                        if (esInt(val2) && esInt(val2))
                                        {
                                            return (int)int.Parse(val1.ToString()) * int.Parse(val2.ToString());
                                        }
                                        else
                                        {
                                            return (Double)double.Parse(val1.ToString()) * double.Parse(val2.ToString());
                                        }

                                    }
                                    else
                                    {
                                        ErrorA n = new ErrorA("Semantico", "Tipos no validos para una multiplicacion", nodo.FindToken());
                                        Form1.errores.addError(n);
                                        return "nulo";
                                    }

                                }
                            case "/":
                                {
                                    if ((esInt(val1) || esDouble(val1)) &&
                                        (esInt(val2) || esDouble(val2)))
                                    {
                                        if (int.Parse(val2.ToString()) != 0)
                                        {
                                            return double.Parse(val1.ToString()) / double.Parse(val2.ToString());
                                        }
                                        else
                                        {
                                            ErrorA n = new ErrorA("Semantico", "No se puede realizar division por cero", nodo.FindToken());
                                            Form1.errores.addError(n);
                                            return "nulo";
                                        }

                                    }
                                    else
                                    {
                                        ErrorA n = new ErrorA("Semantico", "Tipos no validos para una division", nodo.FindToken());
                                        Form1.errores.addError(n);
                                        return "nulo";
                                    }

                                }

                            case "^":
                                {
                                    if ((esInt(val1) || esDouble(val1)) &&
                                        (esInt(val2) || esDouble(val2)))
                                    {
                                        if (esInt(val2) && esInt(val2))
                                        {
                                            return (int)Math.Pow(int.Parse(val1.ToString()), int.Parse(val2.ToString()));
                                        }
                                        else
                                        {
                                            return (Double)Math.Pow(Double.Parse(val1.ToString()), Double.Parse(val2.ToString()));
                                        }

                                    }
                                    else
                                    {
                                        ErrorA n = new ErrorA("Semantico", "Tipos no validos para una potencia", nodo.FindToken());
                                        Form1.errores.addError(n);
                                        return "nulo";
                                    }

                                }





                        }//fin switch operaciones
                        #endregion


                    }

                    return "nulo";
            }
            return "nulo";

        }

        private bool esDouble(Object val)
        {
            return val is Double;
        }

        private bool esInt(Object val)
        {
            return val is int;
        }

        #endregion


    }
}
