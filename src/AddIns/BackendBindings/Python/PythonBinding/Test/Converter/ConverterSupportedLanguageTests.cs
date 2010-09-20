// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
