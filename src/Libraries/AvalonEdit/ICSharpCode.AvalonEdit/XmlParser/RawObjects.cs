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
		
		protected XName EncodeXName(string name, string ns)
		{
			if (string.IsNullOrEmpty(name)) name = "_";
			name = XmlConvert.EncodeLocalName(name);
			
			if (ns == null) ns = string.Empty;
			ns = XmlConvert.EncodeLocalName(ns);
			
			return XName.Get(name, ns);
		}
	}
	
	public abstract class RawContainer: RawObject
	{
		/// <summary>
		/// Children of the node.  Can be Elements, Attributes, etc...
		/// Please do not modify directly!
		/// </summary>
		public ObservableCollection<RawObject> Children { get; private set; }
		
		public RawContainer()
		{
			this.Children = new ObservableCollection<RawObject>();
		}
		
		public ObservableCollection<RawObject> Helper_Elements {
			get {
				return new FilteredObservableCollection<RawObject>(this.Children, x => x is RawElement);
			}
		}
		
		public override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawContainer src = (RawContainer)source;
			UpdateChildrenFrom(src.Children);
		}
		
		// The following should be the only methods that are ever
		// used to modify the children collection
		
		public void AddChild(RawObject item)
		{
			item.Parent = this;
			this.Children.Add(item);
		}
		
		protected virtual void Insert(int index, RawObject item)
		{
			LogDom("Inserting {0} at index {1}", item, index);
			item.Parent = this;
			this.Children.Insert(index, item);
//			if (this.Document != null) {
//				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
//					this.Document.OnObjectAttached(new RawObjectEventArgs() { Object = obj });
//				}
//			}
		}
		
		protected virtual void RemoveAt(int index)
		{
			LogDom("Removing {0} at index {1}", this.Children[index], index);
			this.Children[index].Parent = null;
			this.Children.RemoveAt(index);
//			if (this.Document != null) {
//				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
//					this.Document.OnObjectDettached(new RawObjectEventArgs() { Object = obj });
//				}
//			}
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
					Insert(i, srcList[i]);
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
							Insert(j, srcList[j]);
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
							RemoveAt(i);
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
					RemoveAt(i);
					continue;
				}
				// Otherwise just add the item
				{
					Insert(i, srcList[i]);
					i++; continue;
				}
				// Continue for inner loops
				continue2:;
			}
			// Remove extra items
			while(dstList.Count > srcList.Count) {
				RemoveAt(srcList.Count);
			}
		}
	}
	
	public class RawDocument: RawContainer
	{
//		public event EventHandler<RawObjectEventArgs> ObjectAttached;
//		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		
//		internal void OnObjectAttached(RawObjectEventArgs e)
//		{
//			if (ObjectAttached != null) ObjectAttached(this, e);
//		}
//		
//		internal void OnObjectDettached(RawObjectEventArgs e)
//		{
//			if (ObjectDettached != null) ObjectDettached(this, e);
//		}
		
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
	
	public class RawTag: RawContainer
	{
		public string OpeningBracket { get; set; } // "<" or "</"
		public string Namesapce { get; set; }
		public string Name { get; set; }
		public string ClosingBracket { get; set; } // ">" or "/>" for well formed
		
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
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4}]", base.ToString(), this.OpeningBracket, this.Name, this.ClosingBracket, this.Children.Count);
		}
	}
	
	public class RawElement: RawContainer
	{
		public RawTag StartTag {
			get {
				return (RawTag)this.Children[0];
			}
		}
		
		public ObservableCollection<RawObject> Helper_AttributesAndElements {
			get {
				return new MergedObservableCollection<RawObject>(
					new FilteredObservableCollection<RawObject>(this.StartTag.Children, x => x is RawAttribute),
					new FilteredObservableCollection<RawObject>(this.Children, x => x is RawElement)
				);
			}
		}
		
		public XElement CreateXElement(bool autoUpdate)
		{
			LogLinq("Creating XElement '{0}'", this.StartTag.Name);
			XElement elem = new XElement(EncodeXName(this.StartTag.Name, this.StartTag.Namesapce));
			elem.AddAnnotation(this);
			UpdateXElement(elem, autoUpdate);
			UpdateXElementAttributes(elem, autoUpdate);
			UpdateXElementChildren(elem, autoUpdate);
			if (autoUpdate) {
				this.StartTag.LocalDataChanged += delegate { UpdateXElement(elem, autoUpdate); };
				this.StartTag.Children.CollectionChanged += delegate { UpdateXElementAttributes(elem, autoUpdate); };
				this.Children.CollectionChanged += delegate { UpdateXElementChildren(elem, autoUpdate); };
			}
			return elem;
		}

		void UpdateXElement(XElement elem, bool autoUpdate)
		{
			LogLinq("Updating XElement '{0}'", this.StartTag.Name);
			elem.Name = EncodeXName(this.StartTag.Name, this.StartTag.Namesapce);
		}
		
		internal void UpdateXElementAttributes(XElement elem, bool autoUpdate)
		{
			List<XAttribute> xAttrs = new List<XAttribute>();
			foreach(RawAttribute attr in this.StartTag.Children.OfType<RawAttribute>()) {
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
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Children.Count, this.Children.Count);
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
	}
}
