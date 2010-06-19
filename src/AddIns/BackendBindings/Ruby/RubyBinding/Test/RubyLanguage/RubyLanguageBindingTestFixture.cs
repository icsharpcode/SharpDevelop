// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
