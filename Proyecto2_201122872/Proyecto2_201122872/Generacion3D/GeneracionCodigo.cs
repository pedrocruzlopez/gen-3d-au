using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using Proyecto2_201122872.UML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.AnalizadorPython;
using System.IO;
using Proyecto2_201122872.Errores;


namespace Proyecto2_201122872.Generacion3D
{
    public class GeneracionCodigo
    {

        public tablaDeSimbolos tablaSimbolos;
        public clasesDiagrama uml;
        private Arbol analizadorJava;
        private ArbolPy analizadorPython;
        
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
               clasesDiagrama uml2= analizadorJava.parseConvertirUML2(contenido);
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
                    escribirConstructores(constructores, ambito,claseActual.getNombre());

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
                    c3d.addCodigo("Principal  "+principal.firma + "(){");
                    if (principal.cuerpo != null)
                        evaluarCuerpo(principal.cuerpo, ambito, principal.firma, claseActual.getNombre());
                    c3d.addCodigo("}");
                    ambito.ambitos.Pop();
                }

                //3. escribir los demas metodos y funciones

                funcionesMetodos = claseActual.getFunciones();
                escribirConstructores(funcionesMetodos, ambito,claseActual.getNombre());
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


                   case Constantes.asignacion:{
                       /*
                       int noHijos1 = nodo.ChildNodes[0].ChildNodes.Count;
                       int noHijos2 = nodo.ChildNodes[1].ChildNodes.Count;
                       if (noHijos1 == 1)
                       {//debe ser una variable o un arreglo
                           ParseTreeNode nodoVar= nodo.ChildNodes[0]; 
                           if (string.Equals(nodoVar.ChildNodes[0].Term.Name, Constantes.id, StringComparison.OrdinalIgnoreCase))
                           {
                               string nombre = nodoVar.ChildNodes[0].Token.ValueString;
                               int pos = tablaSimbolos.getPosicion(nombre, ambitos);
                               string tipo = tablaSimbolos.getTipo(nombre, ambitos);
                               if (pos == -1)
                               {//buscar si es una variable de clase
                                   pos = tablaSimbolos.getPosicionDeClase(nombre, ambitos);
                                   tipo = tablaSimbolos.getTipo(nombre, ambitos);
                                   if (pos == -1)
                                   {
                                       //vairable no existe
                                   }
                                   else
                                   {

                                   }
                               }
                               else
                               {

                               }
                           }
                           else if (string.Equals(nodoVar.ChildNodes[0].Term.Name, Constantes.declaArreglo, StringComparison.OrdinalIgnoreCase))
                           {

                           }
                           else
                           {
                               //error
                           }

                       }
                       else { }

                       */
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
                           /* DECLARACION.Rule = TIPO + identificador + ToTerm(";")
                | TIPO + identificador + ToTerm("=") + EXPRESION + ";"
                | TIPO + identificador + LPOSICIONES + ToTerm(";")
                | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
                | TIPO + identificador + ToTerm("=") + INSTANCIA + ";"; */
                           
                           int noHijos = nodo.ChildNodes.Count;
                           if (noHijos == 3)
                           {
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
                                               int posObjInstanciar = tablaSimbolos.getPosicion(nombreObjetoInstanciar,ambitos);
                                               if (posObjInstanciar != -1)
                                               {
                                                   //resolvemos el tipo de parametros
                                                   string cad = "";
                                                   for (int i = 0; i < nodoInstancia.ChildNodes[1].ChildNodes.Count; i++)
                                                   {
                                                       cad += validarTipo(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos);//resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
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
                                                               Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambitos);
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
                                                       // Asignar Parametros
                                                       c3d.addCodigo(l4);
                                                       c3d.addCodigo(l5);
                                                       c3d.addCodigo(l6);
                                                       c3d.addCodigo(l7);
                                                       c3d.addCodigo(l8);
                                                       ParseTreeNode nodoParametros = nodoInstancia.ChildNodes[1];
                                                       int cont=1;
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
                                                       string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, "", tipoInstacia2);
                                                       string l10 = "call " + firmaLlamada + "();";
                                                       string l11 = "P = P - " + tamanhoFuncActual + ";";
                                                       
                                                       c3d.addCodigo(l9);
                                                       c3d.addCodigo(l10);
                                                       c3d.addCodigo(l11);
                                                       
                                                       
                                                       
                                                       /* string temp1 = c3d.getTemporal();
                                                       string l1 = temp1 + "= P + " + posObjInstanciar + "; //pos del obj " + nombreObjetoInstanciar;
                                                       string temp2 = c3d.getTemporal();
                                                       string l2 = temp2 + " = STACK[" + temp1 + "];";
                                                       string temp3 = c3d.getTemporal();
                                                       int tamanhoFuncActual = tablaSimbolos.sizeFuncion(nombreClase, nombreMetodo);
                                                       string l3 = temp3 + " = P + " + tamanhoFuncActual + ";";
                                                       int posThis = tablaSimbolos.getPosicion("this", ambitos);
                                                       string firmaLlamada = tablaSimbolos.getFirmaMetodo(nombreClase, cad, tipoInstacia2);
                                                       string temp4 = c3d.getTemporal();
                                                       string l4 = temp4 + " = " + temp3 + " + " + posThis + "; //posicion del this";
                                                       string l5 = "STACK[" + temp4 + "] = " + temp2 + ";";
                                                       String lParametros = "";

                                                       for (int i = 0; i < nodoInstancia.ChildNodes[1].ChildNodes.Count; i++)
                                                       {
                                                           Object temp = resolverExpresiones(nodoInstancia.ChildNodes[1].ChildNodes[i].ChildNodes[0], ambitos, nombreClase, nombreMetodo);
                                                           String tempT = c3d.getTemporal();
                                                           string lTemp = tempT + " = P + " + tamanhoFuncActual + ";\n";
                                                           string tempT2 = c3d.getTemporal();
                                                           int val = 1 + i;
                                                           string lTemp2 = tempT2 + " = " + tempT + " + " +val + ";\n";
                                                           string lTemp3 = "STACK[" + tempT2 + "] = " + temp + "; // asignamos al parametro\n";
                                                           lParametros += lTemp;
                                                           lParametros += lTemp2;
                                                           lParametros += lTemp3;
                                                       }


                                                       string l6 = "P = P + " + tamanhoFuncActual + ";";
                                                       string l7 = "call " + firmaLlamada + "();";
                                                       string l8 = "P = P - " + tamanhoFuncActual + ";";
                                                       c3d.addCodigo(l1);
                                                       c3d.addCodigo(l2);
                                                       c3d.addCodigo(l3);
                                                       c3d.addCodigo(l4);
                                                       c3d.addCodigo(l5);
                                                       c3d.addCodigo(lParametros);
                                                       c3d.addCodigo(l6);
                                                       c3d.addCodigo(l7);
                                                       c3d.addCodigo(l8);*/
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
                                               int posObjInstanciar = tablaSimbolos.getPosicion(nombreObjetoInstanciar,ambitos);
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
                                                           Object tipoExpAtributo = validarTipo(item.expresionAtributo, ambitos);
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
                                                   string l8 = "STACK["+temp5+"] = "+temp3+"; //guardo en el this del constructor el ap del heap";
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
                                                       ErrorA nue = new ErrorA(Constantes.errorSemantico, "El objeto " +nombreObjetoInstanciar+ ", no existe.", nodo.FindToken());
                                                       Form1.errores.addError(nue);

                                                   }
                                               }

                                              
                                           }


                                       }
                                       else
                                       {//objeto a instaincair no existe
                                           ErrorA nue = new ErrorA(Constantes.errorSemantico,"El objeto "+ tipoInstancia + ", no existe.", nodo.FindToken());
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


                               #region instancias
                              
                               
                                
                               

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
                                           Object tipoExp = validarTipo(nodoExpresion, ambitos);
                                           if (tipoExp.ToString().Equals(tipo, StringComparison.OrdinalIgnoreCase))
                                           {
                                               Object val = resolverExpresiones(nodoExpresion, ambitos, nombreClase, nombreMetodo);
                                               string l2 = "STACK[" + temp1 + "] = " + val.ToString() + ";";

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
                                   else if(noAmbiente==1)
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
                   case "":
                       {
                           /* DECLARACION.Rule = TIPO + identificador + ToTerm(";")
                           | TIPO + identificador + ToTerm("=") + EXPRESION + ";"
                           | TIPO + identificador + LPOSICIONES + ToTerm(";")
                           | TIPO + identificador + LPOSICIONES + ToTerm("=") + "{" + LFILAS + "}" + ";"
                           | TIPO + identificador + ToTerm("=") + INSTANCIA + ";"; */

                           int noHijos = nodo.ChildNodes.Count;

                           if (noHijos == 3)
                           {
                               if (nodo.ChildNodes[2].Term.Name.Equals(Constantes.instancia, StringComparison.OrdinalIgnoreCase))
                               {
                                   String nombreObjetoInstanciar = nodo.ChildNodes[1].Token.ValueString;
                                   string tipoInstancia = nodo.ChildNodes[0].Token.ValueString;
                                   ParseTreeNode nodoInstancia = nodo.ChildNodes[2];
                                   string tipoInstacia2 = nodoInstancia.ChildNodes[0].Token.ValueString;

                                   if (tipoInstancia.Equals(tipoInstacia2, StringComparison.OrdinalIgnoreCase))
                                   {// si son del mismo tipo al que quiero instancia
                                       int hijosInstancia = nodoInstancia.ChildNodes.Count;
                                       if (hijosInstancia == 2)
                                       {// si posee parametros
                                           ParseTreeNode nodoParametros = nodoInstancia.ChildNodes[1];
                                           int noParametros = nodoParametros.ChildNodes.Count;
                                           List<List<String>> tipoParametros = tablaSimbolos.existeConstructor(tipoInstacia2, noParametros);
                                           if (tipoParametros.Count>0)
                                           {
                                               List<String> parametrosConstructor = new List<string>();
                                               for (int i = 0; i < nodoParametros.ChildNodes.Count; i++)
                                               {
                                                   Object tipoParametro = validarTipo(nodoParametros.ChildNodes[i], ambitos);
                                                   parametrosConstructor.Add(tipoParametro.ToString());
                                               }
                                               bool resParametros = sonNulos(parametrosConstructor);
                                               if (resParametros)
                                               {
                                                   bool bandera;
                                                   int cont = 0;
                                                   
                                                   List<String> lTemporal;
                                                   for (int i = 0; i < tipoParametros.Count; i++)
                                                   {
                                                       bandera = true;
                                                       lTemporal = tipoParametros.ElementAt(i);
                                                       if (lTemporal.Count == parametrosConstructor.Count)
                                                       {
                                                           for (int j = 0; j < lTemporal.Count; j++)
                                                           {
                                                               bandera = bandera && lTemporal.ElementAt(j).Equals(parametrosConstructor.ElementAt(j), StringComparison.OrdinalIgnoreCase);

                                                           }
                                                           if (bandera)
                                                           {
                                                               cont++;
                                                           }
                                                       }
                                                   }

                                                   if (cont > 0)
                                                   {
                                                       //si existe
                                                       string cad = "";
                                                       foreach (String item in parametrosConstructor)
                                                       {
                                                           cad += item.ToUpper();
                                                       }

                                                       

                                                   }
                                                   else
                                                   {
                                                       //no existe un constructor con esos parametros
                                                   }
                                               }
                                               else
                                               {
                                                   //error los parametros devuelven algun nulo
                                               }

                                           }
                                          /* {//si exite un constructor con ese numero de parametros
                                               List<String> parametrosConstructor = new List<string>();
                                               for (int i = 0; i < nodoParametros.ChildNodes.Count; i++)
                                               {
                                                   Object tipoParametro = validarTipo(nodoParametros.ChildNodes[i], ambitos);
                                                   parametrosConstructor.Add(tipoParametro.ToString());
                                               }
                                               bool resParametros = sonNulos(tipoParametros);
                                               if (resParametros)
                                               {//sus parametros no devuelven algun nulo

                                                   if (parametrosConstructor.Count == tipoParametros.Count)
                                                   {
                                                       bool bandera = true;
                                                       for (int i = 0; i < tipoParametros.Count; i++)
                                                       {
                                                           bandera = bandera && tipoParametros.ElementAt(i).Equals(parametrosConstructor.ElementAt(i), StringComparison.OrdinalIgnoreCase);
                                                       }
                                                       if (bandera)
                                                       {

                                                       }
                                                       else
                                                       {

                                                       }

                                                   }

                                               }
                                               else
                                               {
                                                   //error los parametros devuelven algun nulo
                                               }

                                           }*/
                                           else
                                           {
                                               //error, no exite un constructor cone se nuermer de parametros
                                           }
                                          
                                           
                                           
                                           
                                           
                                          


                                       }
                                       else
                                       {//no posee parametros



                                       }
                                           
                                   }
                                   else
                                   {
                                       //error el elemento que declara no es del mismo tipo al que quier instacia
                                   }


                                   //es un instancia
                                   int noHijosInst = nodo.ChildNodes[2].ChildNodes.Count;
                                   string objetoInstanciar = nodo.ChildNodes[0].Token.ValueString;
                                   //1. verificamos si existe el objeto que queresmos instanciar
                                   int sizeObjeto = tablaSimbolos.sizeClase(objetoInstanciar);
                                   if (sizeObjeto != -1)
                                   {//objeto a instanciar si existe



                                   }
                                   else
                                   {//objeto a instaincair no existe

                                   }


                               }
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
                           c3d.addCodigo(etiqCiclo+":");
                           ambitos.addWhile();
                           Object g = evaluarExp(nodo.ChildNodes[0].ChildNodes[0]);
                           if (g is nodoCondicion)
                           {
                               nodoCondicion condWhile = (nodoCondicion)g;
                               evaluarCuerpo(nodo.ChildNodes[1], ambitos,nombreMetodo,nombreClase);
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

                   case Constantes.llamada:
                       {
                           break;
                       }







            }



        }



        #endregion



        /* --- Generaacion de codigo  Expresiones  -------*/





        #region resolverExpresiones 

        private Object resolverExpresion(ParseTreeNode nodo, Ambitos ambiente)
        {

            switch (nodo.Term.Name)
            {
                
                
                case Constantes.llamada:
                    {
                        int noHijos = nodo.ChildNodes.Count;
                        string nombreFunc = nodo.ChildNodes[0].Token.ValueString;
                        if (noHijos == 1)
                        {//no posee parametros

                        }
                        else
                        {//si posee parametros


                        }



                        return null;
                    }

                case Constantes.instancia:
                    {
                        /*INSTANCIA.Rule = Constantes.nuevo + identificador + "(" + LEXPRESIONES + ")"
                             | Constantes.nuevo + identificador + "(" + ")";*/




                        return null;
                    }

            }
            return null;
        }

           

        #endregion

















        #region generacion expresion c3d

        #region evaluar int
        public object evaluarInt(ParseTreeNode nodo, Ambitos ambitos)
        {
            switch (nodo.Term.Name.ToString())
            {

                #region 
                case Constantes.expresion:
                    {
                        Console.WriteLine("entre a una exp");
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region id

                case Constantes.id:
                    {
                        string nombre = nodo.ChildNodes[0].Token.ValueString;
                        int pos = tablaSimbolos.getPosicion(nombre, ambitos);
                        string tipo = tablaSimbolos.getTipo(nombre, ambitos);
                        if (pos == -1)
                        {
                            pos = tablaSimbolos.getPosicionDeClase(nombre, ambitos);
                            if (pos == -1)
                            {
                                ErrorA err = new ErrorA("semantico", "No existe la variable " + nombre, nodo.FindToken());
                                Form1.errores.addError(err);
                            }
                            else
                            {
                              
                                string temp1 = c3d.getTemporal();
                                string temp2 = c3d.getTemporal();
                                string temp3 = c3d.getTemporal();
                                string temp4 = c3d.getTemporal();

                                c3d.addCodigo(temp1 + " = P + 0; //pos this");
                                c3d.addCodigo(temp2 + " = STACK[ " + temp1 + " ];");
                                c3d.addCodigo(temp3 + " = " + temp2 + " + " + pos + ";");
                                c3d.addCodigo(temp4 + " = HEAP[ " + temp3 + " ];");
                                return temp4;

                            }
                        }
                        else
                        {
                            if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo( temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre );
                                return temp2;
                            }
                            else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else if (string.Equals(tipo, Constantes.tipoBool, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else
                            {
                                ErrorA er= new ErrorA(Constantes.errorSemantico, "Error, tipo no valido para asignacion int, "+ tipo, nodo.FindToken());
                                Form1.errores.addError(er);
                                
                            }

                        }

                        return "nulo";

                    }
                #endregion




                #region valores primitivos

                case Constantes.tipoEntero:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }

                case Constantes.tipoDecimal:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo entero, no es valido un double", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;

                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case Constantes.tipoCadena:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo entero, no es valido una cadena", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                #endregion
                    


                #region operaciones
                case Constantes.suma:
                    {
                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " + " + val2 + ";");
                        return temp;
                    }
                case Constantes.resta:
                    {
                        
                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " - " + val2 + ";");
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " * " + val2 + ";");
                        return temp;
                    }


                case Constantes.potencia:
                    {

                        object val1 = evaluarInt(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarInt(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " ^ " + val2 + ";");
                        return temp;
                    }

                case Constantes.division:
                    {
                        ErrorA er = new ErrorA(Constantes.errorSemantico, "Division no valida para un tipo int", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                #endregion


                #region unarios

                case Constantes.negativo:
                    {
                        Object resultado = evaluarInt(nodo.ChildNodes[0], ambitos);
                        string et = c3d.getTemporal();
                        c3d.addCodigo(et + " = " + resultado + " * -1;");
                        return et;
                    }

                case Constantes.masmas://tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo( temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }
                    }

                case Constantes.menosmenos: //tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarInt(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }


                    }

                #endregion


            }


            return "nulo";
        }


#endregion

        #region evaluar double

        public object evaluarDouble(ParseTreeNode nodo, Ambitos ambitos)
        {
            switch (nodo.Term.Name.ToString())
            {

                #region
                case Constantes.expresion:
                    {
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region id

                case Constantes.id:
                    {
                        string nombre = nodo.ChildNodes[0].Token.ValueString;
                        int pos = tablaSimbolos.getPosicion(nombre, ambitos);
                        string tipo = tablaSimbolos.getTipo(nombre, ambitos);
                        if (pos == -1)
                        {
                            pos = tablaSimbolos.getPosicionDeClase(nombre, ambitos);
                            if (pos == -1)
                            {
                                ErrorA err = new ErrorA("semantico", "No existe la variable " + nombre, nodo.FindToken());
                                Form1.errores.addError(err);
                            }
                            else
                            {

                                string temp1 = c3d.getTemporal();
                                string temp2 = c3d.getTemporal();
                                string temp3 = c3d.getTemporal();
                                string temp4 = c3d.getTemporal();

                                c3d.addCodigo(temp1 + " = P + 0; //pos this");
                                c3d.addCodigo(temp2 + " = STACK[ " + temp1 + " ];");
                                c3d.addCodigo(temp3 + " = " + temp2 + " + " + pos + ";");
                                c3d.addCodigo(temp4 + " = HEAP[ " + temp3 + " ];");
                                return temp4;

                            }
                        }
                        else
                        {
                            if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;
                            }
                            else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else if (string.Equals(tipo, Constantes.tipoBool, StringComparison.OrdinalIgnoreCase))
                            {
                                String temp1 = c3d.getTemporal();
                                String temp2 = c3d.getTemporal();
                                c3d.addCodigo(temp1 + " = P + " + pos + ";");
                                c3d.addCodigo(temp2 + " = STACK[" + temp1 + "];     // valor de " + nombre);
                                return temp2;

                            }
                            else
                            {
                                ErrorA er = new ErrorA(Constantes.errorSemantico, "Error, tipo no valido para asignacion int, " + tipo, nodo.FindToken());
                                Form1.errores.addError(er);

                            }

                        }

                        return "nulo";

                    }
                #endregion




                #region valores primitivos

                case Constantes.tipoEntero:
                    {
                        return int.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }

                case Constantes.tipoDecimal:
                    {
                        return double.Parse(nodo.ChildNodes[0].Token.ValueString);
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;

                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                case Constantes.tipoCadena:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo entero, no es valido una cadena", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                #endregion



#region operaciones
                case Constantes.suma:
                    {
                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " + " + val2 + ";");
                        return temp;
                    }
                case Constantes.resta:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " - " + val2 + ";");
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " * " + val2 + ";");
                        return temp;
                    }


                case Constantes.potencia:
                    {

                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " ^ " + val2 + ";");
                        return temp;
                    }

                case Constantes.division:
                    {
                        object val1 = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        object val2 = evaluarDouble(nodo.ChildNodes[1], ambitos);
                        string temp = c3d.getTemporal();
                        c3d.addCodigo(temp + " = " + val1 + " / " + val2 + ";");
                        return temp;
                    }


                #endregion
                

                #region unarios

                case Constantes.negativo:
                    {
                        Object resultado = evaluarDouble(nodo.ChildNodes[0], ambitos);
                        string et = c3d.getTemporal();
                        c3d.addCodigo(et + " = " + resultado + " * -1;");
                        return et;
                    }

                case Constantes.masmas://tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " + 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }
                    }

                case Constantes.menosmenos: //tree
                    {
                        string tipo = nodo.ChildNodes[0].Term.Name;
                        if (string.Equals(tipo, Constantes.tipoEntero, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else if (string.Equals(tipo, Constantes.tipoChar, StringComparison.OrdinalIgnoreCase))
                        {
                            object uno = evaluarDouble(nodo.ChildNodes[0], ambitos);
                            String temp = c3d.getTemporal();
                            c3d.addCodigo(temp + " = " + uno + " - 1;");
                            return temp;

                        }
                        else
                        {
                            ErrorA er = new ErrorA(Constantes.errorSemantico, "Tipo " + tipo + ", no valido para un unario en tree", nodo.FindToken());
                            Form1.errores.addError(er);
                            return "nulo";
                        }


                    }

                #endregion


            }


            return "nulo";
        }


        #endregion


        #region evaluar bool


        public object evaluarBool(ParseTreeNode nodo, Ambitos ambitos)
        {

            switch(nodo.Term.Name)
            {


                #region
                case Constantes.expresion:
                    {
                        Console.WriteLine("entre a una exp");
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region valoresPrimitivos

                case Constantes.tipoEntero:
                    {
                        string valor = nodo.ChildNodes[0].Token.ValueString;
                        if (valor.Equals("1"))
                        {
                            return valor;
                        }
                        else if(valor.Equals("0"))
                        {
                            return valor;
                        }
                        else
                        {
                            return "nulo";
                        }

                    }

                case Constantes.tipoCadena:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una cadena", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                    }


                case Constantes.tipoChar:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una char", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.tipoDecimal:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un decimal", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;

                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }

               





                #endregion


                #region operaciones
                case Constantes.suma:
                    {
                        object uno = evaluarBool(nodo.ChildNodes[0],ambitos);
                        object dos = evaluarBool(nodo.ChildNodes[1],ambitos);
                        String temp = c3d.getTemporal();
                        if (uno.ToString().Equals("0") && dos.ToString().Equals("0"))
                        {
                            c3d.addCodigo(temp + " = 0;");
                            return temp;
                        }
                        else
                        {
                            c3d.addCodigo(temp + " = 1;");
                            return temp;
                        }
                    }
                case Constantes.resta:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una resta", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                       
                    }

                case Constantes.multiplicacion:
                    {
                        object uno = evaluarBool(nodo.ChildNodes[0], ambitos);
                        object dos = evaluarBool(nodo.ChildNodes[1], ambitos);
                        String temp = c3d.getTemporal();
                        if (uno.ToString().Equals("1") && dos.ToString().Equals("1"))
                        {
                            c3d.addCodigo(temp + " = 1;");
                            return temp;
                        }
                        else
                        {
                            c3d.addCodigo(temp + " = 0;");
                            return temp;
                        }
                    }


                case Constantes.potencia:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una potencia", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                        
                    }

                case Constantes.division:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido una division", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";

                    }


                #endregion


                #region unarios

                case Constantes.negativo:
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un negativo", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.masmas://tree
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un unario", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";
                    }

                case Constantes.menosmenos: //tree
                    {
                        ErrorA er = new ErrorA("Semantico", "Asignacion de tipo booleano, no es valido un unario", nodo.FindToken());
                        Form1.errores.addError(er);
                        return "nulo";


                    }

                #endregion



            }




            return null;
        }



        #endregion

        
        #region validaString

        private object evaluarCadena(ParseTreeNode nodo, Ambitos ambitos)
        {


            switch(nodo.Term.Name){


                #region
                case Constantes.expresion:
                    {
                        Console.WriteLine("entre a una exp");
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion


                case Constantes.tipoCadena:
                    {
                        string id = nodo.ChildNodes[0].Token.ValueString;
                        char caracter;
                        int ascii;

                        for (int i = 0; i < id.Length; i++)
                        {
                            caracter = id.ElementAt(i).ToString()[0];
                            ascii = (int)caracter;
                            c3d.addCodigo("HEAP[H] = " + ascii + ";");
                            c3d.addCodigo("H = H + 1;");
                        }
                        return "nulo";
                    }





        }



            return "nulo";
        }


        #endregion


        #region evaluar Condicion

        public object evaluarExp(ParseTreeNode nodo)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {


                #region expresion puntitos
                case Constantes.expresion:
                    {
                        foreach (ParseTreeNode exp in nodo.ChildNodes)
                        {
                            evaluarExp(exp);
                        }
                        break;

                    }

                #endregion

                #region llamadaFuncion
                case Constantes.llamada:
                    {
                        /*
                         * LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
                                | identificador + ToTerm("(") + ")";
                         */
                        int noHijos = nodo.ChildNodes.Count;
                        string nombreFuncion = nodo.ChildNodes[0].Token.ValueString;
                        if (noHijos == 1) {
                            /* Pasos a seguir
                             * 1. buscamos si existe la funcion a la cual queremos llamar
                             * 
                             */


                        }
                        else
                        {//posee paramatros

                        }




                        return "nulo";

                    }



                #endregion


                #region id

                case Constantes.id:
                    {
                        string id = nodo.ChildNodes[0].Token.ValueString;
                        break;


                    }

                #endregion

                #region valores primitivos

                case Constantes.tipoEntero:
                    {
                        int val = int.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return val;
                    }

                case Constantes.tipoDecimal:
                    {
                        double val = double.Parse(nodo.ChildNodes[0].Token.ValueString);
                        return val;
                    }

                case Constantes.tipoChar:
                    {
                        char valor = char.Parse(nodo.ChildNodes[0].Token.ValueString);
                        char caracter = valor.ToString()[0];
                        int ascii = (int)caracter;
                        return ascii;
                    }


                case Constantes.tipoBool:
                    {
                        String val_bol = nodo.ChildNodes[0].Token.ValueString;
                        
                        if (string.Equals(val_bol, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            return 1;
                        }
                        else
                        {
                            return  0;
                        }
                    }


                #endregion


                #region operaciones 

                case Constantes.suma:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " + " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.resta:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " - " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.multiplicacion:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " * " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }


                case Constantes.division:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " / " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }

                case Constantes.potencia:
                    {

                        object val1 = evaluarExp(nodo.ChildNodes[0]);
                        object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string temp = c3d.getTemporal();
                        string cod = temp + " = " + val1 + " ^ " + val2 + ";";
                        c3d.addCodigo(cod);
                        return temp;
                    }



                #endregion


                #region relacionales

                case Constantes.mayor:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        Object val2 = evaluarExp(nodo.ChildNodes[1]);
                        string etV = c3d.getEtiqueta();
                        string etF = c3d.getEtiqueta();
                        string codigo = "if ( " + val1 + " > " + val2 + ") goto " + etV + "\n goto " + etF;
                        c3d.addCodigo(codigo);
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
                        c3d.addCodigo(codigo);
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
                        c3d.addCodigo(codigo);
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
                        c3d.addCodigo(codigo);
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
                        c3d.addCodigo(codigo);
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
                        c3d.addCodigo(codigo);
                        nodoCondicion cond = new nodoCondicion(codigo);
                        cond.addEtiquetaFalsa(etF);
                        cond.addEtiquetaVerdadera(etV);
                        return cond;

                    }




                #endregion


                

                #region logicas


                case Constantes.andJava:
                    {
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        if (val1 is nodoCondicion)
                        {
                            nodoCondicion retorno = new nodoCondicion("");
                            nodoCondicion cond1 = (nodoCondicion)val1;
                            string etVs = cond1.getEtiquetasVerdaderas();
                            c3d.addCodigo(etVs);
                            Object val2 = evaluarExp(nodo.ChildNodes[1]);
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion cond2 = (nodoCondicion)val2;
                                string etFs = cond2.getEtiquetasFalsas();
                                //c3d.addCodigo(etFs);
                                retorno.addEtiquetasVerdaderas(cond2.etiquetasVerdaderas);
                                retorno.addEtiquetasFalsas(cond1.etiquetasFalsas);
                                retorno.addEtiquetasFalsas(cond2.etiquetasFalsas);
                                Console.WriteLine("Verdaderas: " + retorno.getEtiquetasVerdaderas());
                                Console.WriteLine("Falsas: " + retorno.getEtiquetasFalsas());
                                return retorno;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "Segundo valor para condicion and debe ser una condicion", nodo.ChildNodes[1].Token);
                                Form1.errores.addError(n);
                                return "nulo";
                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "Primer valor para condicion and debe ser una condicion", nodo.ChildNodes[0].Token);
                            Form1.errores.addError(n);
                            return "nulo";
                        }
                    }



                case Constantes.orJava:
                    {
                        nodoCondicion retorno = new nodoCondicion("");
                        Object val1 = evaluarExp(nodo.ChildNodes[0]);
                        if (val1 is nodoCondicion)
                        {
                            nodoCondicion cond1 = (nodoCondicion)val1;
                            c3d.addCodigo(cond1.getEtiquetasFalsas());
                            Object val2 = evaluarExp(nodo.ChildNodes[1]);
                            if (val2 is nodoCondicion)
                            {
                                nodoCondicion cond2 = (nodoCondicion)val2;
                                retorno.addEtiquetasVerdaderas(cond1.etiquetasVerdaderas);
                                retorno.addEtiquetasVerdaderas(cond2.etiquetasVerdaderas);
                                retorno.addEtiquetasFalsas(cond2.etiquetasFalsas);
                                Console.WriteLine("Verdaderas: " + retorno.getEtiquetasVerdaderas());
                                Console.WriteLine("Falsas: " + retorno.getEtiquetasFalsas());
                                return retorno;
                            }
                            else
                            {
                                ErrorA n = new ErrorA("Semantico", "Segundo valor para condicion or debe ser una condicion", nodo.ChildNodes[1].Token);
                                Form1.errores.addError(n);
                                return "nulo";

                            }

                        }
                        else
                        {
                            ErrorA n = new ErrorA("Semantico", "Primer valor para condicion or debe ser una condicion", nodo.ChildNodes[0].Token);
                            Form1.errores.addError(n);
                            return "nulo";

                        }



                    }

                #endregion 


                    /*

                #region llamadaMetodo

                /* LEXPRESIONES.Rule = MakeStarRule(LEXPRESIONES, ToTerm(","), EXPRESION);

            LLAMADA.Rule = identificador + ToTerm("[") + LEXPRESIONES + "]"
                | identificador + ToTerm("[") + "]";*/

                /*
                 * LEXPRESIONES.Rule = MakePlusRule(LEXPRESIONES, ToTerm(","), EXPRESION);
                 LLAMADA.Rule = identificador + ToTerm("(") + LEXPRESIONES + ")"
            | identificador + ToTerm("(") + ")";

                     
                \

                case Constantes.llamada:
                    {
                        int noHijos = nodo.ChildNodes.Count;
                        if (noHijos == 1)
                        {//no tiene parametros

                        }
                        else
                        {//si tiene parametros

                        }




                        break;
                    }

                #endregion

                    */
            }




            return "nulo";
        }

        #endregion


#endregion


        #region validacionTipos Retorna de que tipo es la expresion

        private object validarTipo(ParseTreeNode nodo, Ambitos ambito)
        {
            switch (nodo.Term.Name)
            {
                case Constantes.expresion:
                    {
                        return validarTipo(nodo.ChildNodes[0], ambito);
                    }

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
                        string tipo= tablaSimbolos.getTipo(nodo.ChildNodes[0].Token.ValueString, ambito);
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
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarSuma(tipo1, tipo2);
                    }
                case Constantes.resta:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarResta(tipo1, tipo2);
                    }

                case Constantes.multiplicacion:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarMultiplicacion(tipo1, tipo2);
                    }

                case Constantes.division:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarDivision(tipo1, tipo2);
                    }

                case Constantes.potencia:
                    {
                        object tipo1 = validarTipo(nodo.ChildNodes[0], ambito);
                        object tipo2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarPotencia(tipo1, tipo2);
                    }

                #endregion

                #region Relacionales

                case Constantes.mayor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.menor:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.mayorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }


                case Constantes.menorIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.distintoA:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }

                case Constantes.igualIgual:
                    {
                        Object val1 = validarTipo(nodo.ChildNodes[0], ambito);
                        Object val2 = validarTipo(nodo.ChildNodes[1], ambito);
                        return validarRelacional(val1, val2);
                    }
                #endregion


                #region Logicas

                case Constantes.andJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                    }
                case Constantes.orJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                    }
                case Constantes.xorJava:
                    {
                        Object val1= validarTipo(nodo.ChildNodes[0],ambito);
                        Object val2= validarTipo(nodo.ChildNodes[1],ambito);
                        return validarLogica(val1, val2);
                        //return validarUnario(validarTipo(nodo.ChildNodes[0],ambito));
                    }

                case Constantes.notJavaPython:
                    {
                        return validarNot(validarTipo(nodo.ChildNodes[0], ambito));
                    }
                #endregion

                #region unarios

                case Constantes.masmas:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
                    }

                case Constantes.menosmenos:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
                    }

                case Constantes.negativo:
                    {
                        return validarUnario(validarTipo(nodo.ChildNodes[0], ambito));
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

                if((esDouble(val1) || esInt(val1) || esChar(val1)) &&
                    esDouble(val2) || esInt(val2) || esChar(val2))
                {
                    return Constantes.tipoBool;
                }
                else if((esCadena(val1) || esChar(val1)) &&
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



        public object resolverExpresiones(ParseTreeNode nodo, Ambitos ambiente, String nombreClase, String nommbreMetodo)
        {
            string nombreNodo = nodo.Term.Name;
            switch (nombreNodo)
            {


                    case Constantes.expresion:
                    {
                        return resolverExpresiones(nodo.ChildNodes[0], ambiente, nombreClase, nommbreMetodo);
                    }


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
                        int posId = tablaSimbolos.getPosicion(nombreId,ambiente);
                        string tipoId = tablaSimbolos.getTipo(nombreId, ambiente);
                        if (posId != -1)
                        {
                            // es una variable local
                            if(tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
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
                                string l15= eti1 + ":\n";
                                string temp3 = c3d.getTemporal();
                                string l4 = temp3 + " = HEAP[" + temp2 + "];";
                                string l5 = "if( " + temp3 + " == -1 ) then goto " + eti2 + ";";
                                string l6 = "goto " + eti3 + ";";
                                string l7= eti2+":";
                                string l8 = "goto "+ eti4+";";
                                string l9 = eti3 + ":";
                                string l10 = "HEAP[H] = " + temp3 + ";";
                                string l11 = "H = H + 1;";
                                string l12 = temp2 + " = " + temp2 + " + 1" + ";";
                                string l13= "goto " + eti1 + ";";
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
                            if(tipoId.Equals(Constantes.tipoBool, StringComparison.OrdinalIgnoreCase) ||
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








            }
            return "nulo";

        }
       



    }
}
