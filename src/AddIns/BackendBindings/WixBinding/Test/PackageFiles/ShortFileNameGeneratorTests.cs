// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Tests the generation of 8.3 short filenames.
	/// </summary>
	[TestFixture]
	public class ShortFileNameGeneratorTests
	{
		int returnFileNameExistsCount = 0;
		
		[Test]
		public void ValidShortName()
		{
			string name = "01234567.ABC";
			Assert.AreEqual(name, ShortFileName.Convert(name));
		}
		
		[Test]
		public void LowerCaseShortName()
		{
			string name = "abcdefgh.abc";
			Assert.AreEqual(name.ToUpperInvariant(), ShortFileName.Convert(name));
		}
		
		[Test]
		public void NullFileName()
		{
			Assert.AreEqual(String.Empty, ShortFileName.Convert(null));
		}
	
		[Test]
		public void InvalidCharacters()
		{
			string name = "abcdefgh [];=,.txt";
			Assert.AreEqual("ABCDEFGH.TXT", ShortFileName.Convert(name));
		}
		
		[Test]
		public void PathRemoved()
		{
			string name = @"C:\Temp\temp.txt";
			Assert.AreEqual("TEMP.TXT", ShortFileName.Convert(name));
		}
		
		[Test]
		public void FileNameTooLong()
		{
			string name = "abcdefgh0.txt";
			Assert.AreEqual("ABCDEF~1.TXT", ShortFileName.Convert(name));
		}
		
		[Test]
		public void ExtensionTooLong()
		{
			string name = "abcdefgh.txt1";
			Assert.AreEqual("ABCDEFGH.TXT", ShortFileName.Convert(name));
		}
		
		[Test]
		public void FirstFileNameAlreadyExists()
		{
			string name = "abcdefghij.txt";
			returnFileNameExistsCount = 1;
			Assert.AreEqual("ABCDEF~2.TXT", ShortFileName.Convert(name, GetFileNameExists));
		}
		
		[Test]
		public void FileNameAlreadyExistsCountToDoubleFigures()
		{
			string name = "abcdefghij.txt";
			returnFileNameExistsCount = 9;
			Assert.AreEqual("ABCDE~10.TXT", ShortFileName.Convert(name, GetFileNameExists));
		}
		
		[Test]
		public void FileNameAlreadyExistsCountToTripleFigures()
		{
			string name = "abcdefghij.txt";
			returnFileNameExistsCount = 99;
			Assert.AreEqual("ABCD~100.TXT", ShortFileName.Convert(name, GetFileNameExists));
		}

		
		bool GetFileNameExists(string fileName)
		{
			if (returnFileNameExistsCount == 0) {
				return false;
			}
			--returnFileNameExistsCount;
			return true;
		}
		
	}
}
