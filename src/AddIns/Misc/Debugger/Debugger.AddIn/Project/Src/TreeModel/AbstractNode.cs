// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Drawing;

namespace Debugger.AddIn.TreeModel
{
	public abstract class AbstractNode
	{
		Image  image = null;
		string name  = string.Empty;
		string text  = string.Empty;
		string type  = string.Empty;
		IEnumerable<AbstractNode> childNodes = null;
		
		public Image Image {
			get { return image; }
			protected set { image = value; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		public string Type {
			get { return type; }
			set { type = value; }
		}
		
		public IEnumerable<AbstractNode> ChildNodes {
			get { return childNodes; }
			set { childNodes = value; }
		}
	}
}
