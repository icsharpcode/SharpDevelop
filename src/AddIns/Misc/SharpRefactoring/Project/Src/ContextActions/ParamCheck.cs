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
