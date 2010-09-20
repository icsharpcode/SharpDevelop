// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			string expectedCode = 
				"  self._components = System.ComponentModel.Container()\r\n" +
				"  self._listView = System.Windows.Forms.ListView()\r\n";
			
			Assert.AreEqual(expectedCode, codeBuilder.ToString());
		}
	}
}
