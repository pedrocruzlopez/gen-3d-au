using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using Proyecto2_201122872.AnalizadorJava;
namespace Proyecto2_201122872.Instrucciones
{
    class Declaracion:Instruccion
    {

        public ParseTreeNode nodo;

        
        public Declaracion(ParseTreeNode nodo)
        {
            this.nodo = nodo;
        }




        








    }
}
