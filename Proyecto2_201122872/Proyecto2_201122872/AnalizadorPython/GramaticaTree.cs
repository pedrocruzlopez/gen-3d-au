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
                L_EXTRAS = new NonTerminal(Constantes.lextra),
                SALIR = new NonTerminal(Constantes.salir),
                CONTINUAR = new NonTerminal(Constantes.constructor),
                MIENTRAS = new NonTerminal(Constantes.mientras),
                HACER = new NonTerminal(Constantes.hacer),
                REPETIR = new NonTerminal(Constantes.repetir),
                PARA = new NonTerminal(Constantes.para),
                LOOP = new NonTerminal(Constantes.loop),
                CASO = new NonTerminal(Constantes.caso),
                DEFECTO = new NonTerminal(Constantes.defecto),
                LISTACASOS = new NonTerminal(Constantes.lcasos),
                ELEGIR = new NonTerminal(Constantes.elegir),
                CUERPOELEGIR = new NonTerminal(Constantes.cuerpoElegir),
                LLAMADA = new NonTerminal(Constantes.llamada),
                INSTANCIA = new NonTerminal(Constantes.instancia),
                LEXPRESIONES = new NonTerminal(Constantes.lexpresiones),
                CLASE = new NonTerminal(Constantes.clase);

            NonTerminal LISTACLASES = new NonTerminal(Constantes.l_clases),
               CUERPO_CLASE = new NonTerminal(Constantes.CUERPO_CLASE),
               L_ELEMENTOS = new NonTerminal(Constantes.listaElementos),
               ELEMENTO = new NonTerminal(Constantes.elementoClase);

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
                POSICIONES = new NonTerminal(Constantes.lposiciones),
                OUT_STRING = new NonTerminal(Constantes.out_string),
                PARSEINT = new NonTerminal(Constantes.parseint),
                PARSEDOUBLE = new NonTerminal(Constantes.parsedouble),
                INTTOSTR = new NonTerminal(Constantes.inttostr),
                DOUBLETOSTR = new NonTerminal(Constantes.doubletostr),
                DOUBLETOINT = new NonTerminal(Constantes.doubletoint),
                SUPER = new NonTerminal(Constantes.super),
                SELF = new NonTerminal(Constantes.self),
                DECLAARREGLO = new NonTerminal(Constantes.declaArreglo);


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

           

            LISTACLASES.Rule = MakePlusRule(LISTACLASES, CLASE);
            CLASE.Rule = ToTerm(Constantes.clase) + identificador + "[" + identificador + "]"+":"+Eos + CUERPO_CLASE
              | ToTerm(Constantes.clase) + identificador + "[" + "]"+":"+Eos + CUERPO_CLASE;


            CUERPO_CLASE.Rule = Indent + L_ELEMENTOS + Dedent;


            L_ELEMENTOS.Rule = MakeStarRule(L_ELEMENTOS, ELEMENTO);

            ELEMENTO.Rule = FUNCION
                | ATRIBUTO
                | CONSTRUCTOR
                | FUNCION_SOBRE;



            #endregion
             



            #region instrucciones funcion
            

            CUERPO.Rule = Indent + INSTRUCCIONES + Dedent;

            INSTRUCCIONES.Rule = MakeStarRule(INSTRUCCIONES, INSTRUCCION);

            INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos
                | SI
                | SALIR + Eos
                | CONTINUAR + Eos
                | MIENTRAS
                | PARA
                | LOOP
                | HACER
                | REPETIR
                | ELEGIR;

            DECLAARREGLO.Rule = identificador + POSICIONES;

            DECLRACION.Rule = TIPO + L_IDS
                | TIPO + DECLAARREGLO;

            ASIGNACION.Rule = EXPRESION + ToTerm("=") + ">" + EXPRESION;


            #region estructuras de control

            SI.Rule = ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS + SI_NO
               | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + L_EXTRAS
               | ToTerm(Constantes.si) + EXPRESION + ":" + Eos + CUERPO + SI_NO;



            SI_NO.Rule = ToTerm(Constantes.sino_python) + ":" + Eos + CUERPO;

            EXTRA.Rule = ToTerm(Constantes.sino_si_python) + EXPRESION + ":" + Eos + CUERPO;

            L_EXTRAS.Rule = MakeStarRule(L_EXTRAS, EXTRA);


            SALIR.Rule = ToTerm(Constantes.salir);
            CONTINUAR.Rule = ToTerm(Constantes.continuar);

            MIENTRAS.Rule = ToTerm(Constantes.mientras) + EXPRESION+":" + Eos + CUERPO;

            HACER.Rule = ToTerm(Constantes.hacer) + ":" + Eos + CUERPO + Constantes.mientras + EXPRESION+Eos;

            REPETIR.Rule = ToTerm(Constantes.repetir) + ":" + Eos + CUERPO + Constantes.hasta + EXPRESION+Eos;

            PARA.Rule = ToTerm(Constantes.para) + "[" + ASIGNACION + ":" + EXPRESION + ":" + EXPRESION + "]" + ":" + Eos + CUERPO;

            LOOP.Rule = ToTerm(Constantes.loop) + ":" + Eos + CUERPO;

            CASO.Rule = EXPRESION + ToTerm(":") + Eos + CUERPO;
            
            DEFECTO.Rule = ToTerm(Constantes.defecto) + ":" + Eos + CUERPO;

            LISTACASOS.Rule = MakeStarRule(LISTACASOS, CASO);

            CUERPOELEGIR.Rule = Indent + LISTACASOS + DEFECTO + Dedent
                | Indent + LISTACASOS + Dedent
                | Indent + DEFECTO + Dedent;

            ELEGIR.Rule = ToTerm(Constantes.elegir) + EXPRESION + ":" + Eos + CUERPOELEGIR;



            #endregion






            #endregion


            #region expresion

            LEXPRESIONES.Rule = MakeStarRule(LEXPRESIONES, ToTerm(","), EXPRESION);

            LLAMADA.Rule = identificador + ToTerm("[") + LEXPRESIONES + "]"
                | identificador + ToTerm("[") + "]";

            INSTANCIA.Rule = ToTerm(Constantes.nuevoPython) + identificador + ToTerm("[") + LEXPRESIONES + "]"
                | ToTerm(Constantes.nuevoPython) + identificador + ToTerm("[") + "]";



            OUT_STRING.Rule = ToTerm(Constantes.out_string) + "[" + EXPRESION + "]";
            PARSEINT.Rule = ToTerm(Constantes.parseint) + "[" + EXPRESION + "]";
            PARSEDOUBLE.Rule = ToTerm(Constantes.parsedouble) + "[" + EXPRESION + "]";
            INTTOSTR.Rule = ToTerm(Constantes.inttostr) + "[" + EXPRESION + "]";
            DOUBLETOSTR.Rule = ToTerm(Constantes.doubletostr) + "[" + EXPRESION + "]";
            DOUBLETOINT.Rule = ToTerm(Constantes.doubletoint) + "[" + EXPRESION + "]";


            POSICION.Rule = ToTerm("[") + EXPRESION + "]";

            POSICIONES.Rule = MakePlusRule(POSICIONES, POSICION);
            SUPER.Rule= ToTerm(Constantes.super);
            SELF.Rule= ToTerm(Constantes.self);

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
                | BOOL
                | OUT_STRING
                | PARSEINT
                | PARSEDOUBLE
                | INTTOSTR
                | DOUBLETOSTR
                | DOUBLETOINT
                | INSTANCIA
                | LLAMADA
                | SUPER
                | SELF
                | DECLAARREGLO;



            EXPRESION.Rule = MakePlusRule(EXPRESION, ToTerm("."), TERMINO);



            #endregion

            #endregion



            this.Root = LISTACLASES;

        }


        public override void CreateTokenFilters(LanguageData language, TokenFilterList filters)
        {
            var outlineFilter = new CodeOutlineFilter(language.GrammarData,
              OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces, ToTerm(@"\")); // "\" is continuation symbol
            filters.Add(outlineFilter);

        }



    }
}
