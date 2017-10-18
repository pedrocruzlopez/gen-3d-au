using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Instrucciones
{
    public class Cuerpo
    {

        public List<Instruccion> instruccionesCuerpo;
        

        public Cuerpo()
        {
            this.instruccionesCuerpo = new List<Instruccion>();
        }

        public void addInstruccion(Instruccion inst)
        {
            this.instruccionesCuerpo.Add(inst);
        }





    }
}
