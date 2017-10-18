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

namespace Proyecto2_201122872.Generacion3D
{
    public class GeneracionCodigo
    {

        public tablaDeSimbolos tablaSimbolos;
        public clasesDiagrama uml;
        public Arbol analizadorJava;
        public ArbolPy analizadorPython;

        public GeneracionCodigo()
        {
            this.tablaSimbolos = new tablaDeSimbolos();
            uml = new clasesDiagrama();
            analizadorJava = new Arbol();
            analizadorPython = new ArbolPy();
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
            if (extension.Equals("tree", StringComparison.OrdinalIgnoreCase))
            {
                analizadorPython.parseConvertirUML(contenido);
                
            }
            else
            {
                analizadorJava.parseConvertirUML(contenido);
               
            }

        }





    }
}
