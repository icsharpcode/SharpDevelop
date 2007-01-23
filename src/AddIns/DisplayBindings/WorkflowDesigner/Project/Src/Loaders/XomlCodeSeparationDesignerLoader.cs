// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Text;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.CodeDom;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of XomlCodeSeparationDesignerLoader.
	/// </summary>
	public class XomlCodeSeparationDesignerLoader : XomlDesignerLoader
	{
		private string codeFileName;
		
		public XomlCodeSeparationDesignerLoader(IViewContent viewContent, string fileName, Stream stream, string codeFileName) : base(viewContent, fileName, stream)
		{
			this.codeFileName = codeFileName;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			// TODO: Install the Add the additional services into the designer here.
			LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService());
			LoaderHost.AddService(typeof(IEventBindingService), new EventBindingService(LoaderHost));
			LoaderHost.AddService(typeof(IWorkflowDesignerGeneratorService), new CSharpWorkflowDesignerGeneratorService(LoaderHost,codeFileName));
		}
		
		protected override void Load()
		{
			UpdateCCU();
			
			LoadFromXoml();
			
			LoaderHost.Activate();
		}
      
        CodeCompileUnit ccu;
		public void UpdateCCU()
		{
			LoggingService.Debug("UpdateCCU");

			TypeProvider typeProvider = (TypeProvider)this.GetService(typeof(ITypeProvider));

			if (ccu != null) 
				typeProvider.RemoveCodeCompileUnit(ccu);
			
			ccu = Parse();
			
			if (ccu != null) 
				typeProvider.AddCodeCompileUnit(ccu);
			
		}
		
		CodeCompileUnit Parse()
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Parse()");
			
			string fileContent = ParserService.GetParseableFileContent(codeFileName);
			
			ICSharpCode.NRefactory.IParser  parser = ICSharpCode.NRefactory.ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(fileContent));
			parser.Parse();
			if (parser.Errors.Count > 0) {
				throw new Exception("Syntax errors in " + codeFileName + ":\r\n" + parser.Errors.ErrorOutput);
			}
			
			CodeDomVisitor visitor = new CodeDomVisitor();
			visitor.VisitCompilationUnit(parser.CompilationUnit, null);
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
			return visitor.codeCompileUnit;
		}
		
	}
}
