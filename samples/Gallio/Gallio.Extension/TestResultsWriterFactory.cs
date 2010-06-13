// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Gallio.Extension
{
	public class TestResultsWriterFactory : ITestResultsWriterFactory
	{
		public ITestResultsWriter Create(string fileName)
		{
			return new TestResultsWriter(fileName);
		}
	}
}
