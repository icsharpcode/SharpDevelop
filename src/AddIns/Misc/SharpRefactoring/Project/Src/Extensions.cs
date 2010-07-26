// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

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
	}
}
