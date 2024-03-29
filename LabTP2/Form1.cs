﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabTP2
{
    public partial class Fprincipal : Form
    {
        Trivago gestor = new Trivago(100);
        
        Alojamiento[] mostrarAlojamientos;
        public Fprincipal()
        {
            InitializeComponent();
            this.Show();
        }
       
        private void registrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            registrarCliente();
        }

        void registrarCliente()
        {
            Fregistrar fregistro = new Fregistrar();
            if (fregistro.ShowDialog() == DialogResult.OK)
            {
                Cliente c = new Cliente(fregistro.tBnombre.Text, Convert.ToInt32(fregistro.tBdni.Text), fregistro.tBdireccion.Text, fregistro.tBtelefono.Text);
                gestor.RegistrarCliente(c);
            }
        }


        int selectedRow = -1;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
        }

        

        private void Fprincipal_Load(object sender, EventArgs e)
        {
            cBalojamiento.SelectedIndex = 0;
            cBestrellas.SelectedIndex = 0;
            cBhabit.SelectedIndex = 0;
            cBcamas.SelectedIndex = 0;
            FprecioBase fpb = new FprecioBase();

            decimal precio = 100;
            if (fpb.ShowDialog() == DialogResult.OK)
            {
                precio = fpb.tBprecioBase.Text != "" ? Convert.ToDecimal(fpb.tBprecioBase.Text) : precio;
            }
            gestor = new Trivago(precio);

            
            dataGridView1.ColumnCount = 2;

            llenarDataGrid();
        }



        private void cBalojamiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            //habilita los combobox correspondientes
            cBestrellas.Visible = cBalojamiento.SelectedIndex == 1;
            cBestrellas.Enabled = cBalojamiento.SelectedIndex == 1;
            cBhabit.Visible = cBalojamiento.SelectedIndex == 1;
            cBhabit.Enabled = cBalojamiento.SelectedIndex == 1;
            cBcamas.Visible = cBalojamiento.SelectedIndex == 2;
            cBcamas.Enabled = cBalojamiento.SelectedIndex == 2;
            mostrarAlojamientos = null;

            switch (cBalojamiento.SelectedIndex)
            {
                case 0:
                    mostrarAlojamientos = gestor.MostrarAlojamientos();
                    break;
                case 1:
                    Habitacion h = new Habitacion(cBhabit.SelectedIndex, "", cBestrellas.SelectedIndex+1, "");
                    mostrarAlojamientos = gestor.MostrarAlojamientos(h);
                    break;
                case 2:
                    Casa c = new Casa(cBcamas.SelectedIndex, "", new bool[] { false }, 1);
                    mostrarAlojamientos = gestor.MostrarAlojamientos(c);
                    break;
            }
            
            llenarDataGrid(mostrarAlojamientos);
            
        }

        void llenarDataGrid(Alojamiento[] al)
        {
            dataGridView1.Rows.Clear();
            foreach (Alojamiento a in al)
            {
                dataGridView1.Rows.Add(a.Direccion, $"${a.PrecioPorDia().ToString("0.00")}");
            }
            selectedRow = -1;
            dataGridView1.ClearSelection();
        }
        void llenarDataGrid()
        {
            llenarDataGrid(gestor.MostrarAlojamientos());
        }

        private void Fprincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            gestor.BackUp();
        }

        

        private void bajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void cBcamas_SelectedIndexChanged(object sender, EventArgs e)
        {
            cBalojamiento_SelectedIndexChanged(sender, e);
        }

        private void cBhabit_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            cBalojamiento_SelectedIndexChanged(sender, e);
        }

        private void cBestrellas_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            cBalojamiento_SelectedIndexChanged(sender, e);
        }



        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            verReservas();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            verReservas();

        }

        void verReservas()
        {
            FcalendarioYdatos fCyD = new FcalendarioYdatos();
            if (selectedRow != -1)
            {
                
                Alojamiento al = mostrarAlojamientos[selectedRow];
                fCyD.lAlojElegido.Text = $"{al.Direccion}";
                reservasDelAlojamiento(al);
                fCyD.imagenes = al.listaImagenes;
                if (fCyD.imagenes!=null)
                {
                    fCyD.pbImagenes.Image = fCyD.imagenes[0];
                }
            
                foreach (Cliente c in gestor.Clientes)
                {
                    fCyD.cbClientes.Items.Add(c.Nombre);
                }
                if (fCyD.ShowDialog() == DialogResult.OK)
                {
                    TimeSpan dif = fCyD.monthCalendar1.SelectionRange.End - fCyD.monthCalendar1.SelectionRange.Start;
                    bool seSolapan = false;
                    //for(DateTime i= fCyD.monthCalendar1.SelectionRange.Start;i<= fCyD.monthCalendar1.SelectionRange.End; i.AddDays(1))
                    //{
                    //    foreach(DateTime d in fCyD.monthCalendar1.BoldedDates)
                    //    {
                    //        if(d==i) seSolapan=true;    
                    //    }
                    //}
                    if (dif.Days>0 && fCyD.cbClientes.SelectedIndex!=-1 && seSolapan==false)
                    {
                        Reserva r = new Reserva((Cliente)gestor.Clientes[fCyD.cbClientes.SelectedIndex], DateTime.Today, fCyD.monthCalendar1.SelectionRange.Start, dif.Days + 1, al);
                        gestor.CrearReservas(r);

                    }
                }
            }

            void reservasDelAlojamiento(Alojamiento al)
            {
                foreach (Reserva r in al.mostrarReservas())
                {
                    if (r.alojamiento.Equals(al))
                    {
                        for (int i = 0; i < r.Dias; i++)
                        {
                            fCyD.monthCalendar1.AddBoldedDate(r.FechaChekIn.AddDays(i));

                        }
                    }
                }
            }
        }


        private void bajaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Fregistro fr = new Fregistro();
            foreach (Reserva r in gestor.Reservas)
            {
                fr.lBregistro.Items.Add($"{r.Cliente.Nombre}, {r.FechaChekIn:dd/MM} al {r.FechaChekOut:dd/MM} en {r.alojamiento.Direccion} ");
            }
            fr.ShowDialog();
        }

        private void menuAlojAlta_Click(object sender, EventArgs e)
        {
            fAlojamiento fa = new fAlojamiento("agregar");
            fa.comboBox1.SelectedIndex = 0;
            fa.comboBox2.SelectedIndex = 0;
            if (fa.ShowDialog() == DialogResult.OK)
            {
                Alojamiento a = null; ;

                if (fa.comboBox1.SelectedIndex == 1)
                {
                    bool[] serv = new bool[] { fa.chBCochera.Checked, fa.chBPileta.Checked, fa.chBWifi.Checked, fa.chBLimpieza.Checked, fa.chBDesayuno.Checked, fa.chBMascotas.Checked };
                    a = new Casa(Convert.ToInt32(fa.numericUpDown1.Value), fa.tBDIreccion.Text, serv, Convert.ToInt32(fa.numericUpDown2.Value));
                    gestor.AgregarAlojamiento(a);
                }
                else if (fa.comboBox1.SelectedIndex == 0)
                {

                    a = new Habitacion(Convert.ToInt32(fa.numericUpDown1.Value), fa.tBDIreccion.Text, Convert.ToInt32(fa.comboBox2.SelectedIndex) + 2, fa.tBNumHab.Text);
                    gestor.AgregarAlojamiento(a);
                }
                a.listaImagenes = fa.listaimagenes;
                mostrarAlojamientos = gestor.MostrarAlojamientos();
                llenarDataGrid();
            }
            cBalojamiento.SelectedIndex = 0;
        }

        


        private void menuAlojBaja_Click(object sender, EventArgs e)
        {
            fAlojamiento fa = new fAlojamiento("baja");
            foreach (Alojamiento a in gestor.MostrarAlojamientos())
            {
                fa.cbAlojamientos.Items.Add(a);
            }
            if (fa.ShowDialog() == DialogResult.No)
            {
                if (fa.cbAlojamientos.SelectedIndex >= 0)
                {
                    Alojamiento a = (Alojamiento)fa.cbAlojamientos.SelectedItem;
                    foreach (Reserva r in gestor.MostrarReservas())
                    {
                        if (r.alojamiento.Equals(a)) gestor.Reservas.Remove(r);
                    }
                    gestor.QuitarAlojamiento(a);
                }
                mostrarAlojamientos = gestor.MostrarAlojamientos();
                llenarDataGrid();
            }
            cBalojamiento.SelectedIndex = 0;
        }

        private void menuAlojModi_Click(object sender, EventArgs e)
        {
            fAlojamiento fa = new fAlojamiento("modif");
            foreach (Alojamiento a in gestor.MostrarAlojamientos())
            {
                fa.cbAlojamientos.Items.Add(a);
            }
            
            if (fa.ShowDialog() == DialogResult.Yes)
            {
                {
                    Alojamiento a = null;

                    if (fa.comboBox1.SelectedIndex == 1)
                    {
                        bool[] serv = new bool[] { fa.chBCochera.Checked, fa.chBPileta.Checked, fa.chBWifi.Checked, fa.chBLimpieza.Checked, fa.chBDesayuno.Checked, fa.chBMascotas.Checked };
                        a = new Casa(Convert.ToInt32(fa.numericUpDown1.Value), fa.tBDIreccion.Text, serv, Convert.ToInt32(fa.numericUpDown2.Value));
                        gestor.Alojamientos[fa.cbAlojamientos.SelectedIndex] = a;
                        //gestor.Alojamientos[fa.cbAlojamientos.SelectedIndex]= a;
                    }
                    else if (fa.comboBox1.SelectedIndex == 0)
                    {

                        a = new Habitacion(Convert.ToInt32(fa.numericUpDown1.Value), fa.tBDIreccion.Text, Convert.ToInt32(fa.comboBox2.SelectedIndex) + 2, fa.tBNumHab.Text);
                        //alojamientos[fa.cbAlojamientos.SelectedIndex] = a;
                        gestor.Alojamientos[fa.cbAlojamientos.SelectedIndex] = a;

                    }
                    a.listaImagenes = fa.listaimagenes;
                    mostrarAlojamientos = gestor.MostrarAlojamientos();
                    llenarDataGrid();
                }
            }


            //aca deberia cargar los datos del alojamiento en los elementos de la ventana y si habo cambios sobreescribir el alojamiento
            //con gesto.modificarAlojamiento(indice del alojamiento, nuevo objeto alojamiento)

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void menuClienteRegistrar_Click(object sender, EventArgs e)
        {
            Fregistrar fr = new Fregistrar();
            if (fr.ShowDialog() == DialogResult.OK)
            {
                Cliente c = new Cliente(fr.tBnombre.Text, Convert.ToInt32(fr.tBdni.Text), fr.tBdireccion.Text, fr.tBtelefono.Text);
                gestor.RegistrarCliente(c);
            }
        }

        private void menuClienteVer_Click(object sender, EventArgs e)
        {
            Fregistro fr= new Fregistro();
            fr.bBaja.Enabled = false;
            fr.bBaja.Visible = false;
            foreach (Cliente c in gestor.Clientes)
            {
                fr.lBregistro.Items.Add(c.Nombre);
            }
            if (fr.ShowDialog() == DialogResult.OK) { }
            
        }

        private void menuReservaConsu_Click(object sender, EventArgs e)
        {
            Fregistro fr = new Fregistro();
            fr.button1.Visible = false;
            LLenarList();

            while (fr.ShowDialog() != DialogResult.Cancel)
            {

                Reserva res = (Reserva)fr.lBregistro.SelectedItem;
                if (res != null)
                {
                    res.alojamiento.CancelarReserva(res);
                    gestor.CancelarReserva(res);

                }

                LLenarList();
            }

            void LLenarList()
            {
                fr.lBregistro.Items.Clear();
                foreach (Reserva r in gestor.Reservas)
                {
                    fr.lBregistro.Items.Add(r);
                }
            }
        }
    }
}
