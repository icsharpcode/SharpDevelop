// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class CreateConverterWithNullParseInfoTestFixture
	{		
		[Test]
		public void NRefactoryToRubyConverterCreateThrowsArgumentNullExceptionWhenParseInfoParameterIsNull()
		{
			ArgumentNullException ex = 
				Assert.Throws<ArgumentNullException>(delegate { NRefactoryToRubyConverter.Create("test.cs", null); });
			Assert.AreEqual("parseInfo", ex.ParamName);
		}
	}
}
