// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests that the IsOverridable property returns the expected value.
	/// </summary>
	[TestFixture]
	public class AbstractEntityIsOverridableTestFixture
	{
		MockEntity entity;
		
		[SetUp]
		public void SetUp()
		{
			entity = new MockEntity();
		}
		
		[Test]
		public void NotOverridableByDefault()
		{
			Assert.IsFalse(entity.IsOverridable);
		}

		[Test]
		public void IsOverrideSet()
		{
			entity.Modifiers = ModifierEnum.Override;
			Assert.IsTrue(entity.IsOverridable);
		}
		
		[Test]
		public void IsVirtualSet()
		{
			entity.Modifiers = ModifierEnum.Virtual;
			Assert.IsTrue(entity.IsOverridable);
		}
		
		[Test]
		public void IsAbstractSet()
		{
			entity.Modifiers = ModifierEnum.Abstract;
			Assert.IsTrue(entity.IsOverridable);
		}
		
		[Test]
		public void IsAbstractAndSealedSet()
		{
			entity.Modifiers = ModifierEnum.Abstract | ModifierEnum.Sealed;
			Assert.IsFalse(entity.IsOverridable);
		}
	}
}
