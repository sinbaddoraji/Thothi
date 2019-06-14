using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Thothi
{
    class FileHandler
    {
        File f = new File();
        public bool CaseSensitive
        {
            get => f.caseSensitive;
            set => f.caseSensitive = value;
        }
        public bool Regex
        {
            get => f.regex;
            set => f.regex = value;
        }
        public bool Supports(string file)
        {
            return new Pdf().IsDocument(file);
        }

        public FindDetails SearchDocument(string file, string searchPhrase)
        {
            try
            {
                if (new Pdf().IsDocument(file)) f = new Pdf();
                else return null; //Is not supported

                return f.SearchDocument(file, searchPhrase);
            }
            catch 
            {
                return null;
            }
           
        }
    }

    class File
    {
        public bool caseSensitive = false;
        public bool regex = false;
        public virtual bool IsDocument(string file) => false;

        protected virtual List<string> ExtractPages(string file) => null;

        public virtual FindDetails SearchDocument(string file, string searchPhrase)
        {
            var pages = ExtractPages(file);
            FindDetails output = new FindDetails();

            for (int i = 0; i < pages.Count; i++)
            {
                string page = pages[i];
                if(regex && !new Regex(searchPhrase).IsMatch(searchPhrase)) continue;
                else if (caseSensitive && !page.Contains(searchPhrase)) continue;
                else if (!caseSensitive)
                {
                    bool ismatch = page.ToLower().Contains(searchPhrase.ToLower());
                    if (!ismatch) continue;
                }

                //Record page number
                output.searchPhrase = searchPhrase;
                output.filename = file;
                output.pagesSearchFound.Add(i + 1);
            }

            if(output.filename == null) return null;
            else return output;
        }

    }
    class Pdf : File
    {
        public override bool IsDocument(string file) =>
            new FileInfo(file).Extension.Equals(".pdf");
        protected override List<string> ExtractPages(string file)
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
        public static void ForcePdfOpenAt(string file, int page)
        {
            //Modify pdf to load at specific page on next run
            using(PdfStamper stamper = new PdfStamper(new PdfReader(System.IO.File.ReadAllBytes(file))
                , new FileStream(file, FileMode.Create)))
            {
                var destination = new PdfDestination(PdfDestination.FITH);

                var pdfAction = PdfAction.GotoLocalPage(page, destination, stamper.Writer);

                stamper.Writer.SetOpenAction(pdfAction);
            }
        }
    }


}
