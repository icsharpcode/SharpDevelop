// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			if (symbol == null)
				return;
			
			FilePosition pos = symbol.GetDefinitionPosition();
			if (pos.IsEmpty) {
				IEntity entity;
				if (symbol is MemberResolveResult) {
					entity = ((MemberResolveResult)symbol).ResolvedMember;
				} else if (symbol is TypeResolveResult) {
					entity = ((TypeResolveResult)symbol).ResolvedClass;
				} else {
					entity = null;
				}
				if (entity != null) {
					NavigationService.NavigateTo(entity);
				}
			} else {
				try {
					if (pos.Position.IsEmpty)
						FileService.OpenFile(pos.FileName);
					else
						FileService.JumpToFilePosition(pos.FileName, pos.Line, pos.Column);
				} catch (Exception ex) {
					MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
				}
			}
		}
	}
}
