// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadMonthCalendarTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @monthCalendar1 = System::Windows::Forms::MonthCalendar.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # monthCalendar1\r\n" +
					"        # \r\n" +
					"        @monthCalendar1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @monthCalendar1.MonthlyBoldedDates = System::Array[System::DateTime].new(\r\n" +
					"            [System::DateTime.new(2009, 1, 2, 0, 0, 0, 0),\r\n" +
					"            System::DateTime.new(0)])\r\n" +
					"        @monthCalendar1.Name = \"monthCalendar1\"\r\n" +
				 	"        @monthCalendar1.SelectionRange = System::Windows::Forms::SelectionRange.new(System::DateTime.new(2009, 8, 4, 0, 0, 0, 0), System::DateTime.new(2009, 8, 5, 0, 0, 0, 0))\r\n" +
					"        @monthCalendar1.TabIndex = 0\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
					"        self.Controls.Add(@monthCalendar1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
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
