
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using namespace Parser;
using namespace Scanner;
namespace CompilerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
               List<string> ctokenvalue = new List<string>();
               List<string> ctokentype = new List<string>();
            string s = "x:=2; \n y:= 32436; \n   z:= 5; \n  a:= 9 * 5 - 2 - 1 + 6 / 3; \n if z < 8 then \n repeat \n a:= a * 2; \n z:= z - 1;   \n repeat  \n read x;           \n write x;  \n if ((x < 4)) then  \n x:= x + 1 + 2 + 3 + 4 + s \n else   \n  x:= x - 1 \n end \n until x < 5 \n until z = 0; \n  write a;  \n   read b \n else   \n   read b;    \n if b = 1 then \n write b * (x - y); \n if x = 1 then \n  x:= x + 1 \n else \n read x \n end \n else \n write a; \n write z \n end; \n read b \n end; \n write z; \n write x*x * x; \n read x;  \n x:= x * 4 / 2 * s";
            //string s = "x";   
            Scanner.scanner(s,ctokenvalue,ctokentype);
            

            // Console.WriteLine("Token value" + "  "+ "Token type");

            /*for(int i = 0; i < ctokentype.Count; i++)
            {
                Console.WriteLine(" " +ctokenvalue[i] +"  " + " "+ctokentype[i]);
            }
         */
            Procedures p = new Procedures(ctokentype,ctokenvalue);
            Node n =p.program();
            p.inOrderConstantFolding(ref n);
            if (p.Status==1) { p.inOrderTraversal(n);Console.WriteLine($"{(n.Next).X},{(n.Next).Y}"); }
          else { Console.WriteLine("Can`t be parsed"); }
         

        

        }




    }
    }
