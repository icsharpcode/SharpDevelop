// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.PythonLanguage
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
