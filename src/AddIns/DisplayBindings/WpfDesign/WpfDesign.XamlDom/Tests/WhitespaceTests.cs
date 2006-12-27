// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using NUnit.Framework;
using System;

namespace ICSharpCode.WpfDesign.XamlDom.Tests
{
	[TestFixture]
	public class WhitespaceTests : TestHelper
	{
		[Test]
		public void TrimSurroundingWhitespace()
		{
			TestLoading(@"
<t:ExampleClass
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
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
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
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
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
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
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
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
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
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
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:t=""clr-namespace:ICSharpCode.WpfDesign.XamlDom.Tests;assembly=ICSharpCode.WpfDesign.XamlDom.Tests""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xml:space=""preserve"">
   a test
   string
</t:ExampleClass>
			");
		}
	}
}
