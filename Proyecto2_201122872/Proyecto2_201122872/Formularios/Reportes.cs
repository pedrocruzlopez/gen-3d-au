using Proyecto2_201122872.Errores;
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
    public partial class Reportes : Form
    {
        public Reportes()
        {
            InitializeComponent();
        }

        private void Reportes_Load(object sender, EventArgs e)
        {

        }

        private void mostrarErrores()
        {
            /*public string tipoError;
        public int fila;
        public int posicion;
        public int columna;
        public string mensaje;*/
            this.dataGridView1.Columns.Add("tipoError", "Tipo Error");
            this.dataGridView1.Columns.Add("fila", "Fila");
            this.dataGridView1.Columns.Add("posicion", "Posicion");
            this.dataGridView1.Columns.Add("columna", "Columna");
            this.dataGridView1.Columns.Add("mensaje", "Mensaje");

            foreach (ErrorA s in Form1.errores.errores)
            {
                string[] row0 = { s.tipoError, s.fila.ToString(), s.posicion.ToString(), s.columna.ToString(), s.mensaje};//.ambito, s.rol, s.apuntador + "", s.tamanho + "", s.tamanhoParametros + "" };
                dataGridView1.Rows.Add(row0);
            }




        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mostrarErrores();
        }


    }
}
