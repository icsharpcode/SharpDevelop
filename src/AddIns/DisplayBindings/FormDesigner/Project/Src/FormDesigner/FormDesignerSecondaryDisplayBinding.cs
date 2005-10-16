// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.FormDesigner
{
	public class FormDesignerSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		public static IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" || method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		public static bool BaseClassIsFormOrControl(IClass c)
		{
			// Simple test for fully qualified name
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseType.FullyQualifiedName == "System.Windows.Forms.Form" || baseType.FullyQualifiedName == "System.Windows.Forms.UserControl") {
					return true;
				}
			}
			// Test for real base type (does not work while solution load thread is still running)
			IProjectContent pc = ProjectContentRegistry.WinForms;
			return c.IsTypeInInheritanceTree(pc.GetClass("System.Windows.Forms.Form")) || c.IsTypeInInheritanceTree(pc.GetClass("System.Windows.Forms.UserControl"));
		}
		
		public static bool IsDesignable(ParseInformation info)
		{
			if (info != null) {
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					if (BaseClassIsFormOrControl(c)) {
						IMethod method = GetInitializeComponents(c);
						if (method == null) {
							return false;
						}
						return true;
					}
				}
			}
			return false;
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				string fileExtension = String.Empty;
				string fileName      = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
				if (fileName == null)
					return false;
				
				fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
				
				switch (fileExtension) {
					case ".cs":
					case ".vb":
						ParseInformation info = ParserService.ParseFile(fileName, textAreaControlProvider.TextEditorControl.Document.TextContent, false, true);
						if (IsDesignable(info))
							return true;
						break;
					case ".xfrm":
						return true;
				}
			}
			return false;
		}
		
		public ISecondaryViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			string fileExtension = String.Empty;
			string fileName      = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
			
			fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
			
			IDesignerLoaderProvider loader;
			IDesignerGenerator generator;
			
			switch (fileExtension) {
				case ".cs":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.CSharp, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new CSharpDesignerGenerator();
					break;
				case ".vb":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.VBNet, ((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new VBNetDesignerGenerator();
					break;
				case ".xfrm":
					loader    = new XmlDesignerLoaderProvider(((ITextEditorControlProvider)viewContent).TextEditorControl);
					generator = new XmlDesignerGenerator();
					break;
				default:
					throw new ApplicationException("Cannot create content for " + fileExtension);
			}
			return new ISecondaryViewContent[] { new FormDesignerViewContent(viewContent, loader, generator) };
		}
	}
}
