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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

namespace HexEditor.View
{
	public class HexEditView : AbstractViewContent, IClipboardHandler, IUndoHandler
	{
		HexEditContainer hexEditContainer;
		
		public HexEditView(OpenedFile file)
		{
			hexEditContainer = new HexEditContainer();
			hexEditContainer.hexEditControl.DocumentChanged += new EventHandler(DocumentChanged);
			
			this.Files.Add(file);
			
			file.ForceInitializeView(this);
			
			SD.AnalyticsMonitor.TrackFeature(typeof(HexEditView));
		}

		public override object Control {
			get { return hexEditContainer; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			SD.AnalyticsMonitor.TrackFeature(typeof(HexEditView), "Save");
			this.hexEditContainer.SaveFile(file, stream);
			this.TitleName = Path.GetFileName(file.FileName);
			this.TabPageText = this.TitleName;
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			SD.AnalyticsMonitor.TrackFeature(typeof(HexEditView), "Load");
			this.hexEditContainer.LoadFile(file, stream);
		}
		
		public override bool IsReadOnly {
			get { return false; }
		}
		
		#region IClipboardHandler
		public bool EnableCut {
			get { return hexEditContainer.HasSomethingSelected & hexEditContainer.EditorFocused; }
		}
		
		public bool EnableCopy {
			get { return hexEditContainer.HasSomethingSelected & hexEditContainer.EditorFocused; }
		}
		
		public bool EnablePaste {
			get { return hexEditContainer.EditorFocused; }
		}
		
		public bool EnableDelete {
			get { return hexEditContainer.HasSomethingSelected & hexEditContainer.EditorFocused; }
		}
		
		public bool EnableSelectAll {
			get { return hexEditContainer.EditorFocused; }
		}
		
		public void Cut()
		{
			if (hexEditContainer.HasSomethingSelected) SD.Clipboard.SetText(hexEditContainer.Cut());
		}
		
		public void Copy()
		{
			if (hexEditContainer.HasSomethingSelected) SD.Clipboard.SetText(hexEditContainer.Copy());
		}
		
		public void Paste()
		{
			hexEditContainer.Paste(SD.Clipboard.GetText());
		}
		
		public void Delete()
		{
			if (hexEditContainer.HasSomethingSelected) hexEditContainer.Delete();
		}
		
		public void SelectAll()
		{
			hexEditContainer.SelectAll();
		}
		
		#endregion
		
		#region IUndoHandler
		public bool EnableUndo {
			get { return hexEditContainer.CanUndo; }
		}
		
		public bool EnableRedo {
			get { return hexEditContainer.CanRedo; }
		}
		
		public void Undo()
		{
			if (hexEditContainer.CanUndo) hexEditContainer.Undo();
		}
		
		public void Redo()
		{
			if (hexEditContainer.CanRedo) hexEditContainer.Redo();
		}
		
		#endregion
		
		void DocumentChanged(object sender, EventArgs e)
		{
			if (PrimaryFile != null) PrimaryFile.MakeDirty();
		}
		
		public override bool IsDirty {
			get { return base.IsDirty; }
		}
	}
}
