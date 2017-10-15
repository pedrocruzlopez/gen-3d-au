using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.UML;
using Proyecto2_201122872.Errores;
using Proyecto2_201122872.Instrucciones;

namespace Proyecto2_201122872.AnalizadorPython
{
    class AccionPy
    {
         private List<ParseTreeNode> clasesUML;

        public AccionPy()
        {
            this.clasesUML = new List<ParseTreeNode>();
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
                    claseActual.setLenguaje("python");
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


        private Clase generarClase(ParseTreeNode clase)
        {
            /*
              CLASE.Rule = ToTerm(Constantes.clase) + identificador + "[" + identificador + "]"+":"+Eos + CUERPO_CLASE
              | ToTerm(Constantes.clase) + identificador + "[" + "]"+":"+Eos + CUERPO_CLASE;
             */
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




        private List<Atributo> generarAtributos(ParseTreeNode nodoAtri)
        {

            /*VISIBILIDAD + TIPO + L_IDS
                | VISIBILIDAD + TIPO + DECLAARREGLO
                | TIPO + L_IDS
                | TIPO + DECLAARREGLO;*/
            List<Atributo> lista = new List<Atributo>();
            Atributo nuevo;
            int noHijos = nodoAtri.ChildNodes.Count;
            string tipo, visibilidad, nombre;

            if (noHijos == 3 && nodoAtri.ChildNodes[2].Term.Name.Equals(Constantes.declaArreglo))
            {// declaracion de un arreglo



            }
            else if (noHijos == 3 && nodoAtri.ChildNodes[2].Term.Name.Equals(Constantes.listaIds))
            {//declaracion de una lista de is con visibidad
                visibilidad = nodoAtri.ChildNodes[0].ChildNodes[0].Token.ValueString;
                tipo = nodoAtri.ChildNodes[1].ChildNodes[0].Token.ValueString;
                foreach (ParseTreeNode item in nodoAtri.ChildNodes[2].ChildNodes)
                {
                    nombre = item.Token.ValueString;
                    nuevo = new Atributo(visibilidad, nombre, tipo);
                    lista.Add(nuevo);
                }
                return lista;

            }
            else if (noHijos == 2 && nodoAtri.ChildNodes[1].Term.Name.Equals(Constantes.listaIds))
            {//declaracion d euna lista de ids sin visibilidad
                visibilidad = Constantes.publico;
                tipo = nodoAtri.ChildNodes[0].ChildNodes[0].Token.ValueString;
                foreach (ParseTreeNode item in nodoAtri.ChildNodes[1].ChildNodes)
                {
                    nombre = item.Token.ValueString;
                    nuevo = new Atributo(visibilidad, nombre, tipo);
                    lista.Add(nuevo);

                }
                return lista;
            }
            else
            {//es una declaracion de un arreglo sin visibilidad

            }





            return lista;

        }


        private ListaParametro getParametros(ParseTreeNode nodoParametro)
        {
            ListaParametro parametros = new ListaParametro();
            string tipo, nombre;
            variable nuevoParametro;

            foreach (ParseTreeNode item in nodoParametro.ChildNodes)
            {
                tipo = item.ChildNodes[0].ChildNodes[0].Token.Value.ToString();
                nombre = item.ChildNodes[1].Token.Value.ToString();
                nuevoParametro = new variable(nombre, tipo);
                parametros.addParametro(nuevoParametro);
            }


            return parametros;
        }

        private Cuerpo getCuerpo(ParseTreeNode nodoCuerpo)
        {
            Cuerpo contenido = new Cuerpo();

            Instruccion actual;
            foreach (ParseTreeNode item in nodoCuerpo.ChildNodes)
            {
                actual = getInstruccion(item);
                if (actual != null)
                    contenido.addInstruccion(actual);
                
            }

            return contenido;
        }


        private Instruccion getInstruccion(ParseTreeNode instr)
        {

            switch (instr.Term.Name)
            {
                case Constantes.expresion: //para las llamadas
                    {
                        int noHijos = instr.ChildNodes.Count;
                        ExpresionLlamada nuevaLlamada;
                        if(instr.ChildNodes[noHijos-1].Term.Name.Equals(Constantes.llamada,StringComparison.OrdinalIgnoreCase){
                            nuevaLlamada= new ExpresionLlamada(instr);
                            return nuevaLlamada;
                        }else{
                            ErrorA err = new ErrorA(Constantes.errorSemantico,"En invalido colocar este tipo de expresion, debe ser una llamada",instr.Token);
                            Form1.errores.addError(err);
                            return null;
                        }
                        
                    }

                case Constantes.declaracion:{

                    Declaracion nuevo = new Declaracion(instr);
                    return nuevo;
                }





            }


            return null;
        }



        private Funcion getFuncion(ParseTreeNode nodoFuncion, string nombreClase)
        { //5465
            /*FUNCION.Rule = VISIBILIDAD + ToTerm(Constantes.metodo) + identificador + ToTerm("[") + PARAMETROS + "]" + ":" + Eos + CUERPO
                | ToTerm(Constantes.metodo) + identificador + ToTerm("[") + PARAMETROS + "]" + ":" + Eos + CUERPO 4
                | VISIBILIDAD + ToTerm(Constantes.funcion) + TIPO + identificador + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO
                | ToTerm(Constantes.funcion) + TIPO + identificador + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO;*/


            Funcion nueva;
            string visibilidad, tipo, nombre;
            int noHijos = nodoFuncion.ChildNodes.Count;
            Cuerpo instr;
            ListaParametro parametros;
            if (noHijos == 4)
            {// es un metodo sin visibilidad
                visibilidad = Constantes.publico;
                tipo = Constantes.tipoVoid;
                nombre = nodoFuncion.ChildNodes[1].Token.ValueString;
                parametros = getParametros(nodoFuncion.ChildNodes[2]);
                instr = getCuerpo(nodoFuncion.ChildNodes[3]);
                nueva = new Funcion(nombreClase, nombre, tipo, parametros, visibilidad);
                nueva.setCuerpo(instr);
                return nueva;
            }
            else if (noHijos == 6)
            {
             
                visibilidad = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.ValueString;
                tipo = nodoFuncion.ChildNodes[2].ChildNodes[0].Token.ValueString;
                nombre = nodoFuncion.ChildNodes[3].Token.ValueString;
                parametros = getParametros(nodoFuncion.ChildNodes[4]);
                instr = getCuerpo(nodoFuncion.ChildNodes[5]);
                nueva = new Funcion(nombreClase, nombre, tipo, parametros, visibilidad);
                nueva.setCuerpo(instr);
                return nueva;
            }
            else if (noHijos == 5 && nodoFuncion.ChildNodes[0].Term.Name.Equals(Constantes.visibilidad))
            {//es un metodo
                // VISIBILIDAD + ToTerm(Constantes.metodo) + identificador + ToTerm("[") + PARAMETROS + "]" + ":" + Eos + CUERPO
                visibilidad = nodoFuncion.ChildNodes[0].ChildNodes[0].Token.ValueString;
                nombre = nodoFuncion.ChildNodes[2].Token.ValueString;
                parametros = getParametros(nodoFuncion.ChildNodes[3]);
                instr = getCuerpo(nodoFuncion.ChildNodes[4]);
                nueva = new Funcion(nombreClase, nombre, Constantes.tipoVoid, parametros, visibilidad);
                nueva.setCuerpo(instr);
                return nueva;

            }
            else
            {// es una funcion
                // | ToTerm(Constantes.funcion) + TIPO + identificador + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO
                tipo = nodoFuncion.ChildNodes[1].ChildNodes[0].Token.ValueString;
                parametros = getParametros(nodoFuncion.ChildNodes[3]);
                nombre = nodoFuncion.ChildNodes[2].Token.ValueString;
                instr = getCuerpo(nodoFuncion.ChildNodes[4]);
                nueva = new Funcion(nombreClase, nombre, tipo, parametros, Constantes.publico);
                nueva.setCuerpo(instr);
                return nueva;
                
            }


        }


        private Funcion getconstructor(ParseTreeNode nodoConstructor, string nombreClase)
        {
            //CONSTRUCTOR.Rule = ToTerm("__constructor") + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO;
            Cuerpo instr;
            ListaParametro parametros = getParametros(nodoConstructor.ChildNodes[0]);
           string nombre = nodoConstructor.ChildNodes[0].Token.Value.ToString();
            Funcion nueva = new Funcion(nombreClase, Constantes.constructor, Constantes.tipoVoid, parametros, Constantes.publico);
            if (nombre.Equals(nombreClase,StringComparison.OrdinalIgnoreCase))
            {
                nueva = new Funcion(nombreClase, nombre, Constantes.tipoVoid, parametros, Constantes.publico);
                nueva.setConstructor(true);
                instr = getCuerpo(nodoConstructor.ChildNodes[1]);
                nueva.setCuerpo(instr);
                return nueva;
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





        private Clase agregarInstruccionesClase(Clase claseModificar, ParseTreeNode instrucciones)
        {
            foreach (ParseTreeNode item in instrucciones.ChildNodes)
            {
                switch (item.Term.Name)
                {/*ELEMENTO.Rule = FUNCION
                | ATRIBUTO
                | CONSTRUCTOR
                | FUNCION_SOBRE;
*/


                    case Constantes.funcion:
                        {
                            Funcion nueva = getFuncion(item, claseModificar.getNombre());
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

                    case Constantes.constructor:
                        {
                            Funcion constructor = getconstructor(item, claseModificar.getNombre());
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

                    case Constantes.funSobre:
                        {
                            Funcion nueva = getFuncion(item.ChildNodes[0], claseModificar.getNombre());
                            nueva.setSobreescrita(true);
                            claseModificar.addFuncion(nueva);
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
                            generarListadoClases(item);
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
