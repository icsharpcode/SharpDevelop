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
