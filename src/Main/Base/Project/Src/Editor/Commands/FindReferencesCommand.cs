// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
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
			var entity = GetSymbol(symbol) as IEntity;
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
			var entity = GetSymbol(symbol);
			if (entity != null) {
				var project = GetProjectFromSymbol(entity);
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
		
		ICSharpCode.SharpDevelop.Project.IProject GetProjectFromSymbol(ISymbol symbol)
		{
			switch (symbol.SymbolKind) {
				case SymbolKind.None:
					return null;
				case SymbolKind.TypeDefinition:
				case SymbolKind.Field:
				case SymbolKind.Property:
				case SymbolKind.Indexer:
				case SymbolKind.Event:
				case SymbolKind.Method:
				case SymbolKind.Operator:
				case SymbolKind.Constructor:
				case SymbolKind.Destructor:
				case SymbolKind.Accessor:
					return ((IEntity)symbol).ParentAssembly.GetProject();
				case SymbolKind.Namespace:
					return null; // TODO : extend rename on namespaces
				case SymbolKind.Variable:
					return SD.ProjectService.FindProjectContainingFile(new FileName(((IVariable)symbol).Region.FileName));
				case SymbolKind.Parameter:
					if (((IParameter) symbol).Owner != null) {
						return ((IParameter)symbol).Owner.ParentAssembly.GetProject();
					} else {
						return SD.ProjectService.FindProjectContainingFile(new FileName(((IParameter)symbol).Region.FileName));
					}
				case SymbolKind.TypeParameter:
					return null; // TODO : extend rename on type parameters
				default:
					throw new ArgumentOutOfRangeException();
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
