// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
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
