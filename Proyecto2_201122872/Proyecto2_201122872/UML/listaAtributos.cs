﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
    public class listaAtributos
    {
        public List<Atributo> atributos;


        public listaAtributos()
        {
            this.atributos = new List<Atributo>();
        }



        private Boolean existe(String nombre)
        {
            foreach (Atributo item in atributos)
            {
                if (item.getNombre().ToUpper().Equals(nombre.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean addAtributo(Atributo nuevo){
            
            if(!existe(nuevo.getNombre())){
                this.atributos.Add(nuevo);
                return true;
            }else{
                return false;
            }

        }


        public string getCadenaGraphivz()
        {

            Atributo actual;
            string cad = "";

            for (int i = 0; i <atributos.Count; i++)
            {
                actual = atributos.ElementAt(i);
                if (i == atributos.Count - 1)
                {
                    cad += actual.getCadenaAtributo();
                }
                else
                {
                    cad += actual.getCadenaAtributo() + "\\n";
                }
                
            }
            return cad;
        }


        /*-------------- traduccion a codigo --------------*/

        public string getCodigoJava()
        {
            string cad = "";

            foreach (Atributo item in atributos)
            {
                cad += item.getCodigoJava() + "\n";
            }
            return cad;

        }


        public string getCodigoPython()
        {

            string cad = "";

            foreach (Atributo item in atributos)
            {
                cad +="\t"+ item.getCodigoPython();
            }
            return cad;
        }

    }
}
