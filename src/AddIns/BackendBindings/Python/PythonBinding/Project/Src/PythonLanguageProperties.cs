// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
