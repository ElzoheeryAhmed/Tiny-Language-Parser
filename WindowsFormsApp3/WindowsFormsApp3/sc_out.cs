using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp3;

namespace scanner
{
    public partial class sc_out : Form
    {
        List<string> ctokenvalue;
        List<string> ctokentype;
        Form1 f;
        public sc_out(List<string> ctokenvalue, List<string> ctokentype, Form1 f)
        {
            this.f = f;
            this.ctokentype = ctokentype;
            this.ctokenvalue = ctokenvalue;
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            f.Show();
            this.Hide();
        }

       

        private void sc_out_Load_1(object sender, EventArgs e)
        {
            if (ctokentype.Count != 0 && ctokentype.Last() == "Undefined string")
            {
                button2.Hide();
            }
            else
            {
                button2.Show();
            }
            string out_1 = "";
            string out_2 = "";
            for (int i = 0; i < ctokentype.Count; i++)
            {
                out_1 = out_1 + (i + 1) + "-   " + ctokenvalue[i] + System.Environment.NewLine;
                out_2 = out_2 + (i + 1) + "-   " + ctokentype[i] + System.Environment.NewLine;

            }

            textBox1.Text = out_1;
            textBox2.Text = out_2;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            graph form = new graph(ctokenvalue, ctokentype, this, 0 );
            form.Show();
            this.Hide();
        }

        private void sc_out_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            graph form = new graph(ctokenvalue, ctokentype, this, 1);
            form.Show();
            this.Hide();
        }
    }
}
