// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace StressTest
{
	/// <summary>
	/// Interaction logic for UserControl.xaml
	/// </summary>
	public partial class UserControl : System.Windows.Controls.UserControl
	{
		public UserControl()
		{
			InitializeComponent();
		}
		
		void Run(string name, IEnumerable<DispatcherPriority> process)
		{
			var e = process.GetEnumerator();
			Stopwatch w = Stopwatch.StartNew();
			Action cont = null;
			cont = delegate {
				if (e.MoveNext()) {
					Dispatcher.BeginInvoke(e.Current, cont);
				} else {
					e.Dispose();
					w.Stop();
					TaskService.BuildMessageViewCategory.AppendLine(name + " (" + Repetitions + "x): " + w.Elapsed.ToString());
				}
			};
			cont();
		}
		
		int Repetitions {
			get { return int.Parse(repetitionsTextBox.Text); }
		}
		
		void openFileButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Open File", OpenFile());
		}
		
		string bigFile = Path.Combine(FileUtility.ApplicationRootPath, @"src\Libraries\NRefactory\Project\Src\Ast\Generated.cs");
		
		IEnumerable<DispatcherPriority> OpenFile()
		{
			for (int i = 0; i < Repetitions; i++) {
				IViewContent newContent = FileService.OpenFile(bigFile);
				yield return DispatcherPriority.SystemIdle;
				newContent.WorkbenchWindow.CloseWindow(true);
				yield return DispatcherPriority.SystemIdle;
			}
		}
		
		void TypeTextButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Type Comment In C# file", TypeText(typeCommentTextBox.Text));
		}
		
		IEnumerable<DispatcherPriority> TypeText(string theText)
		{
			const string csharpHeader = "using System;\n\nclass Test {\n\tpublic void M() {\n\t\t";
			const string csharpFooter = "\n\t}\n}\n";
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = ((ITextEditorProvider)vc).TextEditor;
			editor.Document.Text = csharpHeader + csharpFooter;
			editor.Caret.Offset = csharpHeader.Length;
			TextArea textArea = (TextArea)editor.GetService(typeof(TextArea));
			yield return DispatcherPriority.SystemIdle;
			for (int i = 0; i < Repetitions; i++) {
				foreach (char c in "// " + theText + "\n") {
					textArea.PerformTextInput(c.ToString());
					yield return DispatcherPriority.SystemIdle;
				}
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void EraseTextButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Erase Text In C# file", EraseText((eraseTextBackwards.IsChecked == true) ? EditingCommands.Backspace : EditingCommands.Delete));
		}
		
		IEnumerable<DispatcherPriority> EraseText(RoutedUICommand deleteCommand)
		{
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = ((ITextEditorProvider)vc).TextEditor;
			TextArea textArea = (TextArea)editor.GetService(typeof(TextArea));
			editor.Document.Text = File.ReadAllText(bigFile);
			editor.Caret.Offset = editor.Document.TextLength / 2;
			yield return DispatcherPriority.SystemIdle;
			for (int i = 0; i < Repetitions; i++) {
				deleteCommand.Execute(null, textArea);
				yield return DispatcherPriority.SystemIdle;
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void TypeCodeButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Type Code In C# file", TypeCode());
		}
		
		IEnumerable<DispatcherPriority> TypeCode()
		{
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = ((ITextEditorProvider)vc).TextEditor;
			TextArea textArea = (TextArea)editor.GetService(typeof(TextArea));
			string inputText = string.Join("\n", File.ReadAllLines(bigFile).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("//", StringComparison.Ordinal)));
			yield return DispatcherPriority.SystemIdle;
			for (int i = 0; i < Math.Min(inputText.Length, Repetitions); i++) {
				textArea.PerformTextInput(inputText[i].ToString());
				yield return DispatcherPriority.SystemIdle;
				while (!textArea.StackedInputHandlers.IsEmpty)
					textArea.PopStackedInputHandler(textArea.StackedInputHandlers.Peek());
				yield return DispatcherPriority.SystemIdle;
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void SwitchLayoutButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Switch Layout", SwitchLayout());
		}
		
		IEnumerable<DispatcherPriority> SwitchLayout()
		{
			for (int i = 0; i < Repetitions; i++) {
				LayoutConfiguration.CurrentLayoutName = "Debug";
				yield return DispatcherPriority.SystemIdle;
				LayoutConfiguration.CurrentLayoutName = "Default";
				yield return DispatcherPriority.SystemIdle;
			}
		}
	}
}