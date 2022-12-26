using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{   //Nodes classes
    abstract class Node
    {
        internal Node next;
        protected string type;
        int x, y;

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public virtual string Header { get { return $"{type}"; } }
        public string Type { get { return type; } }
        public Node Next
        {
            get { return next; }
            set { next = value; }
        }


    }
    abstract class ValueNode : Node
    {
        internal protected string value;
        public string Value { get { return value; } set { this.value = value; } }
        public override string Header { get { return $"{type}\n({value})"; } }

    }
    class IfNode : Node
    {
        internal Node test, then, els;

        public IfNode()
        {
            type = "if";

        }

        public Node Test
        {
            get { return test; }
            set { test = value; }
        }

        public Node Then
        {
            get { return then; }
            set { then = value; }
        }

        public Node Else
        {
            get { return els; }
            set { els = value; }
        }

    }
    class RepeatNode : Node
    {
        internal Node body, test;

        public RepeatNode()
        {
            type = "repeat";

        }

        public Node Body
        {
            get { return body; }
            set { body = value; }
        }

        public Node Test
        {
            get { return test; }
            set { test = value; }
        }


    }
    class WriteNode : Node
    {
        internal Node exp;
        public WriteNode()
        {
            type = "write";

        }

        public Node Exp
        {
            get { return exp; }
            set { exp = value; }
        }

    }
    class AssignNode : ValueNode
    {
        internal Node exp;
        public AssignNode()
        {
            type = "assign";

        }

        public Node Exp
        {
            get { return exp; }
            set { exp = value; }
        }

    }
    class ReadNode : ValueNode
    {


        public ReadNode()
        {
            type = "read";

        }


    }
    class OpNode : ValueNode
    {
        internal Node expl, expr;
        public OpNode()
        {
            type = "op";

        }

        public Node Expl
        {
            get { return expl; }
            set { expl = value; }
        }

        public Node Expr
        {
            get { return expr; }
            set { expr = value; }
        }


    }
    class IdentifierNode : ValueNode
    {

        public IdentifierNode()
        {
            type = "id";

        }

    }
    class NumberNode : ValueNode
    {

        public NumberNode()
        {
            type = "const";

        }

    }


    //parsing procedures
    class Procedures {

        string tokent;
        string tokenv;
        int i;
        List<string> lt;
        List<string> lv;
        int status;

        public Procedures(List<String> listt, List<String> listv)
        {
            i = 0;
            status = 1;
            lt = listt;
            lv = listv;
            tokent = lt[i];
            tokenv = lv[i];
        }

        public int Status
        {
            get{ return status; }
        }

        public Node program()
        {
            try
            {   Node n= stmt_sequence();
                if (n != null)
                {
                    n.X = 0; n.Y = 0;
                    inOrderPositionSetting(n);
                }
                    return n;
                
            }
            catch(Exception e) { status = 0; return null; }
        
        }


        public Node stmt_sequence()
        {
            Node temp = statement();
            Node temp1= temp;

            while (tokent == "SEMICOLON")
            {
                match("SEMICOLON");
                temp1.Next = statement();
                temp1 = temp1.Next;
            }

            return temp;
        }


        public Node statement()
        {
            Node temp;
            switch (tokent)
            {   case "IF":
                    temp = if_stmt();
                    break;
                case "REPEAT":
                    temp = repeat_stmt();
                    break;
                case "IDENTIFIER": 
                    temp = assign_stmt();
                    break;
                case "READ":
                    temp = read_stmt();
                    break;

                case "WRITE":
                    temp = write_stmt();
                    break;
                default:
                    throw new Exception(); //Not reachable
                    

            }

            return temp;

        }


        public Node if_stmt()
        {
            IfNode temp=new IfNode();
            match("IF");
            temp.Test = exp();
            match("THEN");
            temp.Then = stmt_sequence();
            if (tokent == "END")
            {
                match("END");
            }
            else
            {
                if (tokent == "ELSE")
                {
                    match("ELSE");
                    temp.Else = stmt_sequence();
                    match("END");
                }
                else { throw new Exception(); }
            }

            return temp;
        }

        public Node repeat_stmt()
        {
            RepeatNode temp=new RepeatNode();
            match("REPEAT");
            temp.Body = stmt_sequence();
            match("UNTIL");
            temp.Test = exp();

            return temp;
        }

        public void match(string expected)
        {
            if (i + 1 < lt.Count) {
                if ((tokent == expected)) { i++; tokent = lt[i]; tokenv = lv[i]; } //pop token
                else { throw new Exception(); }
            }
            else { if ((tokent == expected)&& (i < lt.Count)) { i++; } else { throw new Exception(); } }
        }

        public Node assign_stmt() {

            AssignNode temp=new AssignNode();
            temp.Value = tokenv;
            match("IDENTIFIER");
            match("ASSIGN");
            temp.Exp = exp();

            return temp;
        }

        public Node read_stmt()
        {
            ReadNode temp=new ReadNode();
            match("READ");
            temp.Value = tokenv;
            match("IDENTIFIER");

            return temp;

        }

        public Node write_stmt()
        {
            WriteNode temp=new WriteNode();
            match("WRITE");
            temp.Exp = exp();

            return temp;
        }

        public Node exp()
        {
            Node temp = simple_exp();
            

            if (tokent == "LESSTHAN" || tokent == "EQUAL"|| tokent == "GREATERTHAN")
            {
                OpNode temp1 = new OpNode();
                temp1.Expl = temp;
                temp1.Value = tokenv;
                match(tokent);
                temp1.Expr = simple_exp();
                return temp1;
            }

            else
            {
                return temp;
            }

         
        }

        public Node simple_exp()
        {

            Node temp = term();
            
           
            OpNode temp1 ;
            while (tokent == "PLUS" || tokent == "MINUS")
            {   
                temp1 = new OpNode();
                temp1.Expl = temp;
                temp1.Value = tokenv;
                match(tokent);
                temp1.Expr = term();
                temp = temp1;
            }
            
            return temp;
 
        }

        public Node term()
        {

            Node temp = factor();

           
            OpNode temp1;
            while (tokent == "MULT" || tokent == "DIV")
            {
                temp1 = new OpNode();
                temp1.Expl = temp;
                temp1.Value = tokenv;
                match(tokent);
                temp1.Expr = factor();
                temp = temp1;

            }
            

            return temp;
        }

        public Node factor()
        {

            Node temp ;

            if (tokent == "NUMBER") { temp = new NumberNode(); ((NumberNode)temp).Value=tokenv ; match(tokent);return temp; }
            else if (tokent == "IDENTIFIER") { temp = new IdentifierNode(); ((IdentifierNode)temp).Value = tokenv; match(tokent); return temp; }
            else
            {
                match("OPENBRACKET");
                temp = exp();
                match("CLOSEDBRACKET");
                return temp;
            }


            
        }

        public void inOrderPositionSetting(Node node)
        {
            if (node == null) return;

            switch (node.Type)
            {
                case "if":
                    IfNode n = (IfNode)node;
                    if (n.Test != null) { (n.Test).X = (n.X - 1); (n.Test).Y = n.Y + 1; }
                    if (n.Then != null) { (n.Then).X = n.X; (n.Then).Y = n.Y + 1; }
                    if (n.Else != null) { (n.Else).X = n.X + 1; (n.Else).Y = n.Y + 1; }
                    if (n.Next != null) { (n.Next).X = n.X + 2; (n.Next).Y = n.Y; }
                    inOrderPositionSetting(n.Test);
                    inOrderPositionSetting(n.Then);
                    inOrderPositionSetting(n.Else);
                    inOrderPositionSetting(n.Next);
                    break;
                case "repeat":
                    RepeatNode n1 = (RepeatNode)node;
                    if (n1.Body != null) { (n1.Body).X = n1.X - 1; (n1.Body).Y = n1.Y + 1; }
                    if (n1.Test != null) { (n1.Test).X = n1.X + 1; (n1.Test).Y = n1.Y + 1; }
                    if (n1.Next != null) { (n1.Next).X = n1.X + 2; (n1.Next).Y = n1.Y; }
                    inOrderPositionSetting(n1.Body);
                    inOrderPositionSetting(n1.Test);
                    inOrderPositionSetting(n1.Next);
                    break;
                case "write":
                    WriteNode n2 = (WriteNode)node;
                    if (n2.Exp != null) { (n2.Exp).X = n2.X; (n2.Exp).Y = n2.Y + 1; }
                    if (n2.Next != null) { (n2.Next).X = n2.X + 2; (n2.Next).Y = n2.Y; }
                    inOrderPositionSetting(n2.Exp);
                    inOrderPositionSetting(n2.Next);
                    break;
                case "assign":
                    AssignNode n3 = (AssignNode)node;
                    if (n3.Exp != null) { (n3.Exp).X = n3.X; (n3.Exp).Y = n3.Y + 1; }
                    if (n3.Next != null) { (n3.Next).X = n3.X + 2; (n3.Next).Y = n3.Y; }
                    inOrderPositionSetting(n3.Exp);
                    inOrderPositionSetting(n3.Next);
                    break;
                case "read":
                    ReadNode n4=(ReadNode)node;
                    if (n4.Next != null) { (n4.Next).X = n4.X + 2; (n4.Next).Y = n4.Y; }
                    inOrderPositionSetting(n4.Next);
                    break;
                case "op":
                    OpNode n5 = (OpNode)node;
                    if (n5.Expl != null) { (n5.Expl).X = n5.X - 1; (n5.Expl).Y = n5.Y + 1; }
                    if (n5.Expr != null) { (n5.Expr).X = n5.X + 1; (n5.Expr).Y = n5.Y + 1; }
                    inOrderPositionSetting(n5.Expl);
                    inOrderPositionSetting(n5.Expr);
                    break;
                
                default :
                    break;
                
            }

        }

        public void inOrderTraversal(Node node)
        {
            if (node == null) return;

            switch (node.Type)
            {
                case "if":
                    IfNode n = (IfNode)node;
                    Console.WriteLine(n.Header);
                    inOrderTraversal(n.Test);
                    inOrderTraversal(n.Then);
                    inOrderTraversal(n.Else);
                    inOrderTraversal(n.Next);
                    break;
                case "repeat":
                    RepeatNode n1 = (RepeatNode)node;
                    Console.WriteLine(n1.Header);
                    inOrderTraversal(n1.Body);
                    inOrderTraversal(n1.Test);
                    inOrderTraversal(n1.Next);
                    break;
                case "write":
                    WriteNode n2 = (WriteNode)node;
                    Console.WriteLine(n2.Header);
                    inOrderTraversal(n2.Exp);
                    inOrderTraversal(n2.Next);
                    break;
                case "assign":
                    AssignNode n3 = (AssignNode)node;
                    Console.WriteLine(n3.Header);
                    inOrderTraversal(n3.Exp);
                    inOrderTraversal(n3.Next);
                    break;
                case "read":
                    ReadNode n4 = (ReadNode)node;
                    Console.WriteLine(n4.Header);
                    inOrderTraversal(n4.Next);
                    break;
                case "op":
                    OpNode n5 = (OpNode)node;
                    Console.WriteLine(n5.Header);
                    inOrderTraversal(n5.Expl);
                    inOrderTraversal(n5.Expr);
                    break;
                case "id":
                    IdentifierNode n6 = (IdentifierNode)node;
                    Console.WriteLine(n6.Header);
                    break;
                case "const":
                    NumberNode n7 = (NumberNode)node;
                    Console.WriteLine(n7.Header);
                    break;
                default:
                    break;

            }

        }

        public void inOrderConstantFolding(ref Node node)
        {
            if (node == null) return;

            switch (node.Type)
            {
                case "if":
                    IfNode n = (IfNode)node;
                    inOrderConstantFolding(ref (n.test));
                    inOrderConstantFolding(ref(n.then));
                    inOrderConstantFolding(ref(n.els));
                    inOrderConstantFolding(ref(n.next));
                    break;
                case "repeat":
                    RepeatNode n1 = (RepeatNode)node;
                    inOrderConstantFolding(ref(n1.body));
                    inOrderConstantFolding(ref(n1.test));
                    inOrderConstantFolding(ref(n1.next));
                    break;
                case "write":
                    WriteNode n2 = (WriteNode)node;
                    inOrderConstantFolding(ref(n2.exp));
                    inOrderConstantFolding(ref(n2.next));
                    break;
                case "assign":
                    AssignNode n3 = (AssignNode)node;
                    inOrderConstantFolding(ref(n3.exp));
                    inOrderConstantFolding(ref(n3.next));
                    break;
                case "read":
                    ReadNode n4 = (ReadNode)node;
                    inOrderConstantFolding(ref(n4.next));
                    break;
                case "op":
                    OpNode n5 = (OpNode)node;
                    inOrderConstantFolding(ref (n5.expl));
                    inOrderConstantFolding(ref(n5.expr));
                    if (((n5.Expl).Type == "const") && ((n5.Expr).Type == "const"))
                    {
                        double l = Double.Parse(((NumberNode)(n5.Expl)).Value);
                        double r = double.Parse(((NumberNode)(n5.Expr)).Value);
                        switch (n5.Value)
                        {
                            case "+":
                                node = new NumberNode();
                                ((NumberNode)node).Value = (l + r).ToString();
                                break;
                            case "-":
                                node = new NumberNode();
                                ((NumberNode)node).Value = (l - r).ToString();
                                break;
                            case "*":
                                node = new NumberNode();
                                ((NumberNode)node).Value = (l * r).ToString();
                                break;
                            case "/":
                                node = new NumberNode();
                                ((NumberNode)node).Value = (l / r).ToString();
                                break;
                            default:
                                break;
                        }


                    }
                    break;
                case "id":
                    IdentifierNode n6 = (IdentifierNode)node;
                    break;
                case "const":
                    NumberNode n7 = (NumberNode)node;
                    break;
                default:
                    break;

            }

        }


    }

}
