// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that an event is wired to its event handler after the user specifies an event handler
	/// method in the property grid.
	/// </summary>
	[TestFixture]
	public class GenerateEventHandlerFormTestFixture
	{
		string generatedRubyCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				host.AddService(typeof(IEventBindingService), eventBindingService);
				
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Simulate giving a name to the Load event handler in the property grid.
				EventDescriptorCollection events = TypeDescriptor.GetEvents(form);
				EventDescriptor loadEvent = events.Find("Load", false);
				PropertyDescriptor loadEventProperty = eventBindingService.GetEventProperty(loadEvent);
				loadEventProperty.SetValue(form, "MainFormLoad");
				
				// Add a second event handler method.
				EventDescriptor closedEvent = events.Find("FormClosed", false);
				PropertyDescriptor closedEventProperty = eventBindingService.GetEventProperty(closedEvent);
				closedEventProperty.SetValue(form, "MainFormClosed");

				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode =
				"    self.SuspendLayout()\r\n" +
				"    # \r\n" +
				"    # MainForm\r\n" +
				"    # \r\n" +
				"    self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
				"    self.Name = \"MainForm\"\r\n" +
				"    self.FormClosed { |sender, e| self.MainFormClosed(sender, e) }\r\n" +
				"    self.Load { |sender, e| self.MainFormLoad(sender, e) }\r\n" +
				"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode, generatedRubyCode);
		}
	}
}
