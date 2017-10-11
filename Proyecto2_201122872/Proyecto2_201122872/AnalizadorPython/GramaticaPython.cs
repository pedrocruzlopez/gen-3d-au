using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;

namespace Proyecto2_201122872.AnalizadorPython
{
    class GramaticaPython:Grammar
    {
        public GramaticaPython()
            : base(caseSensitive: false)
        {

            #region expresiones regulares

            var identificador = TerminalFactory.CreateCSharpIdentifier("identificador");
            var numero = TerminalFactory.CreateCSharpNumber("numero");
            var val_false = Constantes.falso;
            var val_true = Constantes.verdadero;
            StringLiteral cadena = new StringLiteral("cadena", "\"");
            var caracter = TerminalFactory.CreateCSharpChar(Constantes.caracter);

            CommentTerminal COMENT_BLOQUE = new CommentTerminal("COMENTARIO BLOQUE", "/*", "*/");
            CommentTerminal COMENT_LINEA = new CommentTerminal("COMENTARIO LINEA", "//", "\n", "\r\n");
            NonGrammarTerminals.Add(COMENT_BLOQUE);
            NonGrammarTerminals.Add(COMENT_LINEA);

            #endregion

            NonTerminal EXPRESION = new NonTerminal(Constantes.expresion);
            NonTerminal DECIMAL = new NonTerminal(Constantes.tipoDecimal);
            NonTerminal ENTERO = new NonTerminal(Constantes.tipoEntero);
            NonTerminal CADENA = new NonTerminal(Constantes.tipoCadena);
            NonTerminal CHAR = new NonTerminal(Constantes.tipoChar);
            NonTerminal BOOLEANO = new NonTerminal(Constantes.tipoBool);
            NonTerminal SIMB_ARIT = new NonTerminal(Constantes.simb_arit);
            NonTerminal SIMB_REL = new NonTerminal(Constantes.simb_rel);
            NonTerminal UNARIO = new NonTerminal(Constantes.unario);
            NonTerminal ID = new NonTerminal(Constantes.id);
            NonTerminal NEGATIVO = new NonTerminal(Constantes.negativo);
            NonTerminal RELACIONAL = new NonTerminal(Constantes.relacional);
            NonTerminal ARITMETICA = new NonTerminal(Constantes.aritmetica);
            NonTerminal LOGICA = new NonTerminal(Constantes.logica);
            NonTerminal LLAMADA = new NonTerminal(Constantes.llamada);
            NonTerminal POSVECTOR = new NonTerminal(Constantes.posvector);
            NonTerminal LISTAPUNTOS = new NonTerminal(Constantes.listapuntos);
            NonTerminal SIMB_LOG = new NonTerminal(Constantes.simb_log);
            NonTerminal MAS_MAS = new NonTerminal(Constantes.masmas);
            NonTerminal MENOS_MENOS = new NonTerminal(Constantes.menosmenos);
            NonTerminal ASIG_ARRAY = new NonTerminal(Constantes.asig_array);
            NonTerminal termino = new NonTerminal(Constantes.termino);

            NonTerminal L_IDS = new NonTerminal(Constantes.listaIds);
            NonTerminal SUMA = new NonTerminal(Constantes.suma);
            NonTerminal RESTA = new NonTerminal(Constantes.resta);
            NonTerminal MULTIPLICACION = new NonTerminal(Constantes.multiplicacion);
            NonTerminal DIVISION = new NonTerminal(Constantes.division);
            NonTerminal POTENCIA = new NonTerminal(Constantes.potencia);
            NonTerminal MENOR = new NonTerminal(Constantes.menor);
            NonTerminal MENORIGUAL = new NonTerminal(Constantes.menorIgual);
            NonTerminal MAYOR = new NonTerminal(Constantes.mayor);
            NonTerminal MAYORIGUAL = new NonTerminal(Constantes.mayorIgual);
            NonTerminal IGUALIGUAL = new NonTerminal(Constantes.igualIgual);
            NonTerminal DISTINTOA = new NonTerminal(Constantes.distintoA);
            NonTerminal XOR = new NonTerminal(Constantes.xorJava);
            NonTerminal AND = new NonTerminal(Constantes.andJava);
            NonTerminal NOT = new NonTerminal(Constantes.notJavaPython);
            NonTerminal OR = new NonTerminal(Constantes.orJava);
            NonTerminal INSTANCIA = new NonTerminal(Constantes.instancia);
            NonTerminal POSICION = new NonTerminal(Constantes.posicion);
            NonTerminal FILA = new NonTerminal(Constantes.fila);
            NonTerminal LPOSICIONES = new NonTerminal(Constantes.lposiciones);
            NonTerminal LEXPRESIONES = new NonTerminal(Constantes.lexpresiones);
            NonTerminal LFILAS = new NonTerminal(Constantes.lfilas);


            #region expresion


            POSICION.Rule = ToTerm("[") + EXPRESION + "]";

            LPOSICIONES.Rule = MakePlusRule(LPOSICIONES, POSICION);

            FILA.Rule = ToTerm("{") + LEXPRESIONES + "}";

            LEXPRESIONES.Rule = MakePlusRule(LEXPRESIONES, ToTerm(","), EXPRESION);

            LFILAS.Rule = MakePlusRule(LFILAS, ToTerm(","), FILA);

            DECIMAL.Rule = numero;
            ENTERO.Rule = numero;
            CADENA.Rule = cadena;
            ID.Rule = identificador;
            CHAR.Rule = caracter;
            BOOLEANO.Rule = ToTerm(val_false)
                | ToTerm(val_true);





            ARITMETICA.Rule = SUMA
                | RESTA
                | MULTIPLICACION
                | DIVISION
                | POTENCIA;

            RELACIONAL.Rule = MENOR
                | MAYOR
                | MENORIGUAL
                | MAYORIGUAL
                | DISTINTOA
                | IGUALIGUAL;

            LOGICA.Rule = XOR
                | OR
                | AND
                | NOT;





            SUMA.Rule = EXPRESION + ToTerm(Constantes.suma) + EXPRESION;
            RESTA.Rule = EXPRESION + ToTerm(Constantes.resta) + EXPRESION;
            MULTIPLICACION.Rule = EXPRESION + ToTerm(Constantes.multiplicacion) + EXPRESION;
            DIVISION.Rule = EXPRESION + ToTerm(Constantes.division) + EXPRESION;
            POTENCIA.Rule = EXPRESION + ToTerm(Constantes.potencia) + EXPRESION;
            MENOR.Rule = EXPRESION + ToTerm(Constantes.menor) + EXPRESION;
            MENORIGUAL.Rule = EXPRESION + ToTerm(Constantes.menorIgual) + EXPRESION;
            MAYOR.Rule = EXPRESION + ToTerm(Constantes.mayor) + EXPRESION;
            MAYORIGUAL.Rule = EXPRESION + ToTerm(Constantes.mayorIgual) + EXPRESION;
            IGUALIGUAL.Rule = EXPRESION + ToTerm(Constantes.igualIgual) + EXPRESION;
            DISTINTOA.Rule = EXPRESION + ToTerm(Constantes.distintoA) + EXPRESION;
            XOR.Rule = EXPRESION + ToTerm(Constantes.xorJava) + EXPRESION;
            AND.Rule = EXPRESION + ToTerm(Constantes.andJava) + EXPRESION;
            NOT.Rule = ToTerm(Constantes.notJavaPython) + EXPRESION;
            OR.Rule = EXPRESION + ToTerm(Constantes.orJava) + EXPRESION;
            INSTANCIA.Rule = Constantes.nuevo + identificador + "(" + LEXPRESIONES + ")"
                | Constantes.nuevo + identificador + "(" + ")";

            UNARIO.Rule = MAS_MAS
                | MENOS_MENOS;

            MAS_MAS.Rule = identificador + ToTerm(Constantes.masmas);

            MENOS_MENOS.Rule = identificador + ToTerm(Constantes.menosmenos);


            NEGATIVO.Rule = ToTerm("-") + EXPRESION;

            termino.Rule = //ARITMETICA
                //| RELACIONAL
                // | LOGICA
                 DECIMAL
                | ENTERO
                | ID
                | CADENA
                | BOOLEANO
                | CHAR;
                //| LLAMADA
                //| POSVECTOR
                //| UNARIO
                //| ToTerm("(") + EXPRESION + ")"
                //| NEGATIVO
                //| "{" + LFILAS + "}"
                //| INSTANCIA;

            LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
                | identificador + ToTerm("(") + ")";


            POSVECTOR.Rule = identificador + LPOSICIONES;

            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);



            EXPRESION.Rule = MakePlusRule(EXPRESION, ToTerm("."), termino);



            NonTerminal n = new NonTerminal("fdsfd");
            n.Rule=  EXPRESION;




            #endregion


            Root = n;


        }

        
    }
}
