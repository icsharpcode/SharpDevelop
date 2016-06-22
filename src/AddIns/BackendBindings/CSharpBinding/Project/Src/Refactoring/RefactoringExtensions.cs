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
using System.Linq;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

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
		/// Checks whether a type is nullable.
		/// </summary>
		/// <param name="type">Checked type</param>
		/// <returns>True if nullable, false otherwise.</returns>
		public static bool IsNullable(this IType type)
		{
			// true = reference, null = generic or unknown
			return type.IsReferenceType != false
				|| (type.FullName == "System.Nullable");
		}
		
		/// <summary>
		/// Checks whether a type has a range.
		/// </summary>
		/// <param name="type">Checked type</param>
		/// <returns>True if type has range, false otherwise.</returns>
		public static bool HasRange(this IType type)
		{
			return IsTypeWithRange(type) ||
				(type.FullName == "System.Nullable")
				&& IsTypeWithRange(type.TypeArguments.First());
		}
		
		static bool IsTypeWithRange(IType type)
		{
			string crtType = type.FullName;
			return crtType == "System.Int32" ||
				crtType == "System.Int16" ||
				crtType == "System.Int64" ||
				crtType == "System.Single" ||
				crtType == "System.Double" ||
				crtType == "System.UInt16" ||
				crtType == "System.UInt32" ||
				crtType == "System.UInt64";
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
			return CreateRefactoringContext(region, entity.ParentAssembly.GetProject());
		}
		
		public static SDRefactoringContext CreateRefactoringContext(DomRegion region, IProject project = null)
		{
			var view = SD.FileService.OpenFile(new FileName(region.FileName), false);
			if (view == null)
				throw new NotSupportedException("Could not open " + region.FileName);
			var editor = view.GetService<ITextEditor>();
			if (editor == null)
				throw new NotSupportedException("Could not find editor for " + region.FileName);
			var fileName = FileName.Create(region.FileName);
			var parseInfo = SD.ParserService.Parse(fileName, editor.Document, project) as CSharpFullParseInformation;
			if (parseInfo == null)
				throw new NotSupportedException("Could not C# parse info for " + region.FileName);
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
