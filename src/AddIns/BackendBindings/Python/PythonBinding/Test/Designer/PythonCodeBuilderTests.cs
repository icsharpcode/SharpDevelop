// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonCodeBuilderTests
	{
		PythonCodeBuilder codeBuilder;
		
		[SetUp]
		public void Init()
		{
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = "\t";
		}
		
		[Test]
		public void AppendNewLine()
		{
			codeBuilder.AppendLine();
			Assert.AreEqual("\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendText()
		{
			codeBuilder.Append("abc");
			Assert.AreEqual("abc", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendIndentedText()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void IncreaseIndentTwice()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\t\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void DecreaseIndent()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndented("abc");
			codeBuilder.AppendLine();
			codeBuilder.DecreaseIndent();			
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\tabc\r\nabc", codeBuilder.ToString());
		}
		
		[Test]
		public void AppendIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("abc");
			Assert.AreEqual("\tabc\r\n", codeBuilder.ToString());
		}
		
		[Test]
		public void InitialIndentWhenCodeBuilderCreatedIsZero()
		{
			Assert.AreEqual(0, codeBuilder.Indent);
		}
		
		[Test]
		public void IncreaseIndentByOne()
		{
			codeBuilder.IncreaseIndent();
			Assert.AreEqual(1, codeBuilder.Indent);
		}
		
		[Test]
		public void LengthAfterAddingText()
		{
			codeBuilder.Append("abc");
			Assert.AreEqual(3, codeBuilder.Length);
		}
		
		[Test]
		public void IndentPassedToConstructor()
		{
			codeBuilder = new PythonCodeBuilder(2);
			codeBuilder.AppendIndented("abc");
			Assert.AreEqual("\t\tabc", codeBuilder.ToString());
		}
		
		[Test]
		public void InsertIndentedLine()
		{
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("def");
			codeBuilder.InsertIndentedLine("abc");
			Assert.AreEqual("\tabc\r\n\tdef\r\n", codeBuilder.ToString());
		}
		
		/// <summary>
		/// Check that the "self._components = System.ComponentModel.Container()" line is generated
		/// the once and before any other lines of code.
		/// </summary>
		[Test]
		public void AppendCreateComponentsContainerTwice()
		{
			codeBuilder.IndentString = "  ";
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("self._listView = System.Windows.Forms.ListView()");
			codeBuilder.InsertCreateComponentsContainer();
			codeBuilder.InsertCreateComponentsContainer();
			
			string expectedCode = "  self._components = System.ComponentModel.Container()\r\n" +
				"  self._listView = System.Windows.Forms.ListView()\r\n";
			
			Assert.AreEqual(expectedCode, codeBuilder.ToString());
		}
	}
}
