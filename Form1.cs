using System;
using System.Windows.Forms;

namespace Thothi
{
    public partial class Form1 : Form
    {
        private readonly SearchEngine searchEngine = new SearchEngine();

        public Form1()
        {
            InitializeComponent();
            searchEngine.results = results1;
            label3.Hide();
        }

        private  void Button1_ClickAsync(object sender, EventArgs e)
        {
            if (results1.HasResults)
            {
                DialogResult confirm = MessageBox.Show(this, "This will clear everyting in the results control", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) return;
            }

            label3.Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if(searchEngine.isSearching)
            {
                searchEngine.stopSearch = true; // Stop search
                button1.Enabled = true;
                searchEngine.isSearching = false;

                backgroundWorker1.CancelAsync();
               
                Text = "Thothi";

                label3.Hide();

                MessageBox.Show("Search successfully stopped");
            }
            
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            searchEngine.CaseSensitive = checkBox2.Checked;
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            searchEngine.Regex = checkBox3.Checked;
        }

        private async void BackgroundWorker1_DoWorkAsync(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            results1.Invoke(new MethodInvoker(() => {
                results1.Clear();
                searchEngine.CaseSensitive = checkBox2.Checked;
                searchEngine.Regex = checkBox3.Checked;

                results1.searchPhrase = textBox1.Text;
                button1.Enabled = false;

                Text = "Thothi (Searching...)";
            }));

            try
            {
                
                await searchEngine.SearchDirectoriesAsync(folderBrowserDialog1.SelectedPath, checkBox1.Checked, textBox1.Text);
            }
            catch (Exception ed)
            {
                MessageBox.Show(ed.Message);
                MessageBox.Show("Something made the search to stop...");
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(searchEngine.isSearching == true)
            {
                button1.Enabled = true;
                searchEngine.isSearching = false;
                Text = "Thothi";

                label3.Hide();

                MessageBox.Show("Search Complete");
            }
            
        }

    }
}
