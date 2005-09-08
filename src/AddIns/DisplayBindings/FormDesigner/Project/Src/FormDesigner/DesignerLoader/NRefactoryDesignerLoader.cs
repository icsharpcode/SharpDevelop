// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.FormDesigner.Services;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.FormDesigner
{
	public class DefaultMemberRelationshipService : MemberRelationshipService
	{
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			return true;
		}
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			return base.GetRelationship(source);
		}
	}
	
	public class NRefactoryDesignerLoader : CodeDomDesignerLoader
	{
		bool                  loading               = true;
		IDesignerLoaderHost   designerLoaderHost    = null;
		ITypeResolutionService typeResolutionService = null;
		SupportedLanguages    language;
		CodeDomProvider       provider = new Microsoft.CSharp.CSharpCodeProvider();
		
		TextEditorControl textEditorControl;
		
		public string TextContent {
			get {
				return textEditorControl.Document.TextContent;
			}
		}
		
		public override bool Loading {
			get {
				return loading;
			}
		}
		
		public IDesignerLoaderHost DesignerLoaderHost {
			get {
				return designerLoaderHost;
			}
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get {
				return provider;
			}
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get {
				return typeResolutionService;
			}
		}
		
		protected override bool IsReloadNeeded()
		{
			return base.IsReloadNeeded() || TextContent != lastTextContent;
		}
		
		public NRefactoryDesignerLoader(SupportedLanguages language, TextEditorControl textEditorControl)
		{
			this.language = language;
			this.textEditorControl = textEditorControl;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			this.loading = true;
			typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			base.BeginLoad(host);
		}
		
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			this.loading = false;
			base.OnEndLoad(successful, errors);
		}
		
		string lastTextContent;
		
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Parse()");
			
			lastTextContent = TextContent;
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(language, new StringReader(lastTextContent));
			p.Parse();
			
			if (p.Errors.count > 0) {
				throw new FormDesignerLoadException(p.Errors.ErrorOutput);
			}
			
			// Try to fix the type names to fully qualified ones
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
			FixTypeNames(p.CompilationUnit, parseInfo.BestCompilationUnit);
			
			CodeDOMVisitor visitor = new CodeDOMVisitor();
			visitor.Visit(p.CompilationUnit, null);
			
			// output generated CodeDOM to the console :
//			CodeDOMVerboseOutputGenerator outputGenerator = new CodeDOMVerboseOutputGenerator();
//			outputGenerator.GenerateCodeFromMember(visitor.codeCompileUnit.Namespaces[0].Types[0], Console.Out, null);
//			provider.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
			return visitor.codeCompileUnit;
		}
		
		void FixTypeNames(object o, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu)
		{
			if (domCu == null)
				return;
			CompilationUnit cu = o as CompilationUnit;
			if (cu != null) {
				foreach (object c in cu.Children) {
					FixTypeNames(c, domCu);
				}
				return;
			}
			NamespaceDeclaration namespaceDecl = o as NamespaceDeclaration;
			if (namespaceDecl != null) {
				foreach (object c in namespaceDecl.Children) {
					FixTypeNames(c, domCu);
				}
				return;
			}
			TypeDeclaration typeDecl = o as TypeDeclaration;
			if (typeDecl != null) {
				foreach (TypeReference tref in typeDecl.BaseTypes) {
					FixTypeReference(tref, typeDecl.StartLocation, domCu);
				}
				foreach (object c in typeDecl.Children) {
					FixTypeNames(c, domCu);
				}
				return;
			}
			FieldDeclaration fieldDecl = o as FieldDeclaration;
			if (fieldDecl != null) {
				FixTypeReference(fieldDecl.TypeReference, fieldDecl.StartLocation, domCu);
				foreach (VariableDeclaration var in fieldDecl.Fields) {
					if (var != null) {
						FixTypeReference(var.TypeReference, fieldDecl.StartLocation, domCu);
					}
				}
			}
		}
		
		void FixTypeReference(TypeReference type, Point location, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu)
		{
			if (type == null)
				return;
			if (type.SystemType != type.Type)
				return;
			foreach (TypeReference tref in type.GenericTypes) {
				FixTypeReference(tref, location, domCu);
			}
			ICSharpCode.SharpDevelop.Dom.IClass curType = domCu.GetInnermostClass(location.Y, location.X);
			ICSharpCode.SharpDevelop.Dom.IReturnType rt = domCu.ProjectContent.SearchType(type.Type, curType, domCu, location.Y, location.X);
			if (rt != null) {
				type.Type = rt.FullyQualifiedName;
			}
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Info("DesignerLoader.Write called");
			provider.GenerateCodeFromCompileUnit(unit, Console.Out, null);
		}
		
//		public void Reload()
//		{
//			base.Reload(BasicDesignerLoader.ReloadFlags.Default);
//		}
//		public override void Flush()
//		{
//			base.Flush();
//		}
		
//		void InitializeExtendersForProject(IDesignerHost host)
//		{
//			IExtenderProviderService elsi = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
//			elsi.AddExtenderProvider(new ICSharpCode.FormDesigner.Util.NameExtender());
//		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
