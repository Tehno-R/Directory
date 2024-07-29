namespace MyStructures.RBTree;

public enum Color
{
    Red,
    Black
}

public class NodeRbTree(
    MyDate key,
    NodeRbTree left,
    NodeRbTree right,
    NodeRbTree parent,
    int hashTableStartSize,
    Color nodeColor = Color.Black)
{
    public MyDate Key { get; set; } = key;
    public NodeRbTree Left { get; set; } = left;
    public NodeRbTree Right { get; set; } = right;
    public NodeRbTree Parent { get; set; } = parent;
    public HashTable.HashTable Data { get; set; } = new(hashTableStartSize);
    public Color NodeColor { get; set; } = nodeColor;


    // public static Node<T> Default => new Node<T>() { Number = 42, Text = "Default text" };
    
}

public class RbTree
{
    public NodeRbTree NilNodeRbTree { get; }
    public NodeRbTree Root { get; private set; }
    private readonly int _hashTableStartSize;

    public RbTree(int hashTableStartSize)
    {
        _hashTableStartSize = hashTableStartSize;
        NilNodeRbTree = new NodeRbTree(new MyDate(), null!, null!, null!, _hashTableStartSize); // '!' сдесь по просьбе комплилятора
        Root = NilNodeRbTree;
    }

    public bool Insert(Key key)
    {
        if (Root == NilNodeRbTree)
        {
            Root = new NodeRbTree(key.Date, NilNodeRbTree, NilNodeRbTree, NilNodeRbTree, _hashTableStartSize);
            Root.Parent = NilNodeRbTree;
            Root.Data.Insert(key); //list->insertList(numStr);
            return true;
        }

        NodeRbTree node = FindNode(key);
        if (node != NilNodeRbTree && node.Data.Find(key))
        {
            return false; // cout << "Такой элемент уже есть в хт\n";
        }
        else if (node != NilNodeRbTree)
        {
            return node.Data.Insert(key); // Добавлен новый елемент в хт по уже ключу в бдп
        }
        else
        {
            var parent = NilNodeRbTree;
            var current = Root;

            while (current != NilNodeRbTree)
            {
                parent = current;
                current = current.Key.CompareTo(key.Date) > 0 ? current.Left : current.Right;
            }

            node = new NodeRbTree(key.Date, NilNodeRbTree, NilNodeRbTree, NilNodeRbTree,
                _hashTableStartSize, Color.Red);
            node.Data.Insert(key);

            node.Parent = parent;
            if (parent == NilNodeRbTree)
                Root = node;
            else if (parent.Key.CompareTo(key.Date) > 0)
                parent.Left = node;
            else
                parent.Right = node;

            InsertFixUp(node);
        }
        return true;
    }
    private void RotateToLeft(NodeRbTree node)
    {
        NodeRbTree rotate = node.Right;
        node.Right = rotate.Left;

        if (rotate.Left != NilNodeRbTree)
            rotate.Left.Parent = node;

        rotate.Parent = node.Parent;
        if (node.Parent == NilNodeRbTree)
            Root = rotate;
        else if (node == node.Parent.Left)
            node.Parent.Left = rotate;
        else
            node.Parent.Right = rotate;

        rotate.Left = node;
        node.Parent = rotate;
    }
    private void RotateToRight(NodeRbTree node)
    {
        NodeRbTree rotate = node.Left;
        node.Left = rotate.Right;

        if (rotate.Right != NilNodeRbTree)
            rotate.Right.Parent = node;

        rotate.Parent = node.Parent;

        if (node.Parent == NilNodeRbTree) // меняем указатель родителя
            Root = rotate;
        else if (node == node.Parent.Left)
            node.Parent.Left = rotate;
        else
            node.Parent.Right = rotate;

        rotate.Right = node;
        node.Parent = rotate;
    }
    private void InsertFixUp(NodeRbTree current)
    {
        while (current.Parent.NodeColor == Color.Red)
            if (current.Parent == current.Parent.Parent.Left)
            {
                // папа левый
                NodeRbTree uncle = current.Parent.Parent.Right;
                if (uncle.NodeColor == Color.Red) // Если дядя красный, меняем цвет родителя и дяди на чёрный
                {
                    uncle.NodeColor = Color.Black;
                    current.Parent.NodeColor = Color.Black;
                    current.Parent.Parent.NodeColor = Color.Red; // деда в красный
                    current = current.Parent.Parent; // текущий становится дед
                }
                else
                {
                    // дядя чёрный
                    if (current == current.Parent.Right)
                    {
                        // если мы правый сын
                        current = current.Parent; // указываем теперь на родителя
                        RotateToLeft(current); // делаем левый поворот
                    }

                    current.Parent.NodeColor = Color.Black; // меняем цвет нашего родителя на чёрный
                    current.Parent.Parent.NodeColor = Color.Red; // увет деда меняем на красный
                    RotateToRight(current.Parent.Parent); // поворот вправо относительно деда
                }
            }
            else // папа правый
            {
                NodeRbTree uncle = current.Parent.Parent.Left;
                if (uncle.NodeColor == Color.Red)
                {
                    uncle.NodeColor = Color.Black;
                    current.Parent.NodeColor = Color.Black;
                    current.Parent.Parent.NodeColor = Color.Red;
                    current = current.Parent.Parent;
                }
                else
                {
                    if (current == current.Parent.Left)
                    {
                        current = current.Parent;
                        RotateToRight(current);
                    }
                    current.Parent.NodeColor = Color.Black;
                    current.Parent.Parent.NodeColor = Color.Red;
                    RotateToLeft(current.Parent.Parent);
                }
            }
        Root.NodeColor = Color.Black;
    }
    private NodeRbTree MaxLeft(NodeRbTree nodeRbTree)
    {
        while (nodeRbTree.Right != NilNodeRbTree)
        {
            nodeRbTree = nodeRbTree.Right;
        }
        return nodeRbTree;
    }
    public bool Delete(Key key)
    {
        NodeRbTree foundNode = FindNode(key);

        bool keyInHt = foundNode.Data.Find(key);
        if (foundNode == NilNodeRbTree || (foundNode != NilNodeRbTree && !keyInHt)) return false; // ненайдена такая нода

        if (foundNode.Parent == NilNodeRbTree && foundNode.Left == NilNodeRbTree && foundNode.Right == NilNodeRbTree) // есди единственный узел в дереве
        {
            foundNode.Data.Delete(key);
            if (foundNode.Data.Empty())
            {
                Root = NilNodeRbTree;
            }

            return true;
        }

        NodeRbTree current = foundNode;
        Color origColor = current.NodeColor;
        NodeRbTree x;
        if (foundNode.Left == NilNodeRbTree)
        {
            foundNode.Data.Delete(key);
            if (foundNode.Data.Empty())
            {
                x = foundNode.Right;
                Transplant(foundNode, foundNode.Right);
            }
            else
            {
                return true; // tip warning
            }
        }
        else if (foundNode.Right == NilNodeRbTree)
        {
            foundNode.Data.Delete(key);
            if (foundNode.Data.Empty())
            {
                x = foundNode.Left;
                Transplant(foundNode, foundNode.Left);
            }
            else
            {
                return true; // tip warning
            }
        }
        else
        {
            foundNode.Data.Delete(key);
            if (foundNode.Data.Empty())
            {
                current = MaxLeft(foundNode.Left); // находим максимальный слева
                origColor = current.NodeColor;
                x = current.Left;
                if (current.Parent == foundNode)
                {
                    x.Parent = current;
                }
                else
                {
                    Transplant(current, current.Left);
                    current.Left = foundNode.Left;
                    current.Left.Parent= current;
                }
                Transplant(foundNode, current);
                current.Right = foundNode.Right;
                current.Right.Parent = current;
                current.NodeColor = foundNode.NodeColor;
            }
            else
            {
                return true;
            }
        }
        if (origColor == Color.Black)
        {
            DeleteFixUp(x);
        }

        // if (foundNode == NilNodeRbTree) return false;
        // if (foundNode.Left == NilNodeRbTree && foundNode.Right == NilNodeRbTree) // Нет потомков
        // {
        //     bool isBlack = foundNode.NodeColor == Color.Black;
        //     if (isBlack)
        //     {
        //         DeleteFixUp(foundNode);
        //     }
        //     if (foundNode.Key.CompareTo(foundNode.Parent.Key) < 0) foundNode.Parent.Left = NilNodeRbTree;
        //     else foundNode.Right = NilNodeRbTree;
        // }
        // else if ((foundNode.Left != NilNodeRbTree && foundNode.Right == NilNodeRbTree) ||  
        //     (foundNode.Left == NilNodeRbTree && foundNode.Right != NilNodeRbTree)) // Один потомок
        // {
        //     Node_RBTree replace;
        //     if (foundNode.Left != NilNodeRbTree) replace = foundNode.Left;
        //     else replace = foundNode.Right;
        //     foundNode.Key = replace.Key;
        //     foundNode.Data = replace.Data;
        //     if (replace.NodeColor == Color.Black)
        //     {
        //
        //         DeleteFixUp(foundNode);
        //     }
        //     if (replace.Key.CompareTo(foundNode.Key) < 0) foundNode.Left = NilNodeRbTree;
        //     else foundNode.Right = NilNodeRbTree;
        // }
        // else
        // {
        //     Node_RBTree newNode = MaxLeft(foundNode.Left);
        //     foundNode.Key = newNode.Key;
        //     foundNode.Data = newNode.Data;
        //     Delete(newNode);
        // }
        //
        // return true;
        return true;
    }
    private void Transplant(NodeRbTree first, NodeRbTree second)
    {
        if (first.Parent == NilNodeRbTree)
            Root = second;
        else if (first == first.Parent.Left)
            first.Parent.Left = second;
        else
            first.Parent.Right = second;

        second.Parent = first.Parent;
    }

    // private bool Delete(Node_RBTree foundNode)
    // {
    //     if (foundNode == NilNodeRbTree) return false;
    //     if (foundNode.Left == NilNodeRbTree && foundNode.Right == NilNodeRbTree) // Нет потомков
    //     {
    //         bool isBlack = foundNode.NodeColor == Color.Black;
    //         if (isBlack)
    //         {
    //             DeleteFixUp(foundNode);
    //         }
    //         if (foundNode.Key.CompareTo(foundNode.Parent.Key) < 0) foundNode.Parent.Left = NilNodeRbTree;
    //         else foundNode.Right = NilNodeRbTree;
    //     }
    //     else if (foundNode.Left != NilNodeRbTree || foundNode.Right != NilNodeRbTree) // Один потомок
    //     {
    //         Node_RBTree replace;
    //         if (foundNode.Left != NilNodeRbTree) replace = foundNode.Left;
    //         else replace = foundNode.Right;
    //         foundNode.Key = replace.Key;
    //         foundNode.Data = replace.Data;
    //         if (replace.NodeColor == Color.Black)
    //         {
    //             if (replace.Key.CompareTo(foundNode.Key) < 0) foundNode.Left = NilNodeRbTree;
    //             else foundNode.Right = NilNodeRbTree;
    //             DeleteFixUp(foundNode);
    //         }
    //     }
    //     else
    //     {
    //         Node_RBTree newNode = MaxLeft(foundNode.Left);
    //         foundNode.Key = newNode.Key;
    //         foundNode.Data = newNode.Data;
    //         Delete(newNode);
    //     }
    //
    //     return true;
    // }

    private void DeleteFixUp(NodeRbTree node)
    {
        while (node != Root && node.NodeColor == Color.Black)
        {
            if (node == node.Parent.Left)
            {                                           // мы слева
                NodeRbTree brother = node.Parent.Right;// брат справа

                if (brother.NodeColor == Color.Red)
                {                                     // брат красный
                    brother.NodeColor = Color.Black;    // меняем цвет брата
                    node.Parent.NodeColor = Color.Red; // и меняем цвет папы
                    RotateToLeft(node.Parent);   // делаем левый поворот относительно папы
                    brother = node.Parent.Right;    // меняем указатель брата
                }

                if (brother.Left.NodeColor == Color.Black && brother.Right.NodeColor == Color.Black)
                {                                // если дети брата чёрные
                    brother.NodeColor = Color.Red; // брат становится красным
                    node = node.Parent;         // указатель смещаем на родителя
                }
                else
                {
                    if (brother.Right.NodeColor == Color.Black)
                    {                                        // внешний(правый) чёрный
                        brother.Left.NodeColor = Color.Black; // цвет внутреннего потомка становится чёрным
                        brother.NodeColor = Color.Red;         // цвет брата становится красным
                        RotateToRight(brother);          // делаем поворот вправо относительно брата
                        brother = node.Parent.Right;       // смещаем указатель брата
                    }
                    brother.NodeColor = node.Parent.NodeColor; // меняем цвет брата в цвет папы
                    node.Parent.NodeColor = Color.Black;   // папа становится чёрным
                    brother.Right.NodeColor = Color.Black; // внешний(правый) сын становится чёрным
                    RotateToLeft(node.Parent);       // левый поворот относительно папы
                    node = Root;
                }
            }
            else
            {
                NodeRbTree brother = node.Parent.Left;
                if (brother.NodeColor == Color.Red)
                {
                    brother.NodeColor = Color.Black;
                    node.Parent.NodeColor = Color.Red;
                    RotateToRight(node.Parent);
                    brother = node.Parent.Left;
                }

                if (brother.Left.NodeColor == Color.Black && brother.Right.NodeColor == Color.Black)
                {
                    brother.NodeColor = Color.Red;
                    node = node.Parent;
                }
                else
                {
                    if (brother.Left.NodeColor == Color.Black)
                    {
                        brother.Right.NodeColor = Color.Black;
                        brother.NodeColor = Color.Red;
                        RotateToLeft(brother);
                        brother = node.Parent.Left;
                    }

                    brother.NodeColor = node.Parent.NodeColor;
                    node.Parent.NodeColor = Color.Black;
                    brother.Left.NodeColor = Color.Black;
                    RotateToRight(node.Parent);
                    node = Root;
                }
            }
        }
        node.NodeColor = Color.Black;
    }
    private NodeRbTree FindNode(Key key)
    {
        NodeRbTree current = Root;
        while (current != NilNodeRbTree && current.Key.CompareTo(key.Date) != 0)
        {
            current = current.Key.CompareTo(key.Date) > 0 ? current.Left : current.Right;
        }
        return current;
    }
    public List<Key> Find(Key from, Key to)
    {
        NodeRbTree cur = Root;

        while (!(cur.Key.CompareTo(from.Date) >= 0 && cur.Key.CompareTo(to.Date) <= 0 && 
               cur != NilNodeRbTree))
        {
            cur = cur.Key.CompareTo(from.Date) < 0 ? cur.Right : cur.Left;
        }

        List<Key> keys = new List<Key>();
        if (cur != NilNodeRbTree)
        {
            keys.Add(cur.Data.FindKey(from));
            RecFind(from, to, cur, keys);
        }

        return keys;
    }

    private void RecFind(Key from, Key to, NodeRbTree cur, List<Key> keys)
    {
        if (cur.Left != NilNodeRbTree)
        {
            if (cur.Left.Key.CompareTo(from.Date) >= 0)
            {
                keys.Add(cur.Left.Data.FindKey(from));
                RecFind(from, to, cur.Left, keys);
            }
            else
            {
                NodeRbTree iter = cur.Left.Right;
                while (iter.Right != NilNodeRbTree && iter.Key.CompareTo(from.Date) >= 0)
                {
                    iter = iter.Right;
                }

                if (iter != NilNodeRbTree)
                {
                    keys.Add(iter.Data.FindKey(from));
                    RecFind(from, to, iter, keys);
                }
            }
        }
        if (cur.Right != NilNodeRbTree)
        {
            if (cur.Right.Key.CompareTo(to.Date) <= 0)
            {
                keys.Add(cur.Right.Data.FindKey(to));
                RecFind(from, to, cur.Right, keys);
            }
            else
            {
                NodeRbTree iter = cur.Right.Left;
                while (iter.Left != NilNodeRbTree && iter.Key.CompareTo(to.Date) <= 0)
                {
                    iter = iter.Left;
                }

                if (iter != NilNodeRbTree)
                {
                    keys.Add(iter.Data.FindKey(from));
                    RecFind(from, to, iter, keys);
                }
            }
        }
    }

    // public List<T> Print()
    // {
    //     List<T> objects = new List<T>();
    //     Print(_root, objects);
    //     return objects;
    // }
    //
    // private void Print(Node node, List<T> objects)
    // {
    //     if (node == _nilNode) return;
    //     Print(node.Left, objects);
    //     objects.AddRange(node.Data);
    //     Print(node.Right, objects);
    // }

    public List<Key> GetAllKeys()
    {
        List<Key> keys = [];
        
        List<HashTable.HashTable> hashTables = InOrderTraversal();
        foreach (var hashTable in hashTables)
        {
            keys.AddRange(hashTable.GetAllKeys());
        }

        return keys;
    }

    private List<HashTable.HashTable> InOrderTraversal()
    {
        List<HashTable.HashTable> keys = new List<HashTable.HashTable>();
        InOrderTraversal(Root, keys);
        return keys;
    }

    private void InOrderTraversal(NodeRbTree nodeRbTree, List<HashTable.HashTable> keys)
    {
        if (nodeRbTree == NilNodeRbTree) return;
        InOrderTraversal(nodeRbTree.Left, keys);
        keys.Add(nodeRbTree.Data);
        InOrderTraversal(nodeRbTree.Right, keys);
    }
}
