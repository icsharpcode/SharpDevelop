// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of NewLineConsistencyCheck.
	/// </summary>
	public class NewLineConsistencyCheck
	{
		public static void StartConsistencyCheck(CodeEditor editor)
		{
			NewLineConsistencyCheck c = new NewLineConsistencyCheck(editor);
			ThreadPool.QueueUserWorkItem(c.CheckNewLinesForConsistency);
		}
		
		CodeEditor editor;
		ITextSource snapshot;
		
		private NewLineConsistencyCheck(CodeEditor editor)
		{
			this.editor = editor;
			this.snapshot = editor.Document.CreateSnapshot();
		}
		
		void CheckNewLinesForConsistency(object state)
		{
			int numCRLF = 0;
			int numCR = 0;
			int numLF = 0;
			
			int offset = 0;
			while (offset >= 0) {
				string type;
				offset = TextUtilities.FindNextNewLine(snapshot, offset, out type);
				if (type != null) {
					offset += type.Length;
					switch (type) {
						case "\r\n":
							numCRLF++;
							break;
						case "\n":
							numLF++;
							break;
						case "\r":
							numCR++;
							break;
					}
				}
			}
			snapshot = null; // we don't need the snapshot anymore, allow the GC to collect it
			
			// don't allow mac-style newlines; accept either unix or windows-style newlines but avoid mixing them
			bool isConsistent = (numCR == 0) && (numLF == 0 || numCRLF == 0);
			if (!isConsistent) {
				SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall(ShowInconsistentWarning, numLF > numCRLF);
			}
		}
		
		
		GroupBox groupBox;
		Button normalizeButton, cancelButton;
		RadioButton windows, unix;
		
		void ShowInconsistentWarning(bool preferUnixNewLines)
		{
			if (editor.Document == null)
				return; // editor was disposed
			
			groupBox = new GroupBox();
			groupBox.Background = SystemColors.WindowBrush;
			groupBox.Foreground = SystemColors.WindowTextBrush;
			groupBox.Header = ResourceService.GetString("AddIns.AvalonEdit.InconsistentNewlines.Header");
			groupBox.HorizontalAlignment = HorizontalAlignment.Right;
			groupBox.VerticalAlignment = VerticalAlignment.Bottom;
			groupBox.MaxWidth = 300;
			groupBox.Margin = new Thickness(0, 0, 20, 20);
			Grid.SetRow(groupBox, 1);
			
			windows = new RadioButton {
				IsChecked = !preferUnixNewLines,
				Content = ResourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton"),
				Margin = new Thickness(0, 0, 8, 0)
			};
			unix = new RadioButton {
				IsChecked = preferUnixNewLines,
				Content = ResourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton")
			};
			
			normalizeButton = new Button { Content = ResourceService.GetString("AddIns.AvalonEdit.InconsistentNewlines.Normalize") };
			cancelButton = new Button { Content = ResourceService.GetString("Global.CancelButtonText") };
			
			groupBox.Content = new StackPanel {
				Children = {
					new TextBlock {
						Text = ResourceService.GetString("AddIns.AvalonEdit.InconsistentNewlines.Description"),
						TextWrapping = TextWrapping.WrapWithOverflow
					},
					windows,
					unix,
					new StackPanel {
						Margin = new Thickness(0, 2, 0, 0),
						Orientation = Orientation.Horizontal,
						Children = { normalizeButton, cancelButton }
					}
				}
			};
			editor.Children.Add(groupBox);
			
			var featureUse = AnalyticsMonitorService.TrackFeature(typeof(NewLineConsistencyCheck));
			
			EventHandler removeWarning = null;
			removeWarning = delegate {
				editor.Children.Remove(groupBox);
				editor.PrimaryTextEditor.TextArea.Focus();
				editor.LoadedFileContent -= removeWarning;
				
				featureUse.EndTracking();
			};
			
			editor.LoadedFileContent += removeWarning;
			cancelButton.Click += delegate {
				AnalyticsMonitorService.TrackFeature(typeof(NewLineConsistencyCheck), "cancelButton");
				removeWarning(null, null);
			};
			normalizeButton.Click += delegate {
				AnalyticsMonitorService.TrackFeature(typeof(NewLineConsistencyCheck), "normalizeButton");
				removeWarning(null, null);
				
				TextDocument document = editor.Document;
				string newNewLine = (unix.IsChecked == true) ? "\n" : "\r\n";
				using (document.RunUpdate()) {
					for (int i = 1; i <= document.LineCount; i++) {
						// re-fetch DocumentLine for every iteration because we're modifying the newlines so that DocumentLines might get re-created
						DocumentLine line = document.GetLineByNumber(i);
						if (line.DelimiterLength > 0) {
							int endOffset = line.EndOffset;
							if (document.GetText(endOffset, line.DelimiterLength) != newNewLine)
								document.Replace(endOffset, line.DelimiterLength, newNewLine);
						}
					}
				}
			};
		}
	}
}
