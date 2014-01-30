// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring
{
	public static class Extensions
	{
		public static IMember GetInnermostMember(this ICompilationUnit unit, int caretLine, int caretColumn)
		{
			IClass c = unit.GetInnermostClass(caretLine, caretColumn);
			
			if (c == null)
				return null;
			
			return c.AllMembers
				.SingleOrDefault(m => m.BodyRegion.IsInside(caretLine, caretColumn));
		}
		
		public static IMember GetInnermostMember(this IClass instance, int caretLine, int caretColumn)
		{
			instance = instance.GetInnermostClass(caretLine, caretColumn);
			
			if (instance == null)
				return null;
			
			return instance.AllMembers
				.SingleOrDefault(m => m.BodyRegion.IsInside(caretLine, caretColumn));
		}
		
		public static NRefactoryResolver CreateResolverForContext(LanguageProperties language, ITextEditor context)
		{
			NRefactoryResolver resolver = new NRefactoryResolver(language);
			resolver.Initialize(ParserService.GetParseInformation(context.FileName), context.Caret.Line, context.Caret.Column);
			return resolver;
		}
		
		public static bool IsUserCode(this IReturnType rt)
		{
			if (rt == null)
				return false;
			IClass c = rt.GetUnderlyingClass();
			return c != null && IsUserCode(c);
		}
		
		public static bool IsUserCode(this IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			return !c.BodyRegion.IsEmpty;
		}
		
		public static bool IsInnerClass(this IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			 return c.DeclaringType != null;
		}
	}
}
