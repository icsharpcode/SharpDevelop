// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingFilePreprocessor : ITextTemplatingFilePreprocessor
	{
		public bool IsPreprocessTemplateCalled;
		
		public void PreprocessTemplate()
		{
			IsPreprocessTemplateCalled = true;
		}
	}
}
