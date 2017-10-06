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
       
        
        
        private void iniciarValores()
        {
            cargarAtributos();
            cargarClases();
            
        }



        public crearClase()
        {
            InitializeComponent();
            iniciarValores();
        }


        private void actualizarTiposAtributos()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("entero");
            comboBox2.Items.Add("decimal");
            comboBox2.Items.Add("cadena");
            comboBox2.Items.Add("char");
            comboBox2.Items.Add("bool");
            
            for (int i = 0; i < Form1.uml.getSize(); i++)
            {
                comboBox2.Items.Add(Form1.uml.listaClases.ElementAt(i).getNombre());
            }

           


        }

        private void actualizarTiposFunciones()
        {
            comboBox4.Items.Clear();
            comboBox4.Items.Add("entero");
            comboBox4.Items.Add("decimal");
            comboBox4.Items.Add("cadena");
            comboBox4.Items.Add("char");
            comboBox4.Items.Add("bool");
            comboBox4.Items.Add("void");
            for (int i = 0; i < Form1.uml.getSize(); i++)
            {
                comboBox4.Items.Add(Form1.uml.listaClases.ElementAt(i).getNombre());
            }


        }

        private void cargarAtributos()
        {
            this.listBox1.Items.Clear();
            string temp = "";
            Atributo temporal;
            for (int i = 0; i < Form1.uml.claseActual.sizeAtributos(); i++)
            {
                temporal = Form1.uml.claseActual.atributos.atributos.ElementAt(i);
                listBox1.Items.Add(temporal.getCadenaAtributo());

            }
        }
        private void capturarAtributo()
        {

            String visibilidad = this.comboBox1.Text;
            String tipo = this.comboBox2.Text;
            String nombre = this.textBox1.Text;
            Atributo atri = new Atributo(visibilidad, nombre, tipo);
            bool res = Form1.uml.addAtributoActual(atri);
            if (!res)
            {
                MessageBox.Show("Ya existe un atributo con el mismo nombre");
            }
            else
            {
                MessageBox.Show("Se ha creado con exito el atributo");
            }

            cargarAtributos();
        }

        private void cargarClases()
        {
            String nombreTemporal = "";
            foreach (Clase item in Form1.uml.listaClases)
            {
                nombreTemporal = item.getNombre();
                comboBox3.Items.Add(nombreTemporal);

            }

        }



        private void usarClase()
        {
            String nombreClase = this.comboBox3.Text;
            bool res = Form1.uml.seleccionarClaseActual(nombreClase);
            if (res)
            {
                MessageBox.Show("Se ha seleccionado la clase " + nombreClase + ", como actual con exito");
                label5.Text = "Clase actual: " + nombreClase;

            }
            else
            {
                MessageBox.Show("Ha ocurrido un error");
            }


        }


        private void actualizarClase()
        {

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
            usarClase();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String nuevaClase = textBox2.Text;
            Clase nueva = new Clase(nuevaClase);
            Boolean v= Form1.uml.insertarClase(nueva);
            if (!v)
            {
                MessageBox.Show("Ya existe una clase con el mismo nombre", "Error");
            }
            else
            {
                MessageBox.Show("Se ha creado con exito la nueva clase");
                actualizarTiposAtributos();
                actualizarTiposFunciones();
            }

            comboBox3.Items.Clear();
            cargarClases();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";

        }
    }
}
