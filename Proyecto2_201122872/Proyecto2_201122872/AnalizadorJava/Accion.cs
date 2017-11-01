using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.UML;
using Proyecto2_201122872.Errores;


namespace Proyecto2_201122872.AnalizadorJava
{
    

   public class Accion
    {
        private List<ParseTreeNode> clasesUML;

        public Accion()
        {
            this.clasesUML = new List<ParseTreeNode>();
        }

        public clasesDiagrama generarUML2(ParseTreeNode raiz)
        {
            clasesDiagrama nuevo = new clasesDiagrama();
            generarListadoClases(raiz);
            Clase claseActual;
            foreach (ParseTreeNode actual in this.clasesUML)
            {
                claseActual = generarClase(actual);
                if (claseActual != null)
                {
                    claseActual.setLenguaje("java");
                    if (!nuevo.insertarClase(claseActual))
                    {
                        ErrorA nuev3o = new ErrorA(Constantes.errorSemantico, "La clase " + claseActual.getNombre() + ", no se pudo crear, ya existe", actual.FindToken());
                        Form1.errores.addError(nuev3o);
                    }
                }
                else
                {
                    ErrorA nuev3o = new ErrorA(Constantes.errorSemantico, "Ocurrio un error, no se pudo generar la clase ", actual.FindToken());
                    Form1.errores.addError(nuev3o);

                }

            }
            return nuevo;

        }

        public void generarUML(ParseTreeNode raiz)
        {
            generarListadoClases(raiz);
            Clase claseActual;
            foreach (ParseTreeNode actual in this.clasesUML)
            {
                claseActual = generarClase(actual);
                if (claseActual != null)
                {
                    claseActual.setLenguaje("java");
                    if (!Form1.uml.insertarClase(claseActual))
                    {
                        ErrorA nuevo = new ErrorA(Constantes.errorSemantico, "La clase " + claseActual.getNombre() + ", no se pudo crear, ya existe", actual.FindToken());
                        Form1.errores.addError(nuevo);
                    }
                }
                else
                {
                    ErrorA nuevo = new ErrorA(Constantes.errorSemantico, "Ocurrio un error, no se pudo generar la clase ", actual.FindToken());
                    Form1.errores.addError(nuevo);

                }
                

            }



        }


        private string getTipoAtributo(String tipo)
        {
            if (tipo.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
            {
                return Constantes.VARNORMAL;
            }
            else
            {
                return Constantes.OBJETO;
            }
        }
        
        private Clase generarClase(ParseTreeNode clase)
        {
            string nombre, herencia;
            Clase nuevaClase;
            if (clase.ChildNodes.Count == 2)
            {//no posee herencia
                nombre = clase.ChildNodes[0].Token.Value.ToString();
                nuevaClase = new Clase(nombre);
                nuevaClase = agregarInstruccionesClase(nuevaClase, clase.ChildNodes[1]);

                return nuevaClase;

            }
            else if (clase.ChildNodes.Count == 3)
            {//si posee herencia
                nombre = clase.ChildNodes[0].Token.Value.ToString();
                herencia = clase.ChildNodes[1].Token.Value.ToString();
                nuevaClase = new Clase(nombre, herencia);
                nuevaClase = agregarInstruccionesClase(nuevaClase, clase.ChildNodes[2]);
                return nuevaClase;

            }

            return null;
            
        }




        private ListaParametro getParametros(ParseTreeNode nodoParametros)
        {
            ListaParametro parametros = new ListaParametro();
            string tipo, nombre;
            variable nuevoParametro; 
            foreach (ParseTreeNode item in nodoParametros.ChildNodes)
            {
                tipo = item.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                nombre = item.ChildNodes[1].Token.Value.ToString();
                nuevoParametro = new variable(nombre, tipo);
                parametros.addParametro(nuevoParametro);
            }

            return parametros;
        }


        private Funcion getFuncion(ParseTreeNode nodoFuncion, string nombreClase)
        {

            /*FUNCION.Rule = VISIBILIDAD + TIPO + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | VISIBILIDAD + ToTerm(tipoVoid) + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | TIPO + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | ToTerm(tipoVoid) + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO;*/
            Funcion nueva;
            string visibilidad, tipo, nombre;
            int noHijos = nodoFuncion.ChildNodes.Count;
            ListaParametro parametros;
            if (noHijos == 5)
            {//posee visibilidad;
                visibilidad = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                nombre = nodoFuncion.ChildNodes[2].Token.Value.ToString();
                parametros = getParametros(nodoFuncion.ChildNodes[3]);
                if (nodoFuncion.ChildNodes[1].Term.Name.Equals(Constantes.tipo))
                    tipo = nodoFuncion.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                else
                    tipo = nodoFuncion.ChildNodes[1].Token.Value.ToString();
                nueva = new Funcion(nombreClase, nombre, tipo, parametros, visibilidad,nodoFuncion.ChildNodes[4]);
                return nueva;

            }
            else 

            {//no posee visibilidad;
                visibilidad = Constantes.publico;
                if (nodoFuncion.ChildNodes[0].Term.Name.Equals(Constantes.tipo))
                    tipo = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                else
                    tipo = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                nombre = nodoFuncion.ChildNodes[1].Token.Value.ToString();
                parametros = getParametros(nodoFuncion.ChildNodes[2]);
                nueva = new Funcion(nombreClase, nombre, tipo, parametros, visibilidad, nodoFuncion.ChildNodes[3]);
                return nueva;

            }

        }

        private ParseTreeNode[] getExpresionesArreglo(ParseTreeNode nodo){
            ParseTreeNode[] expresiones = new ParseTreeNode[nodo.ChildNodes.Count];
            for (int i = 0; i < expresiones.Length; i++)
            {
                expresiones[i] = nodo.ChildNodes[i];
                
            }

            return expresiones;
        
        }

        private List<Atributo> generarAtributos(ParseTreeNode nodoAtributo)
        {

            List<Atributo> lista = new List<Atributo>();
            Atributo nuevo;
            int noHijos = nodoAtributo.ChildNodes.Count;
            string tipo, nombre, visibilidad;

            if (noHijos == 5)
            {//arreglo con filas declaradas y visibilidad
                // VISIBILIDAD + TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
                visibilidad = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                nombre = nodoAtributo.ChildNodes[2].Token.ValueString;
                int noPosiciones = nodoAtributo.ChildNodes[3].ChildNodes.Count;
                ParseTreeNode[] expresionesDimensiones = getExpresionesArreglo(nodoAtributo.ChildNodes[3]);
                if (noPosiciones == expresionesDimensiones.Length && noPosiciones == nodoAtributo.ChildNodes[4].ChildNodes.Count)
                {//si se puede crear el arreglo
                    nuevo = new Atributo(visibilidad, nombre, tipo, Constantes.ARREGLO, noPosiciones, expresionesDimensiones);
                    nuevo.setExpresionAtributo(nodoAtributo.ChildNodes[4]);
                    lista.Add(nuevo);
                }
                else
                {//error semantico 
                    ErrorA err = new ErrorA(Constantes.errorSemantico, "No coinciden numero de dimensiones", nodoAtributo.Token);
                    Form1.errores.addError(err);

                }

            }
            else if (noHijos == 4)
            {
                
                

                if (nodoAtributo.ChildNodes[0].Term.Name.Equals(Constantes.tipo, StringComparison.OrdinalIgnoreCase))
                {
                  // TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";";
                 
                    visibilidad = Constantes.publico;
                    tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    nombre = nodoAtributo.ChildNodes[1].Token.ValueString;
                    int noPosiciones = nodoAtributo.ChildNodes[3].ChildNodes.Count;
                    ParseTreeNode[] expresionesDimensiones = getExpresionesArreglo(nodoAtributo.ChildNodes[2]);
                    int noFilasExpresion = nodoAtributo.ChildNodes[3].ChildNodes.Count;
                    if (expresionesDimensiones.Length == noPosiciones && noPosiciones == noFilasExpresion)
                    {
                        nuevo = new Atributo(visibilidad, nombre, tipo, Constantes.ARREGLO, noPosiciones, expresionesDimensiones);
                        nuevo.setExpresionAtributo(nodoAtributo.ChildNodes[3]);
                        lista.Add(nuevo);

                    }
                    else
                    {
                        ErrorA err = new ErrorA(Constantes.errorSemantico, "No coinciden numero de dimensiones", nodoAtributo.Token);
                        Form1.errores.addError(err);

                    }


                }
                else if (nodoAtributo.ChildNodes[3].Term.Name.Equals(Constantes.lposiciones, StringComparison.OrdinalIgnoreCase)) 
                {
                    // VISIBILIDAD + TIPO + identificador + LPOSICIONES + ToTerm(";")
                    visibilidad = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    nombre = nodoAtributo.ChildNodes[2].Token.ValueString;
                    int noPosiciones = nodoAtributo.ChildNodes[3].ChildNodes.Count;
                    ParseTreeNode[] expresionesDimensiones = getExpresionesArreglo(nodoAtributo.ChildNodes[3]);
                    if (noPosiciones == expresionesDimensiones.Length)
                    {//si se puede crear el arreglo
                        nuevo = new Atributo(visibilidad, nombre, tipo, Constantes.ARREGLO, noPosiciones, expresionesDimensiones);
                        lista.Add(nuevo);
                    }
                    else
                    {//error semantico 
                        ErrorA err = new ErrorA(Constantes.errorSemantico, "No coinciden numero de dimensiones", nodoAtributo.Token);
                        Form1.errores.addError(err);

                    }

                }
                else
                {
                    /*VISIBILIDAD + TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";") //1
                */
                    visibilidad = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    nombre = nodoAtributo.ChildNodes[2].Token.ValueString;
                    nuevo = new Atributo(visibilidad, nombre, tipo, getTipoAtributo(tipo));
                    nuevo.setExpresionAtributo(nodoAtributo.ChildNodes[3]);
                    lista.Add(nuevo);

                }

            }//fin de ==4
            else if (noHijos == 3)
            {

                if (nodoAtributo.ChildNodes[0].Term.Name.Equals(Constantes.visibilidad, StringComparison.OrdinalIgnoreCase))
                {
                    //ATRIBUTO.Rule = VISIBILIDAD + TIPO + L_IDS + ToTerm(";")
                    visibilidad = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.ValueString;
                    foreach (ParseTreeNode item in nodoAtributo.ChildNodes[2].ChildNodes)
                    {
                        nombre = item.Token.Value.ToString();
                        nuevo = new Atributo(visibilidad, nombre, tipo,getTipoAtributo(tipo));
                        lista.Add(nuevo);
                    }
                    
                }
                else if (nodoAtributo.ChildNodes[2].Term.Name.Equals(Constantes.lposiciones, StringComparison.OrdinalIgnoreCase))
                {
                    //| TIPO + identificador + LPOSICIONES + ToTerm(";")
                    visibilidad = Constantes.publico;
                    nombre = nodoAtributo.ChildNodes[1].Token.ValueString;
                    tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    int noPosiciones = nodoAtributo.ChildNodes[2].ChildNodes.Count;
                    ParseTreeNode[] expresionesDimensiones = getExpresionesArreglo(nodoAtributo.ChildNodes[2]);
                    if (noPosiciones == expresionesDimensiones.Length)
                    {//si se puede crear el arreglo
                        nuevo = new Atributo(visibilidad, nombre, tipo, Constantes.ARREGLO, noPosiciones, expresionesDimensiones);
                        lista.Add(nuevo);
                    }
                    else
                    {//error semantico 
                        ErrorA err = new ErrorA(Constantes.errorSemantico, "No coinciden numero de dimensiones", nodoAtributo.Token);
                        Form1.errores.addError(err);

                    }

                    

                }
                else
                {
                    //| TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";") //1
                    visibilidad = Constantes.publico;
                    nombre = nodoAtributo.ChildNodes[1].Token.ValueString;
                    tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                    nuevo = new Atributo(visibilidad, nombre, tipo, getTipoAtributo(tipo));
                    nuevo.setExpresionAtributo(nodoAtributo.ChildNodes[2]);
                    lista.Add(nuevo);
                }


            }
            else if(noHijos==2)
            {
                //TIPO + L_IDS + ToTerm(";")

                visibilidad = Constantes.publico;
                tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                foreach (ParseTreeNode item in nodoAtributo.ChildNodes[1].ChildNodes)
                {
                    nombre = item.Token.Value.ToString();
                    nuevo = new Atributo(visibilidad, nombre, tipo, getTipoAtributo(tipo));
                    lista.Add(nuevo);
                }

            }

            return lista;
        }


/*
        private List<Atributo> generarAtributos(ParseTreeNode nodoAtributo)
        {
            
              ATRIBUTO.Rule = VISIBILIDAD + TIPO + L_IDS + ToTerm(";")3,4,2,3
                | VISIBILIDAD + TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";")
                | TIPO + L_IDS + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";");
             
             
            List<Atributo> lista = new List<Atributo>();
            Atributo nuevo;
            int noHijos = nodoAtributo.ChildNodes.Count;
            string tipo, nombre, visibilidad;
            if (noHijos == 2)
            {
                visibilidad = Constantes.publico;
                tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                foreach (ParseTreeNode item in nodoAtributo.ChildNodes[1].ChildNodes)
                {
                    nombre = item.Token.Value.ToString();
                    nuevo = new Atributo(visibilidad, nombre, tipo);
                    lista.Add(nuevo);
                }
                return lista;

            }
            else if (noHijos == 4)
            {
                visibilidad = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                nombre = nodoAtributo.ChildNodes[2].Token.Value.ToString();
                nuevo = new Atributo(visibilidad, nombre, tipo);
                lista.Add(nuevo);
                return lista;
            }
            else
            {//posee tres hijos
                if (nodoAtributo.ChildNodes[0].Term.Name.Equals(Constantes.visibilidad))
                {

                    visibilidad =nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                    tipo = nodoAtributo.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                    foreach (ParseTreeNode item in nodoAtributo.ChildNodes[2].ChildNodes)
                    {
                        nombre = item.Token.Value.ToString();
                        nuevo = new Atributo(visibilidad, nombre, tipo);
                        lista.Add(nuevo);
                    }
                    return lista;

                }
                else
                {
                    visibilidad = Constantes.publico;
                    tipo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                    nombre = nodoAtributo.ChildNodes[1].Token.Value.ToString();
                    nuevo = new Atributo(visibilidad, nombre, tipo);
                    lista.Add(nuevo);
                    return lista;


                }

            }

        }
*/
     
        private Funcion getConstructor(ParseTreeNode nodoConstructor, string nombreClase)
        {
            /* CONSTRUCTOR.Rule = identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO;*/
            Funcion constructor;
            string nombre = nodoConstructor.ChildNodes[0].Token.Value.ToString();
            if (nombre.ToUpper().Equals(nombreClase.ToUpper()))
            {
                ListaParametro parametros = getParametros(nodoConstructor.ChildNodes[1]);
                constructor = new Funcion(nombreClase, nombre, Constantes.tipoVoid, parametros, Constantes.publico,nodoConstructor.ChildNodes[2]);
                constructor.setConstructor(true);
                return constructor;
            }
            else
            {
                ErrorA nuevo = new ErrorA(Constantes.errorSemantico, nodoConstructor.ChildNodes[0].Token.Location.Line,
                    nodoConstructor.ChildNodes[0].Token.Location.Position, nodoConstructor.ChildNodes[0].Token.Location.Column,
                    "El nombre de " + nombre + ", no coincide con el nombre de clase, " + nombreClase);
                Form1.errores.addError(nuevo);
                return null;
                //error semantico
            }
        }


        private Funcion getPrincipal(ParseTreeNode nodoPrincipal, string nombreClase)
        {
            /*
            PRINCIPAL.Rule = ToTerm(Constantes.principal) + ToTerm("(") + ToTerm(")") + CUERPO;*/

            Funcion principal = new Funcion(nombreClase, Constantes.principal, Constantes.tipoVoid, new ListaParametro(), Constantes.publico,nodoPrincipal.ChildNodes[0]);
            principal.setPrincipal(true);
            return principal;


        }



        private Clase agregarInstruccionesClase(Clase claseModificar, ParseTreeNode instrucciones)
        {
            foreach (ParseTreeNode item in instrucciones.ChildNodes)
            {
                switch (item.Term.Name)
                {
                    case Constantes.funcion:
                        {
                            Funcion nueva = getFuncion(item, claseModificar.getNombre());
                            claseModificar.addFuncion(nueva);
                            break;
                        }

                    case Constantes.funSobre:
                        {
                            Funcion nueva = getFuncion(item.ChildNodes[0], claseModificar.getNombre());
                            nueva.setSobreescrita(true);
                            claseModificar.addFuncion(nueva);
                            break;
                        }

                    case Constantes.atributo:
                        {
                            List<Atributo> atributos = generarAtributos(item);
                            foreach (Atributo atr in atributos)
                            {
                                claseModificar.addAtributo(atr);
                            }
                            break;
                        }

                    case Constantes.principal:
                        {
                            if (!claseModificar.funciones.hayPrincipal())
                            {
                                Funcion principal = getPrincipal(item, claseModificar.getNombre());
                                claseModificar.addFuncion(principal);
                            }
                            else
                            {
                                ErrorA err = new ErrorA(Constantes.errorSemantico, item.FindToken().Location.Line, item.FindToken().Location.Position, item.FindToken().Location.Column, "Una clase unicamente puede poseer un metodo principal");
                                Form1.errores.addError(err);
                            }
                            break;
                        }
                    case Constantes.constructor:
                        {
                            Funcion constructor = getConstructor(item, claseModificar.getNombre());
                            if (constructor != null)
                            {
                                claseModificar.addFuncion(constructor);
                            }
                            else
                            {
                                ErrorA nuevo = new ErrorA("Semantico", "No se pudo generar el constructor", item.Token);
                                Form1.errores.addError(nuevo);

                            }

                            break;
                        }
                }
            }


            return claseModificar;
        }


        private void generarListadoClases(ParseTreeNode nodo)
        {
            String nombreNodo = nodo.Term.Name.ToString();

            switch (nombreNodo)
            {

                case Constantes.l_clases:
                    {
                        foreach (ParseTreeNode item in nodo.ChildNodes)
                        {
                            generarListadoClases (item);
                        }
                        break;
                    }

                case Constantes.clase:
                    {
                        Console.WriteLine("Entre a una clase");
                        this.clasesUML.Add(nodo);
                        break;

                    }

            }

        }


    }
}
