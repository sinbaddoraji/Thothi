using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ms = Microsoft.Office.Interop;

namespace Thothi
{
    internal class OfficeDoc
    {
        public static bool IsWordFile(string file)
        {
            string[] extensions = new[] {".doc",".dot",".wbk",".docx",".docm",".dotx",".dotm",".docb" };
           
            return extensions.Contains(new FileInfo(file).Extension);
        }

        public static bool IsPptFile(string file)
        {
            string[] extensions = new[] { ".ppt", ".pot", ".pps", ".pptx", ".pptm", ".potx", 
                                          ".potm",".ppam", ".ppsx", ".ppsm", ".sldx", ".sldm" };

            return extensions.Contains(new FileInfo(file).Extension);
        }


        public static List<string> ExtractWordPages(string file)
        {
            List<string> output = new List<string>();

            ms.Word.Application word = new ms.Word.Application();
            word.Visible = false;

            object path = file;
            object miss = Missing.Value;
            object readOnly = true;
            object visible = false;

            Document doc = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, 
                ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref visible, ref miss, 
                ref miss, ref miss, ref miss);

            long pageCount = doc.ComputeStatistics(WdStatistic.wdStatisticPages, ref miss);

            //Get pages
            object What = WdGoToItem.wdGoToPage; object Which = WdGoToDirection.wdGoToAbsolute;
            object pageStart; object pageEnd;
            object CurrentPageNumber; object NextPageNumber;

            for (int Index = 1; Index < pageCount + 1; Index++)
            {
                CurrentPageNumber = Convert.ToInt32(Index.ToString());
                NextPageNumber = Convert.ToInt32((Index + 1).ToString());

                pageStart = word.Selection.GoTo(ref What, ref Which, ref CurrentPageNumber, ref miss).Start;               
                pageEnd = word.Selection.GoTo(ref What, ref Which, ref NextPageNumber, ref miss).End;

                // Get text
                if (Convert.ToInt32(pageStart.ToString()) != Convert.ToInt32(pageEnd.ToString()))
                    output.Add(doc.Range(ref pageStart, ref pageEnd).Text);
                else
                    output.Add(doc.Range(ref pageStart).Text);
            }

            word.Quit();
            doc.Close();
            Marshal.ReleaseComObject(word);
            Marshal.ReleaseComObject(doc);

            return output;
        }


        public static List<string> ExtractPptPages(string file)
        {
            List<string> output = new List<string>();
            ms.PowerPoint.Application ppt = new ms.PowerPoint.Application();

            Presentation presentation = ppt.Presentations.Open(file, Microsoft.Office.Core.MsoTriState.msoTrue,
                    Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoFalse);

            if (presentation.Slides == null)
            {
                return null;
            }

            for (int i = 1; i <= presentation.Slides.Count; i++)
            {
                Slide slide = presentation.Slides[i];
                string pageStr = "";

                foreach (ms.PowerPoint.Shape shape in slide.Shapes)
                {
                    TextRange pptTxtRange = shape.TextFrame.TextRange;
                    pageStr += pptTxtRange.Text + " ";

                    Marshal.ReleaseComObject(pptTxtRange);
                    Marshal.ReleaseComObject(shape);
                }
                output.Add(pageStr);
                Marshal.ReleaseComObject(slide);
            }

            ppt.Quit();
            presentation.Close();
            Marshal.ReleaseComObject(ppt);
            Marshal.ReleaseComObject(presentation);

            return output;
        }


    }
}
