// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class GetMarkupExtensionAtPositionTests
	{
		[Test]
		public void EmptyTest()
		{
			string markup = "{x:Type}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 8
			};
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 5, info);
		}
		
		[Test]
		public void SimpleTest()
		{
			string markup = "{x:Type Button}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 15
			};
			
			info.PositionalArguments.Add(new AttributeValue("Button") { StartOffset = 8 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 10, info);
		}
		
		[Test]
		public void SimplePosArgTest()
		{
			string markup = "{x:Type Button, Button2}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 24
			};
			
			info.PositionalArguments.Add(new AttributeValue("Button") { StartOffset = 8 });
			info.PositionalArguments.Add(new AttributeValue("Button2") { StartOffset = 16 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 23, info);
		}
		
		[Test]
		public void SimpleNamedArgTest()
		{
			string markup = "{x:Type Type=Button}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 20
			};
			
			info.NamedArguments.Add("Type", new AttributeValue("Button") { StartOffset = 13 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 0, info);
		}

		[Test]
		public void SimpleNamedArgTest1()
		{
			string markup = "{x:Type Type=Button, a=b}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 25
			};
			
			info.NamedArguments.Add("Type", new AttributeValue("Button") { StartOffset = 13 });
			info.NamedArguments.Add("a", new AttributeValue("b") { StartOffset = 23 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 20, info);
		}
		
		[Test]
		public void PosAndNamedArgTest()
		{
			string markup = "{x:Type xy, Type=Button, a=b}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 29
			};
			
			info.PositionalArguments.Add(new AttributeValue("xy") { StartOffset = 8 });
			
			info.NamedArguments.Add("Type", new AttributeValue("Button") { StartOffset = 17 });
			info.NamedArguments.Add("a", new AttributeValue("b") { StartOffset = 27 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 29, info);
		}
		
		[Test]
		public void MissingColonTest()
		{
			string markup = "{x:Type xy Type=Button, a=b}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 28
			};
			
			info.PositionalArguments.Add(new AttributeValue("xy") { StartOffset = 8 });
			
			info.NamedArguments.Add("Type", new AttributeValue("Button") { StartOffset = 16 });
			info.NamedArguments.Add("a", new AttributeValue("b") { StartOffset = 26 });
			
			RunParseTest(markup, info);
		}
		
		[Test]
		public void MissingColonTest2()
		{
			string markup = "{x:Type xy Type=Button a=b}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "x:Type",
				StartOffset = 0,
				EndOffset = 27
			};
			
			info.PositionalArguments.Add(new AttributeValue("xy") { StartOffset = 8 });
			
			info.NamedArguments.Add("Type", new AttributeValue("Button") { StartOffset = 16 });
			info.NamedArguments.Add("a", new AttributeValue("b") { StartOffset = 25 });
			
			RunParseTest(markup, info);
		}
		
		[Test]
		public void NestedTest1()
		{
			string markup = "{bla {bla2}}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "bla",
				StartOffset = 0,
				EndOffset = 12
			};
			
			info.PositionalArguments.Add(new AttributeValue(new MarkupExtensionInfo() { ExtensionType = "bla2", StartOffset = 5, EndOffset = 11 }) { StartOffset = 5 });
			
			RunParseTest(markup, info);
		}
		
		[Test]
		public void NestedTest2()
		{
			string markup = "{bla a, {bla2}}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "bla",
				StartOffset = 0,
				EndOffset = 15
			};
			
			info.PositionalArguments.Add(new AttributeValue("a") { StartOffset = 5 });
			info.PositionalArguments.Add(
				new AttributeValue(
					new MarkupExtensionInfo() {
						ExtensionType = "bla2",
						StartOffset = 8,
						EndOffset = 14
					}
				) {
					StartOffset = 8
				}
			);
			
			RunParseTest(markup, info);
		}
		
		[Test]
		public void NestedTest3()
		{
			string markup = "{bla a, {bla2}, b, {bla3 {a}, b}}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "bla",
				StartOffset = 0,
				EndOffset = 33
			};
			
			info.PositionalArguments.Add(new AttributeValue("a") { StartOffset = 5 });
			info.PositionalArguments.Add(
				new AttributeValue(
					new MarkupExtensionInfo() {
						ExtensionType = "bla2",
						StartOffset = 8,
						EndOffset = 14
					}
				) {
					StartOffset = 8
				}
			);
			info.PositionalArguments.Add(new AttributeValue("b") { StartOffset = 16 });
			
			var nested2 = 	new MarkupExtensionInfo() {
				ExtensionType = "bla3",
				StartOffset = 19,
				EndOffset = 32
			};
			
			nested2.PositionalArguments.Add(
				new AttributeValue(
					new MarkupExtensionInfo() {
						ExtensionType = "a",
						StartOffset = 25,
						EndOffset = 28
					}
				) {
					StartOffset = 25
				}
			);
			
			nested2.PositionalArguments.Add(new AttributeValue("b") { StartOffset = 30 });
			
			info.PositionalArguments.Add(
				new AttributeValue(nested2) {
					StartOffset = 19
				}
			);
			
			RunParseTest(markup, info);
		}
		
		[Test]
		public void NestedTest4()
		{
			string markup = "{bla a, x={bla3 {a}, b={c}, d=e}, y=z}";
			
			MarkupExtensionInfo info = new MarkupExtensionInfo() {
				ExtensionType = "bla",
				StartOffset = 0,
				EndOffset = 38
			};
			
			info.PositionalArguments.Add(new AttributeValue("a") { StartOffset = 5 });
			
			//x={bla3 {a}, b={c}, d=e}
			
			MarkupExtensionInfo inner = new MarkupExtensionInfo() {
				ExtensionType = "bla3",
				StartOffset = 10,
				EndOffset = 32
			};
			
			MarkupExtensionInfo smallInner = new MarkupExtensionInfo() {
				ExtensionType = "a",
				StartOffset = 16,
				EndOffset = 19
			};
			
			inner.PositionalArguments.Add(new AttributeValue(smallInner) { StartOffset = 16 });
			
			MarkupExtensionInfo smallInner2 = new MarkupExtensionInfo() {
				ExtensionType = "c",
				StartOffset = 23,
				EndOffset = 26
			};
			
			inner.NamedArguments.Add("b", new AttributeValue(smallInner2) { StartOffset = 23 });
			
			inner.NamedArguments.Add("d", new AttributeValue("e") { StartOffset = 30 });
			
			info.NamedArguments.Add("x", new AttributeValue(inner) { StartOffset = 10 });
			
			info.NamedArguments.Add("y", new AttributeValue("z") { StartOffset = 36 });
			
			RunParseTest(markup, info);
			RunDetectionTest(markup, 9, info);
			RunDetectionTest(markup, 11, inner);
			RunDetectionTest(markup, 17, smallInner);
			RunDetectionTest(markup, 19, inner);
			RunDetectionTest(markup, 32, info);
		}
		
		static void RunDetectionTest(string markup, int offset, MarkupExtensionInfo expectedResult)
		{
			MarkupExtensionInfo data = MarkupExtensionParser.Parse(markup);
			MarkupExtensionInfo result = Utils.GetMarkupExtensionAtPosition(data, offset);
			
			Assert.AreEqual(expectedResult, result);
		}
		
		static void RunParseTest(string markup, MarkupExtensionInfo expectedResult)
		{
			MarkupExtensionInfo data = MarkupExtensionParser.Parse(markup);
			
			Assert.AreEqual(expectedResult, data);
		}
	}
}
