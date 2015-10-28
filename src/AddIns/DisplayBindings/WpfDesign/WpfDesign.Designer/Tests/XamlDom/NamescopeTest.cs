using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.UIExtensions;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class NamescopeTest : TestHelper
	{
		/// <summary>
		/// NamescopeTest 1
		/// </summary>
		[Test]
		public void NamescopeTest1()
		{
			var xaml= @"
<UserControl
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    x:Name=""root""
    >
    <Grid x:Name=""rootGrid"" >
        <Button x:Name=""aa"" />
        <Button x:Name=""bb"" />
<t:ExampleControl Property1=""{x:Reference aa}"" />
    </Grid>
</UserControl>";

			var obj = XamlParser.Parse(new StringReader(xaml));

			((FrameworkElement)obj.RootInstance).CreateVisualTree();

			var example = ((FrameworkElement) obj.RootInstance).TryFindChild<ExampleControl>();
			var buttonAa = ((FrameworkElement)obj.RootInstance).TryFindChild<Button>("aa");

			Assert.AreEqual(example.Property1, buttonAa);
		}

		/// <summary>
		/// NamescopeTest 2
		/// </summary>
		[Test]
		public void NamescopeTest2()
		{
			var xaml = @"
<UserControl
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    x:Name=""root""
    >
    <Grid x:Name=""grid"" >
        <Button Content=""level0"" x:Name=""aa"" />
        <t:NamscopeTestUsercontrol />
        <Button Content=""level0"" x:Name=""bb"" />
        <t:ExampleControl Property1=""{x:Reference aa}"" />
        <t:ExampleControl x:Name=""exampleb"" Property1=""{x:Reference bb}"" />
    </Grid>
</UserControl>";

			object officialResult = XamlReader.Load(new XmlTextReader(new StringReader(xaml)));
			((FrameworkElement)officialResult).CreateVisualTree();
			var example1 = ((FrameworkElement)officialResult).TryFindChild<ExampleControl>();
			var exampleb1 = ((FrameworkElement)officialResult).TryFindChild<ExampleControl>("exampleb");
			var buttonAa1 = ((FrameworkElement)officialResult).TryFindChild<Button>("aa");
			var buttonbb1 = ((FrameworkElement)officialResult).TryFindChild<Button>("bb");
			Assert.AreEqual(example1.Property1, buttonAa1);
			Assert.AreNotEqual(exampleb1.Property1, buttonbb1);


			var obj = XamlParser.Parse(new StringReader(xaml));
			((FrameworkElement)obj.RootInstance).CreateVisualTree();
			var example2 = ((FrameworkElement)obj.RootInstance).TryFindChild<ExampleControl>();
			var exampleb2 = ((FrameworkElement)obj.RootInstance).TryFindChild<ExampleControl>("exampleb");
			var buttonAa2 = ((FrameworkElement)obj.RootInstance).TryFindChild<Button>("aa");
			var buttonbb2 = ((FrameworkElement)obj.RootInstance).TryFindChild<Button>("bb");
			Assert.AreEqual(example2.Property1, buttonAa2);
			Assert.AreNotEqual(exampleb2.Property1, buttonbb2);
		}

		/// <summary>
		/// NamescopeTest 3
		/// </summary>
		[Test]
		public void NamescopeTest3()
		{
			var xaml = @"
<Grid
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    x:Name=""root""
    >
        <Button Content=""level0"" x:Name=""aa"" />
        <t:NamscopeTestUsercontrol />
        <Button Content=""level0"" x:Name=""bb"" />
        <t:ExampleControl Property1=""{x:Reference aa}"" />
        <t:ExampleControl x:Name=""exampleb"" Property1=""{x:Reference bb}"" />
</Grid>";

			object officialResult = XamlReader.Load(new XmlTextReader(new StringReader(xaml)));
			((FrameworkElement)officialResult).CreateVisualTree();
			var example1 = ((FrameworkElement)officialResult).TryFindChild<ExampleControl>();
			var exampleb1 = ((FrameworkElement)officialResult).TryFindChild<ExampleControl>("exampleb");
			var buttonAa1 = ((FrameworkElement)officialResult).TryFindChild<Button>("aa");
			var buttonbb1 = ((FrameworkElement)officialResult).TryFindChild<Button>("bb");
			Assert.AreEqual(example1.Property1, buttonAa1);
			Assert.AreNotEqual(exampleb1.Property1, buttonbb1);


			var obj = XamlParser.Parse(new StringReader(xaml));
			((FrameworkElement)obj.RootInstance).CreateVisualTree();
			var example2 = ((FrameworkElement)obj.RootInstance).TryFindChild<ExampleControl>();
			var exampleb2 = ((FrameworkElement)obj.RootInstance).TryFindChild<ExampleControl>("exampleb");
			var buttonAa2 = ((FrameworkElement)obj.RootInstance).TryFindChild<Button>("aa");
			var buttonbb2 = ((FrameworkElement)obj.RootInstance).TryFindChild<Button>("bb");
			Assert.AreEqual(example2.Property1, buttonAa2);
			Assert.AreNotEqual(exampleb2.Property1, buttonbb2);
		}
	}
}
