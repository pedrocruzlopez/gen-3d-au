using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;
using Proyecto2_201122872.Interprete3D.Ejecucion3D;
using Proyecto2_201122872.Errores;

namespace Proyecto2_201122872.Interprete3D
{
    class AccionInterprete
    {
        public  double[] Pila = new double[1000];         
        public double[] Heap = new double[1000];
        public  String Imprimir ;
        public ListaTemporales temporales;
        private string ir_a ;
        private string etiqueta ;
        private int bandera ;

        public AccionInterprete()
        {
            this.temporales = new ListaTemporales();
            Pila = new double[10000];
            Heap = new double[30000];
            Imprimir = "";
            ir_a = "";
            etiqueta = "";
            bandera = 0;

        }

        private void imprimir(String cadena){
            Imprimir += cadena + "\n";
        }

        public void Evaluar(ParseTreeNode raiz, String nombreMain){
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
                if(!val.ToString().Equals("nulo"))
                temporales.agregarTemp(new Temporal(id, Pila[aux]));
                else
                {
                    ErrorA er = new ErrorA("Semantico", "Ocurrio un error a realizar la opeacino", nodo.FindToken());
                    Form1.errores.addError(er);
                }
            }
            else if (modo ==1)
            {// stack.Rule = ToTerm("STACK") + "[" + EXPRESION + "]" + "=" + EXPRESION + ";";
                Object indice = resolverExp(nodo.ChildNodes[0]);
                Object valor = resolverExp(nodo.ChildNodes[1]);
                 if(!valor.ToString().Equals("nulo") &&
                     !indice.ToString().Equals("nulo"))
                 {
                     int ind = (int)indice;
                     Pila[ind] =(double) valor;

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
                if (!esNula(val)){
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










        #region resolver Condicion



        #endregion



        #region resolverExp

        private Object resolverExp(ParseTreeNode nodo)
        {
            switch (nodo.Term.Name)
            {
                case ConstantesInterprete.ENTERO:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }
                case ConstantesInterprete.DECIMAL:
                    {
                        return double.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case ConstantesInterprete.ID:
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

                        switch (nodo.ChildNodes[1].Term.Name)
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

            private bool esDouble(Object val){
                return val is Double;
            }

        private bool esInt(Object val){
            return val is int;
        }

        #endregion


    }
}
