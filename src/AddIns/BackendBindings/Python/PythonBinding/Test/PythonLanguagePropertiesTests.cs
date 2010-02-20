// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests
{
	[TestFixture]
	public class PythonLanguagePropertiesTests
	{
		[Test]
		public void HasCodeDomProvider()
		{
			Assert.IsNotNull(PythonLanguageProperties.Default.CodeDomProvider);
		}
		
		[Test]
		public void AllowObjectConstructionOutsideContextReturnsTrueToEnableMethodInsightForConstructors()
		{
			Assert.IsTrue(PythonLanguageProperties.Default.AllowObjectConstructionOutsideContext);
		}
	}
}
