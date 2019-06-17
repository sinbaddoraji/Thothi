using System;
using System.Windows.Forms;

namespace Thothi
{
    public partial class ResultItem : UserControl
    {
        public int CurrentPage()
        {
            int.TryParse(comboBox1.Text, out int output);
            return output;
        }

        public ResultItem(FindDetails fd) 
        {
            InitializeComponent();

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
