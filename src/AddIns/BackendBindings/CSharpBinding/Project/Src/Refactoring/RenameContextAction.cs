// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	[ContextAction("Rename", Description = "Renames a symbol and all references to it.")]
	public class RenameContextAction : ContextAction
	{
		public override async Task<bool> IsAvailableAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
		{
			var rr = await context.GetCurrentSymbolAsync();
			if (rr == null || rr.GetDefinitionRegion().IsEmpty) return false;
			if (rr is MemberResolveResult) {
				// check whether all members in the hierarchy are defined in code...
				IEntity thisMember = GetSurrogateEntity(((MemberResolveResult)rr).Member);
				switch (thisMember.EntityType) {
					case EntityType.TypeDefinition:
					case EntityType.Field:
						return !thisMember.Region.IsEmpty;
						break;
					case EntityType.Property:
					case EntityType.Event:
					case EntityType.Method:
						return !FindReferenceService.FindAllMembers((IMember)thisMember)
							.Distinct()
							.Any(m => m.Region.IsEmpty);
					case EntityType.Indexer:
						return false;
					case EntityType.Operator:
						return false;
					default:
						throw new NotSupportedException();
				}
			}
			
			return true;
		}
		
		IEntity GetSurrogateEntity(IMember member)
		{
			if (member.EntityType == EntityType.Accessor)
				return ((IMethod)member).AccessorOwner;
			if (member.EntityType == EntityType.Constructor || member.EntityType == EntityType.Destructor)
				return member.DeclaringTypeDefinition;
			return member;
		}
		
		public override async void Execute(EditorRefactoringContext context)
		{
			var rr = await context.GetCurrentSymbolAsync();
			List<SearchedFile> references = null;
			string oldName = null;
			string text = null;
			bool isLocal = rr is LocalResolveResult;
			IVariable variable = null;
			IEntity entity = null;
			IProgressMonitor monitor = new DummyProgressMonitor();
			
			if (isLocal) {
				variable = ((LocalResolveResult)rr).Variable;
				oldName = variable.Name;
				text = "${res:SharpDevelop.Refactoring.RenameMemberText}";
				references = await FindReferenceService.FindLocalReferences(variable, monitor).ToListAsync();
			} else if (rr is TypeResolveResult) {
				ITypeDefinition td = rr.Type.GetDefinition();
				if (td == null) return;
				entity = td;
				oldName = td.Name;
				text = "${res:SharpDevelop.Refactoring.RenameClassText}";
				references = await FindReferenceService.FindReferences(td, monitor).ToListAsync();
			} else if (rr is MemberResolveResult) {
				entity = GetSurrogateEntity(((MemberResolveResult)rr).Member);
				oldName = entity.Name;
				if (entity is IMember) {
					text = "${res:SharpDevelop.Refactoring.RenameMemberText}";
					references = await FindReferenceService.FindReferencesToHierarchy((IMember)entity, monitor).ToListAsync();
				} else {
					text = "${res:SharpDevelop.Refactoring.RenameClassText}";
					references = await FindReferenceService.FindReferences(entity, monitor).ToListAsync();
				}
			}
			
			if (references == null || oldName == null) return;
			string newName = SD.MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", text, oldName);
			if (newName == null) return;
			// if both identifiers are the same (in the language the symbol was defined in)
			// => abort rename
			ILanguageBinding definitionLanguage = SD.LanguageService
				.GetLanguageByFileName(new FileName(rr.GetDefinitionRegion().FileName));
			if (definitionLanguage.IdentifierComparer.Compare(oldName, newName) == 0)
				return;
			
			// check if identifier is valid in all target languages:
			string lastExtension = null;
			ILanguageBinding language = null;
			bool isRenamePossible = true;
			
			Dictionary<Reference, Conflict[]> conflicts = new Dictionary<Reference, Conflict[]>();
			
			foreach (var file in references) {
				string ext = file.FileName.GetExtension();
				if (language == null || !string.Equals(ext, lastExtension, StringComparison.OrdinalIgnoreCase)) {
					lastExtension = ext;
					language = SD.LanguageService.GetLanguageByExtension(ext);
				}
				
				if (!language.CodeGenerator.IsValidIdentifier(newName)) {
					isRenamePossible = false;
					SD.MessageService.ShowErrorFormatted("The symbol '{0}' cannot be renamed, because its new name '{1}' would be invalid in {2}, please choose a different name and try again.", oldName, newName, language.Name);
					break;
				}
				
				foreach (Reference reference in file.Matches.OfType<Reference>()) {
					var currentConflicts = language.CodeGenerator.FindRenamingConflicts(reference, newName).ToArray();
					if (currentConflicts.Any(conflict => !conflict.IsSolvableConflict)) {
						isRenamePossible = false;
						SD.MessageService.ShowErrorFormatted("The symbol '{0}' cannot be renamed, because its new name '{1}' would cause an unsolvable conflict in {2}, at line {3}, column {4}, please choose a different name and try again.", oldName, newName, reference.FileName, reference.StartLocation.Line, reference.StartLocation.Column);
						break;
					}
					conflicts.Add(reference, currentConflicts);
				}
			}
			
			// TODO : ask if user wants to rename corresponding IField/IProperty/IEvent as well...
			
			if (!isRenamePossible) return;
			
			lastExtension = null;
			language = null;
			
			foreach (var reference in references.SelectMany(file => file.Matches).OfType<Reference>()) {
				string ext = reference.FileName.GetExtension();
				if (language == null || !string.Equals(ext, lastExtension, StringComparison.OrdinalIgnoreCase)) {
					lastExtension = ext;
					language = SD.LanguageService.GetLanguageByExtension(ext);
				}
				
				IList<Conflict> currentConflicts = conflicts.GetOrDefault(reference);
				if (currentConflicts == null)
					currentConflicts = EmptyList<Conflict>.Instance;
				
				var renamingContext = new RenameReferenceContext(reference, currentConflicts);
				
				if (isLocal)
					renamingContext.OldVariable = variable;
				else
					renamingContext.OldEntity = entity;
				
				if (renamingContext != null)
					language.CodeGenerator.RenameSymbol(renamingContext, newName);
			}
		}
		
		public override string DisplayName {
			get { return "Rename symbol"; }
		}
	}
}
