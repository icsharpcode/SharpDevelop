using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

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
            throw new NotImplementedException("This feature has been removed");
        }

        [WebMethod]
        public string[] RetrieveAvailableHighlighters()
        {
            throw new NotImplementedException("This feature has been removed");
        }
    }
}
