// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingPathResolver : ITextTemplatingPathResolver
	{
		ITextTemplatingVariables templatingVariables;
		ITextTemplatingEnvironment environment;
		
		public TextTemplatingPathResolver()
			: this(new TextTemplatingVariables(), new TextTemplatingEnvironment())
		{
		}
		
		public TextTemplatingPathResolver(
			ITextTemplatingVariables templatingVariables,
			ITextTemplatingEnvironment environment)
		{
			this.templatingVariables = templatingVariables;
			this.environment = environment;
		}
		
		public string ResolvePath(string path)
		{
			path = environment.ExpandEnvironmentVariables(path);
			return templatingVariables.ExpandVariables(path);
		}
	}
}
