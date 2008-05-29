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

using ICSharpCode.HtmlSyntaxColorizer;
using ICSharpCode.TextEditor.Document;

namespace CCSite
{
    public partial class FormatCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack) return;

            Hashtable ht = HighlightingManager.Manager.HighlightingDefinitions;
            foreach (DictionaryEntry de in ht)
            {
                string currentKey = de.Key.ToString();
                languageChoice.Items.Add(currentKey);
            }

            languageChoice.Items.FindByValue("C#").Selected = true;
        }

        protected void formatCode_Click(object sender, EventArgs e)
        {
            HtmlWriter writer = new HtmlWriter();
            writer.ShowLineNumbers = ShowLineNumbers.Checked;
            writer.AlternateLineBackground = UseAlternatingBackground.Checked;

            string generatedHtml = writer.GenerateHtml(inputTextBox.Text, languageChoice.SelectedValue);

            string codeHtmlDocument = "<html><body>" + generatedHtml + "</body></html>";
            outputTextBox.Text = codeHtmlDocument;
            inlineCodePreview.Text = generatedHtml;
            PreviewPane.Visible = true;
        }
    }
}
