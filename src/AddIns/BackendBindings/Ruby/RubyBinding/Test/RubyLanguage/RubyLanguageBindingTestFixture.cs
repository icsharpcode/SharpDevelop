// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace RubyBinding.Tests.RubyLanguage
{
	[TestFixture]
	public class RubyLanguageBindingTestFixture
	{
		RubyLanguageBinding languageBinding;
		
		[SetUp]
		public void Init()
		{
			languageBinding = new RubyLanguageBinding();
		}
		
		[Test]
		public void FormattingStrategyIsRubyFormattingStrategy()
		{
			Assert.IsInstanceOf(typeof(RubyFormattingStrategy), languageBinding.FormattingStrategy);
		}
		
		[Test]
		public void ImplementsILanguageBinding()
		{
			Assert.IsNotNull(languageBinding as ILanguageBinding);
		}
		
		[Test]
		public void LanguagePropertiesIsRubyLanguageProperties()
		{
			Assert.IsInstanceOf(typeof(RubyLanguageProperties), languageBinding.Properties);
		}
	}
}
