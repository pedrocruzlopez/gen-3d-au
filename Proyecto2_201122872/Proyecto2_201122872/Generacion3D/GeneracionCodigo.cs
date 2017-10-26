﻿using Proyecto2_201122872.Generacion3D.TablaSimbolos;
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
              clasesDiagrama uml2= analizadorJava.parseConvertirUML2(contenido);
              uml = uml2.agregarHerencia();
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
                    c3d.addCodigo(Constantes.tipoVoid + " " + principal.firma + "(){");
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
                c3d.addCodigo(Constantes.tipoVoid + " " + constructor.firma + "(){");
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
                
                | 
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
                                   //es un double
                                   else if (tipo.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase))
                                   {
                                       string et1 = c3d.getTemporal();
                                       c3d.addCodigo(et1 + " = P + " + posVar + ";");
                                       object exp = evaluarDouble(nodoExpresion.ChildNodes[0], ambitos);
                                       c3d.addCodigo("STACK[ " + et1 + " ] = " + exp + ";");

                                   }
                                   //es un char
                                   else if (tipo.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                                   {
                                       String val = nodoExpresion.ChildNodes[0].Term.Name.ToString();
                                       if (val.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                                       {
                                           char caracter = (nodoExpresion.ChildNodes[0].ChildNodes[0].Token.ValueString).ToString()[0];
                                           int ascii = (int)caracter;
                                           string temp = c3d.getTemporal();
                                           c3d.addCodigo(temp + " =  P +" + posVar + ";");
                                           c3d.addCodigo("STACK[" + temp + "] = " + ascii + ";");
                                       }
                                       else
                                       {
                                           ErrorA er = new ErrorA(Constantes.errorSemantico, "Valor no valido para un asignacion de tipo char", nodoExpresion.FindToken());
                                           Form1.errores.addError(er);

                                       }
                                   }
                                   //es un bool
                                   else if (tipo.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase))
                                   {
                                       string et1 = c3d.getTemporal();
                                       c3d.addCodigo(et1 + " = P + " + posVar + ";");
                                       object exp = evaluarBool(nodoExpresion.ChildNodes[0], ambitos);
                                       c3d.addCodigo("STACK[ " + et1 + " ] = " + exp + ";");

                                   }
                                   //es un string
                                   else if (tipo.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase))
                                   {
                                       string temp1 = c3d.getTemporal();
                                       c3d.addCodigo(temp1 + " = H;");
                                       Object val = evaluarCadena(nodoExpresion.ChildNodes[0],ambitos);
                                       c3d.addCodigo("HEAP[H] = -1;");
                                       c3d.addCodigo("H = H + 1;");
                                       String temp = c3d.getTemporal();
                                       c3d.addCodigo( temp + " = P + " + posVar + ";");
                                       c3d.addCodigo("STACK[" + temp + "]=" + temp1 + ";");

                                   }
                                   //es un objeto
                                   else
                                   {

                                   }

                               }
                               else
                               {
                                   //es un arreglo
                                   //TIPO + identificador + LPOSICIONES + ToTerm(";")


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

                   #region While
                   case Constantes.mientras: 
                       {
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
                           c3d.addCodigo(etiqCiclo+":");
                           ambitos.addWhile();
                           Object g = evaluarExp(nodo.ChildNodes[0].ChildNodes[0]);
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
                           ambitos.ambitos.Pop();

                           break;

                       }
#endregion







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
                        Console.WriteLine("entre a una exp");
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

        #region evaluar double

        public object evaluarDouble(ParseTreeNode nodo, Ambitos ambitos)
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
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
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
                                ErrorA er = new ErrorA(Constantes.errorSemantico, "Error, tipo no valido para asignacion int, " + tipo, nodo.FindToken());
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
                        return double.Parse(nodo.ChildNodes[0].Token.ValueString);
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
                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " + " + val2 + ";");
                        return temp;
                    }
                case Constantes.resta:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " - " + val2 + ";");
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " * " + val2 + ";");
                        return temp;
                    }


                case Constantes.potencia:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " ^ " + val2 + ";");
                        return temp;
                    }

                case Constantes.division:
                    {
                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " / " + val2 + ";");
                        return temp;
                    }


                #endregion
                

                #region unarios

                case Constantes.negativo:
                    {
                        Object resultado = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        string et = c3d.getTemporal();
                        c3d.addCodigo(et + " = " + resultado + " * -1;");
                        return et;
                    }

                case Constantes.masmas://tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
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
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
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



        #region evaluar bool


        public object evaluarBool(ParseTreeNode nodo, Ambitos ambitos)
        {

            switch(nodo.Term.Name)
            {


                #region
                case Constantes.expresion:
                    {
                        Console.WriteLine("entre a una exp");
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region valoresPrimitivos

                case Constantes.tipoEntero:
                    {
                        string valor = nodo.ChildNodes[0].Token.ValueString;
                        if (valor.Equals("1"))
                        {
                            return valor;
                        }
                        else if(valor.Equals("0"))
                        {
                            return valor;
                        }
                        else
                        {
                            return "nulo";
                        }

                    }

                case Constantes.tipoCadena:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una cadena", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                    }


                case Constantes.tipoChar:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una char", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.tipoDecimal:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un decimal", nodo.FindToken());
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

               





                #endregion


                #region operaciones
                case Constantes.suma:
                    {
                        object uno = evaluarBool(nodo.ChildNodes[0],ambitos);
                        object dos = evaluarBool(nodo.ChildNodes[1],ambitos);
                        String temp = c3d.getTemporal();
                        if (uno.ToString().Equals("0") && dos.ToString().Equals("0"))
                        {
                            c3d.addCodigo(temp + " = 0;");
                            return temp;
                        }
                        else
                        {
                            c3d.addCodigo(temp + " = 1;");
                            return temp;
                        }
                    }
                case Constantes.resta:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una resta", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                       
                    }

                case Constantes.multiplicacion:
                    {
                        object uno = evaluarBool(nodo.ChildNodes[0], ambitos);
                        object dos = evaluarBool(nodo.ChildNodes[1], ambitos);
                        String temp = c3d.getTemporal();
                        if (uno.ToString().Equals("1") && dos.ToString().Equals("1"))
                        {
                            c3d.addCodigo(temp + " = 1;");
                            return temp;
                        }
                        else
                        {
                            c3d.addCodigo(temp + " = 0;");
                            return temp;
                        }
                    }


                case Constantes.potencia:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una potencia", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                        
                    }

                case Constantes.division:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una division", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                    }


                #endregion


                #region unarios

                case Constantes.negativo:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un negativo", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.masmas://tree
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un unario", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.menosmenos: //tree
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un unario", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";


                    }

                #endregion



            }




            return null;
        }



        #endregion


        #region validaString

        private object evaluarCadena(ParseTreeNode nodo, Ambitos ambitos)
        {


            switch(nodo.Term.Name){


                #region
                case Constantes.expresion:
                    {
                        Console.WriteLine("entre a una exp");
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion


                case Constantes.tipoCadena:
                    {
                        string id = nodo.ChildNodes[0].Token.ValueString;
                        char caracter;
                        int ascii;

                        for (int i = 0; i < id.Length; i++)
                        {
                            caracter = id.ElementAt(i).ToString()[0];
                            ascii = (int)caracter;
                            c3d.addCodigo("HEAP[H] = " + ascii + ";");
                            c3d.addCodigo("H = H + 1;");
                        }
                        return "nulo";
                    }





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


                #region expresion puntitos
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



                #region llamadaMetodo

                /* LEXPRESIONES.Rule = MakeStarRule(LEXPRESIONES, ToTerm(","), EXPRESION);

            LLAMADA.Rule = identificador + ToTerm("[") + LEXPRESIONES + "]"
                | identificador + ToTerm("[") + "]";*/

                /*
                 * LEXPRESIONES.Rule = MakePlusRule(LEXPRESIONES, ToTerm(","), EXPRESION);
                 LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
            | identificador + ToTerm("(") + ")";

                     
                 */

                case Constantes.llamada:
                    {
                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 1)
                        {//no tiene parametros

                        }
                        else
                        {//si tiene parametros

                        }




                        break;
                    }

                #endregion


            }




            return "nulo";
        }

        #endregion




        #region validacionTipos Retorna de que tipo es la expresion

        private object validarTipo(ParseTreeNode nodo, Ambitos ambito)
        {
            switch (nodo.Term.Name)
            {

                #region valores primitivos y Id (leer la i en ingles jaja asi no se lee y i)

                case Constantes.tipoEntero:
                    {
                        return Constantes.tipoEntero;
                    }
                case Constantes.tipoBool:
                    {
                        return Constantes.tipoBool;
                    }
                case Constantes.tipoCadena:
                    {
                        return Constantes.tipoCadena;
                    }
                case Constantes.tipoChar:
                    {
                        return Constantes.tipoChar;
                    }
                case Constantes.tipoDecimal:
                    {
                        return Constantes.tipoDecimal;
                    }

                case Constantes.id:
                    {
                        string tipo= tablaSimbolos.getTipo(nodo.ChildNodes[0].Token.ValueString, ambito);
                        if (tipo.Equals("nulo", StringComparison.OrdinalIgnoreCase))
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "La variable " + nodo.ChildNodes[0].Token.ValueString + " no existe", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }
                        else
                        {
                            return tipo;
                        }
                    }

                #endregion

                #region validacion Operaciones 

                case Constantes.suma:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarSuma(tipo1, tipo2);
                    }
                case Constantes.resta:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarResta(tipo1, tipo2);
                    }

                case Constantes.multiplicacion:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarMultiplicacion(tipo1, tipo2);
                    }

                case Constantes.division:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarDivision(tipo1, tipo2);
                    }

                case Constantes.potencia:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarPotencia(tipo1, tipo2);
                    }

                #endregion

                #region Relacionales

                case Constantes.mayor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.menor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.mayorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }


                case Constantes.menorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.distintoA:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.igualIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }
                #endregion


                #region Logicas

                case Constantes.andJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                    }
                case Constantes.orJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                    }
                case Constantes.xorJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                        //return validarUnario(validarTipo(nodo.ChildNodes[0],ambito));
                    }

                case Constantes.notJavaPython:
                    {
                        return validarNot(validarTipo(nodo.ChildNodes[0], ambito));
                    }
                #endregion

                #region unarios

                case Constantes.masmas:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
                    }

                case Constantes.menosmenos:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
                    }

                case Constantes.negativo:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
                    }

                #endregion

            }




            return "nulo";
        }


        private object validarSuma(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                /*retornos tipo double */
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                //valiaciones  entero
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }// validaciones de tipo cadena
                else if (esCadena(val1) && esInt(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esDouble(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esDouble(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esInt(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esChar(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esChar(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }//tipo bool
                else if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";

            }           
        }

        private object validarResta(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }//
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }//
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }



        }


        private object validarMultiplicacion(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }//
                else if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else 
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }


        private object validarDivision(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else
                {
                    return "nulo";
                }
                
            }
            else
            {
                return "nulo";
            }


        }


        private object validarPotencia(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else
                {
                    return "nulo";
                }
            }
            else
            {
                return "nulo";
            }


        }


        private object validarUnario(Object val1)
        {
            if (!esNulo(val1))
            {
                if (esDouble(val1))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) || esChar(val1))
                {
                    return Constantes.tipoEntero;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }


        private object validarRelacional(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {

                if((esDouble(val1) || esInt(val1) || esChar(val1)) &&
                    esDouble(val2) || esInt(val2) || esChar(val2))
                {
                    return Constantes.tipoBool;
                }
                else if((esCadena(val1) || esChar(val1)) &&
                    (esCadena(val2) || esChar(val2)))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }



        private object validarLogica(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
            
        }

        private object validarNot(Object val1)
        {
            if (!esNulo(val1))
            {
                if (esBool(val1))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }
            }
            else
            {
                return "nulo";
            }
        }



        #region terminales

        private Boolean esNulo(Object val)
        {
            return val.ToString().Equals("nulo", StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esInt(Object val)
        {
            return val.ToString().Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase);
        }


        private Boolean esDouble(Object val)
        {
            return val.ToString().Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esCadena(Object val)
        {
            return val.ToString().Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esChar(Object val)
        {
            return val.ToString().Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esBool(Object val)
        {
            return val.ToString().Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase);
        }


        #endregion

        #endregion



       



    }
}
