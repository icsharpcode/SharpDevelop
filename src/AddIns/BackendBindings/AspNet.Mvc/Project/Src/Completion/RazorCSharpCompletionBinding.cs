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
using System.IO;
using System.Web.Razor;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpCompletionBinding : ICodeCompletionBinding
	{
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			if (ch == '.') {
				var binding = CreateBinding(editor);
				return binding.HandleKeyPressed(editor, ch);
			}
			return false;
		}

		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}

		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			// We use HandleKeyPressed instead.
			return CodeCompletionKeyPressResult.None;
		}
		
		ICodeCompletionBinding CreateBinding(ITextEditor editor)
		{
			return SD.LanguageService.GetLanguageByExtension(".cs")
				.CreateCompletionBinding(FindExpressionToComplete(editor), CreateContext(editor));
		}

		string FindExpressionToComplete(ITextEditor editor)
		{
			int endOffset = editor.Caret.Offset;
			int startOffset = endOffset;
			while (startOffset > 0 && IsValidCharacter(editor.Document.GetCharAt(startOffset - 1)))
				startOffset--;
			return editor.Document.GetText(startOffset, endOffset - startOffset);
		}
		
		bool IsValidCharacter(char ch)
		{
			return Char.IsLetterOrDigit(ch) ||
				(ch == '.') ||
				(ch == '_');
		}

		ICodeContext CreateContext(ITextEditor editor)
		{
			var compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			var project = SD.ProjectService.FindProjectContainingFile(editor.FileName);
			
			var resolveContext = new SimpleTypeResolveContext(compilation.MainAssembly);
			
			var currentTypeDefinition = new DefaultUnresolvedTypeDefinition(project.RootNamespace, Path.GetFileNameWithoutExtension(editor.FileName));
			ITypeReference baseTypeReference = new GetClassTypeReference("System.Web.Mvc", "WebViewPage", 1);
			baseTypeReference = new ParameterizedTypeReference(baseTypeReference, new[] { FindModelType(editor) });
			currentTypeDefinition.BaseTypes.Add(baseTypeReference);
			
			var currentMethod = new DefaultUnresolvedMethod(currentTypeDefinition, "__ContextStub__");
			currentMethod.ReturnType = KnownTypeReference.Void;
			currentTypeDefinition.Members.Add(currentMethod);
			
			var currentResolvedTypeDef = new DefaultResolvedTypeDefinition(resolveContext, currentTypeDefinition);
			
			var projectContent = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			
			var currentFile = new CSharpUnresolvedFile();
			
			currentFile.RootUsingScope.AddSimpleUsing("System.Web.Mvc");
			currentFile.RootUsingScope.AddSimpleUsing("System.Web.Mvc.Ajax");
			currentFile.RootUsingScope.AddSimpleUsing("System.Web.Mvc.Html");
			currentFile.RootUsingScope.AddSimpleUsing("System.Web.Routing");
			
			currentFile.TopLevelTypeDefinitions.Add(currentTypeDefinition);
			
			if (projectContent != null) {
				compilation = projectContent.AddOrUpdateFiles(currentFile).CreateCompilation(SD.ParserService.GetCurrentSolutionSnapshot());
			}
			
			var context = new CSharpTypeResolveContext(compilation.MainAssembly,
			                                           currentFile.RootUsingScope.Resolve(compilation),
			                                           currentResolvedTypeDef,
			                                           currentMethod.CreateResolved(resolveContext.WithCurrentTypeDefinition(currentResolvedTypeDef)));
			return new CSharpResolver(context);
		}

		ITypeReference FindModelType(ITextEditor editor)
		{
			ParserResults results = ParseTemplate(editor.Document);
			string typeName = GetModelTypeName(results);
			if (string.IsNullOrWhiteSpace(typeName))
				return KnownTypeReference.Object;
			return new CSharpParser().ParseTypeReference(typeName)
				.ToTypeReference(NameLookupMode.BaseTypeReference);
		}
		
		ParserResults ParseTemplate(ITextSource textBuffer)
		{
			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
			var engine = new RazorTemplateEngine(host);
			return engine.ParseTemplate(textBuffer.CreateReader());
		}
		
		string GetModelTypeName(ParserResults results)
		{
			var visitor = new RazorCSharpParserModelTypeVisitor();
			results.Document.Accept(visitor);
			return visitor.ModelTypeName;
		}
	}
	
	static class NRUtils
	{
		/// <remarks>Does not support type arguments!</remarks>
		public static void AddSimpleUsing(this UsingScope scope, string fullName)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");
			string[] parts = fullName.Trim().Split('.');
			TypeOrNamespaceReference reference = null;
			foreach (var part in parts) {
				if (reference != null) {
					reference = new MemberTypeOrNamespaceReference(reference, part, EmptyList<ITypeReference>.Instance);
				} else {
					reference = new SimpleTypeOrNamespaceReference(part, EmptyList<ITypeReference>.Instance);
				}
			}
			
			scope.Usings.AddIfNotNull(reference);
		}
	}
}
