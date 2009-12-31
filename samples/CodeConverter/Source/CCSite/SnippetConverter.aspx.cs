using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
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
        
        string typeOfConversion = languageChoice.SelectedValue;
        string sourceCode = inputTextBox.Text.Trim();

        bool result = CodeConversionHelpers.ConvertSnippet(typeOfConversion, sourceCode, out convertedSource, out errorMessage);

        if (result)
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
