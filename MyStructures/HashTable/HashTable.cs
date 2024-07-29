using MyStructures.LinkedList;

namespace MyStructures.HashTable;

public enum Status 
{
    Zero,
    One,
    Two
}

public class Cell
{
    public Cell()
    {
        Key = default;
        Status = Status.Zero;
        Value = null;
    }
    public Cell(Passport key = default, Status status = Status.Zero, LinkedList.LinkedList? value = null)
    {
        Key = key;
        Status = status;
        Value = value ?? new LinkedList.LinkedList();
    }

    public Passport Key { get; set; }

    public Status Status { get; set; }

    public LinkedList.LinkedList? Value { get; set; }
}

public class HashTable
{
    public HashTable(int size, string filepath = " ")
    {
            Elements = new Cell[size];
            for (var i = 0; i < Elements.Length; i++)
            {
                Elements[i] = new Cell();
            }
    }

    public bool Insert(Key key)
    {
        var el = new Cell(key.Passport);
        var hash = HashFunc(el.Key);

        if (Elements[hash].Status == Status.Zero)
        {
            Elements[hash] = el;
            Elements[hash].Value!.Add(key);
            Elements[hash].Status = Status.One;
            _countElements++;
            RehashInsert();
            return true;
        }
        else if (Elements[hash].Status == Status.One)
        {
            Elements[hash].Value!.Add(key);
        }
            
        // hash = CollisionInserting(el, hash);
        // if (hash == -1) return;
        // if (hash == -2)
        // {
        //     Insert(el);
        //     return;
        // }
        // Elements[hash] = el;
        // Elements[hash].Status = Status.One;
        // _countElements++;
        // RehashInsert();
        return true;
    }
    public void Delete(Key key)
    {
        var hash = HashFunc(key.Passport);

        if (Elements[hash].Status == Status.One)
        {
            // Elements[hash].Status = Status.Two;
            Elements[hash].Value!.Remove(key);
            if (Elements[hash].Value!.Empty())
            {
                Elements[hash].Status = Status.Two;
                _countElements--;
            }
        }
    }
    private void Insert(Cell el) 
    {
        var hash = HashFunc(el.Key);

        if (Elements[hash].Status == Status.Zero)
        {
            Elements[hash] = el;
            Elements[hash].Status = Status.One;
            _countElements++;
            // RehashInsert();
            // return;
        }
    }
    public bool Find(Key key)
    {
        var hash = HashFunc(key.Passport);
        return Elements[hash].Status == Status.One && Elements[hash].Value.Find(key);
    }
    public Key FindKey(Key key)
    {
        int hash = HashFunc(key.Passport);
        if (Elements[hash].Status == Status.One)
        {
            Node head = Elements[hash].Value.Head;
            Node cur = head;
            do
            {
                if (cur.Key.Passport.CompareTo(key.Passport) == 0) return cur.Key;
                cur = cur.Next;
            } while (cur != head);
        }
        return null;
    }

    private void RehashInsert(bool force = false)
    {
        var loadFactor = _countElements/(double)Elements.Length;
        if (loadFactor > 0.7 || force)
        {
            Cell[] oldTable = Elements;
            var capacity = Elements.Length * 2;
            Elements = new Cell[capacity];
            for (var i = 0; i < Elements.Length; i++)
            {
                Elements[i] = new Cell();
            }
            _countElements = 0;
            foreach (var item in oldTable)
            {
                if (item.Status == Status.One)
                    Insert(item);
            }    
        }
    }

    private void RehashDelete()
    {
        var loadFactor = _countElements / (double)Elements.Length;
        if (!(loadFactor < 0.2)) return;
        Cell[] oldTable = Elements;
        var capacity = Elements.Length / 2;
        Elements = new Cell[capacity];
        for (var i = 0; i < Elements.Length; i++)
        {
            Elements[i] = new Cell();
        }
        _countElements = 0;
        foreach (var item in oldTable)
        {
            if(item.Status == Status.One)
                Insert(item);
        }
    }
        
    private int HashFunc(Passport key)
    { // hash function 1
        int num = key.KeyNumber();
        var k = 0.26f;
        var hash = (int)(Elements.Length * (num * k % 1));
        
        return hash;
    }

    // private int LinearSearch(int hash, int j)
    // { // hash function 2
    //     int k = 1;
    //     return (hash + j * k) % Elements.Length;
    // }

    public int CountElements { get; private set; } = 0;
    
    public Cell[] Elements { get; private set; }
    private int _countElements;

    public List<Key> GetAllKeys()
    {
        List<Key> keys = new List<Key>();
        foreach (var item in Elements)
        {
            if (item.Status == Status.One)
            {
                keys.AddRange(item.Value!.GetAllKeys());
            }
        }

        return keys;
    }

    public bool Empty()
    {
        foreach (var item in Elements)
        {
            if (item.Status == Status.One) return false;
        }
        return true;
    }
}