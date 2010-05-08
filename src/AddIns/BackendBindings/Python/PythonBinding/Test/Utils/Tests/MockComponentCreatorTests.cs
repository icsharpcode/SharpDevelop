// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
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
