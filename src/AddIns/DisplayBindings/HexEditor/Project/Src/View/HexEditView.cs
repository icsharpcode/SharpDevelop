// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
		}

		public override System.Windows.Forms.Control Control {
			get { return hexEditContainer; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			this.hexEditContainer.SaveFile(file, stream);
			this.TitleName = Path.GetFileName(file.FileName);
			this.TabPageText = this.TitleName;
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
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
			if (hexEditContainer.HasSomethingSelected) ClipboardWrapper.SetText(hexEditContainer.Cut());
		}
		
		public void Copy()
		{
			if (hexEditContainer.HasSomethingSelected) ClipboardWrapper.SetText(hexEditContainer.Copy());
		}
		
		public void Paste()
		{
			hexEditContainer.Paste(ClipboardWrapper.GetText());
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
