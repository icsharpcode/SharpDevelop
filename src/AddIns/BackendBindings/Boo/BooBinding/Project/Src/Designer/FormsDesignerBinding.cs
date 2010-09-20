// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Grunwald.BooBinding.Designer
{
	public class FormsDesignerDisplayBinding : ISecondaryDisplayBinding
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
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorProvider) {
				ITextEditorProvider textEditorProvider = (ITextEditorProvider)viewContent;
				string fileExtension = String.Empty;
				string fileName      = viewContent.PrimaryFileName;
				if (fileName == null)
					return false;
				if (Path.GetExtension(fileName).Equals(".boo", StringComparison.OrdinalIgnoreCase)) {
					ParseInformation info = ParserService.ParseFile(fileName, textEditorProvider.TextEditor.Document);
					if (FormsDesignerSecondaryDisplayBinding.IsDesignable(info))
						return true;
				}
			}
			return false;
		}
		
		public IViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			if (viewContent.SecondaryViewContents.Any(c => c is FormsDesignerViewContent)) {
				return new IViewContent[0];
			}
			
			IDesignerLoaderProvider loader = new BooDesignerLoaderProvider();
			IDesignerGenerator generator = new BooDesignerGenerator();
			return new IViewContent[] { new FormsDesignerViewContent(viewContent, loader, generator) };
		}
	}
	
	public class BooDesignerLoaderProvider : IDesignerLoaderProvider
	{
		public BooDesignerLoaderProvider()
		{
		}
		
		public System.ComponentModel.Design.Serialization.DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new BooDesignerLoader(generator);
		}
	}
}
