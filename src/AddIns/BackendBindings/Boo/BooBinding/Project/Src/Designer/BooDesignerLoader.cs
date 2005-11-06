// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.FormDesigner;
using ICSharpCode.FormDesigner.Services;
using Boo.Lang.Parser;
using Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerLoader : CodeDomDesignerLoader
	{
		bool                  loading               = true;
		IDesignerLoaderHost   designerLoaderHost    = null;
		IDesignerGenerator    generator;
		ITypeResolutionService typeResolutionService = null;
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
		
		public BooDesignerLoader(TextEditorControl textEditorControl, IDesignerGenerator generator)
		{
			this.textEditorControl = textEditorControl;
			this.generator = generator;
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
			LoggingService.Debug("BooDesignerLoader.Parse()");
			
			lastTextContent = TextContent;
			BooParsingStep step = new BooParsingStep();
			
			StringBuilder errors = new StringBuilder();
			Module module = BooParser.ParseModule(4, new CompileUnit(), "BooDesignerLoaderModule",
			                                      new StringReader(lastTextContent),
			                                      delegate(antlr.RecognitionException e) {
			                                      	errors.AppendLine(e.ToString());
			                                      });
			
			if (errors.Length > 0) {
				throw new FormDesignerLoadException(errors.ToString());
			}
			
			// Try to fix the type names to fully qualified ones
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
			
			/*
			bool foundInitMethod = false;
			//FixTypeNames(p.CompilationUnit, parseInfo.BestCompilationUnit, ref foundInitMethod);
			if (!foundInitMethod)
				throw new FormDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			 */
			
			CodeDomVisitor visitor = new CodeDomVisitor(parseInfo.MostRecentCompilationUnit.ProjectContent);
			module.Accept(visitor);
			
			// output generated CodeDOM to the console :
			ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator outputGenerator = new ICSharpCode.NRefactory.Parser.CodeDOMVerboseOutputGenerator();
			outputGenerator.GenerateCodeFromMember(visitor.OutputCompileUnit.Namespaces[0].Types[0], Console.Out, null);
			provider.GenerateCodeFromCompileUnit(visitor.OutputCompileUnit, Console.Out, null);
			
			LoggingService.Debug("BooDesignerLoader.Parse() finished");
			return visitor.OutputCompileUnit;
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Info("BooDesignerLoader.Write called");
			generator.MergeFormChanges(unit);
		}
	}
}
