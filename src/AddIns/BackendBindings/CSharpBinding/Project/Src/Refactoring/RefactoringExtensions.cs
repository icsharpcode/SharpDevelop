// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Some extensions related to refactoring features.
	/// </summary>
	public static class RefactoringExtensions
	{
		/// <summary>
		/// Checks whether a property is auto-implemented (has only "get; set;" definitions).
		/// </summary>
		/// <param name="property">Property to check</param>
		/// <returns>True if auto-implemented, false otherwise.</returns>
		public static bool IsAutoImplemented(this IProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			if (property.IsAbstract)
				return false;
			
			CSharpFullParseInformation parseInformation;
			PropertyDeclaration propDeclaration = property.GetDeclaration(out parseInformation) as PropertyDeclaration;
			if ((propDeclaration != null) && (propDeclaration.Getter != null) && (propDeclaration.Setter != null)) {
				bool containsGetterBlock = propDeclaration.Getter.Children.Any(node => node is BlockStatement);
				bool containsSetterBlock = propDeclaration.Setter.Children.Any(node => node is BlockStatement);
				
				// This property is only auto-generated, if it contains get; and set; without block statements!
				return !containsGetterBlock && !containsSetterBlock;
			}
			
			return false;
		}
		
		/// <summary>
		/// Retrieves the declaration for the specified entity.
		/// Returns null if the entity is not defined in C# source code.
		/// </summary>
		public static EntityDeclaration GetDeclaration(this IEntity entity, out CSharpFullParseInformation parseInfo)
		{
			if (entity == null || string.IsNullOrEmpty(entity.Region.FileName)) {
				parseInfo = null;
				return null;
			}
			parseInfo = SD.ParserService.Parse(FileName.Create(entity.Region.FileName),
			                                   parentProject: entity.ParentAssembly.GetProject())
				as CSharpFullParseInformation;
			if (parseInfo == null)
				return null;
			return parseInfo.SyntaxTree.GetNodeAt<EntityDeclaration>(entity.Region.Begin);
		}
		
		
		/// <summary>
		/// Returns a refactoring context for the file that contains the entity.
		/// This will open the file in the text editor if necessary.
		/// </summary>
		public static SDRefactoringContext CreateRefactoringContext(this IEntity entity)
		{
			var typeDef = entity as ITypeDefinition;
			DomRegion region;
			if (typeDef != null) {
				IUnresolvedTypeDefinition bestPart = null;
				foreach (var part in typeDef.Parts) {
					if (bestPart == null || EntityModelContextUtils.IsBetterPart(part, bestPart, ".cs"))
						bestPart = part;
				}
				region = bestPart.Region;
			} else {
				region = entity.Region;
			}
			var view = SD.FileService.OpenFile(new FileName(region.FileName), false);
			if (view == null)
				return null;
			var editor = view.GetService<ITextEditor>();
			if (editor == null)
				return null;
			var project = entity.ParentAssembly.GetProject();
			var fileName = FileName.Create(region.FileName);
			var parseInfo = SD.ParserService.Parse(fileName, editor.Document, project) as CSharpFullParseInformation;
			if (parseInfo == null)
				return null;
			ICompilation compilation;
			if (project != null)
				compilation = SD.ParserService.GetCompilation(project);
			else
				compilation = SD.ParserService.GetCompilationForFile(fileName);
			var resolver = parseInfo.GetResolver(compilation);
			return new SDRefactoringContext(editor, resolver, region.Begin);
		}
	}
}
