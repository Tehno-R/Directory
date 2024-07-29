using MyStructures.HashTable;
using MyStructures.RBTree;

namespace wpfApp;

public partial class AlterView
{
    private readonly NodeRbTree _nil;

    public AlterView()
    {
        InitializeComponent();
        _nil = MainWindow.Database.NilNodeRbTree;
        if (MainWindow.Database.Root == null || MainWindow.Database.Root == _nil)
            return;
        
        if (MainWindow.Database.Root == _nil) Close();
        else
        {
            string treeOutput = PrintTree(MainWindow.Database.Root);
            string hashtableOutput = PrintHashTables(MainWindow.Database.Root);
            FullSizeTextBox.Text = treeOutput + "\n\n" + hashtableOutput;    
        }
    }

    private string PrintTree(NodeRbTree cur, int k = 0, string textEdit = "")
    {
        if (cur.Right != _nil)
        {
            textEdit = PrintTree(cur.Right, k + 10, textEdit);
        }
        
        textEdit += new string(' ', k) + cur.NodeColor + "|" + cur.Key + '\n';
        // textEdit += new string(' ', k) + nodeRbTree.NodeColor + '\n';
        // var hashElem = nodeRbTree.Data.Elements;
        // for (int i = 0; i < hashElem.Length; i++)
        // {
        //     textEdit += new string(' ', k);
        //     if (hashElem[i].Status == Status.One)
        //     {
        //         if (!hashElem[i].Value!.Empty())
        //         {
        //             textEdit += $"{i}. ";
        //             var head = hashElem[i].Value!.GetHead();
        //             var cur = head;
        //             
        //             do
        //             {
        //                 textEdit += cur!.Key.ToString() + '|';
        //                 cur = cur.Next;
        //             } while (cur != head);    
        //         }
        //     }
        //     else
        //     {
        //         textEdit += "Empty";
        //     }
        //
        //     textEdit += '\n';
        // }
        
        if (cur.Left != _nil)
        {
            textEdit = PrintTree(cur.Left, k + 10, textEdit);
        }

        return textEdit;
    }
    private string PrintHashTables(NodeRbTree cur, string textEdit = "")
    {
        if (cur.Right != _nil)
        {
            textEdit = PrintHashTables(cur.Right, textEdit);
        }
        
        textEdit += cur.Key.ToString() + '\n';
        var hashElem = cur.Data.Elements;
        for (int i = 0; i < hashElem.Length; i++)
        {
            if (hashElem[i].Status == Status.One)
            {
                textEdit += $"{i}. ";
                var headNode = hashElem[i].Value!.Head;
                var curNode = headNode;
                do
                {
                    textEdit += curNode!.Key + " -> ";
                    curNode = curNode.Next;
                } while (curNode != headNode);
            }
            else
            {
                textEdit += "Empty";
            }
        
            textEdit += '\n';
        }
        textEdit += "\n\n";
        
        if (cur.Left != _nil)
        {
            textEdit = PrintHashTables(cur.Left, textEdit);
        }

        return textEdit;
    }
}