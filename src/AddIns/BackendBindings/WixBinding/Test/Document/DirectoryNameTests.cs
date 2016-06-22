// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Resources;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class DirectoryNameTests
	{
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();
		}

		[Test]
		public void AdminToolsFolder()
		{
			Assert.AreEqual("Admin Tools", WixDirectoryElement.GetSystemDirectory("AdminToolsFolder"));
		}
		
		[Test]
		public void AppDataFolder()
		{
			Assert.AreEqual("Application Data", WixDirectoryElement.GetSystemDirectory("AppDataFolder"));
		}
		
		[Test]
		public void CommonAppDataFolder()
		{
			Assert.AreEqual("Common Application Data", WixDirectoryElement.GetSystemDirectory("CommonAppDataFolder"));
		}
		
		[Test]
		public void CommonFiles64Folder()
		{
			Assert.AreEqual("Common Files 64", WixDirectoryElement.GetSystemDirectory("CommonFiles64Folder"));
		}
		
		[Test]
		public void CommonFilesFolder()
		{
			Assert.AreEqual("Common Files", WixDirectoryElement.GetSystemDirectory("CommonFilesFolder"));
		}
		
		[Test]
		public void DesktopFolder()
		{
			Assert.AreEqual("Desktop", WixDirectoryElement.GetSystemDirectory("DesktopFolder"));
		}
		
		[Test]
		public void FavoritesFolder()
		{
			Assert.AreEqual("Favorites", WixDirectoryElement.GetSystemDirectory("FavoritesFolder"));
		}
		
		[Test]
		public void FontsFolder()
		{
			Assert.AreEqual("Fonts", WixDirectoryElement.GetSystemDirectory("FontsFolder"));
		}
		
		[Test]
		public void LocalAppDataFolder()
		{
			Assert.AreEqual("Local Application Data", WixDirectoryElement.GetSystemDirectory("LocalAppDataFolder"));
		}
		
		[Test]
		public void MyPicturesFolder()
		{
			Assert.AreEqual("My Pictures", WixDirectoryElement.GetSystemDirectory("MyPicturesFolder"));
		}
		
		[Test]
		public void PersonalFolder()
		{
			Assert.AreEqual("Personal", WixDirectoryElement.GetSystemDirectory("PersonalFolder"));
		}
		
		[Test]
		public void ProgramFiles64Folder()
		{
			Assert.AreEqual("Program Files (x64)", WixDirectoryElement.GetSystemDirectory("ProgramFiles64Folder"));
		}
		
		[Test]
		public void ProgramMenuFolder()
		{
			Assert.AreEqual("Program Menu", WixDirectoryElement.GetSystemDirectory("ProgramMenuFolder"));
		}
		
		[Test]
		public void SendToFolder()
		{
			Assert.AreEqual("Send To", WixDirectoryElement.GetSystemDirectory("SendToFolder"));
		}
		
		[Test]
		public void StartMenuFolder()
		{
			Assert.AreEqual("Start Menu", WixDirectoryElement.GetSystemDirectory("StartMenuFolder"));
		}
		
		[Test]
		public void StartupFolder()
		{
			Assert.AreEqual("Startup", WixDirectoryElement.GetSystemDirectory("StartupFolder"));
		}
		
		[Test]
		public void System16Folder()
		{
			Assert.AreEqual("System (x16)", WixDirectoryElement.GetSystemDirectory("System16Folder"));
		}
		
		[Test]
		public void System64Folder()
		{
			Assert.AreEqual("System (x64)", WixDirectoryElement.GetSystemDirectory("System64Folder"));
		}
		
		[Test]
		public void SystemFolder()
		{
			Assert.AreEqual("System", WixDirectoryElement.GetSystemDirectory("SystemFolder"));
		}
		
		[Test]
		public void TempFolder()
		{
			Assert.AreEqual("Temp", WixDirectoryElement.GetSystemDirectory("TempFolder"));
		}
		
		[Test]
		public void TemplateFolder()
		{
			Assert.AreEqual("Templates", WixDirectoryElement.GetSystemDirectory("TemplateFolder"));
		}
		
		[Test]
		public void WindowsFolder()
		{
			Assert.AreEqual("Windows", WixDirectoryElement.GetSystemDirectory("WindowsFolder"));
		}
		
		[Test]
		public void WindowsVolume()
		{
			Assert.AreEqual("Windows Volume", WixDirectoryElement.GetSystemDirectory("WindowsVolume"));
		}
	}
}
