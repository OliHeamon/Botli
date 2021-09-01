using System;
using System.Collections.Generic;
using System.Linq;

namespace Botli.Modules.Maths.Parsing
{
    public class Parser
    {
        public List<string> FunctionKeys => Functions.Keys.ToList();

        public Dictionary<string, Operator> Operators => new Dictionary<string, Operator>
        {
            ["^"] = new Operator(4, Associativity.Right, (a, b) => Math.Pow(a, b)),
            ["*"] = new Operator(3, Associativity.Left, (a, b) => a * b),
            ["/"] = new Operator(3, Associativity.Left, (a, b) => a / b),
            ["+"] = new Operator(2, Associativity.Left, (a, b) => a + b),
            ["-"] = new Operator(2, Associativity.Left, (a, b) => a - b)
        };

        public Dictionary<string, FunctionInfo> Functions => new Dictionary<string, FunctionInfo>()
        {
            ["floor"] = new FunctionInfo(new Function(1, arguments => Math.Floor(arguments[0]), "unknown"), "Finds the floor (round down) of the input.", "x"),

            ["ceiling"] = new FunctionInfo(new Function(1, arguments => Math.Ceiling(arguments[0]), "unknown"), "Finds the ceiling (round up) of the input.", "x"),

            ["round"] = new FunctionInfo(new Function(1, arguments => Math.Round(arguments[0]), "unknown"), "Rounds the input to the nearest integer.", "x"),

            ["exp"] = new FunctionInfo(new Function(1, arguments => Math.Exp(arguments[0]), "unknown"), "Finds e^x for the input x.", "x"),

            ["ln"] = new FunctionInfo(new Function(1, arguments => Math.Log(arguments[0]), "unknown"), "Finds the natural logarithm (base e) of the input x.", "x"),

            ["log"] = new FunctionInfo(new Function(1, arguments => Math.Log10(arguments[0]), "unknown"), "Finds the logarithm (base 10) of the input x.", "x"),

            ["sin"] = new FunctionInfo(new Function(1, arguments => Math.Sin(arguments[0]), "unknown"), "Finds the sine of the input x.", "x"),

            ["cos"] = new FunctionInfo(new Function(1, arguments => Math.Cos(arguments[0]), "unknown"), "Finds the cosine of the input x.", "x"),

            ["tan"] = new FunctionInfo(new Function(1, arguments => Math.Tan(arguments[0]), "unknown"), "Finds the tangent of the input x.", "x"),

            ["min"] = new FunctionInfo(new Function(2, arguments => Math.Min(arguments[0], arguments[1]), "unknown"), "Finds minimum of the two inputs.", "a", "b"),

            ["max"] = new FunctionInfo(new Function(2, arguments => Math.Max(arguments[0], arguments[1]), "unknown"), "Finds maximum of the two inputs.", "a", "b"),

            ["rand"] = new FunctionInfo(new Function(2, arguments => Utils.Random.Value.NextDouble(arguments[0], arguments[1]),
                "lower bound being greater than or equal to the upper bound"), "Finds a random value between the min and max.", "min", "max"),

            ["randint"] = new FunctionInfo(new Function(2, arguments => Utils.Random.Value.Next((int)arguments[0], (int)arguments[1]),
                "lower bound being greater than or equal to the upper bound"), "Finds a random integer between the min and max.", "min", "max")
        };

        public Queue<string> ShuntingYard(List<string> tokens, List<char> delimiters, out string error)
        {
            Queue<string> outputQueue = new Queue<string>();
            Stack<string> operatorStack = new Stack<string>();

            Dictionary<string, Operator> operators = Operators;
            Dictionary<string, FunctionInfo> functions = Functions;

            error = null;

            // Algorithm taken from https://en.wikipedia.org/wiki/Shunting-yard_algorithm.

            // While there are tokens to be read:
            foreach (string token in tokens)
            {
                // If token is a number, add to output queue.
                if (double.TryParse(token, out _))
                {
                    outputQueue.Enqueue(token);
                }
                // If token is a function, push to operator stack.
                else if (functions.ContainsKey(token))
                {
                    operatorStack.Push(token);
                }
                // If token is an operator, push to operator stack.
                else if (operators.ContainsKey(token))
                {
                    // While the top of the operator stack is an operator AND the operator at the top of the operator stack is not a left parenthesis
                    // AND (the operator at the top of the operator stack has greater precedence OR (the operator at the top of the operator stack has equal precedence and the token is left associative)),
                    // pop the operator stack into the output queue.
                    while (operatorStack.Count > 0 && operators.ContainsKey(operatorStack.Peek()) && operatorStack.Peek() != "("
                        && (operators[operatorStack.Peek()].Precedence > operators[token].Precedence ||
                        (operators[operatorStack.Peek()].Precedence == operators[token].Precedence && operators[token].Associativity == Associativity.Left)))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                }
                // If token is a left parenthesis, push the token onto the operator stack.
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                // If token is a right parenthesis:
                else if (token == ")")
                {
                    // While the top of the operator stack is not a left parenthesis, pop the operator stack into the output queue.
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.Count == 0)
                    {
                        error = "number of opening and closing parentheses are not equal";

                        return null;
                    }

                    // If the top of the operator stack is a left parenthesis, pop it and discard.
                    if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                    {
                        operatorStack.Pop();
                    }
                    // If the top of the operator stack is a function, pop it into the output queue.
                    if (operatorStack.Count > 0 && functions.ContainsKey(operatorStack.Peek()))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                }
                // If the token is not a delimiter, then it must be an unknown token.
                else if (!(char.TryParse(token, out char character) && delimiters.Contains(character)))
                {
                    error = $"unexpected token: {token}";

                    return null;
                }
            }

            // While there are still operators on the operator stack, pop them into the output queue.
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")")
                {
                    error = "number of opening and closing parentheses are not equal";
                }

                outputQueue.Enqueue(operatorStack.Pop());
            }

            return outputQueue;
        }
    }
}
