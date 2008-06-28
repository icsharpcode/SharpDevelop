// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class FileUtilityTests
	{
		#region NormalizePath
		[Test]
		public void NormalizePath()
		{
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\project\..\test.txt"));
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\project\.\..\test.txt"));
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\\test.txt")); // normalize double backslash
			Assert.AreEqual(@"c:\temp", FileUtility.NormalizePath(@"c:\temp\."));
			Assert.AreEqual(@"c:\temp", FileUtility.NormalizePath(@"c:\temp\subdir\.."));
		}
		
		[Test]
		public void NormalizePath_DriveRoot()
		{
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:\"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/."));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/.."));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/./"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/..\"));
		}
		
		[Test]
		public void NormalizePath_UNC()
		{
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"\\server\share"));
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"\\server\share\"));
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"//server/share/"));
			Assert.AreEqual(@"\\server\share\otherdir", FileUtility.NormalizePath(@"//server/share/dir/..\otherdir"));
		}
		
		[Test]
		public void NormalizePath_Web()
		{
			Assert.AreEqual(@"http://danielgrunwald.de/path/", FileUtility.NormalizePath(@"http://danielgrunwald.de/path/"));
			Assert.AreEqual(@"browser://http://danielgrunwald.de/path/", FileUtility.NormalizePath(@"browser://http://danielgrunwald.de/wrongpath/../path/"));
		}
		#endregion
		
		[Test]
		public void TestCombine()
		{
			Assert.AreEqual(Path.Combine("A", Path.Combine("B", Path.Combine("Long", "Longer.txt"))), FileUtility.Combine("A", "B", "Long", "Longer.txt"));
		}
		
		[Test]
		public void TestIsBaseDirectory()
		{
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A\b\hello"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a\"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a\"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\A", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\x\fWufhweoe", @"C:\a\x\fwuFHweoe\a\b\hello"));
			
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\b\..\A", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\HELLO\..\B\..\a", @"C:\b\..\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\.\B\..\.\.\a", @"C:\.\.\.\.\.\.\.\a"));
			
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\b", @"C:\a\b\hello"));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\a\b\hello", @"C:\b"));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\a\x\fwufhweoe", @"C:\a\x\fwuFHweoex\a\b\hello"));
		}
		
		[Test]
		public void TestGetAbsolutePath()
		{
			Assert.AreEqual(@"C:\a\blub", FileUtility.GetAbsolutePath(@"C:\hello\.\..\a", @".\blub"));
			Assert.AreEqual(@"C:\a\blub", FileUtility.GetAbsolutePath(@"C:\hello\", @"..\a\blub"));
		}
		
		[Test]
		public void TestGetRelativePath()
		{
			Assert.AreEqual(@"blub", FileUtility.GetRelativePath(@"C:\hello\.\..\a", @"C:\.\a\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\hello", @"C:\.\blub\.\..\.\a\.\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\hello\", @"C:\.\blub\.\..\.\a\.\blub"));
			
			// casing troubles
			Assert.AreEqual(@"blub", FileUtility.GetRelativePath(@"C:\hello\.\..\A", @"C:\.\a\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\HELlo", @"C:\.\blub\.\..\.\a\.\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\heLLo\A\..", @"C:\.\blub\.\..\.\a\.\blub"));
			
			// Project filename could be an URL
			Assert.AreEqual("http://example.com/vdir/", FileUtility.GetRelativePath("C:\\temp", "http://example.com/vdir/"));
		}
		
		[Test]
		public void TestIsEqualFile()
		{
			Assert.IsTrue(FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO WOrld.exe"));
			Assert.IsTrue(FileUtility.IsEqualFileName(@"C:\bla\..\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\a\MY.FILE.IS.THIS"));
			
			Assert.IsFalse(FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO_WOrld.exe"));
			Assert.IsFalse(FileUtility.IsEqualFileName(@"C:\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\b\MY.FILE.IS.THIS"));
			
		}
		
		[Test]
		public void TestRenameBaseDirectory()
		{
			Assert.AreEqual(@"C:\x\y\z\c\hello.txt", FileUtility.RenameBaseDirectory(@"C:\a\b\c\hello.txt", @"C:\hello\..\A\.\B\.", @"C:\.\x\y\z\."));
			Assert.AreEqual(@"C:\a\b\c\hello.txt",   FileUtility.RenameBaseDirectory(@"C:\a\b\c\hello.txt", @"C:\hello\..\A\.\B\.\FF", @"C:\.\x\y\z\."));
			Assert.AreEqual(@"C:\A\hello.txt", FileUtility.RenameBaseDirectory(@"C:\B\hello.txt", @"C:\B\", @"C:\A\"));
			Assert.AreEqual(@"C:\A\MyDir", FileUtility.RenameBaseDirectory(@"C:\B\OldDir", @"C:\B\OldDir", @"C:\A\MyDir"));
		}
	}
}
