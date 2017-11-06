using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.AnalizadorPython;
using Proyecto2_201122872.Errores;
using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using Proyecto2_201122872.UML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Proyecto2_201122872.Generacion3D
{
    public class GeneracionCodigo
    {

        public tablaDeSimbolos tablaSimbolos;
        public clasesDiagrama uml;
        private Arbol analizadorJava;
        private ArbolPy analizadorPython;
        string nombreObj = "";
        public Codigo c3d;

        public GeneracionCodigo()
        {
            this.tablaSimbolos = new tablaDeSimbolos();
            uml = new clasesDiagrama();
            analizadorJava = new Arbol();
            analizadorPython = new ArbolPy();

            c3d = new Codigo();
        }



        private string getCadenaArchivo(String ruta)
        {
            StreamReader archivo = new StreamReader(ruta);
            string linea;
            string contenido = "";
            while ((linea = archivo.ReadLine()) != null)
            {
                contenido += linea + "\n";
            }

            archivo.Close();
            return contenido;

        }

        public void ejecutarArchivos(String ruta, String extension)
        {
            String contenido = getCadenaArchivo(ruta);
            if (extension.Equals(".tree", StringComparison.OrdinalIgnoreCase))
            {
                uml = analizadorPython.parseConvertirUML2(contenido);
                generarTablaSimbolos();
            }
            else if (extension.Equals(".olc", StringComparison.OrdinalIgnoreCase))
            {
                clasesDiagrama uml2 = analizadorJava.parseConvertirUML2(contenido);
                uml = uml2.agregarHerencia();
                generarTablaSimbolos();

            }

        }


        public void generarTablaSimbolos()
        {
            if (uml != null)
            {
                foreach (Clase clase in uml.listaClases)
                {
                    this.tablaSimbolos.addLista(clase.getSimbolosClase());
                }
            }
        }


        /*------------------------------------- Generacion de codigo -------------------------------------*/

        #region escribirC3D clases
        public void escribirC3DClases()
        {
            Ambitos ambito = new Ambitos();
            /*
             * 1. Creacion de constructores si los tuviera sino se crea uno por defecto = new persona() <----
             * 2. Creacion de metodo principal si los tuviera sino asi se queda
             * 3. Creacion de las demas funciones
             */

            foreach (Clase claseActual in uml.listaClases)
            {
                //1. creacion de constructores
                ambito.addAmbito(claseActual.getNombre());
                List<Funcion> constructores = claseActual.getConstructores();
                Funcion principal;
                List<Funcion> funcionesMetodos;
                if (constructores.Count > 0)
                {
                    escribirConstructores(constructores, ambito, claseActual.getNombre());

                }
                else
                {
                    //no posee constructor


                }
                //2. Creacion del metodo principal, si este lo tuviese

                principal = claseActual.getPrincipal();
                if (principal != null)
                {
                    ambito.addAmbito(principal.firma);
                    c3d.addCodigo("Principal(){");
                    if (principal.cuerpo != null)
                        evaluarCuerpo(principal.cuerpo, ambito, principal.firma, claseActual.getNombre());
                    c3d.addCodigo("}");
                    ambito.ambitos.Pop();
                }

                //3. escribir los demas metodos y funciones

                funcionesMetodos = claseActual.getFunciones();
                escribirConstructores(funcionesMetodos, ambito, claseActual.getNombre());
                ambito.ambitos.Pop();

            }



        }




        private void escribirConstructores(List<Funcion> constructores, Ambitos ambito, String nombreClase)
        {
            foreach (Funcion constructor in constructores)
            {
                ambito.addAmbito(constructor.firma);//ingresando el ambito del constructor
                c3d.addCodigo(Constantes.tipoVoid + " " + constructor.firma + "(){");
                if (constructor.cuerpo != null)
                    evaluarCuerpo(constructor.cuerpo, ambito, constructor.firma, nombreClase);
                c3d.addCodigo("}");
                ambito.ambitos.Pop();//saliendo del ambito del constructor
            }
        }

        #endregion






        #region generacion c3d cuerpo


        private bool sonNulos(List<String> tipos)
        {
            bool res = true;
            for (int i = 0; i < tipos.Count; i++)
            {
                res = res && (tipos.ElementAt(i).Equals("nulo", StringComparison.OrdinalIgnoreCase));
            }
            return res;
        }


        public void evaluarCuerpo(ParseTreeNode nodo, Ambitos ambitos, String nombreMetodo, String nombreClase)
        {

            switch (nodo.Term.Name)
            {


                case Constantes.retorno:
                    {

                        string tipoExpresion = validarTipo(nodo.ChildNodes[0], ambitos, nombreClase, nombreMetodo).ToString();
                        Console.WriteLine("retornor de tipo " + tipoExpresion);

                        string temp1 = c3d.getTemporal();
                        string l1 = temp1 + " = P + 0;";
                        string temp2 = c3d.getTemporal();
                        //string l2 = temp2 + " = STACK[" + temp1 + "];//APuntador del objto en el heap";
                        string temp3 = c3d.getTemporal();
                        int posRetorno = tablaSimbolos.getPosicion(Constantes3D.retorno, ambitos);
                        string l3 = temp3 + " = " + temp1 + " + " + posRetorno + ";//pos del return";
                        string temp4 = c3d.getTemporal();
                        string val = resolverExpresiones(nodo.ChildNodes[0], ambitos, nombreClase, nombreMetodo).ToString();
                        string l4 = "STACK[" + temp3 + "] = " + val + "; //asigno al return";
                        c3d.addCodigo(l1);
                        //c3d.addCodigo(l2);
                        c3d.addCodigo(l3);
                        c3d.addCodigo(l4);
                        break;
                    }

                #region inicialesCuerpo
                case Constantes.cuerpo:
                    {
                        if (nodo.ChildNodes.Count > 0)
                            evaluarCuerpo(nodo.ChildNodes[0], ambitos, nombreMetodo, nombreClase);
                        else
                            break;
                        break;
                    }
                case Constantes.instrucciones:
                    {
                        foreach (ParseTreeNode nodoHijo in nodo.ChildNodes)
                        {
                            evaluarCuerpo(nodoHijo, ambitos, nombreMetodo, nombreClase);
                        }
                        break;
                    }
                case Constantes.instruccion:
                    {
                        evaluarCuerpo(nodo.ChildNodes[0], ambitos, nombreMetodo, nombreClase);
                        break;
                    }

                #endregion

                #region asignacion


                case Constantes.asignacion:
                    {
                        /*ASIGNACION.Rule = EXPRESION + ToTerm("=") + EXPRESION + ";"
                | EXPRESION + ToTerm("=") + INSTANCIA;
                         NonTerminal THIS = new NonTerminal("THIS");
             NonTerminal SUPER = new NonTerminal("SUPER");
             NonTerminal LLAMADAOBJETO = new NonTerminal("LLAMADA_OBJETO") O UN ID O ASIG POS VECTOR;
                         */
                        string nombreExpresion = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
                        if (nombreExpresion.Equals("THIS") ||
                            nombreExpresion.Equals("SUPER") ||
                            nombreExpresion.Equals("LLAMADA_OBJETO") ||
                            nombreExpresion.Equals(Constantes.id) ||
                            nombreExpresion.Equals(Constantes.posvector))
                        {
                            if (nombreExpresion.Equals("THIS"))
                            {
                                #region asignar a un this
                                ParseTreeNode nodoThis = nodo.ChildNodes[0].ChildNodes[0];
                                ParseTreeNode nodoExpThis = nodo.ChildNodes[1];
                                string temp1 = c3d.getTemporal();
                                string l1 = temp1 + " = P + 0; //pos this";
                                string temp2 = c3d.getTemporal();
                                string l2 = temp2 + " = STACK[" + temp1 + "]; //apuntador del this en el heap";
                                int noHijos = nodoThis.ChildNodes.Count;
                                //es un this.id
                                if (noHijos == 1 && nodoThis.ChildNodes[0].Term.Name.Equals(Constantes.id))
                                {
                                    string id = nodoThis.ChildNodes[0].ChildNodes[0].Token.ValueString;
                                    int posId = tablaSimbolos.getPosicionDeClase(id, ambitos);
                                    string tipoId = tablaSimbolos.getTipo(id, ambitos);
                                    if (posId != -1)
                                    {
                                        if (tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                                           tipoId.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase) ||
                                            tipoId.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                                            tipoId.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                                            tipoId.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                                        {
                                            string temp1_1 = c3d.getTemporal(); //pos que necesito asginar
                                            string l1_1 = temp1_1 + " = " + temp2 + " + " + posId + "; //pos de " + id;
                                            string temp2_1 = c3d.getTemporal();
                                            string l1_2 = temp2_1 + " = HEAP[ " + temp1_1 + "];//valor de " + id;

                                            if (!nodo.ChildNodes[1].Term.Name.Equals(Constantes.instancia))
                                            {
                                                Object tipoExpresion = validarTipo(nodoExpThis, ambitos,nombreClase,nombreMetodo);
                                                if (tipoExpresion.ToString().Equals(tipoId, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    c3d.addCodigo(l1);
                                                    c3d.addCodigo(l2);
                                                    c3d.addCodigo(l1_1);
                                                    c3d.addCodigo(l1_2);
                                                    Object val = resolverExpresiones(nodoExpThis, ambitos, nombreClase, nombreMetodo);
                                                    string l3 = "HEAP[" + temp1_1 + "] = " + val.ToString() + ";";
                                                    c3d.addCodigo(l3);
                                                }

                                            }
                                            else
                                            {
                                                ErrorA er = new ErrorA(Constantes.errorSemantico, "El atributo " + id + ", no posee un tipo valido para una instancia", nodo.FindToken());
                                                Form1.errores.addError(er);
                                            }

                                        }
                                        else
                                        {
                                            //es un objeto
                                            if (nodo.ChildNodes[1].Term.Name.Equals(Constantes.instancia))
                                            {

                                            }
                                            else
                                            {
                                                ErrorA er = new ErrorA(Constantes.errorSemantico, "El atributo " + id + ", debe posee una instancia", nodo.FindToken());
                                                Form1.errores.addError(er);
                                            }


                                        }
                                    }
                                    else
                                    {
                                        ErrorA er = new ErrorA(Constantes.errorSemantico, "El atributo " + id + ", no existe", nodo.FindToken());
                                        Form1.errores.addError(er);
                                    }

                                }
                                #endregion
                            }
                            else if (nombreExpresion.Equals(Constantes.id))
                            {
                                #region asinganar a un id
                                ParseTreeNode opIzq = nodo.ChildNodes[0].ChildNodes[0];
                                ParseTreeNode nodoExpThis = nodo.ChildNodes[1];
                                string nombreId = opIzq.ChildNodes[0].Token.ValueString;



                                #endregion
                            }
                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Expresion no validad para una asignacion", nodo.ChildNodes[0].FindToken());
                            Form1.errores.addError(er);
                        }

                        break;

                    }

                #endregion


                #region Declaraciones

                case Constantes.declaracion:
                    {//declaracion para variables de pythodn , lo unicio que se debe hacer es calcular el tamanho  de un arreglo

                        break;
                    }

                case Constantes.decla2:
                    {
                        /* DECLARACION.Rule = TIPO + identificador + ToTerm(";") no se hace
             | TIPO + identificador + ToTerm("=") + EXPRESION + ";"
             | TIPO + identificador + LPOSICIONES + ToTerm(";") no se hace
             | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
              */

                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 3)
                        {
                            //TIPO + identificador + ToTerm("=") + INSTANCIA + ";";

                            #region instancias
                            if (nodo.ChildNodes[2].Term.Name.Equals(Constantes.instancia, StringComparison.OrdinalIgnoreCase))
                            {
                                //es una instancia
                                String nombreObjetoInstanciar = nodo.ChildNodes[1].Token.ValueString;
                                string tipoInstancia = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                                ParseTreeNode nodoInstancia = nodo.ChildNodes[2];
                                string tipoInstacia2 = nodoInstancia.ChildNodes[0].Token.ValueString;

                                if (tipoInstancia.Equals(tipoInstacia2, StringComparison.OrdinalIgnoreCase))
                                {
                                    int sizeObjeto = tablaSimbolos.sizeClase(tipoInstancia);
                                    if (sizeObjeto != -1)
                                    {//objeto a instanciar si existe
                                        if (nodoInstancia.ChildNodes.Count == 2)
                                        {//posee parametros
                                            int posObjInstanciar = tablaSimbolos.getPosicion(nombreObjetoInstanciar, ambitos);
                                            if (posObjInstanciar != -1)
                                            {
                                                //resolvemos el tipo de parametros
                                                string cad = "";
                                                for (int i = 0; i < nodoInstancia.ChildNodes[1].ChildNodes.Count; i++)
                                                {
                                                    cad += validarTipo(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos,nombreClase,nombreMetodo);//resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                                                }

                                                string firmaMetodo = tablaSimbolos.getFirmaMetodo(nombreClase, cad, nombreMetodo);
                                                if (!firmaMetodo.Equals(""))
                                                {

                                                    //1. Crear apuntador en el stack de heap para el nuevo objeto
                                                    string temp1 = c3d.getTemporal();
                                                    string l1 = temp1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar;
                                                    string l2 = "STACK[" + temp1 + "] = H; //apuntador del heap en el stack del nuevo obj";

                                                    //2. Reservo espacio en el heap para el nuevo objeto
                                                    string l3 = "H = H + " + sizeObjeto + "; //reservar espacio para atributos";
                                                    c3d.addCodigo(l1);
                                                    c3d.addCodigo(l2);
                                                    c3d.addCodigo(l3);

                                                    //3. Verifico si atributos posee asignacion ej publico a = 4;
                                                    List<Simbolo> atributosClase = tablaSimbolos.obtenerAtributosClase(tipoInstancia);
                                                    foreach (Simbolo item in atributosClase)
                                                    {
                                                        if (item.expresionAtributo != null)
                                                        {//genero el codigo3d para el atributo
                                                            Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambitos,nombreClase,nombreMetodo);
                                                            if (tipoExpAtributo.ToString().Equals(item.tipo, StringComparison.OrdinalIgnoreCase))
                                                            {
                                                                string temp1_1 = c3d.getTemporal();
                                                                c3d.addCodigo(temp1_1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar);
                                                                string temp2_1 = c3d.getTemporal();
                                                                c3d.addCodigo(temp2_1 + " = STACK[" + temp1_1 + "];//apuntador del this en el heap");
                                                                string temp3_1 = c3d.getTemporal();
                                                                c3d.addCodigo(temp3_1 + " = " + temp2_1 + " + " + item.apuntador + ";//this + pos de " + item.nombreReal);
                                                                nombreObj = item.nombreReal;
                                                                Object val = resolverExpresiones(item.expresionAtributo, ambitos, nombreClase, nombreMetodo);
                                                                c3d.addCodigo("HEAP[" + temp3_1 + "] = " + val.ToString() + ";");
                                                            }
                                                            else
                                                            {
                                                                ErrorA er = new ErrorA(Constantes.errorSemantico, "Operacion no valida para el tipo de " + item.tipo + ", " + tipoExpAtributo, item.expresionAtributo.FindToken());
                                                                Form1.errores.addError(er);
                                                            }
                                                        }
                                                    }
                                                    string temp2 = c3d.getTemporal();
                                                    string l4 = temp2 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar;
                                                    string temp3 = c3d.getTemporal();
                                                    string l5 = temp3 + " = STACK[" + temp2 + "]; //pos this del apuntador del heap";
                                                    int tamanhoFuncActual = tablaSimbolos.sizeFuncion(nombreClase, nombreMetodo);
                                                    string temp4 = c3d.getTemporal();
                                                    ambitos.addAmbito(firmaMetodo);
                                                    string l6 = temp4 + " = P + " + tamanhoFuncActual + "; //p+ size funcion actual";
                                                    string temp5 = c3d.getTemporal();
                                                    string l7 = temp5 + " = " + temp4 + " + 0; //pos this en el constructor";
                                                    string l8 = "STACK[" + temp5 + "] = " + temp3 + "; //guardo en el this del constructor el ap del heap";
                                                    // Asignar Parametros
                                                    c3d.addCodigo(l4);
                                                    c3d.addCodigo(l5);
                                                    c3d.addCodigo(l6);
                                                    c3d.addCodigo(l7);
                                                    c3d.addCodigo(l8);
                                                    ParseTreeNode nodoParametros = nodoInstancia.ChildNodes[1];
                                                    int cont = 1;
                                                    foreach (ParseTreeNode item in nodoParametros.ChildNodes)
                                                    {
                                                        Object val = resolverExpresiones(item, ambitos, nombreClase, nombreMetodo);
                                                        string temp1_1 = c3d.getTemporal();
                                                        string l1_1 = temp1_1 + " = P + " + tamanhoFuncActual + "; // p + size func actual";
                                                        c3d.addCodigo(l1_1);
                                                        string temp2_1 = c3d.getTemporal();
                                                        l1_1 = temp2_1 + " = " + temp1_1 + " + " + cont + "; //pos del parametro";
                                                        c3d.addCodigo(l1_1);
                                                        l1_1 = "STACK[" + temp2_1 + "] = " + val + "; //paso por valor del parametro";
                                                        c3d.addCodigo(l1_1);
                                                        cont++;
                                                    }

                                                    string l9 = "P = P + " + tamanhoFuncActual + ";";
                                                    string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, cad, tipoInstacia2);
                                                    string l10 = "call " + firmaLlamada + "();";
                                                    string l11 = "P = P - " + tamanhoFuncActual + ";";

                                                    c3d.addCodigo(l9);
                                                    c3d.addCodigo(l10);
                                                    c3d.addCodigo(l11);
                                                }
                                                else
                                                {
                                                    //no existe un constructor con esos parametros
                                                }


                                            }

                                            else
                                            {
                                                posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObjetoInstanciar, ambitos);
                                                if (posObjInstanciar != -1)
                                                {
                                                    Console.WriteLine("tengo que instanciar un atributo");
                                                    //es un atributo al cual vamos a instanciar
                                                }
                                                else
                                                {
                                                    ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + nombreObjetoInstanciar + ", no existe.", nodo.FindToken());
                                                    Form1.errores.addError(nue);

                                                }

                                            }
                                        }
                                        else
                                        {//no tiene parametros
                                            int posObjInstanciar = tablaSimbolos.getPosicion(nombreObjetoInstanciar, ambitos);
                                            if (posObjInstanciar != -1)
                                            {//si existe y es una vairable local
                                                //1. Crear apuntador en el stack de heap para el nuevo objeto
                                                string temp1 = c3d.getTemporal();
                                                string l1 = temp1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar;
                                                string l2 = "STACK[" + temp1 + "] = H; //apuntador del heap en el stack del nuevo obj";

                                                //2. Reservo espacio en el heap para el nuevo objeto
                                                string l3 = "H = H + " + sizeObjeto + "; //reservar espacio para atributos";
                                                c3d.addCodigo(l1);
                                                c3d.addCodigo(l2);
                                                c3d.addCodigo(l3);

                                                //3. Verifico si atributos posee asignacion ej publico a = 4;
                                                List<Simbolo> atributosClase = tablaSimbolos.obtenerAtributosClase(tipoInstancia);
                                                foreach (Simbolo item in atributosClase)
                                                {
                                                    if (item.expresionAtributo != null)
                                                    {//genero el codigo3d para el atributo
                                                        Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambitos,nombreClase,nombreMetodo);
                                                        if (tipoExpAtributo.ToString().Equals(item.tipo, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            string temp1_1 = c3d.getTemporal();
                                                            c3d.addCodigo(temp1_1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar);
                                                            string temp2_1 = c3d.getTemporal();
                                                            c3d.addCodigo(temp2_1 + " = STACK[" + temp1_1 + "];//apuntador del this en el heap");
                                                            string temp3_1 = c3d.getTemporal();
                                                            c3d.addCodigo(temp3_1 + " = " + temp2_1 + " + " + item.apuntador + ";//this + pos de " + item.nombreReal);

                                                            Object val = resolverExpresiones(item.expresionAtributo, ambitos, nombreClase, nombreMetodo);
                                                            c3d.addCodigo("HEAP[" + temp3_1 + "] = " + val.ToString() + ";");
                                                        }
                                                        else
                                                        {
                                                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Operacion no valida para el tipo de " + item.tipo + ", " + tipoExpAtributo, item.expresionAtributo.FindToken());
                                                            Form1.errores.addError(er);
                                                        }
                                                    }
                                                }
                                                string temp2 = c3d.getTemporal();
                                                string l4 = temp2 + " = P + " + posObjInstanciar + "; //pos de " + nombreObjetoInstanciar;
                                                string temp3 = c3d.getTemporal();
                                                string l5 = temp3 + " = STACK[" + temp2 + "]; //pos this del apuntador del heap";
                                                int tamanhoFuncActual = tablaSimbolos.sizeFuncion(nombreClase, nombreMetodo);
                                                string temp4 = c3d.getTemporal();
                                                string l6 = temp4 + " = P + " + tamanhoFuncActual + "; //p+ size funcion actual";
                                                string temp5 = c3d.getTemporal();
                                                string l7 = temp5 + " = " + temp4 + " + 0; //pos this en el constructor";
                                                string l8 = "STACK[" + temp5 + "] = " + temp3 + "; //guardo en el this del constructor el ap del heap";
                                                string l9 = "P = P + " + tamanhoFuncActual + ";";
                                                string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, "", tipoInstacia2);
                                                string l10 = "call " + firmaLlamada + "();";
                                                string l11 = "P = P - " + tamanhoFuncActual + ";";
                                                c3d.addCodigo(l4);
                                                c3d.addCodigo(l5);
                                                c3d.addCodigo(l6);
                                                c3d.addCodigo(l7);
                                                c3d.addCodigo(l8);
                                                c3d.addCodigo(l9);
                                                c3d.addCodigo(l10);
                                                c3d.addCodigo(l11);
                                            }
                                            else
                                            {
                                                posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObjetoInstanciar, ambitos);
                                                if (posObjInstanciar != -1)
                                                {
                                                    //es un atributo al cual vamos a instanciar
                                                }
                                                else
                                                {
                                                    ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + nombreObjetoInstanciar + ", no existe.", nodo.FindToken());
                                                    Form1.errores.addError(nue);

                                                }
                                            }


                                        }


                                    }
                                    else
                                    {//objeto a instaincair no existe
                                        ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + tipoInstancia + ", no existe.", nodo.FindToken());
                                        Form1.errores.addError(nue);
                                    }

                                }
                                else
                                {
                                    ErrorA nue = new ErrorA(Constantes.errorSemantico, tipoInstancia + ", no es igual a " + tipoInstacia2 + ", no se puede realizar la instancia.", nodo.FindToken());
                                    Form1.errores.addError(nue);
                                }

                            }
                            #endregion


                            #region Arreglos





                            #endregion


                            #region expresion normal
                            if (nodo.ChildNodes[2].Term.Name.Equals(Constantes.expresion, StringComparison.OrdinalIgnoreCase))
                            {
                                //TIPO + identificador + ToTerm("=") + EXPRESION + ";"
                                string tipo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                                string nombreDeclaracion = nodo.ChildNodes[1].Token.ValueString;
                                ParseTreeNode nodoExpresion = nodo.ChildNodes[2];
                                int posId;
                                int noAmbiente = ambitos.ambitos.Count;
                                if (noAmbiente > 1)
                                {
                                    //ando en un ambiente local
                                    posId = tablaSimbolos.getPosicion(nombreDeclaracion, ambitos);
                                    if (posId != -1)
                                    {
                                        //es una variabel local
                                        string temp1 = c3d.getTemporal();
                                        string l1 = temp1 + " = P + " + posId + "; //pos de " + nombreDeclaracion;
                                        Object tipoExp = validarTipo(nodoExpresion, ambitos,nombreClase,nombreMetodo);
                                        if (tipoExp.ToString().Equals(tipo, StringComparison.OrdinalIgnoreCase))
                                        {
                                            Object val = resolverExpresiones(nodoExpresion, ambitos, nombreClase, nombreMetodo);
                                            string l2 = "STACK[" + temp1 + "] = " + val.ToString() + ";";
                                            c3d.addCodigo(l1);
                                            c3d.addCodigo(l2);
                                        }
                                        else
                                        {
                                            //error no coinciden los tipos
                                        }
                                    }
                                    else
                                    {
                                        posId = tablaSimbolos.getPosicionDeClase(nombreDeclaracion, ambitos);
                                        if (posId != -1)
                                        {
                                            // es un atributo de clase


                                        }
                                        else
                                        {
                                            //variabe no existe

                                        }
                                    }
                                }
                                else if (noAmbiente == 1)
                                {//ando eun ambiente global
                                    /*
                                    String temp1 = tabla.Obtener_Temporal();
                                    String tem2 = tabla.Obtener_Temporal();
                                    String temp3 = tabla.Obtener_Temporal();
                                    Codigo += temp1 + " = P + 0;\n";
                                    Codigo += tem2 + " = STACK[" + temp1 + "];\n";
                                    Codigo += temp3 + " = " + tem2 + " + " + pos + ";\n";
                                    Object val = Expresion_bool(nodo.ChildNodes[2]);
                                    Codigo += "HEAP[" + temp3 + "]= " + val + " ;\n";*/

                                }


                            }


                            #endregion
                        }



                        break;
                    }

                #endregion




                /*INSTRUCCION.Rule = DECLRACION + Eos
                | ASIGNACION + Eos
                | SI
                | SALIR + Eos
                | CONTINUAR + Eos
                | MIENTRAS
                | PARA
                | LOOP
                | HACER
                | REPETIR
                | ELEGIR
                | EXPRESION;*/

                #region While
                case Constantes.mientras:
                    {
                        // MIENTRAS.Rule = ToTerm(Constantes.mientras) + EXPRESION + ":" + Eos + CUERPO;
                        /*
                        L1: 
                        if (i < 10) goto L2; 
                        goto L3; 
                        //codigo i = i + 1;
                        goto L1: 
                        L3:
                        */

                        String etiqCiclo = c3d.getEtiqueta();
                        c3d.addCodigo(etiqCiclo + ":");
                        ambitos.addWhile();
                        Object g = null;// evaluarExp(nodo.ChildNodes[0].ChildNodes[0]);
                        if (g is nodoCondicion)
                        {
                            nodoCondicion condWhile = (nodoCondicion)g;
                            evaluarCuerpo(nodo.ChildNodes[1], ambitos, nombreMetodo, nombreClase);
                            c3d.addCodigo(Constantes3D.goto_ + " " + etiqCiclo + ";");
                            c3d.addCodigo(condWhile.getEtiquetasFalsas());

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Condicion no valida para un ciclo while", nodo.FindToken());
                            Form1.errores.addError(er);
                        }
                        ambitos.ambitos.Pop();

                        break;

                    }
                #endregion

                #region imprimir:

                case Constantes.imprimir:
                    {
                        //IMPRIMIR.Rule = ToTerm(Constantes.imprimir) + "(" + EXPRESION + ")"+";";
                        // OUT_STRING.Rule = ToTerm(Constantes.out_string) + "[" + EXPRESION + "]";
                        Object tipoImpresion = validarTipo(nodo.ChildNodes[0], ambitos,nombreClase,nombreMetodo);
                        Object val = resolverExpresiones(nodo.ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                        if (tipoImpresion.ToString().Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                            tipoImpresion.ToString().Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            c3d.addCodigo("print(\"%d\" , " + val + ");");
                        }
                        else if (tipoImpresion.ToString().Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase))
                        {
                            c3d.addCodigo("print(\"%f\" , " + val + ");");
                        }
                        else if (tipoImpresion.ToString().Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            c3d.addCodigo("print(\"%c\" , " + val + ");");
                        }
                        else if (tipoImpresion.ToString().Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase))
                        {
                            c3d.addCodigo("print(\"%s\" , " + val + ");");
                        }

                        break;
                    }

                #endregion







            }



        }






        #endregion



















        /* --- Generaacion de codigo  Expresiones  -------*/















        #region validacionTipos Retorna de que tipo es la expresion


        private Boolean esObjeto(String tipo)
        {
            if (tipo.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private object validarTipo(ParseTreeNode nodo, Ambitos ambito, string nombreClase, string nombreMetodo)
        {
            switch (nodo.Term.Name)
            {

                case Constantes.llamada:
                    {
                        int noHijos = nodo.ChildNodes.Count;
                        string firmaLlamada = "nulo";
                        String nombreLlamada = nodo.ChildNodes[0].Token.ValueString;
                        if (noHijos == 1)
                        {
                             firmaLlamada = tablaSimbolos.getTipoMetodoFuncion(nombreClase, "", nombreLlamada);
                            return firmaLlamada;

                        }
                        else
                        {
                            string cad = "";
                            for (int i = 0; i < nodo.ChildNodes[1].ChildNodes.Count; i++)
                            {
                                cad += validarTipo(nodo.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambito,nombreClase,nombreMetodo);//resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                            }
                             firmaLlamada = tablaSimbolos.getTipoMetodoFuncion(nombreClase, cad, nombreLlamada);
                            return firmaLlamada;
                        }

                        return "nulo";
                    }

                #region instancia
                case Constantes.instancia:
                    {
                        return nodo.ChildNodes[0].Token.ValueString;
                    }
                #endregion

                #region globales expresion
                case Constantes.termino:
                    {
                        return validarTipo(nodo.ChildNodes[0], ambito,nombreClase, nombreMetodo);
                    }
                case Constantes.expresion:
                    {
                        return validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                    }
                #endregion

                #region valores primitivos y Id (leer la i en ingles jaja asi no se lee y i)

                case Constantes.tipoEntero:
                    {
                        return Constantes.tipoEntero;
                    }
                case Constantes.tipoBool:
                    {
                        return Constantes.tipoBool;
                    }
                case Constantes.tipoCadena:
                    {
                        return Constantes.tipoCadena;
                    }
                case Constantes.tipoChar:
                    {
                        return Constantes.tipoChar;
                    }
                case Constantes.tipoDecimal:
                    {
                        return Constantes.tipoDecimal;
                    }

                case Constantes.id:
                    {
                        string tipo = tablaSimbolos.getTipo(nodo.ChildNodes[0].Token.ValueString, ambito);
                        if (tipo.Equals("nulo", StringComparison.OrdinalIgnoreCase))
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "La variable " + nodo.ChildNodes[0].Token.ValueString + " no existe", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }
                        else
                        {
                            return tipo;
                        }
                    }

                #endregion

                #region validacion Operaciones

                case Constantes.suma:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarSuma(tipo1, tipo2);
                    }
                case Constantes.resta:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarResta(tipo1, tipo2);
                    }

                case Constantes.multiplicacion:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarMultiplicacion(tipo1, tipo2);
                    }

                case Constantes.division:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarDivision(tipo1, tipo2);
                    }

                case Constantes.potencia:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarPotencia(tipo1, tipo2);
                    }

                #endregion

                #region Relacionales

                case Constantes.mayor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.menor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.mayorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }


                case Constantes.menorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.distintoA:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.igualIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarRelacional(val1, val2);
                    }
                #endregion

                #region this

                case "THIS":
                    {
                        //tipoAtributoClase(string nombreClase, String nombreAtributo)
                        int noHijos = nodo.ChildNodes.Count;
                        string nombreAtributo = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                        string tipoAtributo = tablaSimbolos.getTipo(nombreAtributo, ambito);
                        if (noHijos > 1)
                        {
                            if (esObjeto(tipoAtributo))
                            {
                                for (int i = 1; i < nodo.ChildNodes.Count; i++)
                                {
                                    nombreAtributo = nodo.ChildNodes[i].ChildNodes[0].Token.ValueString;
                                    tipoAtributo = uml.tipoAtributoClase(tipoAtributo, nombreAtributo);

                                }
                                return tipoAtributo;
                            }
                            else
                            {
                                ErrorA nuevo = new ErrorA(Constantes.errorSemantico, "Elemento de tipo " + tipoAtributo + ", no posee atributos", nodo.FindToken());
                                Form1.errores.addError(nuevo);
                            }
                        }
                        else
                        {
                            return tipoAtributo;
                        }


                        return "nulo";


                    }
                #endregion


                #region Logicas

                case Constantes.andJava:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarLogica(val1, val2);
                    }
                case Constantes.orJava:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarLogica(val1, val2);
                    }
                case Constantes.xorJava:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito, nombreClase, nombreMetodo);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito, nombreClase, nombreMetodo);
                        return validarLogica(val1, val2);
                        //return validarUnario(validarTipo(nodo.ChildNodes[0],ambito));
                    }

                case Constantes.notJavaPython:
                    {
                        return validarNot(validarTipo(nodo.ChildNodes[0], ambito,nombreClase, nombreMetodo));
                    }
                #endregion

                #region unarios

                case Constantes.masmas:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito,nombreClase, nombreMetodo));
                    }

                case Constantes.menosmenos:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito,nombreClase, nombreMetodo));
                    }

                case Constantes.negativo:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito,nombreClase, nombreMetodo));
                    }

                #endregion

            }




            return "nulo";
        }


        private object validarSuma(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                /*retornos tipo double */
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                //valiaciones  entero
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }// validaciones de tipo cadena
                else if (esCadena(val1) && esInt(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esDouble(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esDouble(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esInt(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esChar(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esChar(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }
                else if (esCadena(val1) && esCadena(val2))
                {
                    return Constantes.tipoCadena;
                }//tipo bool
                else if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";

            }
        }

        private object validarResta(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }//
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }//
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }



        }


        private object validarMultiplicacion(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }//
                else if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }


        private object validarDivision(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }


        }


        private object validarPotencia(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esInt(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esInt(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esChar(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esChar(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esBool(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esBool(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esDouble(val1) && esDouble(val2))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) && esChar(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esChar(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esBool(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esBool(val2))
                {
                    return Constantes.tipoEntero;
                }
                else if (esInt(val1) && esInt(val2))
                {
                    return Constantes.tipoEntero;
                }
                else
                {
                    return "nulo";
                }
            }
            else
            {
                return "nulo";
            }


        }


        private object validarUnario(Object val1)
        {
            if (!esNulo(val1))
            {
                if (esDouble(val1))
                {
                    return Constantes.tipoDecimal;
                }
                else if (esInt(val1) || esChar(val1))
                {
                    return Constantes.tipoEntero;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }


        private object validarRelacional(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {

                if ((esDouble(val1) || esInt(val1) || esChar(val1)) &&
                    esDouble(val2) || esInt(val2) || esChar(val2))
                {
                    return Constantes.tipoBool;
                }
                else if ((esCadena(val1) || esChar(val1)) &&
                    (esCadena(val2) || esChar(val2)))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }
        }



        private object validarLogica(Object val1, Object val2)
        {
            if (!esNulo(val1) && !esNulo(val2))
            {
                if (esBool(val1) && esBool(val2))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }

            }
            else
            {
                return "nulo";
            }

        }

        private object validarNot(Object val1)
        {
            if (!esNulo(val1))
            {
                if (esBool(val1))
                {
                    return Constantes.tipoBool;
                }
                else
                {
                    return "nulo";
                }
            }
            else
            {
                return "nulo";
            }
        }



        #region terminales

        private Boolean esNulo(Object val)
        {
            return val.ToString().Equals("nulo", StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esInt(Object val)
        {
            return val.ToString().Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase);
        }


        private Boolean esDouble(Object val)
        {
            return val.ToString().Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esCadena(Object val)
        {
            return val.ToString().Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esChar(Object val)
        {
            return val.ToString().Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase);
        }

        private Boolean esBool(Object val)
        {
            return val.ToString().Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase);
        }


        #endregion

        #endregion



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



        private Object llamadaFuncion(ParseTreeNode nodoLlamada, String nombreClase, String nommbreMetodo, Ambitos ambiente)
        {
            /* LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
                | identificador + ToTerm("(") + ")";*/
            int noHijos = nodoLlamada.ChildNodes.Count;
            string nombreLlamada = nodoLlamada.ChildNodes[0].Token.ValueString;
            string firmaLlamada = "";
            int sizeActual = tablaSimbolos.sizeFuncion(nombreClase, nommbreMetodo);
            if (noHijos == 1)
            {
                firmaLlamada = tablaSimbolos.getFirmaMetodoFuncion(nombreClase, "", nombreLlamada);
                if (!firmaLlamada.Equals(""))
                {

                    string temp1 = c3d.getTemporal();

                    string l1 = temp1 + " = P + 0;";
                    string temp11 = c3d.getTemporal();
                    string l11 = temp11 + " = STACK[" + temp1 + "];";

                    string temp2 = c3d.getTemporal();
                    string l2 = temp2 + " = P + " + sizeActual + "; // p + tamanho actual";
                    string temp3 = c3d.getTemporal();
                    string l3 = temp3 + " = " + temp2 + " + 0; //pos del this";
                    string l4 = "STACK[" + temp3 + "] = " + temp11 + ";//apuntador del objeto al heap";

                    //aqui asigno parametros 

                    string l5 = "P = P + " + sizeActual + ";";
                    string l6 = "call " + firmaLlamada + "();";
                    string temp4 = c3d.getTemporal();
                    string l7 = temp4 + " = P + " + (tablaSimbolos.sizeFuncion(nombreClase, firmaLlamada) - 1) + ";//pos del return";
                    string temp5 = c3d.getTemporal();
                    string l8 = temp5 + " = STACK[" + temp4 + "];//valor del return";
                    string l9 = "P = P - " + sizeActual + ";";

                    c3d.addCodigo(l1);
                    c3d.addCodigo(l11);
                    c3d.addCodigo(l2);
                    c3d.addCodigo(l3);
                    c3d.addCodigo(l4);
                    c3d.addCodigo(l5);
                    c3d.addCodigo(l6);
                    c3d.addCodigo(l7);
                    c3d.addCodigo(l8);
                    c3d.addCodigo(l9);


                    return temp5;
                }
                else
                {
                    ErrorA nuevo = new ErrorA(Constantes.errorSemantico, "No existe la funcion/procedimiento, " + nombreLlamada + ", sin parametros", nodoLlamada.FindToken());
                    Form1.errores.addError(nuevo);
                }


            }
            else if (noHijos == 2)
            {
                string cad = "";
                for (int i = 0; i < nodoLlamada.ChildNodes[1].ChildNodes.Count; i++)
                {
                    cad += validarTipo(nodoLlamada.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambiente,nombreClase,nommbreMetodo).ToString();//resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                }

                firmaLlamada = tablaSimbolos.getFirmaMetodoFuncion(nombreClase, cad, nombreLlamada);
                if (!firmaLlamada.Equals(""))
                {

                    string temp1 = c3d.getTemporal();
                    string l1 = temp1 + " = P + 0;";
                    string temp11 = c3d.getTemporal();
                    string l11 = temp11 + " = STACK[" + temp1 + "];";
                    string temp2 = c3d.getTemporal();
                    string l2 = temp2 + " = P + " + sizeActual + "; // p + tamanho actual";
                    string temp3 = c3d.getTemporal();
                    string l3 = temp3 + " = " + temp2 + " + 0; //pos del this";
                    string l4 = "STACK[" + temp3 + "] = " + temp11 + ";//apuntador del objeto al heap";

                    //aqui asigno parametros 

                    int cont = 1;
                    ParseTreeNode nodoParametros = nodoLlamada.ChildNodes[1];
                    
                    foreach (ParseTreeNode item in nodoParametros.ChildNodes)
                    {
                        Object val = resolverExpresiones(item, ambiente, nombreClase, nommbreMetodo);
                        string temp1_1 = c3d.getTemporal();
                        string l1_1 = temp1_1 + " = P + " + sizeActual + "; // p + size func actual";
                        c3d.addCodigo(l1_1);
                        string temp2_1 = c3d.getTemporal();
                        l1_1 = temp2_1 + " = " + temp1_1 + " + " + cont + "; //pos del parametro";
                        c3d.addCodigo(l1_1);
                        l1_1 = "STACK[" + temp2_1 + "] = " + val + "; //paso por valor del parametro";
                        c3d.addCodigo(l1_1);
                        cont++;
                    }

                    string l5 = "P = P + " + sizeActual + ";";
                    string l6 = "call " + firmaLlamada + "();";
                    string temp4 = c3d.getTemporal();
                    string l7 = temp4 + " = P + " + (tablaSimbolos.sizeFuncion(nombreClase, firmaLlamada) - 1) + ";//pos del return";
                    string temp5 = c3d.getTemporal();
                    string l8 = temp5 + " = STACK[" + temp4 + "];//valor del return";
                    string l9 = "P = P - " + sizeActual + ";";

                    c3d.addCodigo(l1);
                    c3d.addCodigo(l11);
                    c3d.addCodigo(l2);
                    c3d.addCodigo(l3);
                    c3d.addCodigo(l4);
                    c3d.addCodigo(l5);
                    c3d.addCodigo(l6);
                    c3d.addCodigo(l7);
                    c3d.addCodigo(l8);
                    c3d.addCodigo(l9);


                    return temp5;


                }
                else
                {
                    ErrorA nuevo = new ErrorA(Constantes.errorSemantico, "No existe la funcion/procedimiento, " + nombreLlamada + ", parametros, " + cad, nodoLlamada.FindToken());
                    Form1.errores.addError(nuevo);
                }

            }

            return "nulo";
        }








        public object resolverExpresiones(ParseTreeNode nodo, Ambitos ambiente, String nombreClase, String nommbreMetodo)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {

                case Constantes.termino:
                    {
                        return resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                    }
                case Constantes.expresion:
                    {
                        return resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                    }


                #region this
                case "THIS":
                    {
                        /* Pasos de verificacion:
                         * 1. Verificar que existan tanto las llamadas a funciones como los objetos y atributos que se hacen referencia
                         
                         */
                        string temp1 = c3d.getTemporal();
                        string l1 = temp1 + " = P + 0; //pos this";
                        string temp2 = c3d.getTemporal();
                        string l2 = temp2 + " = STACK[" + temp1 + "]; //apuntador del this en el heap";
                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 1)
                        {
                            string id = nodo.ChildNodes[0].ChildNodes[0].Token.ValueString;
                            int posId = tablaSimbolos.getPosicionDeClase(id, ambiente);
                            string tipoId = tablaSimbolos.getTipo(id, ambiente);
                            if (posId != -1)
                            {
                                if (tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                                   tipoId.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                                {
                                    string temp1_1 = c3d.getTemporal();
                                    string l1_1 = temp1_1 + " = " + temp2 + " + " + posId + "; //pos de " + id;
                                    string temp2_1 = c3d.getTemporal();
                                    string l1_2 = temp2_1 + " = HEAP[ " + temp1_1 + "];//valor de " + id;
                                    c3d.addCodigo(l1);
                                    c3d.addCodigo(l2);
                                    c3d.addCodigo(l1_1);
                                    c3d.addCodigo(l1_2);
                                    return temp2_1;
                                }
                                else
                                {
                                    //es un objeto
                                }
                            }
                            else
                            {
                                ErrorA er = new ErrorA(Constantes.errorSemantico, "El atributo " + id + ", no existe", nodo.FindToken());
                                Form1.errores.addError(er);
                            }
                        }
                        else if (noHijos > 1)
                        {//asegurar que sea valida la expresion


                        }


                        return "nulo";

                    }

                #endregion

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
                        string punteroHeapCadena = c3d.getTemporal();
                        c3d.addCodigo(punteroHeapCadena + " = H;");
                        string cadena = nodo.ChildNodes[0].Token.ValueString;

                        char caracter;
                        int ascii;


                        for (int i = 0; i < cadena.Length; i++)
                        {
                            caracter = cadena.ElementAt(i).ToString()[0];
                            ascii = (int)caracter;
                            c3d.addCodigo("HEAP[H] = " + ascii + "; // " + caracter);
                            c3d.addCodigo("H = H + 1;");
                        }

                        c3d.addCodigo("HEAP[H] = -1; //fin cadena ");
                        c3d.addCodigo("H = H + 1;");

                        return punteroHeapCadena;
                    }


                #endregion

                #region id

                case Constantes.id:
                    {
                        string nombreId = nodo.ChildNodes[0].Token.ValueString;
                        int posId = tablaSimbolos.getPosicion(nombreId, ambiente);
                        string tipoId = tablaSimbolos.getTipo(nombreId, ambiente);
                        if (posId != -1)
                        {
                            // es una variable local
                            if (tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                                tipoId.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                                tipoId.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                                tipoId.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                            {
                                string temp1 = c3d.getTemporal();
                                string l1 = temp1 + " = P + " + posId + ";//pos de " + nombreId;
                                string temp2 = c3d.getTemporal();
                                string l2 = temp2 + " = STACK[" + temp1 + "]; //valor de " + nombreId;
                                c3d.addCodigo(l1);
                                c3d.addCodigo(l2);
                                return temp2;
                            }
                            else if (tipoId.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase))
                            {
                                //es una cadena
                                string eti1 = c3d.getEtiqueta();
                                string eti2 = c3d.getEtiqueta();
                                string eti3 = c3d.getEtiqueta();
                                string eti4 = c3d.getEtiqueta();
                                string temp1 = c3d.getTemporal();
                                string l1 = temp1 + " = P + " + posId + "; //pos de " + nombreId;
                                string temp2 = c3d.getTemporal();
                                string l2 = temp2 + " =  STACK[" + temp1 + "];//apuntador de la cadenana en el heap";
                                string l3 = "goto " + eti1 + ":";
                                string l15 = eti1 + ":\n";
                                string temp3 = c3d.getTemporal();
                                string l4 = temp3 + " = HEAP[" + temp2 + "];";
                                string l5 = "if( " + temp3 + " == -1 ) then goto " + eti2 + ";";
                                string l6 = "goto " + eti3 + ";";
                                string l7 = eti2 + ":";
                                string l8 = "goto " + eti4 + ";";
                                string l9 = eti3 + ":";
                                string l10 = "HEAP[H] = " + temp3 + ";";
                                string l11 = "H = H + 1;";
                                string l12 = temp2 + " = " + temp2 + " + 1" + ";";
                                string l13 = "goto " + eti1 + ";";
                                string l14 = eti4 + ":";
                                c3d.addCodigo(l1);
                                c3d.addCodigo(l2);
                                c3d.addCodigo(l3);
                                c3d.addCodigo(l15);
                                c3d.addCodigo(l4);
                                c3d.addCodigo(l5);
                                c3d.addCodigo(l6);
                                c3d.addCodigo(l7);
                                c3d.addCodigo(l8);
                                c3d.addCodigo(l9);
                                c3d.addCodigo(l10);
                                c3d.addCodigo(l11);
                                c3d.addCodigo(l12);
                                c3d.addCodigo(l13);
                                c3d.addCodigo(l14);
                                return temp3;



                            }
                            else
                            {
                                // es un objeto
                            }
                        }
                        else
                        {
                            posId = tablaSimbolos.getPosicionDeClase(nombreId, ambiente);
                            if (posId != -1)
                            {
                                //es un atributo
                                // es una variable local
                                if (tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoDecimal, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoChar, StringComparison.OrdinalIgnoreCase) ||
                                    tipoId.Equals(Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                                {
                                    string temp1 = c3d.getTemporal();
                                    string l1 = temp1 + " = P + " + posId + ";//pos de " + nombreId;
                                    string temp2 = c3d.getTemporal();
                                    string l2 = temp2 + " = STACK[" + temp1 + "]; //valor de " + nombreId;
                                    c3d.addCodigo(l1);
                                    c3d.addCodigo(l2);
                                    return temp2;
                                }
                                else if (tipoId.Equals(Constantes.tipoCadena, StringComparison.OrdinalIgnoreCase))
                                {
                                    //es una cadena
                                    //es una cadena
                                    string eti1 = c3d.getEtiqueta();
                                    string eti2 = c3d.getEtiqueta();
                                    string eti3 = c3d.getEtiqueta();
                                    string eti4 = c3d.getEtiqueta();
                                    string temp1 = c3d.getTemporal();
                                    string l1 = temp1 + " = P + " + posId + "; //pos de " + nombreId;
                                    string temp2 = c3d.getTemporal();
                                    string l2 = temp2 + " =  STACK[" + temp1 + "];//apuntador de la cadenana en el heap";
                                    string l3 = "goto " + eti1 + ":";
                                    string l15 = eti1 + ":\n";
                                    string temp3 = c3d.getTemporal();
                                    string l4 = temp3 + " = HEAP[" + temp2 + "];";
                                    string l5 = "if( " + temp3 + " == -1 ) then goto " + eti2 + ";";
                                    string l6 = "goto " + eti3 + ";";
                                    string l7 = eti2 + ":";
                                    string l8 = "goto " + eti4 + ";";
                                    string l9 = eti3 + ":";
                                    string l10 = "HEAP[H] = " + temp3 + ";";
                                    string l11 = "H = H + 1;";
                                    string l12 = temp2 + " = " + temp2 + " + 1" + ";";
                                    string l13 = "goto " + eti1 + ";";
                                    string l14 = eti4 + ":";
                                    c3d.addCodigo(l1);
                                    c3d.addCodigo(l2);
                                    c3d.addCodigo(l3);
                                    c3d.addCodigo(l15);
                                    c3d.addCodigo(l4);
                                    c3d.addCodigo(l5);
                                    c3d.addCodigo(l6);
                                    c3d.addCodigo(l7);
                                    c3d.addCodigo(l8);
                                    c3d.addCodigo(l9);
                                    c3d.addCodigo(l10);
                                    c3d.addCodigo(l11);
                                    c3d.addCodigo(l12);
                                    c3d.addCodigo(l13);
                                    c3d.addCodigo(l14);
                                    return temp3;
                                }
                                else
                                {
                                    //es un objeto
                                }

                            }
                            else
                            {
                                //la variable no existe

                            }
                        }
                        return "nulo";
                    }

                #endregion

                #region operaciones Aritmeticas

                case Constantes.suma:
                    {

                        object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " + " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.resta:
                    {

                        object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " - " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " * " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.division:
                    {

                        object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " / " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.potencia:
                    {

                        object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " ^ " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }



                #endregion
                /*
                #region operaciones relacionales 

                case Constantes.mayor:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " > " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.menor:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " < " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.mayorIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " >= " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.menorIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " <= " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.igualIgual:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " == " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }

                case Constantes.distintoA:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " != " + val2 + ") goto " + etV + "\n goto " + etF;
                        //c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;
                    }



                #endregion
                    */
                #region operaciones logicas

                case Constantes.andJava:
                    {
                        Object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        Object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        if (val1 is nodoCondicion)
                        {
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion oper1 = (nodoCondicion)val1;
                                nodoCondicion oper2 = (nodoCondicion)val2;
                                string codigoAnd = oper1.codigo + "\n" +
                                                  oper1.getEtiquetasVerdaderas() + "\n" +
                                                  oper2.codigo + "\n";
                                nodoCondicion resultadoAnd = new nodoCondicion(codigoAnd);
                                resultadoAnd.addEtiquetasVerdaderas(oper2.etiquetasVerdaderas);
                                resultadoAnd.addEtiquetasFalsas(oper1.etiquetasFalsas);
                                resultadoAnd.addEtiquetasFalsas(oper2.etiquetasFalsas);
                                return resultadoAnd;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "El segundo operando para la operacion logica and debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                                Form1.errores.addError(n);
                                return "nulo";

                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "El primer operando para la operacion logica and debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                            Form1.errores.addError(n);
                            return "nulo";

                        }
                    }

                case Constantes.orJava:
                    {
                        Object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        Object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        if (val1 is nodoCondicion)
                        {
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion oper1 = (nodoCondicion)val1;
                                nodoCondicion oper2 = (nodoCondicion)val2;
                                string codigoOr = oper1.codigo + "\n" +
                                                  oper1.getEtiquetasFalsas() + "\n" +
                                                  oper2.codigo + "\n";
                                nodoCondicion resultadoOr = new nodoCondicion(codigoOr);
                                resultadoOr.addEtiquetasVerdaderas(oper1.etiquetasVerdaderas);
                                resultadoOr.addEtiquetasVerdaderas(oper2.etiquetasVerdaderas);
                                resultadoOr.addEtiquetasFalsas(oper2.etiquetasFalsas);
                                return resultadoOr;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "El segundo operando para la operacion logica or debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                                Form1.errores.addError(n);
                                return "nulo";

                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "El primer operando para la operacion logica or debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                            Form1.errores.addError(n);
                            return "nulo";

                        }

                    }

                case Constantes.xorJava:
                    {
                        Object val1 = resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                        Object val2 = resolverExpresiones(nodo.ChildNodes[1], ambiente, nombreClase, nommbreMetodo);
                        if (val1 is nodoCondicion)
                        {
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion oper1 = (nodoCondicion)val1;
                                nodoCondicion oper2 = (nodoCondicion)val2;

                                string codigoOr = oper1.codigo + "\n" +
                                                  oper1.getEtiquetasFalsas() + "\n" +
                                                  oper2.codigo + "\n";
                                nodoCondicion resultadoOr = new nodoCondicion(codigoOr);
                                resultadoOr.addEtiquetasVerdaderas(oper1.etiquetasVerdaderas);
                                resultadoOr.addEtiquetasVerdaderas(oper2.etiquetasVerdaderas);
                                resultadoOr.addEtiquetasFalsas(oper2.etiquetasFalsas);
                                return resultadoOr;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "El segundo operando para la operacion logica xor debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                                Form1.errores.addError(n);
                                return "nulo";

                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "El primer operando para la operacion logica xor debe ser una operacion relacional", nodo.ChildNodes[0].FindToken());
                            Form1.errores.addError(n);
                            return "nulo";

                        }

                    }

                #endregion

                #region instancias
                case Constantes.instancia:
                    {
                        /*INSTANCIA.Rule = Constantes.nuevo + identificador + "(" + LEXPRESIONES + ")"
                | Constantes.nuevo + identificador + "(" + ")";*/


                        #region instancias

                        //es una instancia

                        string tempRetorno = "nulo";
                        ParseTreeNode nodoInstancia = nodo;
                        string tipoInstacia2 = nodoInstancia.ChildNodes[0].Token.ValueString;


                        int sizeObjeto = tablaSimbolos.sizeClase(tipoInstacia2);
                        if (sizeObjeto != -1)
                        {//objeto a instanciar si existe
                            if (nodoInstancia.ChildNodes.Count == 2)
                            {//posee parametros
                                int posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObj, ambiente);
                                if (posObjInstanciar != -1)
                                {
                                    //resolvemos el tipo de parametros
                                    string cad = "";
                                    for (int i = 0; i < nodoInstancia.ChildNodes[1].ChildNodes.Count; i++)
                                    {
                                        cad += validarTipo(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambiente,nombreClase,nommbreMetodo);//resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                                    }

                                    string firmaMetodo = tablaSimbolos.getFirmaMetodo(nombreClase, cad, nommbreMetodo);
                                    if (!firmaMetodo.Equals(""))
                                    {
                                        //1. Crear apuntador en el stack de heap para el nuevo objeto
                                        string temp1 = c3d.getTemporal();
                                        string l1 = temp1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj;
                                        tempRetorno = c3d.getTemporal();
                                        string ltemporal = tempRetorno + " = H; ";
                                        string l2 = "STACK[" + temp1 + "] = " + tempRetorno + "; //apuntador del heap en el stack del nuevo obj";

                                        //2. Reservo espacio en el heap para el nuevo objeto
                                        string l3 = "H = H + " + sizeObjeto + "; //reservar espacio para atributos";
                                        c3d.addCodigo(l1);
                                        c3d.addCodigo(ltemporal);
                                        c3d.addCodigo(l2);
                                        c3d.addCodigo(l3);

                                        //3. Verifico si atributos posee asignacion ej publico a = 4;
                                        List<Simbolo> atributosClase = tablaSimbolos.obtenerAtributosClase(tipoInstacia2);
                                        foreach (Simbolo item in atributosClase)
                                        {
                                            if (item.expresionAtributo != null)
                                            {//genero el codigo3d para el atributo
                                                Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambiente,nombreClase,nommbreMetodo);
                                                if (tipoExpAtributo.ToString().Equals(item.tipo, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    string temp1_1 = c3d.getTemporal();
                                                    c3d.addCodigo(temp1_1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj);
                                                    string temp2_1 = c3d.getTemporal();
                                                    c3d.addCodigo(temp2_1 + " = STACK[" + temp1_1 + "];//apuntador del this en el heap");
                                                    string temp3_1 = c3d.getTemporal();
                                                    c3d.addCodigo(temp3_1 + " = " + temp2_1 + " + " + item.apuntador + ";//this + pos de " + item.nombreReal);

                                                    Object val = resolverExpresiones(item.expresionAtributo, ambiente, nombreClase, nommbreMetodo);
                                                    c3d.addCodigo("HEAP[" + temp3_1 + "] = " + val.ToString() + ";");
                                                }
                                                else
                                                {
                                                    ErrorA er = new ErrorA(Constantes.errorSemantico, "Operacion no valida para el tipo de " + item.tipo + ", " + tipoExpAtributo, item.expresionAtributo.FindToken());
                                                    Form1.errores.addError(er);
                                                }
                                            }
                                        }
                                        string temp2 = c3d.getTemporal();
                                        string l4 = temp2 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj;
                                        string temp3 = c3d.getTemporal();
                                        string l5 = temp3 + " = STACK[" + temp2 + "]; //pos this del apuntador del heap";
                                        int tamanhoFuncActual = tablaSimbolos.sizeFuncion(nombreClase, nommbreMetodo);
                                        string temp4 = c3d.getTemporal();
                                        string l6 = temp4 + " = P + " + tamanhoFuncActual + "; //p+ size funcion actual";
                                        string temp5 = c3d.getTemporal();
                                        string l7 = temp5 + " = " + temp4 + " + 0; //pos this en el constructor";
                                        string l8 = "STACK[" + temp5 + "] = " + temp3 + "; //guardo en el this del constructor el ap del heap";
                                        // Asignar Parametros
                                        c3d.addCodigo(l4);
                                        c3d.addCodigo(l5);
                                        c3d.addCodigo(l6);
                                        c3d.addCodigo(l7);
                                        c3d.addCodigo(l8);
                                        ParseTreeNode nodoParametros = nodoInstancia.ChildNodes[1];
                                        int cont = 1;
                                        foreach (ParseTreeNode item in nodoParametros.ChildNodes)
                                        {
                                            Object val = resolverExpresiones(item, ambiente, nombreClase, nommbreMetodo);
                                            string temp1_1 = c3d.getTemporal();
                                            string l1_1 = temp1_1 + " = P + " + tamanhoFuncActual + "; // p + size func actual";
                                            c3d.addCodigo(l1_1);
                                            string temp2_1 = c3d.getTemporal();
                                            l1_1 = temp2_1 + " = " + temp1_1 + " + " + cont + "; //pos del parametro";
                                            c3d.addCodigo(l1_1);
                                            l1_1 = "STACK[" + temp2_1 + "] = " + val + "; //paso por valor del parametro";
                                            c3d.addCodigo(l1_1);
                                            cont++;
                                        }

                                        string l9 = "P = P + " + tamanhoFuncActual + ";";
                                        string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, cad, tipoInstacia2);
                                        string l10 = "call " + firmaLlamada + "();";
                                        string l11 = "P = P - " + tamanhoFuncActual + ";";

                                        c3d.addCodigo(l9);
                                        c3d.addCodigo(l10);
                                        c3d.addCodigo(l11);
                                    }
                                    else
                                    {
                                        //no existe un constructor con esos parametros
                                    }


                                }

                                else
                                {
                                    posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObj, ambiente);
                                    if (posObjInstanciar != -1)
                                    {
                                        Console.WriteLine("tengo que instanciar un atributo");
                                        //es un atributo al cual vamos a instanciar
                                    }
                                    else
                                    {
                                        ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + nombreObj + ", no existe.", nodo.FindToken());
                                        Form1.errores.addError(nue);

                                    }

                                }
                            }
                            else
                            {//no tiene parametros
                                int posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObj, ambiente);
                                if (posObjInstanciar != -1)
                                {//si existe y es una vairable local
                                    //1. Crear apuntador en el stack de heap para el nuevo objeto
                                    string temp1 = c3d.getTemporal();
                                    string l1 = temp1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj;
                                    string l2 = "STACK[" + temp1 + "] = H; //apuntador del heap en el stack del nuevo obj";

                                    //2. Reservo espacio en el heap para el nuevo objeto
                                    string l3 = "H = H + " + sizeObjeto + "; //reservar espacio para atributos";
                                    c3d.addCodigo(l1);
                                    c3d.addCodigo(l2);
                                    c3d.addCodigo(l3);

                                    //3. Verifico si atributos posee asignacion ej publico a = 4;
                                    List<Simbolo> atributosClase = tablaSimbolos.obtenerAtributosClase(tipoInstacia2);
                                    foreach (Simbolo item in atributosClase)
                                    {
                                        if (item.expresionAtributo != null)
                                        {//genero el codigo3d para el atributo
                                            Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambiente,nombreClase,nommbreMetodo);
                                            if (tipoExpAtributo.ToString().Equals(item.tipo, StringComparison.OrdinalIgnoreCase))
                                            {
                                                string temp1_1 = c3d.getTemporal();
                                                c3d.addCodigo(temp1_1 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj);
                                                string temp2_1 = c3d.getTemporal();
                                                c3d.addCodigo(temp2_1 + " = STACK[" + temp1_1 + "];//apuntador del this en el heap");
                                                string temp3_1 = c3d.getTemporal();
                                                c3d.addCodigo(temp3_1 + " = " + temp2_1 + " + " + item.apuntador + ";//this + pos de " + item.nombreReal);

                                                Object val = resolverExpresiones(item.expresionAtributo, ambiente, nombreClase, nommbreMetodo);
                                                c3d.addCodigo("HEAP[" + temp3_1 + "] = " + val.ToString() + ";");
                                            }
                                            else
                                            {
                                                ErrorA er = new ErrorA(Constantes.errorSemantico, "Operacion no valida para el tipo de " + item.tipo + ", " + tipoExpAtributo, item.expresionAtributo.FindToken());
                                                Form1.errores.addError(er);
                                            }
                                        }
                                    }
                                    string temp2 = c3d.getTemporal();
                                    string l4 = temp2 + " = P + " + posObjInstanciar + "; //pos de " + nombreObj;
                                    string temp3 = c3d.getTemporal();
                                    string l5 = temp3 + " = STACK[" + temp2 + "]; //pos this del apuntador del heap";
                                    int tamanhoFuncActual = tablaSimbolos.sizeFuncion(nombreClase, nommbreMetodo);
                                    string temp4 = c3d.getTemporal();
                                    string l6 = temp4 + " = P + " + tamanhoFuncActual + "; //p+ size funcion actual";
                                    string temp5 = c3d.getTemporal();
                                    string l7 = temp5 + " = " + temp4 + " + 0; //pos this en el constructor";
                                    string l8 = "STACK[" + temp5 + "] = " + temp3 + "; //guardo en el this del constructor el ap del heap";
                                    string l9 = "P = P + " + tamanhoFuncActual + ";";
                                    string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, "", tipoInstacia2);
                                    string l10 = "call " + firmaLlamada + "();";
                                    string l11 = "P = P - " + tamanhoFuncActual + ";";
                                    c3d.addCodigo(l4);
                                    c3d.addCodigo(l5);
                                    c3d.addCodigo(l6);
                                    c3d.addCodigo(l7);
                                    c3d.addCodigo(l8);
                                    c3d.addCodigo(l9);
                                    c3d.addCodigo(l10);
                                    c3d.addCodigo(l11);
                                }
                                else
                                {
                                    posObjInstanciar = tablaSimbolos.getPosicionDeClase(nombreObj, ambiente);
                                    if (posObjInstanciar != -1)
                                    {
                                        //es un atributo al cual vamos a instanciar
                                    }
                                    else
                                    {
                                        ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + nombreObj + ", no existe.", nodo.FindToken());
                                        Form1.errores.addError(nue);

                                    }
                                }


                            }


                        }
                        else
                        {//objeto a instaincair no existe
                            ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " + tipoInstacia2 + ", no existe.", nodo.FindToken());
                            Form1.errores.addError(nue);
                        }



                        #endregion
                        return tempRetorno;

                    }

                #endregion


                #region llamadaFuncion

                case Constantes.llamada:
                    {
                        return llamadaFuncion(nodo, nombreClase, nommbreMetodo, ambiente);
                    }


                #endregion



            }
            return "nulo";

        }




    }
}
