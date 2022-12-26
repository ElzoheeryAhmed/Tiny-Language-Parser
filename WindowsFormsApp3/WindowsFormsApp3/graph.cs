using scanner;
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

namespace WindowsFormsApp3
{
    public partial class graph : Form
    {

        List<string> ctokenvalue;
        List<string> ctokentype;
        sc_out f;


        Graphics s;
        int index = 0;
        int x = 50, y = 10;
        int i = 0;
        int flag = 1;
        int invalid_rule_flag = 0;
        Brush a1 = new SolidBrush(Color.Green);
        Brush a2 = new SolidBrush(Color.White);
        Font Fo1 = new Font("Tahoma", 10);
        int opt_flag;



        public graph(List<string> ctokenvalue, List<string> ctokentype, sc_out f,int o)
        {
            this.opt_flag = o;
            this.f = f;
            this.ctokentype = ctokentype;
            this.ctokenvalue = ctokenvalue;
            InitializeComponent();
        }
        public void draw_statement(int x, int y, string info) // rectangle
        {
            s = panel1.CreateGraphics();
            s.FillRectangle(a1, x, y, 50, 50);
            s.DrawString(info, new Font("Tahoma", 11), a2, (x + 5), y);
        }

        public void draw_expression(int x1, int y1, string info, int r1 = 50) // oval
        {
            s = panel1.CreateGraphics();
            s.FillEllipse(a1, x1, y1, r1 + 20, r1);
            s.DrawString(info, new Font("Tahoma", 11), a2, x1 + 10, y1 + 5);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
            s = panel1.CreateGraphics();
            panel1.Width = 10000;
            panel1.Height = 10000;
            x = 450;
            y = 10;
            if (ctokentype.Count != 0)
            {
                Procedures p = new Procedures(ctokentype, ctokenvalue);
                Node n = p.program();
                if (p.Status == 1)
                {
                    if (opt_flag == 1) { p.inOrderConstantFolding(ref n); }
                    p.inOrderPositionSetting(n);
                    List<Node> g = p.graph;
                    int[,] cord = new int[g.Count, 2];
                    
                    for (int i = 0; i < g.Count-1; i++)
                    {
                        if (g[i].Y >=g[i+1].Y && g[i].X > g[i + 1].X)
                        {
                            if (g[i + 1].parent != null)
                            {
                                g[i + 1].parent.X++;
                            }
                            for (int k = i+1; k < g.Count; k++)
                            {
                                
                                g[k].X = g[k].X + 1;
                            }
                            i = 0;
                        }
                    }
                    


                    for (int i = 0; i < g.Count; i++)
                    {
                        for (int j = i + 1; j < g.Count; j++)
                        {
                            int temp = 0;
                            if (g[i].X == g[j].X && g[i].Y == g[j].Y)
                            {
                                
                                for (int k = j; k < g.Count; k++)
                                {
                                    g[k].X = g[k].X + 1;
                                }
                                //g[j].X = g[j].X + 1;
                                temp = 1;
                            }
                            if (temp==1) { i = 0; }
                        }//p.inOrderPositionSetting(n);
                    }
                    
                    
                    

                    foreach (Node t in g)
                    {
                        switch (t.Type)
                        {
                            case "const":
                            case "op":
                            case "id":

                                draw_expression((t.X) * 100 + x, t.Y * 100 + y, t.Header);
                                if (t.Next != null) { draw_line(t.X * 100 + x + 50, t.Y * 100 + y + 25, t.Next.X * 100 + x, t.Next.Y * 100 + y + 25); }
                                if (t.child.Count != 0)
                                {
                                    foreach (Node l in t.child)
                                    {
                                        draw_line(t.X * 100 + x + 25, t.Y * 100 + y + 50, l.X * 100 + x + 25, l.Y * 100 + y);
                                    }
                                }
                                break;
                            default:
                                draw_statement((t.X) * 100 + x, t.Y * 100 + y, t.Header);
                                if (t.Next != null) { draw_line(t.X * 100 + x + 50, t.Y * 100 + y + 25, t.Next.X * 100 + x, t.Next.Y * 100 + y + 25); }
                                if (t.child.Count != 0)
                                {
                                    foreach (Node l in t.child)
                                    {
                                        draw_line(t.X * 100 + x + 25, t.Y * 100 + y + 50, l.X * 100 + x + 25, l.Y * 100 + y);
                                    }
                                }
                                break;


                        }

                    }
                }
                else
                {
                    MessageBox.Show("The parser spotted an isuue \n the code doesn't follow the correct rules and cannot be parsed ", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void graph_Load(object sender, EventArgs e)
        {

            
            //panel1.AutoScroll = true;
            //if (p.Status == 1) { p.inOrderTraversal(n); }
            //else { Console.WriteLine("Can`t be parsed"); }
        }

        private void graph_FormClosed(object sender, FormClosedEventArgs e)
        {
            f.Show();
        }

        public void draw_line(int x1, int y1, int x2, int y2)
        {
            Pen p = new Pen(Color.RosyBrown, 3);
            s = panel1.CreateGraphics();
            p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            s.DrawLine(p, x1, y1, x2, y2);
        }


        




    }
    //public static class FileDotEngine
    //{
    //    public static Bitmap Run(string dot)
    //    {
    //        string executable = @".\external\dot.exe";
    //        string output = @".\external\tempgraph";
    //        File.WriteAllText(output, dot);

    //        System.Diagnostics.Process process = new System.Diagnostics.Process();

    //        // Stop the process from opening a new window
    //        process.StartInfo.RedirectStandardOutput = true;
    //        process.StartInfo.UseShellExecute = false;
    //        process.StartInfo.CreateNoWindow = true;

    //        // Setup executable and parameters
    //        process.StartInfo.FileName = executable;
    //        process.StartInfo.Arguments = string.Format(@"{0} -Tjpg -O", output);

    //        // Go
    //        process.Start();
    //        // and wait dot.exe to complete and exit
    //        process.WaitForExit();
    //        Bitmap bitmap = null; ;
    //        using (Stream bmpStream = System.IO.File.Open(output + ".jpg", System.IO.FileMode.Open))
    //        {
    //            Image image = Image.FromStream(bmpStream);
    //            bitmap = new Bitmap(image);
    //        }
    //        File.Delete(output);
    //        File.Delete(output + ".jpg");
    //        return bitmap;
    //    }
    //}
}
