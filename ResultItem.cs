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
    public partial class ResultItem : UserControl
    {
        public ResultItem() => InitializeComponent();

        public int GetCurrentPage()
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

            if(comboBox1.Items.Count > 0)
            comboBox1.Text = comboBox1.Items[0].ToString();
        }

        private void ResultDisplay_SizeChanged(object sender, EventArgs e)
        {
            fName.Left = (Width - fName.Width) / 2;
            label2.Left = (comboBox1.Width-label2.Width) / 2;

            if(Height > 101) Size = new Size(Width,101);
        }
    }
}
