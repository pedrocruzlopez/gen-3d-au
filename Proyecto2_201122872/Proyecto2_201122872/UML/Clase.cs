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

        private List<Simbolo> generarSimbolosMetodo(ParseTreeNode nodo, int apuntador, List<Simbolo> lista, Ambitos ambitos)
        {

            String nombreNodo = nodo.Term.Name.ToString();
            switch (nombreNodo)
            {
                /*INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos
                | SI
                | SALIR + Eos//
                | CONTINUAR + Eos//
                | MIENTRAS
                | PARA
                | LOOP
                | HACER
                | REPETIR
                | ELEGIR;*/


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
                                }
                                else
                                {
                                    nuevo = new Simbolo(Constantes.noTieneVisi, nombre, tipo, getTipoAtributo(tipo), ambitos.ambitos.Peek(), Constantes.varLocal, apuntador, 1);
                                }
                                lista.Add(nuevo);
                            }

                        }
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
            int apuntador;
            foreach (Funcion func in this.funciones.funciones)
            {
                apuntador = 0;
                ambitos = new Ambitos();
                ambitos.addAmbito(func.firma);
                List<Simbolo> lTemporalFuncion= new List<Simbolo>();
                lTemporalFuncion = generarSimbolosMetodo(func.cuerpo, apuntador, lTemporalFuncion, ambitos);
                foreach (Simbolo item in lTemporalFuncion)
                {
                    retorno.Add(item);
                }
            }
            return retorno;
        }



    }
}
