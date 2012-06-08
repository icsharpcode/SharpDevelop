// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Semantics;

namespace Debugger.AddIn
{
	public enum SupportedLanguage
	{
		CSharp,
		VBNet
	}
	
	public class ExpressionEvaluationVisitor
	{
		public ExpressionEvaluationVisitor()
		{
		}
		
		public Value Convert(ResolveResult result)
		{
			return Visit((dynamic)result);
		}
		
		Value Visit(ResolveResult result)
		{
			return null;
		}
		
		Value Visit(ThisResolveResult result)
		{
			return null;
		}
		
		Value Visit(MemberResolveResult result)
		{
			return null;
		}
	}
}
