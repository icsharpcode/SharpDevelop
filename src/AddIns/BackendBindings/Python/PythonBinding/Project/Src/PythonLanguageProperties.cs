// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.CSharp;

namespace ICSharpCode.PythonBinding
{
	public class PythonLanguageProperties : LanguageProperties
	{
		static readonly PythonLanguageProperties defaultProperties = new PythonLanguageProperties();
		
		public PythonLanguageProperties() : base(StringComparer.Ordinal)
		{
		}
		
		public static PythonLanguageProperties Default {
			get { return defaultProperties; }
		}
		
		public override CodeDomProvider CodeDomProvider {
			get { return new CSharpCodeProvider(); }
		}
		
		public override bool AllowObjectConstructionOutsideContext {
			get { return true; }
		}
	}
}
