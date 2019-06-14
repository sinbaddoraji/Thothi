using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Thothi
{
    public partial class Result : UserControl
    {
        //path to the pdf reader's exe
        public string searchPhrase;

        string pdfReader = Pdf.GetAssociatedProgram(".pdf");
        public Result() => InitializeComponent();

        public bool HasResults => flowLayoutPanel1.Controls.Count > 0;

        public void AddResult(FindDetails details)
        {
            if (details == null) return;

            ResultItem rd = new ResultItem(details);
            rd.Width = Width - 30;

            string filename = rd.fName.Text;

            rd.fName.Click += delegate
            {
                new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = $"/A \"page={rd.CurrentPage()}\"&search=\"{searchPhrase}\" \"{filename}\"",
                        FileName = pdfReader
                    }
                }.Start();

                //#search="word1 word2"
            };

            flowLayoutPanel1.Controls.Add(rd);

        }

        public void ClearResults()
        {
            flowLayoutPanel1.Controls.Clear();
        }

    }
}
