// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3497 $</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class WpfPrimaryDisplayBinding : TextEditorDisplayBinding
	{
		protected override TextEditorDisplayBindingWrapper CreateWrapper(OpenedFile file)
		{
			return new WpfPrimaryViewContent(file);
		}
	}
}
