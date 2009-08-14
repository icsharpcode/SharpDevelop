// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Represents any markup starting with "&lt;" and (hopefully) ending with ">"
	/// </summary>
	public class RawTag: RawContainer
	{
		/// <summary> These identify the start of DTD elements </summary>
		public static readonly string[] DTDNames = new string[] {"<!DOCTYPE", "<!NOTATION", "<!ELEMENT", "<!ATTLIST", "<!ENTITY"};
		
		/// <summary> Opening bracket - usually "&lt;" </summary>
		public string OpeningBracket { get; internal set; }
		/// <summary> Name following the opening bracket </summary>
		public string Name { get; internal set; }
		/// <summary> Opening bracket - usually "&gt;" </summary>
		public string ClosingBracket { get; internal set; }
		
		/// <summary> True if tag starts with "&lt;" </summary>
		public bool IsStartOrEmptyTag       { get { return OpeningBracket == "<"; } }
		/// <summary> True if tag starts with "&lt;" and ends with "&gt;" </summary>
		public bool IsStartTag              { get { return OpeningBracket == "<" && ClosingBracket == ">"; } }
		/// <summary> True if tag starts with "&lt;" and does not end with "&gt;" </summary>
		public bool IsEmptyTag              { get { return OpeningBracket == "<" && ClosingBracket != ">" ; } }
		/// <summary> True if tag starts with "&lt;/" </summary>
		public bool IsEndTag                { get { return OpeningBracket == "</"; } }
		/// <summary> True if tag starts with "&lt;?" </summary>
		public bool IsProcessingInstruction { get { return OpeningBracket == "<?"; } }
		/// <summary> True if tag starts with "&lt;!--" </summary>
		public bool IsComment               { get { return OpeningBracket == "<!--"; } }
		/// <summary> True if tag starts with "&lt;![CDATA[" </summary>
		public bool IsCData                 { get { return OpeningBracket == "<![CDATA["; } }
		/// <summary> True if tag starts with one of the DTD starts </summary>
		public bool IsDocumentType          { get { return DTDNames.Contains(OpeningBracket); } }
		/// <summary> True if tag starts with "&lt;!" </summary>
		public bool IsUnknownBang           { get { return OpeningBracket == "<!"; } }
		
		internal override void DebugCheckConsistency(bool allowNullParent)
		{
			Assert(OpeningBracket != null, "Null OpeningBracket");
			Assert(Name != null, "Null Name");
			Assert(ClosingBracket != null, "Null ClosingBracket");
			foreach(RawObject child in this.Children) {
				Assert(child is RawText || child is RawAttribute, "Only attribute or text children allowed");
			}
			base.DebugCheckConsistency(allowNullParent);
		}
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitTag(this);
		}
		
		/// <inheritdoc/>
		internal override void UpdateDataFrom(RawObject source)
		{
			base.UpdateDataFrom(source); // Check asserts
			if (this.LastUpdatedFrom == source) return;
			RawTag src = (RawTag)source;
			if (this.OpeningBracket != src.OpeningBracket ||
				this.Name != src.Name ||
				this.ClosingBracket != src.ClosingBracket)
			{
				OnChanging();
				this.OpeningBracket = src.OpeningBracket;
				this.Name = src.Name;
				this.ClosingBracket = src.ClosingBracket;
				OnChanged();
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4}]", base.ToString(), this.OpeningBracket, this.Name, this.ClosingBracket, this.Children.Count);
		}
	}
}
