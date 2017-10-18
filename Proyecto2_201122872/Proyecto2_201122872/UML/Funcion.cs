﻿using Proyecto2_201122872.Instrucciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;

namespace Proyecto2_201122872.UML
{
    public class Funcion
    {
        private string clase;
        private string nombre;
        private string tipo;
        private ListaParametro parametros;
        private string visibilidad;
        public string firma;
        public Boolean esSobreescrita=false;
        public Boolean esPrincipal = false;
        public Boolean esConstructor = false;
        ///public Cuerpo instrucciones;
        public ParseTreeNode cuerpo; 
        

        public void setSobreescrita(bool val)
        {
            this.esSobreescrita = val;
        }

        public void setPrincipal(bool val)
        {
            this.esPrincipal = val;
        }


        public void setConstructor(bool val)
        {
            this.esConstructor = val;
        }

        public Funcion(string clase, string nombre, string tipo, ListaParametro parametros, string visibilidad, ParseTreeNode cuerpo)
        {
            this.clase = clase;
            this.nombre = nombre;
            this.tipo = tipo;
            this.parametros = parametros;
            this.visibilidad = visibilidad;
            this.firma = generarFirma();
            this.cuerpo = cuerpo;
        }



        public void setCuerpo (ParseTreeNode cuerpo){
            this.cuerpo= cuerpo;
        }


        private string generarFirma()
        {
            string retorno = tipo + "_" + nombre;
            if (parametros.parametros.Count > 0)
            {
                retorno += "_" + this.parametros.getFirma();
                return retorno;
            }

            return retorno;
        }



        public string getCadenaGraphivz()
        {
           return getVisibilidadGraphivz() + " " + this.nombre + "(" + this.parametros.getCadenaGraphivz() + "): " + tipo;


        }


        private string getVisibilidadGraphivz()
        {
            switch (this.visibilidad.ToUpper())
            {
                case "PUBLICO":
                    {
                        return "+";
                    }

                case "PRIVADO":
                    {
                        return "-";
                    }

                case "PROTEGIDO":
                    {
                        return "#";
                    }

            }
            return "+";
        }



        
        /*traduccion a codio */

        public string getCodigoJava()
        {
            string cad = "";

            cad = this.visibilidad + " " + this.tipo + " " + this.nombre + "(" + this.parametros.getCodigoJavaPython() + " ){\n}";

            return cad;
        }

        private string getTipoFunPython(){

            if(this.tipo.ToUpper().Equals("VOID")){
                return "metodo";
            }else{
                return "funcion";
            }
        }

        public String getCodigoPython()
        {
            string cad = "";
            cad = this.visibilidad + " " + getTipoFunPython() + " " + this.nombre + " [ " + this.parametros.getCodigoJavaPython() + "]:\n";
            return cad;

        }






        /*
         class Metodo:
  def __init__(self, clase, nombre, tipo, parametros):
    self.clase = clase
    self.nombre = nombre
    self.tipo = tipo
    self.parametros = parametros

  def esConstructor(self):
    return (self.nombre == self.clase.nombre and self.tipo == self.clase.nombre)

  def generarNombre(self):
    firma = ""
    for parametro in self.parametros:
      firma = "%s__%s" % (firma, parametro.tipo)
    return "%s__%s__%s%s" % (self.clase.nombre, self.nombre, self.tipo, firma)

  def generar(self):
    nombre = "void %s() {" % (self.generarNombre())
    cuerpo = ""
    if self.esConstructor() and self.clase.claseBase != None:
      constructorBase = self.clase.claseBase.devolverConstructorPorDefecto()
      if constructorBase != None:
        cuerpo = "\n  %s();\n" % (constructorBase.generarNombre())
    return "%s%s}\n" % (nombre, cuerpo)
         
         */





    }
}
