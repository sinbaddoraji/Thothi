using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thothi
{
    public class FindDetails
    {
        public string filename;
        public string searchPhrase;
        public List<int> pagesSearchFound = new List<int>();

        public FindDetails(string filename, string searchPhrase)
        {
            this.filename = filename;
            this.searchPhrase = searchPhrase;
        }
    }

    internal class SearchEngine
    {
        public Result results;
        public ProgressBar progressBar;
        private readonly FileHandler fileHandler = new FileHandler();

        public bool stopSearch = false;

        public delegate void SearchEvent();
        public SearchEvent SearchFound;
        public SearchEvent SearchComplete;

        public bool CaseSensitive
        {
            get => fileHandler.CaseSensitive;
            set => fileHandler.CaseSensitive = value;
        }
        public bool Regex
        {
            get => fileHandler.Regex;
            set => fileHandler.Regex = value;
        }

        private Task<FindDetails> SearchDocument(string file, string searchPhrase)
        {
            TaskCompletionSource<FindDetails> tcs = new TaskCompletionSource<FindDetails>();
            Task.Run(() =>
            {
                FindDetails result = fileHandler.SearchDocument(file, searchPhrase);
                //set the result to TaskCompletionSource
                tcs.SetResult(result);
            });
            //return the Task
            return tcs.Task;
        }

        private string[] GetAllFiles(string path, bool searchSubdir, string searchPattern)
        {
            string[] output;

            if (searchSubdir)
            {
                output = Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories);
            }
            else
            {
                output = Directory.GetFiles(path, "*.pdf", SearchOption.TopDirectoryOnly);
            }

            return output;
        }

        public async Task<bool> SearchDirectoriesAsync(string directory, bool searchSubDirs, string searchPhrase)
        {
            List<FindDetails> output = new List<FindDetails>();

            string[] files = GetAllFiles(directory, searchSubDirs, searchPhrase);

            progressBar.Maximum = files.Length - 1;
            progressBar.Value = progressBar.Minimum = 0;

            for (int i = 0; i < files.Length; i++)
            {
                if (stopSearch == true)
                {
                    SearchComplete();
                    progressBar.Value = 0;
                    return stopSearch = false;
                }

                try
                {
                    if (!fileHandler.Supports(files[i]))
                    {
                        continue;
                    }
                    else
                    {
                        FindDetails result = await SearchDocument(files[i], searchPhrase);

                        if (result != null)
                        {
                            results.AddResult(result);
                            SearchFound();
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            SearchComplete();
            progressBar.Value = 0;
            return true;
        }

        private void SearchFoundEventHandler()
        {
            progressBar.Value++;
        }

        public SearchEngine()
        {
            SearchFound = SearchFoundEventHandler;
        }
    }


}
