/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 25.08.2008
 * Time: 09:37
 */
using System;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Project;

namespace Debugger.AddIn.Service
{
	/// <summary>
	/// Description of EditBreakpointScriptForm.
	/// </summary>
	public partial class EditBreakpointScriptForm : Form
	{
		BreakpointBookmark data;
		
		public BreakpointBookmark Data {
			get { return data; }
		}
		
		public EditBreakpointScriptForm(BreakpointBookmark data)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.data = data;
			
			this.data.Action = BreakpointAction.Condition;
			
			this.txtCode.Document.TextContent = data.Condition;
			this.cmbLanguage.Items.AddRange(new string[] { "C#", "VBNET" });
			this.cmbLanguage.SelectedIndex = 
				(!string.IsNullOrEmpty(data.ScriptLanguage)) ? 
				this.cmbLanguage.Items.IndexOf(data.ScriptLanguage.ToUpper()) :
				this.cmbLanguage.Items.IndexOf(ProjectService.CurrentProject.Language.ToUpper());
			this.txtCode.SetHighlighting(data.ScriptLanguage.ToUpper());
			
			// Setup translation text
			this.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.ScriptingWindow.Title}");
			
			this.btnCancel.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			this.btnOK.Text = StringParser.Parse("${res:Global.OKButtonText}");
			
			this.label1.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.ScriptingWindow.ScriptingLanguage}") + ":";
			this.btnCheckSyntax.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.Conditional.Breakpoints.ScriptingWindow.CheckSyntax}");
		}
		
		void CmbLanguageSelectedIndexChanged(object sender, EventArgs e)
		{
			this.txtCode.SetHighlighting(this.cmbLanguage.SelectedItem.ToString());
			this.data.ScriptLanguage = this.cmbLanguage.SelectedItem.ToString();
		}
		
		void BtnCheckSyntaxClick(object sender, EventArgs e)
		{
			CheckSyntax();
		}

		bool CheckSyntax()
		{
			SupportedLanguage language = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), this.cmbLanguage.SelectedItem.ToString().Replace("#", "Sharp"), true);
			using (IParser parser = ParserFactory.CreateParser(language, new StringReader(this.txtCode.Document.TextContent))) {
				parser.ParseExpression();
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(parser.Errors.ErrorOutput);
					return false;
				}
			}
			
			return true;
		}

		void BtnOKClick(object sender, EventArgs e)
		{
			if (!this.CheckSyntax())
				return;
			this.data.Condition = this.txtCode.Document.TextContent;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
