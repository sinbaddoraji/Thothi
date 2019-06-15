using System.Diagnostics;
using System.Windows.Forms;

namespace Thothi
{
    public partial class Result : UserControl
    {
        //path to the pdf reader's exe
        public string searchPhrase;
        private readonly string pdfReader = Pdf.GetAssociatedProgram();
        public Result()
        {
            InitializeComponent();
        }

        public bool HasResults => flowLayoutPanel1.Controls.Count > 0;

        public void AddResult(FindDetails details)
        {
            if (details == null)
            {
                return;
            }

            ResultItem rd = new ResultItem(details)
            {
                Width = this.Width - 28
            };

            SizeChanged += delegate { rd.Width = Width - 28; };
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
