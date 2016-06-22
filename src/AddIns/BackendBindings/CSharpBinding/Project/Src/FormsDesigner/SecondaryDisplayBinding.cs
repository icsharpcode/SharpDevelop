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
				    || baseTypeName == "System.Windows.Forms.UserControl"
				    || baseTypeName == "System.ComponentModel.Component")
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
