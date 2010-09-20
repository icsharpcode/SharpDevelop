// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class SamplesTests : TestHelper
	{
		/// <summary>
		/// Non-trivial because of: InlineCollection wrapping a string
		/// </summary>
		[Test]
		public void Intro1()
		{
			TestLoading(@"
<Page
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    >
  <StackPanel
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <TextBlock>
      Hello, World!
    </TextBlock>
  </StackPanel>
</Page>");
		}
		
		/// <summary>
		/// Non-trivial because of: found a bug in Control.Content handling
		/// </summary>
		[Test]
		public void Intro2()
		{
			TestLoading(@"
<!--<SnippetSimpleLayout>-->
<StackPanel
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Button HorizontalAlignment=""Left""
          Width=""100""
          Margin=""10,10,10,10"">Button 1</Button>
  <Button HorizontalAlignment=""Left""
          Width=""100""
          Margin=""10,10,10,10"">Button 2</Button>
  <Button HorizontalAlignment=""Left""
          Width=""100""
          Margin=""10,10,10,10"">Button 3</Button>
</StackPanel>
<!--</SnippetSimpleLayout>-->");
		}
		
		/// <summary>
		/// Non-trivial because of: use of attached properties, units for width+height
		/// </summary>
		[Test]
		public void Intro3()
		{
			TestLoading(@"
<Page
	xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
	xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	<DockPanel
		xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
		xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
		<TextBlock Background=""LightBlue""
		           DockPanel.Dock=""Top"">Some Text</TextBlock>
		<TextBlock DockPanel.Dock=""Bottom""
		           Background=""LightYellow"">Some text at the bottom of the page.</TextBlock>
		<TextBlock DockPanel.Dock=""Left""
		           Background=""Lavender"">Some More Text</TextBlock>
		<DockPanel Background=""Bisque"">
			<StackPanel DockPanel.Dock=""Top"">
				<Button HorizontalAlignment=""Left""
				        Height=""30px""
				        Width=""100px""
				        Margin=""10,10,10,10"">Button1</Button>
				<Button HorizontalAlignment=""Left""
				        Height=""30px""
				        Width=""100px""
				        Margin=""10,10,10,10"">Button2</Button>
			</StackPanel>
			<TextBlock Background=""LightGreen"">Some Text Below the Buttons</TextBlock>
		</DockPanel>
	</DockPanel>
</Page>
");
		}
		
		/// <summary>
		/// Using Hyperlinks
		/// </summary>
		[Test]
		public void Intro4()
		{
			TestLoading(@"
		<Page
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <StackPanel Background=""LightBlue"">
    <TextBlock Margin=""10,10,10,10"">Start Page</TextBlock>
    <TextBlock  HorizontalAlignment=""Left""
                Margin=""10,10,10,10"">
      <Hyperlink  NavigateUri=""Page2.xaml"">Go To Page 2</Hyperlink>
    </TextBlock>
  </StackPanel>
</Page>");
		}
		
		[Test]
		public void Resources()
		{
			TestLoading(@"<Page Name=""root""
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
>
  <Page.Resources>
    <SolidColorBrush x:Key=""MyBrush"" Color=""Gold""/>
  </Page.Resources>
  <StackPanel Background=""{StaticResource MyBrush}"">
  </StackPanel>
</Page>");
		}
	}
}
