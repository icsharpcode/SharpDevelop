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
	public class AbstractDecorationIsOverridableTestFixture
	{
		MockDecoration decoration;
		
		[SetUp]
		public void SetUp()
		{
			decoration = new MockDecoration();
		}
		
		[Test]
		public void NotOverridableByDefault()
		{
			Assert.IsFalse(decoration.IsOverridable);
		}

		[Test]
		public void IsOverrideSet()
		{
			decoration.Modifiers = ModifierEnum.Override;
			Assert.IsTrue(decoration.IsOverridable);
		}
		
		[Test]
		public void IsVirtualSet()
		{
			decoration.Modifiers = ModifierEnum.Virtual;
			Assert.IsTrue(decoration.IsOverridable);
		}
		
		[Test]
		public void IsAbstractSet()
		{
			decoration.Modifiers = ModifierEnum.Abstract;
			Assert.IsTrue(decoration.IsOverridable);
		}
		
		[Test]
		public void IsAbstractAndSealedSet()
		{
			decoration.Modifiers = ModifierEnum.Abstract | ModifierEnum.Sealed;
			Assert.IsFalse(decoration.IsOverridable);
		}
	}
}
