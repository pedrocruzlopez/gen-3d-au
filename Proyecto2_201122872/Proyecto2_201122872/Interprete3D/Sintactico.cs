using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace Proyecto2_201122872.Interprete3D
{
    public class Sintactico:Grammar
    {
        public Sintactico():
            base(false)
        {


#region terminales
             var doble = new RegexBasedTerminal("doble", "[0-9]+[.][0-9]+");
            var entero = new RegexBasedTerminal("entero", "[0-9]+");
            var identificador = TerminalFactory.CreateCSharpIdentifier("identificador");
#endregion




            #region noTerminales

            NonTerminal EXPRESION = new NonTerminal(ConstantesInterprete.EXPRESION);
            NonTerminal OPERACION = new NonTerminal(ConstantesInterprete.OPERACION);
            NonTerminal DECIMAL= new NonTerminal(ConstantesInterprete.DECIMAL);
            NonTerminal ENTERO = new NonTerminal(ConstantesInterprete.ENTERO);
             NonTerminal ID = new NonTerminal(ConstantesInterprete.ID);
             NonTerminal  NEGATIVO= new NonTerminal(ConstantesInterprete.NEGATIVO);
             NonTerminal RELACIONALES= new NonTerminal(ConstantesInterprete.RELACIONALES);
             NonTerminal ARITMETICOS = new NonTerminal(ConstantesInterprete.ARITMETICOS);
             NonTerminal RELACINAL= new NonTerminal(ConstantesInterprete.RELACIONAL);
             NonTerminal TERMINO = new NonTerminal(ConstantesInterprete.TERMINO);
            #endregion




            #region producciones

             ARITMETICOS.Rule = ToTerm("+")
                 | ToTerm("-")
                 | ToTerm("*")
                 | ToTerm("/")
                 | ToTerm("^");


             RELACIONALES.Rule = ToTerm("<")
                 | ToTerm(">")
                 | ToTerm("<=")
                 | ToTerm(">=")
                 | ToTerm("!=");

             ENTERO.Rule = entero;
             DECIMAL.Rule = doble;
             ID.Rule = identificador;

             NEGATIVO.Rule = DECIMAL
                 | ENTERO
                 | ID;

             TERMINO.Rule = ToTerm("-") + NEGATIVO
                 | DECIMAL
                 | ID
                 | ENTERO;

             OPERACION.Rule = EXPRESION + ARITMETICOS + EXPRESION;

             EXPRESION.Rule = OPERACION
                 | TERMINO;

             RELACINAL.Rule = EXPRESION + RELACIONALES + EXPRESION;





            #endregion

             this.RegisterOperators(5,  NEGATIVO);
             this.RegisterOperators(4, Associativity.Right, "^");
             this.RegisterOperators(3, Associativity.Left, "/", "*");
             this.RegisterOperators(2, Associativity.Left, "-", "+");
             this.RegisterOperators(1, "==", "!=", "<", ">", "<=", ">=");


             this.Root = EXPRESION;


        }

        
        



    }
}
