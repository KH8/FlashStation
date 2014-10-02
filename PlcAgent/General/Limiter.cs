namespace _PlcAgent.General
{
    static class Limiter
    {
        public static double DoubleLimit(double value, double limit)
        {
            return value > limit ? value : limit;
        }
    }
}
