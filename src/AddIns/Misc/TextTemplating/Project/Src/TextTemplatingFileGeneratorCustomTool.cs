// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFileGeneratorCustomTool : TextTemplatingCustomTool
	{
		public override void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			using (var generator = CreateTextTemplatingFileGenerator(item, context)) {
				generator.ProcessTemplate();
			}
		}
		
		protected virtual ITextTemplatingFileGenerator CreateTextTemplatingFileGenerator(
			FileProjectItem templateFile,
			CustomToolContext context)
		{
			TextTemplatingHost host = CreateTextTemplatingHost(context.Project);
			var textTemplatingCustomToolContext = new TextTemplatingCustomToolContext(context);
			
			return new TextTemplatingFileGenerator(host, templateFile, textTemplatingCustomToolContext);
		}
	}
}