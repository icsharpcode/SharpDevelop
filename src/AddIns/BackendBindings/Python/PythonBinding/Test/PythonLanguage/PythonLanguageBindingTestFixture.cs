// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
