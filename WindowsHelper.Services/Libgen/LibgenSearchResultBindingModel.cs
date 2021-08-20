using System.Collections.Generic;

namespace WindowsHelper.Services.Libgen
{
    public class LibgenSearchResultBindingModel
    {
        public string Id { get; set; }
        public string[] Authors { get; set; }
        public string Title { get; set; }
        public string Size { get; set; }
        public string Year { get; set; }
        public string PageCount { get; set; }
        public string Language { get; set; }
        public string Extension { get; set; }
        public string[] Mirrors { get; set; }
        public List<string> DownloadUris { get; set; } = new();
    }
}