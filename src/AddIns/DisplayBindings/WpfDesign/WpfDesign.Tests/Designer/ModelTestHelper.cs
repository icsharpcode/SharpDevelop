// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2626 $</version>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Tests.Xaml;
using ICSharpCode.WpfDesign.Designer.XamlBackend;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	/// <summary>
	/// Base class for model tests.
	/// </summary>
	public class ModelTestHelper
	{
		protected StringBuilder log;
		
		//protected XamlDesignContext CreateContext(string xaml)
		//{
		//    log = new StringBuilder();
		//    XamlDesignContext context = new XamlDesignContext(new XmlTextReader(new StringReader(xaml)), new XamlLoadSettings());
		//    /*context.Services.Component.ComponentRegistered += delegate(object sender, DesignItemEventArgs e) {
		//        log.AppendLine("Register " + ItemIdentity(e.Item));
		//    };
		//    context.Services.Component.ComponentUnregistered += delegate(object sender, DesignItemEventArgs e) {
		//        log.AppendLine("Unregister " + ItemIdentity(e.Item));
		//    };*/
		//    return context;
		//}
		
		protected DesignItem CreateCanvasContext(string xaml)
		{
			var doc = TestHelper.TestXamlProject.ParseDocument(@"<Canvas
				xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">" + 
				xaml + "</Canvas>");

			var context = new XamlDesignContext(doc);
			var canvasChild = context.ModelService.Root.Content.CollectionElements[0];	
			log = new StringBuilder();
			return canvasChild;
		}
		
		protected void AssertCanvasDesignerOutput(string expectedXaml, DesignContext context)
		{
			var doc1 = XDocument.Parse(@"<Canvas
				xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
				xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">" + 
				expectedXaml + "</Canvas>");

			var doc2 = XDocument.Parse(context.Save());
			Assert.IsTrue(XNode.DeepEquals(doc1, doc2));
		}
		
		//static string ItemIdentity(DesignItem item)
		//{
		//    return item.ComponentType.Name + " (" + item.GetHashCode() + ")";
		//}
		
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
	}
}

