// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Reflection;

namespace Gallio.SharpDevelop.Tests.Utils
{
	public class GallioBodyTagFactory
	{
		public StructuredStream CreateAssertionFailureStructuredStream()
		{
			StructuredStream stream = new StructuredStream("Failures");
			stream.Body = CreateAssertionFailure();
			return stream;
		}
		
		public BodyTag CreateAssertionFailure()
		{
			BodyTag bodyTag = new BodyTag();
			bodyTag.Contents.Add(CreateAssertionFailureMarkerTag());
			return bodyTag;
		}
		
		MarkerTag CreateAssertionFailureMarkerTag()
		{
			MarkerTag markerTag = new MarkerTag(Marker.AssertionFailure);
			markerTag.Contents.Add(CreateAssertionFailureSectionTag());
			return markerTag;
		}
		
		SectionTag CreateAssertionFailureSectionTag()
		{
			SectionTag sectionTag = new SectionTag("Expected value to be true.");
			sectionTag.Contents.Add(CreateUserAssertionMessageTextTag());
			sectionTag.Contents.Add(CreateMonospaceMarkerTag());
			sectionTag.Contents.Add(CreateEmptyTextTag());
			sectionTag.Contents.Add(CreateStackTraceMarkerTag());
			return sectionTag;
		}
		
		TextTag CreateUserAssertionMessageTextTag()
		{
			return new TextTag("User assertion message.");
		}
		
		MarkerTag CreateMonospaceMarkerTag()
		{
			return new MarkerTag(Marker.Monospace);
		}
		
		TextTag CreateEmptyTextTag()
		{
			return new TextTag(String.Empty);
		}
		
		MarkerTag CreateStackTraceMarkerTag()
		{
			MarkerTag markerTag = new MarkerTag(Marker.StackTrace);
			markerTag.Contents.Add(CreateStackTraceTextTag());
			markerTag.Contents.Add(CreateCodeLocationMarkerTag());
			return markerTag;
		}
		
		TextTag CreateStackTraceTextTag()
		{
			string text = "   at GallioTest.MyClass.AssertWithFailureMessage() in ";
			return new TextTag(text);
		}
		
		MarkerTag CreateCodeLocationMarkerTag()
		{
			CodeLocation location = new CodeLocation();
			Marker marker = Marker.CodeLocation(location);
			MarkerTag markerTag = new MarkerTag(marker);
			markerTag.Contents.Add(CreateCodeLocationTextTag());
			return markerTag;
		}
		
		TextTag CreateCodeLocationTextTag()
		{
			string text = @"d:\temp\test\GallioTest\MyClass.cs:line 46 ";
			return new TextTag(text);
		}
	}
}
