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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The text editor used in SharpDevelop.
	/// Serves both as base class for CodeEditorView and as text editor control
	/// for editors used in other parts of SharpDevelop (e.g. all ConsolePad-based controls)
	/// </summary>
	public class SharpDevelopTextEditor : TextEditor
	{
		static SharpDevelopTextEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SharpDevelopTextEditor),
			                                         new FrameworkPropertyMetadata(typeof(SharpDevelopTextEditor)));
		}
		
		protected readonly CodeEditorOptions options;
		
		public SharpDevelopTextEditor()
		{
			AvalonEditDisplayBinding.RegisterAddInHighlightingDefinitions();
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, OnPrint));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview, OnPrintPreview));
			
			options = ICSharpCode.AvalonEdit.AddIn.Options.CodeEditorOptions.Instance;
			options.BindToTextEditor(this);
		}
		
		protected virtual ICSharpCode.Core.FileName FileName {
			get { return ICSharpCode.Core.FileName.Create("untitled"); }
		}
		
		SharpDevelopCompletionWindow completionWindow;
		SharpDevelopInsightWindow insightWindow;
		
		void CloseExistingCompletionWindow()
		{
			if (completionWindow != null) {
				completionWindow.Close();
			}
		}
		
		void CloseExistingInsightWindow()
		{
			if (insightWindow != null) {
				insightWindow.Close();
			}
		}
		
		public SharpDevelopCompletionWindow ActiveCompletionWindow {
			get { return completionWindow; }
		}
		
		internal SharpDevelopInsightWindow ActiveInsightWindow {
			get { return insightWindow; }
		}
		
		internal void ShowCompletionWindow(SharpDevelopCompletionWindow window)
		{
			CloseExistingCompletionWindow();
			completionWindow = window;
			window.Closed += delegate {
				completionWindow = null;
			};
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
				delegate {
					if (completionWindow == window) {
						window.Show();
					}
				}
			));
		}
		
		internal void ShowInsightWindow(SharpDevelopInsightWindow window)
		{
			CloseExistingInsightWindow();
			insightWindow = window;
			window.Closed += delegate {
				insightWindow = null;
			};
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
				delegate {
					if (insightWindow == window) {
						window.Show();
					}
				}
			));
		}
		
		#region Printing
		void OnPrint(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			if (printDialog.ShowDialog() == true) {
				FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
				PrintPreviewViewContent.ApplySettingsToFlowDocument(printDialog, fd);
				IDocumentPaginatorSource doc = fd;
				printDialog.PrintDocument(doc.DocumentPaginator, Path.GetFileName(this.FileName));
			}
		}
		
		void OnPrintPreview(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
			PrintPreviewViewContent.ApplySettingsToFlowDocument(printDialog, fd);
			PrintPreviewViewContent.ShowDocument(fd, Path.GetFileName(this.FileName));
		}
		#endregion
	}
	
	sealed class ZoomLevelToTextFormattingModeConverter : IValueConverter
	{
		public static readonly ZoomLevelToTextFormattingModeConverter Instance = new ZoomLevelToTextFormattingModeConverter();
		
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (((double)value) == 1.0)
				return TextFormattingMode.Display;
			else
				return TextFormattingMode.Ideal;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
