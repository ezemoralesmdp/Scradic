using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Interfaces;
using Scradic.Services.Utils;
using Scradic.Utils;
using Scradic.Utils.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace Scradic
{
    public class Start : IStart
    {
        private static readonly HashSet<string> keyWords = new HashSet<string> 
        {   
            "!exit",
            "!words",
            "!pdf",
            "!help",
            "!remove",
            "!removeall",
            "!clear",
            "!seepdf",
        };

        private string? inputFormatted = "";
        private readonly IMemoryCache _cache;
        private readonly IWordService _service;
        private bool goSearchCache;

        public Start(IMemoryCache memoryCache, IWordService wordService)
        {
            _service = wordService;
            _cache = memoryCache;
        }

        private async Task ShowIncrementAndCachingWord(Word word)
        {
            _service.ShowWord(word);
            word = await _service.IncrementHints(word);
            if (!string.IsNullOrEmpty(word.Title))
                _cache.Set(word.Title.ToLower(), word);
        }

        public async Task StartScradic() 
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Messages.Welcome);
            Console.ResetColor();

            do
            {
                if (Ask.EnterWordToSearchTranslate(out inputFormatted)) Formatter.SanitizeDataInput(inputFormatted);
                Word? word = new();

                if (!string.IsNullOrEmpty(inputFormatted) && !keyWords.Contains(inputFormatted))
                {
                    if(inputFormatted.StartsWith("!top"))
                    {
                        var numberPart = inputFormatted.Substring(4);
                        if (int.TryParse(numberPart, out int number))
                            await _service.ShowTop(number);
                        else
                            ErrorMessage.Syntax();
                    }
                    else
                    {
                        goSearchCache = _cache.TryGetValue(inputFormatted, out word);

                        if (goSearchCache == true)
                        {
                            await ShowIncrementAndCachingWord(word);
                        }
                        else if(goSearchCache == false && _service.CheckWordExistsAsync(inputFormatted))
                        {
                            word = await _service.GetWordByTitleAsync(inputFormatted);
                            if (word != null) await ShowIncrementAndCachingWord(word);
                        }
                        else
                        {
                            var url = $"https://dictionary.cambridge.org/es/diccionario/ingles-espanol/{inputFormatted}";
                            var oWeb = new HtmlWeb
                            {
                                OverrideEncoding = Encoding.UTF8
                            };
                            var doc = oWeb.Load(url);

                            word = new();

                            #region Title

                            var titleNode = doc.DocumentNode.SelectSingleNode("//span[@class='hw dhw']");
                            if (titleNode == null)
                                titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='h2 tw-bw dhw dpos-h_hw di-title ']");
                            if (titleNode != null)
                                word.Title = titleNode.InnerText;

                            #endregion Title

                            #region Another suggestion

                            var anotherSuggestionNode = doc.DocumentNode.SelectSingleNode("//span[@class='var dvar']");
                            if (anotherSuggestionNode != null)
                                word.AnotherSuggestion = Regex.Replace(anotherSuggestionNode.InnerText, @"\(|\)", "");

                            #endregion Another suggestion

                            #region Grammatical category

                            var grammaticalCategoryNode = doc.DocumentNode.SelectSingleNode("//span[@class='pos dpos']");
                            if (grammaticalCategoryNode != null)
                                word.GramaticalCategory = grammaticalCategoryNode.InnerHtml;

                            #endregion Grammatical category

                            #region Body

                            var posBodyDiv = doc.DocumentNode.SelectSingleNode("//div[@class='pos-body']");
                            if (posBodyDiv != null)
                            {
                                #region Definitions

                                var definitionsNodes = posBodyDiv.SelectNodes(".//div[@class='def ddef_d db']");
                                if (definitionsNodes != null)
                                {
                                    foreach (var definition in definitionsNodes)
                                    {
                                        var definitionText = Regex.Replace(definition.InnerHtml, "<.*?>", " ");
                                        definitionText = Regex.Replace(definitionText, @"\s+", " ").Replace(" ,", ",").Trim();
                                        word.Definitions.Add(new Definition { Description = definitionText });
                                    }
                                }

                                #endregion Definitions

                                #region Translations

                                var translationsNodes = posBodyDiv.SelectNodes(".//span[@class='trans dtrans dtrans-se ']");
                                if (translationsNodes != null)
                                {
                                    for (int i = 0; i < word.Definitions.Count; i++)
                                    {
                                        var translationText = Regex.Replace(translationsNodes[i].InnerHtml, "<.*?>", " ").Trim();
                                        translationText = Regex.Replace(translationText, @"\s+", " ").Replace(" ,", ",");
                                        word.Definitions[i].Translation = translationText;
                                    }
                                }

                                #endregion Translations

                                #region Examples

                                var examplesNodes = posBodyDiv.SelectNodes("//div[@class='examp dexamp']");
                                if (examplesNodes != null)
                                {
                                    foreach (var example in examplesNodes.Take(10))
                                    {
                                        var englishNode = example.SelectSingleNode(".//span[@class='eg deg']");
                                        var spanishNode = example.SelectSingleNode(".//span[@class='trans dtrans dtrans-se hdb']");

                                        var englishText = englishNode?.InnerText.Trim() ?? "";
                                        var spanishText = spanishNode?.InnerText.Trim() ?? "";

                                        if (!string.IsNullOrEmpty(englishText) && !string.IsNullOrEmpty(spanishText))
                                            word.Examples.Add(new Example($"{englishText} || {spanishText}"));
                                        else if (!string.IsNullOrEmpty(englishText))
                                            word.Examples.Add(new Example(englishText));
                                        else if (!string.IsNullOrEmpty(spanishText))
                                            word.Examples.Add(new Example(spanishText));
                                    }
                                }

                                #endregion Examples
                            }

                            #endregion Body

                            //Caching
                            if (!string.IsNullOrEmpty(word.Title))
                                _cache.Set(word.Title.ToLower(), word);

                            if (!string.IsNullOrEmpty(word.Title) ||
                                !string.IsNullOrEmpty(word.GramaticalCategory) ||
                                !string.IsNullOrEmpty(word.AnotherSuggestion) ||
                                word.Definitions?.Count > 0 ||
                                word.Examples?.Count > 0)
                            {
                                _service.ShowWord(word);
                                await _service.SaveWordAsync(word);

                                if (!string.IsNullOrEmpty(word.Title) && Ask.WordToPdf(word.Title))
                                    await _service.AddToPdf(word);
                            }
                        }
                    }
                }

                if (inputFormatted == "!clear")
                    _service.ClearConsole();

                if (inputFormatted == "!pdf")
                    await _service.CreatePDF();

                if (inputFormatted == "!seepdf")
                    await _service.SeePDFList();

                if (inputFormatted == "!words")
                    await _service.GetAllSavedWordsAsync();

            } while(inputFormatted != "!exit");
        }
    }
}