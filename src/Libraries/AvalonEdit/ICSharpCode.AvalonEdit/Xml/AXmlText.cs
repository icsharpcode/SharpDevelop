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

namespace ICSharpCode.AvalonEdit.Xml
{
	/// <summary>
	/// Whitespace or character data
	/// </summary>
	public class AXmlText: AXmlObject
	{
		/// <summary> The context in which the text occured </summary>
		internal TextType Type { get; set; }
		/// <summary> The text exactly as in source </summary>
		public string EscapedValue { get; set; }
		/// <summary> The text with all entity references resloved </summary>
		public string Value { get; set; }
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IAXmlVisitor visitor)
		{
			visitor.VisitText(this);
		}
		
		/// <inheritdoc/>
		internal override bool UpdateDataFrom(AXmlObject source)
		{
			if (!base.UpdateDataFrom(source)) return false;
			AXmlText src = (AXmlText)source;
			if (this.EscapedValue != src.EscapedValue ||
			    this.Value != src.Value)
			{
				OnChanging();
				this.EscapedValue = src.EscapedValue;
				this.Value = src.Value;
				OnChanged();
				return true;
			} else {
				return false;
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0} Text.Length={1}]", base.ToString(), this.EscapedValue.Length);
		}
	}
}
