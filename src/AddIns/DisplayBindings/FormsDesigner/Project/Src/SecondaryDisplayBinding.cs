// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Workbench;

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
		
		public static IMethod GetInitializeComponents(ITypeDefinition c)
		{
			foreach (IMethod method in c.Methods) {
				if (IsInitializeComponentsMethodName(method.Name) && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}

		public static bool BaseClassIsFormOrControl(ITypeDefinition c)
		{
			if (c == null)
				return false;
			// Simple test for fully qualified name
			foreach (var baseType in c.GetNonInterfaceBaseTypes()) {
				var baseTypeName = baseType.FullName;
				if (baseTypeName == "System.Windows.Forms.Form"
				    || baseTypeName == "System.Windows.Forms.UserControl")
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsDesignable(IUnresolvedFile parsedFile, ICompilation compilation)
		{
			if (parsedFile == null)
				return false;
			foreach (var utd in parsedFile.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (td != null) {
					IMethod method = GetInitializeComponents(td);
					if (method != null) {
						return BaseClassIsFormOrControl(td);
					}
				}
			}
			return false;
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorProvider) {
				FileName fileName      = viewContent.PrimaryFileName;
				if (fileName == null)
					return false;
				
				ITextEditor textEditor = viewContent.GetService<ITextEditor>();
				string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
				
				switch (fileExtension) {
					case ".cs":
//					case ".vb":
						var parsedFile = SD.ParserService.ParseFile(fileName, textEditor.Document);
						var compilation = SD.ParserService.GetCompilationForFile(fileName);
						if (IsDesignable(parsedFile, compilation))
							return true;
						break;
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			return new IViewContent[0];
			/*
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
//					loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.CSharp);
//					generator = new CSharpDesignerGenerator();
					throw new NotImplementedException();
					break;
				case ".vb":
					//loader    = new NRefactoryDesignerLoaderProvider(SupportedLanguage.VBNet);
					//generator = new VBNetDesignerGenerator();
					throw new NotImplementedException();
					break;
				default:
					throw new ApplicationException("Cannot create content for " + fileExtension);
			}
			return new IViewContent[] { new FormsDesignerViewContent(viewContent, loader, generator) }; */
		}
	}
}
