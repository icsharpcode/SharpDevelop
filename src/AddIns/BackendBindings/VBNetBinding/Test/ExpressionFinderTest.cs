/*
 * Created by SharpDevelop.
 * User: DG
 * Date: 13.12.2005
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using VBNetBinding.Parser;

namespace VBNetBinding.Tests
{
	[TestFixture]
	public class ExpressionFinderTest
	{
		ExpressionFinder ef;
		
		[SetUp]
		public void Init()
		{
			ef = new ExpressionFinder();
		}
		
		void Test(string expr, int offset)
		{
			string fulltext = "Test\n " + expr + ".AnotherField \n TestEnde";
			Assert.AreEqual(expr, ef.FindFullExpression(fulltext, 6 + offset).Expression);
		}
		
		[Test]
		public void FieldReference()
		{
			Test("abc", 1);
			Test("abc.def", 6);
		}
		
		[Test]
		public void WithFieldReference()
		{
			Test(".abc", 2);
			Test(".abc.def", 7);
		}
		
		[Test]
		public void MethodCall()
		{
			Test("abc.Method().Method()", 16);
		}
	}
}
