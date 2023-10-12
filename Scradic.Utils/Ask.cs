using Scradic.Utils.Resources;

namespace Scradic.Utils
{
    public class Ask
    {
        public static bool WordToPdf(string title)
        {
            var input = "";
            do
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{Messages.WordToPdf_Ask_1} \"{title}\"? {Messages.WordToPdf_Ask_2}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Globals.Y_N);
                Console.Write($"{Globals.Answer} ");
                Console.ResetColor();
                input = Console.ReadLine();
            } while (input?.ToLower() != "y" && input?.ToLower() != "n");

            return input == "y";
        }

        public static bool EnterWordToSearchTranslate(out string? input)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Messages.EnterWordToSearchTranslate_Ask} ");
            Console.ForegroundColor = ConsoleColor.Green;
            input = Console.ReadLine()?.ToLower();
            Console.ResetColor();
            return input != null;
        }

        public static string SavePdf()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Messages.SavePdf_Ask} ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Globals.Y_N);
            Console.Write($"{Globals.Answer} ");
            Console.ForegroundColor = ConsoleColor.Green;
            return Console.ReadLine().ToLower();
        }

        public static int SeePdf()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.SeePDF} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Messages.SeePdf_Ask} ");
            Console.ForegroundColor = ConsoleColor.Green;
            return int.Parse(Console.ReadLine());
        }
    }
}