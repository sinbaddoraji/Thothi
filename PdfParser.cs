using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Thothi
{
    internal class PdfParser
    {
        public static List<string> ExtractPages(string file)
        {
            // Create a reader for the given PDF file
            using (PdfReader reader = new PdfReader(System.IO.File.ReadAllBytes(file)))
            {
                List<string> pages = new List<string>();

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy extractionStrategy = new SimpleTextExtractionStrategy();
                    pages.Add(PdfTextExtractor.GetTextFromPage(reader, page, extractionStrategy));
                }

                return pages;
            }
        }


    }
}


