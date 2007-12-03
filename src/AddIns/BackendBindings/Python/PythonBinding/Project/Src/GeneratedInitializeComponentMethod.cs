// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IronPython.CodeDom;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Stores the CodeTypeDeclaration for the form generated
	/// by the CodeDomProvider and the InitializeComponent
	/// method's CodeMember
	/// </summary>
	public class GeneratedInitializeComponentMethod
	{
		CodeTypeDeclaration type;
		CodeMemberMethod method;
		
		GeneratedInitializeComponentMethod(CodeTypeDeclaration type, CodeMemberMethod method)
		{
			this.type = type;
			this.method = method;
		}
		
		/// <summary>
		/// Gets the type associated with the InitializeComponent method.
		/// </summary>
		public CodeTypeDeclaration Type {
			get { return type; }
		}
		
		/// <summary>
		/// Gets the InitializeComponent method.
		/// </summary>
		public CodeMemberMethod Method {
			get { return method; }
		}
		
		/// <summary>
		/// Finds the InitializeComponent method in the CodeCompileUnit
		/// and returns a new GeneratedInitializeComponentMethod that 
		/// stores the InitializeComponent method (CodeMemberMethod) and the
		/// class (CodeTypeDeclaration) the method is in.
		/// </summary>
		public static GeneratedInitializeComponentMethod GetGeneratedInitializeComponentMethod(CodeCompileUnit unit)
		{
			// Get the generated form and InitializeComponent method
			// from the code compile unit.
			CodeTypeDeclaration generatedFormClass = null;
			CodeMemberMethod generatedInitializeComponentMethod = null;
			foreach (CodeNamespace n in unit.Namespaces) {
				foreach (CodeTypeDeclaration typeDeclaration in n.Types) {
					foreach (CodeTypeMember member in typeDeclaration.Members) {
						if (member.Name == "InitializeComponent") {
						//if (m is CodeMemberMethod && m.Name == "InitializeComponent") {
							generatedFormClass = typeDeclaration;
							generatedInitializeComponentMethod = (CodeMemberMethod)member;
							return new GeneratedInitializeComponentMethod(typeDeclaration, (CodeMemberMethod)member);
						}
					}
				}
			}
			return null;
		}
		
		
		/// <summary>
		/// Gets the non-generated InitializeComponents from parse info.
		/// </summary>
		public static IMethod GetInitializeComponents(ParseInformation parseInfo)
		{
			return GetInitializeComponents(parseInfo.BestCompilationUnit);
		}
		
		/// <summary>
		/// Gets the non-generated InitializeComponents from the compilation unit.
		/// </summary>
		public static IMethod GetInitializeComponents(ICompilationUnit unit)
		{
			foreach (IClass c in unit.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					IMethod method = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (method != null) {
						return method;
					}
				}
			}
			return null;			
		}
		
		/// <summary>
		/// Converts from the DOM region to a document region.
		/// </summary>
		public static DomRegion GetBodyRegionInDocument(IMethod method)
		{
			DomRegion bodyRegion = method.BodyRegion;
			return new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);			
		}
				
		/// <summary>
		/// Merges the generated code into the specified document.
		/// </summary>
		/// <param name="document">The document that the generated
		/// code will be merged into.</param>
		/// <param name="parseInfo">The current compilation unit
		/// for the <paramref name="document"/>.</param>
		public void Merge(IDocument document, ICompilationUnit compilationUnit)
		{
			// Get the document's initialize components method.
			IMethod documentInitializeComponentsMethod = GetInitializeComponents(compilationUnit);

			// Generate source code from the code DOM.
			string generatedCode = GenerateCode();
			Console.WriteLine("GeneratedCode: " + generatedCode);
			
			// Parse the generated source code so we can
			// find the InitializeComponent method. We can
			// only generate code for the entire form.
			IMethod generatedCodeInitializeComponentMethod = GetInitializeComponentFromGeneratedCode(generatedCode);
			string generatedInitializeComponentsMethodBody = GetMethodBody(generatedCodeInitializeComponentMethod, generatedCode);
	
			// Merge the code.
			DomRegion methodRegion = GetBodyRegionInDocument(documentInitializeComponentsMethod);
			int startOffset = document.PositionToOffset(new TextLocation(methodRegion.BeginColumn - 1, methodRegion.BeginLine - 1));
			int endOffset   = document.PositionToOffset(new TextLocation(methodRegion.EndColumn - 1, methodRegion.EndLine - 1));
			document.Replace(startOffset, endOffset - startOffset, generatedInitializeComponentsMethodBody);
		}
		
		/// <summary>
		/// Generates code from the code DOM.
		/// </summary>
		string GenerateCode()
		{
			// Set up the code generation options.
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			options.BlankLinesBetweenMembers = false;
			options.IndentString = "\t";
			
			// Generate the code.
			StringWriter writer = new StringWriter();
			PythonProvider provider = new PythonProvider();
			provider.GenerateCodeFromType(type, writer, options);
			
			return writer.ToString();
		}
		
		/// <summary>
		/// Gets the InitializeCompoment
		/// </summary>
		static IMethod GetInitializeComponentFromGeneratedCode(string source)
		{
			PythonParser parser = new PythonParser();
			DefaultProjectContent projectContent = new DefaultProjectContent();
			ICompilationUnit unit = parser.Parse(projectContent, @"GeneratedForm.py", source);
			return GetInitializeComponents(unit);
		}
		
		/// <summary>
		/// Gets the method body text.
		/// </summary>
		static string GetMethodBody(IMethod method, string source)
		{
			DocumentFactory factory = new DocumentFactory();
			IDocument document = factory.CreateDocument();
			document.TextContent = source;
			
			DomRegion methodRegion = GetBodyRegionInDocument(method);
			int startOffset = document.PositionToOffset(new TextLocation(methodRegion.BeginColumn - 1, methodRegion.BeginLine - 1));
			int endOffset  = document.PositionToOffset(new TextLocation(methodRegion.EndColumn - 1, methodRegion.EndLine - 1));
			return document.GetText(startOffset, endOffset - startOffset);
		}
	}
}
