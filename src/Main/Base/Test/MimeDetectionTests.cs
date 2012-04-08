// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Text;
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
			TestMime(MimeTypeDetection.UTF8, "text/plain");
			// UTF-16 Big Endian
			TestMime(MimeTypeDetection.UTF16BE, "text/plain");
			// UTF-16 Little Endian
			TestMime(MimeTypeDetection.UTF16LE, "text/plain");
			// UTF-32 Big Endian
			TestMime(MimeTypeDetection.UTF32BE, "text/plain");
			// UTF-32 Little Endian
			TestMime(MimeTypeDetection.UTF32LE, "text/plain");
		}
		
		[Test]
		public void TextXml()
		{
			string xml = "<?xml version=\"1.0\" ?>";
			TestMime(Encoding.Default.GetBytes(xml), "text/xml");
			TestMime(MimeTypeDetection.UTF8.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(MimeTypeDetection.UTF16BE.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(MimeTypeDetection.UTF16LE.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(MimeTypeDetection.UTF32BE.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(MimeTypeDetection.UTF32LE.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
		}
		
		void TestMime(byte[] bytes, string expectedMime)
		{
			Assert.AreEqual(expectedMime, MimeTypeDetection.FindMimeType(bytes));
		}
	}
}
