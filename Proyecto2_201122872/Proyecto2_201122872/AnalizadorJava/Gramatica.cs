using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;


namespace Proyecto2_201122872.AnalizadorJava
{
    class Gramatica:Grammar
    {
         public Gramatica()
            : base(caseSensitive: false)
        {

            #region expresiones regulares

            var identificador = TerminalFactory.CreateCSharpIdentifier("identificador");
            var numero = TerminalFactory.CreateCSharpNumber("numero");
            var val_false = Constantes.falso;
            var val_true = Constantes.verdadero;
             StringLiteral cadena = new StringLiteral("cadena", "\"");
            var caracter = TerminalFactory.CreateCSharpChar(Constantes.caracter);

            CommentTerminal COMENT_BLOQUE = new CommentTerminal("COMENTARIO BLOQUE", "#*", "*#");
            CommentTerminal COMENT_LINEA = new CommentTerminal("COMENTARIO LINEA", "#", "\n", "\r\n");
            NonGrammarTerminals.Add(COMENT_BLOQUE);
            NonGrammarTerminals.Add(COMENT_LINEA);

            #endregion


            #region palabras reservadas
            var tipoDecimal = Constantes.tipoDecimal;
            var tipoString = Constantes.tipoCadena;
            var tipoBoolean = Constantes.tipoBool;
            var tipoCaracter = Constantes.tipoChar;
            var tipoInt = Constantes.tipoEntero;
            var tipoVoid = Constantes.tipoVoid;

            var visiPublico = Constantes.publico;
            var visiPrivado = Constantes.privado;
            var visiProtegido = Constantes.protegido;

            var clase = Constantes.clase;
            var sobreescribir = Constantes.sobreescribir;
            

          
             

            #endregion




            #region No terminales
            NonTerminal FUN_SOBRE = new NonTerminal(Constantes.funSobre);
            NonTerminal TIPO = new NonTerminal(Constantes.tipo);
            NonTerminal FUNCION = new NonTerminal(Constantes.funcion);
            NonTerminal VISIBILIDAD = new NonTerminal(Constantes.visibilidad);
            NonTerminal CUERPO = new NonTerminal(Constantes.cuerpo);
            NonTerminal PARAMETRO = new NonTerminal(Constantes.parametro);
            NonTerminal L_PARAMETRO = new NonTerminal(Constantes.listaParametros);
            NonTerminal L_ELEMENTOS = new NonTerminal(Constantes.listaElementos);
            NonTerminal ATRIBUTO = new NonTerminal(Constantes.atributo);
            NonTerminal L_IDS = new NonTerminal(Constantes.listaIds);
            NonTerminal EXPRESION = new NonTerminal(Constantes.expresion);
            NonTerminal ELEMENTO = new NonTerminal(Constantes.elementoClase);
            NonTerminal CLASE = new NonTerminal(Constantes.clase);
            NonTerminal CUERPO_CLASE = new NonTerminal(Constantes.CUERPO_CLASE);
            NonTerminal PRINCIPAL = new NonTerminal(Constantes.principal);
            NonTerminal CONSTRUCTOR = new NonTerminal(Constantes.constructor);
            
            NonTerminal SI = new NonTerminal(Constantes.si);
            NonTerminal EXTRA = new NonTerminal(Constantes.extraSi);
            NonTerminal SINO = new NonTerminal(Constantes.sino);
            NonTerminal CUERPOSI = new NonTerminal(Constantes.cuerposi);
            NonTerminal L_EXTRAS = new NonTerminal(Constantes.lextra);






            #endregion

            #region gramatica


            #region clases

            CLASE.Rule = ToTerm(clase) + identificador + identificador + CUERPO_CLASE
              | ToTerm(clase) + identificador + CUERPO_CLASE;


            CUERPO_CLASE.Rule = ToTerm("{") + L_ELEMENTOS + ToTerm("}");


            L_ELEMENTOS.Rule = MakeStarRule(L_ELEMENTOS, ELEMENTO);

            ELEMENTO.Rule = FUNCION
                | ATRIBUTO
                | PRINCIPAL
                | CONSTRUCTOR
                | FUN_SOBRE;

            FUN_SOBRE.Rule = ToTerm("@") + ToTerm(sobreescribir) + FUNCION;

            #endregion



            #region funciones

            PRINCIPAL.Rule = ToTerm(Constantes.principal) + ToTerm("(") + ToTerm(")") + CUERPO;

            CONSTRUCTOR.Rule = identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO;

            FUNCION.Rule = VISIBILIDAD + TIPO + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | VISIBILIDAD + ToTerm(tipoVoid) + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | TIPO + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO
                           | ToTerm(tipoVoid) + identificador + ToTerm("(") + L_PARAMETRO + ToTerm(")") + CUERPO;



            CUERPO.Rule = ToTerm("{") + "}";




            #endregion




            EXPRESION.Rule = numero
                | identificador
                | caracter
                | cadena
                | ToTerm(val_false)
                | ToTerm(val_true);


            ATRIBUTO.Rule = VISIBILIDAD + TIPO + L_IDS + ToTerm(";")
                | VISIBILIDAD + TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";")
                | VISIBILIDAD + TIPO + identificador + ToTerm("=") + ToTerm(Constantes.nuevo) + identificador + ToTerm("(") + ToTerm(")") + ToTerm(";")
                | TIPO + L_IDS + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + ToTerm(Constantes.nuevo) + identificador + ToTerm("(") + ToTerm(")") + ToTerm(";");

            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);


            

            TIPO.Rule = ToTerm(tipoBoolean)
                | ToTerm(tipoCaracter)
                | ToTerm(tipoDecimal)
                | ToTerm(tipoInt)
                | ToTerm(tipoString)
                | identificador;


            VISIBILIDAD.Rule = ToTerm(visiPrivado)
                | ToTerm(visiProtegido)
                | ToTerm(visiPublico);


            PARAMETRO.Rule = TIPO + identificador;
            
            L_PARAMETRO.Rule = MakeStarRule(L_PARAMETRO, ToTerm(","), PARAMETRO);




            #region sentenciasControl

            SI.Rule = ToTerm(Constantes.si) + "(" + EXPRESION + ")" + CUERPO + L_EXTRAS + SINO
                | ToTerm(Constantes.si) + "(" + EXPRESION + ")" + CUERPO + L_EXTRAS;


            SINO.Rule = ToTerm(Constantes.sino) + CUERPO;

            EXTRA.Rule = ToTerm(Constantes.sino) + Constantes.si + CUERPO;

            L_EXTRAS.Rule = MakeStarRule(L_EXTRAS, EXTRA);


           








            MarkPunctuation(",", "(", ")", ";", "=", "@", "{","}","clase");


           
           MarkTransient(L_ELEMENTOS, ELEMENTO);















            this.Root = CLASE;








            #endregion




        }

    }
}
