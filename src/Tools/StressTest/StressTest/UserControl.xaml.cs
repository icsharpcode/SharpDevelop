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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
		
		async void Run(string name, Func<Task> process)
		{
			Stopwatch w = Stopwatch.StartNew();
			await process();
			w.Stop();
			TaskService.BuildMessageViewCategory.AppendLine(name + " (" + Repetitions + "x): " + w.Elapsed.ToString());
		}
		
		int Repetitions {
			get { return int.Parse(repetitionsTextBox.Text); }
		}
		
		void openFileButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Open File", OpenFile);
		}
		
		string bigFile = Path.Combine(FileUtility.ApplicationRootPath, @"src\Libraries\NRefactory\ICSharpCode.NRefactory.CSharp\Parser\mcs\expression.cs");
		
		const DispatcherPriority Idle = DispatcherPriority.ApplicationIdle;
		
		async Task OpenFile()
		{
			for (int i = 0; i < Repetitions; i++) {
				IViewContent newContent = FileService.OpenFile(bigFile);
				await Dispatcher.Yield(Idle);
				newContent.WorkbenchWindow.CloseWindow(true);
				await Dispatcher.Yield(Idle);
			}
		}
		
		void TypeTextButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Type Comment In C# file", () => TypeText(typeCommentTextBox.Text));
		}
		
		async Task TypeText(string theText)
		{
			const string csharpHeader = "using System;\n\nclass Test {\n\tpublic void M() {\n\t\t";
			const string csharpFooter = "\n\t}\n}\n";
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = vc.GetRequiredService<ITextEditor>();
			editor.Document.Text = csharpHeader + csharpFooter;
			editor.Caret.Offset = csharpHeader.Length;
			TextArea textArea = (TextArea)editor.GetService(typeof(TextArea));
			await Dispatcher.Yield(Idle);
			for (int i = 0; i < Repetitions; i++) {
				foreach (char c in "// " + theText + "\n") {
					textArea.PerformTextInput(c.ToString());
					await Dispatcher.Yield(Idle);
				}
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void EraseTextButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Erase Text In C# file", () => EraseText((eraseTextBackwards.IsChecked == true) ? EditingCommands.Backspace : EditingCommands.Delete));
		}
		
		async Task EraseText(RoutedUICommand deleteCommand)
		{
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = vc.GetRequiredService<ITextEditor>();
			TextArea textArea = editor.GetRequiredService<TextArea>();
			editor.Document.Text = File.ReadAllText(bigFile);
			editor.Caret.Offset = editor.Document.TextLength / 2;
			await Dispatcher.Yield(Idle);
			for (int i = 0; i < Repetitions; i++) {
				deleteCommand.Execute(null, textArea);
				await Dispatcher.Yield(Idle);
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void TypeCodeButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Type Code In C# file", TypeCode);
		}
		
		async Task TypeCode()
		{
			IViewContent vc = FileService.NewFile("stresstest.cs", "");
			ITextEditor editor = vc.GetRequiredService<ITextEditor>();
			TextArea textArea = editor.GetRequiredService<TextArea>();
			string inputText = string.Join("\n", File.ReadAllLines(bigFile).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("//", StringComparison.Ordinal)));
			await Dispatcher.Yield(Idle);
			for (int i = 0; i < Math.Min(inputText.Length, Repetitions); i++) {
				textArea.PerformTextInput(inputText[i].ToString());
				await Dispatcher.Yield(Idle);
				while (!textArea.StackedInputHandlers.IsEmpty)
					textArea.PopStackedInputHandler(textArea.StackedInputHandlers.Peek());
				await Dispatcher.Yield(Idle);
			}
			vc.WorkbenchWindow.CloseWindow(true);
		}
		
		void SwitchLayoutButton_Click(object sender, RoutedEventArgs e)
		{
			Run("Switch Layout", SwitchLayout);
		}
		
		async Task SwitchLayout()
		{
			for (int i = 0; i < Repetitions; i++) {
				SD.Workbench.CurrentLayoutConfiguration = "Debug";
				await Dispatcher.Yield(Idle);
				SD.Workbench.CurrentLayoutConfiguration = "Default";
				await Dispatcher.Yield(Idle);
			}
		}
	}
}
