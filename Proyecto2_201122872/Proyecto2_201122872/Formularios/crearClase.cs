using Proyecto2_201122872.UML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Proyecto2_201122872.Formularios
{
    public partial class crearClase : Form
    {
        private listaAtributos atributos;
        private LinkedList<Funcion> funciones;
        private string nombreClase;
        private string visibilidad;
        
        
        private void iniciarValores()
        {
            this.atributos = new listaAtributos();
            this.funciones = new LinkedList<Funcion>();
            this.nombreClase = "";
            cargarClases();
            
        }



        public crearClase()
        {
            InitializeComponent();
            iniciarValores();
        }



        private void capturarAtributo()
        {

            String visibilidad = this.comboBox1.Text;
            String tipo = this.comboBox2.Text;
            String nombre = this.textBox1.Text;
            Atributo atri = new Atributo(visibilidad, nombre, tipo);
            bool res = this.atributos.addAtributo(atri);
            if (!res)
            {
                MessageBox.Show("Error", "Ya existe un atributo con el mismo nombre");
            }
            
        }

        private void cargarClases()
        {
            String nombreTemporal = "";
            foreach (Clase item in Form1.listaClases.listaClases)
            {
                nombreTemporal = item.getNombre();
                comboBox3.Items.Add(nombreTemporal);

            }

        }



        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            capturarAtributo();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            String nuevaClase = textBox2.Text;
            Clase nueva = new Clase(nuevaClase);
            Boolean v= Form1.listaClases.insertar(nueva);
            if (!v)
            {
                MessageBox.Show("Ya existe una clase con el mismo nombre", "Error");
            }
            else
            {
                MessageBox.Show("SE ha creado con exito la nueva clase");
            }

            comboBox3.Items.Clear();
            cargarClases();
        }
    }
}
