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

        public Stack<String> ambitos;
        int valIf, valWhile, valPara, valLoop, valHacer, valRepetir, valElegir; 

        public Ambitos()
        {
            this.ambitos = new Stack<String>();
            valIf = 0;
            valWhile = 0;
            valPara = 0;
            valLoop = 0;
            valHacer = 0;
            valRepetir = 0;
            valElegir = 0;
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

        public void addWhile()
        {
            valWhile++;
            this.ambitos.Push(Constantes.mientras + valWhile);
        }




    }
}
