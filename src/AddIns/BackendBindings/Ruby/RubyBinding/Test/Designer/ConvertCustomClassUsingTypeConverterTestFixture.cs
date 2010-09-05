// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Converts a custom class that has a custom TypeConverter defined.
	/// This type converter implements an InstanceDescriptor which is used to generate the 
	/// code to create an instance of the class.
	/// </summary>
	[TestFixture]
	public class ConvertCustomClassUsingTypeConverterTestFixture
	{		
		[Test]
		public void ConvertCustomClass()
		{
			CustomClass customClass = new CustomClass("Test", "Category");
			string text = RubyPropertyValueAssignment.ToString(customClass);
			string expectedText = "ICSharpCode::Scripting::Tests::Utils::CustomClass.new(\"Test\", \"Category\")";
			Assert.AreEqual(expectedText, text);
		}
	}
}
