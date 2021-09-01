using System;

namespace Botli.Modules.Maths
{
    public struct Operator
    {
        public int Precedence { get; private set; }

        public Associativity Associativity { get; private set; }

        private readonly Func<double, double, double> function;

        public Operator(int precedence, Associativity associativity, Func<double, double, double> function)
        {
            Precedence = precedence;
            Associativity = associativity;

            this.function = function;
        }

        public double Evaluate(double a, double b)
            => function.Invoke(a, b);
    }
}
