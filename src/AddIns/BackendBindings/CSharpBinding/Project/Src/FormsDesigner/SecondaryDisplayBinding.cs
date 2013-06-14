// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace CSharpBinding.FormsDesigner
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
			if (c == null)
				return null;
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

		public static bool IsDesignable(ITypeDefinition td)
		{
			return GetInitializeComponents(td) != null && BaseClassIsFormOrControl(td);
		}
		
		public static bool IsDesignable(IUnresolvedFile parsedFile, ICompilation compilation)
		{
			IUnresolvedTypeDefinition td;
			return GetDesignableClass(parsedFile, compilation, out td) != null;
		}
		
		public static ITypeDefinition GetDesignableClass(IUnresolvedFile parsedFile, ICompilation compilation, out IUnresolvedTypeDefinition primaryPart)
		{
			primaryPart = null;
			if (parsedFile == null)
				return null;
			foreach (var utd in parsedFile.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (IsDesignable(td)) {
					primaryPart = utd;
					return td;
				}
			}
			return null;
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			ITextEditor textEditor = viewContent.GetService<ITextEditor>();
			if (textEditor == null)
				return false;
			FileName fileName = viewContent.PrimaryFileName;
			if (fileName == null)
				return false;
			
			var parsedFile = SD.ParserService.ParseFile(fileName, textEditor.Document);
			var compilation = SD.ParserService.GetCompilationForFile(fileName);
			return IsDesignable(parsedFile, compilation);
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			if (viewContent.SecondaryViewContents.Any(c => c is FormsDesignerViewContent)) {
				return new IViewContent[0];
			}
			
			return new IViewContent[] {
				new FormsDesignerViewContent(viewContent, new CSharpDesignerLoaderProvider())
			};
		}
	}
}
