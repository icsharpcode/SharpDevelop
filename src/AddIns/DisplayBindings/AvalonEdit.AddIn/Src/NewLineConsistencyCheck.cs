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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

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
				SD.MainThread.InvokeAsyncAndForget(() => ShowInconsistentWarning(numLF > numCRLF));
			}
		}
		
		IOverlayUIElement groupBox;
		Button normalizeButton, cancelButton;
		RadioButton windows, unix;
		
		void ShowInconsistentWarning(bool preferUnixNewLines)
		{
			if (editor.Document == null)
				return; // editor was disposed
			
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
			
			var content = new StackPanel {
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
			
			groupBox = editor.GetService<IEditorUIService>().CreateOverlayUIElement(content);
			groupBox.Title = ResourceService.GetString("AddIns.AvalonEdit.InconsistentNewlines.Header");
			
			var featureUse = SD.AnalyticsMonitor.TrackFeature(typeof(NewLineConsistencyCheck));
			
			EventHandler removeWarning = null;
			removeWarning = delegate {
				groupBox.Remove();
				editor.PrimaryTextEditor.TextArea.Focus();
				editor.LoadedFileContent -= removeWarning;
				
				featureUse.EndTracking();
			};
			
			editor.LoadedFileContent += removeWarning;
			cancelButton.Click += delegate {
				SD.AnalyticsMonitor.TrackFeature(typeof(NewLineConsistencyCheck), "cancelButton");
				removeWarning(null, null);
			};
			normalizeButton.Click += delegate {
				SD.AnalyticsMonitor.TrackFeature(typeof(NewLineConsistencyCheck), "normalizeButton");
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
