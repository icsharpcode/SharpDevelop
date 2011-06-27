// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class TestableTextTemplatingFilePreprocessorCustomTool : TextTemplatingFilePreprocessorCustomTool
	{
		public FileProjectItem TemplateFilePassedToCreateTextTemplatingFilePreprocessor;
		public CustomToolContext ContextPassedToCreateTextTemplatingFilePreprocessor;
		public FakeTextTemplatingFilePreprocessor FakeTextTemplatingFilePreprocessor = new FakeTextTemplatingFilePreprocessor();
		
		protected override ITextTemplatingFilePreprocessor CreateTextTemplatingFilePreprocessor(
			FileProjectItem templateFile,
			CustomToolContext context)
		{
			TemplateFilePassedToCreateTextTemplatingFilePreprocessor = templateFile;
			ContextPassedToCreateTextTemplatingFilePreprocessor = context;
			
			return FakeTextTemplatingFilePreprocessor;
		}
	}
}
