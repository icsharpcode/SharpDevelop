// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
