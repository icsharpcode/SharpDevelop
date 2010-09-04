// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class TestableScriptingDesignerGenerator : ScriptingDesignerGenerator
	{
		public TestableScriptingDesignerGenerator(ITextEditorOptions textEditorOptions)
			: base(textEditorOptions)
		{
		}
	}
}
