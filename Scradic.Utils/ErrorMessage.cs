using Scradic.Utils.Resources;

namespace Scradic.Services.Utils
{
    public class ErrorMessage
    {
        public static void NoWordsAvailable()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.NoWordsAvailable);
            Console.ResetColor();
        }

        public static void SyntaxSpecifyNumericValue()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.SyntaxSpecifyNumericValue);
            Console.ResetColor();
        }

        public static void NoWordsAvailableOrInvalidRange()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.NoWordsAvailableOrInvalidRange);
            Console.ResetColor();
        }

        public static void Syntax()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.SyntaxError);
            Console.ResetColor();
        }

        public static void OnlyNumericCharacters()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.OnlyNumericCharacters);
            Console.ResetColor();
        }

        public static void PdfIndexDoesNotExist()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.PdfIndexDoesNotExist);
            Console.ResetColor();
        }

        public static void PdfFolderEmpty()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.PdfFolderEmpty);
            Console.ResetColor();
        }

        public static void PdfFolderDoesNotExist()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.PdfFolderDoesNotExist);
            Console.ResetColor();
        }

        public static void WordNonExistingById()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Messages.WordNonExistingById);
            Console.ResetColor();
        }
    }
}
