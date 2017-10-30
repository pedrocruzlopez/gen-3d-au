using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;

namespace Proyecto2_201122872.Generacion3D
{
    class Codigo3D
    {


        public Codigo3D()
        {

        }


        /*
          TERMINO.Rule = ARITMETICA//
                | RELACIONAL//
                | LOGICA//
                | DECIMAL//
                | ENTERO //
                | ID//
                | CADENA//
                | BOOLEANO//
                | CHAR//
                | LLAMADA//
                | POSVECTOR //
                | UNARIO
                | ToTerm("(") + TERMINO + ")"//no es necesario en python
                | NEGATIVO
                | "{" + LFILAS + "}"//no existe en python
                | INSTANCIA;//
         */



        public object resolverExpresiones(ParseTreeNode nodo, Ambitos ambiente, String nombreClase)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {

                #region int, double, char, bool
                case Constantes.tipoEntero:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case Constantes.tipoDecimal:
                    {
                        return double.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case Constantes.tipoBool:
                    {
                        string val = nodo.ChildNodes[0].Token.ValueString;
                        if (val.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }
                #endregion

                #region cadenas
                case Constantes.tipoCadena:
                    {
                        return "nulo";
                    }


                #endregion
            }
                








            return "nulo"; 

        }



        





    }
}
