// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.RubyBinding;
using ICSharpCode.NRefactory;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ConverterSupportedLanguageTests
	{
		[Test]
		public void CSharpSupportedLanguage()
		{
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".cs", new ParseInformation());
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void VBNetSupportedLanguage()
		{
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".vb", new ParseInformation());
			Assert.AreEqual(SupportedLanguage.VBNet, converter.SupportedLanguage);
		}
		
		[Test]
		public void CSharpCaseInsensitive()
		{
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".CS", new ParseInformation());
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.IsNull(NRefactoryToRubyConverter.Create(null, new ParseInformation()));
		}

		[Test]
		public void TextFileCannotBeConverted()
		{
			Assert.IsNull(NRefactoryToRubyConverter.Create(".txt", new ParseInformation()));
		}
		
		[Test]
		public void CanConvertCSharpFiles()
		{
			Assert.IsTrue(NRefactoryToRubyConverter.CanConvert(".cs"));
		}
		
		[Test]
		public void CanConvertVBNetFiles()
		{
			Assert.IsTrue(NRefactoryToRubyConverter.CanConvert(".vb"));
		}
		
		[Test]
		public void CanConvertIsCaseInsensitive()
		{
			Assert.IsTrue(NRefactoryToRubyConverter.CanConvert(".CS"));
		}
		
		[Test]
		public void CannotConvertTextFile()
		{
			Assert.IsFalse(NRefactoryToRubyConverter.CanConvert(".txt"));
		}
		
		[Test]
		public void CannotConvertNullFileName()
		{
			Assert.IsFalse(NRefactoryToRubyConverter.CanConvert(null));
		}
	}
}
