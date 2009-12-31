using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

using ICSharpCode.CodeConversion;


/// <summary>
/// Summary description for ConvertService
/// </summary>
[WebService(Namespace = "http://developer.sharpdevelop.net/CodeConverter.NET/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ConvertService : System.Web.Services.WebService {

    public ConvertService () 
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool PerformConversion(string TypeOfConversion, string SourceCode, out string ConvertedCode, out string ErrorMessage) 
    {
        return ConvertSnippet(TypeOfConversion, SourceCode, out ConvertedCode, out ErrorMessage);
    }

    // a simple wrapper to streamline method names
    [WebMethod]
    public bool Convert(string TypeOfConversion, string SourceCode, out string ConvertedCode, out string ErrorMessage)
    {
        return ConvertSnippet(TypeOfConversion, SourceCode, out ConvertedCode, out ErrorMessage);
    }

    [WebMethod]
    public bool ConvertSnippet(string TypeOfConversion, string SourceCode, out string ConvertedCode, out string ErrorMessage)
    {
        ErrorMessage = ConvertedCode = "";

        bool result = CodeConversionHelpers.ConvertSnippet(TypeOfConversion, SourceCode, out ConvertedCode, out ErrorMessage);
        
        return result;
    }
}

