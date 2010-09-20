// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a VB.NET class is converted to Ruby successfully.
	/// </summary>
	[TestFixture]
	public class VBClassConversionTestFixture
	{
		string vb = "Namespace DefaultNamespace\r\n" +
					"    Public Class Class1\r\n" +
					"    End Class\r\n" +
					"End Namespace";

		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(vb);
			string expectedRuby = 
				"module DefaultNamespace\r\n" +
				"    class Class1\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
