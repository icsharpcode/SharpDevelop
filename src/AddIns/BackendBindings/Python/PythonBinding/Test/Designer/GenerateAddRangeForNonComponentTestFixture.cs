// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	class NonComponentToolbar
	{
		NonComponentToolbarToolsCollection tools = new NonComponentToolbarToolsCollection();
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NonComponentToolbarToolsCollection Tools { 
			get { return tools; }
		}
	}
	
	class NonComponentToolbarToolsCollection : CollectionBase
	{
		public void Add(string s)
		{
			InnerList.Add(s);
		}
		
		public void AddRange(string[] strings)
		{
			InnerList.AddRange(strings);
		}
	}
	
	/// <summary>
	/// Tests that we can generate python code for a property which is not an IComponent and one of 
	/// its properties is a collection that needs its content generated as code.
	/// 
	/// class Toolbar : IDisposable
	/// {
	/// 	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    /// 	public ToolsCollection Tools { get; }
	/// }
	/// 
	/// class ToolsCollection : CollectionBase
	/// {
	/// }
	/// </summary>
	[TestFixture]
	public class GenerateAddRangeForNonComponentTestFixture
	{
		[Test]
		public void GenerateAddRangeCode()
		{
			NonComponentToolbar toolbar = new NonComponentToolbar();
			toolbar.Tools.Add("One");
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(toolbar).Find("Tools", true);
			PythonCodeBuilder codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = "    ";
			PythonDesignerComponent.AppendMethodCallWithArrayParameter(codeBuilder, "self.ReportViewer.Toolbar", toolbar, propertyDescriptor, false);
		
			string expectedCode = "self.ReportViewer.Toolbar.Tools.AddRange(System.Array[System.String](\r\n" +
				"    [\"One\"]))\r\n";
			
			Assert.AreEqual(expectedCode, codeBuilder.ToString(), codeBuilder.ToString());
		}
	}
}
