using Botli.Modules.Maths.Parsing;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Botli.Modules.Maths
{
    [Group("math")]
    public class MathsModule : ModuleBase<SocketCommandContext>
    {
        private readonly Lexer lexer;

        private readonly Parser parser;

        private readonly Evaluator evaluator;

        public MathsModule()
        {
            lexer = new Lexer();
            parser = new Parser();
            evaluator = new Evaluator();
        }

        [Command]
        [Summary("Computes the result of the input mathematical expression.")]
        public Task Evaluate(int decimalPlaces, [Remainder] string expression)
        {
            List<string> tokens = lexer.Tokenise(expression.RemoveWhitespace(), parser.FunctionKeys);
            List<char> delimiters = lexer.Delimiters;

            Queue<string> outputQueue = parser.ShuntingYard(tokens, delimiters, out string error);

            if (error != null)
            {
                return ReplyAsync($"Error: {error}.");
            }

            double result = evaluator.EvaluateReversePolishNotation(parser.Operators, parser.Functions, outputQueue, out error);

            if (error != null)
            {
                return ReplyAsync($"Error: {error}.");
            }

            string roundedResult = result.ToString($"N{decimalPlaces}");

            return ReplyAsync($"Expression evaluation: {roundedResult}.");
        }

        [Command("functions")]
        [Summary("Lists all the currently available functions for the math command.")]
        public Task ListFunctions()
        {
            EmbedBuilder builder = new EmbedBuilder();

            Dictionary<string, FunctionInfo> functions = parser.Functions;

            foreach (string key in functions.Keys)
            {
                FunctionInfo functionInfo = functions[key];

                string info = functionInfo.Info ?? "Summary not found.\n";

                string parameters = "";

                for (int i = 0; i < functionInfo.ParameterInfo.Length; i++)
                {
                    string parameterInfo = functionInfo.ParameterInfo[i];

                    parameters += $"{parameterInfo}{(i < functionInfo.ParameterInfo.Length - 1 ? ", " : "")}";
                }

                builder.AddField($"{key}({parameters})", info, true);
            }

            builder
                .WithTitle($"Available math functions (for use with the math command):")
                .WithColor(Constants.EmbedBlue);

            return ReplyAsync(embed: builder.Build());
        }
    }
}
