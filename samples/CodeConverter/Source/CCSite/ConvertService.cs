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

    public ConvertService () {

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
        string convertedSource = "", errorMessage = "";
        bool bSuccessfulConversion = false;
        IConvertCode currentConverter = null;

        switch (TypeOfConversion)
        {
            case "cs2boo":
                currentConverter = new ConvertCSharpToBoo();
                break;
            case "vbnet2boo":
                currentConverter = new ConvertVbNetToBoo();
                break;
            case "cs2vbnet":
                currentConverter = new ConvertCSharpSnippetToVbNet();
                break;
            case "vbnet2cs":
                currentConverter = new ConvertVbNetSnippetToCSharp();
                break;
            default:
                return false;
        }

        try
        {
            bSuccessfulConversion = currentConverter.Convert(SourceCode,
                        out convertedSource,
                        out errorMessage);
        }
        catch (Exception ex)
        {
            bSuccessfulConversion = false;
            errorMessage = "Exception occured: " + ex.ToString() + "\r\n\r\nError Message:" + errorMessage;
        }

        if (bSuccessfulConversion)
        {
            ConvertedCode = convertedSource;
        }
        else
        {
            ErrorMessage = errorMessage;
        }

        return bSuccessfulConversion;
    }
}

