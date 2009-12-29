// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests
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
