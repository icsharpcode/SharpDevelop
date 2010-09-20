// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockComponentCreatorTests
	{
		MockComponentCreator componentCreator;
		
		[SetUp]
		public void Init()
		{
			componentCreator = new MockComponentCreator();
		}
		
		[Test]
		public void GetTypeReturnsConstraintTypeWhenPassedSystemDataConstraintString()
		{
			Type type = componentCreator.GetType(typeof(Constraint).FullName);
			Assert.AreEqual(typeof(Constraint), type);
		}
		
		[Test]
		public void GetTypeReturnsUniqueConstraintTypeWhenPassedSystemDataUniqueConstraintString()
		{
			Type type = componentCreator.GetType(typeof(UniqueConstraint).FullName);
			Assert.AreEqual(typeof(UniqueConstraint), type);
		}
	}
}
