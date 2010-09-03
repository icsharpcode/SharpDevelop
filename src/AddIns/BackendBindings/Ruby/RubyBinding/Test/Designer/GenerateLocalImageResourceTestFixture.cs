// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateLocalImageResourceTestFixture
	{
		MockResourceWriter resourceWriter;
		MockComponentCreator componentCreator;
		string generatedRubyCode;
		MockResourceWriter resourceWriter2;
		MockComponentCreator componentCreator2;
		Bitmap bitmap;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resourceWriter = new MockResourceWriter();
			componentCreator = new MockComponentCreator();
			componentCreator.SetResourceWriter(resourceWriter);

			resourceWriter2 = new MockResourceWriter();
			componentCreator2 = new MockComponentCreator();
			componentCreator2.SetResourceWriter(resourceWriter2);
			
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				host.AddService(typeof(IResourceService), componentCreator);
				
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add picture box
				PictureBox pictureBox = (PictureBox)host.CreateComponent(typeof(PictureBox), "pictureBox1");
				pictureBox.Location = new Point(0, 0);
				bitmap = new Bitmap(10, 10);
				pictureBox.Image = bitmap;
				pictureBox.Size = new Size(100, 120);
				pictureBox.TabIndex = 0;
				form.Controls.Add(pictureBox);
				
				// Add bitmap to form.
				form.BackgroundImage = new Bitmap(10, 10);

				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}

				// Check that calling the GenerateInitializeComponentMethodBody also generates a resource file.
				host.RemoveService(typeof(IResourceService));
				host.AddService(typeof(IResourceService), componentCreator2);
				
				serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}			
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    resources = System::Resources::ResourceManager.new(\"MainForm\", System::Reflection::Assembly.GetEntryAssembly())\r\n" +
								"    @pictureBox1 = System::Windows::Forms::PictureBox.new()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # pictureBox1\r\n" +
								"    # \r\n" +
								"    @pictureBox1.Image = resources.GetObject(\"pictureBox1.Image\")\r\n" +
								"    @pictureBox1.Location = System::Drawing::Point.new(0, 0)\r\n" +
								"    @pictureBox1.Name = \"pictureBox1\"\r\n" +
								"    @pictureBox1.Size = System::Drawing::Size.new(100, 120)\r\n" +
								"    @pictureBox1.TabIndex = 0\r\n" +
								"    @pictureBox1.TabStop = false\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.BackgroundImage = resources.GetObject(\"$this.BackgroundImage\")\r\n" +
								"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
								"    self.Controls.Add(@pictureBox1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
		
		[Test]
		public void ResourceWriterRetrievedFromComponentCreator()
		{
			Assert.IsTrue(componentCreator.GetResourceWriterCalled);
		}
		
		[Test]
		public void CultureInfoInvariantCulturePassedToGetResourceWriter()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, componentCreator.CultureInfoPassedToGetResourceWriter);
		}
		
		[Test]
		public void ResourceWriterRetrievedFromComponentCreator2()
		{
			Assert.IsTrue(componentCreator2.GetResourceWriterCalled);
		}
		
		[Test]
		public void CultureInfoInvariantCulturePassedToGetResourceWriter2()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, componentCreator2.CultureInfoPassedToGetResourceWriter);
		}
		
//		[Test]
//		public void ResourceWriter2IsDisposed()
//		{
//			Assert.IsTrue(resourceWriter2.IsDisposed);
//		}
		
		[Test]
		public void BitmapAddedToResourceWriter()
		{
			Assert.IsTrue(Object.ReferenceEquals(bitmap, resourceWriter.GetResource("pictureBox1.Image")));
		}
		
		[Test]
		public void ResourceWriterHasNonNullPictureBox1ImageResource()
		{
			Assert.IsNotNull(resourceWriter.GetResource("pictureBox1.Image"));
		}
	}
}
