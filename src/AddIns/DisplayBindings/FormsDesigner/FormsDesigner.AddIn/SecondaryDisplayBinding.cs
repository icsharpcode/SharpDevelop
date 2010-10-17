// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerSecondaryDisplayBinding : ISecondaryDisplayBinding
	{
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get {
				return true;
			}
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

		public static bool BaseClassIsFormOrControl(IClass c)
		{
			// Simple test for fully qualified name
			c = c.GetCompoundClass();
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseType.FullyQualifiedName == "System.Windows.Forms.Form"
				    || baseType.FullyQualifiedName == "System.Windows.Forms.UserControl"
				    // also accept Form and UserControl when they could not be resolved
				    || baseType.FullyQualifiedName == "Form"
				    || baseType.FullyQualifiedName == "UserControl")
				{
					return true;
				}
			}
			
			IClass form = c.ProjectContent.GetClass("System.Windows.Forms.Form", 0);
			IClass userControl = c.ProjectContent.GetClass("System.Windows.Forms.UserControl", 0);
			if (form != null && c.IsTypeInInheritanceTree(form))
				return true;
			if (userControl != null && c.IsTypeInInheritanceTree(userControl))
				return true;
			return false;
		}

		public static bool IsDesignable(ParseInformation info)
		{
			if (info != null) {
				ICompilationUnit cu = (ICompilationUnit)info.CompilationUnit;
				foreach (IClass c in cu.Classes) {
					IMethod method = GetInitializeComponents(c);
					if (method != null) {
						return BaseClassIsFormOrControl(c);
					}
				}
			}
			return false;
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorProvider) {
				ITextEditorProvider textEditorProvider = (ITextEditorProvider)viewContent;
				string fileExtension = String.Empty;
				string fileName      = viewContent.PrimaryFileName;
				if (fileName == null)
					return false;
				
				fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
				
				switch (fileExtension) {
					case ".cs":
					case ".vb":
						ParseInformation info = ParserService.ParseFile(fileName, textEditorProvider.TextEditor.Document);
						
						if (IsDesignable(info))
							return true;
						break;
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			if (viewContent.SecondaryViewContents.Any(c => c is FormsDesignerViewContent)) {
				return new IViewContent[0];
			}
			
			string fileExtension = String.Empty;
			string fileName      = viewContent.PrimaryFileName;
			
			fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
			
			IDesignerLoaderProvider loader;
			IDesignerGenerator generator;
			
			switch (fileExtension) {
				case ".cs":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.CSharp);
					generator = new CSharpDesignerGenerator();
					break;
				case ".vb":
					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.VBNet);
					generator = new VBNetDesignerGenerator();
					break;
				default:
					throw new ApplicationException("Cannot create content for " + fileExtension);
			}
			return new IViewContent[] { new FormsDesignerViewContent(viewContent, loader, generator) };
		}
	}
}
