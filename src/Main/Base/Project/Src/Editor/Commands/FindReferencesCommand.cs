// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Runs the find references command.
	/// </summary>
	public class FindReferencesCommand : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			var entity = GetEntity(symbol);
			if (entity != null) {
				FindReferencesAndRenameHelper.RunFindReferences(entity);
				return;
			}
			if (symbol is LocalResolveResult) {
				FindReferencesAndRenameHelper.RunFindReferences((LocalResolveResult)symbol);
			}
		}
	}
	
	/// <summary>
	/// Runs the rename refactoring.
	/// </summary>
	public class RenameSymbolCommand : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			var entity = GetEntity(symbol);
			
			string title = "${res:SharpDevelop.Refactoring.Rename}";
			string text = title; // TODO add proper text
			if (entity != null) {
				switch (entity.SymbolKind) {
					case ICSharpCode.NRefactory.TypeSystem.SymbolKind.TypeDefinition:
						text = "${res:SharpDevelop.Refactoring.RenameClassText}";
						break;
					default:
						text = "${res:SharpDevelop.Refactoring.RenameMemberText}";
						break;
				}
				
				string newName = SD.MessageService.ShowInputBox(title, text, entity.Name);
				if (newName != entity.Name) {
					if (!CheckName(newName)) {
						SD.MessageService.ShowError("The symbol cannot be renamed because its new name is invalid!");
						return;
					}
					using (IProgressMonitor progressMonitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}"))
						FindReferenceService.RenameSymbol(entity, newName, progressMonitor)
							.ObserveOnUIThread()
							.Subscribe(error => SD.MessageService.ShowError(error.Message), ex => SD.MessageService.ShowException(ex), () => {});
				}
			}
			
		}
		
		bool CheckName(string name)
		{
			// TODO implement for current language!
			return !string.IsNullOrWhiteSpace(name);
		}
	}
}
