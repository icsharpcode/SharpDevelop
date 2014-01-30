// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.PInvokeAddIn.WebServices;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Form that allows the user to find PInvoke signatures and insert
	/// them into the code.
	/// </summary>
	public class InsertPInvokeSignaturesForm : XmlForm
	{
		Button findButton;
		Button insertButton;
		Button closeButton;
		ComboBox functionNameComboBox;
		ComboBox moduleNameComboBox;
		RichTextBox signatureRichTextBox;
		ComboBox languageComboBox;
		LinkLabel moreInfoLinkLabel;
		
		SignatureInfo[] signatures;
		string allLanguages = StringParser.Parse("${res:ICSharpCode.PInvokeAddIn.InsertPInvokeSignaturesForm.AllLanguages}");
		
		const string pinvokeWebSiteUrl = "http://www.pinvoke.net/";
		
		string pinvokeUrl = pinvokeWebSiteUrl;

		public InsertPInvokeSignaturesForm()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("PInvokeAddIn.Resources.InsertPInvokeSignaturesForm.xfrm"));
			
			signatureRichTextBox = ((RichTextBox)ControlDictionary["SignatureRichTextBox"]);
			
			// Hook up events.
			closeButton = ((Button)ControlDictionary["CloseButton"]);
			closeButton.Click += new EventHandler(CloseButtonClick);
			
			insertButton = ((Button)ControlDictionary["InsertButton"]);
			insertButton.Enabled = false;
			insertButton.Click += new EventHandler(InsertButtonClick);
			
			findButton = ((Button)ControlDictionary["FindButton"]);
			findButton.Click += new EventHandler(FindButtonClick);
			
			functionNameComboBox = ((ComboBox)ControlDictionary["FunctionNameComboBox"]);
			functionNameComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
			functionNameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

			moduleNameComboBox = ((ComboBox)ControlDictionary["ModuleNameComboBox"]);
			moduleNameComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
			moduleNameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
			
			moreInfoLinkLabel = ((LinkLabel)ControlDictionary["MoreInfoLinkLabel"]);
			moreInfoLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(MoreInfoLinkClicked);

			languageComboBox = ((ComboBox)ControlDictionary["LanguageComboBox"]);
			languageComboBox.SelectedIndexChanged += new EventHandler(LanguageComboBoxSelectedIndexChanged);
			
			SetupLanguages();
			SetupFunctionNames();
			SetupModuleNames();
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
		
		/// <summary>
		/// Populates the language combo box.
		/// </summary>
		void SetupLanguages()
		{
			string[] supportedLanguages = PInvokeRepository.Instance.GetSupportedLanguages();
			
			languageComboBox.Items.Add(allLanguages);
			
			foreach (string language in supportedLanguages) {
				languageComboBox.Items.Add(language);
			}
			
			languageComboBox.SelectedIndex = 0;
		}
		
		/// <summary>
		/// Populates the function name combo box.
		/// </summary>
		void SetupFunctionNames()
		{
			string[] names = PInvokeRepository.Instance.GetFunctionNames();
			
			foreach (string name in names) {
				functionNameComboBox.Items.Add(name);
			}
		}
		
		/// <summary>
		/// Populates the module name combo box.
		/// </summary>
		void SetupModuleNames()
		{
			string[] names = PInvokeRepository.Instance.GetModuleNames();
			
			foreach (string name in names) {
				moduleNameComboBox.Items.Add(name);
			}
		}
		
		void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		/// <summary>
		/// Insert PInvoke signature into code.
		/// </summary>
		void InsertButtonClick(object sender, EventArgs e)
		{
			Close();
			PInvokeCodeGenerator generator = new PInvokeCodeGenerator();
			
			string language = languageComboBox.Text;
			if (language == allLanguages) {
				language = GetSourceFileLanguage();
			}
			
			string signature = GetSelectedPInvokeSignature(language);
			
			if (signature.Length > 0) {
				ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
				if (textEditor != null) {
					generator.Generate(textEditor, signature);
				}
			} else {
				MessageService.ShowError(String.Format(StringParser.Parse("${res:ICSharpCode.PInvokeAddIn.InsertPInvokeSignaturesForm.NoSignatureFoundForLanguage}"), language));
			}
		}
		
		void FindButtonClick(object sender, EventArgs e)
		{
			try {
				signatures = Search(functionNameComboBox.Text, moduleNameComboBox.Text);
				
				int signaturesAdded = DisplaySearchResults(languageComboBox.Text);
				
				if (signatures.Length > 0) {
					pinvokeUrl = signatures[0].Url;
				}
				
				if (signaturesAdded > 0) {
					insertButton.Enabled = true;
				} else {
					insertButton.Enabled = false;
				}
				
			} catch(Exception ex) {
				signatures = null;
				MessageService.ShowError(ex.Message);
			}
		}
		
		string GetSelectedPInvokeSignature(string language)
		{
			StringBuilder signatureBuilder = new StringBuilder();
			
			foreach (SignatureInfo info in signatures) {
				if (info.Language.Equals(language, StringComparison.OrdinalIgnoreCase)) {
					signatureBuilder.Append(GetSignature(info));
					signatureBuilder.Append("\r\n");
				}
			}
			
			return signatureBuilder.ToString();
		}
		
		SignatureInfo[] Search(string functionName, string moduleName)
		{
			PInvokeService webService = new PInvokeService();
			return webService.GetResultsForFunction(functionName, moduleName);
		}
		
		int DisplaySearchResults(string language)
		{
			signatureRichTextBox.Clear();
			
			if (signatures.Length > 0) {
				if (signatures[0].Summary.Length > 0) {
					signatureRichTextBox.Text = String.Concat(signatures[0].Summary, "\r\n\r\n");
				}
			}
			
			int signaturesAdded = 0;
			
			foreach (SignatureInfo info in signatures) {
				
				bool languageWanted = false;
				if ((language == allLanguages) || (language.Equals(info.Language, StringComparison.OrdinalIgnoreCase))) {
					languageWanted = true;
				}
				
				if (languageWanted) {
					++signaturesAdded;
					
					string signatureText = GetSignature(info);
					if (signatureText.EndsWith("\r\n")) {
						signatureRichTextBox.Text += String.Concat(signatureText, "\r\n\r\n");
					} else {
						signatureRichTextBox.Text += String.Concat(signatureText, "\r\n\r\n");
					}
				}
			}
			
			if (signaturesAdded == 0) {
				signatureRichTextBox.Text += StringParser.Parse("${res:ICSharpCode.PInvokeAddIn.InsertPInvokeSignaturesForm.NoSignaturesFound}");
			}
			
			return signaturesAdded;
		}
		
		/// <summary>
		/// Replaces the "|" in the signature string with new lines.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		string GetSignature(SignatureInfo info)
		{
			return info.Signature.Replace("|", "\r\n");
		}

		string GetSourceFileLanguage()
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor != null) {
				string fileExtension = Path.GetExtension(textEditor.FileName);
				if (fileExtension.Equals(".vb", StringComparison.OrdinalIgnoreCase)) {
					return "VB";
				}
			}
			return "C#";
		}
		
		void MoreInfoLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try {
				Process.Start(pinvokeUrl);
			} catch (Exception ex) {
				LoggingService.Warn("Cannot start " + pinvokeUrl, ex);
			}
		}

		/// <summary>
		/// Updates the displayed PInvoke signatures based on the selected
		/// language.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event arguments.</param>
		void LanguageComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (signatures != null) {
				if (signatures.Length > 0) {
					int signaturesAdded = DisplaySearchResults(languageComboBox.Text);
					if (signaturesAdded > 0) {
						insertButton.Enabled = true;
					} else {
						insertButton.Enabled = false;
					}
				}
			}
		}
	}
}
