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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using Microsoft.CSharp;
using CSharpBinding.Parser;
using CSharpBinding.Refactoring;

namespace CSharpBinding.FormsDesigner
{
	public class CSharpDesignerLoader : AbstractCodeDomDesignerLoader
	{
		IUnresolvedTypeDefinition primaryPart;

		readonly CodeDomProvider codeDomProvider = new CSharpCodeProvider();
		readonly ICSharpDesignerLoaderContext context;
		
		public CSharpDesignerLoader(ICSharpDesignerLoaderContext context)
		{
			this.context = context;
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
		
		protected override void Initialize()
		{
			base.Initialize();
			
			base.DesignerLoaderHost.AddService(typeof(System.ComponentModel.Design.IEventBindingService), new CSharpEventBindingService(context, base.DesignerLoaderHost, this));
		}
		
		public ITypeDefinition GetPrimaryTypeDefinition()
		{
			return primaryPart.Resolve(new SimpleTypeResolveContext(context.GetCompilation().MainAssembly)).GetDefinition();
		}
		
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
			ITypeDefinition designerClass = FormsDesignerSecondaryDisplayBinding.GetDesignableClass(primaryParseInfo.UnresolvedFile, compilation, out primaryPart);
			IMethod initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(designerClass);
			
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
				codeField.Attributes = GetAccessibility(field);
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
				MessageService.ShowWarning("The form must be the first class in the file in order for form resources to be compiled correctly.\n" +
				                           "Please move other classes below the form class definition or move them to other files.");
			}
			
			return codeUnit;
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Info("DesignerLoader.Write called");
			// output generated CodeDOM to the console :
			#if DEBUG
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				this.CodeDomProvider.GenerateCodeFromCompileUnit(unit, Console.Out, null);
			}
			#endif
			try {
				var generator = new CSharpDesignerGenerator(context);
				generator.MergeFormChanges(unit);
			} catch (Exception ex) {
				SD.AnalyticsMonitor.TrackException(ex);
				MessageService.ShowException(ex);
			}
		}

		MemberAttributes GetAccessibility(IField field)
		{
			switch (field.Accessibility) {
				case Accessibility.None:
				case Accessibility.Private:
					return MemberAttributes.Private;
				case Accessibility.Public:
					return MemberAttributes.Public;
				case Accessibility.Protected:
					return MemberAttributes.Family;
				case Accessibility.Internal:
					return MemberAttributes.Assembly;
				case Accessibility.ProtectedOrInternal:
					return MemberAttributes.FamilyOrAssembly;
				case Accessibility.ProtectedAndInternal:
					return MemberAttributes.FamilyAndAssembly;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		protected override CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			SD.Log.Debug("CSharpDesignerLoader.GetCurrentLocalizationModelFromDesignedFile()");
			
			var primaryParseInfo = context.GetPrimaryFileParseInformation();
			var compilation = context.GetCompilation();
			
			// Find designer class
			ITypeDefinition designerClass = FormsDesignerSecondaryDisplayBinding.GetDesignableClass(primaryParseInfo.UnresolvedFile, compilation, out primaryPart);
			IMethod initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(designerClass);
			
			if (initializeComponents == null) {
				throw new FormsDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			}
			
			
			CSharpFullParseInformation designerParseInfo;
			var initializeComponentsDeclaration = initializeComponents.GetDeclaration(out designerParseInfo);
			
			FindLocalizationModelVisitor visitor = new FindLocalizationModelVisitor();
			initializeComponentsDeclaration.AcceptVisitor(visitor);
			if (visitor.Model != CodeDomLocalizationModel.None) {
				return visitor.Model;
			}

			return CodeDomLocalizationModel.None;
		}
		
		sealed class FindLocalizationModelVisitor : DepthFirstAstVisitor
		{
			CodeDomLocalizationModel model = CodeDomLocalizationModel.None;
			
			public CodeDomLocalizationModel Model {
				get { return this.model; }
			}
			
			public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
			{
				if (this.model != CodeDomLocalizationModel.PropertyReflection) {
					IdentifierExpression iex = memberReferenceExpression.Target as IdentifierExpression;
					if (iex != null && iex.Identifier == "resources") {
						if (memberReferenceExpression.MemberName == "ApplyResources") {
							this.model = CodeDomLocalizationModel.PropertyReflection;
						} else if (memberReferenceExpression.MemberName == "GetString") {
							this.model = CodeDomLocalizationModel.PropertyAssignment;
						}
					}
				}
				base.VisitMemberReferenceExpression(memberReferenceExpression);
			}
		}
		
		protected override void OnComponentRename(object component, string oldName, string newName)
		{
			base.OnComponentRename(component, oldName, newName);
			if (oldName != newName) {
				var primaryParseInfo = context.GetPrimaryFileParseInformation();
				var compilation = context.GetCompilation();
				
				// Find designer class
				ITypeDefinition designerClass = FormsDesignerSecondaryDisplayBinding.GetDesignableClass(primaryParseInfo.UnresolvedFile, compilation, out primaryPart);
				
				ISymbol controlSymbol;
				if (DesignerLoaderHost != null && DesignerLoaderHost.RootComponent == component)
					controlSymbol = designerClass;
				else
					controlSymbol = designerClass.GetFields(f => f.Name == oldName, GetMemberOptions.IgnoreInheritedMembers)
						.SingleOrDefault();
				if (controlSymbol != null) {
					AsynchronousWaitDialog.ShowWaitDialogForAsyncOperation(
						"${res:SharpDevelop.Refactoring.Rename}",
						progressMonitor =>
						FindReferenceService.RenameSymbol(controlSymbol, newName, progressMonitor)
						.ObserveOnUIThread()
						.Subscribe(error => SD.MessageService.ShowError(error.Message), // onNext
						           ex => SD.MessageService.ShowException(ex), // onError
						           // onCompleted - force refresh of the DesignerCodeFile's parse info, because the code generator
						           // seems to work with an outdated version, when the document is saved.
						           () => SD.ParserService.Parse(new FileName(context.DesignerCodeFileDocument.FileName), context.DesignerCodeFileDocument)
						          )
					);

				}
			}
		}
	}
}
