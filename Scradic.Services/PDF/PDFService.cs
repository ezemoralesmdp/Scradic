using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Scradic.Core.Entities;
using Scradic.Core.Interfaces.Repositories;
using Scradic.Core.Interfaces.Services;
using Scradic.Services.Utils;
using Scradic.Utils;
using Scradic.Utils.Resources;
using System.Diagnostics;

namespace Scradic.Services
{
    public class PDFService : IPDFService
    {
        private readonly IWordRepository _wordRepository;
        private readonly IPDFRepository _PDFRepository;

        public PDFService(IWordRepository wordRepository, IPDFRepository pdfRepository)
        {
            _wordRepository = wordRepository;
            _PDFRepository = pdfRepository;
        }

        private string GetNameFile()
        {
            return $"{Globals.ScradicWordsFolderName}_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}";
        }

        public async Task CreatePDF()
        {
            var words = await _wordRepository.GetAllToPdfAsync();

            if (words.Count > 0)
            {
                try
                {
                    var pdfInfo = new PDFInfo();
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string folderPath = Path.Combine(documentsPath, Globals.ScradicWordsFolderName);
                    LineSeparator line = new LineSeparator(new SolidLine());
                    line.SetWidth(520f);
                    var input = "";

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var pdfNameFile = $"{GetNameFile()}.pdf";
                    string pdfFilePath = Path.Combine(folderPath, pdfNameFile);

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
                    document.Add(new Paragraph().Add(new Text(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")).SetItalic()).AddStyle(fontStyle));
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

                    //Clean words
                    CleanWordsPDF();

                    #region Save PDFInfo

                    var files = Directory.GetFiles(folderPath);

                    if (files.Length > 0)
                    {
                        var latestFile = files
                            .Select(filePath => new FileInfo(filePath))
                            .OrderByDescending(fileInfo => fileInfo.LastWriteTime)
                            .First();

                        pdfInfo.Name = pdfNameFile;
                        pdfInfo.FolderPath = folderPath;
                        pdfInfo.Size = latestFile.Length;
                        pdfInfo.TotalWords = words.Count;
                        pdfInfo.FileCreationDate = latestFile.LastWriteTime;

                        await _PDFRepository.SaveLatestPDFInfoAsync(pdfInfo);
                    }
                    
                    #endregion Save PDFInfo

                    do
                    {
                        input = Ask.SavePdf();

                        if (input.ToLower() == "y")
                        {
                            if (File.Exists(pdfFilePath))
                            {
                                var process = new Process();
                                process.StartInfo = new ProcessStartInfo
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

        public void CleanWordsPDF()
        {
            _wordRepository.CleanWordsPDF();
        }

        public void SeePDFList()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = Path.Combine(documentsPath, Globals.ScradicWordsFolderName);
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
                        catch (FormatException)
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

        public async Task<PDFInfo> GetLatestPDFInfoCreatedAsync()
        {
            return await _PDFRepository.GetLatestPDFInfoCreatedAsync();
        }

        public async Task AddToPdf(int wordId)
        {
            var word = await _wordRepository.GetWordByIdAsync(wordId);

            if (word != null && word.Pdf == false)
            {
                await _wordRepository.UpdateWordPdfAsync(word.Id, true);
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"The word \"");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(word.Title);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\" has been successfully added to the PDF!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else if (word != null && word.Pdf == true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"The word \"");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(word.Title);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\" is already added to PDF!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else
                ErrorMessage.WordNonExistingById();
        }

        public async Task RemoveToPdf(int wordId)
        {
            var word = await _wordRepository.GetWordByIdAsync(wordId);

            if (word != null && word.Pdf == true)
            {
                await _wordRepository.UpdateWordPdfAsync(word.Id, false);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"The word \"");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(word.Title);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\" has been successfully removed from the PDF!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else if (word != null && word.Pdf == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Globals.Warning} ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"The word \"");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(word.Title);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\" is already removed from the PDF!");
                Console.ResetColor();
                Console.WriteLine();
            }
            else
                ErrorMessage.WordNonExistingById();
        }
    }
}