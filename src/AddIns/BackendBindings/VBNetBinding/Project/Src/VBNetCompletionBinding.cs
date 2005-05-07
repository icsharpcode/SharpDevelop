// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace VBNetBinding
{
	public class VBNetCompletionBinding : DefaultCodeCompletionBinding
	{
		public VBNetCompletionBinding() : base(".vb")
		{
		}
	}
}
