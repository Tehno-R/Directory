namespace MyStructures.LinkedList;

public class Node
{
    public Node(LinkedList par, Key key, Node? next = null, Node? prev = null)
    {
        Key = key;
        Next = next ?? this;
        Prev = prev ?? this;
        Parent = par;
    }

    public Key Key { get; set; }
    public Node Next { get; set; }
    public Node Prev { get; set; }
    public LinkedList Parent { get; set; }
}

public class LinkedList
{
    public Node? Head { get; private set; }
    public int Count { get; private set; }

    /// <summary>
    /// Return 1 if list is empty, otherwise 0.
    /// </summary>

    public LinkedList()
    {
        Head = null;
        Count = 0;
    }

    public bool Empty()
    {
        return Head == null;
    }

    public Key this[int index]
    {
        get
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative");
            }

            if (Count == 0)
            {
                throw new InvalidOperationException("List is empty");
            }

            index = index % Count;
            Node current = Head!;

            for (; index > 0; index--)
            {
                current = current.Next;
            }

            return current.Key;
        }
    }
    
    /// <summary>
    /// Just add <c>data</c> as new element at end of list.
    /// </summary>
    public void Add(Key data)
    {
        // if (data == null)
        // {
        //     throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        // }

        Node newNode = new Node(this, data);

        if (Head == null)
        {
            Head = newNode;
        }
        else
        {
            Node cur = Head;
            bool flag = false;
            do
            {
                if (cur.Key.Passport.CompareTo(newNode.Key.Passport) == 0) return; // Предметная область
                if (cur.Key.CompareTo(newNode.Key) > 0)
                {
                    flag = true;
                }

                cur = cur.Next;
            } while (cur != Head && !flag);

            newNode.Next = cur;
            newNode.Prev = cur.Prev;
            cur.Prev.Next = newNode;
            cur.Prev = newNode;
            
            if (cur == Head && flag) Head = newNode;
        }
        Count++;
    }

    public void Remove(Key key)
    {
        Node cur = Head!;
        bool flag = true;
        do
        {
            if (cur.Key.CompareTo(key) == 0) flag = false;
            else cur = cur.Next;
        } while (cur != Head && flag);

        if (!flag)
        {
            if (cur == Head)
            {
                if (Head.Next == Head)
                {
                    Head = null;
                }
                else
                {
                    cur.Next.Prev = cur.Prev;
                    cur.Prev.Next = cur.Next;
                    Head = cur.Next;
                }
            }
            else
            {
                cur.Next.Prev = cur.Prev;
                cur.Prev.Next = cur.Next;
            }
        }

        Count--;
    }

    public List<Key> GetAllKeys()
    {
        List<Key> keys = new List<Key>();
        
        Node? cur = Head;
        if (Head == null) return keys;

        do
        {
            keys.Add(cur!.Key);
            cur = cur.Next;
        } while (cur != Head);

        return keys;
    }

    public bool Find(Key key)
    {
        Node cur = Head;
        do
        {
            if (cur.Key.CompareTo(key) == 0) return true;
            cur = cur.Next;
        } while (cur != Head);

        return false;
    }
}