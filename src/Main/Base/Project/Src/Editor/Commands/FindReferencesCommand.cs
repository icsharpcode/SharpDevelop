// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Dialogs;
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
			if (entity != null) {
				var project = entity.ParentAssembly.GetProject();
				if (project != null) {
					var languageBinding = project.LanguageBinding;
					
					RenameSymbolDialog renameDialog = new RenameSymbolDialog(name => CheckName(name, languageBinding))
					{
						Owner = SD.Workbench.MainWindow,
						OldSymbolName = entity.Name,
						NewSymbolName = entity.Name
					};
					if ((bool) renameDialog.ShowDialog()) {
						using (IProgressMonitor progressMonitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}"))
							FindReferenceService.RenameSymbol(entity, renameDialog.NewSymbolName, progressMonitor)
								.ObserveOnUIThread()
								.Subscribe(error => SD.MessageService.ShowError(error.Message), ex => SD.MessageService.ShowException(ex), () => {});
					}
				}
			}
		}
		
		bool CheckName(string name, ILanguageBinding language)
		{
			if (string.IsNullOrEmpty(name))
				return false;
			
			if ((language.CodeDomProvider == null) || !language.CodeDomProvider.IsValidIdentifier(name))
				return false;
			
			return true;
		}
		
		public override bool CanExecute(ResolveResult symbol)
		{
			if (symbol == null)
				return false;
			return !symbol.GetDefinitionRegion().IsEmpty;
		}
	}
}
