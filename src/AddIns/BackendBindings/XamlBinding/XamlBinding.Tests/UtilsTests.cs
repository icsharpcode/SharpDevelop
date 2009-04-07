// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class UtilsTests
	{
		[Test]
		public void XmlNamespacesForOffsetSimple()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
		
		[Test]
		public void XmlNamespacesForOffsetSimple2()
		{
			string xaml = File.ReadAllText("Test2.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml"},
				{"xmlns:y", "clr-namespace:ICSharpCode.Profiler.Controls;assembly=ICSharpCode.Profiler.Controls"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
		
		[Test]
		public void XmlNamespacesForOffsetComplex()
		{
			string xaml = File.ReadAllText("Test3.xaml");
			int offset = xaml.IndexOf("CheckBox") + "CheckBox ".Length;
			
			var expectedResult = new Dictionary<string, string> {
				{"xmlns", "http://schemas.microsoft.com/netfx/2007/xaml/presentation"},
				{"xmlns:x", "clr-namespace:ICSharpCode.Profiler.Controls;assembly=ICSharpCode.Profiler.Controls"}
			};
			
			var result = Utils.GetXmlNamespacesForOffset(xaml, offset);
			
			foreach (var p in result)
				Debug.Print(p.Key + " " + p.Value);
			
			Assert.AreEqual(expectedResult, result, "Is not equal");
		}
	}
}
