using System;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace Thothi
{
    internal class FileHandler
    {
        private readonly Pdf f = new Pdf();
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
            return f.SearchDocument(file, searchPhrase);
        }
    }

    internal class Pdf
    {
        public bool caseSensitive = false;
        public bool regex = false;
        public bool IsDocument(string file)
        {
            return new FileInfo(file).Extension.Equals(".pdf");
        }

        private List<string> ExtractPages(string file)
        {
            // Create a reader for the given PDF file
            using (PdfReader reader = new PdfReader(File.ReadAllBytes(file)))
            {
                List<string> pages = new List<string>();
                ITextExtractionStrategy extractionStrategy = new SimpleTextExtractionStrategy();

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    pages.Add(PdfTextExtractor.GetTextFromPage(reader, page, extractionStrategy));
                }

                return pages;
            }
        }

        private bool ContainsMatch(string page, string searchPhrase)
        {
            if (regex)
            {
                try
                {
                    return new Regex(searchPhrase).IsMatch(page);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("Invalid regex expression");
                    return false;
                }
            }
            else
            {
                return caseSensitive == true
                    ? page.Contains(searchPhrase)
                    : page.IndexOf(searchPhrase, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        public FindDetails SearchDocument(string file, string searchPhrase)
        {
            try
            {
                List<string> pages = ExtractPages(file);

                FindDetails output = new FindDetails(file, searchPhrase);

                for (int i = 0; i < pages.Count; i++)
                {
                    if (ContainsMatch(pages[i], searchPhrase))
                    {
                        //Record page number
                        output.pagesSearchFound.Add(i + 1);
                    }
                }

                return output.pagesSearchFound.Count > 0 ? output : null;
            }
            catch 
            {
                return null; 
            }
        }

        public static string GetAssociatedProgram()
        {
            try
            {
                var strExtValue = Convert.ToString(Registry.ClassesRoot.OpenSubKey(".pdf").GetValue(null));

                var objAppReg2 = Registry.ClassesRoot.OpenSubKey(strExtValue + "\\shell\\open\\command");

                var splitArray = Convert.ToString(objAppReg2.GetValue(null)).Split('"');

                var spI = splitArray[0].Trim().Length == 0 ? 1 : 0;

                return splitArray[spI].Replace("%1", "");
            }
            catch
            {
                return null;
            }
        }

    }


}
