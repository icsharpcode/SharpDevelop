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
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.AvalonEdit.Document;

// Missing XML comment
#pragma warning disable 1591

namespace ICSharpCode.AvalonEdit.XmlParser
{
	public class RawObjectEventArgs: EventArgs
	{
		public RawObject Object { get; set; }
	}
	
	/// <summary>
	/// The base class for all XML objects.  The objects store the precise text 
	/// representation so that generated text will preciesly match original.
	/// </summary>
	public abstract class RawObject: TextSegment
	{
		/// <summary>
		/// Unique identifier for the specific call of parsing read function.  
		/// It is used to uniquely identify all object data (including nested).
		/// </summary>
		internal object ReadCallID { get; private set; }
		
		public RawObject Parent { get; set; }
		
//		public RawDocument Document {
//			get {
//				if (this.Parent != null) {
//					return this.Parent.Document;
//				} else if (this is RawDocument) {
//					return (RawDocument)this;
//				} else {
//					return null;
//				}
//			}
//		}
		
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
			System.Diagnostics.Debug.WriteLine("XML DOM: " + format, args);
		}
		
		public static void LogLinq(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine("XML Linq: " + format, args);
		}
		
		internal void OnInserting(RawObject parent, int index)
		{
			LogDom("Inserting {0} at index {1}", this, index);
			this.Parent = parent;
//			if (this.Document != null) {
//				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
//					this.Document.OnObjectAttached(new RawObjectEventArgs() { Object = obj });
//				}
//			}
		}
		
		internal void OnRemoving(RawObject parent, int index)
		{
			LogDom("Removing {0} at index {1}", this, index);
			this.Parent = null;
//			if (this.Document != null) {
//				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
//					this.Document.OnObjectDettached(new RawObjectEventArgs() { Object = obj });
//				}
//			}
		}
		
		protected XName EncodeXName(string name, string ns)
		{
			if (string.IsNullOrEmpty(name)) name = "_";
			name = XmlConvert.EncodeLocalName(name);
			
			if (ns == null) ns = string.Empty;
			ns = XmlConvert.EncodeLocalName(ns);
			
			return XName.Get(name, ns);
		}
	}
	
	public class RawDocument: RawObject
	{
		public ObservableCollection<RawObject> Children { get; set; }
		
//		public event EventHandler<RawObjectEventArgs> ObjectAttached;
//		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		
		public ObservableCollection<RawObject> Helper_Elements {
			get {
				return new FilteredObservableCollection<RawObject>(this.Children, x => x is RawElement);
			}
		}
		
//		internal void OnObjectAttached(RawObjectEventArgs e)
//		{
//			if (ObjectAttached != null) ObjectAttached(this, e);
//		}
//		
//		internal void OnObjectDettached(RawObjectEventArgs e)
//		{
//			if (ObjectDettached != null) ObjectDettached(this, e);
//		}
		
		public RawDocument()
		{
			this.Children = new ObservableCollection<RawObject>();
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawDocument src = (RawDocument)source;
			RawUtils.SmartListUpdate(src.Children, this.Children, this);
		}
		
		public XDocument CreateXDocument(bool autoUpdate)
		{
			LogLinq("Creating XDocument");
			XDocument doc = new XDocument();
			doc.AddAnnotation(this);
			UpdateXDocument(doc, autoUpdate);
			this.Children.CollectionChanged += delegate { UpdateXDocument(doc, autoUpdate); };
			return doc;
		}
		
		void UpdateXDocument(XDocument doc, bool autoUpdate)
		{
			RawElement root = this.Children.OfType<RawElement>().FirstOrDefault(x => x.StartTag.OpeningBracket == "<");
			if (doc.Root.GetRawObject() != root) {
				if (root != null) {
					doc.ReplaceNodes(root.CreateXElement(autoUpdate));
				} else {
					doc.RemoveNodes();
				}
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} Chld:{1}]", base.ToString(), this.Children.Count);
		}
	}
	
	public class RawTag: RawObject
	{
		public string OpeningBracket { get; set; } // "<" or "</"
		public string Namesapce { get; set; }
		public string Name { get; set; }
		public ObservableCollection<RawObject> Attributes { get; set; }
		public string ClosingBracket { get; set; } // ">" or "/>" for well formed
		
		public RawTag()
		{
			this.Attributes = new ObservableCollection<RawObject>();
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawTag src = (RawTag)source;
			if (this.OpeningBracket != src.OpeningBracket ||
			    this.Namesapce != src.Namesapce ||
				this.Name != src.Name ||
				this.ClosingBracket != src.ClosingBracket)
			{
				this.OpeningBracket = src.OpeningBracket;
				this.Namesapce = src.Namesapce;
				this.Name = src.Name;
				this.ClosingBracket = src.ClosingBracket;
				OnLocalDataChanged();
			}
			RawUtils.SmartListUpdate(src.Attributes, this.Attributes, this);
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4}]", base.ToString(), this.OpeningBracket, this.Name, this.ClosingBracket, this.Attributes.Count);
		}
	}
	
	public class RawElement: RawObject
	{
		public RawTag StartTag { get; set; }
		public ObservableCollection<RawObject> Children { get; set; }
		public bool HasEndTag { get; set; }
		public RawTag EndTag { get; set; }
		
		public ObservableCollection<RawObject> Helper_AttributesAndElements {
			get {
				return new MergedObservableCollection<RawObject>(
					new FilteredObservableCollection<RawObject>(this.StartTag.Attributes, x => x is RawAttribute),
					new FilteredObservableCollection<RawObject>(this.Children, x => x is RawElement)
				);
			}
		}
		
		public RawElement()
		{
			this.StartTag = new RawTag() { Parent = this };
			this.Children = new ObservableCollection<RawObject>();
			this.EndTag   = new RawTag() { Parent = this };
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawElement src = (RawElement)source;
			this.StartTag.UpdateDataFrom(src.StartTag);
			RawUtils.SmartListUpdate(src.Children, this.Children, this);
			this.EndTag.UpdateDataFrom(src.EndTag);
		}
		
		public XElement CreateXElement(bool autoUpdate)
		{
			LogLinq("Creating XElement '{0}'", this.StartTag.Name);
			XElement elem = new XElement(EncodeXName(this.StartTag.Name, this.EndTag.Namesapce));
			elem.AddAnnotation(this);
			UpdateXElement(elem, autoUpdate);
			UpdateXElementAttributes(elem, autoUpdate);
			UpdateXElementChildren(elem, autoUpdate);
			if (autoUpdate) {
				this.StartTag.LocalDataChanged += delegate { UpdateXElement(elem, autoUpdate); };
				this.StartTag.Attributes.CollectionChanged += delegate { UpdateXElementAttributes(elem, autoUpdate); };
				this.Children.CollectionChanged += delegate { UpdateXElementChildren(elem, autoUpdate); };
			}
			return elem;
		}

		void UpdateXElement(XElement elem, bool autoUpdate)
		{
			LogLinq("Updating XElement '{0}'", this.StartTag.Name);
			elem.Name = EncodeXName(this.StartTag.Name, this.EndTag.Namesapce);
		}
		
		internal void UpdateXElementAttributes(XElement elem, bool autoUpdate)
		{
			List<XAttribute> xAttrs = new List<XAttribute>();
			foreach(RawAttribute attr in this.StartTag.Attributes.OfType<RawAttribute>()) {
				XAttribute existing = elem.Attributes().FirstOrDefault(x => x.GetRawObject() == attr);
				xAttrs.Add(existing ?? attr.CreateXAttribute(autoUpdate));
			}
			elem.ReplaceAttributes(xAttrs.ToArray());
		}
		
		void UpdateXElementChildren(XElement elem, bool autoUpdate)
		{
			List<XElement> xElems = new List<XElement>();
			foreach(RawElement rawElem in this.Children.OfType<RawElement>()) {
				XElement existing = (XElement)elem.Nodes().FirstOrDefault(x => x.GetRawObject() == rawElem);
				xElems.Add(existing ?? rawElem.CreateXElement(autoUpdate));
			}
			elem.ReplaceNodes(xElems);
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Attributes.Count, this.Children.Count);
		}
	}
	
	public class RawAttribute: RawObject
	{
		public string Namesapce { get; set; }
		public string Name { get; set; }
		public string EqualsSign { get; set; }
		public string Value { get; set; }
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawAttribute src = (RawAttribute)source;
			if (this.Namesapce != src.Namesapce ||
				this.Name != src.Name ||
				this.EqualsSign != src.EqualsSign ||
				this.Value != src.Value)
			{
				this.Namesapce = src.Namesapce;
				this.Name = src.Name;
				this.EqualsSign = src.EqualsSign;
				this.Value = src.Value;
				OnLocalDataChanged();
			}
		}
		
		public XAttribute CreateXAttribute(bool autoUpdate)
		{
			LogLinq("Creating XAttribute '{0}={1}'", this.Name, this.Value);
			XAttribute attr = new XAttribute(EncodeXName(this.Name, this.Namesapce), string.Empty);
			attr.AddAnnotation(this);
			bool deleted = false;
			UpdateXAttribute(attr, autoUpdate, ref deleted);
			if (autoUpdate) this.LocalDataChanged += delegate { UpdateXAttribute(attr, autoUpdate, ref deleted); };
			return attr;
		}
		
		void UpdateXAttribute(XAttribute attr, bool autoUpdate, ref bool deleted)
		{
			if (deleted) return;
			LogLinq("Updating XAttribute '{0}={1}'", this.Name, this.Value);
			if (attr.Name == EncodeXName(this.Name, this.Namesapce)) {
				attr.Value = this.Value ?? string.Empty;
			} else {
				XElement parent = attr.Parent;
				attr.Remove();
				deleted = true;
				((RawElement)parent.GetRawObject()).UpdateXElementAttributes(parent, autoUpdate);
			}
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}']", base.ToString(), this.Name, this.EqualsSign, this.Value);
		}
	}
	
	public class RawText: RawObject
	{
		public string Value { get; set; }
		
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
		
		/// <summary>
		/// Copy items from source list over to destination list.  
		/// Prefer updating items with matching offsets.
		/// </summary>
		public static void SmartListUpdate(IList<RawObject> srcList, IList<RawObject> dstList, RawObject dstListOwner)
		{
			// Items up to 'i' shall be matching
			for(int i = 0; i < srcList.Count;) {
				// Item is missing - 'i' is invalid index
				if (i >= dstList.Count) {
					srcList[i].OnInserting(dstListOwner, i);
					dstList.Insert(i, srcList[i]);
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
						for(int j = i; j < srcItemIndex; j++) {
							srcList[j].OnInserting(dstListOwner, j);
							dstList.Insert(j, srcList[j]);
						}
						i = srcItemIndex;
						goto continue2;
					}
				}
				// Scr offset matches with future dst
				for(int dstItemIndex = i; dstItemIndex < dstList.Count; dstItemIndex++) {
					RawObject dst = dstList[dstItemIndex];
					if (srcItem.StartOffset == dst.StartOffset && srcItem.GetType() == dst.GetType()) {
						for(int j = 0; j < dstItemIndex - i; j++) {
							dstList[i].OnRemoving(dstListOwner, i);
							dstList.RemoveAt(i);
						}
						goto continue2;
					}
				}
				// No matches found - just update
				if (dstItem.GetType() == srcItem.GetType()) {
					dstItem.UpdateDataFrom(srcItem);
					i++; continue;
				}
				// Remove whitespace in hope that element update will occur next
				if (dstItem is RawText) {
					dstList[i].OnRemoving(dstListOwner, i);
					dstList.RemoveAt(i);
					continue;
				}
				// Otherwise just add the item
				{
					srcList[i].OnInserting(dstListOwner, i);
					dstList.Insert(i, srcList[i]);
					i++; continue;
				}
			}
			// Remove extra items
			while(dstList.Count > srcList.Count) {
				dstList[srcList.Count].OnRemoving(dstListOwner, srcList.Count);
				dstList.RemoveAt(srcList.Count);
			}
			// Continue for inner loops
			continue2:;
		}
	}
}
