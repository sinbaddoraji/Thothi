using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thothi
{
    public partial class Form1 : Form
    {
        SearchEngine searchEngine = new SearchEngine();

        public Form1()
        {
            InitializeComponent();
            searchEngine.results = results1;
            searchEngine.progressBar = progressBar1;
            searchEngine.SearchComplete = SearchComplete;
        }

        private void SearchComplete()
        {
            button1.Enabled = true;
        }
        private async void Button1_ClickAsync(object sender, EventArgs e)
        {
            if(!results1.HasResults) goto SearchJob;

            var confirm = MessageBox.Show(this,"This will clear everyting in the results control","Are you sure?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(confirm == DialogResult.No) return;

            results1.ClearResults();
            searchEngine.CaseSensitive = checkBox2.Checked; 
            searchEngine.Regex = checkBox3.Checked;

            SearchJob:
            try
            {
                button1.Enabled = false;
                await searchEngine.SearchDirectoriesAsync(folderBrowserDialog1.SelectedPath, checkBox1.Checked, textBox1.Text);
            }
            catch
            {
                SearchComplete();
                MessageBox.Show("Something made the search to stop...");
            }
            
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                textBox2.Text = folderBrowserDialog1.SelectedPath;
        }

        private void Button3_Click(object sender, EventArgs e) => searchEngine.stopSearch = true; // Stop search

        private void CheckBox2_CheckedChanged(object sender, EventArgs e) => searchEngine.CaseSensitive = checkBox2.Checked;

        private void CheckBox3_CheckedChanged(object sender, EventArgs e) => searchEngine.Regex = checkBox3.Checked;
    }
}
