// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace PythonBinding.Tests.PythonLanguage
{
	[TestFixture]
	public class PythonLanguageBindingTestFixture
	{
		PythonLanguageBinding languageBinding;
		
		[SetUp]
		public void Init()
		{
			languageBinding = new PythonLanguageBinding();
		}
		
		[Test]
		public void FormattingStrategyIsPythonFormattingStrategy()
		{
			Assert.IsInstanceOf(typeof(PythonFormattingStrategy), languageBinding.FormattingStrategy);
		}
		
		[Test]
		public void ImplementsILanguageBinding()
		{
			Assert.IsNotNull(languageBinding as ILanguageBinding);
		}
		
		[Test]
		public void LanguagePropertiesIsPythonLanguageProperties()
		{
			Assert.IsInstanceOf(typeof(PythonLanguageProperties), languageBinding.Properties);
		}
	}
}
