using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
