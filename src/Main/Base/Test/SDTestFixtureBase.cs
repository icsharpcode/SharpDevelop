// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Base class for test fixtures that use SD services.
	/// Calls <c>SD.InitializeForUnitTests()</c> in the fixture set up, and <c>SD.TearDownForUnitTests</c> in the fixture tear down.
	/// </summary>
	public class SDTestFixtureBase
	{
		[TestFixtureSetUp]
		public virtual void FixtureSetUp()
		{
			SD.InitializeForUnitTests();
		}
		
		[TestFixtureTearDown]
		public virtual void FixtureTearDown()
		{
			SD.TearDownForUnitTests();
		}
	}
}
