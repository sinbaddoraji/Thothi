using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        private readonly FileHandler fileHandler = new FileHandler();

        public bool stopSearch = false;
        public bool isSearching = false;

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

        string[] GetFiles(string path) => Directory.GetFiles(path, "*.pdf", SearchOption.TopDirectoryOnly);


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

        private async Task<bool> SearchDirectoryBlock(List<string> files,string searchPhrase)
        {
            List<FindDetails> output = new List<FindDetails>();

            for (int i = 0; i < files.Count; i++)
            {
                if (stopSearch == true)
                {
                    return stopSearch = false;
                }

                try
                {
                    if (!fileHandler.Supports(files[i])) continue;
                    else
                    {
                        FindDetails result = await SearchDocument(files[i], searchPhrase);

                        if (result == null) continue;

                        results.Invoke(new MethodInvoker(() => {
                            results.AddResult(result);
                        }));
                    }
                }
                catch
                {
                    continue;
                }
            }

            return true;
        }

        
        private async void SearchInnerDirectoriesAsync(string directory, string searchPhrase)
        {
            var directories = Directory.GetDirectories(directory);
               
            for (int i = 0; i < directories.Length; i++)
            {
                if(stopSearch == true) break;
                try
                {
                    await SearchDirectoriesAsync(directories[i],true,searchPhrase);
                }
                catch
                {
                    //Ignore
                }
            }
        }


        public async Task<bool> SearchDirectoriesAsync(string directory, bool searchSubDirs, string searchPhrase)
        {
            List<string> files = new List<string>(GetFiles(directory));
            isSearching = true;

            await SearchDirectoryBlock(files, searchPhrase);

            if (searchSubDirs)
                SearchInnerDirectoriesAsync(directory, searchPhrase);

            isSearching = false;
            return true;
        }

    }


}
