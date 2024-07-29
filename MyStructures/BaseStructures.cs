
namespace MyStructures;

public readonly struct Passport : IComparer<Passport>, IComparable<Passport>
{
    public Passport(string pass)
    {
        string[] passSplit = pass.Split(' ');
        Serie = int.Parse(passSplit[0]);
        Number = int.Parse(passSplit[1]);
    }
    // public Passport(int serie, int number)
    // {
    //     Serie = serie;
    //     Number = number;
    // }

    private int Serie { get; }
    private int Number { get; }


    public override string ToString()
    {
        if (Serie == 0 && Number == 0) return "0000 000000";
        int countZeros1 = 0;
        int countZeros2 = 0;
        int temp = Serie;
        
        while (temp < 1000 && countZeros1 < 4)
        {
            countZeros1 += 1;
            temp *= 10;
        }

        temp = Number;
        while (temp < 100000 && countZeros2 < 6)
        {
            countZeros2 += 1;
            temp *= 10;
        }
        
        return $"{new string('0', countZeros1)}{Serie.ToString()} {new string('0', countZeros2)}{Number.ToString()}";
    }

    public int Compare(Passport x, Passport y)
    {
        var serieComparison = x.Serie.CompareTo(y.Serie);
        if (serieComparison != 0) return serieComparison;
        return x.Number.CompareTo(y.Number);
    }

    public int CompareTo(Passport other)
    {
        var serieComparison = Serie.CompareTo(other.Serie);
        if (serieComparison != 0) return serieComparison;
        return Number.CompareTo(other.Number);
    }

    public int KeyNumber()
    {
        int sum = 0;
        foreach (var chr in Serie.ToString().ToCharArray())
        {
            sum += chr;
        }
        foreach (var chr in Number.ToString().ToCharArray())
        {
            sum += chr;
        }

        return sum;
    }
}
public readonly struct Fio(string f, string i, string o) : IComparer<Fio>, IComparable<Fio>
{
    private string Surname { get; } = f;
    private string Name { get; } = i;
    private string Patronymic { get; } = o;

    public override string ToString()
    {
        return $"{Surname} {Name} {Patronymic}";
    }

    public int Compare(Fio x, Fio y)
    {
        var surnameComparison = string.Compare(x.Surname, y.Surname, StringComparison.Ordinal);
        if (surnameComparison != 0) return surnameComparison;
        var nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        return string.Compare(x.Patronymic, y.Patronymic, StringComparison.Ordinal);
    }

    public int CompareTo(Fio other)
    {
        var surnameComparison = string.Compare(Surname, other.Surname, StringComparison.Ordinal);
        if (surnameComparison != 0) return surnameComparison;
        var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
        if (nameComparison != 0) return nameComparison;
        return string.Compare(Patronymic, other.Patronymic, StringComparison.Ordinal);
    }
}
public readonly struct GosNum(string gosNum)
{
    private string StrVal { get; } = $"{gosNum[0]}{gosNum[4]}{gosNum[5]}";
    private int NumVal { get; } = int.Parse($"{gosNum[1]}{gosNum[2]}{gosNum[3]}");

    public override string ToString()
    {
        return $"{StrVal[0]}{NumVal}{StrVal[1]}{StrVal[2]}";
    }
}
public readonly struct MyDate : IComparer<MyDate>, IComparable<MyDate>
{
    public MyDate() //overload struct default constructor
    {
        Day = 0;
        Month = 0;
        Year = 0;
    }
    public MyDate(string date)
    {
        string[] split = date.Split('.');
        Day = int.Parse(split[0]);
        Month = int.Parse(split[1]);
        Year = int.Parse(split[2]);
    }

    private int Day { get; }
    private int Month { get; }
    private int Year { get; }

    public override string ToString()
    {
        return $"{Day}.{Month}.{Year}";
    }
    public int Compare(MyDate x, MyDate y)
    {
        var dayComparison = x.Day.CompareTo(y.Day);
        if (dayComparison != 0) return dayComparison;
        var monthComparison = x.Month.CompareTo(y.Month);
        if (monthComparison != 0) return monthComparison;
        return x.Year.CompareTo(y.Year);
    }
    public int CompareTo(MyDate other)
    {
        var yearComparison = Year.CompareTo(other.Year);
        if (yearComparison != 0) return yearComparison;
        var mounthComparison = Month.CompareTo(other.Month);
        if (mounthComparison != 0) return mounthComparison;
        return Day.CompareTo(other.Day);
    }
}
public class Key(Passport pass = default, Fio fio = default, GosNum gn = default, MyDate date = default) : IComparable<Key>
{
    public Passport Passport { get; } = pass;
    public Fio Fio { get; } = fio;
    public GosNum GosNum { get; } = gn;
    public MyDate Date { get; } = date;

    public override string ToString()
    {
        return $"{Passport} {Fio} {GosNum} {Date}";
    }

    public int CompareTo(Key obj)
    {
        return Passport.CompareTo(obj.Passport);
    }

}