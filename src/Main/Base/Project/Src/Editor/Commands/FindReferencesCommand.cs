// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Runs the find references command.
	/// </summary>
	public class FindReferencesCommand : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			var entity = GetEntity(symbol);
			if (entity != null) {
				FindReferencesAndRenameHelper.RunFindReferences(entity);
				return;
			}
			if (symbol is LocalResolveResult) {
				#warning Find References not implemented for locals
				throw new NotImplementedException();
				//FindReferencesAndRenameHelper.RunFindReferences((LocalResolveResult)symbol);
			}
		}
	}
}
