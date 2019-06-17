using System.Diagnostics;
using System.Reflection;
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

            SetStyle(ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.UserPaint, true);

        }

        public bool HasResults => flowLayoutPanel1.Controls.Count > 0;

        public void Clear() => flowLayoutPanel1.Controls.Clear();

        public void AddResult(FindDetails details)
        {
            if (details == null) return;

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
                        Arguments = $"/A search=\"{searchPhrase}\"&page=\"{rd.CurrentPage()}\" \"{filename}\"",
                        FileName = pdfReader
                    }
                }.Start();
            };

            flowLayoutPanel1.Controls.Add(rd);
        }

    }
}
