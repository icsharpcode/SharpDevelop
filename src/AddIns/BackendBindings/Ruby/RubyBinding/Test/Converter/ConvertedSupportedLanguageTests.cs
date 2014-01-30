// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
