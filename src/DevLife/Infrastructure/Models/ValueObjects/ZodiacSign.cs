namespace DevLife.Infrastructure.Models.ValueObjects;

public class ZodiacSign : ValueObject
{
    public string Name { get; set; }

    public ZodiacSign(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
    
    public static ZodiacSign FromDate(DateOnly birthDate)
    {
        var day = birthDate.Day;
        var month = birthDate.Month;
        
        //pattern strategy-s xom ver gamoviyenebdi, momitevet amdeni if 
        if ((month == 3 && day >= 21) || (month == 4 && day <= 19))
            return new ZodiacSign("Aries");
        if ((month == 4 && day >= 20) || (month == 5 && day <= 20))
            return new ZodiacSign("Taurus");
        if ((month == 5 && day >= 21) || (month == 6 && day <= 20))
            return new ZodiacSign("Gemini");
        if ((month == 6 && day >= 21) || (month == 7 && day <= 22))
            return new ZodiacSign("Cancer");
        if ((month == 7 && day >= 23) || (month == 8 && day <= 22))
            return new ZodiacSign("Leo");
        if ((month == 8 && day >= 23) || (month == 9 && day <= 22))
            return new ZodiacSign("Virgo");
        if ((month == 9 && day >= 23) || (month == 10 && day <= 22))
            return new ZodiacSign("Libra");
        if ((month == 10 && day >= 23) || (month == 11 && day <= 21))
            return new ZodiacSign("Scorpio");
        if ((month == 11 && day >= 22) || (month == 12 && day <= 21))
            return new ZodiacSign("Sagittarius");
        if ((month == 12 && day >= 22) || (month == 1 && day <= 19))
            return new ZodiacSign("Capricorn");
        if ((month == 1 && day >= 20) || (month == 2 && day <= 18))
            return new ZodiacSign("Aquarius");
        if ((month == 2 && day >= 19) || (month == 3 && day <= 20))
            return new ZodiacSign("Pisces");

        throw new ArgumentException("Invalid date for zodiac sign");
    }
    
    public string ToString() => Name;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}