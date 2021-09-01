using System;
using System.Collections.Generic;

namespace Botli.Modules.Maths.Parsing
{
    public class Evaluator
    {
        public double EvaluateReversePolishNotation(Dictionary<string, Operator> operators, Dictionary<string, FunctionInfo> functions, Queue<string> outputQueue, out string error)
        {
            Stack<double> evaluationStack = new Stack<double>();

            error = null;

            while (outputQueue.Count > 0)
            {
                string item = outputQueue.Dequeue();

                if (double.TryParse(item, out double value))
                {
                    evaluationStack.Push(value);
                }
                else if (functions.ContainsKey(item))
                {
                    Function function = functions[item].Function;

                    try
                    {
                        double[] arguments = new double[function.RequiredArguments];

                        if (evaluationStack.Count < arguments.Length)
                        {
                            throw new IndexOutOfRangeException();
                        }

                        for (int i = 0; i < arguments.Length; i++)
                        {
                            arguments[arguments.Length - 1 - i] = evaluationStack.Pop();
                        }

                        evaluationStack.Push(function.Evaluate(arguments));
                    }
                    catch (Exception exception)
                    {
                        if (exception is IndexOutOfRangeException)
                        {
                            error = "detected incorrect number of function arguments";
                        }
                        else
                        {
                            error = $"probable cause is {function.PossibleCausesOfErrors}";
                        }

                        return 0;
                    }
                }
                else if (operators.ContainsKey(item))
                {
                    if (evaluationStack.Count < 2)
                    {
                        error = $"operator {item} does not have the correct number of operands";

                        return 0;
                    }

                    Operator operatorType = operators[item];

                    double b = evaluationStack.Pop();
                    double a = evaluationStack.Pop();

                    evaluationStack.Push(operatorType.Evaluate(a, b));
                }
            }

            return evaluationStack.Pop();
        }
    }
}
