// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PythonBinding
{
	public class PythonLanguageBinding : DefaultLanguageBinding
	{
		public override IFormattingStrategy FormattingStrategy {
			get { return new PythonFormattingStrategy(); }
 		}
		
		public override LanguageProperties Properties {
			get { return PythonLanguageProperties.Default; }
 		}
	}
}
