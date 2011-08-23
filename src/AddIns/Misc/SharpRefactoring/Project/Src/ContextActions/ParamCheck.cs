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
			var paramAtCaret = GetParameterAtCaret(context);
			if (paramAtCaret == null)
				return false;
			
			return IsAvailable(paramAtCaret.ResolvedType);
		}
		
		public LocalResolveResult GetParameterAtCaret(EditorContext context)
		{
			LocalResolveResult paramAtCaret = context.CurrentSymbol as LocalResolveResult;
			if (paramAtCaret == null || paramAtCaret.CallingClass == null || paramAtCaret.ResolvedType == null)
				return null;
			// only for C#
			if (paramAtCaret.CallingClass.ProjectContent.Language != LanguageProperties.CSharp)
				return null;
			if (!paramAtCaret.IsParameter)
				return null;
			// must be definition
			if (!paramAtCaret.VariableDefinitionRegion.IsInside(context.CurrentExpression.Region.BeginLine, context.CurrentExpression.Region.BeginColumn))
				return null;
			// must be not abstract/interface method
			if (paramAtCaret.CallingMember == null || paramAtCaret.CallingMember.IsAbstract || 
			    paramAtCaret.CallingMember.DeclaringType.ClassType == ClassType.Interface)
				return null;
			return paramAtCaret;
		}
		
		public override void Execute(EditorContext context)
		{
			var paramAtCaret = GetParameterAtCaret(context);
			if (paramAtCaret != null)
			{
				Extensions.AddCodeToMethodStart(paramAtCaret.CallingMember, context.Editor, GetCodeToInsert(paramAtCaret.VariableName));
			}
		}
		
		public abstract bool IsAvailable(IReturnType parameterType);
		
		public abstract string GetCodeToInsert(string parameterName);
	}
}
