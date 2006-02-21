// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// Ensures that a code coverage module that has no included methods 
	/// points is flagged as excluded.
	/// </summary>
	[TestFixture]
	public class ExcludedModuleTestFixture
	{
		[Test]
		public void IsExcluded()
		{
			CodeCoverageModule module = new CodeCoverageModule("test");
			CodeCoverageMethod method = new CodeCoverageMethod("Test1", "MyTestFixture");
			CodeCoverageSequencePoint pt = new CodeCoverageSequencePoint(@"c:\test\MyTestFixture.cs", 0, 10, 0, 10, 20, true);
			method.SequencePoints.Add(pt);
			module.Methods.Add(method);
			
			Assert.IsTrue(module.IsExcluded, "Module should be excluded.");
		}
	}
}
