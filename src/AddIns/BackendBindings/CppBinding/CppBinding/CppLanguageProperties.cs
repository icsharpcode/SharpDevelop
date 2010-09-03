// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.CppBinding
{
	public class CppLanguageProperties : LanguageProperties
	{
		public CppLanguageProperties() : base(StringComparer.Ordinal) { }

		public override IAmbience GetAmbience()
		{
			return new CppAmbience();
		}
	}
}
