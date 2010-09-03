// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageTestRunnerFactory : ICodeCoverageTestRunnerFactory
	{	
		public CodeCoverageTestRunner CreateCodeCoverageTestRunner()
		{
			return new CodeCoverageTestRunner();
		}
	}
}
