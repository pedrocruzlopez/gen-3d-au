﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;

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
             NonTerminal GLOBAL = new NonTerminal(ConstantesInterprete.GLOBAL);
             NonTerminal salto = new NonTerminal(ConstantesInterprete.salto);
             NonTerminal SI = new NonTerminal(ConstantesInterprete.IF);
             NonTerminal stack = new NonTerminal(ConstantesInterprete.stack);
             NonTerminal heap = new NonTerminal(ConstantesInterprete.heap);
             NonTerminal imprimir = new NonTerminal(ConstantesInterprete.print);
             NonTerminal metodo = new NonTerminal(ConstantesInterprete.metodo);
             NonTerminal main = new NonTerminal(ConstantesInterprete.main);

             NonTerminal asignacionNormal = new NonTerminal("asignacionNormal");
             NonTerminal asignacionStack = new NonTerminal("asignacionStack");
             NonTerminal asignacionHeap = new NonTerminal("asignacionHeap");
             NonTerminal etiqueta = new NonTerminal("etiqueta");
             NonTerminal llamada = new NonTerminal("llamada");
             NonTerminal parametro = new NonTerminal("parametro");
             NonTerminal instruccion = new NonTerminal("instruccion");
             NonTerminal instrucciones = new NonTerminal("instruccicones");
             NonTerminal cuerpoArchivo = new NonTerminal("cuerpoArchivo");

            #endregion




            #region producciones

             parametro.Rule = ToTerm("c")
                 | ToTerm("d")
                 | ToTerm("f")
                 | ToTerm("s");

             asignacionNormal.Rule = identificador + ToTerm("=") + EXPRESION + ";";
             asignacionStack.Rule = identificador + ToTerm("=") + "STACK" + "[" + EXPRESION + "]" + ";";
             asignacionHeap.Rule = identificador + ToTerm("=") + "HEAP" + "[" + EXPRESION + "]" + ";";

             etiqueta.Rule = identificador + ToTerm(":");

             llamada.Rule = identificador + ToTerm("(") + ")" + ";";

             GLOBAL.Rule = asignacionNormal
                 | asignacionHeap
                 | asignacionStack
                 | etiqueta
                 | llamada;

             salto.Rule = ToTerm("goto") + identificador + ";";

             SI.Rule = ToTerm("if") + "(" + RELACINAL + ")" + "goto" + identificador + ";" + "goto" + identificador + ";";

             stack.Rule = ToTerm("STACK") + "[" + EXPRESION + "]" + "=" + EXPRESION + ";";


             heap.Rule = ToTerm("HEAP") + "[" + EXPRESION + "]" + "=" + EXPRESION + ";";

             imprimir.Rule = ToTerm(ConstantesInterprete.print) + ToTerm("(") + "'" + "%" + parametro + "'" + "," + EXPRESION + ")" + ";";

             instruccion.Rule = GLOBAL
                 | salto
                 | SI
                 | stack
                 | heap
                 | imprimir;

             instrucciones.Rule = MakeStarRule(instrucciones, instruccion);

             metodo.Rule = ToTerm(Constantes.tipoVoid) + identificador + "(" + ")" + "{" + instrucciones + "}";

             cuerpoArchivo.Rule = MakeStarRule(cuerpoArchivo, metodo);




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


             this.Root = cuerpoArchivo;


        }

        
        



    }
}
