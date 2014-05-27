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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class MarkupExtensionModelTests : ModelTestHelper
	{
		private const string PathWithCommasAndSpaces = "C:\\Folder A\\Sub,Folder,A\\SubFolderB\\file,with,commas and spaces.txt";
		private const string Simple = "AbcDef";
		
		[Test]
		public void ElementMarkupExtensionWithSimpleString()
		{
			TestMarkupExtensionPrinter(Simple, true);
		}
		
		[Test]
		public void ShorthandMarkupExtensionWithSimpleString()
		{
			TestMarkupExtensionPrinter(Simple, false);
		}
		
		[Test]
		public void ElementMarkupExtensionWithFilePathString()
		{
			TestMarkupExtensionPrinter(PathWithCommasAndSpaces, true);
		}
		
		[Test]
		public void ShorthandMarkupExtensionWithFilePathString()
		{
			TestMarkupExtensionPrinter(PathWithCommasAndSpaces, false);
		}
		
		private void TestMarkupExtensionPrinter(string s, bool useElementStyle)
		{
			var checkBoxItem = CreateCanvasContext("<CheckBox/>");
			var tagProp = checkBoxItem.Properties["Tag"];
			
			tagProp.SetValue(new DataExtension());
			tagProp.Value.Properties["Data"].SetValue(s);
			
			string expectedXaml;
			
			if (useElementStyle) {
				// Setting this should force element style
				tagProp.Value.Properties["Object"].SetValue(new ExampleClass());
				
				expectedXaml = @"<CheckBox>
  <CheckBox.Tag>
    <t:DataExtension Data=""" + s + @""">
      <t:DataExtension.Object>
        <t:ExampleClass />
      </t:DataExtension.Object>
    </t:DataExtension>
  </CheckBox.Tag>
</CheckBox>";
			} else {
				StringBuilder sb = new StringBuilder("<CheckBox Tag=\"{t:Data Data=");
				
				bool containsSpace = s.Contains(' ');
				
				if(containsSpace) {
					sb.Append('\'');
				}
				
				sb.Append(s.Replace("\\", "\\\\"));
				
				if(containsSpace) {
					sb.Append('\'');
				}
				
				sb.Append("}\" />");
				
				expectedXaml = sb.ToString();
			}
			
			AssertCanvasDesignerOutput(expectedXaml, checkBoxItem.Context);
			AssertLog("");
			
			// The following tests that the official XamlReader is parsing the resulting xaml into the
			// same string that we are testing, regardless if element or shorthand style is being used.
			
			string xaml = expectedXaml.Insert("<CheckBox".Length, " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
			                                  "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
			                                  "xmlns:t=\"" + DesignerTestsNamespace + "\"");
			
			var checkBox = (CheckBox)System.Windows.Markup.XamlReader.Parse(xaml);
			
			Assert.AreEqual(s, (string)checkBox.Tag);
		}
	}
	
	public class DataExtension : MarkupExtension
	{
		public DataExtension()
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Data;
		}
		
		public string Data { get; set; }
		
		public object Object { get; set; }
	}
}
