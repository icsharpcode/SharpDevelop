// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
