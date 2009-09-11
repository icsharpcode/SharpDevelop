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
