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
    public class GramaticaTree:Grammar
    {

        public GramaticaTree()
            : base(caseSensitive: false)
        {

            #region expresiones regulares

            var identificador = TerminalFactory.CreateCSharpIdentifier("identificador");
            var numero = TerminalFactory.CreateCSharpNumber("numero");
            var val_false = Constantes.falso;
            var val_true = Constantes.verdadero;
            StringLiteral cadena = new StringLiteral("cadena", "\"");
            var caracter = TerminalFactory.CreateCSharpChar(Constantes.caracter);

            CommentTerminal COMENT_BLOQUE = new CommentTerminal("COMENTARIO BLOQUE", "{--", "--}");
            CommentTerminal COMENT_LINEA = new CommentTerminal("COMENTARIO LINEA", "##", "\n", "\r\n");
            NonGrammarTerminals.Add(COMENT_BLOQUE);
            NonGrammarTerminals.Add(COMENT_LINEA);

            #endregion


            #region declaracion de producciones
            NonTerminal
                VISIBILIDAD = new NonTerminal(Constantes.visibilidad),
                TIPO = new NonTerminal(Constantes.tipo),
                L_IDS = new NonTerminal(Constantes.listaIds),
                DECLRACION = new NonTerminal(Constantes.declaracion),
                ATRIBUTO = new NonTerminal(Constantes.atributo),
                INSTRUCCION = new NonTerminal(Constantes.instruccion),
                INSTRUCCIONES = new NonTerminal(Constantes.instrucciones),
                CUERPO = new NonTerminal(Constantes.cuerpo),
                FUNCION = new NonTerminal(Constantes.funcion),
                FUNCION_SOBRE = new NonTerminal(Constantes.funSobre),
                METODO = new NonTerminal(Constantes.metodo),
                PARAMETRO = new NonTerminal(Constantes.parametro),
                PARAMETROS = new NonTerminal(Constantes.listaParametros),
                CONSTRUCTOR = new NonTerminal(Constantes.constructor),
                ASIGNACION = new NonTerminal(Constantes.asignacion),
                SI = new NonTerminal(Constantes.si),
                SI_NO = new NonTerminal(Constantes.sino),
                EXTRA = new NonTerminal(Constantes.extraSi),
                L_EXTRAS = new NonTerminal(Constantes.lextra);



            NonTerminal
                EXPRESION = new NonTerminal(Constantes.expresion),
                ID = new NonTerminal(Constantes.id),
                ENTERO = new NonTerminal(Constantes.tipoEntero),
                DECIMAL = new NonTerminal(Constantes.tipoDecimal),
                BOOL = new NonTerminal(Constantes.tipoBool),
                CADENA = new NonTerminal(Constantes.tipoCadena),
                TERMINO = new NonTerminal(Constantes.termino),
                CHAR = new NonTerminal(Constantes.tipoChar),
                POSICION = new NonTerminal(Constantes.posicion),
                POSICIONES = new NonTerminal(Constantes.lposiciones);



            #endregion



            #region reglas semanticas

            VISIBILIDAD.Rule = ToTerm(Constantes.publico)
                | ToTerm(Constantes.protegido)
                | ToTerm(Constantes.privado);


            TIPO.Rule = ToTerm(Constantes.tipoBool)
                | ToTerm(Constantes.tipoCadena)
                | ToTerm(Constantes.tipoChar)
                | ToTerm(Constantes.tipoDecimal)
                | ToTerm(Constantes.tipoEntero)
                | identificador;


            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);


            #region instrucciones clase

            ATRIBUTO.Rule = VISIBILIDAD + TIPO + L_IDS;

            PARAMETRO.Rule = TIPO + identificador;

            PARAMETROS.Rule = MakeStarRule(PARAMETROS, ToTerm(","), PARAMETRO);

            FUNCION.Rule = VISIBILIDAD + ToTerm(Constantes.metodo) + identificador + ToTerm("[") + PARAMETROS + "]" + ":" + Eos + CUERPO
                | ToTerm(Constantes.metodo) + identificador + ToTerm("[") + PARAMETROS + "]" + ":" + Eos + CUERPO
                | VISIBILIDAD + ToTerm(Constantes.funcion) + TIPO + identificador + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO
                | ToTerm(Constantes.funcion) + TIPO + identificador + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO;

            FUNCION_SOBRE.Rule = ToTerm("/**Sobreescribir**/") + FUNCION;


            CONSTRUCTOR.Rule = ToTerm("__constructor") + "[" + PARAMETROS + "]" + ":" + Eos + CUERPO;


            #endregion




            #region instrucciones funcion
            

            CUERPO.Rule = Indent + INSTRUCCIONES + Dedent;

            INSTRUCCIONES.Rule = MakeStarRule(INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos
                | SI;


            DECLRACION.Rule = TIPO + L_IDS
                | TIPO + L_IDS + POSICIONES;

            ASIGNACION.Rule = EXPRESION + ToTerm("=") + ">" + EXPRESION;


            #region estructuras de control

            SI.Rule = ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS + SI_NO
               | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS
               | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + SI_NO;



            SI_NO.Rule = ToTerm(Constantes.sino_python) + ":" + Eos + CUERPO;

            EXTRA.Rule = ToTerm(Constantes.sino_si_python) + EXPRESION + ":" + Eos + CUERPO;

            L_EXTRAS.Rule = MakeStarRule(L_EXTRAS, EXTRA);

            #endregion






            #endregion


            #region expresion

            POSICION.Rule = ToTerm("[") + EXPRESION + "]";

            POSICIONES.Rule = MakePlusRule(POSICIONES, POSICION);

            ID.Rule = identificador;
            ENTERO.Rule = numero;
            DECIMAL.Rule = numero;
            CADENA.Rule = cadena;
            CHAR.Rule = caracter;
            BOOL.Rule = ToTerm(val_true)
                | ToTerm(val_false);

            TERMINO.Rule = ID
                | ENTERO
                | DECIMAL
                | CADENA
                | CHAR
                | BOOL;

            EXPRESION.Rule = TERMINO;
            



            #endregion

            #endregion



            this.Root = FUNCION;

        }


        public override void CreateTokenFilters(LanguageData language, TokenFilterList filters)
        {
            var outlineFilter = new CodeOutlineFilter(language.GrammarData,
              OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces, ToTerm(@"\")); // "\" is continuation symbol
            filters.Add(outlineFilter);

        }



    }
}
