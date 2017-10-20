using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_201122872.AnalizadorJava;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.Generacion3D;
using Proyecto2_201122872.Instrucciones;

namespace Proyecto2_201122872.UML
{
    public class Clase
    {
        private string nombre;
        private string herencia;
        public listaAtributos atributos;
        public listaFunciones funciones;
        public string lenguaje;
        public int tamanho;

        int apuntador;


        public Boolean esNula()
        {
            return (this.nombre==null);

        }


        public void setLenguaje(string l)
        {
            this.lenguaje = l;
        }


        private void iniciarValores()
        {
           
            this.atributos = new listaAtributos();
            funciones = new listaFunciones();
            this.tamanho = 0;
        }


        public Clase()
        {
            iniciarValores();
            
        }

        public Clase(String nombre)
        {
            this.nombre = nombre;
            iniciarValores();
        }

        public Clase(String nombre, String herencia)
        {
            this.nombre = nombre;
            this.herencia = herencia;
            iniciarValores();
        }


        public String getNombre()
        {
            return this.nombre;
        }



        /*------------ Atibutos ----------------------*/

        public Boolean addAtributo(Atributo atr)
        {
            return this.atributos.addAtributo(atr);
        }


        public int sizeAtributos()
        {
            return this.atributos.atributos.Count;
        }
        /*------------- Funciones --------------------*/

        public Boolean addFuncion(Funcion nueva)
        {
            return this.funciones.addFuncion(nueva);
        }


        public int getSizeClase()
        {
            return this.atributos.atributos.Count;
        }

        /*------------- RElaciones ----------------------*/




        /*------- Traduccion a codigo ---------------*/

        public string getCodigoJava()
        {
            string cad = "Clase " + this.nombre;
            if (this.herencia != null || this.herencia != "")
                cad += " " + this.herencia;
            cad += " {\n"+ this.atributos.getCodigoJava()+"\n"+this.funciones.getCodigoJava()+"\n}";
            return cad;

        }


        public string getCodigoPython()
        {
            string cad = "Clase "+ this.nombre+" [";
            if (this.herencia != null || this.herencia != "")
                cad += " " + this.herencia;
            cad += " ]:\n"+ this.atributos.getCodigoPython()+"\n"+this.funciones.getCodigoPython();
            return cad;
        }



        /*------------ Graphviz---------------------*/

        public string getCadenaGraphivz()
        {
           
            return "[shape=record,label=\"{ "+ nombre+"|{"+atributos.getCadenaGraphivz()+"}|{"+funciones.getCadenaGrapvhiz()+"}}\"];";

        }


        /*-------------------- Pasar Elementos a tabla de simbolos ----------------------*/
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

        private List<Simbolo> generarSimbolosMetodo(ParseTreeNode nodo, List<Simbolo> lista, Ambitos ambitos)
        {

            String nombreNodo = nodo.Term.Name.ToString();
            switch (nombreNodo)
            {
                /*INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos no se HACWE //
                | SI
                | SALIR + Eos//
                | CONTINUAR + Eos//
                | MIENTRAS//
                | PARA//
                | LOOP//
                | HACER//
                 * 
                | REPETIR//
                | ELEGIR;//*/
                
                
                case Constantes.si:
                    {
                        /* *   SI.Rule = ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS + SI_NO
                | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS
                | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + SI_NO;
                 */

                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 3)
                        {
                            ambitos.addIf();
                            lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                            ambitos.ambitos.Pop();
                            lista = generarSimbolosMetodo(nodo.ChildNodes[2], lista, ambitos);

                        }
                        else
                        {
                            ambitos.addIf();
                            lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                            ambitos.ambitos.Pop();
                            lista = generarSimbolosMetodo(nodo.ChildNodes[2], lista, ambitos);
                            lista = generarSimbolosMetodo(nodo.ChildNodes[3], lista, ambitos);
                            
                        }
                        return lista;
                    }

                case Constantes.lextra:
                    {
                        //L_EXTRAS.Rule = MakeStarRule(L_EXTRAS, EXTRA);
                        foreach (ParseTreeNode nodoHijo in nodo.ChildNodes)
                        {
                            lista = generarSimbolosMetodo(nodoHijo, lista, ambitos);
                        }
                        return lista;
                    }

                case Constantes.extraSi:
                    {
                        //EXTRA.Rule = ToTerm(Constantes.sino_si_python) + EXPRESION + ":" + Eos + CUERPO;
                        ambitos.addIf();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;
                    }

                case Constantes.sino:
                    {
                        // SI_NO.Rule = ToTerm(Constantes.sino_python) + ":" + Eos + CUERPO;
                        ambitos.addElse();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;

                    }


                case Constantes.elegir:
                    {
                        // ELEGIR.Rule = ToTerm(Constantes.elegir) + EXPRESION + ":" + Eos + CUERPOELEGIR;
                        ambitos.addElegir();
                        lista= generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;
                    }

                case Constantes.cuerpoElegir:
                    {
                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 2)
                        {
                            lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                            lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                        }
                        else
                        {
                            lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);

                        }
                        return lista;
                    }

                case Constantes.lcasos:
                    {
                        foreach (ParseTreeNode nodoHijo in nodo.ChildNodes)
                        {
                            lista = generarSimbolosMetodo(nodoHijo, lista, ambitos);
                        }
                        return lista;
                    }

                case Constantes.defecto:
                    {
                        //DEFECTO.Rule = ToTerm(Constantes.defecto) + ":" + Eos + CUERPO;
                        ambitos.addDefecto();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;


                    }

                case Constantes.caso:
                    {
                        //CASO.Rule = EXPRESION + TtoTerm(":") + Eos + CUERPO;
                        ambitos.addCaso();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;

                    }


                case Constantes.loop:
                    {
                        //LOOP.Rule = ToTerm(Constantes.loop) + ":" + Eos + CUERPO;
                        ambitos.addLoop();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;
                    }



                case Constantes.para:
                    {
                        //PARA.Rule = ToTerm(Constantes.para) + "[" + ASIGNACION + ":" + EXPRESION + ":" + EXPRESION + "]" + ":" + Eos + CUERPO;
                        ambitos.addPara();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[3], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;


                    }

                case Constantes.hacer:
                    {

                        //HACER.Rule = ToTerm(Constantes.hacer) + ":" + Eos + CUERPO + Constantes.mientras + EXPRESION + Eos;
                        ambitos.addHAcer();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;

                    }
                case Constantes.repetir:
                    {

                        //REPETIR.Rule = ToTerm(Constantes.repetir) + ":" + Eos + CUERPO + Constantes.hasta + EXPRESION+Eos;
                        ambitos.addRepetir();
                        lista = generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;

                    }





                case Constantes.asignacion:
                    {
                        break;

                    }
                case Constantes.cuerpo:
                    {
                        return generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                    }
                case Constantes.instrucciones:
                    {
                        foreach (ParseTreeNode nodoHijo in nodo.ChildNodes)
                        {
                            lista = generarSimbolosMetodo(nodoHijo, lista, ambitos);
                        }
                        return lista;
                    }
                case Constantes.instruccion:
                    {
                        return generarSimbolosMetodo(nodo.ChildNodes[0], lista, ambitos);
                    }

                case Constantes.continuar:
                    {
                        break;
                    }

                case Constantes.salir:
                    {
                        break;
                    }

                case Constantes.declaracion:
                    {
                        /*DECLAARREGLO.Rule = identificador + POSICIONES;

            DECLRACION.Rule = TIPO + L_IDS
                | TIPO + DECLAARREGLO;*/

                        string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        if (nodo.ChildNodes[1].Term.Name.Equals(Constantes.lposiciones, StringComparison.OrdinalIgnoreCase))
                        {//es un arreglo



                        }
                        else
                        {//idnomarmal
                            string nombre;
                            Simbolo nuevo;
                            foreach (ParseTreeNode item in nodo.ChildNodes[1].ChildNodes)
                            {
                                nombre = item.Token.ValueString;
                                if (getTipoAtributo(tipo).Equals(Constantes.OBJETO, StringComparison.OrdinalIgnoreCase))
                                {
                                    nuevo = new Simbolo(Constantes.noTieneVisi, nombre, tipo, getTipoAtributo(tipo), ambitos.ambitos.Peek(), Constantes.varLocal, apuntador, 1);
                                    this.apuntador++;
                                }
                                else
                                {
                                    nuevo = new Simbolo(Constantes.noTieneVisi, nombre, tipo, getTipoAtributo(tipo), ambitos.ambitos.Peek(), Constantes.varLocal, apuntador, 1);
                                    this.apuntador++;
                                }
                                lista.Add(nuevo);
                            }

                        }
                        return lista;

                    }

                case Constantes.mientras:
                    {

                        ambitos.addWhile();
                        //MIENTRAS.Rule = ToTerm(Constantes.mientras) + EXPRESION+":" + Eos + CUERPO;
                        lista = generarSimbolosMetodo(nodo.ChildNodes[1], lista, ambitos);
                        ambitos.ambitos.Pop();
                        return lista;
                    }




            }

           

            return lista;

        }


        private List<Simbolo> generarSimbolosAtributos()
        {
            int apuntador = 0;
            List<Simbolo> listado = new List<Simbolo>();

            //generamos atributo

            Simbolo nuevo;
            foreach (Atributo item in atributos.atributos)
            {
                
                if(item.tipoAtributo.Equals(Constantes.ARREGLO,StringComparison.OrdinalIgnoreCase)){
                    nuevo = new Simbolo(item.visibilidad, item.nombre, item.tipo, Constantes.ARREGLO, this.nombre, Constantes3D.variableDeClase, apuntador, 1);
                    listado.Add(nuevo);
                    apuntador++;

                }
                else if (item.tipoAtributo.Equals(Constantes.OBJETO, StringComparison.OrdinalIgnoreCase))
                {
                    nuevo = new Simbolo(item.visibilidad, item.nombre, item.tipo, Constantes.OBJETO, this.nombre, Constantes3D.variableDeClase, apuntador, 1);
                    listado.Add(nuevo);
                    apuntador++;

                }
                else
                {
                    nuevo = new Simbolo(item.visibilidad, item.nombre, item.tipo, Constantes.VARNORMAL, this.nombre, Constantes3D.variableDeClase, apuntador, 1);
                    listado.Add(nuevo);
                    apuntador++;

                }
                
              
            }

            return listado;

        }

        

        /*---  Generacion de tabla de simbolos -------*/


        public List<Simbolo> getSimbolosClase()
        {

            List<Simbolo> retorno = new List<Simbolo>();

            retorno = generarSimbolosAtributos();
            Ambitos ambitos;
            
            foreach (Funcion func in this.funciones.funciones)
            {
                apuntador = 0;
                ambitos = new Ambitos();
                ambitos.addAmbito(func.firma);
                List<Simbolo> lTemporalFuncion= new List<Simbolo>();
                lTemporalFuncion = generarSimbolosMetodo(func.cuerpo, lTemporalFuncion, ambitos);
                foreach (Simbolo item in lTemporalFuncion)
                {
                    retorno.Add(item);
                }
            }
            return retorno;
        }



    }
}
