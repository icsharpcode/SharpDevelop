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
	/// Name-value pair in a tag
	/// </summary>
	public class RawAttribute: RawObject
	{
		/// <summary> Name with namespace prefix - exactly as in source file </summary>
		public string Name { get;  internal set; }
		/// <summary> Equals sign and surrounding whitespace </summary>
		public string EqualsSign { get; internal set; }
		/// <summary> The raw value - exactly as in source file (*probably* quoted and escaped) </summary>
		public string QuotedValue { get; internal set; }
		/// <summary> Unquoted and dereferenced value of the attribute </summary>
		public string Value { get; internal set; }
		
		internal override void DebugCheckConsistency(bool allowNullParent)
		{
			DebugAssert(Name != null, "Null Name");
			DebugAssert(EqualsSign != null, "Null EqualsSign");
			DebugAssert(QuotedValue != null, "Null QuotedValue");
			DebugAssert(Value != null, "Null Value");
			base.DebugCheckConsistency(allowNullParent);
		}
		
		#region Helpper methods
		
		/// <summary> The element containing this attribute </summary>
		/// <returns> Null if orphaned </returns>
		public RawElement ParentElement {
			get {
				RawTag tag = this.Parent as RawTag;
				if (tag != null) {
					return tag.Parent as RawElement;
				}
				return null;
			}
		}
		
		/// <summary> The part of name before ":"</summary>
		/// <returns> Empty string if not found </returns>
		public string Prefix {
			get {
				return GetNamespacePrefix(this.Name);
			}
		}
		
		/// <summary> The part of name after ":" </summary>
		/// <returns> Whole name if ":" not found </returns>
		public string LocalName {
			get {
				return GetLocalName(this.Name);
			}
		}
		
		/// <summary>
		/// Resolved namespace of the name.  Empty string if not found
		/// From the specification: "The namespace name for an unprefixed attribute name always has no value."
		/// </summary>
		public string Namespace {
			get {
				if (string.IsNullOrEmpty(this.Prefix)) return NoNamespace;
				
				RawElement elem = this.ParentElement;
				if (elem != null) {
					return elem.ReslovePrefix(this.Prefix);
				}
				return NoNamespace; // Orphaned attribute
			}
		}
		
		/// <summary> Attribute is declaring namespace ("xmlns" or "xmlns:*") </summary>
		public bool IsNamespaceDeclaration {
			get {
				return this.Name == "xmlns" || this.Prefix == "xmlns";
			}
		}
		
		#endregion
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitAttribute(this);
		}
		
		/// <inheritdoc/>
		internal override void UpdateDataFrom(RawObject source)
		{
			base.UpdateDataFrom(source);  // Check asserts
			if (this.LastUpdatedFrom == source) return;
			RawAttribute src = (RawAttribute)source;
			if (this.Name != src.Name ||
				this.EqualsSign != src.EqualsSign ||
				this.QuotedValue != src.QuotedValue ||
				this.Value != src.Value)
			{
				OnChanging();
				this.Name = src.Name;
				this.EqualsSign = src.EqualsSign;
				this.QuotedValue = src.QuotedValue;
				this.Value = src.Value;
				OnChanged();
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}']", base.ToString(), this.Name, this.EqualsSign, this.Value);
		}
	}
}
