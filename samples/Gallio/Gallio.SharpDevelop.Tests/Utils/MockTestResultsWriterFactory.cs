// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Extension;
using Gallio.SharpDevelop;

namespace Gallio.SharpDevelop.Tests.Utils
{
	public class MockTestResultsWriterFactory : ITestResultsWriterFactory
	{
		MockTestResultsWriter testResultsWriter;
		
		public ITestResultsWriter Create(string fileName)
		{
			testResultsWriter = new MockTestResultsWriter(fileName);
			return testResultsWriter;
		}
		
		public MockTestResultsWriter TestResultsWriter {
			get { return testResultsWriter; }
		}
	}
}
