using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace scanner
{
    public partial class Form1 : Form
    {


        public Form1()
        {

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        public static void scanner(string s, List<string> ctokenvalue, List<string> ctokentype)
        {
            #region Initialization
            Dictionary<string, string> dic = new Dictionary<String, string>();
            dic.Add(";", "SEMICOLON");
            dic.Add("if", "IF");
            dic.Add("then", "THEN");
            dic.Add("else", "ELSE");
            dic.Add("end", "END");
            dic.Add("repeat", "REPEAT");
            dic.Add("until", "UNTIL");
            dic.Add(":=", "ASSIGN");
            dic.Add("read", "READ");
            dic.Add("write", "WRITE");
            dic.Add("<", "LESSTHAN");
            dic.Add(">", "GREATERTHAN");
            dic.Add("=", "EQUAL");
            dic.Add("+", "PLUS");
            dic.Add("-", "MINUS");
            dic.Add("*", "MULT");
            dic.Add("/", "DIV");
            dic.Add("(", "OPENBRACKET");
            dic.Add(")", "CLOSEDBRACKET");

            #endregion

            Regex startIdentifier = new Regex(@"[a-z_A-Z]");
            Regex anyword = new Regex(@"[\w]");  //[a-z_A-Z0-9]
            Regex number = new Regex(@"\d"); //any digit

            for (int i = 0; i < s.Length; i++)
            {
                //Space or Linebreak
                if ((s[i] == ' ') || (s[i] == '\n') || (s[i] == '\t')) { continue; }
                if ((s[i] == '\r') && (s[i + 1] == '\n')) { i++; continue; } //need to add ((i+1)< s.Length)

                //Comment
                if (s[i] == '{') { i++; while ((i < s.Length) && (s[i] != '}')) { i++; } continue; }

                //Reserved word or Identifier
                if (startIdentifier.IsMatch(s[i].ToString()))
                {
                    int j = i;


                    while (i < s.Length && anyword.IsMatch(s[i].ToString())) //collect the characters
                    {
                        i++;

                    }

                    //Reserved word
                    if (dic.TryGetValue(s.Substring(j, i - j), out string s2))  //if it reserverd word
                    {

                        ctokenvalue.Add(s.Substring(j, i - j));
                        ctokentype.Add(s2);
                    }

                    //Identifier
                    else
                    {
                        ctokenvalue.Add(s.Substring(j, i - j));
                        ctokentype.Add("IDENTIFIER");
                    }
                    i -= 1;
                    continue;
                }
                //Number
                else if (number.IsMatch(s[i].ToString()))
                {
                    int j = i;
                    while (i < s.Length && number.IsMatch(s[i].ToString()))
                    {
                        i++;
                        if (i < s.Length && s[i] == '.' && (!s.Substring(j, i - j).Contains('.'))){ i++; }
                    }
                    ctokenvalue.Add(s.Substring(j, i - j));
                    ctokentype.Add("NUMBER");

                    i -= 1;
                    continue;

                }
                //Reserved of one character
                else if (dic.TryGetValue(s[i].ToString(), out string s2))
                {
                    ctokenvalue.Add(s[i].ToString());
                    ctokentype.Add(s2);
                    continue;
                }
                //Reserved of Two characters
                else if (((i + 1) < s.Length) && (dic.TryGetValue(s.Substring(i, 2), out string s3)))
                {

                    ctokenvalue.Add(s.Substring(i, 2));
                    ctokentype.Add(s3);
                    i++;
                    continue;
                }

                //error handling
                else
                {
                    int start = 0, end = s.Length - 1;

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (s[j] == ' ' || s[j] == '\n') { start = j + 1; break; }
                        else if (((j - 1) >= 0) && s[j] == '\n' && s[j - 1] == '\r') { start = j + 1; break; }
                    }

                    for (int k = i + 1; k < s.Length; k++)
                    {
                        if (s[k] == ' ' || s[k] == '\n') { end = k - 1; break; }
                        else if (((k + 1) < s.Length) && s[k] == '\r' && s[k + 1] == '\n') { end = k - 1; break; }
                    }

                    string undefinedString = s.Substring(start, end - start + 1);
                    ctokenvalue.Add(undefinedString);
                    ctokentype.Add("Undefined string");
                    break;

                }
            }


        }
    

    private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            List<string> ctokenvalue = new List<string>();
            List<string> ctokentype = new List<string>();

            string input = textBox1.Text;
            scanner(input, ctokenvalue, ctokentype);
            if (ctokentype.Count == 0)
            {
                MessageBox.Show("no input detected \n please write the code in the box to scan it ", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (( ctokentype.Last() == "Undefined string"))
                {
                    MessageBox.Show("The scanner spotted an undifined string\n the undefined string is " + ctokenvalue.Last() + "\n the scanning process has been stopped ", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    sc_out form = new sc_out(ctokenvalue, ctokentype, this);
                    form.Show();
                    this.Hide();
                }
            }
        }
    }
}
