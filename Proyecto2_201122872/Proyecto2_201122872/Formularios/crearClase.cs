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

        private ListaParametro lParametros;
       
        
        
        private void iniciarValores()
        {
            lParametros = new ListaParametro();
            cargarAtributos();
            cargarClases();
            actualizarTipos();
            
        }



        public crearClase()
        {
            InitializeComponent();
            iniciarValores();
        }


        private void actualizarTipos()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("entero");
            comboBox2.Items.Add("decimal");
            comboBox2.Items.Add("cadena");
            comboBox2.Items.Add("char");
            comboBox2.Items.Add("bool");
            comboBox4.Items.Clear();
            comboBox4.Items.Add("entero");
            comboBox4.Items.Add("decimal");
            comboBox4.Items.Add("cadena");
            comboBox4.Items.Add("char");
            comboBox4.Items.Add("bool");
            comboBox4.Items.Add("Void");
            comboBox6.Items.Clear();
            comboBox6.Items.Add("entero");
            comboBox6.Items.Add("decimal");
            comboBox6.Items.Add("cadena");
            comboBox6.Items.Add("char");
            comboBox6.Items.Add("bool");
            
            for (int i = 0; i < Form1.uml.getSize(); i++)
            {
                comboBox2.Items.Add(Form1.uml.listaClases.ElementAt(i).getNombre());
                comboBox4.Items.Add(Form1.uml.listaClases.ElementAt(i).getNombre());
                comboBox6.Items.Add(Form1.uml.listaClases.ElementAt(i).getNombre());
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
            Form1.uml.actualizarClase(Form1.uml.claseActual);

        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!Form1.uml.claseActual.esNula())
                capturarAtributo();
            else
            {
                MessageBox.Show("Debe seleccionar una clase en la cual trabajar","Error");
            }
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
                actualizarTipos();
            }

            comboBox3.Items.Clear();
            cargarClases();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nombreVar = textBox4.Text;
            string tipo = comboBox6.Text;
            if (!this.lParametros.addParametro(new variable(nombreVar, tipo)))
            {
                MessageBox.Show("No se ha podido crear el parametro");
            }
            textBox4.Text = "";
            comboBox7.Items.Clear();
            for (int i = 0; i < lParametros.parametros.Count; i++)
            {
                this.comboBox7.Items.Add(lParametros.parametros.ElementAt(i).getNombreTipoVar());    
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(!Form1.uml.claseActual.esNula()){
                string nombreFun, visiFun, tipoFunc;
                nombreFun = textBox3.Text;
                visiFun = comboBox5.Text;
                tipoFunc = comboBox4.Text;
                comboBox7.Items.Clear();
                Funcion nueva = new Funcion(Form1.uml.claseActual.getNombre(), nombreFun, tipoFunc, this.lParametros, visiFun);
                if (Form1.uml.claseActual.addFuncion(nueva))
                {
                    MessageBox.Show("Funcion creada exitosamente");
                }
                else
                {
                    MessageBox.Show("La funcion no ha sido creada, existe una igual", "Error");
                } 
                textBox3.Text = "";
                lParametros = new ListaParametro();

            }
            else
            {
                MessageBox.Show("Debe seleccionar una clase actual", "Error");
            }
        }

        private void crearClase_Load(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            actualizarClase();
            this.Close();
            Form1.uml.generarGrafo();
            Form1.mostraImagen();

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}
