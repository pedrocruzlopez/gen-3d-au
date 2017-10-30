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

            var doble = new RegexBasedTerminal("doble", "[0-9]+[.][0-9]+");
            var entero = new RegexBasedTerminal("entero", "[0-9]+");
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

            NonTerminal MIENTRAS = new NonTerminal(Constantes.mientras);
            NonTerminal REPETIR = new NonTerminal(Constantes.repetir);
            NonTerminal HACER = new NonTerminal(Constantes.hacer);
            NonTerminal X = new NonTerminal(Constantes.x);
            NonTerminal PARA = new NonTerminal(Constantes.para);
            NonTerminal IMPRIMIR = new NonTerminal(Constantes.imprimir);
            NonTerminal DECLAPARA = new NonTerminal("DECLAPARA");
            NonTerminal INSTRUCCIONES = new NonTerminal(Constantes.instrucciones);
            NonTerminal INSTRUCCION = new NonTerminal(Constantes.instruccion);
            NonTerminal DECLARACION = new NonTerminal(Constantes.decla2);
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
            NonTerminal TERMINO = new NonTerminal(Constantes.termino);
            NonTerminal RETORNO = new NonTerminal(Constantes.retorno);
            NonTerminal LISTACLASES = new NonTerminal(Constantes.l_clases);




            NonTerminal SUMA = new NonTerminal(Constantes.suma);
            NonTerminal RESTA = new NonTerminal(Constantes.resta);
            NonTerminal MULTIPLICACION= new NonTerminal(Constantes.multiplicacion);
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




            #endregion

            #region gramatica


            #region clases

            LISTACLASES.Rule = MakePlusRule(LISTACLASES, CLASE);
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



            CUERPO.Rule = ToTerm("{")+INSTRUCCIONES + "}";




            #endregion





            RETORNO.Rule = ToTerm(Constantes.retorno) + EXPRESION + ToTerm(";");

            ATRIBUTO.Rule = VISIBILIDAD + TIPO + L_IDS + ToTerm(";")
                | VISIBILIDAD + TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";") //1
                | TIPO + L_IDS + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + EXPRESION + ToTerm(";") //1
                | VISIBILIDAD + TIPO + identificador + LPOSICIONES + ToTerm(";")
                | VISIBILIDAD + TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
                | TIPO + identificador + LPOSICIONES + ToTerm(";")
                | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";";


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
                | ToTerm(Constantes.si) + "(" + EXPRESION + ")" + CUERPO + L_EXTRAS
                | ToTerm(Constantes.si) + "(" + EXPRESION + ")" + CUERPO + SINO;



            SINO.Rule = ToTerm(Constantes.sino) + CUERPO;

            EXTRA.Rule = ToTerm(Constantes.sino) + Constantes.si + CUERPO;

            L_EXTRAS.Rule = MakeStarRule(L_EXTRAS, EXTRA);


            MIENTRAS.Rule = ToTerm(Constantes.mientras) + "(" + EXPRESION + ")" + CUERPO;

            HACER.Rule = ToTerm(Constantes.hacer) + CUERPO + Constantes.mientras + "(" + EXPRESION + ")" + ";";

            X.Rule = ToTerm(Constantes.x) + "(" + EXPRESION + "," + EXPRESION + ")" + CUERPO;

            REPETIR.Rule = ToTerm(Constantes.repetir) + CUERPO + Constantes.until + "(" + EXPRESION + ")" + ";";



            PARA.Rule = ToTerm(Constantes.para) + "(" + DECLAPARA + ";" + EXPRESION + ";" + EXPRESION + ")" + CUERPO;

            IMPRIMIR.Rule = ToTerm(Constantes.imprimir) + "(" + EXPRESION + ")"+";";


            INSTRUCCION.Rule = IMPRIMIR
                | PARA//--
                | REPETIR//--
                | X
                | HACER//--
                | MIENTRAS//--
                | SI//--
                | DECLARACION//
                | ASIGNACION//--
                | EXPRESION + ToTerm(";")//--
                | RETORNO;//--

            DECLAPARA.Rule = DECLARACION
                | ASIGNACION;

            INSTRUCCIONES.Rule = MakeStarRule(INSTRUCCIONES, INSTRUCCION);




            DECLARACION.Rule = TIPO + identificador + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + EXPRESION + ";"
                | TIPO + identificador + LPOSICIONES + ToTerm(";")
                | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
                | TIPO + identificador + ToTerm("=") + INSTANCIA + ";"; 


            ASIGNACION.Rule = EXPRESION + ToTerm("=") + EXPRESION + ";";








            #region expresion


            POSICION.Rule = ToTerm("[") + EXPRESION + "]";

            LPOSICIONES.Rule = MakePlusRule(LPOSICIONES, POSICION);

            FILA.Rule = ToTerm("{") + LEXPRESIONES + "}";

            

            LFILAS.Rule = MakePlusRule(LFILAS, ToTerm(","), FILA);

            DECIMAL.Rule = doble;
            ENTERO.Rule = entero;
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



            SUMA.Rule = TERMINO + ToTerm(Constantes.suma) + TERMINO;
            RESTA.Rule = TERMINO + ToTerm(Constantes.resta) + TERMINO;
            MULTIPLICACION.Rule = TERMINO + ToTerm(Constantes.multiplicacion) + TERMINO;
            DIVISION.Rule = TERMINO + ToTerm(Constantes.division) + TERMINO;
            POTENCIA.Rule = TERMINO + ToTerm(Constantes.potencia) + TERMINO;
            MENOR.Rule = TERMINO + ToTerm(Constantes.menor) + TERMINO;
            MENORIGUAL.Rule = TERMINO + ToTerm(Constantes.menorIgual) + TERMINO;
            MAYOR.Rule = TERMINO + ToTerm(Constantes.mayor) + TERMINO;
            MAYORIGUAL.Rule = TERMINO + ToTerm(Constantes.mayorIgual) + TERMINO;
            IGUALIGUAL.Rule = TERMINO + ToTerm(Constantes.igualIgual) + TERMINO;
            DISTINTOA.Rule = TERMINO + ToTerm(Constantes.distintoA) + TERMINO;
            XOR.Rule = TERMINO + ToTerm(Constantes.xorJava) + TERMINO;
            AND.Rule = TERMINO + ToTerm(Constantes.andJava) + TERMINO;
            NOT.Rule = ToTerm(Constantes.notJavaPython) + TERMINO;
            OR.Rule = TERMINO + ToTerm(Constantes.orJava) + TERMINO;


            
            INSTANCIA.Rule = Constantes.nuevo + identificador + "(" + LEXPRESIONES + ")"
                | Constantes.nuevo + identificador + "(" + ")";

            UNARIO.Rule = MAS_MAS
                | MENOS_MENOS;

            MAS_MAS.Rule = identificador + ToTerm(Constantes.masmas);

            MENOS_MENOS.Rule = identificador + ToTerm(Constantes.menosmenos);


            NEGATIVO.Rule = ToTerm("-") + TERMINO;

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
                | "{" + LFILAS + "}";//no existe en python;
               // | INSTANCIA;//



            LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
                | identificador + ToTerm("(") + ")";


            POSVECTOR.Rule = identificador + LPOSICIONES;

            L_IDS.Rule = MakePlusRule(L_IDS, ToTerm(","), identificador);


            LEXPRESIONES.Rule = MakePlusRule(LEXPRESIONES, ToTerm(","), EXPRESION);
            EXPRESION.Rule = MakePlusRule(EXPRESION, ToTerm("."), TERMINO);

            

            




            #endregion



            this.RegisterOperators(9, UNARIO,NEGATIVO);
            this.RegisterOperators(8, Associativity.Right, "^");
            this.RegisterOperators(7, Associativity.Left, "/", "*");
            this.RegisterOperators(6, Associativity.Left, "-", "+");
            this.RegisterOperators(5, "==", "!=", "<", ">", "<=", ">=");
            this.RegisterOperators(4, Associativity.Left, "NOT");
            this.RegisterOperators(3, Associativity.Left, "&&");
            this.RegisterOperators(2, Associativity.Left, "??");
            this.RegisterOperators(1, Associativity.Left, "||");
            this.RegisterOperators(10, Associativity.Left, "(");




            MarkPunctuation(",", "(", ")", ";", "=", "@", "{","}","clase","[","]",Constantes.nuevo,".","si","sino",
                "mientras","hacer","para","x","repetir","return","imprimir",Constantes.masmas, Constantes.menosmenos,
                Constantes.menor, Constantes.mayor, Constantes.menorIgual, Constantes.mayorIgual, Constantes.igualIgual, Constantes.distintoA,
                Constantes.principal,Constantes.orJava, Constantes.andJava, Constantes.xorJava, Constantes.notJavaPython,sobreescribir,"*","^","+","-","/");


           
           MarkTransient(L_ELEMENTOS, ELEMENTO,POSICION,SIMB_ARIT,SIMB_LOG,SIMB_REL,DECLAPARA, INSTRUCCION, INSTRUCCIONES,
               ARITMETICA,LOGICA,RELACIONAL,UNARIO,INSTRUCCIONES,TERMINO,CUERPO_CLASE);




           this.Root =  LISTACLASES;






            #endregion



            #endregion

        }

    }
}
