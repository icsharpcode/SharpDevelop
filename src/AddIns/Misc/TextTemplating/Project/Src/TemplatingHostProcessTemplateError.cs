// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;

namespace ICSharpCode.TextTemplating
{
	public class TemplatingHostProcessTemplateError : CompilerError
	{
		public TemplatingHostProcessTemplateError(Exception ex, string fileName)
		{
			this.ErrorText = ex.Message;
			this.FileName = fileName;
		}
	}
}
