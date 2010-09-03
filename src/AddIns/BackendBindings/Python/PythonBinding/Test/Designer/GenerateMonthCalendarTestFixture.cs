// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateMonthCalendarTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add month calendar.
				MonthCalendar calendar = (MonthCalendar)host.CreateComponent(typeof(MonthCalendar), "monthCalendar1");
				calendar.TabIndex = 0;
				calendar.Location = new Point(0, 0);
				calendar.AddMonthlyBoldedDate(new DateTime(2009, 1, 2));
				calendar.AddMonthlyBoldedDate(new DateTime(0));
				
				form.Controls.Add(calendar);
								
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    self._monthCalendar1 = System.Windows.Forms.MonthCalendar()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # monthCalendar1\r\n" +
								"    # \r\n" +
								"    self._monthCalendar1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._monthCalendar1.MonthlyBoldedDates = System.Array[System.DateTime](\r\n" +
								"        [System.DateTime(2009, 1, 2, 0, 0, 0, 0),\r\n" +
								"        System.DateTime(0)])\r\n" +
								"    self._monthCalendar1.Name = \"monthCalendar1\"\r\n" +
								"    self._monthCalendar1.TabIndex = 0\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._monthCalendar1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
