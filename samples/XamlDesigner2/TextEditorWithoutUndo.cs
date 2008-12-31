using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.TextEditor;
using System.Windows.Forms;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.Samples.XamlDesigner
{
	class TextEditorWithoutUndo : TextEditorControl, ITextHolder
	{
		public TextEditorWithoutUndo()
		{
			editactions.Remove(Keys.Control | Keys.Z);
			editactions.Remove(Keys.Control | Keys.Y);
		}
	}
}
