// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class RazorFoldGeneratorFactory : IFoldGeneratorFactory
	{
		public RazorFoldGeneratorFactory(string fileExtension)
		{
			this.FileExtension = fileExtension;
		}
		
		string FileExtension { get; set; }
		
		public IFoldGenerator CreateFoldGenerator(ITextEditorWithParseInformationFolding textEditor)
		{
			return new ScheduledFoldGenerator(
				new FoldGenerator(textEditor, new RazorHtmlFoldParser(FileExtension)));
		}
	}
}
