﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Ast;
using Irony.Parsing;
using Proyecto2_201122872.UML;

namespace Proyecto2_201122872.Interprete3D
{
    public class Arbol3D
    {

        private int niv = 0;
     public AccionInterprete accion;
        private LanguageData lenguaje;
        private Parser p;
        private String path;
        public string nombreMain;

        public Arbol3D()
        {
            
            
            lenguaje = new LanguageData(new Sintactico());//clase de la gramatica
            p = new Parser(lenguaje);
            path = @"C:\";
        }

        #region 1. ANALIZAR ARBOL


        public ParseTreeNode parse(String str)
        {
            ParseTree s_tree = p.Parse(str); //parsear la entrada
            if (s_tree.HasErrors())//SE VERIFICA SI LA ENTRADA POSEE ERRORES
                foreach (var item in s_tree.ParserMessages)
                {   //SI EXISTEN ERRORES LOS IMPRIME van errores <--------- 
                    Console.WriteLine("en Fil: {0}, Col: {1}, Pos: {2}, Tipo: {3}", item.Location.Line + 1, item.Location.Column + 1, item.Location.Position, item.Message);
                }


            if (s_tree.Root != null)
            {
                //Console.WriteLine("========ACCIONES AL EVVALUAR EL ARBOL=============");
                accion = new AccionInterprete(s_tree.Root);
                accion.setMetodoInicio(nombreMain);
                accion.ejecutarCodigo();
                 //accion.Evaluar(s_tree.Root, nombreMain);//EVALUO EL ARBOL
               // Console.WriteLine("==================================================");
               // dispTree(s_tree.Root, 0);//IMPRIMO EL ARBOL
               // graficarArbol(s_tree.Root);//GRAFICO EL ARBOL
                return s_tree.Root;
            }
            else
                MessageBox.Show(null, "Entrada posee errores", "Error", 0);
            return null;
        }


        public void dispTree(ParseTreeNode node, int level)
        {
            for (int i = 0; i < level; i++)
                Console.Write("  ");
            Console.WriteLine(node);

            foreach (ParseTreeNode child in node.ChildNodes)
                dispTree(child, level + 1);


        }

        #endregion

        #region 2. GRAFICAR ARBOL >>>>NO LE PONGAS ATENCION A ESTO CON QUE LO GRAFIQUE ES MAS QUE SUFICIENTE<<<
        //Generar Arbol


        public void graficarArbol(ParseTreeNode root)
        {
            String arbolDerivacion = path + "Arbol.txt";
            String texto = getCodigoArbol(root);
            System.IO.File.WriteAllText(arbolDerivacion, texto);
            Console.WriteLine(arbolDerivacion);
            try
            {
                var command = string.Format("dot -Tjpg {0} -o {1}", "\"" + arbolDerivacion + "\"", "\"" + arbolDerivacion.Replace(".txt", ".jpg") + "\"");
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C " + command);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

        private String getCodigoArbol(ParseTreeNode raiz)
        {
            String texto = "digraph G{\n graph [ dpi = 100 ]; \n",
                nivel = "0";
            texto += graficarNodo(raiz, nivel);
            texto += "}";
            return texto;
        }


        private String graficarNodo(ParseTreeNode raiz, String nivel)//NO LE PONGAS ATENCION A ESTO CON QUE LO GRAFIQUE ES MAS QUE SUFICIENTE
        {
            String texto = String.Empty;
            //            texto += ("nnivel" + "n_" + niv) + " [label=\"" + generarNodo(raiz.ToString()) + "\"];\n ";
            texto += generarNodo("nivel", nivel) + " [label=\"" + generarNodo(raiz.ToString()) + "\"];\n ";
            for (int i = 0; i < raiz.ChildNodes.Count; i++)
            {
                texto += generarNodo("nivel", nivel) + " -> " + generarNodo("nivel", nivel + "_" + i) + ";\n ";
                texto += graficarNodo(raiz.ChildNodes[i], nivel + "_" + i);
                niv++;
            }
            return texto;


        }

        private String generarNodo(String texto, String nivel) //NO LE PONGAS ATENCION A ESTO CON QUE LO GRAFIQUE ES MAS QUE SUFICIENTE
        {
            String txt = texto
                         .Replace("+=", "asgmas")
                         .Replace("=", "asg")
                         .Replace("-", "biMns")//
                         .Replace("+", "biMas")
                         .Replace("%", "mod")
                         .Replace("/", "div")
                         .Replace("*", "mul")
                         .Replace("^", "pot")
                         .Replace("++", "umas")
                         .Replace("--", "umns")
                         .Replace("&&", "and")//
                         .Replace("!&&", "nand")
                         .Replace("||", "or")
                         .Replace("!||", "nor")
                         .Replace("&|", "xor")
                         .Replace("!", "not")
                         .Replace("!¡", "esNulo")
                         .Replace("==", "igual")//
                         .Replace("!=", "dif")
                         .Replace(">", "myr")
                         .Replace("<", "mnr")
                         .Replace(">=", "myri")
                         .Replace("<=", "mnri")
                         .Replace("_", "gb")
                         .Replace(".", "pto")
                         .Replace(":", "dpts");

            return "n" + generarNodo(txt) + nivel;
        }

        private String generarNodo(String texto)//NO LE PONGAS ATENCION A ESTO CON QUE LO GRAFIQUE ES MAS QUE SUFICIENTE
        {
            return
                texto
                .Replace(" (Keyword)", "")
                .Replace(" (Key symbol)", "")
                .Replace(" (numero)", "")
                .Replace(" (entero)", "")
                .Replace(" (doble)", "")
                .Replace(" (identificador)", "")
                .Replace(" (cadena)", "")
                .Replace(" (caracter)", "")
                .Replace(" (booleano)", "");
        }

        #endregion


    }
}
