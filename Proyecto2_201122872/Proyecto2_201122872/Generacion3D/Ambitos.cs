using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_201122872.AnalizadorJava;
namespace Proyecto2_201122872.Generacion3D
{
    public class Ambitos
    {

        public Stack<String> ambitos;
        int valIf, valWhile, valPara, valLoop, valHacer, valRepetir, valCaso, valDefecto, valElse, valElegir,valX; 

        public Ambitos()
        {
            this.ambitos = new Stack<String>();
            valIf = 0;
            valWhile = 0;
            valPara = 0;
            valLoop = 0;
            valHacer = 0;
            valRepetir = 0;
            valCaso = 0;
            valDefecto = 0;
            valElse = 0;
            valX = 0;
        }


        public void addRepetir()
        {
            valRepetir++;
            this.ambitos.Push(Constantes.repetir + valWhile);
        }
        public void addElegir()
        {
            valElegir++;
            this.ambitos.Push(Constantes.elegir + valWhile);
        }
        public void addAmbito(String ambito){

            this.ambitos.Push(ambito);
        }

        public void addPara()
        {
            valPara++;
            this.ambitos.Push(Constantes.para + valWhile);
        }

        public void addLoop()
        {
            valLoop++;
            this.ambitos.Push(Constantes.loop + valWhile);
        }

        public void addHAcer()
        {
            valHacer++;
            this.ambitos.Push(Constantes.hacer + valWhile);
        }

        public void addIf()
        {
            valIf++;
            this.ambitos.Push(Constantes.si + valIf);
        }

        public void addElse()
        {
            valElse++;
            this.ambitos.Push(Constantes.sino + valElse);
        }

        
        public void addCaso()
        {
            valCaso++;
            this.ambitos.Push(Constantes.caso + valCaso);
        }

        public void addDefecto()
        {
            valDefecto++;
            this.ambitos.Push(Constantes.defecto + valDefecto);
        }

        public void addWhile()
        {
            valWhile++;
            this.ambitos.Push(Constantes.mientras + valWhile);
            
        }

        public void addX()
        {
            valX++;
            this.ambitos.Push(Constantes.x + valX);
        }

        public String getAmbito()
        {
            string contexto="";
            string val;
            for (int i = ambitos.Count - 1; i >= 0; i--)
            {

                val = ambitos.ElementAt(i);
                if (i==0)
                {
                    contexto += val;
                }
                else
                {
                    contexto += val + "_";

                }

            }

            return contexto;
        }





    }
}
