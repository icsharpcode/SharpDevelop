using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

using CodeConvertServiceClient.ICSharpCode.OnlineConverter;

namespace CodeConvertServiceClient
{
    public partial class CodeConversionForm : Form
    {
        StringDictionary ConversionTypes;

        public CodeConversionForm()
        {
            InitializeComponent();

            ConversionTypes = new StringDictionary();
            ConversionTypes.Add("C# to VB.NET", "cs2vbnet");
            ConversionTypes.Add("VB.NET to C#", "vbnet2cs");
            ConversionTypes.Add("C# to Boo", "cs2boo");
            ConversionTypes.Add("VB.NET to Boo", "vbnet2boo");
        }

        private void CodeConversionForm_Load(object sender, EventArgs e)
        {
            typeOfConversion.SelectedIndex = 0;
        }

        private void performConversion_Click(object sender, EventArgs e)
        {
            string buttonText = "";

            try
            {
                ConvertService cs = new ConvertService();
                string convertedCode = "", errorMessage = "";
                string conversionType = ConversionTypes[typeOfConversion.Items[typeOfConversion.SelectedIndex].ToString()];

                buttonText = performConversion.Text;
                performConversion.Text = "waiting...";
                performConversion.Enabled = false;

                bool bResult = cs.PerformConversion(conversionType, inputSource.Text,
                    out convertedCode, out errorMessage);

                if (bResult)
                {
                    outputTextBox.Text = convertedCode.Replace("\n", "\r\n");
                }
                else
                {
                    outputTextBox.Text = errorMessage.Replace("\n", "\r\n");
                }
            }
            catch (Exception ex)
            {
                // Failure is reported via dialog box, but not presented in a "nice way" - after all, this is for developers
                MessageBox.Show("Web Service failed: " + ex.ToString());
            }
            finally
            {
                performConversion.Text = buttonText;
                performConversion.Enabled = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://codeconverter.sharpdevelop.net/SnippetConverter.aspx");
        }

    }
}