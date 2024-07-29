namespace wpfApp;
using System.Text.RegularExpressions;
public static class Checker
{
    public static bool IsValidPassport(string passport)
    {
        // Регулярное выражение для проверки формата паспорта
        string pattern = @"^\d{4} \d{6}$";
        return Regex.IsMatch(passport, pattern);
    }
    public static bool IsValidName(string name)
    {
        // Регулярное выражение для проверки корректности имени (только буквы)
        string pattern = @"^[А-ЯЁ][а-яё]+(?:-[А-ЯЁ][а-яё]+)?$";
        return Regex.IsMatch(name, pattern);
    }
    public static bool IsValidLicensePlate(string licensePlate)
    {
        // Регулярное выражение для проверки корректности гос. номера (например: A123BC)
        string pattern = @"^[A-Z]{1}\d{3}[A-Z]{2}$";
        return Regex.IsMatch(licensePlate, pattern);
    }
    public static bool IsValidDate(string? date)
    {
        // Проверка, что дата выбрана и находится в разумном диапазоне
        if (date == null)
        {
            return false;
        }

        string[] dateSep = date.Split('.');
        DateTime curDate;
        try
        {
            int day = int.Parse(dateSep[0]);
            int mounth = int.Parse(dateSep[1]);
            int year = int.Parse(dateSep[2]);

            curDate = new DateTime(year, mounth, day);
        }
        catch (Exception e)
        {
            return false;
        }
        
        DateTime minDate = new DateTime(1900, 1, 1);
        DateTime maxDate = new DateTime(9999, 1, 1);

        return curDate >= minDate && curDate <= maxDate;
    }
}