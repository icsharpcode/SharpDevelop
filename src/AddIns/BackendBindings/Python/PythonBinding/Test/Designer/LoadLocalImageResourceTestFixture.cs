// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadLocalImageResourceTestFixture : LoadFormTestFixtureBase
	{
		MockResourceReader reader;
		Bitmap bitmap;
		
		public override string PythonCode {
			get {
				bitmap = new Bitmap(10, 20);
				reader = new MockResourceReader();
				reader.AddResource("pictureBox1.Image", bitmap);
				ComponentCreator.SetResourceReader(reader);
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        resources = System.Resources.ResourceManager(\"RootNamespace.MainForm\", System.Reflection.Assembly.GetEntryAssembly())\r\n" +			
							"        self._pictureBox1 = System.Windows.Forms.PictureBox()\r\n" +
							"        self._pictureBox1.BeginInit()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # pictureBox1\r\n" +
							"        # \r\n" +
							"        self._pictureBox1.Image = resources.GetObject(\"pictureBox1.Image\")\r\n" +
							"        self._pictureBox1.Location = System.Drawing.Point(0, 0)\r\n" +
							"        self._pictureBox1.Name = \"button1\"\r\n" +
							"        self._pictureBox1.Size = System.Drawing.Size(10, 10)\r\n" +
							"        self._pictureBox1.TabIndex = 0\r\n" +
							"        self._pictureBox1.Text = \"button1\"\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.Controls.Add(self._pictureBox1)\r\n" +
							"        self._pictureBox1.EndInit()\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}

		public PictureBox PictureBox1 {
			get { return base.Form.Controls[0] as PictureBox; }
		}
		
		[Test]
		public void ResourceReaderRetrievedFromComponentCreator()
		{
			Assert.IsTrue(ComponentCreator.GetResourceReaderCalled);
		}
		
		[Test]
		public void CultureInfoInvariantCulturePassedToGetResourceReader()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, ComponentCreator.CultureInfoPassedToGetResourceReader);
		}
		
		[Test]
		public void ResourceReaderIsDisposed()
		{
			Assert.IsTrue(reader.IsDisposed);
		}
		
		[Test]
		public void ComponentResourceManagerCreated()
		{
			CreatedInstance expectedInstance = new CreatedInstance(typeof(ResourceManager), new object[0], "resources", false);
			CreatedInstance instance = base.ComponentCreator.CreatedInstances[0];
			Assert.AreEqual(expectedInstance, instance);
		}
		
		[Test]
		public void BitmapAssignedToPictureBoxRetrievedFromResourceReader()
		{
			Assert.IsTrue(Object.Equals(bitmap, PictureBox1.Image));
		}
		
		[Test]
		public void PictureBoxImageIsNotNull()
		{
			Assert.IsNotNull(PictureBox1.Image);
		}
	}
}
