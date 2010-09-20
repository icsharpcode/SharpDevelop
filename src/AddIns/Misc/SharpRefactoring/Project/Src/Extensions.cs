// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
