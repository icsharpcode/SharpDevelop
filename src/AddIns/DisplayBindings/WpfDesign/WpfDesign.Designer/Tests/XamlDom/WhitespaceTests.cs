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
