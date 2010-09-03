// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.RubyLanguage
{
	[TestFixture]
	public class RubyLanguagePropertiesTests
	{
		[Test]
		public void HasCodeDomProvider()
		{
			Assert.IsNotNull(RubyLanguageProperties.Default.CodeDomProvider);
		}
	}
}
