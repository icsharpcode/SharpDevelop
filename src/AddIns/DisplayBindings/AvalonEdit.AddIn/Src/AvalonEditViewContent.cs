// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditViewContent : AbstractViewContent, IEditable, IMementoCapable
	{
		readonly CodeEditor codeEditor = new CodeEditor();
		
		public AvalonEditViewContent(OpenedFile file)
		{
			this.Files.Add(file);
			file.ForceInitializeView(this);
			codeEditor.Document.Changed += textEditor_Document_Changed;
		}
		
		void textEditor_Document_Changed(object sender, DocumentChangeEventArgs e)
		{
			if (!isLoading) {
				PrimaryFile.IsDirty = true;
			}
		}
		
		public CodeEditor CodeEditor {
			get { return codeEditor; }
		}
		
		public override object Content {
			get { return codeEditor; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			codeEditor.Save(stream);
		}
		
		bool isLoading;
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			isLoading = true;
			try {
				codeEditor.FileName = file.FileName;
				codeEditor.SyntaxHighlighting =
					HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(file.FileName));
				codeEditor.Load(stream);
			} finally {
				isLoading = false;
			}
		}
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			if (file == PrimaryFile)
				codeEditor.FileName = file.FileName;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			// Unload document on dispose.
			codeEditor.Document = null;
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
		}
		
		#region IEditable
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired)
					return WorkbenchSingleton.SafeThreadFunction(() => codeEditor.Text);
				else
					return codeEditor.Text;
			}
			set {
				WorkbenchSingleton.SafeThreadCall(text => codeEditor.Text = text, value);
			}
		}
		#endregion
		
		#region IMementoCapable
		public Properties CreateMemento()
		{
			Properties memento = new Properties();
			memento.Set("CaretOffset", codeEditor.CaretOffset);
			memento.Set("ScrollPositionY", codeEditor.VerticalOffset);
			return memento;
		}
		
		public void SetMemento(Properties memento)
		{
			codeEditor.ScrollToVerticalOffset(memento.Get("ScrollPositionY", 0.0));
			try {
				codeEditor.CaretOffset = memento.Get("CaretOffset", 0);
			} catch (ArgumentOutOfRangeException) {
				// ignore caret out of range - maybe file was changed externally?
			}
		}
		#endregion
	}
}
