// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.Utils.Tests
{
	/// <summary>
	/// Description of CamelHumpsMatchTests.
	/// </summary>
	[TestFixture]
	public class CamelHumpsMatchTests
	{
		[Test]
		public void EntityNameStartWithEntityPartName_ShouldReturnTrue()
		{
			bool result = "abc".AutoCompleteWithCamelHumpsMatch("ab");
			Assert.AreEqual(result,true);
		}
		
		[Test]
		public void EntityNameContainEntityPartName_ShouldReturnTrue()
		{
			bool result = "abc".AutoCompleteWithCamelHumpsMatch("b");
			Assert.AreEqual(result,true);
		}
		
		[Test]
		public void EntityNameDoesntContainEntityPartName_ShouldReturnFalse()
		{
			bool result = "abc".AutoCompleteWithCamelHumpsMatch("de");
			Assert.AreEqual(result,false);
		}
		
		[Test]
		public void EntityPartNameIsTheFirstLetterInWordsByCamelHumps_ShouldReturnTrue()
		{
			bool result = "AbcDefGh".AutoCompleteWithCamelHumpsMatch("AD");
			Assert.AreEqual(result,true);
		}
	}
}
