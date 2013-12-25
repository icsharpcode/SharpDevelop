// 
// TestRefactoringContext.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.NRefactory.CSharp.CodeActions
{
	public class TestRefactoringContext : RefactoringContext
	{
		public static bool UseExplict {
			get;
			set;
		}

		internal string defaultNamespace;
		public override string DefaultNamespace {
			get {
				return defaultNamespace;
			}
		}

		internal readonly IDocument doc;
		readonly TextLocation location;
		List<TestRefactoringContext> projectContexts;
		
		public TestRefactoringContext (IDocument document, TextLocation location, CSharpAstResolver resolver) : base(resolver, CancellationToken.None)
		{
			this.doc = document;
			this.location = location;
			this.UseExplicitTypes = UseExplict;
			this.FormattingOptions = FormattingOptionsFactory.CreateMono ();
			UseExplict = false;
			Services.AddService (typeof(NamingConventionService), new TestNameService ());
			Services.AddService (typeof(CodeGenerationService), new DefaultCodeGenerationService ());
		}
		
		class TestNameService : NamingConventionService
		{
			public override IEnumerable<NamingRule> Rules {
				get {
					return DefaultRules.GetFdgRules ();
				}
			}
		}

		public override bool Supports(Version version)
		{
			return this.version == null || this.version.CompareTo(version) >= 0;
		}
		
		public override TextLocation Location {
			get { return location; }
		}

		public Version version;
		
		public CSharpFormattingOptions FormattingOptions { get; set; }

		public Script StartScript ()
		{
			return new TestScript (this);
		}
		
		sealed class TestScript : DocumentScript
		{
			readonly TestRefactoringContext context;
			public TestScript(TestRefactoringContext context) : base(context.doc, context.FormattingOptions, new TextEditorOptions ())
			{
				this.context = context;
			}
			
			public override Task Link (params AstNode[] nodes)
			{
				// check that all links are valid.
				foreach (var node in nodes) {
					Assert.IsNotNull (GetSegment (node));
				}
				return new Task (() => {});
			}
			
			public override Task<Script> InsertWithCursor(string operation, InsertPosition defaultPosition, IList<AstNode> nodes)
			{
				EntityDeclaration entity = context.GetNode<EntityDeclaration>();
				if (entity is Accessor) {
					entity = (EntityDeclaration) entity.Parent;
				}

				foreach (var node in nodes) {
					InsertBefore(entity, node);
				}
				var tcs = new TaskCompletionSource<Script> ();
				tcs.SetResult (this);
				return tcs.Task;
			}

			public override Task<Script> InsertWithCursor(string operation, ITypeDefinition parentType, Func<Script, RefactoringContext, IList<AstNode>> nodeCallback)
			{
				var unit = context.RootNode;
				var insertType = unit.GetNodeAt<TypeDeclaration> (parentType.Region.Begin);

				var startOffset = GetCurrentOffset (insertType.LBraceToken.EndLocation);
				var nodes = nodeCallback(this, context);
				foreach (var node in nodes.Reverse ()) {
					var output = OutputNode (1, node, true);
					if (parentType.Kind == TypeKind.Enum) {
						InsertText (startOffset, output.Text + (!parentType.Fields.Any() ? "" : ","));
					} else {
						InsertText (startOffset, output.Text);
					}
					output.RegisterTrackedSegments (this, startOffset);
				}
				var tcs = new TaskCompletionSource<Script> ();
				tcs.SetResult (this);
				return tcs.Task;
			}

			void Rename (AstNode node, string newName)
			{
				if (node is ObjectCreateExpression)
					node = ((ObjectCreateExpression)node).Type;

				if (node is InvocationExpression)
					node = ((InvocationExpression)node).Target;
			
				if (node is MemberReferenceExpression)
					node = ((MemberReferenceExpression)node).MemberNameToken;
			
				if (node is MemberType)
					node = ((MemberType)node).MemberNameToken;
			
				if (node is EntityDeclaration) 
					node = ((EntityDeclaration)node).NameToken;
			
				if (node is ParameterDeclaration) 
					node = ((ParameterDeclaration)node).NameToken;
				if (node is ConstructorDeclaration)
					node = ((ConstructorDeclaration)node).NameToken;
				if (node is DestructorDeclaration)
					node = ((DestructorDeclaration)node).NameToken;
				if (node is VariableInitializer)
					node = ((VariableInitializer)node).NameToken;
				Replace (node, new IdentifierExpression (newName));
			}

			public override void Rename (ISymbol symbol, string name)
			{
				if (symbol.SymbolKind == SymbolKind.Variable || symbol.SymbolKind == SymbolKind.Parameter) {
					Rename(symbol as IVariable, name);
					return;
				}

				FindReferences refFinder = new FindReferences ();

				foreach (var fileContext in context.projectContexts)
				{
					using (var newScript = (TestScript) fileContext.StartScript()) {
						refFinder.FindReferencesInFile(refFinder.GetSearchScopes(symbol), 
						                               fileContext.UnresolvedFile, 
						                               fileContext.RootNode as SyntaxTree, 
						                               fileContext.Compilation,
						                               (n, r) => newScript.Rename(n, name), 
						                               context.CancellationToken);
					}
				}
			}

			void Rename (IVariable variable, string name)
			{
				FindReferences refFinder = new FindReferences ();

				refFinder.FindLocalReferences(variable, 
				                              context.UnresolvedFile, 
				                              context.RootNode as SyntaxTree, 
				                              context.Compilation, (n, r) => Rename(n, name), 
				                              context.CancellationToken);
			}
			
			public override void CreateNewType (AstNode newType, NewTypeContext context)
			{
				var output = OutputNode (0, newType, true);
				InsertText (0, output.Text);
			}

			public override void DoGlobalOperationOn(IEnumerable<IEntity> entities, Action<RefactoringContext, Script, IEnumerable<AstNode>> callback, string operationDescripton)
			{
				foreach (var projectContext in context.projectContexts) {
					DoLocalOperationOn(projectContext, entities, callback);
				}
			}

			void DoLocalOperationOn(TestRefactoringContext localContext, IEnumerable<IEntity> entities, Action<RefactoringContext, Script, IEnumerable<AstNode>> callback)
			{
				List<AstNode> nodes = new List<AstNode>();
				FindReferences refFinder = new FindReferences();
				refFinder.FindCallsThroughInterface = true;
				refFinder.FindReferencesInFile(refFinder.GetSearchScopes(entities),
				                               localContext.UnresolvedFile,
				                               localContext.RootNode as SyntaxTree,
				                               localContext.Compilation,
				                               (node, result) => {
					                               nodes.Add(node);
				                               },
				                               CancellationToken.None);

				using (var script = localContext.StartScript()) {
					callback(localContext, script, nodes);
				}
			}
		}

		#region Text stuff

		public override bool IsSomethingSelected { get { return selectionStart > 0; }  }

		public override string SelectedText { get { return IsSomethingSelected ? doc.GetText (selectionStart, selectionEnd - selectionStart) : ""; } }
		
		int selectionStart;
		public override TextLocation SelectionStart { get { return doc.GetLocation (selectionStart); } }
		
		int selectionEnd;
		public override TextLocation SelectionEnd { get { return doc.GetLocation (selectionEnd); } }

		public override int GetOffset (TextLocation location)
		{
			return doc.GetOffset (location);
		}
		
		public override TextLocation GetLocation (int offset)
		{
			return doc.GetLocation (offset);
		}

		public override string GetText (int offset, int length)
		{
			return doc.GetText (offset, length);
		}
		
		public override string GetText (ISegment segment)
		{
			return doc.GetText (segment);
		}
		
		public override IDocumentLine GetLineByOffset (int offset)
		{
			return doc.GetLineByOffset (offset);
		}
		#endregion
		public string Text {
			get {
				return doc.Text;
			}
		}

		public static TestRefactoringContext Create (string content, bool expectErrors = false, CSharpParser parser = null)
		{
			return Create(new List<string>() { content }, 0, expectErrors, parser);
		}

		public static TestRefactoringContext Create (List<string> contents, int mainIndex, bool expectErrors = false, CSharpParser parser = null)
		{
			List<int> indexes = new List<int>();
			List<int> selectionStarts = new List<int>();
			List<int> selectionEnds = new List<int>();
			List<IDocument> documents = new List<IDocument>();
			List<CSharpUnresolvedFile> unresolvedFiles = new List<CSharpUnresolvedFile>();
			List<SyntaxTree> units = new List<SyntaxTree>();

			for (int i = 0; i < contents.Count; i++) {
				string content = contents[i];
				int idx = content.IndexOf("$");
				if (idx >= 0)
					content = content.Substring(0, idx) + content.Substring(idx + 1);
				int idx1 = content.IndexOf("<-");
				int idx2 = content.IndexOf("->");
				int selectionStart = 0;
				int selectionEnd = 0;
				if (0 <= idx1 && idx1 < idx2) {
					content = content.Substring(0, idx2) + content.Substring(idx2 + 2);
					content = content.Substring(0, idx1) + content.Substring(idx1 + 2);
					selectionStart = idx1;
					selectionEnd = idx2 - 2;
					idx = selectionEnd;
				}
				indexes.Add(idx);
				selectionStarts.Add(selectionStart);
				selectionEnds.Add(selectionEnd);
				var doc = new StringBuilderDocument(content);
				if (parser == null)
					parser = new CSharpParser();
				var unit = parser.Parse(content, "program_" + i + ".cs");
				if (!expectErrors) {
					if (parser.HasErrors) {
						Console.WriteLine(content);
						Console.WriteLine("----");
					}
					foreach (var error in parser.ErrorsAndWarnings) {
						Console.WriteLine(error.Message);
					}
					Assert.IsFalse(parser.HasErrors, "The file " + i + " contains unexpected parsing errors.");
				}
				else {
					Assert.IsTrue(parser.HasErrors, "Expected parsing errors, but the file " + i + "doesn't contain any.");
				}
				unit.Freeze();
				CSharpUnresolvedFile unresolvedFile = unit.ToTypeSystem();
				units.Add(unit);
				documents.Add(doc);
				unresolvedFiles.Add(unresolvedFile);
			}

			IProjectContent pc = new CSharpProjectContent ();
			pc = pc.AddOrUpdateFiles (unresolvedFiles);
			pc = pc.AddAssemblyReferences (new[] { CecilLoaderTests.Mscorlib, CecilLoaderTests.SystemCore });

			var compilation = pc.CreateCompilation ();
			List<TestRefactoringContext> contexts = new List<TestRefactoringContext>();

			for (int documentIndex = 0; documentIndex < documents.Count; ++documentIndex)
			{
				var doc = documents [documentIndex];
				var resolver = new CSharpAstResolver (compilation, units[documentIndex], unresolvedFiles[documentIndex]);
				TextLocation location = TextLocation.Empty;
				if (indexes[documentIndex] >= 0)
					location = doc.GetLocation (indexes[documentIndex]);
				var context = new TestRefactoringContext(doc, location, resolver) {
					selectionStart = selectionStarts[documentIndex],
					selectionEnd = selectionEnds[documentIndex],
					projectContexts = contexts,
					version = parser.CompilerSettings.LanguageVersion,
					defaultNamespace = "Test"
				};

				contexts.Add(context);
			}

			return contexts [mainIndex];
		}

		public string GetSideDocumentText(int index)
		{
			return projectContexts [index].Text;
		}
		
		internal static void Print (AstNode node)
		{
			var v = new CSharpOutputVisitor (Console.Out, FormattingOptionsFactory.CreateMono ());
			node.AcceptVisitor (v);
		}
	}


}
