using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeFormatServiceClient.ICSharpCode.CodeFormat;

namespace CodeFormatServiceClient
{
    public partial class CodeFormatForm : Form
    {
        CodeFormatService cfs = null;

        public CodeFormatForm()
        {
            InitializeComponent();
        }

        private void CodeFormatForm_Load(object sender, EventArgs e)
        {
            // yes, a stupid idea to do this synchronous - sample only
            cfs = new CodeFormatService();
            string[] serviceHighlighters = cfs.RetrieveAvailableHighlighters();

            for (int i = 0; i < serviceHighlighters.Length; i++)
                availableHighlighters.Items.Add(serviceHighlighters[i]);

            availableHighlighters.SelectedIndex = availableHighlighters.FindStringExact("C#");
        }

        private void buttonFormatCode_Click(object sender, EventArgs e)
        {
            string htmlSource = cfs.Format(sourceTextDocument.Text,
                availableHighlighters.Items[availableHighlighters.SelectedIndex].ToString(), false);
            htmlOutput.Text = htmlSource;
            formatPreview.DocumentText = htmlSource;
            tabctrlMain.SelectedTab = tabPagePreview;
        }
    }
}