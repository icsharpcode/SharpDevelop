// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
