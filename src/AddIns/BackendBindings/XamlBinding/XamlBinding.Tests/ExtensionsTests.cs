// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>


using System;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		public void StringReplaceTest1()
		{
			string text = "Hello World!";
			int index = 0;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest2()
		{
			string text = "My Hello World!";
			int index = 3;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "My Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest3()
		{
			string text = "Hello World!";
			int index = 6;
			int length = 5;
			string newText = "Byte";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello Byte!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest4()
		{
			string text = "Hello World!";
			int index = 11;
			int length = 1;
			string newText = "?";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello World?";
			
			Assert.AreEqual(expected, result);
		}
	}
}
