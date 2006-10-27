// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	abstract class NodeOptionPanel : BaseSharpDevelopUserControl
	{
		protected AbstractNode parent;
		
		public AbstractNode ParentNode {
			get {
				return parent;
			}
		}
		
		public NodeOptionPanel(AbstractNode Parent) {
			parent = Parent;
			
			this.Dock = DockStyle.Fill;
			this.ClientSize = new Size(320, 392);

		}
		
		public virtual bool ValidateSettings()
		{
			return true;
		}
		
		
		
		protected void ValidationMessage(string str)
		{
			MessageService.ShowWarning("${res:Dialog.HighlightingEditor.ValidationError}\n\n" + str);
		}

		protected static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
			
		protected static void PreviewUpdate(Label label, EditorHighlightColor color)
		{
			if (label == null) return;
			
			if (color == null) {
				label.ForeColor = label.BackColor = Color.Transparent;
				return;
			}
			if (color.NoColor) {
				label.ForeColor = label.BackColor = Color.Transparent;
				return;
			}
			
			label.ForeColor = color.GetForeColor();
			label.BackColor = color.GetBackColor();
			
			FontStyle fs = FontStyle.Regular;
			if (color.Bold)   fs |= FontStyle.Bold;
			if (color.Italic) fs |= FontStyle.Italic;
			
			label.Font = new Font(label.Font, fs);
		}
		
		public abstract void StoreSettings();
		public abstract void LoadSettings();
	}
}
