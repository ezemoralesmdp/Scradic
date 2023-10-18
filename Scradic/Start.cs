using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Core.Interfaces.Services;
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
            "!clear",
            "!user",
            "!pdf",
            "!seepdf",
            "!pdfemail",
            "!help",
            "!exit"
        };

        private string? inputFormatted = "";
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;
        private User? _user;
        private readonly IWordService _service;
        private readonly IPDFService _PDFService;
        private readonly IEmailService _emailService;
        private bool goSearchCache;
        private string numberPart = "";

        public Start(
            IMemoryCache memoryCache, 
            IUserService userService, 
            IWordService wordService, 
            IPDFService pdfService, 
            IEmailService emailService)
        {
            _service = wordService;
            _cache = memoryCache;
            _emailService = emailService;
            _userService = userService;
            _PDFService = pdfService;
        }

        private async Task GetSingleUser()
        {
            var user = await _userService.GetSingleUser();
            if(user != null)
                _cache.Set(nameof(User), user);
        }

        private async Task ShowIncrementAndCachingWord(Word word)
        {
            _service.ShowWord(word);
            word = await _service.IncrementHints(word);
            if (!string.IsNullOrEmpty(word.Title))
                _cache.Set(word.Title.ToLower(), word);
        }

        private void Help()
        {
            var listCommands = new List<string>
                {
                    "!clear --> To clear the console",
                    "!user --> To view and update your current user",
                    "!allwords --> To see all the words you have searched for",
                    "!words{num} or !words{num}-{num} --> To see all the words you have searched up to a certain ID. You can search by range. For example: !words10 or !words10-30",
                    "!top{num} --> To see the top of the most searched words. Examples: !top10, !top100",
                    "!addwordpdf{id} --> To add a word already searched in the PDF to be generated. Examples: !addwordpdf1",
                    "!delwordpdf{id} --> To remove a word from the PDF to be generated",
                    "!pdf --> To generate a PDF with all the words you saved",
                    "!seepdf --> To view one of the generated PDFs",
                    "!pdfemail --> To send an email to yourself with the last generated PDF",
                    "!help --> To view the help menu",
                    "!exit --> To end the program",
                };

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[!] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Help: ");

            for (int i = 0; i < listCommands.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{i + 1}) ");
                Console.ResetColor();
                Console.WriteLine(listCommands[i]);
            }
        }

        private void TryAgain()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{Globals.Warning} ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Messages.TryAgain}");
            Console.ResetColor();
        }

        public async Task StartScradic() 
        {
            await GetSingleUser();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Messages.Welcome);
            Console.ResetColor();

            if(!_cache.TryGetValue(nameof(User), out _user))
            {
                User user = new();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Messages.FirstTime);

                do
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Enter your username: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    user.Username = Console.ReadLine();

                } while (string.IsNullOrEmpty(user.Username));

                do
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Enter your email: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    user.Email = Console.ReadLine();

                } while (string.IsNullOrEmpty(user.Email));

                if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Email))
                {
                    await _userService.RegisterSingleUser(user);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{Globals.Warning} ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"User \"{user.Username}\" created successfully!");
                }

                Console.ResetColor();
            }

            do
            {
                if (Ask.EnterWordToSearchTranslate(out inputFormatted)) Formatter.SanitizeDataInput(inputFormatted);
                Word? word = new();

                if (!string.IsNullOrEmpty(inputFormatted) && !keyWords.Contains(inputFormatted) && inputFormatted != "!allwords")
                {
                    if(inputFormatted.StartsWith("!top"))
                    {
                        numberPart = inputFormatted.Substring(4);
                        if (int.TryParse(numberPart, out int amount))
                            await _service.ShowTop(amount);
                        else
                            ErrorMessage.Syntax();
                    }
                    else if (inputFormatted.StartsWith("!words"))
                    {
                        string numberPart = inputFormatted.Substring(6);
                        bool exitIf = false;

                        if (string.IsNullOrWhiteSpace(numberPart))
                        {
                            ErrorMessage.SyntaxSpecifyNumericValue();
                            exitIf = true;
                        }

                        if(!exitIf)
                        {
                            string[] numbers = numberPart.Split('-');

                            if (numbers.Length == 1) // Case: !words30
                            {
                                if (int.TryParse(numbers[0], out int start))
                                    await _service.GetAllSavedWordsInRangeAsync(start, null);
                                else
                                    ErrorMessage.SyntaxSpecifyNumericValue();
                            }
                            else if (numbers.Length == 2) // Case: !words30-50
                            {
                                if (int.TryParse(numbers[0], out int start) && int.TryParse(numbers[1], out int end))
                                    await _service.GetAllSavedWordsInRangeAsync(start, end);
                                else
                                    ErrorMessage.SyntaxSpecifyNumericValue();
                            }
                            else
                                ErrorMessage.SyntaxSpecifyNumericValue();
                        }
                    }
                    else if(inputFormatted.StartsWith("!addwordpdf"))
                    {
                        string numberPart = inputFormatted.Substring(11);
                        if (int.TryParse(numberPart, out int wordId))
                            await _PDFService.AddToPdf(wordId);
                        else
                            ErrorMessage.Syntax();
                    }
                    else if (inputFormatted.StartsWith("!delwordpdf"))
                    {
                        string numberPart = inputFormatted.Substring(11);
                        if (int.TryParse(numberPart, out int wordId))
                            await _PDFService.RemoveToPdf(wordId);
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
                            if (word != null)
                            {
                                _service.UpdateLastSearch(word);
                                await ShowIncrementAndCachingWord(word);
                            }
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
                            else
                                TryAgain();
                        }
                    }
                }

                if (inputFormatted == "!help")
                    Help();

                if (inputFormatted == "!clear")
                    _service.ClearConsole();

                if (inputFormatted == "!user")
                    await _userService.UpdateUser(_user);

                if (inputFormatted == "!allwords")
                    await _service.GetAllSavedWordsAsync();

                if (inputFormatted == "!pdf")
                    await _PDFService.CreatePDF();

                if (inputFormatted == "!seepdf")
                    _PDFService.SeePDFList();

                if (inputFormatted == "!pdfemail")
                {
                    try
                    {
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string folderPath = Path.Combine(documentsPath, Globals.ScradicWordsFolderName);

                        try
                        {
                            var files = Directory.GetFiles(folderPath);

                            if (files.Length > 0)
                            {
                                #region Email preparation

                                var pdfInfo = await _PDFService.GetLatestPDFInfoCreatedAsync();

                                if (pdfInfo != null)
                                {
                                    EmailRequest mailRequest = new EmailRequest()
                                    {
                                        ToEmail = _user.Email,
                                        Subject = $"{_user.Username} this is incredible! This week you did a very interesting word search, check them out!",
                                        Body =
                                            $"<div style=\"background-color: #e6bda6; padding: 20px; text-align: center;\">" +
                                                $"<div><img width=\"250px\" alt=\"logo\" src=\"cid:logo\"/></div>" +
                                                $"<p>{_user.Username}, we send you the latest PDF you have created!</p>" +
                                                $"<p>File name: <span style =\"font-weight: bolder;\">{pdfInfo.Name}</span></p>" +
                                                $"<p>Total words: <span style =\"font-weight: bolder;\">{pdfInfo.TotalWords}</span></p>" +
                                                $"<p>Size: <span style =\"font-weight: bolder;\">{Formatter.FormatFileSize(pdfInfo.Size)}</span></p>" +
                                                $"<p>File creation date: <span style =\"font-weight: bolder;\">{pdfInfo.FileCreationDate.ToString("MM/dd/yyyy HH:mm:ss tt")}</span></p>" +
                                            $"</div>",
                                        PDFPath = Path.Combine(pdfInfo.FolderPath, pdfInfo.Name),
                                        PDFFileName = pdfInfo.Name,
                                        LogoBase64 = Images64.Logo
                                    };

                                    _emailService.SendEmailWithAttachmentAsync(mailRequest);

                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write($"{Globals.Warning} ");
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write($"{Messages.MailSentSuccessfully}: ");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"{mailRequest.ToEmail}");
                                    Console.ResetColor();

                                }
                                else
                                    ErrorMessage.PdfFolderEmpty();

                                #endregion Email preparation
                            }
                            else
                                ErrorMessage.PdfFolderEmpty();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorMessage.PdfFolderDoesNotExist();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{Messages.FailedSendEmail}: {ex.Message}");
                    }
                }

            } while(inputFormatted != "!exit");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(Messages.Goodbye);
            Console.ResetColor();
        }
    }
}