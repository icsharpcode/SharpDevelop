// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToRubyConverter class.
	/// </summary>
	[TestFixture]
	public class EmptyCSharpClassConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"}";
	
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "class Foo\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}		
	}
}
