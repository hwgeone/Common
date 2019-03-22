using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace Infrastructure.WordHepler
{
    public static class WordHelper
    {
        public static MemoryStream ReplaceText(string filePath, Dictionary<string,string> dics)
        {
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            using (DocX document = DocX.Load(filePath))
            {
                foreach (var dic in dics)
                {
                    var regularBookmark = document.Bookmarks[dic.Key];
                    if (regularBookmark != null)
                    {
                        regularBookmark.SetText(dic.Value);
                    }
                }
                MemoryStream sm = new MemoryStream();
                document.SaveAs(sm);
                return sm;
            }
        }
    }
}
