// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// A menu command that operates on a <see cref="ResolveResult"/>.
	/// 
	/// Supports the following types as <see cref="Owner"/>:
	/// - IUnresolvedTypeDefinition (as used by EntityBookmark)
	/// - IUnresolvedMember (as used by EntityBookmark)
	/// 
	/// If the owner isn't one of the types above, the command operates on the caret position in the current editor.
	/// </summary>
	public abstract class ResolveResultMenuCommand : AbstractMenuCommand
	{
		public abstract void Run(ResolveResult symbol);
		
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			ResolveResult resolveResult = GetResolveResult(editor, Owner);
			Run(resolveResult);
		}
		
		public static ResolveResult GetResolveResult(ITextEditor editor, object owner)
		{
			if (owner is IUnresolvedTypeDefinition || owner is IUnresolvedMember) {
				return GetResolveResultFromUnresolvedEntity((IUnresolvedEntity)owner);
			} else if (editor != null) {
				return SD.ParserService.Resolve(editor, editor.Caret.Location);
			} else {
				return ErrorResolveResult.UnknownError;
			}
		}
		
		static ResolveResult GetResolveResultFromUnresolvedEntity(IUnresolvedEntity entity)
		{
			if (entity.UnresolvedFile == null)
				return ErrorResolveResult.UnknownError;
			ICompilation compilation = SD.ParserService.GetCompilationForFile(FileName.Create(entity.UnresolvedFile.FileName));
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			if (entity is IUnresolvedMember) {
				var member = ((IUnresolvedMember)entity).Resolve(context);
				if (member != null) {
					return new MemberResolveResult(null, member);
				} else {
					return ErrorResolveResult.UnknownError;
				}
			} else { // IUnresolvedTypeDefinition
				var type = ((IUnresolvedTypeDefinition)entity).Resolve(context);
				return new TypeResolveResult(type);
			}
		}
		
		protected IEntity GetEntity(ResolveResult symbol)
		{
			TypeResolveResult trr = symbol as TypeResolveResult;
			if (trr != null)
				return trr.Type.GetDefinition();
			MemberResolveResult mrr = symbol as MemberResolveResult;
			if (mrr != null)
				return mrr.Member.MemberDefinition;
			return null;
		}
	}
}
