// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFilePreprocessorCustomTool : TextTemplatingCustomTool
	{
		public override void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			var processor = CreateTextTemplatingFilePreprocessor(item, context);
			processor.PreprocessTemplate();
		}
		
		protected virtual ITextTemplatingFilePreprocessor CreateTextTemplatingFilePreprocessor(
			FileProjectItem templateFile,
			CustomToolContext context)
		{
			var host = CreateTextTemplatingHost(context.Project);
			var textTemplatingCustomToolContext = new TextTemplatingCustomToolContext(context);
			return new TextTemplatingFilePreprocessor(host, templateFile, textTemplatingCustomToolContext);
		}
	}
}
