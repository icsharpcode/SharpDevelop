// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingReflectionProjectContent : IReflectionProjectContent
	{
		public TextTemplatingReflectionProjectContent(IProjectContent projectContent)
			: this(projectContent as ReflectionProjectContent)
		{
		}
		
		TextTemplatingReflectionProjectContent(ReflectionProjectContent projectContent)
		{
			if (projectContent != null) {
				AssemblyLocation = projectContent.AssemblyLocation;
			}
		}
		
		public string AssemblyLocation { get; private set; }
	}
}
