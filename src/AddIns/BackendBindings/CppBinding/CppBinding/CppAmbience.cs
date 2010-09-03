// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.CppBinding.Parser;

namespace ICSharpCode.CppBinding
{
	public class CppAmbience : CSharpAmbience
	{
		public override string Convert(IClass c)
		{
//			if (c is GlobalMemberContainer)
//				return "<global members>";
			return base.Convert(c);
		}
	}
}
