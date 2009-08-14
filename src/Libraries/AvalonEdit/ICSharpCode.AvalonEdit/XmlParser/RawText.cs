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
	/// Whitespace or character data
	/// </summary>
	public class RawText: RawObject
	{
		/// <summary> The context in which the text occured </summary>
		public RawTextType Type { get; set; }
		/// <summary> The text exactly as in source </summary>
		public string EscapedValue { get; set; }
		/// <summary> The text with all entity references resloved </summary>
		public string Value { get; set; }
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitText(this);
		}
		
		/// <inheritdoc/>
		internal override void UpdateDataFrom(RawObject source)
		{
			base.UpdateDataFrom(source); // Check asserts
			if (this.LastUpdatedFrom == source) return;
			RawText src = (RawText)source;
			if (this.EscapedValue != src.EscapedValue ||
			    this.Value != src.Value)
			{
				OnChanging();
				this.EscapedValue = src.EscapedValue;
				this.Value = src.Value;
				OnChanged();
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0} Text.Length={1}]", base.ToString(), this.EscapedValue.Length);
		}
	}
	
	/// <summary> Identifies the context in which the text occured </summary>
	public enum RawTextType
	{
		/// <summary> Ends with non-whitespace </summary>
		WhiteSpace,
		
		/// <summary> Ends with "&lt;";  "]]&gt;" is error </summary>
		CharacterData,
		
		/// <summary> Ends with "-->";  "--" is error </summary>
		Comment,
		
		/// <summary> Ends with "]]&gt;" </summary>
		CData,
		
		/// <summary> Ends with "?>" </summary>
		ProcessingInstruction,
		
		/// <summary> Ends with "&lt;" or ">" </summary>
		UnknownBang,
		
		/// <summary> Unknown </summary>
		Other
	}
}
