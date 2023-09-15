namespace Scradic.Utils
{
    public class Ask
    {
        public static bool WordToPdf_Ask(string title)
        {
            var input = "";
            do
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[?] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"Do you want to save the word \"{title}\"? to PDF file? ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[y/n]");
                Console.Write("ANSWER: ");
                Console.ResetColor();
                input = Console.ReadLine();
            } while (input?.ToLower() != "y" && input?.ToLower() != "n");

            return input == "y";
        }

        public static bool EnterWordToSearchTranslate_Ask(out string? input)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[!] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Enter the command or word you want to search/translate: ");
            Console.ForegroundColor = ConsoleColor.Green;
            input = Console.ReadLine()?.ToLower();
            Console.ResetColor();
            return input != null;
        }
    }
}