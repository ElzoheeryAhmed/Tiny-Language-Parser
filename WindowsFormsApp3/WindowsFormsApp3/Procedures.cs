using System;
using System.Collections.Generic;

namespace WindowsFormsApp3
{
    abstract class Node
    {
        internal Node next;
        Node Parent;
        protected string type;
        int x = 0, y = 0;
        public List<Node> child = new List<Node>();
        public Node parent
        {
            get { return Parent; }
            set { Parent = value; }
        }
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
        protected string value;
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
    class Procedures
    {

        string tokent;
        string tokenv;
        int i;
        List<string> lt;
        List<string> lv;
        int status;
        public List<Node> graph;

        public Procedures(List<String> listt, List<String> listv)
        {
            i = 0;
            status = 1;
            lt = listt;
            lv = listv;
            graph = new List<Node>() ;
            tokent = lt[i];
            tokenv = lv[i];
        }

        public int Status
        {
            get { return status; }
        }

        public Node program()
        {
            try
            {
                Node n = stmt_sequence();
                if (n != null)
                {
                    n.X = 0; n.Y = 0;
                    //inOrderPositionSetting(n);
                }
                return n;

            }
            catch (Exception e) { status = 0; return null; }

        }


        public Node stmt_sequence()
        {
            Node temp = statement();
            Node temp1 = temp;

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
            {
                case "IF":
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
            IfNode temp = new IfNode();
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
            RepeatNode temp = new RepeatNode();
            match("REPEAT");
            temp.Body = stmt_sequence();
            match("UNTIL");
            temp.Test = exp();

            return temp;
        }

        public void match(string expected)
        {
            if (i + 1 < lt.Count)
            {
                if ((tokent == expected)) { i++; tokent = lt[i]; tokenv = lv[i]; } //pop token
                else { throw new Exception(); }
            }
            else { if ((tokent == expected) && (i < lt.Count)) { i++; } else { throw new Exception(); } }
        }

        public Node assign_stmt()
        {

            AssignNode temp = new AssignNode();
            temp.Value = tokenv;
            match("IDENTIFIER");
            match("ASSIGN");
            temp.Exp = exp();

            return temp;
        }

        public Node read_stmt()
        {
            ReadNode temp = new ReadNode();
            match("READ");
            temp.Value = tokenv;
            match("IDENTIFIER");

            return temp;

        }

        public Node write_stmt()
        {
            WriteNode temp = new WriteNode();
            match("WRITE");
            temp.Exp = exp();

            return temp;
        }

        public Node exp()
        {
            Node temp = simple_exp();


            if (tokent == "LESSTHAN" || tokent == "EQUAL" || tokent == "GREATERTHAN")
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


            OpNode temp1;
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

            Node temp;

            if (tokent == "NUMBER") { temp = new NumberNode(); ((NumberNode)temp).Value = tokenv; match(tokent); return temp; }
            else if (tokent == "IDENTIFIER") { temp = new IdentifierNode(); ((IdentifierNode)temp).Value = tokenv; match(tokent); return temp; }
            else
            {
                match("OPENBRACKET");
                temp = exp();
                match("CLOSEDBRACKET");
                return temp;
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
                    inOrderConstantFolding(ref (n.then));
                    inOrderConstantFolding(ref (n.els));
                    inOrderConstantFolding(ref (n.next));
                    break;
                case "repeat":
                    RepeatNode n1 = (RepeatNode)node;
                    inOrderConstantFolding(ref (n1.body));
                    inOrderConstantFolding(ref (n1.test));
                    inOrderConstantFolding(ref (n1.next));
                    break;
                case "write":
                    WriteNode n2 = (WriteNode)node;
                    inOrderConstantFolding(ref (n2.exp));
                    inOrderConstantFolding(ref (n2.next));
                    break;
                case "assign":
                    AssignNode n3 = (AssignNode)node;
                    inOrderConstantFolding(ref (n3.exp));
                    inOrderConstantFolding(ref (n3.next));
                    break;
                case "read":
                    ReadNode n4 = (ReadNode)node;
                    inOrderConstantFolding(ref (n4.next));
                    break;
                case "op":
                    OpNode n5 = (OpNode)node;
                    inOrderConstantFolding(ref (n5.expl));
                    inOrderConstantFolding(ref (n5.expr));
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

        public void inOrderPositionSetting(Node node)
        {
            if (node == null) return;

            switch (node.Type)
            {
                case "if":
                    IfNode n = (IfNode)node;
                    graph.Add(n);
                    if (n.Test != null)
                    {
                        (n.Test).X = n.X ;
                        (n.Test).Y = n.Y + 1;
                        n.child.Add(n.Test);
                        n.Test.parent = n;
                    }
                    inOrderPositionSetting(n.Test);

                    if (n.Then != null)
                    {
                        (n.Then).X = n.X +1;
                        (n.Then).Y = n.Y + 1;
                        n.child.Add(n.Then);
                        n.Then.parent = n;
                    }
                    inOrderPositionSetting(n.Then);

                    if (n.Else != null)
                    {
                        (n.Else).X = n.X + 2;
                        (n.Else).Y = n.Y + 1;
                        n.child.Add(n.Else);
                        n.Else.parent = n;
                    }
                    inOrderPositionSetting(n.Else);

                    if (n.Next != null)
                    {
                        (n.Next).X = n.X + 2;
                        (n.Next).Y = n.Y;
                        n.Next.parent = n.parent;
                    }
                    
                    inOrderPositionSetting(n.Next);
                    break;
                case "repeat":
                    RepeatNode n1 = (RepeatNode)node;
                    graph.Add(n1);

                    if (n1.Body != null)
                    {
                        (n1.Body).X = n1.X - 1;
                        (n1.Body).Y = n1.Y + 1;
                        n1.child.Add(n1.Body);
                        n1.Body.parent = n1;
                    }
                    inOrderPositionSetting(n1.Body);

                    if (n1.Test != null)
                    {
                        (n1.Test).X = n1.X + 1;
                        (n1.Test).Y = n1.Y + 1;
                        n1.child.Add(n1.Test);
                        n1.Test.parent = n1;
                    }
                    inOrderPositionSetting(n1.Test);

                    if (n1.Next != null) { n1.Next.parent = n1.parent; (n1.Next).X = n1.X + 1; (n1.Next).Y = n1.Y; }
                    inOrderPositionSetting(n1.Next);
                    break;
                case "write":
                    WriteNode n2 = (WriteNode)node;
                    graph.Add(n2);

                    if (n2.Exp != null)
                    {
                        (n2.Exp).X = n2.X;
                        (n2.Exp).Y = n2.Y + 1;
                        n2.child.Add(n2.Exp);
                        n2.Exp.parent = n2;
                    }
                    inOrderPositionSetting(n2.Exp);

                    if (n2.Next != null) { n2.Next.parent = n2.parent; (n2.Next).X = n2.X + 1; (n2.Next).Y = n2.Y; }
                    inOrderPositionSetting(n2.Next);
                    break;
                case "assign":
                    AssignNode n3 = (AssignNode)node;
                    graph.Add(n3);

                    if (n3.Exp != null)
                    {
                        (n3.Exp).X = n3.X;
                        (n3.Exp).Y = n3.Y + 1;
                        n3.child.Add(n3.Exp);
                        n3.Exp.parent = n3;
                    }
                    inOrderPositionSetting(n3.Exp);

                    if (n3.Next != null) { n3.Next.parent = n3.parent; (n3.Next).X = n3.X + 1; (n3.Next).Y = n3.Y; }
                    inOrderPositionSetting(n3.Next);
                    break;
                case "read":
                    ReadNode n4 = (ReadNode)node;
                    this.graph.Add(n4);

                    if (n4.Next != null) { n4.Next.parent = n4.parent; (n4.Next).X = n4.X + 1; (n4.Next).Y = n4.Y; }
                    inOrderPositionSetting(n4.Next);
                    break;
                case "op":
                    OpNode n5 = (OpNode)node;
                    graph.Add(n5);

                    if (n5.Expl != null)
                    {
                        (n5.Expl).X = n5.X ;
                        (n5.Expl).Y = n5.Y + 1;
                        n5.child.Add(n5.Expl);
                        n5.Expl.parent = n5;
                    }
                    inOrderPositionSetting(n5.Expl);

                    if (n5.Expr != null)
                    {
                        (n5.Expr).X = n5.X+1;
                        (n5.Expr).Y = n5.Y + 1;
                        n5.child.Add(n5.Expr);
                        n5.Expr.parent = n5;
                    }
                    inOrderPositionSetting(n5.Expr);

                    if (n5.Next != null) { n5.Next.parent = n5.parent; (n5.Next).X = n5.X + 1; (n5.Next).Y = n5.Y; }

                    inOrderPositionSetting(n5.Next);
                    break;
                case "id":
                    IdentifierNode n6 = (IdentifierNode)node;
                    graph.Add(n6);
                    if (n6.Next != null) { n6.Next.parent = n6.parent; (n6.Next).X = n6.X + 1; (n6.Next).Y = n6.Y; }
                    inOrderPositionSetting(n6.Next);

                    break;
                case "const":
                    NumberNode n7 = (NumberNode)node;
                    graph.Add(n7);
                    if (n7.Next != null) { n7.Next.parent = n7.parent; (n7.Next).X = n7.X + 1; (n7.Next).Y = n7.Y; }
                    inOrderPositionSetting(n7.Next);
                    break;
                default:
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




    }
}