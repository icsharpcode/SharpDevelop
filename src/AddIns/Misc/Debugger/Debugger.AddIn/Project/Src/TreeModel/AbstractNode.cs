// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
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
			protected set { name = value; }
		}
		
		public string Text {
			get { return text; }
			protected set { text = value; }
		}
		
		public string Type {
			get { return type; }
			protected set { type = value; }
		}
		
		public IEnumerable<AbstractNode> ChildNodes {
			get { return childNodes; }
			protected set { childNodes = value; }
		}
	}
}
