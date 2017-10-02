using Proyecto2_201122872.AnalizadorJava;
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

        public Form1()
        {
            InitializeComponent();
            analizador = new Arbol();
        }


        private string getCadenaArchivo(){
            StreamReader archivo = new StreamReader(@"C:\Users\Alina\Documents\Repositorios\Proyecto2\ArchivosEntrada\ejemplo1.txt");
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
            String contenido = getCadenaArchivo();
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
    }
}
