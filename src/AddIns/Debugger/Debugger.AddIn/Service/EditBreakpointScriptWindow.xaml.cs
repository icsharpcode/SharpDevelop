// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.Service
{
	/// <summary>
	/// Interaction logic for EditBreakpointScriptWindow.xaml
	/// </summary>
	public partial class EditBreakpointScriptWindow : Window
	{
		BreakpointBookmark data;
		
		public BreakpointBookmark Data {
			get { return data; }
		}
		
		public EditBreakpointScriptWindow(BreakpointBookmark data)
		{
			InitializeComponent();
			
			this.data = data;
			this.data.Action = BreakpointAction.Condition;
			
			foreach (var name in Enum.GetNames(typeof(SupportedLanguage)))
				cmbLanguage.Items.Add(name);
			
			string language = "CSharp";
			
			if (ProjectService.CurrentProject != null)
				language = ProjectService.CurrentProject.Language.Replace("#", "Sharp");
			
			this.cmbLanguage.SelectedIndex = 
				(!string.IsNullOrEmpty(data.ScriptLanguage)) ? 
				this.cmbLanguage.Items.IndexOf(data.ScriptLanguage) :
				this.cmbLanguage.Items.IndexOf(language);
			
			this.codeEditor.Document.Text = data.Condition;
			
			UpdateHighlighting();
		}
		
		void UpdateHighlighting()
		{
			codeEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(this.cmbLanguage.SelectedItem.ToString().Replace("Sharp", "#"));
		}
		
		bool CheckSyntax()
		{
			SupportedLanguage language = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), this.cmbLanguage.SelectedItem.ToString(), true);
			using (var parser = ParserFactory.CreateParser(language, new StringReader(this.codeEditor.Document.Text))) {
				parser.ParseExpression();
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(parser.Errors.ErrorOutput);
					return false;
				}
			}
			
			return true;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			if (!this.CheckSyntax())
				return;
			this.data.Condition = this.codeEditor.Document.Text;
			DialogResult = true;
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
		
		void CmbLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.data.ScriptLanguage = this.cmbLanguage.SelectedValue.ToString();
			UpdateHighlighting();
		}
		
		void BtnCheckSyntaxClick(object sender, RoutedEventArgs e)
		{
			CheckSyntax();
		}
	}
}
