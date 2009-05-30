// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2752 $</version>
// </file>

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
			expectedPath.Elements.Add(new QualifiedName("DesignSurface", "clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer", "designer"));
			
			Assert.AreEqual(expectedPath, designSurfacePath);
		}

		[Test]
		public void DesignSurfacePath2()
		{
			XmlElementPath expectedPath = new XmlElementPath();
			expectedPath.Elements.Add(new QualifiedName("DesignSurface", "clr-namespace:ICSharpCode.WpfDesign.Designer;assembly=ICSharpCode.WpfDesign.Designer", "designer"));
			
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
