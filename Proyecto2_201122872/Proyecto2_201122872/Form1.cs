using Proyecto2_201122872.AnalizadorJava;
using Proyecto2_201122872.AnalizadorPython;
using Proyecto2_201122872.Formularios;
using Proyecto2_201122872.UML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto2_201122872
{
    
    public partial class Form1 : Form
    {
        private Arbol analizador;
        private ArbolPy analizador2;
        public static clasesDiagrama listaClases;

        public Form1()
        {
            InitializeComponent();
            analizador = new Arbol();
            analizador2 = new ArbolPy();
             listaClases= new clasesDiagrama(); 
            listaClases.insertar(new Clase("ejemplo1"));
            listaClases.insertar(new Clase("ejemplo2"));

        }

      


        private string getCadenaArchivo(String ruta){
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


        private void button1_Click(object sender, EventArgs e)
        {
            String contenido = getCadenaArchivo(@"C:\Users\Alina\Documents\Repositorios\CompiProyecto2\ArchivosEntrada\ejemplo1.txt");
           // if (!contenido.Equals(""))
            //{
                analizador.parse(contenido);
          //  }
           // else
           // {
               // MessageBox.Show("No hay codigo para analisis");
           // }
           // Console.WriteLine(Accion.Cadena);
           // Accion.Cadena = "";
          // /
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String contenido = getCadenaArchivo(@"C:\Users\Alina\Documents\Repositorios\CompiProyecto2\ArchivosEntrada\ejemplo2.txt");
            // if (!contenido.Equals(""))
            //{
            analizador2.parse(contenido);



        }

        private void button2_Click(object sender, EventArgs e)
        {
            crearClase nuevo = new crearClase();
            nuevo.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
