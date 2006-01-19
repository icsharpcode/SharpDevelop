// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.PInvokeAddIn.WebServices;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
		
		const char BackspaceCharacter = (char)0x08;
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
			functionNameComboBox.KeyPress += new KeyPressEventHandler(FunctionNameComboBoxKeyPress);

			moduleNameComboBox = ((ComboBox)ControlDictionary["ModuleNameComboBox"]);
			moduleNameComboBox.KeyPress += new KeyPressEventHandler(ModuleNameComboBoxKeyPress);
		
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
				TextEditorControl textEditor = GetTextEditorControl();
				if (textEditor != null) {
					generator.Generate(textEditor.ActiveTextAreaControl.TextArea, signature);
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
		
		void FunctionNameComboBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			Autocomplete(functionNameComboBox, e);
		}
		
		void ModuleNameComboBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			Autocomplete(moduleNameComboBox, e);
		}		
		
		void Autocomplete(ComboBox comboBox, KeyPressEventArgs e)
		{
			e.Handled = true;
			string searchText = String.Empty;
				
			if (e.KeyChar == BackspaceCharacter) {
				if ((comboBox.SelectionStart == 1) || (comboBox.SelectionStart == 0)) {
					comboBox.Text = String.Empty;
					comboBox.SelectionStart = 0;
					
				} else {
					comboBox.Text = comboBox.Text.Substring(0, comboBox.SelectionStart - 1);
					comboBox.SelectionStart = comboBox.Text.Length;
					searchText = GetComboBoxText(comboBox);
				}
			} else {
				searchText = String.Concat(GetComboBoxText(comboBox), e.KeyChar);
				comboBox.Text = searchText;
				comboBox.SelectionStart = comboBox.Text.Length;
			}
			
			if (searchText.Length > 0) {
				
				int index = comboBox.FindString(searchText);
				
				if (index != -1) {
					comboBox.SelectedIndex = index;
					comboBox.Text = (string)comboBox.Items[index];
               		comboBox.Select(searchText.Length, comboBox.Text.Length - (searchText.Length));
 				} else {
					comboBox.Text = searchText;
					comboBox.SelectionStart = comboBox.Text.Length;
				}
			}
		}
		
		/// <summary>
		/// Gets the combo box text that has been typed in by the user
		/// ignoring any autocomplete text.
		/// </summary>
		/// <param name="comboBox">A combo box control.</param>
		/// <returns>
		/// The combo box text that has been typed in by the user.
		/// </returns>
		string GetComboBoxText(ComboBox comboBox)
		{
			string comboBoxText = String.Empty;
			
			if (comboBox.SelectionStart > 0) {
				comboBoxText = comboBox.Text.Substring(0, comboBox.SelectionStart);
			}
			return comboBoxText;
		}

		string GetSourceFileLanguage()
		{
			TextEditorControl textEditor = GetTextEditorControl();
			if (textEditor != null) {
				string fileExtension = Path.GetExtension(textEditor.ActiveTextAreaControl.TextArea.MotherTextEditorControl.FileName);
				if (fileExtension.Equals(".vb", StringComparison.OrdinalIgnoreCase)) {
					return "VB";
				}
			}
			return "C#";
		}
		
		void MoreInfoLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(pinvokeUrl);
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
		
		static TextEditorControl GetTextEditorControl()
		{
			TextEditorControl textEditorControl = null;
			
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if ((window != null) && (window.ViewContent is ITextEditorControlProvider)) {
				textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			}

			return textEditorControl;
		}
	}
}
