// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

// created on 08.09.2003 at 22:11
using System;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui {
	
	public class InputBox : BaseSharpDevelopForm
	{
		Label label;
		TextBox textBox;

		public Label Label {
			get {
				return label;
			}
		}
		public TextBox TextBox {
			get {
				return textBox;
			}
		}
		
		public InputBox()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.InputBox.xfrm"));
			
			label = (Label)ControlDictionary["label"];
			textBox = (TextBox)ControlDictionary["textBox"];
		}
	}
}
