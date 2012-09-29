// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcFileGenerationErrorReporter
	{
		void ShowErrors(CompilerErrorCollection errors);
	}
}
