// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;

namespace Plugins.Wizards.MessageBoxBuilder.Command {
	
	public class WizardCommand : AbstractMenuCommand
	{
		const string WizardPath = "Plugins/Wizards/MessageBoxBuilderWizard";
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
					
					
			Properties customizer = new Properties();
			Plugins.Wizards.MessageBoxBuilder.Generator.MessageBoxGenerator generator = new Plugins.Wizards.MessageBoxBuilder.Generator.MessageBoxGenerator();
			customizer.Set("Generator", generator);
			string name = window.ViewContent.IsUntitled ? window.ViewContent.UntitledName :window.ViewContent.FileName;
			string language = Path.GetExtension(name).ToLower() == ".cs" ? "C#" : "VBNET";
			customizer.Set("Language",  language);
			
			using (WizardDialog wizard = new WizardDialog("MessageBox Wizard", customizer, WizardPath)) {
				if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
					
					if (textarea == null) {
						return;
					}
					
					string generatedText = generator.GenerateCode(language);
					int startLine = textarea.ActiveTextAreaControl.TextArea.Caret.Line;
					textarea.ActiveTextAreaControl.TextArea.InsertString(generatedText);
					textarea.Document.FormattingStrategy.IndentLines(textarea.ActiveTextAreaControl.TextArea, startLine, textarea.ActiveTextAreaControl.TextArea.Caret.Line);
				}
			}
		}
	}
}
