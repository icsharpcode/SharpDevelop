using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using ICSharpCode.HtmlSyntaxColorizer;
using ICSharpCode.TextEditor.Document;

namespace CCSite
{
    /// <summary>
    /// Summary description for CodeFormatService
    /// </summary>
    [WebService(Namespace = "http://codeconverter.sharpdevelop.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class CodeFormatService : System.Web.Services.WebService
    {

        [WebMethod]
        public string Format(string Document, string HighlighterName, bool IncludeLineNumbers)
        {
            HtmlWriter writer = new HtmlWriter();
            writer.ShowLineNumbers = IncludeLineNumbers;
            writer.AlternateLineBackground = false;

            string generatedHtml = writer.GenerateHtml(Document, HighlighterName);
            return "<html><body>" + generatedHtml + "</body></html>";
        }

        [WebMethod]
        public string[] RetrieveAvailableHighlighters()
        {
            Hashtable ht = HighlightingManager.Manager.HighlightingDefinitions;
            string[] highlighters = new string[ht.Count];

            int currentElement = 0;
            foreach (DictionaryEntry de in ht)
            {
                highlighters[currentElement++] = de.Key.ToString();
            }

            return highlighters;
        }
    }
}
