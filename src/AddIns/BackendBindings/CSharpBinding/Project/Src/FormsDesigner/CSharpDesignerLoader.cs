// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using Microsoft.CSharp;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	public class CSharpDesignerLoader : AbstractCodeDomDesignerLoader
	{
		readonly CodeDomProvider codeDomProvider = new CSharpCodeProvider();
		readonly ICSharpDesignerLoaderContext context;
		
		public CSharpDesignerLoader(ICSharpDesignerLoaderContext context)
		{
			this.context = context;
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			throw new NotImplementedException();
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get {
				return codeDomProvider;
			}
		}
		
		ITextSourceVersion lastTextContentVersion;
		
		protected override bool IsReloadNeeded()
		{
			return base.IsReloadNeeded() || context.DesignerCodeFileDocument.Version.Equals(lastTextContentVersion);
		}
		
		IUnresolvedTypeDefinition primaryPart;
		
		// Steps to load the designer:
		// - Parse main file
		// - Find other files containing parts of the form
		// - Parse all files and look for fields (for controls) and InitializeComponents method
		// - Create CodeDom objects for fields and InitializeComponents statements
		// - If debug build and Ctrl pressed, output CodeDom to console
		// - Return CodeDom objects to the .NET designer
		protected override CodeCompileUnit Parse()
		{
			SD.Log.Debug("CSharpDesignerLoader.Parse()");
			
			lastTextContentVersion = context.DesignerCodeFileDocument.Version;
			var primaryParseInfo = context.GetPrimaryFileParseInformation();
			var compilation = context.GetCompilation();
			
			// Find designer class
			ITypeDefinition designerClass = null;
			IMethod initializeComponents = null;
			foreach (var utd in primaryParseInfo.UnresolvedFile.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (td != null && FormsDesignerSecondaryDisplayBinding.IsDesignable(td)) {
					primaryPart = utd;
					designerClass = td;
					initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(td);
					break;
				}
			}
			
			if (initializeComponents == null) {
				throw new FormsDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			}
			Debug.Assert(primaryPart != null);
			Debug.Assert(designerClass != null);
			
			bool isFirstClassInFile = primaryParseInfo.UnresolvedFile.TopLevelTypeDefinitions[0] == primaryPart;
			
			// TODO: translate
			const string missingReferenceMessage = "Your project is missing a reference to '${Name}' - please add it using 'Project > Add Reference'.";
			if (compilation.FindType(typeof(System.Drawing.Point)).Kind == TypeKind.Unknown) {
				throw new FormsDesignerLoadException(StringParser.Parse(missingReferenceMessage, new StringTagPair("Name", "System.Drawing")));
			}
			if (compilation.FindType(typeof(System.Windows.Forms.Form)).Kind == TypeKind.Unknown) {
				throw new FormsDesignerLoadException(StringParser.Parse(missingReferenceMessage, new StringTagPair("Name" , "System.Windows.Forms")));
			}
			
			CodeDomConvertVisitor cv = new CodeDomConvertVisitor();
			cv.UseFullyQualifiedTypeNames = true;
			
			CSharpFullParseInformation designerParseInfo;
			MethodDeclaration initializeComponentsDeclaration = initializeComponents.GetDeclaration(out designerParseInfo) as MethodDeclaration;
			if (initializeComponentsDeclaration == null)
				throw new FormsDesignerLoadException("Could not find source code for InitializeComponents");
			var resolver = designerParseInfo.GetResolver(compilation);
			var codeMethod = (CodeMemberMethod) cv.Convert(initializeComponentsDeclaration, resolver);
			var codeClass = new CodeTypeDeclaration(designerClass.Name);
			codeClass.Attributes = MemberAttributes.Public;
			codeClass.BaseTypes.AddRange(designerClass.DirectBaseTypes.Select(cv.Convert).ToArray());
			codeClass.Members.Add(codeMethod);
			
			foreach (var field in designerClass.Fields) {
				var codeField = new CodeMemberField(cv.Convert(field.Type), field.Name);
				codeClass.Members.Add(codeField);
			}
			var codeNamespace = new CodeNamespace(designerClass.Namespace);
			codeNamespace.Types.Add(codeClass);
			var codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			
			// output generated CodeDOM to the console :
			#if DEBUG
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				CodeDomVerboseOutputGenerator outputGenerator = new CodeDomVerboseOutputGenerator();
				outputGenerator.GenerateCodeFromMember(codeMethod, Console.Out, null);
				this.CodeDomProvider.GenerateCodeFromCompileUnit(codeUnit, Console.Out, null);
			}
			#endif
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
			
			if (!isFirstClassInFile) {
				MessageService.ShowWarning("The form must be the first class in the file in order for form resources be compiled correctly.\n" +
				                           "Please move other classes below the form class definition or move them to other files.");
			}
			
			return codeUnit;
		}
	}
}
