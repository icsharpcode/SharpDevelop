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
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Parser
{
	/// <summary>
	/// The XmlParser does not find the correct path when the first
	/// element of a namespace is under the current cursor position. It 
	/// incorrectly uses the namespace of the parent element even though
	/// the element itself has its own prefix.
	/// </summary>
	[TestFixture]
	public class XamlMixedNamespaceTestFixture
	{
		string xaml = "<Window\r\n" +
						"	x:Class=\"DefaultNamespace.Window2\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:designer=\"clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer\"\r\n" +
						"	Title=\"DefaultNamespace\"\r\n" +
						"	Height=\"300\"\r\n" +
						"	Width=\"300\">\r\n" +
						"	<Grid>\r\n" +
						"		<TabControl\r\n" +
						"			TabStripPlacement=\"Bottom\"\r\n" +
						"			Name=\"tabControl\"\r\n" +
						"			SelectionChanged=\"tabControlSelectionChanged\"\r\n" +
						"			Grid.Column=\"0\">\r\n" +
						"			<TabItem\r\n" +
						"				Header=\"Design\"\r\n" +
						"				Name=\"designTab\">\r\n" +
						"				<designer:DesignSurface\r\n" +
						"					Name=\"designSurface\"/>\r\n" +
						"			</TabItem>\r\n" +
						"		</TabControl>\r\n" +
						"	</Grid>\r\n" +
						"</Window>";

		XmlElementPath designSurfacePath;
		XmlElementPath designSurfacePath2;
		QualifiedName designSurfaceNameAttribute;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			// Get the path for the DesignSurface element.
			int index = xaml.IndexOf("designer:DesignSurface");
			designSurfacePath = XmlParser.GetActiveElementStartPathAtIndex(xaml, index);
			designSurfacePath2 = XmlParser.GetActiveElementStartPath(xaml, index);
			
			// Get the path for the DesignSurface/@Name attribute.
			index = xaml.IndexOf("Name=\"designSurface\"");
			designSurfaceNameAttribute = XmlParser.GetQualifiedAttributeNameAtIndex(xaml, index, true);
		}
		
		[Test]
		public void DesignSurfacePath()
		{
			XmlElementPath expectedPath = new XmlElementPath();

			string xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
			expectedPath.AddElement(new QualifiedName("Window", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("Grid", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("TabControl", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("TabItem", xamlNamespace));
			
			QualifiedName designSurfaceElement = new QualifiedName("DesignSurface", 
				"clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer", 
				"designer");
			expectedPath.AddElement(designSurfaceElement);
			
			Assert.AreEqual(expectedPath, designSurfacePath);
		}

		[Test]
		public void DesignSurfacePath2()
		{
			XmlElementPath expectedPath = new XmlElementPath();
			
			string xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
			expectedPath.AddElement(new QualifiedName("Window", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("Grid", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("TabControl", xamlNamespace));
			expectedPath.AddElement(new QualifiedName("TabItem", xamlNamespace));
			
			QualifiedName designSurfaceElement = new QualifiedName("DesignSurface", 
				"clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer", 
				"designer");
			expectedPath.AddElement(designSurfaceElement);
			
			Assert.AreEqual(expectedPath, designSurfacePath2);
		}
		
		[Test]
		public void DesignSurfaceNameAttribute()
		{
			QualifiedName expectedName = new QualifiedName("Name", "clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer", String.Empty);
			
			Assert.AreEqual(expectedName, designSurfaceNameAttribute);
		}
	}
}
