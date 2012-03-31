// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class MimeTypeDetectionTests
	{
		[Test]
		public void TextPlain()
		{
			// always open empty files with text editor
			TestMime(new byte[] {}, "text/plain");
			// UTF-8
			TestMime(new byte[] { 0xEF, 0xBB, 0xBF }, "text/plain");
			// UTF-16 Big Endian
			TestMime(new byte[] { 0xFE, 0xFF }, "text/plain");
			// UTF-16 Little Endian
			TestMime(new byte[] { 0xFF, 0xFE }, "text/plain");
			// UTF-32 Big Endian
			TestMime(new byte[] { 0x00, 0x00, 0xFE, 0xFF }, "text/plain");
			// UTF-32 Little Endian
			TestMime(new byte[] { 0xFF, 0xFE, 0x00, 0x00 }, "text/plain");
		}
		
		void TestMime(byte[] bytes, string expectedMime)
		{
			Assert.AreEqual(expectedMime, MimeTypeDetection.FindMimeType(bytes));
		}
	}
}
