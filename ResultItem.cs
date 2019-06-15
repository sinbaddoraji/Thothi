using System;
using System.Drawing;
using System.Windows.Forms;

namespace Thothi
{
    public partial class ResultItem : UserControl
    {
        public ResultItem()
        {
            InitializeComponent();
        }

        public int CurrentPage()
        {
            try
            {
                return int.Parse(comboBox1.Text);
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public ResultItem(FindDetails fd) : this()
        {
            fName.Text = fd.filename;
            comboBox1.DataSource = fd.pagesSearchFound;

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }
        }

        private void ResultDisplay_SizeChanged(object sender, EventArgs e)
        {
            fName.Left = (Width - fName.Width) / 2;
        }
    }
}
