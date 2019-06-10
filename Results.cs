using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thothi
{
    public partial class Results : UserControl
    {
        public Results() => InitializeComponent();

        public bool HasResults => flowLayoutPanel1.Controls.Count > 0;
        public void AddResult(FindDetails details)
        {
            if (details == null) return;

            ResultDisplay rd = new ResultDisplay(details);
            rd.Width = Width - 30;

            string filename = rd.fName.Text;

            rd.fName.Click += delegate
            {
                //Filename click on results form
                //if is pdf open at specific page else just open file
                if (new Pdf().IsDocument(filename))
                    Pdf.ForcePdfOpenAt(filename, rd.GetCurrentPage());

                System.Diagnostics.Process.Start(filename);
            };

            flowLayoutPanel1.Controls.Add(rd);

        }

        public void ClearResults()
        {
            flowLayoutPanel1.Controls.Clear();
        }

    }
}
