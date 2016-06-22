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
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	public class CSharpDesignerLoaderProvider : IDesignerLoaderProvider
	{
		public DesignerLoader CreateLoader(FormsDesignerViewContent viewContent)
		{
			return new CSharpDesignerLoader(new CSharpFormsDesignerLoaderContext(viewContent));
		}
		
		public IReadOnlyList<OpenedFile> GetSourceFiles(FormsDesignerViewContent viewContent, out OpenedFile designerCodeFile)
		{
			// get new initialize components
			var parsedFile = SD.ParserService.ParseFile(viewContent.PrimaryFileName, viewContent.PrimaryFileContent);
			var compilation = SD.ParserService.GetCompilationForFile(viewContent.PrimaryFileName);
			foreach (var utd in parsedFile.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (FormsDesignerSecondaryDisplayBinding.IsDesignable(td)) {
					var initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(td);
					if (initializeComponents != null) {
						string designerFileName = initializeComponents.Region.FileName;
						if (designerFileName != null) {
							designerCodeFile = SD.FileService.GetOrCreateOpenedFile(designerFileName);
							return td.Parts
								.Select(p => SD.FileService.GetOrCreateOpenedFile(p.UnresolvedFile.FileName))
								.Distinct().ToList();
						}
					}
				}
			}
			
			throw new FormsDesignerLoadException("Could not find InitializeComponent method in any part of the open class.");
		}
	}
}
