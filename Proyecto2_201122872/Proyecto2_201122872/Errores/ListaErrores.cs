using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.Errores
{
    public class ListaErrores
    {
        public List<ErrorA> errores;


        public ListaErrores()
        {
            this.errores = new List<ErrorA>();
        }


        public void addError(ErrorA nuevo){
            this.errores.Add(nuevo);
        }


    }
}
