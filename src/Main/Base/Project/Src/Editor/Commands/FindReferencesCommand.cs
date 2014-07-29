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
			RunRename(GetSymbol(symbol));
		}
		
		public static void RunRename(ISymbol symbol, string newName = null)
		{
			if ((symbol is IMember) && ((symbol.SymbolKind == SymbolKind.Constructor) || (symbol.SymbolKind == SymbolKind.Destructor))) {
				// Don't rename constructors/destructors, rename their declaring type instead
				symbol = ((IMember) symbol).DeclaringType.GetDefinition();
			}
			if (symbol != null) {
				var project = GetProjectFromSymbol(symbol);
				if (project != null) {
					var languageBinding = project.LanguageBinding;
					if (newName == null) {
						RenameSymbolDialog renameDialog = new RenameSymbolDialog(name => CheckName(name, languageBinding))
						{
							Owner = SD.Workbench.MainWindow,
							OldSymbolName = symbol.Name,
							NewSymbolName = symbol.Name
						};
						if (renameDialog.ShowDialog() == true) {
							newName = renameDialog.NewSymbolName;
						} else {
							return;
						}
					}
					AsynchronousWaitDialog.ShowWaitDialogForAsyncOperation(
						"${res:SharpDevelop.Refactoring.Rename}",
						progressMonitor =>
						FindReferenceService.RenameSymbol(symbol, newName, progressMonitor)
						.ObserveOnUIThread()
						.Subscribe(error => SD.MessageService.ShowError(error.Message), ex => SD.MessageService.ShowException(ex), () => {}));
				}
			}
		}
		
		static ICSharpCode.SharpDevelop.Project.IProject GetProjectFromSymbol(ISymbol symbol)
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
		
		static bool CheckName(string name, ILanguageBinding language)
		{
			if (string.IsNullOrEmpty(name))
				return false;

			if ((language.CodeDomProvider == null) || (!language.CodeDomProvider.IsValidIdentifier(name) &&
			                                           !language.CodeDomProvider.IsValidIdentifier(language.CodeGenerator.EscapeIdentifier(name))))
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
