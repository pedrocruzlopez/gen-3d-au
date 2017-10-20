﻿using FastColoredTextBoxNS;
using Proyecto2_201122872.Generacion3D;
using Proyecto2_201122872.Generacion3D.TablaSimbolos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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


       private void closeTab()
        {
            TabPage current_tab = tabControl1.SelectedTab;
            ArrayTab.Remove(current_tab);
            tabControl1.TabPages.Remove(current_tab);
            cont--;
        }


       public void Abrir()
       {
           if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
           {
               TabPage auxiliar = null;
               int seleccion = tabControl1.SelectedIndex;
               String path = openFileDialog1.InitialDirectory + openFileDialog1.FileName;
               auxiliar = tabControl1.TabPages[seleccion];
               foreach (Control ctrl in auxiliar.Controls)
               {
                   if (ctrl is FastColoredTextBox)
                   {
                       FastColoredTextBox textoAuxiliar = (FastColoredTextBox)ctrl;
                       MessageBox.Show("Archivo abierto :D");
                       textoAuxiliar.Text = File.ReadAllText(path);


                   }
               }

           }
       }


       public void guarda()
       {
           if (tabControl1.TabCount == 0) return;
           TabPage tab = tabControl1.SelectedTab;
           foreach (Control ctrl in tab.Controls)
           {
               if (ctrl is FastColoredTextBox)
               {
                   FastColoredTextBox tempTxt = (FastColoredTextBox)ctrl;
                   if (tab.Tag != null)
                   {
                       File.WriteAllText((string)tab.Tag, tempTxt.Text);
                       MessageBox.Show("Guardado con exito");
                   }
                   else
                       guardarComo();
               }
           }








       }



       public void guardarComo()
       {
           if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
           {
               TabPage temp = null;
               int seleccion = tabControl1.SelectedIndex;

               temp = tabControl1.TabPages[seleccion];
               foreach (Control ctrl in temp.Controls)
               {
                   if (ctrl is FastColoredTextBox)
                   {
                       FastColoredTextBox tempTxt = (FastColoredTextBox)ctrl;
                       tabControl1.TabPages[seleccion].Tag = saveFileDialog1.FileName + ".olc";
                       tabControl1.TabPages[seleccion].Tag = saveFileDialog1.FileName + ".tree";
                       File.WriteAllText(saveFileDialog1.FileName + ".sbs", tempTxt.Text);
                       File.WriteAllText(saveFileDialog1.FileName + ".tree", tempTxt.Text);
                   }
               }

           }
       }



        #endregion

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            closeTab();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Abrir();

        }



        #region TablaSimbolos

        private void mostrarTabla()
        {

        }



        #endregion

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            guarda();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            guardarComo();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            //ejecutar
            generador = new GeneracionCodigo();
      
            string[] ubicacion = Directory.GetFiles(@"C:\Users\Alina\Documents\Repositorios\CompiProyecto2\ArchivosEntrada");
           
            
            for (int i = 0; i < ubicacion.Length; i++)
            {
                
                Console.WriteLine(Path.GetFullPath(ubicacion[i]));
                Console.WriteLine(Path.GetExtension(ubicacion[i]));
                generador.ejecutarArchivos(Path.GetFullPath(ubicacion[i]), Path.GetExtension(ubicacion[i]));

            }

           // generador.generarTablaSimbolos();
            foreach (Simbolo s in generador.tablaSimbolos.tabla)
            {
               string [] row0={s.visibilidad,s.nombreReferencia,s.nombreReal,s.tipo,s.ambito,s.rol,s.apuntador+"",s.tamanho+""};
               dataGridView1.Rows.Add(row0);
            }


            /* this.dataGridView1.Columns.Add("acceso", "Acceso");
            this.dataGridView1.Columns.Add("nombreAcceso", "Nombre de Acceso");
            this.dataGridView1.Columns.Add("nombre", "Nombre");
            this.dataGridView1.Columns.Add("tipo", "Tipo");
            this.dataGridView1.Columns.Add("ambito", "Ambito");
            this.dataGridView1.Columns.Add("rol", "Rol");
            this.dataGridView1.Columns.Add("apuntador", "Apuntador");
            this.dataGridView1.Columns.Add("tamanho", "Tamanho");

            string[] row0 = { "11/22/1968", "29", "Revolution 9", 
        "Beatles", "The Beatles [White Album]","fdgsgds","dsffdsf","hhhhh" };
            dataGridView1.Rows.Add(row0);*/

        }

    }
}