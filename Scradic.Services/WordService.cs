using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Services.Utils;

namespace Scradic.Services
{
    public class WordService : IWordService
    {
        private readonly IWordRepository _repository;

        public WordService(IWordRepository wordRepository)
        {
            _repository = wordRepository;
        }

        public void ShowWord(Word word)
        {
            Console.WriteLine();
            if(word != null)
            {
                if (!string.IsNullOrEmpty(word.Title))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Title: ");
                    Console.ResetColor();
                    Console.WriteLine(word.Title);
                }
                if (!string.IsNullOrEmpty(word.GramaticalCategory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Gramatical category: ");
                    Console.ResetColor();
                    Console.WriteLine(word.GramaticalCategory);
                }
                if (!string.IsNullOrEmpty(word.AnotherSuggestion))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(word.AnotherSuggestion);
                }
                Console.WriteLine();
                if (word.Definitions.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Definitions]");
                    Console.ResetColor();
                    foreach (var definition in word.Definitions)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Translation: ");
                        Console.ResetColor();
                        Console.WriteLine(definition.Translation);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Description: ");
                        Console.ResetColor();
                        Console.WriteLine(definition.Description);

                        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
                    }
                }
                if (word.Examples.Count > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[Examples]");
                    Console.ResetColor();
                    for (int i = 0; i < word.Examples.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(i + 1 + ") ");
                        Console.ResetColor();
                        Console.WriteLine(word.Examples[i].Description);
                    }
                }
            }
        }

        public async Task<Word> IncrementHints(Word word)
        {
            return await _repository.IncrementHints(word);
        }

        public bool CheckWordExistsAsync(string wordTitle)
        {
            return _repository.CheckWordExists(wordTitle);
        }

        public async Task<Word> GetWordByTitleAsync(string wordTitle)
        {
            return await _repository.GetWordByTitleAsync(wordTitle);
        }

        public async Task SaveWordAsync(Word word)
        {
            await _repository.SaveWordAsync(word);
        }

        public async Task AddToPdf(Word word)
        {
            word.Pdf = true;
            await _repository.UpdateWordAsync(word);
        }

        public async Task ShowTop(int amount)
        {
            var topList = await _repository.GetTop(amount);

            if (topList.Count > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[TOP {amount}]");
                Console.ResetColor();
                for (int i = 0; i < topList.Count; i++)
                {
                    Console.Write($"ID: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(topList[i].Id);
                    Console.ResetColor();
                    Console.Write(" | " + topList[i].Title + " | HITS: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(topList[i].Hits);
                    Console.ResetColor();
                }
            }
            else
                ErrorMessage.NoWordsAvailable();
        }
    }
}