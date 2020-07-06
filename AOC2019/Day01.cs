namespace AOC2019
{
    public static class Day01
    {
        public static int fuelFor(int mass) =>
            mass / 3 - 2;


        public static int correctFuelFor(int mass)
        {
            if (mass <= 0)
                return 0;
            
            var x = fuelFor(mass);
            return x > 0 ? x + correctFuelFor(x) : 0;
        }
        
    }
}
