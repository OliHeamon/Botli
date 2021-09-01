using System;

namespace Botli.Modules.Maths
{
    public struct Function
    {
        public int RequiredArguments { get; private set; }

        public string PossibleCausesOfErrors { get; private set; }

        private readonly Func<double[], double> function;

        public Function(int requiredArguments, Func<double[], double> function, string possibleCausesOfErrors)
        {
            RequiredArguments = requiredArguments;

            PossibleCausesOfErrors = possibleCausesOfErrors;

            this.function = function;
        }

        public double Evaluate(double[] arguments) => function.Invoke(arguments);
    }
}
