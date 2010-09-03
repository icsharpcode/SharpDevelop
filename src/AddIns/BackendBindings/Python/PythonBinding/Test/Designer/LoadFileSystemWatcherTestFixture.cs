// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadFileSystemWatcherTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return
					"class MainForm(System.Windows.Forms.Form):\r\n" +
					"    def InitializeComponent(self):\r\n" +
					"        self._fileSystemWatcher1 = System.IO.FileSystemWatcher()\r\n" +
					"        self._fileSystemWatcher1.BeginInit()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # fileSystemWatcher1\r\n" +
					"        # \r\n" +
					"        self._fileSystemWatcher1.EnableRaisingEvents = True\r\n" +
					"        self._fileSystemWatcher1.SynchronizingObject = self\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self._fileSystemWatcher1.EndInit()\r\n" +
					"        self.ResumeLayout(False)\r\n";
			}
		}
		
		public CreatedInstance FileSystemWatcherInstance {
			get { return ComponentCreator.CreatedInstances[0]; }
		}
		
		public FileSystemWatcher FileSystemWatcher {
			get { return FileSystemWatcherInstance.Object as FileSystemWatcher; }
		}
		
		[Test]
		public void ComponentName()
		{
			Assert.AreEqual("fileSystemWatcher1", FileSystemWatcherInstance.Name);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.AreEqual("System.IO.FileSystemWatcher", FileSystemWatcherInstance.InstanceType.FullName);
		}
		
		[Test]
		public void FileSystemWatcherEnableRaisingEventsIsTrue()
		{
			Assert.IsTrue(FileSystemWatcher.EnableRaisingEvents);
		}
		
		[Test]
		public void FileSystemWatcherSynchronisingObjectIsForm()
		{
			Assert.AreSame(Form, FileSystemWatcher.SynchronizingObject);
		}
	}
}
