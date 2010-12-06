// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ParamCheck.
	/// </summary>
	public abstract class ParamCheck : ContextAction
	{
		public override bool IsAvailable(EditorContext context)
		{
			var paramAtCaret = GetParameterAtCaret(context.CurrentSymbol);
			if (paramAtCaret == null)
				return false;
			
			return IsAvailable(paramAtCaret.ResolvedType);
		}
		
		public LocalResolveResult GetParameterAtCaret(ResolveResult symbol)
		{
			LocalResolveResult param = symbol as LocalResolveResult;
			if (param == null || param.CallingClass == null || param.ResolvedType == null)
				return null;
			if (param.CallingClass.ProjectContent.Language != LanguageProperties.CSharp)
				return null;
			if (!param.IsParameter)
				return null;
			// FIXME must be parameter definition, and method with body (not interface or abstract)
			return param;
		}
			
		public override void Execute(EditorContext context)
		{
			var paramAtCaret = GetParameterAtCaret(context.CurrentSymbol);
			Extensions.AddCodeToMethodStart(paramAtCaret.CallingMember, context.Editor, GetCodeToInsert(paramAtCaret.VariableName));
		}
		
		public abstract bool IsAvailable(IReturnType parameterType);
		
		public abstract string GetCodeToInsert(string parameterName);
	}
}
