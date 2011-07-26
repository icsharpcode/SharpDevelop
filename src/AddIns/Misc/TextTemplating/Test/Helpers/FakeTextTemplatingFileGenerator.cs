// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingFileGenerator : ITextTemplatingFileGenerator
	{
		public bool IsProcessTemplateCalled;
		public bool IsDisposeCalledAfterTemplateProcessed;
		
		public void ProcessTemplate()
		{
			IsProcessTemplateCalled = true;
		}
		
		public void Dispose()
		{
			IsDisposeCalledAfterTemplateProcessed = IsProcessTemplateCalled;
		}
	}
}
