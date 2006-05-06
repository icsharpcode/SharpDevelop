// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.SharpDevelop.Dom;
using NR = ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class NRefactoryRefactoringProvider : RefactoringProvider
	{
		public static readonly NRefactoryRefactoringProvider NRefactoryProviderInstance = new NRefactoryRefactoringProvider();
		
		#region FindUnusedUsingDeclarations
		protected class PossibleTypeReference
		{
			public string Name;
			public bool Global;
			public int TypeParameterCount;
			
			public PossibleTypeReference(string name)
			{
				this.Name = name;
			}
			
			public PossibleTypeReference(TypeReference tr)
			{
				this.Name = tr.SystemType;
				this.Global = tr.IsGlobal;
				this.TypeParameterCount = tr.GenericTypes.Count;
			}
			
			public override int GetHashCode()
			{
				return Name.GetHashCode() ^ Global.GetHashCode() ^ TypeParameterCount.GetHashCode();
			}
			
			public override bool Equals(object obj)
			{
				if (!(obj is PossibleTypeReference)) return false;
				if (this == obj) return true;
				PossibleTypeReference myPossibleTypeReference = (PossibleTypeReference)obj;
				return this.Name == myPossibleTypeReference.Name && this.Global == myPossibleTypeReference.Global && this.TypeParameterCount == myPossibleTypeReference.TypeParameterCount;
			}
		}
		
		private class FindPossibleTypeReferencesVisitor : NR.AbstractAstVisitor
		{
			internal Dictionary<PossibleTypeReference, object> list = new Dictionary<PossibleTypeReference, object>();
			
			public override object Visit(IdentifierExpression identifierExpression, object data)
			{
				list[new PossibleTypeReference(identifierExpression.Identifier)] = data;
				return base.Visit(identifierExpression, data);
			}
			
			public override object Visit(TypeReference typeReference, object data)
			{
				list[new PossibleTypeReference(typeReference)] = data;
				return base.Visit(typeReference, data);
			}
			
			public override object Visit(ICSharpCode.NRefactory.Parser.AST.Attribute attribute, object data)
			{
				list[new PossibleTypeReference(attribute.Name)] = data;
				list[new PossibleTypeReference(attribute.Name + "Attribute")] = data;
				return base.Visit(attribute, data);
			}
		}
		
		protected virtual Dictionary<PossibleTypeReference, object> FindPossibleTypeReferences(string extension, string fileContent)
		{
			NR.IParser parser;
			if (extension.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
				parser = NR.ParserFactory.CreateParser(NR.SupportedLanguage.CSharp, new StringReader(fileContent));
			else if (extension.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
				parser = NR.ParserFactory.CreateParser(NR.SupportedLanguage.VBNet, new StringReader(fileContent));
			else
				return null;
			parser.Parse();
			if (parser.Errors.count > 0) {
				MessageService.ShowMessage("The operation cannot be performed because your sourcecode contains errors:\n" + parser.Errors.ErrorOutput);
				return null;
			} else {
				FindPossibleTypeReferencesVisitor visitor = new FindPossibleTypeReferencesVisitor();
				parser.CompilationUnit.AcceptVisitor(visitor, null);
				return visitor.list;
			}
		}
		
		public override bool SupportsFindUnusedUsingDeclarations {
			get {
				return true;
			}
		}
		
		public override IList<IUsing> FindUnusedUsingDeclarations(string fileName, string fileContent, ICompilationUnit cu)
		{
			IClass @class = cu.Classes.Count == 0 ? null : cu.Classes[0];
			
			Dictionary<PossibleTypeReference, object> references = FindPossibleTypeReferences(Path.GetExtension(fileName), fileContent);
			if (references == null) return new IUsing[0];
			
			Dictionary<IUsing, object> dict = new Dictionary<IUsing, object>();
			foreach (PossibleTypeReference tr in references.Keys) {
				SearchTypeRequest request = new SearchTypeRequest(tr.Name, tr.TypeParameterCount, @class, cu, 1, 1);
				SearchTypeResult response = cu.ProjectContent.SearchType(request);
				if (response.UsedUsing != null) {
					dict[response.UsedUsing] = null;
				}
			}
			
			List<IUsing> list = new List<IUsing>();
			foreach (IUsing import in cu.Usings) {
				if (!dict.ContainsKey(import)) {
					if (import.HasAliases) {
						foreach (string key in import.Aliases.Keys) {
							if (references.ContainsKey(new PossibleTypeReference(key)))
								goto checkNextImport;
						}
					}
					list.Add(import); // this using is unused
				}
				checkNextImport:;
			}
			return list;
		}
		#endregion
	}
}
