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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
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
		
		[Test]
		public void AnchorStyleToString()
		{
			AnchorStyles anchorStyle = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			string expectedText = "System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right";
			Assert.AreEqual(expectedText, PythonPropertyValueAssignment.ToString(anchorStyle));
		}
		
		[Test]
		public void AnchorStyleNoneToString()
		{
			AnchorStyles anchorStyle = AnchorStyles.None;
			string expectedText = "System.Windows.Forms.AnchorStyles.None";
			Assert.AreEqual(expectedText, PythonPropertyValueAssignment.ToString(anchorStyle));
		}

		/// <summary>
		/// Nested type will have a full name including a "+", for example, SpecialFolder type has a full name of:
		/// 
		/// System.Environment+SpecialFolder
		/// 
		/// So the PythonPropertyValueAssignment needs to replace the "+" with a ".".
		/// </summary>
		[Test]
		public void SystemEnvironmentSpecialFolderNestedTypeToString()
		{
			Environment.SpecialFolder folder = Environment.SpecialFolder.ProgramFiles;
			string expectedText = "System.Environment.SpecialFolder.ProgramFiles";
			Assert.AreEqual(expectedText, PythonPropertyValueAssignment.ToString(folder));
		}
		
		/// <summary>
		/// Ensures that when the user types in "\t" the code for a string property in the forms designer
		/// the actual string is generated is "\\t".
		/// </summary>
		[Test]
		public void BackslashCharactersEncodedInStrings()
		{
			string text = @"c:\temp";
			string expectedText = "\"c:\\\\temp\"";
			Assert.AreEqual(expectedText, PythonPropertyValueAssignment.ToString(text));
		}
		
		[Test]
		public void DoubleQuoteCharactersEncodedInStrings()
		{
			string text = "c:\\te\"mp";
			string expectedText = "\"c:\\\\te\\\"mp\"";
			Assert.AreEqual(expectedText, PythonPropertyValueAssignment.ToString(text));
		}
		
		[Test]
		public void DefaultCursorToString()
		{
			Assert.AreEqual("System.Windows.Forms.Cursors.Default", PythonPropertyValueAssignment.ToString(Cursors.Default));
		}
		
		[Test]
		public void NullConversion()
		{
			Assert.AreEqual("None", PythonPropertyValueAssignment.ToString(null));
		}
		
		[Test]
		public void CharConversion()
		{
			Assert.AreEqual("\"*\"", PythonPropertyValueAssignment.ToString('*'));
		}
	}
}
