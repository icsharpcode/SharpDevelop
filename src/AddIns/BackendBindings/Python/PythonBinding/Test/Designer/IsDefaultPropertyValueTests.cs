// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the PythonControlDefaultPropertyValues.IsDefaultValue method.
	/// This checks the property and returns true if the value of the property is the same as its default.
	/// </summary>
	[TestFixture]
	public class IsDefaultPropertyValueTests
	{				
		Form form;
		PythonControlDefaultPropertyValues defaultPropertyValues;
		
		[TestFixtureSetUp]
		public void InitFixture()
		{
			form = new Form();
			defaultPropertyValues = new PythonControlDefaultPropertyValues();
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}
		
		[Test]
		public void TextPropertyDefaultIsEmptyString()
		{
			form.Text = String.Empty;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Text", form));
		}

		[Test]
		public void TextPropertyIsNotEmptyString()
		{
			form.Text = "abc";
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Text", form));
		}
		
		[Test]
		public void AutoValidatePropertyDefaultIsEnablePreventFocusChange()
		{
			form.AutoValidate = AutoValidate.EnablePreventFocusChange;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("AutoValidate", form));
		}

		[Test]
		public void AutoValidatePropertyIsDisable()
		{
			form.AutoValidate = AutoValidate.Disable;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("AutoValidate", form));
		}		
		
		[Test]
		public void EnabledPropertyDefaultIsTrue()
		{
			form.Enabled = true;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Enabled", form));
		}

		[Test]
		public void EnabledPropertyIsFalse()
		{
			form.Enabled = false;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Enabled", form));
		}
		
		[Test]
		public void AccessibleDescriptionDefaultValueIsNull()
		{
			form.AccessibleDescription = null;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("AccessibleDescription", form));
		}
		
		[Test]
		public void AutoScaleModeDefaultIsInherit()
		{
			form.AutoScaleMode = AutoScaleMode.Inherit;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("AutoScaleMode", form));
		}
		
		[Test]
		public void PropertyDoesNotExist()
		{
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("PropertyDoesNotExist", form));
		}
		
		[Test]
		public void DoubleBufferedDefaultIsFalse()
		{
			PropertyInfo property = form.GetType().GetProperty("DoubleBuffered", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			property.SetValue(form, false, null);

			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("DoubleBuffered", form));
		}		
		
		[Test]
		public void CursorDefaultIsCursorsDefault()
		{
			form.Cursor = Cursors.Default;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Cursor", form));
		}
		
		[Test]
		public void HelpCursorIsNotDefaultValue()
		{
			form.Cursor = Cursors.Help;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Cursor", form));
		}				
		
		[Test]
		public void VisiblePropertyDefaultIsTrue()
		{
			PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
			PropertyDescriptor namePropertyDescriptor = descriptors.Find("Visible", false);
			namePropertyDescriptor.SetValue(form, true);

			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Visible", form));
		}

		[Test]
		public void VisiblePropertyIsFalse()
		{
			form.Visible = false;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Visible", form));
		}
		
		[Test]
		public void MinFormSizeDefaultIsEmpty()
		{
			form.MinimumSize = new Size(0, 0);
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("MinimumSize", form));
		}

		[Test]
		public void NonDefaultMinFormSize()
		{
			form.MinimumSize = new Size(100, 100);
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("MinimumSize", form));
		}
		
		[Test]
		public void AutoScrollSizeDefaultIsEmpty()
		{
			form.AutoScrollMinSize = new Size(0, 0);
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("AutoScrollMinSize", form));
		}

		[Test]
		public void NonDefaultAutoScrollMinSize()
		{
			form.AutoScrollMinSize = new Size(100, 100);
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("AutoScrollMinSize", form));
		}
		
		[Test]
		public void AutoScrollMarginDefaultIsEmpty()
		{
			form.AutoScrollMargin = new Size(0, 0);
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("AutoScrollMargin", form));
		}

		[Test]
		public void NonDefaultAutoScrollMargin()
		{
			form.AutoScrollMargin = new Size(100, 100);
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("AutoScrollMargin", form));
		}		
		
		[Test]
		public void LocationDefaultIsEmpty()
		{
			form.Location = new Point(0, 0);
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Location", form));
		}

		[Test]
		public void NonDefaultLocation()
		{
			form.Location = new Point(10, 20);
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Location", form));
		}
		
		[Test]
		public void PaddingPropertyDefaultIsPaddingEmpty()
		{
			form.Padding = Padding.Empty;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Padding", form));
		}

		[Test]
		public void NonDefaultPaddingProperty()
		{
			form.Padding = new Padding(10, 10, 10, 10);
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Padding", form));
		}
		
		[Test]
		public void BackColorPropertyDefaultIsControlDefaultBackColor()
		{
			form.BackColor = Control.DefaultBackColor;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("BackColor", form));
		}

		[Test]
		public void NonDefaultBackColorProperty()
		{
			form.BackColor = Color.Blue;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("BackColor", form));
		}
		
		[Test]
		public void ForeColorPropertyDefaultIsControlDefaultForeColor()
		{
			form.ForeColor = Control.DefaultForeColor;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("ForeColor", form));
		}

		[Test]
		public void NonDefaultForeColorProperty()
		{
			form.ForeColor = Color.Blue;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("ForeColor", form));
		}
		
		[Test]
		public void FontDefaultIsControlDefaultFont()
		{
			form.Font = Control.DefaultFont;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("Font", form));
		}

		[Test]
		public void NonDefaultFont()
		{
			form.Font = new Font("Times New Roman", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("Font", form));
		}
		
		[Test]
		public void TransparencyKeyDefaultIsEmptyColor()
		{
			form.TransparencyKey = Color.Empty;
			Assert.IsTrue(defaultPropertyValues.IsDefaultValue("TransparencyKey", form));
		}

		[Test]
		public void NonDefaultTransparencyKey()
		{
			form.TransparencyKey = Color.White;
			Assert.IsFalse(defaultPropertyValues.IsDefaultValue("TransparencyKey", form));
		}		
	}
}
