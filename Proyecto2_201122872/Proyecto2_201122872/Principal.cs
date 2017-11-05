using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.AnalizadorPython;
using System.IO;
using Proyecto2_201122872.UML;
using Proyecto2_201122872.Generacion3D;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;

namespace Proyecto2_201122872
{
    public partial class Principal : Form
    {
        Arbol analizadorJava;
        ArbolPy analizadorPython;
        GeneracionCodigo generador;

        public Principal()
        {
            InitializeComponent();
            analizadorJava = new Arbol();
            generador = new GeneracionCodigo();
            
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //ejecutar tree
            analizadorJava = new Arbol();
            String contenido = getCadenaArchivo(@"C:\Users\Alina\Documents\Repositorios\CompiProyecto2\ArchivosEntrada\ejemplo2.txt");
            //clasesDiagrama uml = analizadorPython.parseConvertirUML2(contenido);
            ParseTreeNode raiz = analizadorJava.parse(contenido);
            Object g = "";
            if (raiz != null)
            {
                //g = generador.evaluarExp(raiz);

            }

            Console.WriteLine("------ Inicio codigo -------");
            Console.WriteLine(g);
            Console.WriteLine(generador.c3d.codigo3D);
            Console.WriteLine("------ fin codigo  -------");
            


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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
          //Ejecutar Archivo



        }






    }
}
