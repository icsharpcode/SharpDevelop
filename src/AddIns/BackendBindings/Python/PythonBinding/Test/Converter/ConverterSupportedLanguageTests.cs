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
using ICSharpCode.PythonBinding;
using ICSharpCode.NRefactory;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ConverterSupportedLanguageTests
	{
		[Test]
		public void CSharpSupportedLanguage()
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(".cs");
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void VBNetSupportedLanguage()
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(".vb");
			Assert.AreEqual(SupportedLanguage.VBNet, converter.SupportedLanguage);
		}
		
		[Test]
		public void CSharpCaseInsensitive()
		{
			NRefactoryToPythonConverter converter = NRefactoryToPythonConverter.Create(".CS");
			Assert.AreEqual(SupportedLanguage.CSharp, converter.SupportedLanguage);
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.IsNull(NRefactoryToPythonConverter.Create(null));
		}

		[Test]
		public void TextFileCannotBeConverted()
		{
			Assert.IsNull(NRefactoryToPythonConverter.Create(".txt"));
		}
		
		[Test]
		public void CanConvertCSharpFiles()
		{
			Assert.IsTrue(NRefactoryToPythonConverter.CanConvert(".cs"));
		}
		
		[Test]
		public void CanConvertVBNetFiles()
		{
			Assert.IsTrue(NRefactoryToPythonConverter.CanConvert(".vb"));
		}
		
		[Test]
		public void CanConvertIsCaseInsensitive()
		{
			Assert.IsTrue(NRefactoryToPythonConverter.CanConvert(".CS"));
		}
		
		[Test]
		public void CannotConvertTextFile()
		{
			Assert.IsFalse(NRefactoryToPythonConverter.CanConvert(".txt"));
		}
		
		[Test]
		public void CannotConvertNullFileName()
		{
			Assert.IsFalse(NRefactoryToPythonConverter.CanConvert(null));
		}
	}
}
