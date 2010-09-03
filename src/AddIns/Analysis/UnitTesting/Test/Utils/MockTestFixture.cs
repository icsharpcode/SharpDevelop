// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
