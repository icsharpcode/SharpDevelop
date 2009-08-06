// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.AvalonEdit.Document;

// TODO: Missing XML comment
#pragma warning disable 1591

namespace ICSharpCode.AvalonEdit.XmlParser
{
	public class RawObjectEventArgs: EventArgs
	{
		public RawObject Object { get; set; }
	}
	
	/// <summary>
	/// Abstact base class for all types
	/// </summary>
	public abstract class RawObject: TextSegment
	{
		/// <summary>
		/// Unique identifier for the specific call of parsing read function.  
		/// It is used to uniquely identify all object data (including nested).
		/// </summary>
		internal object ReadCallID { get; private set; }
		
		public RawObject Parent { get; set; }
		
		public RawDocument Document {
			get {
				if (this.Parent != null) {
					return this.Parent.Document;
				} else if (this is RawDocument) {
					return (RawDocument)this;
				} else {
					return null;
				}
			}
		}
		
		/// <summary> Occurs when the value of any local properties changes.  Nested changes do not cause the event to occur </summary>
		public event EventHandler LocalDataChanged;
		
		protected void OnLocalDataChanged()
		{
			LogDom("Local data changed for {0}", this);
			if (LocalDataChanged != null) {
				LocalDataChanged(this, EventArgs.Empty);
			}
		}
		
		public new int EndOffset {
			get {
				return this.StartOffset + this.Length;
			}
			set {
				this.Length = value - this.StartOffset;
			}
		}
		
		public RawObject()
		{
			this.ReadCallID = new object();
		}
		
		public virtual IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return new RawObject[] { this };
		}
		
		public abstract void AcceptVisitor(IXmlVisitor visitor);
		
		public virtual void UpdateDataFrom(RawObject source)
		{
			this.ReadCallID = source.ReadCallID;
			// In some cases we are just updating objects of that same
			// type and sequential position hoping to be luckily right
			this.StartOffset = source.StartOffset;
			this.EndOffset = source.EndOffset;
		}
		
		public override string ToString()
		{
			return string.Format("{0}({1}-{2})", this.GetType().Name.Remove(0, 3), this.StartOffset, this.EndOffset);
		}
		
		public static void LogDom(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("XML DOM: " + format, args));
		}
		
		public static void LogLinq(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("XML Linq: " + format, args));
		}
		
		protected XName EncodeXName(string name)
		{
			if (string.IsNullOrEmpty(name)) name = "_";
			
			string namesapce = string.Empty;
			int colonIndex = name.IndexOf(':');
			if (colonIndex != -1) {
				namesapce = name.Substring(0, colonIndex);
				name = name.Substring(colonIndex + 1);
			}
			
			name = XmlConvert.EncodeLocalName(name);
			namesapce = XmlConvert.EncodeLocalName(namesapce);
			return XName.Get(name, namesapce);
		}
	}
	
	/// <summary>
	/// Abstact base class for all types that can contain child nodes
	/// </summary>
	public abstract class RawContainer: RawObject
	{
		/// <summary>
		/// Children of the node.  It is read-only.
		/// Note that is has CollectionChanged event.
		/// </summary>
		public ChildrenCollection<RawObject> Children { get; private set; }
		
		public RawContainer()
		{
			this.Children = new ChildrenCollection<RawObject>();
		}
		
		public ObservableCollection<RawObject> Helper_Elements {
			get {
				return new FilteredCollection<ChildrenCollection<RawObject>, RawObject>(this.Children, x => x is RawElement);
			}
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawContainer src = (RawContainer)source;
			UpdateChildrenFrom(src.Children);
		}
		
		public override IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return Enumerable.Union(
				new RawContainer[] { this },
				this.Children.SelectMany(x => x.GetSelfAndAllChildren())
			);
		}
		
		// Only these four methods should be used to modify the collection
		
		internal void AddChild(RawObject item)
		{
			this.InsertChildren(this.Children.Count, new RawObject[] {item}.ToList());
		}
		
		internal void AddChildren(IEnumerable<RawObject> items)
		{
			this.InsertChildren(this.Children.Count, items.ToList());
		}
		
		/// <summary>
		/// Insert children, set parent for them and notify the document
		/// </summary>
		void InsertChildren(int index, IList<RawObject> items)
		{
			if (items.Count == 1) {
				LogDom("Inserting {0} at index {1}", items[0], index);
			} else {
				LogDom("Inserting at index {0}:", index);
				foreach(RawObject item in items) LogDom("  {0}", item);
			}
			foreach(RawObject item in items) item.Parent = this;
			this.Children.InsertItems(index, items);
			RawDocument document = this.Document;
			if (document != null) {
				foreach(RawObject item in items)  {
					foreach(RawObject obj in item.GetSelfAndAllChildren()) {
						document.OnObjectAttached(obj);
					}
				}
			}
		}
		
		/// <summary>
		/// Remove children, set parent to null for them and notify the document
		/// </summary>
		void RemoveChildrenAt(int index, int count)
		{
			List<RawObject> removed = new List<RawObject>(count);
			for(int i = 0; i < count; i++) {
				removed.Add(this.Children[index + i]);
			}
			if (count == 1) {
				LogDom("Removing {0} at index {1}", removed[0], index);
			} else {
				LogDom("Removing at index {0}:", index);
				foreach(RawObject item in removed) LogDom("  {0}", item);
			}
			foreach(RawObject item in removed) item.Parent = null;
			this.Children.RemoveItems(index, count);
			RawDocument document = this.Document;
			if (document != null) {
				foreach(RawObject item in removed) {
					foreach(RawObject obj in item.GetSelfAndAllChildren()) {
						document.OnObjectDettached(obj);
					}
				}
			}
		}
		
		/// <summary>
		/// Copy items from source list over to destination list.  
		/// Prefer updating items with matching offsets.
		/// </summary>
		public void UpdateChildrenFrom(IList<RawObject> srcList)
		{
			IList<RawObject> dstList = this.Children;
			
			// Items up to 'i' shall be matching
			int i = 0;
			// Do not do anything smart with the start tag
			if (this is RawElement) {
				dstList[0].UpdateDataFrom(srcList[0]);
				i++;
			}
			while(i < srcList.Count) {
				// Item is missing - 'i' is invalid index
				if (i >= dstList.Count) {
					// Add the rest of the items
					List<RawObject> itemsToAdd = new List<RawObject>();
					for(int j = i; j < srcList.Count; j++) {
						itemsToAdd.Add(srcList[j]);
					}
					InsertChildren(i, itemsToAdd);
					i++; continue;
				}
				RawObject srcItem = srcList[i];
				RawObject dstItem = dstList[i];
				// Matching and updated
				if (srcItem.ReadCallID == dstItem.ReadCallID) {
					i++; continue;
				}
				// Offsets and types are matching
				if (srcItem.StartOffset == dstItem.StartOffset &&
				    srcItem.GetType() == dstItem.GetType())
				{
					dstItem.UpdateDataFrom(srcItem);
					i++; continue;
				}
				// Try to be smart by inserting or removing items
				// Dst offset matches with future src
				for(int srcItemIndex = i; srcItemIndex < srcList.Count; srcItemIndex++) {
					RawObject src = srcList[srcItemIndex];
					if (src.StartOffset == dstItem.StartOffset && src.GetType() == dstItem.GetType()) {
						List<RawObject> itemsToAdd = new List<RawObject>();
						for(int j = i; j < srcItemIndex; j++) {
							itemsToAdd.Add(srcList[j]);
						}
						InsertChildren(i, itemsToAdd);
						i = srcItemIndex;
						goto continue2;
					}
				}
				// Scr offset matches with future dst
				for(int dstItemIndex = i; dstItemIndex < dstList.Count; dstItemIndex++) {
					RawObject dst = dstList[dstItemIndex];
					if (srcItem.StartOffset == dst.StartOffset && srcItem.GetType() == dst.GetType()) {
						RemoveChildrenAt(i, dstItemIndex - i);
						goto continue2;
					}
				}
				// No matches found - just update
				if (dstItem.GetType() == srcItem.GetType()) {
					dstItem.UpdateDataFrom(srcItem);
					i++; continue;
				}
				// Remove fluf in hope that element/attribute update will occur next
				if (!(dstItem is RawElement) && !(dstItem is RawAttribute)) {
					RemoveChildrenAt(i, 1);
					continue;
				}
				// Otherwise just add the item
				{
					InsertChildren(i, new RawObject[] {srcList[i]}.ToList());
					i++; continue;
				}
				// Continue for inner loops
				continue2:;
			}
			// Remove extra items
			if (dstList.Count > srcList.Count) {
				RemoveChildrenAt(srcList.Count, dstList.Count - srcList.Count);
			}
		}
	}
	
	/// <summary>
	/// The root object of the XML document
	/// </summary>
	public class RawDocument: RawContainer
	{
		public event EventHandler<RawObjectEventArgs> ObjectAttached;
		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		
		internal void OnObjectAttached(RawObject obj)
		{
			if (ObjectAttached != null) ObjectAttached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectDettached(RawObject obj)
		{
			if (ObjectDettached != null) ObjectDettached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitDocument(this);
		}
		
		XDocument xDoc;
		
		public XDocument GetXDocument()
		{
			if (xDoc == null) {
				LogLinq("Creating XDocument");
				xDoc = new XDocument();
				xDoc.AddAnnotation(this);
				UpdateXDocumentChildren(true);
				this.Children.CollectionChanged += delegate { UpdateXDocumentChildren(false); };
			}
			return xDoc;
		}
		
		void UpdateXDocumentChildren(bool firstUpdate)
		{
			if (!firstUpdate) LogLinq("Updating XDocument Children");
			
			RawElement root = this.Children.OfType<RawElement>().FirstOrDefault(x => x.StartTag.OpeningBracket == "<");
			if (xDoc.Root.GetRawObject() != root) {
				if (root != null) {
					xDoc.ReplaceNodes(root.GetXElement());
				} else {
					xDoc.RemoveNodes();
				}
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} Chld:{1}]", base.ToString(), this.Children.Count);
		}
	}
	
	/// <summary>
	/// Represents any markup starting with "&lt;" and (hopefully) ending with ">"
	/// </summary>
	public class RawTag: RawContainer
	{
		public static readonly string[] DTDNames = new string[] {"<!DOCTYPE", "<!NOTATION", "<!ELEMENT", "<!ATTLIST", "<!ENTITY"};
		
		public string OpeningBracket { get; set; }
		public string Name { get; set; }
		public string ClosingBracket { get; set; }
		
		// Exactly one of the folling will be true
		public bool IsStartTag              { get { return OpeningBracket == "<"; } }
		public bool IsEndTag                { get { return OpeningBracket == "</"; } }
		public bool IsProcessingInstruction { get { return OpeningBracket == "<?"; } }
		public bool IsComment               { get { return OpeningBracket == "<!--"; } }
		public bool IsCData                 { get { return OpeningBracket == "<![CDATA["; } }
		public bool IsDocumentType          { get { return DTDNames.Contains(OpeningBracket); } }
		public bool IsUnknownBang           { get { return OpeningBracket == "<!"; } }
		
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitTag(this);
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawTag src = (RawTag)source;
			if (this.OpeningBracket != src.OpeningBracket ||
				this.Name != src.Name ||
				this.ClosingBracket != src.ClosingBracket)
			{
				this.OpeningBracket = src.OpeningBracket;
				this.Name = src.Name;
				this.ClosingBracket = src.ClosingBracket;
				OnLocalDataChanged();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4}]", base.ToString(), this.OpeningBracket, this.Name, this.ClosingBracket, this.Children.Count);
		}
	}
	
	/// <summary>
	/// Logical grouping of other nodes together.  The first child is always the start tag.
	/// </summary>
	public class RawElement: RawContainer
	{
		/// <summary>
		/// StartTag of an element.  It is always the first child and its identity does not change.
		/// </summary>
		public RawTag StartTag {
			get {
				if (this.Children.Count == 0) return null;
				return (RawTag)this.Children[0];
			}
		}
		
		public ObservableCollection<RawObject> Helper_AttributesAndElements {
			get {
				return new MergedCollection<ObservableCollection<RawObject>, RawObject>(
					new FilteredCollection<ChildrenCollection<RawObject>, RawObject>(this.StartTag.Children, x => x is RawAttribute),
					new FilteredCollection<ChildrenCollection<RawObject>, RawObject>(this.Children, x => x is RawElement)
				);
			}
		}
		
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitElement(this);
		}
		
		XElement xElem;
		
		public XElement GetXElement()
		{
			if (xElem == null) {
				LogLinq("Creating XElement '{0}'", this.StartTag.Name);
				xElem = new XElement(EncodeXName(this.StartTag.Name));
				xElem.AddAnnotation(this);
				UpdateXElement(true);
				UpdateXElementAttributes(true);
				UpdateXElementChildren(true);
				this.StartTag.LocalDataChanged += delegate { UpdateXElement(false); };
				this.StartTag.Children.CollectionChanged += delegate { UpdateXElementAttributes(false); };
				this.Children.CollectionChanged += delegate { UpdateXElementChildren(false); };
			}
			return xElem;
		}

		void UpdateXElement(bool firstUpdate)
		{
			if (!firstUpdate) LogLinq("Updating XElement '{0}'", this.StartTag.Name);
			
			xElem.Name = EncodeXName(this.StartTag.Name);
		}
		
		internal void UpdateXElementAttributes(bool firstUpdate)
		{
			if (!firstUpdate) LogLinq("Updating XElement Attributes of '{0}'", this.StartTag.Name);
			
			// TODO:  Investigate null
			if (xElem != null) {
				xElem.ReplaceAttributes(); // Otherwise we get duplicate item exception
				XAttribute[] attrs = this.StartTag.Children.OfType<RawAttribute>().Select(x => x.GetXAttribute()).Distinct(new AttributeNameComparer()).ToArray();
				xElem.ReplaceAttributes(attrs);
			}
		}
		
		void UpdateXElementChildren(bool firstUpdate)
		{
			if (!firstUpdate) LogLinq("Updating XElement Children of '{0}'", this.StartTag.Name);
			
			xElem.ReplaceNodes(
				this.Children.OfType<RawElement>().Select(x => x.GetXElement()).ToArray()
			);
		}
		
		class AttributeNameComparer: IEqualityComparer<XAttribute>
		{
			public bool Equals(XAttribute x, XAttribute y)
			{
				return x.Name == y.Name;
			}
			
			public int GetHashCode(XAttribute obj)
			{
				return obj.Name.GetHashCode();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Children.Count, this.Children.Count);
		}
	}
	
	/// <summary>
	/// Name-value pair in a tag
	/// </summary>
	public class RawAttribute: RawObject
	{
		public string Name { get; set; }
		public string EqualsSign { get; set; }
		public string Value { get; set; }
		
		// TODO: Provide method to dereference Value - &
		
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitAttribute(this);
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawAttribute src = (RawAttribute)source;
			if (this.Name != src.Name ||
				this.EqualsSign != src.EqualsSign ||
				this.Value != src.Value)
			{
				this.Name = src.Name;
				this.EqualsSign = src.EqualsSign;
				this.Value = src.Value;
				OnLocalDataChanged();
			}
		}
		
		XAttribute xAttr;
		
		public XAttribute GetXAttribute()
		{
			if (xAttr == null) {
				LogLinq("Creating XAttribute '{0}={1}'", this.Name, this.Value);
				xAttr = new XAttribute(EncodeXName(this.Name), string.Empty);
				xAttr.AddAnnotation(this);
				bool deleted = false;
				UpdateXAttribute(true, ref deleted);
				this.LocalDataChanged += delegate { if (!deleted) UpdateXAttribute(false, ref deleted); };
			}
			return xAttr;
		}
		
		void UpdateXAttribute(bool firstUpdate, ref bool deleted)
		{
			if (!firstUpdate) LogLinq("Updating XAttribute '{0}={1}'", this.Name, this.Value);
			
			if (xAttr.Name == EncodeXName(this.Name)) {
				xAttr.Value = this.Value ?? string.Empty;
			} else {
				XElement xParent = xAttr.Parent;
				if (xAttr.Parent != null) xAttr.Remove(); // Duplicate items are not added
				xAttr = null;
				deleted = true; // No longer get events for this instance
				((RawElement)this.Parent.Parent).UpdateXElementAttributes(false);
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}']", base.ToString(), this.Name, this.EqualsSign, this.Value);
		}
	}
	
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
	
	/// <summary>
	/// Whitespace or character data
	/// </summary>
	public class RawText: RawObject
	{
		public RawTextType Type { get; set; }
		public string Value { get; set; }
		
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitText(this);
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawText src = (RawText)source;
			if (this.Value != src.Value) {
				this.Value = src.Value;
				OnLocalDataChanged();
			}
		}
		
		public XText CreateXText(bool autoUpdate)
		{
			LogLinq("Creating XText Length={0}", this.Value.Length);
			XText text = new XText(string.Empty);
			text.AddAnnotation(this);
			UpdateXText(text, autoUpdate);
			if (autoUpdate) this.LocalDataChanged += delegate { UpdateXText(text, autoUpdate); };
			return text;
		}
		
		void UpdateXText(XText text, bool autoUpdate)
		{
			LogLinq("Updating XText Length={0}", this.Value.Length);
			text.Value = this.Value;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} Text.Length={1}]", base.ToString(), this.Value.Length);
		}
	}
	
	public static class RawUtils
	{
		public static RawObject GetRawObject(this XObject xObj)
		{
			if (xObj == null) return null;
			return xObj.Annotation<RawObject>();
		}
	}
}
