// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests
{
	[TestFixture]
	public class FileNameTests
	{
		[Test]
		public void TestGetParentDirectory()
		{
			Assert.AreEqual(@"c:\", FileName.Create("c:\\file.txt").GetParentDirectory().ToString());
			Assert.IsNull(DirectoryName.Create("c:\\").GetParentDirectory());
			Assert.AreEqual("relpath", FileName.Create("relpath\\file.txt").GetParentDirectory().ToString());
			Assert.AreEqual(".", FileName.Create("file.txt").GetParentDirectory().ToString());
			Assert.AreEqual("..", FileName.Create("..\\file.txt").GetParentDirectory().ToString());
		}
	}
}
