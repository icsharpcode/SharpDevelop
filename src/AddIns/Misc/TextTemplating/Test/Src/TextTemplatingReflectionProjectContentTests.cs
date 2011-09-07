// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingReflectionProjectContentTests
	{
		[Test]
		public void AssemblyLocation_NullProjectContentPassedToConstructor_ReturnsNull()
		{
			var projectContent = new TextTemplatingReflectionProjectContent(null);
			string location = projectContent.AssemblyLocation;
			
			Assert.IsNull(location);
		}
	}
}
