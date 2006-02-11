
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace AlternateEditor
{
	public class Editor : AbstractViewContent
	{
		RichTextBox rtb = new RichTextBox();
		
		public override Control Control {
			get {
				return rtb;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return rtb.ReadOnly;
			}
		}
		
		public Editor()
		{
			rtb.Dock = DockStyle.Fill;
		}
		
		public override void RedrawContent()
		{
			rtb.Refresh();
		}
		
		public override void Dispose()
		{
			rtb.Dispose();
		}
		
		public override void Save(string fileName)
		{
			rtb.SaveFile(fileName, RichTextBoxStreamType.PlainText);
			TitleName = fileName;
			IsDirty     = false;
		}
		
		public override void Load(string fileName)
		{
			rtb.LoadFile(fileName, RichTextBoxStreamType.PlainText);
			TitleName = fileName;
			IsDirty     = false;
		}
	}
}
