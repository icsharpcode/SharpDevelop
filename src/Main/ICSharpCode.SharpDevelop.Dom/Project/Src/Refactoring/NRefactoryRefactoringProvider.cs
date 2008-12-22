// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using NR = ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
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
		
		static void ShowSourceCodeErrors(IDomProgressMonitor progressMonitor, string errors)
		{
			if (progressMonitor != null)
				progressMonitor.ShowingDialog = true;
			HostCallback.ShowMessage("${res:SharpDevelop.Refactoring.CannotPerformOperationBecauseOfSyntaxErrors}\n" + errors);
			if (progressMonitor != null)
				progressMonitor.ShowingDialog = false;
		}
		
		NR.IParser ParseFile(IDomProgressMonitor progressMonitor, string fileContent)
		{
			NR.IParser parser = NR.ParserFactory.CreateParser(language, new StringReader(fileContent));
			parser.Parse();
			if (parser.Errors.Count > 0) {
				ShowSourceCodeErrors(progressMonitor, parser.Errors.ErrorOutput);
				parser.Dispose();
				return null;
			} else {
				return parser;
			}
		}
		
		IOutputAstVisitor GetOutputVisitor() {
			switch (language) {
				case NR.SupportedLanguage.CSharp:
					return new CSharpOutputVisitor();
				case NR.SupportedLanguage.VBNet:
					return new VBNetOutputVisitor();
				default:
					throw new NotSupportedException();
			}
		}
		
		string CommentToken {
			get {
				switch (language) {
					case NR.SupportedLanguage.CSharp:
						return "//";
					case NR.SupportedLanguage.VBNet:
						return "'";
					default:
						throw new NotSupportedException();
				}
			}
		}

		#region ExtractInterface
		public override bool SupportsExtractInterface {
			get {
				return true;
			}
		}
		
		private class ExtractInterfaceVisitor : NR.Visitors.AbstractAstVisitor
		{
			string newInterfaceName;
			string sourceClassName;
			string sourceNamespace;
			Dictionary<string, IMember> membersToInclude;
			
			public ExtractInterfaceVisitor(string newInterfaceName,
			                               string sourceNamespace,
			                               string sourceClassName,
			                               IList<IMember> chosenMembers) {
				this.newInterfaceName = newInterfaceName;
				this.sourceNamespace = sourceNamespace;
				this.sourceClassName = sourceClassName;
				
				// store the chosen members in a dictionary for easy lookup
				membersToInclude = new Dictionary<string, IMember>();
				foreach(IMember m in chosenMembers) {
					membersToInclude.Add(m.Name, m);
				}
			}

			public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
			{
				// strip out any usings & extract our TypeReference from the NameSpace
				// we walk backwards so that deletions don't affect the iteration
				NamespaceDeclaration ns;
				TypeDeclaration td;
				object child;
				object nsChild;
				for(int i = compilationUnit.Children.Count-1; i>=0; i--) {
					child = compilationUnit.Children[i];
					if (child is UsingDeclaration) {
						// we don't want our usings here...
						compilationUnit.Children.RemoveAt(i);
					}
					else if (child is NamespaceDeclaration) {
						ns = (NamespaceDeclaration)child;
						if (ns.Name != this.sourceNamespace) {
							// we're not interested in this namespace...
							compilationUnit.Children.RemoveAt(i);
						} else {
							
							// this NamespaceDeclaration presumably contains our source class
							// walk its children backwards to that removing them won't break the iteration
							for(int j = ns.Children.Count-1; j>=0; j--) {
								nsChild = ns.Children[j];
								if (nsChild is TypeDeclaration) {
									td = (TypeDeclaration)nsChild;
									
									if (td.Name == this.sourceClassName) {
										// keep it, and substitute it for the current NamespaceDeclaration
										compilationUnit.Children[i] = td;
									} else {
										// it's not the class we're extracting from
										ns.Children.RemoveAt(j);
									}
								} else {
									// it's not even a class... (e.g. using, etc)
									ns.Children.RemoveAt(j);
								}
							}
						}
					} else {
						// we don't actually want to throw an exception here just because we havn't forseen the node type...
						//throw new NotSupportedException("trimming "+compilationUnit.Children[i].ToString()+" is not supported.");
					}
				}
				return base.VisitCompilationUnit(compilationUnit, data);
			}
			
			public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
			{
				// rewrite the type declaration to an interface
				typeDeclaration.Attributes.Clear();
				typeDeclaration.BaseTypes.Clear();
				
				typeDeclaration.Type = NR.Ast.ClassType.Interface;
				typeDeclaration.Name = newInterfaceName;
				
				// remove those children who are not explicitly listed in our 'membersToInclude' dictionary
				// we walk backwards so that deletions don't affect the iteration
				bool keepIt;
				MethodDeclaration method;
				PropertyDeclaration property;
				object child;
				for (int i = typeDeclaration.Children.Count-1; i >= 0; i--) {
					keepIt = false;
					child = typeDeclaration.Children[i];
					if (child is MethodDeclaration) {
						method = (MethodDeclaration)child;
						if (membersToInclude.ContainsKey(method.Name)
						    && ((method.Modifier & Modifiers.Static) == Modifiers.None)) {
							keepIt = true;
						}
					} else if (child is PropertyDeclaration) {
						property = (PropertyDeclaration)child;
						if (membersToInclude.ContainsKey(property.Name)
						    && ((property.Modifier & Modifiers.Static) == Modifiers.None)) {
							keepIt = true;
						}
					}
					
					if (!keepIt) {
						typeDeclaration.Children.RemoveAt(i);
					}
				}

				// must call the base method to ensure that this type's children get visited
				return base.VisitTypeDeclaration(typeDeclaration, data);
			}
			
			public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
			{
				// strip out the public modifier...
				methodDeclaration.Modifier = NR.Ast.Modifiers.None;
				
				// ...and the method body
				methodDeclaration.Body = BlockStatement.Null;

				return null;
			}
			
			public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
			{
				// strip out the public modifiers...
				propertyDeclaration.Modifier = NR.Ast.Modifiers.None;

				// ... and the body of any get block...
				if (propertyDeclaration.HasGetRegion) {
					propertyDeclaration.GetRegion.Block = BlockStatement.Null;
				}

				// ... and the body of any set block...
				if (propertyDeclaration.HasSetRegion) {
					propertyDeclaration.SetRegion.Block = BlockStatement.Null;
				}
				
				return null;
			}
		}
		
		public override string GenerateInterfaceForClass(string newInterfaceName,
		                                                 IList<IMember> membersToKeep,
		                                                 bool preserveComments,
		                                                 string sourceNamespace,
		                                                 string sourceClassName,
		                                                 string existingCode
		                                                )
		{
			string codeForNewInterface = "<insert code for '"+newInterfaceName+"' here>";
			NR.IParser parser = ParseFile(null, existingCode);
			if (parser == null) {
				return null;
			}
			// use a custom IAstVisitor to strip our class out of this file,
			// rewrite it as our desired interface, and strip out every
			// member except those we want to keep in our new interface.
			ExtractInterfaceVisitor extractInterfaceVisitor = new ExtractInterfaceVisitor(newInterfaceName,
			                                                                              sourceNamespace,
			                                                                              sourceClassName,
			                                                                              membersToKeep);
			parser.CompilationUnit.AcceptVisitor(extractInterfaceVisitor, null);

			// now use an output visitor for the appropriate language (based on
			// extension of the existing code file) to format the new interface.
			IOutputAstVisitor output = GetOutputVisitor();
			if (preserveComments) {
				// run the output visitor with the specials inserter to insert comments
				// NOTE: *all* comments will be preserved, even for code that has been
				//       removed to create the interface...
				// TODO: is it worth enhancing the SpecialsNodeInserter to attach comments directly to code so they can be filtered based on what is preserved after a transformation?
				using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(), output)) {
					parser.CompilationUnit.AcceptVisitor(output, null);
				}
			} else {
				// run the output visitor without the specials inserter
				parser.CompilationUnit.AcceptVisitor(output, null);
			}
			parser.Dispose();
			
			if (output.Errors.Count == 0) {
				// get the output
				codeForNewInterface = output.Text;
			} else {
				// dump errors into the new interface file...
				codeForNewInterface = String.Format("{0} {1}",
				                                    this.CommentToken, output.Errors.ErrorOutput);
			}

			// wrap the new code in the same comments/usings/namespace as the the original class file.
			string newFileContent = CreateNewFileLikeExisting(existingCode, codeForNewInterface);

			return newFileContent;
		}
		
		private class AddTypeToBaseTypesVisitor : NR.Visitors.AbstractAstVisitor
		{
			private TypeReference typeReference;
			
			public AddTypeToBaseTypesVisitor(string newTypeName)
			{
				this.typeReference = new TypeReference(newTypeName);
			}
			
			public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
			{
				// test the Type string property explicitly (rather than .BaseTypes.Contains())
				// to ensure that a matching type name is enough to prevent adding a second
				// reference.
				bool exists = false;
				foreach(TypeReference type in typeDeclaration.BaseTypes) {
					if (type.Type == this.typeReference.Type) {
						exists = true;
						break;
					}
				}
				if (!exists) {
					typeDeclaration.BaseTypes.Add(this.typeReference);
				}
				return base.VisitTypeDeclaration(typeDeclaration, data);
			}
		}
		
		public override string AddBaseTypeToClass(string existingCode, string newInterfaceName)
		{
			string newCode = existingCode;
			NR.IParser parser = ParseFile(null, existingCode);
			if (parser == null) {
				return null;
			}

			AddTypeToBaseTypesVisitor addTypeToBaseTypesVisitor = new AddTypeToBaseTypesVisitor(newInterfaceName);

			parser.CompilationUnit.AcceptVisitor(addTypeToBaseTypesVisitor, null);

			// now use an output visitor for the appropriate language (based on
			// extension of the existing code file) to format the new interface.
			IOutputAstVisitor output = GetOutputVisitor();
			
			// run the output visitor with the specials inserter to insert comments
			using (SpecialNodesInserter.Install(parser.Lexer.SpecialTracker.RetrieveSpecials(), output)) {
				parser.CompilationUnit.AcceptVisitor(output, null);
			}

			parser.Dispose();
			
			if (output.Errors.Count > 0) {
				ShowSourceCodeErrors(null, output.Errors.ErrorOutput);
				return null;
			}
			
			return output.Text;
		}
		#endregion
		
		#region FindUnusedUsingDeclarations
		protected class PossibleTypeReference
		{
			public string Name;
			public int TypeParameterCount;
			public IMethod ExtensionMethod;
			
			public PossibleTypeReference(string name)
			{
				this.Name = name;
			}
			
			public PossibleTypeReference(IdentifierExpression identifierExpression)
			{
				this.Name = identifierExpression.Identifier;
				this.TypeParameterCount = identifierExpression.TypeArguments.Count;
			}
			
			public PossibleTypeReference(TypeReference tr)
			{
				this.Name = tr.Type;
				this.TypeParameterCount = tr.GenericTypes.Count;
			}
			
			public PossibleTypeReference(IMethod extensionMethod)
			{
				this.ExtensionMethod = extensionMethod;
			}
			
			public override int GetHashCode()
			{
				int hashCode = 0;
				unchecked {
					if (Name != null) hashCode += 1000000007 * Name.GetHashCode();
					hashCode += 1000000009 * TypeParameterCount.GetHashCode();
					if (ExtensionMethod != null) hashCode += 1000000021 * ExtensionMethod.GetHashCode();
				}
				return hashCode;
			}
			
			public override bool Equals(object obj)
			{
				PossibleTypeReference other = obj as PossibleTypeReference;
				if (other == null) return false;
				return this.Name == other.Name && this.TypeParameterCount == other.TypeParameterCount && object.Equals(this.ExtensionMethod, other.ExtensionMethod);
			}
		}
		
		private class FindPossibleTypeReferencesVisitor : NR.Visitors.AbstractAstVisitor
		{
			internal HashSet<PossibleTypeReference> list = new HashSet<PossibleTypeReference>();
			NRefactoryResolver.NRefactoryResolver resolver;
			ParseInformation parseInformation;
			
			public FindPossibleTypeReferencesVisitor(ParseInformation parseInformation)
			{
				if (parseInformation != null) {
					this.parseInformation = parseInformation;
					resolver = new NRefactoryResolver.NRefactoryResolver(parseInformation.MostRecentCompilationUnit.ProjectContent.Language);
				}
			}
			
			public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
			{
				list.Add(new PossibleTypeReference(identifierExpression));
				return base.VisitIdentifierExpression(identifierExpression, data);
			}
			
			public override object VisitTypeReference(TypeReference typeReference, object data)
			{
				if (!typeReference.IsGlobal) {
					list.Add(new PossibleTypeReference(typeReference));
				}
				return base.VisitTypeReference(typeReference, data);
			}
			
			public override object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data)
			{
				list.Add(new PossibleTypeReference(attribute.Name));
				list.Add(new PossibleTypeReference(attribute.Name + "Attribute"));
				return base.VisitAttribute(attribute, data);
			}
			
			public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
			{
				base.VisitInvocationExpression(invocationExpression, data);
				// don't use uninitialized resolver
				if (resolver.ProjectContent == null)
					return null;
				if (invocationExpression.TargetObject is MemberReferenceExpression) {
					MemberResolveResult mrr = resolver.ResolveInternal(invocationExpression, ExpressionContext.Default) as MemberResolveResult;
					if (mrr != null) {
						IMethod method = mrr.ResolvedMember as IMethod;
						if (method != null && method.IsExtensionMethod) {
							list.Add(new PossibleTypeReference(method));
						}
					}
				}
				return null;
			}
			
			public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
			{
				// Initialize resolver for method:
				if (!methodDeclaration.Body.IsNull && resolver != null) {
					if (resolver.Initialize(parseInformation, methodDeclaration.Body.StartLocation.Y, methodDeclaration.Body.StartLocation.X)) {
						resolver.RunLookupTableVisitor(methodDeclaration);
					}
				}
				return base.VisitMethodDeclaration(methodDeclaration, data);
			}
			
			public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
			{
				if (resolver != null) {
					if (resolver.Initialize(parseInformation, propertyDeclaration.BodyStart.Y, propertyDeclaration.BodyStart.X)) {
						resolver.RunLookupTableVisitor(propertyDeclaration);
					}
				}
				return base.VisitPropertyDeclaration(propertyDeclaration, data);
			}
			
			public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
			{
				if (resolver != null) {
					resolver.Initialize(parseInformation, fieldDeclaration.StartLocation.X, fieldDeclaration.StartLocation.Y);
				}
				return base.VisitFieldDeclaration(fieldDeclaration, data);
			}
		}
		
		protected virtual HashSet<PossibleTypeReference> FindPossibleTypeReferences(IDomProgressMonitor progressMonitor, string fileContent, ParseInformation parseInfo)
		{
			NR.IParser parser = ParseFile(progressMonitor, fileContent);
			if (parser == null) {
				return null;
			} else {
				FindPossibleTypeReferencesVisitor visitor = new FindPossibleTypeReferencesVisitor(parseInfo);
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
		
		public override IList<IUsing> FindUnusedUsingDeclarations(IDomProgressMonitor progressMonitor, string fileName, string fileContent, ICompilationUnit cu)
		{
			IClass @class = cu.Classes.Count == 0 ? null : cu.Classes[0];
			
			HashSet<PossibleTypeReference> references = FindPossibleTypeReferences(progressMonitor, fileContent, new ParseInformation(cu));
			if (references == null) return new IUsing[0];
			
			HashSet<IUsing> usedUsings = new HashSet<IUsing>();
			foreach (PossibleTypeReference tr in references) {
				if (tr.ExtensionMethod != null) {
					// the invocation of an extension method can implicitly use a using
					StringComparer nameComparer = cu.ProjectContent.Language.NameComparer;
					// go through all usings in all nested child scopes
					foreach (IUsing import in cu.GetAllUsings()) {
						foreach (string i in import.Usings) {
							if (nameComparer.Equals(tr.ExtensionMethod.DeclaringType.Namespace, i)) {
								usedUsings.Add(import);
							}
						}
					}
				} else {
					// normal possible type reference
					SearchTypeRequest request = new SearchTypeRequest(tr.Name, tr.TypeParameterCount, @class, cu, 1, 1);
					SearchTypeResult response = cu.ProjectContent.SearchType(request);
					if (response.UsedUsing != null) {
						usedUsings.Add(response.UsedUsing);
					}
				}
			}
			
			List<IUsing> unusedUsings = new List<IUsing>();
			foreach (IUsing import in cu.GetAllUsings()) {
				if (!usedUsings.Contains(import)) {
					if (import.HasAliases) {
						foreach (string key in import.Aliases.Keys) {
							if (references.Contains(new PossibleTypeReference(key)))
								goto checkNextImport;
						}
					}
					unusedUsings.Add(import); // this using is unused
				}
				checkNextImport:;
			}
			return unusedUsings;
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
			NR.IParser parser = ParseFile(null, existingFileContent);
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
		
		private class RemoveTypesVisitor : NR.Visitors.AbstractAstTransformer
		{
			internal const string DummyIdentifier = "DummyNamespace!InsertionPos";
			
			internal int includeCommentsUpToLine;
			internal int includeCommentsAfterLine = int.MaxValue;
			
			internal bool firstType = true;
			
			public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
			{
				if (firstType) {
					includeCommentsUpToLine = usingDeclaration.EndLocation.Y;
				}
				return null;
			}
			
			public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
			{
				includeCommentsAfterLine = namespaceDeclaration.EndLocation.Y;
				if (firstType) {
					includeCommentsUpToLine = namespaceDeclaration.StartLocation.Y;
					return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
				} else {
					RemoveCurrentNode();
					return null;
				}
			}
			
			public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
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
			NR.Parser.ILexer lexer = NR.ParserFactory.CreateLexer(language, new StringReader(fileContent));
			// use the lexer to determine last token position before type start
			// and next token position after type end
			Stack<NR.Location> stack = new Stack<NR.Location>();
			NR.Location lastPos = NR.Location.Empty;
			NR.Parser.Token t = lexer.NextToken();
			bool csharp = language == NR.SupportedLanguage.CSharp;
			int eof = csharp ? NR.Parser.CSharp.Tokens.EOF : NR.Parser.VB.Tokens.EOF;
			int attribStart = csharp ? NR.Parser.CSharp.Tokens.OpenSquareBracket : NR.Parser.VB.Tokens.LessThan;
			int attribEnd = csharp ? NR.Parser.CSharp.Tokens.CloseSquareBracket : NR.Parser.VB.Tokens.GreaterThan;
			
			while (t.Kind != eof) {
				if (t.Kind == attribStart)
					stack.Push(lastPos);
				if (t.EndLocation.Y >= type.Region.BeginLine)
					break;
				lastPos = t.EndLocation;
				if (t.Kind == attribEnd && stack.Count > 0)
					lastPos = stack.Pop();
				t = lexer.NextToken();
			}
			
			stack = null;
			
			// Skip until end of type
			while (t.Kind != eof) {
				if (t.EndLocation.Y > type.BodyRegion.EndLine)
					break;
				t = lexer.NextToken();
			}
			
			int lastLineBefore = lastPos.IsEmpty ? 0 : lastPos.Y;
			int firstLineAfter = t.EndLocation.IsEmpty ? int.MaxValue : t.EndLocation.Y;
			
			lexer.Dispose();
			lexer = null;
			
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
							largestEmptyLineCount = emptyLinesInRow;
							resultBeginLine = lineNumber + 1;
						}
					} else {
						emptyLinesInRow = 0;
						if (IsEndDirective(trimLine)) {
							largestEmptyLineCount = 0;
							resultBeginLine = lineNumber + 1;
						}
					}
				} else if (lineNumber == type.Region.BeginLine) {
					mainLine = line;
				}
				// Region.BeginLine could be BodyRegion.EndLine
				if (lineNumber == type.BodyRegion.EndLine) {
					largestEmptyLineCount = 0;
					emptyLinesInRow = 0;
					resultEndLine = lineNumber;
				} else if (lineNumber > type.BodyRegion.EndLine) {
					if (lineNumber >= firstLineAfter)
						break;
					string trimLine = line.TrimStart();
					if (trimLine.Length == 0) {
						if (++emptyLinesInRow > largestEmptyLineCount) {
							largestEmptyLineCount = emptyLinesInRow;
							resultEndLine = lineNumber - emptyLinesInRow;
						}
					} else {
						emptyLinesInRow = 0;
						if (IsStartDirective(trimLine)) {
							break;
						}
					}
				}
			}
			
			myReader.Dispose();
			return new DomRegion(resultBeginLine, 0, resultEndLine, int.MaxValue);
		}
		
		static bool IsEndDirective(string trimLine)
		{
			return trimLine.StartsWith("#endregion") || trimLine.StartsWith("#endif");
		}
		
		static bool IsStartDirective(string trimLine)
		{
			return trimLine.StartsWith("#region") || trimLine.StartsWith("#if");
		}
		#endregion
	}
}
