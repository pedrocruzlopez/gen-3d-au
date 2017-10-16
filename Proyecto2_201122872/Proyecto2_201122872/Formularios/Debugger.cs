using FastColoredTextBoxNS;
using Proyecto2_201122872.Generacion3D;
using System;
using System.Collections;
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
    public partial class Debugger : Form
    {

        ArrayList ArrayTab;
        ArrayList ArrayTxt;
        int cont;
        public GeneracionCodigo generador;
        
        public Debugger()
        {
            InitializeComponent();
            inicializar();
        }


        private void inicializar()
        {
            this.ArrayTab = new ArrayList();
            this.ArrayTxt = new ArrayList();
            cont = 0;

            this.dataGridView1.Columns.Add("acceso", "Acceso");
            this.dataGridView1.Columns.Add("nombreAcceso", "Nombre de Acceso");
            this.dataGridView1.Columns.Add("nombre", "Nombre");
            this.dataGridView1.Columns.Add("tipo", "Tipo");
            this.dataGridView1.Columns.Add("ambito", "Ambito");
            this.dataGridView1.Columns.Add("rol", "Rol");
            this.dataGridView1.Columns.Add("apuntador", "Apuntador");
            this.dataGridView1.Columns.Add("tamanho", "Tamanho");

            string[] row0 = { "11/22/1968", "29", "Revolution 9", 
        "Beatles", "The Beatles [White Album]","fdgsgds","dsffdsf","hhhhh" };
            dataGridView1.Rows.Add(row0);

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            nuevaTab();

        }




        #region editor

        private void nuevaTab()
        {
            TabPage nueva = new TabPage("New " + cont);
            FastColoredTextBox txt = new FastColoredTextBox();
            this.AutoScroll = true;
            txt.SetBounds(0, 0, 699, 435);
            ArrayTab.Add(nueva);
            ArrayTxt.Add(txt);
            tabControl1.TabPages.Add(nueva);
            nueva.Controls.Add(txt);
            cont++;
            tabControl1.SelectedTab = nueva;
        }



        #endregion

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            generador = new GeneracionCodigo();

        }



        #region TablaSimbolos

        private void mostrarTabla()
        {

        }



        #endregion

    }
}
