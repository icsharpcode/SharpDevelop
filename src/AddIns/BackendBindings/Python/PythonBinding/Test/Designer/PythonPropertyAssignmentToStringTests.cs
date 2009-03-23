// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Globalization;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the PythonPropertyValueAssignment class which generates the Python code for 
	/// the rhs of a property value assignment.
	/// </summary>
	[TestFixture]
	public class PythonPropertyAssignmentToStringTests
	{
		[Test]
		public void ConvertCustomColorToString()
		{
			Color customColor = Color.FromArgb(0, 192, 10);
			Assert.AreEqual("System.Drawing.Color.FromArgb(0, 192, 10)", PythonPropertyValueAssignment.ToString(customColor));
		}
		
		[Test]
		public void FontToString()
		{
			CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
			try {
				System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
				Font font = new Font("Times New Roman", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
				Assert.AreEqual("System.Drawing.Font(\"Times New Roman\", 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0)",
				                PythonPropertyValueAssignment.ToString(font));
			} finally {
				System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
			}
		}
		
		[Test]
		public void SizeFToString()
		{
			SizeF sizeF = new SizeF(4, 10);
			Assert.AreEqual("System.Drawing.SizeF(4, 10)", PythonPropertyValueAssignment.ToString(sizeF));
		}
	}
}
