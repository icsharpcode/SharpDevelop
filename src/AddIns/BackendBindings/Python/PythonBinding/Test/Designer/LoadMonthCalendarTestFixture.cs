// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadMonthCalendarTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
						"    def InitializeComponent(self):\r\n" +
						"        self._monthCalendar1 = System.Windows.Forms.MonthCalendar()\r\n" +
						"        self.SuspendLayout()\r\n" +
						"        # \r\n" +
						"        # monthCalendar1\r\n" +
						"        # \r\n" +
						"        self._monthCalendar1.Location = System.Drawing.Point(0, 0)\r\n" +
						"        self._monthCalendar1.MonthlyBoldedDates = System.Array[System.DateTime](\r\n" +
						"            [System.DateTime(2009, 1, 2, 0, 0, 0, 0),\r\n" +
						"            System.DateTime(0)])\r\n" +
						"        self._monthCalendar1.Name = \"monthCalendar1\"\r\n" +
					 	"        self._monthCalendar1.SelectionRange = System.Windows.Forms.SelectionRange(System.DateTime(2009, 8, 4, 0, 0, 0, 0), System.DateTime(2009, 8, 5, 0, 0, 0, 0))\r\n" +
						"        self._monthCalendar1.TabIndex = 0\r\n" +
						"        # \r\n" +
						"        # MainForm\r\n" +
						"        # \r\n" +
						"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
						"        self.Controls.Add(self._monthCalendar1)\r\n" +
						"        self.Name = \"MainForm\"\r\n" +
						"        self.ResumeLayout(False)\r\n" +
						"        self.PerformLayout()\r\n";
			}
		}
		
		public MonthCalendar Calendar {
			get { return Form.Controls[0] as MonthCalendar; }
		}
		
		[Test]
		public void MonthlyBoldedDates()
		{
			DateTime[] expectedDates = new DateTime[] { new DateTime(2009, 1, 2), new DateTime(0) };
			Assert.AreEqual(expectedDates, Calendar.MonthlyBoldedDates);
		}
		
		[Test]
		public void SelectionRange()
		{
			SelectionRange expectedRange = new SelectionRange(new DateTime(2009, 8, 4, 0, 0, 0, 0), new DateTime(2009, 8, 5, 0, 0, 0, 0));
			Assert.AreEqual(expectedRange.ToString(), Calendar.SelectionRange.ToString());
		}
	}
}
