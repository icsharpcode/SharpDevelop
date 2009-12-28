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

namespace ICSharpCode.RubyBinding
{
	public class RubyLanguageProperties : LanguageProperties
	{
		static readonly RubyLanguageProperties defaultProperties = new RubyLanguageProperties();
		
		public RubyLanguageProperties() : base(StringComparer.Ordinal)
		{
		}
		
		public static RubyLanguageProperties Default {
			get { return defaultProperties; }
		}
		
		public override CodeDomProvider CodeDomProvider {
			get { return new CSharpCodeProvider(); }
		}
	}
}
