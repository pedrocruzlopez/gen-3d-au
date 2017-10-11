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
    class AnalizadorPy:Grammar
    {



        public AnalizadorPy()
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




            #region declaproducciones
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

            



   
           
            NonTerminal IMPRIMIR = new NonTerminal(Constantes.imprimir);
            NonTerminal DECLAPARA = new NonTerminal("DECLAPARA");
            NonTerminal INSTRUCCIONES = new NonTerminal(Constantes.instrucciones);
            NonTerminal INSTRUCCION = new NonTerminal(Constantes.instruccion);
            NonTerminal DECLARACION = new NonTerminal(Constantes.declaracion);
            NonTerminal LPOSICIONES = new NonTerminal(Constantes.lposiciones);
            NonTerminal LEXPRESIONES = new NonTerminal(Constantes.lexpresiones);
            NonTerminal LFILAS = new NonTerminal(Constantes.lfilas);
            NonTerminal ASIGNACION = new NonTerminal(Constantes.asignacion);
            NonTerminal POSICION = new NonTerminal(Constantes.posicion);
            NonTerminal FILA = new NonTerminal(Constantes.fila);


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
            NonTerminal RETORNO = new NonTerminal(Constantes.retorno);
            NonTerminal LISTACLASES = new NonTerminal(Constantes.l_clases);



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
            NonTerminal SALIR = new NonTerminal(Constantes.salir);
            NonTerminal CONTINUAR = new NonTerminal(Constantes.continuar);

            NonTerminal SI = new NonTerminal(Constantes.si);
            NonTerminal EXTRA = new NonTerminal(Constantes.extraSi);
            NonTerminal SINO = new NonTerminal(Constantes.sino);
            NonTerminal CUERPOSI = new NonTerminal(Constantes.cuerposi);
            NonTerminal L_EXTRAS = new NonTerminal(Constantes.lextra);

            NonTerminal MIENTRAS = new NonTerminal(Constantes.mientras);
            NonTerminal REPETIR = new NonTerminal(Constantes.repetir);
            NonTerminal HACER = new NonTerminal(Constantes.hacer);
            NonTerminal PARA = new NonTerminal(Constantes.para);
            NonTerminal LOOP = new NonTerminal(Constantes.loop);


            #endregion

            VISIBILIDAD.Rule = ToTerm(visiPrivado)
               | ToTerm(visiProtegido)
               | ToTerm(visiPublico);


            TIPO.Rule = ToTerm(tipoBoolean)
               | ToTerm(tipoCaracter)
               | ToTerm(tipoDecimal)
               | ToTerm(tipoInt)
               | ToTerm(tipoString)
               | identificador;


            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);

            DECLARACION.Rule = TIPO + L_IDS
                | TIPO + L_IDS + LPOSICIONES;


            POSICION.Rule = ToTerm("[") + EXPRESION + "]";

            LPOSICIONES.Rule = MakePlusRule(LPOSICIONES, POSICION);

            FILA.Rule = ToTerm("{") + LEXPRESIONES + "}";

            

            LFILAS.Rule = MakePlusRule(LFILAS, ToTerm(","), FILA);


            ASIGNACION.Rule = EXPRESION + ToTerm("=") + ">" + EXPRESION;




            INSTRUCCION.Rule = DECLARACION + Eos
                | CONTINUAR + Eos
                | SALIR + Eos
                | MIENTRAS + Eos
                | HACER
                | REPETIR
                | LOOP + Eos;


            INSTRUCCIONES.Rule = MakePlusRule(INSTRUCCIONES, INSTRUCCION);

            CUERPO.Rule = Indent + INSTRUCCIONES + Dedent;

            FUNCION.Rule = VISIBILIDAD + ToTerm("funcion") + TIPO + identificador + "[" + "]" + Eos + CUERPO;




            #region estructuras de control

            CONTINUAR.Rule = ToTerm(Constantes.continuar);

            SALIR.Rule = ToTerm(Constantes.salir);

            MIENTRAS.Rule = ToTerm(Constantes.mientras) + EXPRESION + ":" + Eos + CUERPO;

            HACER.Rule = ToTerm(Constantes.hacer) + ":" + Eos + CUERPO  + ToTerm(Constantes.mientras) + EXPRESION+ Eos;

            REPETIR.Rule = ToTerm(Constantes.repetir) + ":" + Eos + CUERPO + ToTerm(Constantes.hasta) + EXPRESION + Eos;

            LOOP.Rule = ToTerm(Constantes.loop) + ":" + Eos + CUERPO;

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         




            



            #endregion









            #region expresion




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
            XOR.Rule = EXPRESION + ToTerm(Constantes.xorPython) + EXPRESION;
            AND.Rule = EXPRESION + ToTerm(Constantes.andPython) + EXPRESION;
            NOT.Rule = ToTerm(Constantes.notJavaPython) + EXPRESION;
            OR.Rule = EXPRESION + ToTerm(Constantes.orPython) + EXPRESION;
           
            INSTANCIA.Rule = Constantes.nuevoPython + identificador + "[" + LEXPRESIONES + "]"
                | Constantes.nuevoPython + identificador + "[" + "]";

            UNARIO.Rule = MAS_MAS
                | MENOS_MENOS;

            MAS_MAS.Rule = identificador + ToTerm(Constantes.masmas);

            MENOS_MENOS.Rule = identificador + ToTerm(Constantes.menosmenos);


            NEGATIVO.Rule = ToTerm("-") + EXPRESION;

            termino.Rule = ToTerm("(") + ARITMETICA + ")"
                | ToTerm("[") + RELACIONAL + "]"
                | ToTerm("{") + LOGICA + "}"
                | DECIMAL
                | ENTERO
                | ID
                | CADENA
                | BOOLEANO
                | CHAR
                | LLAMADA
                | POSVECTOR
                | ToTerm("(") + UNARIO + ")"
                | NEGATIVO
                | "{" + LFILAS + "}"
                | INSTANCIA;

            LLAMADA.Rule = identificador + ToTerm("[") + LEXPRESIONES + "]"
                | identificador + ToTerm("[") + "]";


            POSVECTOR.Rule = identificador + LPOSICIONES;

            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);




            EXPRESION.Rule = MakePlusRule(EXPRESION, ToTerm("."), termino);

            LEXPRESIONES.Rule = MakePlusRule(EXPRESION, ToTerm(","), EXPRESION);






            #endregion



            NonTerminal n = new NonTerminal("fdsfd");
            n.Rule = termino + Eos;



            this.Root = n;


        }


        public override void CreateTokenFilters(LanguageData language, TokenFilterList filters)
        {
            var outlineFilter = new CodeOutlineFilter(language.GrammarData,
              OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces, ToTerm(@"\")); // "\" is continuation symbol
            filters.Add(outlineFilter);

        }


    }
}
