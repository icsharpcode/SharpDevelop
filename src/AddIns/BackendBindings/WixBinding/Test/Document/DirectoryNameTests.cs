// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Resources;

using ICSharpCode.Core;
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
