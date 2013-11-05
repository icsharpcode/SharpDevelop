// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
