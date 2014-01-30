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
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// A menu command that operates on a <see cref="ResolveResult"/>.
	/// 
	/// Supports the following types as <c>parameter</c>:
	/// - ResolveResult
	/// - IEntityModel
	/// 
	/// If the parameter isn't one of the types above, the command operates on the caret position in the current editor.
	/// </summary>
	public abstract class ResolveResultMenuCommand : ICommand
	{
		public virtual event EventHandler CanExecuteChanged { add {} remove {} }

		bool ICommand.CanExecute(object parameter)
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			ResolveResult resolveResult = GetResolveResult(editor, parameter);
			return CanExecute(resolveResult);
		}

		void ICommand.Execute(object parameter)
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			ResolveResult resolveResult = GetResolveResult(editor, parameter);
			Run(resolveResult);
		}
		
		public abstract void Run(ResolveResult symbol);
		
		public virtual bool CanExecute(ResolveResult symbol)
		{
			return true;
		}
		
		public static ResolveResult GetResolveResult(object owner)
		{
			return GetResolveResult(null, owner);
		}
		
		public static ISymbol GetSymbol(object owner)
		{
			return GetSymbol(GetResolveResult(null, owner));
		}
		
		static ResolveResult GetResolveResult(ITextEditor currentEditor, object owner)
		{
			if (owner is ResolveResult) {
				return (ResolveResult)owner;
			} else if (owner is IEntityModel) {
				return GetResolveResultFromEntityModel((IEntityModel)owner);
			} else if (currentEditor != null) {
				return SD.ParserService.Resolve(currentEditor, currentEditor.Caret.Location);
			} else {
				return ErrorResolveResult.UnknownError;
			}
		}
		
		static ResolveResult GetResolveResultFromEntityModel(IEntityModel entityModel)
		{
			IEntity entity = entityModel.Resolve();
			if (entity is IMember)
				return new MemberResolveResult(null, (IMember)entity);
			if (entity is ITypeDefinition)
				return new TypeResolveResult((ITypeDefinition)entity);
			return ErrorResolveResult.UnknownError;
		}
		
		protected static ISymbol GetSymbol(ResolveResult symbol)
		{
			TypeResolveResult trr = symbol as TypeResolveResult;
			if (trr != null)
				return trr.Type.GetDefinition();
			MemberResolveResult mrr = symbol as MemberResolveResult;
			if (mrr != null)
				return mrr.Member.MemberDefinition;
			LocalResolveResult lrr = symbol as LocalResolveResult;
			if (lrr != null)
				return lrr.Variable;
			return null;
		}
	}
}
