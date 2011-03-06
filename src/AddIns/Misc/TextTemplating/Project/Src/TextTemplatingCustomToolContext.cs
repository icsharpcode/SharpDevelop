// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingCustomToolContext : ITextTemplatingCustomToolContext
	{
		CustomToolContext context;
		
		public TextTemplatingCustomToolContext(CustomToolContext context)
		{
			this.context = context;
		}
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName)
		{
			return context.EnsureOutputFileIsInProject(baseItem, outputFileName);
		}
	}
}
