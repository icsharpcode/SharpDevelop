// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".cs", CreateParseInfo());
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void VBNetSupportedLanguage()
		{
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".vb", CreateParseInfo());
			Assert.AreEqual(SupportedLanguage.VBNet, converter.SupportedLanguage);
		}
		
		[Test]
		public void CSharpCaseInsensitive()
		{
			NRefactoryToRubyConverter converter = NRefactoryToRubyConverter.Create(".CS", CreateParseInfo());
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.IsNull(NRefactoryToRubyConverter.Create(null, CreateParseInfo()));
		}

		[Test]
		public void TextFileCannotBeConverted()
		{
			Assert.IsNull(NRefactoryToRubyConverter.Create(".txt", CreateParseInfo()));
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
		
		ParseInformation CreateParseInfo()
		{
			return new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
		}
	}
}
