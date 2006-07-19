// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Parser.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Dom;
using NR = ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class NRefactoryRefactoringProvider : RefactoringProvider
	{
		public static readonly NRefactoryRefactoringProvider NRefactoryCSharpProviderInstance = new NRefactoryRefactoringProvider(NR.SupportedLanguage.CSharp);
		public static readonly NRefactoryRefactoringProvider NRefactoryVBNetProviderInstance = new NRefactoryRefactoringProvider(NR.SupportedLanguage.VBNet);
		
		NR.SupportedLanguage language;
		
		private NRefactoryRefactoringProvider(NR.SupportedLanguage language)
		{
			this.language = language;
		}
		
		public override bool IsEnabledForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
				return language == NR.SupportedLanguage.CSharp;
			else if (extension.Equals(".vb", StringComparison.InvariantCultureIgnoreCase))
				return language == NR.SupportedLanguage.VBNet;
			else
				return false;
		}
		
		static void ShowSourceCodeErrors(string errors)
		{
			MessageService.ShowMessage("The operation cannot be performed because your source code contains errors:\n" + errors);
		}
		
		NR.IParser ParseFile(string fileContent)
		{
			NR.IParser parser = NR.ParserFactory.CreateParser(language, new StringReader(fileContent));
			parser.Parse();
			if (parser.Errors.count > 0) {
				ShowSourceCodeErrors(parser.Errors.ErrorOutput);
				parser.Dispose();
				return null;
			} else {
				return parser;
			}
		}
		
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
			
			public override object Visit(ICSharpCode.NRefactory.Parser.Ast.Attribute attribute, object data)
			{
				list[new PossibleTypeReference(attribute.Name)] = data;
				list[new PossibleTypeReference(attribute.Name + "Attribute")] = data;
				return base.Visit(attribute, data);
			}
		}
		
		protected virtual Dictionary<PossibleTypeReference, object> FindPossibleTypeReferences(string extension, string fileContent)
		{
			NR.IParser parser = ParseFile(fileContent);
			if (parser == null) {
				return null;
			} else {
				FindPossibleTypeReferencesVisitor visitor = new FindPossibleTypeReferencesVisitor();
				parser.CompilationUnit.AcceptVisitor(visitor, null);
				parser.Dispose();
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
		
		#region CreateNewFileLikeExisting
		public override bool SupportsCreateNewFileLikeExisting {
			get {
				return true;
			}
		}
		
		public override string CreateNewFileLikeExisting(string existingFileContent, string codeForNewType)
		{
			NR.IParser parser = ParseFile(existingFileContent);
			if (parser == null) {
				return null;
			}
			RemoveTypesVisitor visitor = new RemoveTypesVisitor();
			parser.CompilationUnit.AcceptVisitor(visitor, null);
			List<NR.ISpecial> comments = new List<NR.ISpecial>();
			foreach (NR.ISpecial c in parser.Lexer.SpecialTracker.CurrentSpecials) {
				if (c.StartPosition.Y <= visitor.includeCommentsUpToLine
				    || c.StartPosition.Y > visitor.includeCommentsAfterLine)
				{
					comments.Add(c);
				}
			}
			IOutputAstVisitor outputVisitor = (language==NR.SupportedLanguage.CSharp) ? new CSharpOutputVisitor() : (IOutputAstVisitor)new VBNetOutputVisitor();
			using (SpecialNodesInserter.Install(comments, outputVisitor)) {
				parser.CompilationUnit.AcceptVisitor(outputVisitor, null);
			}
			string expectedText;
			if (language==NR.SupportedLanguage.CSharp)
				expectedText = "using " + RemoveTypesVisitor.DummyIdentifier + ";";
			else
				expectedText = "Imports " + RemoveTypesVisitor.DummyIdentifier;
			using (StringWriter w = new StringWriter()) {
				using (StringReader r1 = new StringReader(outputVisitor.Text)) {
					string line;
					while ((line = r1.ReadLine()) != null) {
						string trimLine = line.TrimStart();
						if (trimLine == expectedText) {
							string indentation = line.Substring(0, line.Length - trimLine.Length);
							using (StringReader r2 = new StringReader(codeForNewType)) {
								while ((line = r2.ReadLine()) != null) {
									w.Write(indentation);
									w.WriteLine(line);
								}
							}
						} else {
							w.WriteLine(line);
						}
					}
				}
				if (visitor.firstType) {
					w.WriteLine(codeForNewType);
				}
				return w.ToString();
			}
		}
		
		private class RemoveTypesVisitor : NR.AbstractAstTransformer
		{
			internal const string DummyIdentifier = "DummyNamespace!InsertionPos";
			
			internal int includeCommentsUpToLine;
			internal int includeCommentsAfterLine = int.MaxValue;
			
			internal bool firstType = true;
			
			public override object Visit(UsingDeclaration usingDeclaration, object data)
			{
				if (firstType) {
					includeCommentsUpToLine = usingDeclaration.EndLocation.Y;
				}
				return null;
			}
			
			public override object Visit(NamespaceDeclaration namespaceDeclaration, object data)
			{
				includeCommentsAfterLine = namespaceDeclaration.EndLocation.Y;
				if (firstType) {
					includeCommentsUpToLine = namespaceDeclaration.StartLocation.Y;
					return base.Visit(namespaceDeclaration, data);
				} else {
					RemoveCurrentNode();
					return null;
				}
			}
			
			public override object Visit(TypeDeclaration typeDeclaration, object data)
			{
				if (typeDeclaration.EndLocation.Y > includeCommentsAfterLine)
					includeCommentsAfterLine = typeDeclaration.EndLocation.Y;
				if (firstType) {
					firstType = false;
					ReplaceCurrentNode(new UsingDeclaration(DummyIdentifier));
				} else {
					RemoveCurrentNode();
				}
				return null;
			}
		}
		#endregion
		
		#region ExtractCodeForType
		public override bool SupportsGetFullCodeRangeForType {
			get {
				return true;
			}
		}
		
		public override DomRegion GetFullCodeRangeForType(string fileContent, IClass type)
		{
			NR.ILexer lexer = NR.ParserFactory.CreateLexer(language, new StringReader(fileContent));
			// use the lexer to determine last token position before type start
			// and next token position after type end
			Stack<NR.Location> stack = new Stack<NR.Location>();
			NR.Location lastPos = NR.Location.Empty;
			NR.Token t = lexer.NextToken();
			bool csharp = language == NR.SupportedLanguage.CSharp;
			int eof = csharp ? NR.CSharp.Tokens.EOF : NR.VB.Tokens.EOF;
			int attribStart = csharp ? NR.CSharp.Tokens.OpenSquareBracket : NR.VB.Tokens.LessThan;
			int attribEnd = csharp ? NR.CSharp.Tokens.CloseSquareBracket : NR.VB.Tokens.GreaterThan;
			
			while (t.kind != eof) {
				if (t.kind == attribStart)
					stack.Push(lastPos);
				if (t.EndLocation.Y >= type.Region.BeginLine)
					break;
				lastPos = t.EndLocation;
				if (t.kind == attribEnd && stack.Count > 0)
					lastPos = stack.Pop();
				t = lexer.NextToken();
			}
			
			stack = null;
			
			// Skip until end of type
			while (t.kind != eof) {
				if (t.EndLocation.Y > type.BodyRegion.EndLine)
					break;
				t = lexer.NextToken();
			}
			
			int lastLineBefore = lastPos.IsEmpty ? 0 : lastPos.Y;
			int firstLineAfter = t.EndLocation.IsEmpty ? int.MaxValue : t.EndLocation.Y;
			
			lexer.Dispose(); lexer = null;
			
			StringReader myReader = new StringReader(fileContent);
			
			string line;
			string mainLine;
			int resultBeginLine = lastLineBefore + 1;
			int resultEndLine = firstLineAfter - 1;
			int lineNumber = 0;
			int largestEmptyLineCount = 0;
			int emptyLinesInRow = 0;
			while ((line = myReader.ReadLine()) != null) {
				lineNumber++;
				if (lineNumber <= lastLineBefore)
					continue;
				if (lineNumber < type.Region.BeginLine) {
					string trimLine = line.TrimStart();
					if (trimLine.Length == 0) {
						if (++emptyLinesInRow > largestEmptyLineCount) {
							resultBeginLine = lineNumber;
						}
					}
				} else if (lineNumber == type.Region.BeginLine) {
					mainLine = line;
				} else if (lineNumber == type.BodyRegion.EndLine) {
					largestEmptyLineCount = 0;
					emptyLinesInRow = 0;
					resultEndLine = lineNumber;
				} else if (lineNumber > type.BodyRegion.EndLine) {
					if (lineNumber >= firstLineAfter)
						break;
					string trimLine = line.TrimStart();
					if (trimLine.Length == 0) {
						if (++emptyLinesInRow > largestEmptyLineCount) {
							emptyLinesInRow = largestEmptyLineCount;
							resultEndLine = lineNumber - emptyLinesInRow;
						}
					}
				}
			}
			
			myReader.Dispose();
			return new DomRegion(resultBeginLine, 0, resultEndLine, int.MaxValue);
		}
		#endregion
	}
}
