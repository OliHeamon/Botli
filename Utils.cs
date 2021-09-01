using Google.Cloud.Translation.V2;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Botli
{
    public static class Utils
    {
        public static ThreadLocal<Random> Random { get; private set; }

        static Utils()
        {
            Random = new ThreadLocal<Random>();

            Random.Value = new Random(Environment.TickCount);
        }

        public static double NextDouble(this Random random, double min, double max) 
            => random.NextDouble() * (max - min) + min;

        public static T GetRandomElement<T>(this T[] array, Random random)
            => array[random.Next(array.Length)];

        public static string RemoveWhitespace(this string input)
            => new string(input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());

        public static string CapitaliseFirstLetter(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text[0].ToString().ToUpper() + text[1..];
        }

        public static string Translate(string text, string target, string source)
        {
            TranslationClient client = TranslationClient.CreateFromApiKey(Constants.TranslateAPIKey, TranslationModel.NeuralMachineTranslation);

            TranslationResult result;

            try
            {
                result = client.TranslateText(text, target, source);
            }
            catch
            {
                return "Invalid target/source language code - could not translate.";
            }

            client.Dispose();

            return $"[{source ?? result.DetectedSourceLanguage} -> {result.TargetLanguage}] {result.TranslatedText}";
        }

        public static IEnumerable<(string, EntityAnnotation)> InspectImages(IEnumerable<string> links)
        {
            ImageAnnotatorClientBuilder builder = new ImageAnnotatorClientBuilder
            {
                CredentialsPath = "credentials.json"
            };

            ImageAnnotatorClient client = builder.Build();

            foreach (string link in links)
            {
                Image image = Image.FetchFromUri(link);

                List<EntityAnnotation> annotations = client.DetectText(image).ToList();

                // The first entry in annotations is all the text (instead of just one element). If it's not English we want to yield it to the result.
                if (annotations.Count > 0 && annotations[0].Locale != LanguageCodes.English)
                {
                    yield return (link, annotations[0]);
                }
            }
        }
    }
}
