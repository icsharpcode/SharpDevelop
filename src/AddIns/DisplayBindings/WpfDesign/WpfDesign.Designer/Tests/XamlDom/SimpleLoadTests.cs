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
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class SimpleLoadTests : TestHelper
	{
		[Test]
		public void Window()
		{
			TestLoading(@"
<Window
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</Window>
			");
		}
		
		[Test]
		public void WindowWithAttributes()
		{
			TestLoading(@"
<Window
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  Width=""300"" Height=""400"">
</Window>
			");
		}
		
		[Test]
		public void WindowWithElementAttribute()
		{
			TestLoading(@"
<Window
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Window.Height>100</Window.Height>
</Window>
			");
		}
		
		[Test]
		public void ExampleClassTest()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassWithStringPropAttribute()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  StringProp=""a test string"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassWithEscapedBraceStringPropAttribute()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  StringProp=""{}{a test string}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassWithFilePathStringPropAttribute()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  StringProp=""C:\Folder A\Sub,Folder,A\SubFolderB\file,with,commas and spaces.txt"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultProperty()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyExplicitly()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.StringProp>
      a test string
   </t:ExampleClass.StringProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyBeforeOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test string
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyAfterOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
   a test string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassUseDefaultPropertyBetweenOtherPropertyElement()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleClass.OtherProp>
      otherValue
   </t:ExampleClass.OtherProp>
   a test string
   <t:ExampleClass.OtherProp2>
      otherValue2
   </t:ExampleClass.OtherProp2>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void Container()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <ExampleClassContainer.List>
      <ExampleClass OtherProp=""a""> </ExampleClass>
      <ExampleClass OtherProp=""b"" />
      <ExampleClass OtherProp=""c"" />
   </ExampleClassContainer.List>
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ContainerImplicitList()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
      <ExampleClass OtherProp=""a"" />
      <ExampleClass OtherProp=""b"" />
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ContainerExplicitList()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <ExampleClassContainer.OtherList>
      <ExampleClassList>
         <ExampleClass OtherProp=""a""> </ExampleClass>
         <ExampleClass OtherProp=""b"" />
         <ExampleClass OtherProp=""c"" />
      </ExampleClassList>
   </ExampleClassContainer.OtherList>
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ContainerImplicitDictionary()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <ExampleClassContainer.Dictionary>
     <ExampleClass x:Key=""key1"" OtherProp=""a""> </ExampleClass>
     <ExampleClass x:Key=""key2"" OtherProp=""b"" />
     <ExampleClass x:Key=""key3"" OtherProp=""c"" />
   </ExampleClassContainer.Dictionary>
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ContainerExplicitDictionary()
		{
			TestLoading(@"
<ExampleClassContainer
  xmlns=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <ExampleClassContainer.Dictionary>
      <ExampleClassDictionary>
         <ExampleClass x:Key=""key1"" OtherProp=""a""> </ExampleClass>
         <ExampleClass x:Key=""key2"" OtherProp=""b"" />
         <ExampleClass x:Key=""key3"" OtherProp=""c"" />
      </ExampleClassDictionary>
   </ExampleClassContainer.Dictionary>
</ExampleClassContainer>
			");
		}
		
		[Test]
		public void ResourceDictionaryImplicit()
		{
			TestLoading(@"
<Window
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Window.Resources>
     <t:ExampleClass x:Key=""key1"" OtherProp=""a""> </t:ExampleClass>
     <t:ExampleClass x:Key=""key2"" OtherProp=""b"" />
  </Window.Resources>
</Window>
			");
		}
		
		[Test]
		public void ResourceDictionaryExplicitWinfx2006()
		{
			ResourceDictionaryExplicitInternal("http://schemas.microsoft.com/winfx/2006/xaml/presentation");
		}
		
		[Test]
		[Ignore("Own XamlParser should handle different namespaces pointing to same types, because builtin XamlReader does.")]
		public void ResourceDictionaryExplicitNetfx2007()
		{
			// The reason this test case fails is because own XamlParser cannot always handle the case where multiple xmlns points to the same type.
			// In this test case the default xmlns is set to netfx/20007 (compare with the test above that uses winfx/2006 and is successfully executed).
			ResourceDictionaryExplicitInternal("http://schemas.microsoft.com/netfx/2007/xaml/presentation");
		}
		
		void ResourceDictionaryExplicitInternal(string defaultXmlns)
		{
			TestLoading(@"
<Window
  xmlns=""" + defaultXmlns + @"""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <Window.Resources>
     <ResourceDictionary>
        <t:ExampleClass x:Key=""key1"" OtherProp=""a""> </t:ExampleClass>
        <t:ExampleClass x:Key=""key2"" OtherProp=""b"" />
     </ResourceDictionary>
  </Window.Resources>
</Window>
			");
		}
		
		[Test]
		public void ExampleServiceTest()
		{
			TestLoading(@"
<t:ExampleDependencyObject
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  t:ExampleService.Example=""attached value"">
</t:ExampleDependencyObject>
			");
		}
		
		[Test]
		public void ExampleServiceCollectionTest()
		{
			TestLoading(@"
<t:ExampleDependencyObject
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   <t:ExampleService.ExampleCollection>
      <t:ExampleClassList>
         <t:ExampleClass OtherProp=""a""> </t:ExampleClass>
         <t:ExampleClass OtherProp=""b"" />
         <t:ExampleClass OtherProp=""c"" />
      </t:ExampleClassList>
   </t:ExampleService.ExampleCollection>
</t:ExampleDependencyObject>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithStringValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""a test string"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithTypeValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Type t:ExampleClass}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithTypeValue2()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Type TypeName=t:ExampleClass}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithNullValue()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  ObjectProp=""{x:Null}"">
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithExplicitMarkupExtension()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <t:ExampleClass.ObjectProp>
    <x:Type TypeName=""t:ExampleClass""/>
  </t:ExampleClass.ObjectProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ExampleClassObjectPropWithExplicitMarkupExtension2()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <t:ExampleClass.ObjectProp>
    <x:TypeExtension TypeName=""t:ExampleClass""/>
  </t:ExampleClass.ObjectProp>
</t:ExampleClass>
			");
		}
		
		[Test]
		public void UsingAttachedPropertyOnDerivedClass()
		{
			TestLoading(@"
<Window
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
  t:DerivedExampleDependencyObject.Example=""test"">
</Window>
			");
		}
	}
}
