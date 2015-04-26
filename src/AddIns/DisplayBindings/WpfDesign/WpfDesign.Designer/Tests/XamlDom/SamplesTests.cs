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
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using ICSharpCode.WpfDesign.XamlDom;
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
		public void Complex1()
		{
			TestLoading(@"
<Page
    xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    >
  
 <Page.Resources>
        <Style x:Key=""HeaderStyle"" TargetType=""TextBlock"">
            <Setter Property=""Foreground"" Value=""Gray"" />
            <Setter Property=""FontSize"" Value=""24"" />
        </Style>
    </Page.Resources>
    <StackPanel Margin=""10"">
        <TextBlock>Header 1</TextBlock>
        <TextBlock Style=""{StaticResource HeaderStyle}"">Header 2</TextBlock>
        <TextBlock>Header 3</TextBlock>
    </StackPanel>
</Page>");
		}

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
		public void Resources1()
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

		[Test]
		public void Resources2()
		{
			TestLoading(@"<UserControl
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <UserControl.Resources>
       <ResourceDictionary />
   </UserControl.Resources>
  </UserControl>");
		}

		[Test]
		public void Resources3()
		{
			TestLoading(@"<UserControl
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <FrameworkElement.Resources>
       <ResourceDictionary />
   </FrameworkElement.Resources>
  </UserControl>");
		}

		[Test]
		public void Resources4()
		{
			TestLoading(@"<UserControl
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <UserControl.Resources>
        <ResourceDictionary>
<SolidColorBrush x:Key=""MyBrush"" Color=""Gold""/>
        </ResourceDictionary>
   </UserControl.Resources>
  </UserControl>");
		}

		[Test]
		public void Resources5()
		{
			TestLoading(@"<UserControl
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <UserControl.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/ICSharpCode.WpfDesign.Tests;component/Test.xaml"" />
             </ResourceDictionary.MergedDictionaries >
        </ResourceDictionary>
   </UserControl.Resources>
  </UserControl>");
		}


		[Test]
		public void Animation1()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Button Width=""100"" Height=""100"" Canvas.Left=""8"" Canvas.Top=""8"">
  <Button.Triggers>
    <EventTrigger RoutedEvent=""Button.Loaded"">
      <BeginStoryboard>
        <Storyboard>
          <DoubleAnimation Duration=""0:0:10""
                           From=""1""
                           To=""0""
                           Storyboard.TargetProperty=""Opacity"" />
        </Storyboard>
      </BeginStoryboard>
    </EventTrigger>
  </Button.Triggers>
</Button>
</Canvas>
</Window>");
		}
		
		[Test]
		public void Animation2()
		{
			//Loaded Property has to be found, because this so also works in WPF
			
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Button Width=""100"" Height=""100"" Canvas.Left=""8"" Canvas.Top=""8"">
  <Button.Triggers>
    <EventTrigger RoutedEvent=""Loaded"">
      <BeginStoryboard>
        <Storyboard>
          <DoubleAnimation Duration=""0:0:10""
                           From=""1""
                           To=""0""
                           Storyboard.TargetProperty=""Opacity"" />
        </Storyboard>
      </BeginStoryboard>
    </EventTrigger>
  </Button.Triggers>
</Button>
</Canvas>
</Window>");
		}
		
		[Test]
		public void Animation3()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Button Width=""100"" Height=""100"" Canvas.Left=""8"" Canvas.Top=""8"">
  <Button.Triggers>
    <EventTrigger RoutedEvent=""Button.Loaded"">
      <BeginStoryboard>
      <BeginStoryboard.Storyboard>
        <Storyboard>
          <DoubleAnimation Duration=""0:0:10""
                           From=""1""
                           To=""0""
                           Storyboard.TargetProperty=""Opacity"" />
        </Storyboard>
        </BeginStoryboard.Storyboard>
      </BeginStoryboard>
    </EventTrigger>
  </Button.Triggers>
</Button>
</Canvas>
</Window>");
		}

		[Test]
		public void ContentControl1()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<ContentControl>
  <Grid />
</ContentControl>
</Canvas>
</Window>");
		}

		[Test]
		public void ContentControl2()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<ContentControl>
<ContentControl.Content>
  <Grid />
</ContentControl.Content>
</ContentControl>
</Canvas>
</Window>");
		}

		[Test]
		public void Children1()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Button />
</Canvas>
</Window>");
		}

		[Test]
		public void Children2()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Canvas.Children>
<Button />
</Canvas.Children>
</Canvas>
</Window>");
		}

		[Test]
		public void Children3()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Canvas.Children>
<Button />
</Canvas.Children>
</Canvas>
</Window>");
		}
		
		[Test]
		public void XReferenceTest1()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Canvas>
    <Button x:Name=""aa""
            Content=""Button""
            Width=""75""
            Height=""23""
            Canvas.Left=""205""
            Canvas.Top=""262"" />
    <Button x:Name=""bb""
            Content=""{Binding Path=Content,Source={x:Reference aa}}""
            Width=""75""
            Height=""23""
            Canvas.Left=""79""
            Canvas.Top=""158"" />
  </Canvas>
</Window>");
		}

		[Test]
		public void Style1()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Canvas>
    <Button Content=""Button""
            Width=""75""
            Height=""23"">
        <Button.Style>
        <Style TargetType=""Button"">
            <Setter Property=""Template"">
                <Setter.Value>
                    <ControlTemplate TargetType=""Button"">
                        <Grid />
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        </Button.Style>
    </Button>
   </Canvas>
</Window>");
		}

		[Test]
		public void Style2()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Canvas>
    <Button Content=""Button""
            Width=""75""
            Height=""23"">
        <Button.Style>
        <Style TargetType=""Button"">
            <Setter Property=""Template"">
                <Setter.Value>
                    <ControlTemplate TargetType=""Button"">
                        <Grid HorizontalAlignment=""Left"">
                            <Rectangle />
                            <TextBlock Text=""AA"" />
                        </Grid>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        </Button.Style>
    </Button>
   </Canvas>
</Window>");
		}

		[Test]
		public void Style3()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Canvas>
    <Button Content=""Button""
            Width=""75""
            Height=""23"">
        <Button.Style>
        <Style TargetType=""Button"">
            <Setter Property=""Template"">
                <Setter.Value>
                    <ControlTemplate TargetType=""Button"">
                        <Grid>
                    <Ellipse Fill=""{TemplateBinding Background}""
                             Stroke=""{TemplateBinding BorderBrush}""/>
                        <ContentPresenter HorizontalAlignment=""Center""
                                          VerticalAlignment=""Center""/>
                </Grid>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        </Button.Style>
    </Button>
   </Canvas>
</Window>");
		}
		
		[Test]
		public void Template1()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Canvas>
    <Button Content=""Button""
            Width=""75""
            Height=""23"">
        <Button.Template>
                    <ControlTemplate TargetType=""Button"">
                        <Grid HorizontalAlignment=""Left"">
                            <Rectangle />
                            <TextBlock Text=""AA"" />
                        </Grid>
                    </ControlTemplate>
        </Button.Template>
    </Button>
   </Canvas>
</Window>");
		}

		[Test]
		public void Template2()
		{
			TestLoading(@"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Window.Resources>
<ResourceDictionary>

   <SolidColorBrush Color=""Blue"" x:Key=""bb"" />
                      
                    <ControlTemplate x:Key=""aa"" TargetType=""Button"">
                        <Grid HorizontalAlignment=""Left"">
                            <Rectangle Fill=""{StaticResource bb}"" />
                        </Grid>
                    </ControlTemplate>
        
</ResourceDictionary>
</Window.Resources>
</Window>");
		}

		[Test]
		public void ListBox1()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<ListBox>
aa
bb
</ListBox>
</Canvas>
</Window>");
		}

		[Test]
		//[Ignore("To fix this Test, we need a special Handling for Setter class, because MS Xaml Parser casts the Value of a Setter to the PropertyType wich is defined in another Property!")]
		//Or maybe we need support for XamlSetTypeConverterAttribute, TypeConverterAttribute(typeof(SetterTriggerConditionValueConverter)), ...
		public void ListBox2()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<ListBox ScrollViewer.VerticalScrollBarVisibility=""Hidden"">
    <ListBox.ItemContainerStyle>
        <Style TargetType=""{x:Type ListBoxItem}"">
            <Setter Property=""Width"" Value=""10"" />
        </Style>
    </ListBox.ItemContainerStyle>
aa
bb
</ListBox>
</Canvas>
</Window>");
		}

		[Test]
		public void ListBox3()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<ListBox>
    <ListBox.ItemContainerStyle>
        <Style />
    </ListBox.ItemContainerStyle>
<Image />
</ListBox>
</Canvas>
</Window>");
		}

		[Test]
		public void Window1()
		{
			var xaml= @"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" IsActive=""true"">
</Window>";

			XamlParser.Parse(new StringReader(xaml));
		}

		[Test]
		public void CData1()
		{
			TestLoading(@"<Window
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
<Canvas>
<Button>Press</Button>
<x:Code>
            <![CDATA[

            public void ButtonOnClick(object sender, RoutedEventArgs args)
            {
    Button btn = sender as Button;
    MessageBox.Show(""Button clicked"", ""Hello"");
}
            ]]>
        </x:Code>
 </Canvas>
</Window>");
		}
	}
}
