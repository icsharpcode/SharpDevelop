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
	public class MockTestFixture : TestFixture
	{
		public MockTestFixture(string fullName) : base(typeof(MockTestFixture))
		{
			base.TestName.FullName = fullName;
		}
	}
}
