// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
		public LocalResolveResult GetParameterAtCaret(ResolveResult symbol)
		{
			LocalResolveResult param = symbol as LocalResolveResult;
			if (param == null || param.CallingClass == null || param.ResolvedType == null)
				return null;
			if (param.CallingClass.ProjectContent.Language != LanguageProperties.CSharp)
				return null;
			if (!param.IsParameter)
				return null;
			return param;
		}
		
		public override bool IsAvailable(EditorContext context)
		{
			var paramAtCaret = GetParameterAtCaret(context.CurrentSymbol);
			if (paramAtCaret == null)
				return false;
			
			return IsAvailable(paramAtCaret.ResolvedType);
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
