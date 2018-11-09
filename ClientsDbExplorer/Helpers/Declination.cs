namespace ClientsDbExplorer.Helpers
{
    public static class Declination
    {
        public static string GetDeclension(int number, string nominative, string genitive, string plural)
        {
            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                return plural;
            }

            var i = number % 10;
            switch (i)
            {
                case 1:
                    return nominative;
                case 2:
                case 3:
                case 4:
                    return genitive;
                default:
                    return plural;
            }
        }

        public static string GetClientDeclension(int number)
        {
            var nom = "клиента";
            var gen = "клиента";
            var pl = "клиентов";

            var str = $"{number} {GetDeclension(number, nom, gen, pl)}";

            return str;
        }
    }
}