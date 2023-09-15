using Scradic.Core.Resources;

namespace Scradic.Services.Utils
{
    public class ErrorMessage
    {
        public static void NoWordsAvailable()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(Globals.Warning);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.NoWordsAvailable);
            Console.ResetColor();
        }

        public static void Syntax()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(Globals.Warning);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.SyntaxError);
            Console.ResetColor();
        }
    }
}
