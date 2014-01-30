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
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Xaml;
using Rhino.Mocks;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	/// <summary>
	/// Base class for model tests.
	/// </summary>
	public class ModelTestHelper
	{
		public const string DesignerTestsNamespace = "clr-namespace:ICSharpCode.WpfDesign.Tests.Designer;assembly=ICSharpCode.WpfDesign.Tests";
		
		protected StringBuilder log;
		
		protected XamlDesignContext CreateContext(string xaml)
		{
			log = new StringBuilder();
			XamlDesignContext context = new XamlDesignContext(new XmlTextReader(new StringReader(xaml)), CreateXamlLoadSettings());
			/*context.Services.Component.ComponentRegistered += delegate(object sender, DesignItemEventArgs e) {
				log.AppendLine("Register " + ItemIdentity(e.Item));
			};
			context.Services.Component.ComponentUnregistered += delegate(object sender, DesignItemEventArgs e) {
				log.AppendLine("Unregister " + ItemIdentity(e.Item));
			};*/
			
			// create required service mocks
			var designPanel = MockRepository.GenerateStub<IDesignPanel>();
			designPanel.Stub(dp => dp.Adorners).Return(new System.Collections.Generic.List<AdornerPanel>());
			context.Services.AddService(typeof(IDesignPanel), designPanel);
			return context;
		}
		
		protected DesignItem CreateCanvasContext(string xaml)
		{
			XamlDesignContext context = CreateContext(@"<Canvas
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  xmlns:t=""" + DesignerTestsNamespace + @""">
  " + xaml + "</Canvas>");
			Canvas canvas = (Canvas)context.RootItem.Component;
			DesignItem canvasChild = context.Services.Component.GetDesignItem(canvas.Children[0]);
			Assert.IsNotNull(canvasChild);
			
			return canvasChild;
		}
		
		protected void AssertCanvasDesignerOutput(string expectedXaml, DesignContext context, params String[] additionalXmlns)
		{
			string canvasStartTag =
				"<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
				 "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
				 "xmlns:t=\"" + DesignerTestsNamespace + "\"";

			
			foreach(string ns in additionalXmlns) {
				canvasStartTag += " " + ns;
			}

			expectedXaml = canvasStartTag + ">\n" + expectedXaml.Trim();
			
			expectedXaml =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
				expectedXaml.Replace("\r", "").Replace("\n", "\n  ")
				+ "\n</Canvas>";
			
			AssertDesignerOutput(expectedXaml, context);
		}
		
		protected DesignItem CreateGridContext(string xaml)
		{
			XamlDesignContext context = CreateContext(@"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
            " + xaml + "</Grid>");
			var grid = (Grid)context.RootItem.Component;
			var gridChild = context.Services.Component.GetDesignItem(grid.Children[0]);
			return gridChild;
		}
		
		protected DesignItem CreateGridContextWithDesignSurface(string xaml)
		{
			var surface = new DesignSurface();
			var xamlWithGrid=@"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
            " + xaml + "</Grid>";
			surface.LoadDesigner(new XmlTextReader(new StringReader(xamlWithGrid)), CreateXamlLoadSettings());
			Assert.IsNotNull(surface.DesignContext.RootItem);
			return surface.DesignContext.RootItem;
		}
		
		protected void AssertGridDesignerOutput(string expectedXaml, DesignContext context, params String[] additionalXmlns)
		{
			string gridStartTag = "<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";
			
			foreach(string ns in additionalXmlns) {
				gridStartTag += " " + ns;
			}

			expectedXaml = gridStartTag + ">\n" + expectedXaml.Trim();
			
			expectedXaml =
				"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
				expectedXaml.Replace("\r", "").Replace("\n", "\n  ")
				+ "\n</Grid>";
			
			AssertDesignerOutput(expectedXaml, context);
		}
		
		static string ItemIdentity(DesignItem item)
		{
			return item.ComponentType.Name + " (" + item.GetHashCode() + ")";
		}
		
		protected void AssertDesignerOutput(string expectedXaml, DesignContext context)
		{
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			xmlWriter.Formatting = Formatting.Indented;
			context.Save(xmlWriter);
			
			string actualXaml = stringWriter.ToString().Replace("\r", "");;
			if (expectedXaml != actualXaml) {
				Debug.WriteLine("expected xaml:");
				Debug.WriteLine(expectedXaml);
				Debug.WriteLine("actual xaml:");
				Debug.WriteLine(actualXaml);
			}
			Assert.AreEqual(expectedXaml, actualXaml);
		}
		
		protected void AssertLog(string expectedLog)
		{
			expectedLog = expectedLog.Replace("\r", "");
			string actualLog = log.ToString().Replace("\r", "");
			if (expectedLog != actualLog) {
				Debug.WriteLine("expected log:");
				Debug.WriteLine(expectedLog);
				Debug.WriteLine("actual log:");
				Debug.WriteLine(actualLog);
			}
			Assert.AreEqual(expectedLog, actualLog);
		}
		
		protected virtual XamlLoadSettings CreateXamlLoadSettings()
		{
			return new XamlLoadSettings();
		}
	}
}
