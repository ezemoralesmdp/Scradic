using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces;
using Scradic.Services.Utils;
using Scradic.Utils;
using Scradic.Utils.Resources;
using System.Diagnostics;

namespace Scradic.Services
{
    public class WordService : IWordService
    {
        private readonly IWordRepository _repository;
        private readonly string folderName = "Dictionary_Words";

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

        private string GetNameFile()
        {
            return $"{folderName}_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}";
        }

        public async Task CreatePDF()
        {
            var words = await _repository.GetAllToPdfAsync();

            if (words.Count > 0)
            {
                try
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string folderPath = Path.Combine(documentsPath, folderName);
                    LineSeparator line = new LineSeparator(new SolidLine());
                    line.SetWidth(520f);
                    var input = "";

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string pdfFilePath = Path.Combine(folderPath, $"{GetNameFile()}.pdf");

                    //Initialize PDF writer
                    PdfWriter writer = new PdfWriter(pdfFilePath);
                    //Initialize PDF document
                    PdfDocument pdf = new PdfDocument(writer);
                    // Initialize document
                    Document document = new Document(pdf);
                    // Create font
                    PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    // Create font style
                    Style fontStyle = new Style()
                        .SetFont(font)
                        .SetFontSize(10);

                    //Date
                    document.SetTextAlignment(TextAlignment.RIGHT);
                    document.Add(new Paragraph().Add(new Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).SetItalic()).AddStyle(fontStyle));
                    document.SetTextAlignment(TextAlignment.LEFT);

                    document.Add(new Paragraph().Add(new Text("Word index: ").SetBold()).AddStyle(fontStyle));

                    //Word index
                    List list = new List().SetSymbolIndent(12).SetListSymbol("\u2022").SetFont(font);

                    foreach (var word in words)
                    {
                        list.Add(new ListItem(word.Title)).AddStyle(fontStyle);
                    }

                    document.Add(list);

                    //New page
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                    //Words
                    foreach (var word in words)
                    {
                        if (word != words.First())
                            document.Add(new Paragraph(""));

                        if (!string.IsNullOrEmpty(word.Title))
                            document.Add(new Paragraph().Add(new Text("Word: ").SetBold()).Add(new Text(word.Title ?? "")).AddStyle(fontStyle));
                        if (!string.IsNullOrEmpty(word.GramaticalCategory))
                            document.Add(new Paragraph().Add(new Text("Gramatical category: ").SetBold()).Add(new Text(word.GramaticalCategory ?? "")).AddStyle(fontStyle));
                        if (!string.IsNullOrEmpty(word.AnotherSuggestion))
                            document.Add(new Paragraph().Add(new Text("Another suggestion: ").SetBold()).Add(new Text(word.AnotherSuggestion ?? "")).AddStyle(fontStyle));

                        if (word.Definitions.Count > 0)
                        {
                            document.Add(new Paragraph().Add(new Text("Definitions: ").AddStyle(fontStyle).SetBold()));
                            for (int i = 0; i < word.Definitions.Count; i++)
                            {
                                document.Add(new Paragraph().Add(new Text($"{i + 1}) ").AddStyle(fontStyle).SetBold()).Add(new Text(word.Definitions[i].Translation ?? "").AddStyle(fontStyle)));
                                document.Add(new Paragraph(word.Definitions[i].Description ?? "").AddStyle(fontStyle).SetItalic());
                            }
                        }

                        if (word.Examples.Count > 0)
                        {
                            document.Add(new Paragraph().Add(new Text("Examples: ").AddStyle(fontStyle).SetBold()));
                            for (int i = 0; i < word.Examples.Count; i++)
                            {
                                document.Add(new Paragraph($"{i + 1}) {word.Examples[i].Description ?? ""}").AddStyle(fontStyle));
                            }
                        }

                        document.Add(line);
                    }

                    //Close document
                    document.Close();

                    do
                    {
                        input = Ask.SavePdf();

                        if (input.ToLower() == "y")
                        {
                            if (File.Exists(pdfFilePath))
                            {
                                var process = new System.Diagnostics.Process();
                                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = pdfFilePath
                                };

                                process.Start();
                            }
                        }

                    } while (input.ToLower() != "y" && input.ToLower() != "n");
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
            else
                ErrorMessage.NoWordsAvailable();
        }

        public async Task SeePDFList()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, folderName);
            var input = 0;

            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                var flag = false;

                if (files.Length > 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(Messages.Pdf_Files);

                    for (int i = 0; i < files.Length; i++)
                    {
                        string fileName = Path.GetFileName(files[i]);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{i + 1}) ");
                        Console.ResetColor();
                        Console.WriteLine(fileName);
                    }

                    do
                    {
                        input = Ask.SeePdf();

                        try
                        {
                            if (input == 0) break;
                            var pdfToOpenPath = files[input - 1];

                            if (File.Exists(pdfToOpenPath))
                            {
                                var process = new Process();
                                process.StartInfo = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = pdfToOpenPath
                                };

                                process.Start();
                                flag = true;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"{Globals.Warning} ");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(Messages.PdfFolderDoesNotExist);
                                Console.WriteLine();
                            }
                        }
                        catch (FormatException e)
                        {
                            ErrorMessage.OnlyNumericCharacters();
                        }
                        catch (IndexOutOfRangeException)
                        {
                            ErrorMessage.PdfIndexDoesNotExist();
                        }

                    } while (flag == false);
                }
                else
                    ErrorMessage.PdfFolderEmpty();
            }
            else
                ErrorMessage.PdfFolderDoesNotExist();
        }

        public async Task GetAllSavedWordsAsync()
        {
            var words = await _repository.GetAllSavedWordsOrderByDescendingAsync();

            if (words.Count > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Words]");
                Console.ResetColor();

                foreach (var word in words)
                {
                    Console.Write($"ID: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(word.Id);
                    Console.ResetColor();
                    Console.Write(" | " + word.Title + " | INSERT DATE: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(word.InsertDate.ToString("dd/MM/yyy HH:mm:ss"));
                    Console.ResetColor();
                }
            }
            else
                ErrorMessage.NoWordsAvailable();
        }
    }
}