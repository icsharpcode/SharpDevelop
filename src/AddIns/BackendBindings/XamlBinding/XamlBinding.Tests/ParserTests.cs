// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class ParserTests
	{
		[Test]
		public void SimplePropertyTest()
		{
			string text = "Test";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "Test")
			              );
		}
		
		[Test]
		public void SimpleIndexerTest()
		{
			string text = "[key]";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.Indexer, "key")
			              );
		}
		
		[Test]
		public void TwoPartsTest()
		{
			string text = "propertyName.propertyName2";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName2")
			              );
		}
		
		[Test]
		public void AttachedPropertyTest()
		{
			string text = "(ownerType.propertyName)";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "ownerType.propertyName")
			              );
		}
		
		[Test]
		public void SourceTraversalTest()
		{
			string text = "propertyName/propertyNameX";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.SourceTraversal, "propertyName/propertyNameX")
			              );
		}
		
		[Test]
		public void MultipleIndexerTest()
		{
			string text = "[index1,index2]";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.Indexer, "index1,index2")
			              );
		}
		
		[Test]
		public void MultipleIndexerTest4()
		{
			string text = "[ index1 , index2 ]";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.Indexer, "index1,index2")
			              );
		}
		
		[Test]
		public void MultipleIndexerTest2()
		{
			string text = "propertyName[index1,index2]";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName"),
			               new PropertyPathSegment(SegmentKind.Indexer, "index1,index2")
			              );
		}
		
		[Test]
		public void MultipleIndexerTest3()
		{
			string text = "propertyName[index1,      index2]";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName"),
			               new PropertyPathSegment(SegmentKind.Indexer, "index1,index2")
			              );
		}
		
		[Test]
		public void ComplexTest()
		{
			string text = "ColorGrid[20,30].SolidColorBrushResult";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "ColorGrid"),
			               new PropertyPathSegment(SegmentKind.Indexer, "20,30"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "SolidColorBrushResult")
			              );
		}
		
		[Test]
		public void ComplexTest2()
		{
			string text = "(TextBlock.Background).(SolidColorBrush.Color)";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "TextBlock.Background"),
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "SolidColorBrush.Color")
			              );
		}
		
		[Test]
		public void ComplexTest3()
		{
			string text = "(TextBlock.Background).(SolidColorBrush.Color).X";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "TextBlock.Background"),
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "SolidColorBrush.Color"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "X")
			              );
		}
		
		[Test]
		public void ComplexTest4()
		{
			string text = "(TextBlock.Background).(SolidColorBrush.Color).X/Y";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "TextBlock.Background"),
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "SolidColorBrush.Color"),
			               new PropertyPathSegment(SegmentKind.SourceTraversal, "X/Y")
			              );
		}
		
		
		
		[Test]
		public void ComplexTest5()
		{
			string text = "propertyName.propertyName2[index].propertyName3";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName2"),
			               new PropertyPathSegment(SegmentKind.Indexer, "index"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName3")
			              );
		}
		
		[Test]
		public void ControlChar1()
		{
			string text = "propertyName.propertyName2[index].";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName"),
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "propertyName2"),
			               new PropertyPathSegment(SegmentKind.Indexer, "index"),
			               new PropertyPathSegment(SegmentKind.ControlChar, ".")
			              );
		}
		
		[Test]
		public void ControlChar2()
		{
			string text = "(";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.ControlChar, "(")
			              );
		}
		
		[Test]
		public void ControlChar3()
		{
			string text = "test[";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.PropertyOrType, "test"),
			               new PropertyPathSegment(SegmentKind.ControlChar, "[")
			              );
		}
		
		[Test]
		public void ControlChar4()
		{
			string text = "(testType.";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "(testType"),
			               new PropertyPathSegment(SegmentKind.ControlChar, ".")
			              );
		}
		
		[Test]
		public void ControlChar5()
		{
			string text = "(testType.prop).(someType.";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "testType.prop"),
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "(someType"),
			               new PropertyPathSegment(SegmentKind.ControlChar, ".")
			              );
		}
		
		[Test]
		public void MixedTest1()
		{
			string text = "(testType.prop).(someType.as";
			PropertyPathSegment[] result = PropertyPathParser.Parse(text).ToArray();
			
			CompareResults(result,
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "testType.prop"),
			               new PropertyPathSegment(SegmentKind.AttachedProperty, "(someType.as")
			              );
		}
		
		void CompareResults(PropertyPathSegment[] result, params PropertyPathSegment[] expected)
		{
			Assert.AreEqual(expected, result);
		}
	}
}
