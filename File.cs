using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using Microsoft.Win32;

namespace Thothi
{
    class FileHandler
    {
        readonly Pdf f = new Pdf();
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
            => new Pdf().IsDocument(file);

        public FindDetails SearchDocument(string file, string searchPhrase) 
            => f.SearchDocument(file, searchPhrase);
    }

    class Pdf 
    {
        public bool caseSensitive = false;
        public bool regex = false;
        private readonly ITextExtractionStrategy extractionStrategy = new SimpleTextExtractionStrategy();

        public bool IsDocument(string file) =>
            new FileInfo(file).Extension.Equals(".pdf");

        private  List<string> ExtractPages(string file)
        {
            // Create a reader for the given PDF file
            using (PdfReader reader = new PdfReader(File.ReadAllBytes(file)))
            {
                List<string> pages = new List<string>();

                for (int page = 1; page <= reader.NumberOfPages; page++)
                    pages.Add(PdfTextExtractor.GetTextFromPage(reader, page, extractionStrategy));

                return pages;
            }
        }

        public FindDetails SearchDocument(string file, string searchPhrase)
        {
            var pages = ExtractPages(file);

            FindDetails output = new FindDetails(file,searchPhrase);

            for (int i = 0; i < pages.Count; i++)
            {
                string page = pages[i];
                if (regex && !new Regex(searchPhrase).IsMatch(searchPhrase)) continue;
                else if (caseSensitive == true && !page.Contains(searchPhrase)) continue; //search for exact searchPhrase
                else if (caseSensitive == false)
                {
                    //search reguardless of case
                    if (page.IndexOf(searchPhrase, StringComparison.OrdinalIgnoreCase) == -1)
                        continue;
                }

                //Record page number
                output.pagesSearchFound.Add(i + 1);
            }

            return output.pagesSearchFound.Count > 0 ? output : null;
        }

        public static string GetAssociatedProgram(string FileExtension)
        {
            RegistryKey objExtReg2 = Registry.ClassesRoot;
            RegistryKey objAppReg2 = Registry.ClassesRoot;
            try
            {
                if(!FileExtension.StartsWith(".")) FileExtension = "." + FileExtension;

                objExtReg2 = objExtReg2.OpenSubKey(FileExtension.Trim());

                string strExtValue = objExtReg2.GetValue("").ToString();

                objAppReg2 = objAppReg2.OpenSubKey(strExtValue + "\\shell\\open\\command");

                
                string[] SplitArray = Convert.ToString(objAppReg2.GetValue(null)).Split('"');


                if (SplitArray[0].Trim().Length > 0) return SplitArray[0].Replace("%1", "");
                return SplitArray[1].Replace("%1", "");
            }
            catch 
            {
               return "";
            }
        }
    }


}
