// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Extension
{
	public class SharpDevelopTagFormatter : ITagVisitor
	{
		StringBuilder textBuilder = new StringBuilder();
		string testResultMessage = String.Empty;
		
		public void Visit(TestStepRun testStepRun)
		{
			foreach (StructuredStream stream in testStepRun.TestLog.Streams) {
				VisitBodyTag(stream.Body);
			}
		}
		
		public void VisitBodyTag(BodyTag tag)
		{
			foreach (Tag childTag in tag.Contents) {
				childTag.Accept(this);
			}
		}
		
		public void VisitSectionTag(SectionTag tag)
		{
			textBuilder.Append(tag.Name + " ");
			tag.AcceptContents(this);
		}
		
		public void VisitMarkerTag(MarkerTag tag)
		{
			if (tag.Class == Marker.StackTraceClass) {
				testResultMessage = textBuilder.ToString();
				textBuilder = new StringBuilder();
			}
			tag.AcceptContents(this);
		}
		
		public void VisitEmbedTag(EmbedTag tag)
		{
		}
		
		public void VisitTextTag(TextTag tag)
		{
			textBuilder.Append(tag.Text);
		}
		
		public string TestResultMessage {
			get { return testResultMessage; }
		}
		
		public string GetStackTrace()
		{
			return textBuilder.ToString();
		}
	}
}
