using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace Proyecto2_201122872.Instrucciones
{
    public class ExpresionLlamada:Instruccion
    {
        ParseTreeNode nodo;

        public ExpresionLlamada(ParseTreeNode nodo)
        {
            this.nodo = nodo;
        }

        public override string ejecutar()
        {

            return "";

        }

    }
}
