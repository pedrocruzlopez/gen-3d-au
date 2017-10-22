using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using Proyecto2_201122872.UML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.AnalizadorPython;
using System.IO;
using Proyecto2_201122872.Errores;

namespace Proyecto2_201122872.Generacion3D
{
    public class GeneracionCodigo
    {

        public tablaDeSimbolos tablaSimbolos;
        public clasesDiagrama uml;
        private Arbol analizadorJava;
        private ArbolPy analizadorPython;
        public Codigo c3d;

        public GeneracionCodigo()
        {
            this.tablaSimbolos = new tablaDeSimbolos();
            uml = new clasesDiagrama();
            analizadorJava = new Arbol();
            analizadorPython = new ArbolPy();
            c3d = new Codigo();
        }


      

        private string getCadenaArchivo(String ruta)
        {
            StreamReader archivo = new StreamReader(ruta);
            string linea;
            string contenido = "";
            while ((linea = archivo.ReadLine()) != null)
            {
                contenido += linea + "\n";
            }

            archivo.Close();
            return contenido;

        }

        public void ejecutarArchivos(String ruta, String extension)
        {
            String contenido = getCadenaArchivo(ruta);
            if (extension.Equals(".tree", StringComparison.OrdinalIgnoreCase))
            {
                
                    uml = analizadorPython.parseConvertirUML2(contenido);
                    generarTablaSimbolos();

               
                
                
            }
            else if (extension.Equals(".olc", StringComparison.OrdinalIgnoreCase))
            {
               uml= analizadorJava.parseConvertirUML2(contenido);
               generarTablaSimbolos();
               
            }

        }

        public void generarTablaSimbolos()
        {
            if (uml != null)
            {
                foreach (Clase clase in uml.listaClases)
                {
                    this.tablaSimbolos.addLista(clase.getSimbolosClase());
                }
            }
        }




        /*------------------------------------- Generacion de codigo -------------------------------------*/
        
        public void escribirC3DClases()
        {
            Ambitos ambito = new Ambitos();
            /*
             * 1. Creacion de constructores si los tuviera sino se crea uno por defecto = new persona() <----
             * 2. Creacion de metodo principal si los tuviera sino asi se queda
             * 3. Creacion de las demas funciones
             */

            foreach (Clase claseActual in uml.listaClases)
            {
                //1. creacion de constructores
                ambito.addAmbito(claseActual.getNombre());
                List<Funcion> constructores = claseActual.getConstructores();
                Funcion principal;
                List<Funcion> funcionesMetodos;
                if (constructores.Count > 0)
                {
                    escribirConstructores(constructores, ambito);

                }
                else
                {
                    //no posee constructor


                }
                //2. Creacion del metodo principal, si este lo tuviese

                principal = claseActual.getPrincipal();
                if (principal != null)
                {
                    ambito.addAmbito(principal.firma);
                    c3d.addCodigo(Constantes.tipoVoid + " " + principal.firma + "{");
                    evaluarCuerpo(principal.cuerpo, ambito);
                    c3d.addCodigo("}");
                    ambito.ambitos.Pop();
                }

                //3. escribir los demas metodos y funciones

                funcionesMetodos = claseActual.getFunciones();
                escribirConstructores(funcionesMetodos, ambito);
                ambito.ambitos.Pop();

            }



        }


        

        private void escribirConstructores(List<Funcion> constructores, Ambitos ambito)
        {
            foreach (Funcion constructor in constructores)
            {
                ambito.addAmbito(constructor.firma);//ingresando el ambito del constructor
                c3d.addCodigo(Constantes.tipoVoid + " " + constructor.firma + "{");
                evaluarCuerpo(constructor.cuerpo, ambito);
                c3d.addCodigo("}");
                ambito.ambitos.Pop();//saliendo del ambito del constructor
            }
        }



        #region generacion c3d cuerpo

        public void evaluarCuerpo(ParseTreeNode nodo, Ambitos ambitos)
        {

            switch (nodo.Term.Name)
            {

                #region inicialesCuerpo
                case Constantes.cuerpo:
                    {
                        if (nodo.ChildNodes.Count > 0)
                            evaluarCuerpo(nodo.ChildNodes[0], ambitos);
                        else
                            break;
                        break;
                    }
                case Constantes.instrucciones:
                    {
                        foreach (ParseTreeNode nodoHijo in nodo.ChildNodes)
                        {
                            evaluarCuerpo(nodoHijo, ambitos);
                        }
                        break;
                    }
                case Constantes.instruccion:
                    {
                        evaluarCuerpo(nodo.ChildNodes[0], ambitos);
                        break;
                    }

                #endregion

                #region asignacion


                   case Constantes.asignacion:{
                       /*
                       int noHijos1 = nodo.ChildNodes[0].ChildNodes.Count;
                       int noHijos2 = nodo.ChildNodes[1].ChildNodes.Count;
                       if (noHijos1 == 1)
                       {//debe ser una variable o un arreglo
                           ParseTreeNode nodoVar= nodo.ChildNodes[0]; 
                           if (string.Equals(nodoVar.ChildNodes[0].Term.Name, Constantes.id, StringComparison.OrdinalIgnoreCase))
                           {
                               string nombre = nodoVar.ChildNodes[0].Token.ValueString;
                               int pos = tablaSimbolos.getPosicion(nombre, ambitos);
                               string tipo = tablaSimbolos.getTipo(nombre, ambitos);
                               if (pos == -1)
                               {//buscar si es una variable de clase
                                   pos = tablaSimbolos.getPosicionDeClase(nombre, ambitos);
                                   tipo = tablaSimbolos.getTipo(nombre, ambitos);
                                   if (pos == -1)
                                   {
                                       //vairable no existe
                                   }
                                   else
                                   {

                                   }
                               }
                               else
                               {

                               }
                           }
                           else if (string.Equals(nodoVar.ChildNodes[0].Term.Name, Constantes.declaArreglo, StringComparison.OrdinalIgnoreCase))
                           {

                           }
                           else
                           {
                               //error
                           }

                       }
                       else { }

                       */
                       break;

                    }

                #endregion


                   #region Declaraciones

                   case Constantes.declaracion:
                       {//declaracion para variables de pythodn , lo unicio que se debe hacer es calcular el tamanho  de un arreglo

                           break;
                       }

                   case Constantes.decla2:
                       {
                           /* 
            DECLARACION.Rule = TIPO + identificador + ToTerm(";")
                
                | TIPO + identificador + LPOSICIONES + ToTerm(";")
                | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";";
                            */
                           int noHijos = nodo.ChildNodes.Count;

                           if (noHijos == 3)
                           {
                               if (!nodo.ChildNodes[2].Term.Name.Equals(Constantes.lposiciones, StringComparison.OrdinalIgnoreCase) &&
                                   nodo.ChildNodes[2].ChildNodes.Count==1)
                               {
                                   //| TIPO + identificador + ToTerm("=") + EXPRESION + ";
                                   ParseTreeNode nodoExpresion = nodo.ChildNodes[2];
                                   string nombreVar = nodo.ChildNodes[1].Token.ValueString;
                                   string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                                   int posVar = tablaSimbolos.getPosicion(nombreVar, ambitos);

                                   //es un int
                                   if (tipo.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                                   {
                                       string et1 = c3d.getTemporal();
                                       c3d.addCodigo(et1 + " = P + " + posVar + ";");
                                       object exp = evaluarInt(nodoExpresion.ChildNodes[0],ambitos);
                                       c3d.addCodigo("STACK[ " + et1 + " ] = " + exp + ";");
                                   }


                               }
                           }





                           break;
                       }

                    
                   #endregion

                   /*INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos
                | SI
                | SALIR + Eos
                | CONTINUAR + Eos
                | MIENTRAS
                | PARA
                | LOOP
                | HACER
                | REPETIR
                | ELEGIR
                | EXPRESION;*/
           
                    
                   case Constantes.mientras: 
                       {
                           // ola k ase?
                           // MIENTRAS.Rule = ToTerm(Constantes.mientras) + EXPRESION + ":" + Eos + CUERPO;
                           /*
                           L1: 
                           if (i < 10) goto L2; 
                           goto L3; 
                           //codigo i = i + 1;
                           goto L1: 
                       L3:
                           */
                           String etiqCiclo = c3d.getEtiqueta();
                           c3d.addCodigo(etiqCiclo);
                           //
                           Object g = evaluarExp(nodo.ChildNodes[0]);
                           if (g is nodoCondicion)
                           {
                               nodoCondicion condWhile = (nodoCondicion)g;
                               evaluarCuerpo(nodo.ChildNodes[1], ambitos);
                               c3d.addCodigo(Constantes3D.goto_ + " " + etiqCiclo + ";");
                               c3d.addCodigo(condWhile.getEtiquetasFalsas());

                           }
                           else
                           {
                               ErrorA er = new ErrorA(Constantes.errorSemantico, "Condicion no valida para un ciclo while", nodo.FindToken());
                               Form1.errores.addError(er);
                           }


                           break;

                       }








            }



        }

        #endregion
        /* --- Generaacion de codigo  Expresiones  -------*/




        #region evaluar int
        public object evaluarInt(ParseTreeNode nodo, Ambitos ambitos)
        {
            switch (nodo.Term.Name.ToString())
            {

                #region 
                case Constantes.expresion:
                    {
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region id

                case Constantes.id:
                    {
                        string nombre = nodo.ChildNodes[0].Token.ValueString;
                        int pos = tablaSimbolos.getPosicion(nombre, ambitos);
                        string tipo = tablaSimbolos.getTipo(nombre, ambitos);
                        if (pos == -1)
                        {
                            pos = tablaSimbolos.getPosicionDeClase(nombre, ambitos);
                            if (pos == -1)
                            {
                                ErrorA err = new ErrorA("semantico", "No existe la variable " + nombre, nodo.FindToken());
                                Form1.errores.addError(err);
                            }
                            else
                            {
                              
                                string temp1 = c3d.getTemporal();
                                string temp2 = c3d.getTemporal();
                                string temp3 = c3d.getTemporal();
                                string temp4 = c3d.getTemporal();

                                c3d.addCodigo(temp1 + " = P + 0; //pos this");
                                c3d.addCodigo(temp2 + " = STACK[ " + temp1 + " ];");
                                c3d.addCodigo(temp3 + " = " + temp2 + " + " + pos + ";");
                                c3d.addCodigo(temp4 + " = HEAP[ " + temp3 + " ];");
                                return temp4;

                            }
                        }
                        else
                        {
                            if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo( temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre );
                                return temp2;
                            }
                            else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else if (string.Equals(tipo, Constantes.tipoBool, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else
                            {
                                ErrorA er= new ErrorA(Constantes.errorSemantico, "Error, tipo no valido para asignacion int, "+ tipo, nodo.FindToken());
                                Form1.errores.addError(er);
                                
                            }

                        }

                        return "nulo";

                    }
                #endregion

                #region valores primitivos

                case Constantes.tipoEntero:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }

                case Constantes.tipoDecimal:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo entero, no es valido un double", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;

                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case Constantes.tipoCadena:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo entero, no es valido una cadena", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                #endregion
                    


                #region operaciones
                case Constantes.suma:
                    {
                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " + " + val2 + ";");
                        return temp;
                    }
                case Constantes.resta:
                    {
                        
                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " - " + val2 + ";");
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " * " + val2 + ";");
                        return temp;
                    }


                case Constantes.potencia:
                    {

                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " ^ " + val2 + ";");
                        return temp;
                    }

                case Constantes.division:
                    {
                        ErrorA er = new ErrorA(Constantes.errorSemantico, "Division no valida para un tipo int", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                #endregion


                #region unarios

                case Constantes.negativo:
                    {
                        Object resultado = evaluarInt(nodo.ChildNodes[0], ambitos);
                        string et = c3d.getTemporal();
                        c3d.addCodigo(et + " = " + resultado + " * -1;");
                        return et;
                    }

                case Constantes.masmas://tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo( temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }
                    }

                case Constantes.menosmenos: //tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }


                    }

                #endregion


            }


            return "nulo";
        }


#endregion


        #region evaluar Condicion

        public object evaluarExp(ParseTreeNode nodo)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {


                #region 
                case Constantes.expresion:
                    {
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region id

                case Constantes.id:
                    {
                        string id = nodo.ChildNodes[0].Token.ValueString;
                        break;


                    }

                #endregion

                #region valores primitivos

                case Constantes.tipoEntero:
                    {
                        int val = int.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return val;
                    }

                case Constantes.tipoDecimal:
                    {
                        double val = double.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return val;
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;
                        
                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return  0;
                        }
                    }


                #endregion


                #region operaciones 

                case Constantes.suma:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " + " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.resta:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " - " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " * " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }


                case Constantes.division:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " / " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.potencia:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " ^ " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }



                #endregion


                #region relacionales

                case Constantes.mayor:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " > " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.menor:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " < " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }
                case Constantes.menorIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " <= " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }

                case Constantes.mayorIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " >= " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }

                case Constantes.igualIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " == " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }

                case Constantes.distintoA:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " != " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }




                #endregion


                #region logicas


                case Constantes.andJava:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        if (val1 is nodoCondicion)
                        {
                            nodoCondicion retorno = new nodoCondicion("");
                            nodoCondicion cond1 = (nodoCondicion)val1;
                            string etVs = cond1.getEtiquetasVerdaderas();
                            c3d.addCodigo(etVs);
                            Object val2 = evaluarExp(nodo.ChildNodes[1]);
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion cond2 = (nodoCondicion)val2;
                                string etFs = cond2.getEtiquetasFalsas();
                                //c3d.addCodigo(etFs);
                                retorno.addEtiquetasVerdaderas(cond2.etiquetasVerdaderas);
                                retorno.addEtiquetasFalsas(cond1.etiquetasFalsas);
                                retorno.addEtiquetasFalsas(cond2.etiquetasFalsas);
                                Console.WriteLine("Verdaderas: " + retorno.getEtiquetasVerdaderas());
                                Console.WriteLine("Falsas: " + retorno.getEtiquetasFalsas());
                                return retorno;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "Segundo valor para condicion and debe ser una condicion", nodo.ChildNodes[1].Token);
                                Form1.errores.addError(n);
                                return "nulo";
                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "Primer valor para condicion and debe ser una condicion", nodo.ChildNodes[0].Token);
                            Form1.errores.addError(n);
                            return "nulo";
                        }
                    }



                case Constantes.orJava:
                    {
                        nodoCondicion retorno = new nodoCondicion("");
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        if (val1 is nodoCondicion)
                        {
                            nodoCondicion cond1 = (nodoCondicion)val1;
                            c3d.addCodigo(cond1.getEtiquetasFalsas());
                            Object val2 = evaluarExp(nodo.ChildNodes[1]);
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion cond2 = (nodoCondicion)val2;
                                retorno.addEtiquetasVerdaderas(cond1.etiquetasVerdaderas);
                                retorno.addEtiquetasVerdaderas(cond2.etiquetasVerdaderas);
                                retorno.addEtiquetasFalsas(cond2.etiquetasFalsas);
                                Console.WriteLine("Verdaderas: " + retorno.getEtiquetasVerdaderas());
                                Console.WriteLine("Falsas: " + retorno.getEtiquetasFalsas());
                                return retorno;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "Segundo valor para condicion or debe ser una condicion", nodo.ChildNodes[1].Token);
                                Form1.errores.addError(n);
                                return "nulo";

                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "Primer valor para condicion or debe ser una condicion", nodo.ChildNodes[0].Token);
                            Form1.errores.addError(n);
                            return "nulo";

                        }



                    }

                #endregion 

            }




            return "nulo";
        }

        #endregion

    }
}
