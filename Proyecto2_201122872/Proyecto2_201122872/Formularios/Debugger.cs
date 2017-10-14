using FastColoredTextBoxNS;
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

        ArrayList ArrayTab = new ArrayList();
        ArrayList ArrayTxt = new ArrayList();
        int cont;
        
        public Debugger()
        {
            InitializeComponent();
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


    }
}
