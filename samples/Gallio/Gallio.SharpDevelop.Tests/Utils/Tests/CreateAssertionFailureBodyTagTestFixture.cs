// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace Gallio.SharpDevelop.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateAssertionFailureBodyTagTestFixture
	{
		StructuredStream structuredStream;
		BodyTag bodyTag;
		MarkerTag assertionFailureMarkerTag;
		SectionTag expectedValueToBeTrueSectionTag;
		TextTag expectedValueToBeTrueTextTag;
		MarkerTag monoSpaceMarkerTag;
		TextTag textTagAfterMonoSpaceMarkerTag;
		MarkerTag stackTraceMarkerTag;
		TextTag stackTraceTextTag;
		MarkerTag codeLocationMarkerTag;
		TextTag codeLocationTextTag;
		
		[SetUp]
		public void Init()
		{
			GallioBodyTagFactory factory = new GallioBodyTagFactory();
			structuredStream = factory.CreateAssertionFailureStructuredStream();
			bodyTag = structuredStream.Body;
			assertionFailureMarkerTag = GetFirstChildMarkerTag(bodyTag);
			expectedValueToBeTrueSectionTag = GetFirstChildSectionTag(assertionFailureMarkerTag);
			expectedValueToBeTrueTextTag = GetFirstChildTextTag(expectedValueToBeTrueSectionTag);
			monoSpaceMarkerTag = GetSecondChildMarkerTag(expectedValueToBeTrueSectionTag);
			textTagAfterMonoSpaceMarkerTag = GetThirdChildTextTag(expectedValueToBeTrueSectionTag);
			stackTraceMarkerTag = GetFourthChildMarkerTag(expectedValueToBeTrueSectionTag);
			stackTraceTextTag = GetFirstChildTextTag(stackTraceMarkerTag);
			codeLocationMarkerTag = GetSecondChildMarkerTag(stackTraceMarkerTag);
			codeLocationTextTag = GetFirstChildTextTag(codeLocationMarkerTag);
		}
		
		MarkerTag GetFirstChildMarkerTag(ContainerTag parentTag)
		{
			return GetFirstChildTag(parentTag) as MarkerTag;
		}
		
		Tag GetFirstChildTag(ContainerTag parentTag)
		{
			return GetChildTagAtPosition(parentTag, 1);
		}
		
		Tag GetChildTagAtPosition(ContainerTag parentTag, int position)
		{
			if (TagHasChildTagAtPosition(parentTag, position)) {
				int index = position - 1;
				return parentTag.Contents[index];
			}
			return null;
		}
		
		bool TagHasChildTagAtPosition(ContainerTag tag, int position)
		{
			if (tag != null) {
				return tag.Contents.Count >= position;
			}
			return false;
		}
		
		SectionTag GetFirstChildSectionTag(ContainerTag parentTag)
		{
			return GetFirstChildTag(parentTag) as SectionTag;
		}
		
		TextTag GetFirstChildTextTag(ContainerTag parentTag)
		{
			return GetFirstChildTag(parentTag) as TextTag;
		}
		
		MarkerTag GetSecondChildMarkerTag(ContainerTag parentTag)
		{
			return GetSecondChildTag(parentTag) as MarkerTag;
		}
		
		Tag GetSecondChildTag(ContainerTag parentTag)
		{
			return GetChildTagAtPosition(parentTag, 2);
		}
		
		TextTag GetThirdChildTextTag(ContainerTag parentTag)
		{
			return GetChildTagAtPosition(parentTag, 3) as TextTag;
		}
		
		MarkerTag GetFourthChildMarkerTag(ContainerTag parentTag)
		{
			return GetChildTagAtPosition(parentTag, 4) as MarkerTag;
		}
		
		[Test]
		public void StructuredStreamNameIsFailures()
		{
			Assert.AreEqual("Failures", structuredStream.Name);
		}
		
		[Test]
		public void BodyTagHasOneChildTag()
		{
			Assert.AreEqual(1, bodyTag.Contents.Count);
		}
		
		[Test]
		public void BodyTagChildIsMarkerTagWithClassSetToAssertionFailure()
		{
			Assert.AreEqual("AssertionFailure", assertionFailureMarkerTag.Class);
		}
		
		[Test]
		public void AssertionFailureMarkerTagHasSectionTagContentsWithTagNameEqualToExpectedValueToBeTrue()
		{
			string expectedName = "Expected value to be true.";
			Assert.AreEqual(expectedName, expectedValueToBeTrueSectionTag.Name);
		}
		
		[Test]
		public void ExpectedValueToBeTrueSectionTagHasTextTagChildWithTextSetToUserAssertionMessage()
		{
			string expectedText = "User assertion message.";
			Assert.AreEqual(expectedText, expectedValueToBeTrueTextTag.Text);
		}
		
		[Test]
		public void ExpectedValueToBeTrueSectionTagHasMarkerTagChildWithClassSetToMonospace()
		{
			Assert.AreEqual("Monospace", monoSpaceMarkerTag.Class);
		}
		
		[Test]
		public void ExpectedValueToBeTrueSectionTagHasTextTagChildAfterMonospaceTagChild()
		{
			Assert.IsNotNull(textTagAfterMonoSpaceMarkerTag);
		}
		
		[Test]
		public void ExpectedValueToBeTrueSectionTagHasMarkerTagWithClassSetToStackTrace()
		{
			Assert.AreEqual("StackTrace", stackTraceMarkerTag.Class);
		}
		
		[Test]
		public void StackTraceTagHasFirstChildTextTagWithFirstLineOfStackTrace()
		{
			string expectedText = "   at GallioTest.MyClass.AssertWithFailureMessage() in ";
			Assert.AreEqual(expectedText, stackTraceTextTag.Text);
		}
		
		[Test]
		public void StackTraceTagHasMarkerTagChildWithClassSetToCodeLocation()
		{
			Assert.AreEqual("CodeLocation", codeLocationMarkerTag.Class);
		}
		
		[Test]
		public void CodeLocationMarkerHasTextTagWithCodeLocationText()
		{
			string expectedText = @"d:\temp\test\GallioTest\MyClass.cs:line 46 ";
			Assert.AreEqual(expectedText, codeLocationTextTag.Text);
		}
	}
}
