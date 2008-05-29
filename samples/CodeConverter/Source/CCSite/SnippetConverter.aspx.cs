using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using ICSharpCode.CodeConversion;

public partial class SnippetConverterPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void convertCode_Click(object sender, EventArgs e)
    {
        string convertedSource = "", errorMessage = "";
        bool bSuccessfulConversion = false;
        IConvertCode currentConverter = null;

        try
        {
            switch (languageChoice.SelectedValue)
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
            }

            bSuccessfulConversion = currentConverter.Convert(inputTextBox.Text,
                        out convertedSource,
                        out errorMessage);
        }
        catch (Exception ex)
        {
            OutputLabel.Text = "Exception occured, please report in the bug reporting forum";
            outputTextBox.Text = ex.ToString();
            return;
        }

        if (bSuccessfulConversion)
        {
            OutputLabel.Text = "Converted Sourcecode";
            outputTextBox.Text = convertedSource;
        }
        else
        {
            OutputLabel.Text = "Conversion error, reason:";
            outputTextBox.Text = errorMessage;
        }
    }
}
