// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Core;

namespace UnitTesting.Tests.Utils
{
	public class MockTestCase : TestCase
	{
		public MockTestCase(string name) : base(null, name)
		{
		}
		
		public override void Run(TestCaseResult result)
		{
		}
	}
}
