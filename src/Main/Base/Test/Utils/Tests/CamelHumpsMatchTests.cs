// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.Utils.Tests
{
	[TestFixture]
	public class CamelHumpsMatchTests
	{
		[Test]
		public void EntityNameStartWithEntityPartName_ShouldReturnTrue()
		{
			bool result = GotoUtils.AutoCompleteWithCamelHumpsMatch("abc", "ab");
			Assert.IsTrue(result);
		}
		
		[Test]
		public void EntityNameContainEntityPartName_ShouldReturnTrue()
		{
			bool result = GotoUtils.AutoCompleteWithCamelHumpsMatch("abc", "b");
			Assert.IsTrue(result);
		}
		
		[Test]
		public void EntityNameDoesntContainEntityPartName_ShouldReturnFalse()
		{
			bool result = GotoUtils.AutoCompleteWithCamelHumpsMatch("abc", "de");
			Assert.IsFalse(result);
		}
		
		[Test]
		public void EntityPartNameIsTheFirstLetterInWordsByCamelHumps_ShouldReturnTrue()
		{
			bool result = GotoUtils.AutoCompleteWithCamelHumpsMatch("AbcDefGh", "AD");
			Assert.IsTrue(result);
		}
	}
}
