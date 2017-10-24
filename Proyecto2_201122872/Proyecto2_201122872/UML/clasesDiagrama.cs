﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.Errores;

namespace Proyecto2_201122872.UML
{
   public  class clasesDiagrama
    {
        public List<Clase> listaClases;
        public Clase claseActual;


        public clasesDiagrama()
        {
            this.claseActual = new Clase();
            this.listaClases = new List<Clase>();
        }


        private Boolean existeClase(String nombre)
        {
            foreach (Clase item in this.listaClases)
            {
                if (item.getNombre().ToUpper().Equals(nombre.ToUpper()))
                    return true;
            }

            return false;
        }


        public Boolean insertarClase(Clase nueva)
        {
            if(!existeClase(nueva.getNombre()))
            {
                this.listaClases.Add(nueva);
                return true;
            }
            return false;
        }


        public Boolean actualizarClase(Clase actualizar)
        {
            Clase temporal;
            for (int i = 0; i < this.listaClases.Count; i++)
            {
                temporal = this.listaClases.ElementAt(i);
                if (temporal.getNombre().ToUpper().Equals(actualizar.getNombre().ToUpper()))
                {
                    this.listaClases.RemoveAt(i);
                    this.listaClases.Add(actualizar);
                    return true;
                }
            }

            return false;
        }



        public Boolean seleccionarClaseActual(String nombreClase)
        {
            Clase temporal;
            if(this.claseActual.getNombre()!=null)
                actualizarClase(this.claseActual);

            for (int i = 0; i < this.listaClases.Count; i++)
            {
                temporal = this.listaClases.ElementAt(i);
                if (temporal.getNombre().ToUpper().Equals(nombreClase.ToUpper()))
                {
                    this.claseActual = temporal;
                    return true;
                }
                
            }

            return false;
        }




        public int getSize()
        {
            return this.listaClases.Count;
        }



       /*    Atributos    */

        public Boolean addAtributoActual(Atributo atr)
        {
            return this.claseActual.addAtributo(atr);
        }



        private string getCadenaGraphiz()
        {
            string cadena = "digraph structs{\n node [shape= record];";
            Clase actual;
            for (int i = 0; i < this.listaClases.Count; i++)
            {
                actual = listaClases.ElementAt(i);
                cadena += "struct" + i + actual.getCadenaGraphivz();

            }
            cadena += "\n}";

            return cadena;

        }


        public void generarGrafo()
        {
            String path = @"C:\";
            String arbolDerivacion = path + "diagrama.txt";
            String texto = getCadenaGraphiz();
            System.IO.File.WriteAllText(arbolDerivacion, texto);
            Console.WriteLine(arbolDerivacion);
            try
            {
                var command = string.Format("dot -Tjpg {0} -o {1}", "\"" + arbolDerivacion + "\"", "\"" + arbolDerivacion.Replace(".txt", ".jpg") + "\"");
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C " + command);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();


            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

       
       /*  traduccion a codigo */

        public string getCodigoJava()
        {
            string cad = "";
            foreach (Clase item in this.listaClases)
            {
                cad += item.getCodigoJava() + "\n";
            }
            return cad;
        }


        public string getCodigoPython()
        {
            string cad = "";
            foreach (Clase item in this.listaClases)
            {
                cad += item.getCodigoPython() + "\n";
            }
            return cad;
        }


        /*----------- Agregar Herencia --------------*/

        private Atributo esValidoAtriHerencia(Atributo atr, String nombreClase)
        {
            if(atr.visibilidad.Equals(Constantes.publico, StringComparison.OrdinalIgnoreCase) ||
                atr.visibilidad.Equals(Constantes.protegido, StringComparison.OrdinalIgnoreCase))
            {
                atr.esHeredado = true;
                return atr;

            }
            return null;
        }



        private Funcion esValidoFuncHerencia(Funcion func, String nombreClase)
        {
            if(func.visibilidad.Equals(Constantes.publico, StringComparison.OrdinalIgnoreCase) ||
                func.visibilidad.Equals(Constantes.protegido, StringComparison.OrdinalIgnoreCase))
            {
                Funcion nueva = new Funcion(nombreClase, func.nombre, func.tipo, func.parametros, func.visibilidad, func.cuerpo);
                nueva.esHeredada = true;
                nueva.firma = nueva.generarFirma();
                return nueva;
            }
            return null;

        }



        public clasesDiagrama agregarHerencia()
        {
            clasesDiagrama umlRetorno = new clasesDiagrama();
            Clase actual;

            for (int i = 0; i < listaClases.Count; i++)
            {
                actual = listaClases.ElementAt(i);
                if (actual.herencia != null)
                {//posee herencia
                    Clase clasePadre = getClase(actual.herencia);
                    Clase temporal = new Clase();
                    if (clasePadre.lenguaje.Equals(actual.lenguaje, StringComparison.OrdinalIgnoreCase))
                    {
                        // buscamos todos los atributos que sean validos


                        temporal.nombre = actual.nombre;
                        temporal.herencia = actual.herencia;
                        temporal.lenguaje = actual.lenguaje;
                        temporal.tamanho = actual.tamanho;
                        temporal.apuntador = actual.apuntador;
                        //atributos de la clase padre
                        foreach (Atributo atrTemp in clasePadre.atributos.atributos)
                        {
                            Atributo nuevo = esValidoAtriHerencia(atrTemp, temporal.nombre);
                            if (nuevo != null)
                            {
                                temporal.addAtributo(nuevo);
                            }
                            
                        }
                        //agregamos los atributos de la clase ya que han sido guardados los de la clase padre

                        foreach (Atributo item in actual.atributos.atributos)
                        {
                            temporal.addAtributo(item);
                        }

                        //agregamos las funciones de la clase padre

                        foreach (Funcion item in clasePadre.funciones.funciones)
                        {
                            Funcion funcNueva = esValidoFuncHerencia(item, temporal.nombre);
                            if (funcNueva != null)
                            {
                                temporal.addFuncion(funcNueva);
                            }

                        }

                        foreach (Funcion item in actual.funciones.funciones)
                        {
                            temporal.funciones.sobreEscribirFunciones(item);
                        }


                        umlRetorno.insertarClase(temporal);

                    }
                    else
                    {
                        ErrorA er = new ErrorA(Constantes.errorSemantico, 0, 0, 0, "La clase " + actual.nombre + " es de otro lenguaje a la clase " + clasePadre.nombre);
                        Form1.errores.addError(er);

                    }

                }
                else
                {
                    umlRetorno.insertarClase(actual);
                }


            }

            return umlRetorno;



        }


       /*

        private void agregarHerencias()
        {
            clasesDiagrama nuevoDiagrama = new clasesDiagrama();

            foreach (Clase actual in this.listaClases)
            {
                if (actual.herencia != null || !actual.herencia.Equals(""))
                {
                    Clase clasePadre = getClase(actual.herencia);
                    if (clasePadre.lenguaje.Equals(actual.lenguaje, StringComparison.OrdinalIgnoreCase))
                    {
                        listaAtributos nuevosAtributos = new listaAtributos();
                        Atributo temporal;
                        //obtener atributos
                        foreach (Atributo atrTemporal in clasePadre.atributos.atributos)
                        {
                            temporal = esValidoAtriHerencia(atrTemporal, actual.nombre);
                            if (temporal != null)
                            {
                                nuevosAtributos.addAtributo(temporal);
                            }
                            
                        }

                        //obtener funciones





                    }
                    else
                    {//no se puede heredar una clase a otra de otro tipo
                        ErrorA er = new ErrorA(Constantes.errorSemantico,0,0,0, "La clase " + actual.nombre + " es de otro lenguaje a la clase " + clasePadre.nombre);
                        Form1.errores.addError(er);
                    }

                }
                else
                {
                    //no posee herencia, solo se guarda
                    nuevoDiagrama.insertarClase(actual);

                }
                
            }



        }

       */
       

        private listaAtributos getAtributosConHerencia(listaAtributos Padre, listaAtributos hijo)
        {
            foreach (Atributo atr in hijo.atributos)
            {
                Padre.addAtributo(atr);
            }
            return Padre;

        }


        private listaFunciones getFuncionesconHerencia(listaFunciones padre, listaFunciones hijo, string nombreHijo)
        {
           

            foreach (Funcion itemPadre in padre.funciones)
            {
                itemPadre.clase = nombreHijo;
                itemPadre.generarFirma();
                foreach (Funcion itemHijo in hijo.funciones)
                {
                    if (itemHijo.firma.Equals(itemPadre.firma, StringComparison.OrdinalIgnoreCase))
                    {//se debe sobreescirbir
                        itemHijo.setCuerpo(itemPadre.cuerpo);
                        padre.addFuncion(itemHijo);
                    }
                    else
                    {
                        padre.addFuncion(itemHijo);
                    }
                }
            }

            return null;
        }



       /*

        public void agregarHerencia()
        {
            string nombreHerencia;
            Clase padre,actual;

            for (int i = 0; i < this.listaClases.Count; i++)
            {
                actual= listaClases.ElementAt(i);
                
                if (actual.herencia != null && !actual.herencia.Equals(""))
                {
                    nombreHerencia = actual.herencia;
                    padre = getClase(nombreHerencia);
                    if (padre != null)
                    {
                        listaAtributos atrPadre = padre.atributos;
                        listaAtributos atrHijo = actual.atributos;
                        listaAtributos nuevosAtributos = getAtributosConHerencia(atrPadre, atrHijo);
                        listaFunciones funcPadre = padre.funciones;
                        listaFunciones funcHijo = actual.funciones;

                        
                        

                    }
                    else
                    {

                    }


                }
            }
        }

       */

        private Clase getClase(String nombre)
        {
            foreach (Clase cl in this.listaClases)
            {
                if (string.Equals(nombre, cl.nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return cl;
                }
            }

            return null;

        }


    }
}
