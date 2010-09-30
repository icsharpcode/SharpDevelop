// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting.Tests.Designer
{
	public abstract class GenerateDesignerCodeTestsBase
	{
		protected string generatedCode;

		protected abstract IScriptingCodeDomSerializer CreateSerializer();
	}
}
