// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of SecondaryDisplayBinding.
	/// </summary>
	public class WorkflowDesignerSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		public WorkflowDesignerSecondaryDisplayBinding()
		{
		}
		
		public bool ReattachWhenParserServiceIsReady {
			get {
				return true;
			}
		}
		
		
		public bool CanAttachTo(IViewContent content)
		{
			if (content == null)
				throw new ArgumentNullException("content");

			if (content is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)content;
				string fileExtension = String.Empty;
				string fileName      = content.PrimaryFileName;
				if (fileName == null)
					return false;
				
				switch (Path.GetExtension(fileName).ToLowerInvariant()) {
					case ".cs":
					case ".vb":
						ParseInformation info = ParserService.ParseFile(fileName, textAreaControlProvider.TextEditorControl.Document.TextContent, false);
						
						if (IsDesignable(info))
							return true;
						break;
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			//HACK: Just create a view for now
			return new IViewContent[] { new WorkflowDesignerSecondaryViewContent(viewContent)};
		}
		
		public static bool IsInitializeComponentsMethodName(string name)
		{
			return name == "InitializeComponents" || name == "InitializeComponent";
		}
		
		public static IMethod GetInitializeComponents(IClass c)
		{
			c = c.GetCompoundClass();
			foreach (IMethod method in c.Methods) {
				if (IsInitializeComponentsMethodName(method.Name) && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		public static bool BaseClassIsWorkflow(IClass c)
		{
			// Simple test for fully qualified name
			c = c.GetCompoundClass();
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseType.FullyQualifiedName == "System.Workflow.ComponentModel.Activity"
				    // also accept when could not be resolved
				    || baseType.FullyQualifiedName == "Activity" )
				{
					return true;
				}
			}
			
			IClass form = c.ProjectContent.GetClass("System.Workflow.ComponentModel.Activity", 0);
			if (form != null && c.IsTypeInInheritanceTree(form))
				return true;
			return false;
		}
		
		public static bool IsDesignable(ParseInformation info)
		{
			if (info != null) {
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					IMethod method = GetInitializeComponents(c);
					if (method != null) {
						return BaseClassIsWorkflow(c);
					}
				}
			}
			return false;
		}
	}
	
}
