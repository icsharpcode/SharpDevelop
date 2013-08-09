/*
 * Created by SharpDevelop.
 * User: Noam
 * Date: 26/12/2012
 * Time: 17:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
