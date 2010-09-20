// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using ICSharpCode.CodeCoverage;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Description of CodeCoverageResultsTestsBase.
	/// </summary>
	public abstract class CodeCoverageResultsTestsBase
	{
		public CodeCoverageResults results;
		
		public CodeCoverageResults CreateCodeCoverageResults(string xml)
		{
			StringReader reader = new StringReader(xml);
			results = new CodeCoverageResults(reader);
			return results;
		}
		
		public CodeCoverageModule FirstModule {
			get { return results.Modules[0]; }
		}
		
		public CodeCoverageModule SecondModule {
			get { return results.Modules[1]; }
		}
		
		public CodeCoverageMethod FirstModuleFirstMethod {
			get { return FirstModule.Methods[0]; }
		}
		
		public CodeCoverageMethod FirstModuleSecondMethod {
			get { return FirstModule.Methods[1]; }
		}
		
		public CodeCoverageSequencePoint FirstModuleFirstMethodFirstSequencePoint {
			get { return FirstModuleFirstMethod.SequencePoints[0]; }
		}
		
		public CodeCoverageSequencePoint FirstModuleFirstMethodSecondSequencePoint {
			get { return FirstModuleFirstMethod.SequencePoints[1]; }
		}
		
		public CodeCoverageSequencePoint FirstModuleFirstMethodThirdSequencePoint {
			get { return FirstModuleFirstMethod.SequencePoints[2]; }
		}
		
		public CodeCoverageSequencePoint CreateSequencePoint()
		{
			return new CodeCoverageSequencePoint();
		}
	}
}
