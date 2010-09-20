// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	[Ignore("Broken on .NET 4")]
	public class WhitespaceTests : TestHelper
	{
		[Test]
		public void TrimSurroundingWhitespace()
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
		public void TrimConsecutiveWhitespace()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test               string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void ConvertLineFeedToSpace()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
   a test
   string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void PreserveSurroundingWhitespace()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xml:space=""preserve"">
      
   a test string
     
</t:ExampleClass>
			");
		}
		
		[Test]
		public void PreserveConsecutiveWhitespace()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xml:space=""preserve"">
   a test               string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void PreserveLineFeed()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xml:space=""preserve"">
   a test
   string
</t:ExampleClass>
			");
		}
		
		[Test]
		public void CDataTest()
		{
			TestLoading(@"
		          <t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xml:space=""preserve""> <![CDATA[
	This is text inside the CData section.
This is another line of text.
	This one is indented again.

And that was an empty line.
		]]>
 </t:ExampleClass>");
		}
		
		[Test]
		public void CDataAndContentMixTest()
		{
			TestLoading(@"
		          <t:ExampleClass
  xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
  xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
	some text
	<![CDATA[ text in <CDATA> ]]>
	more text
 </t:ExampleClass>");
		}
	}
}
