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
			Log("XML DOM: Local data changed for {0}", this);
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
		
		public RawObject Clone()
		{
			RawObject clone = (RawObject)System.Activator.CreateInstance(this.GetType());
			bool oldVal = LoggingEnabled;
			LoggingEnabled = false;
			clone.UpdateDataFrom(this);
			LoggingEnabled = oldVal;
			return clone;
		}
		
		public virtual IEnumerable<RawObject> GetSeftAndAllNestedObjects()
		{
			yield return this;
		}
		
		public virtual void UpdateDataFrom(RawObject source)
		{
			this.ReadCallID = source.ReadCallID;
			this.StartOffset = source.StartOffset;
			this.EndOffset = source.EndOffset;
		}
		
		public override string ToString()
		{
			return string.Format("{0}({1}-{2})", this.GetType().Name.Remove(0, 3), this.StartOffset, this.EndOffset);
		}
		
		public static bool LoggingEnabled = true;
		
		public static void Log(string format, params object[] args)
		{
			if (LoggingEnabled) {
				System.Diagnostics.Debug.WriteLine(format, args);
			}
		}
		
		internal void OnInserting(RawObject parent, int index)
		{
			Log("XML DOM: Inserting {0} at index {1}", this, index);
			this.Parent = parent;
			if (this.Document != null) {
				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
					this.Document.OnObjectAttached(new RawObjectEventArgs() { Object = obj });
				}
			}
		}
		
		internal void OnRemoving(RawObject parent, int index)
		{
			Log("XML DOM: Removing {0} at index {1}", this, index);
			this.Parent = null;
			if (this.Document != null) {
				foreach(RawObject obj in GetSeftAndAllNestedObjects()) {
					this.Document.OnObjectDettached(new RawObjectEventArgs() { Object = obj });
				}
			}
		}
	}
	
	public class RawDocument: RawObject
	{
		public ObservableCollection<RawObject> Children { get; set; }
		
		public event EventHandler<RawObjectEventArgs> ObjectAttached;
		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		
		public ObservableCollection<RawObject> Helper_Elements {
			get {
				return new FilteredObservableCollection<RawObject>(this.Children, x => x is RawElement);
			}
		}
		
		internal void OnObjectAttached(RawObjectEventArgs e)
		{
			if (ObjectAttached != null) ObjectAttached(this, e);
		}
		
		internal void OnObjectDettached(RawObjectEventArgs e)
		{
			if (ObjectDettached != null) ObjectDettached(this, e);
		}
		
		public RawDocument()
		{
			this.Children = new ObservableCollection<RawObject>();
		}
		
		public override IEnumerable<RawObject> GetSeftAndAllNestedObjects()
		{
			return Enumerable.Union(
				new RawObject[] { this },
				this.Children.SelectMany(x => x.GetSeftAndAllNestedObjects())
			);
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
			XDocument doc = new XDocument();
			return doc;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} Chld:{1}]", base.ToString(), this.Children.Count);
		}
	}
	
	public class RawTag: RawObject
	{
		public string OpeningBracket { get; set; } // "<" or "</"
		public string Name { get; set; }
		public ObservableCollection<RawObject> Attributes { get; set; }
		public string ClosingBracket { get; set; } // ">" or "/>" for well formed
		
		public RawTag()
		{
			this.Attributes = new ObservableCollection<RawObject>();
		}
		
		public override IEnumerable<RawObject> GetSeftAndAllNestedObjects()
		{
			return Enumerable.Union(
				new RawObject[] { this },
				this.Attributes // Have no nested objects
			);
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
		
		public override IEnumerable<RawObject> GetSeftAndAllNestedObjects()
		{
			return new IEnumerable<RawObject>[] {
				new RawObject[] { this },
				this.StartTag.GetSeftAndAllNestedObjects(),
				this.Children.SelectMany(x => x.GetSeftAndAllNestedObjects()),
				this.EndTag.GetSeftAndAllNestedObjects()
			}.SelectMany(x => x);
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
			XElement elem = new XElement(string.Empty);
			UpdateXElement(elem);
			if (autoUpdate) this.StartTag.LocalDataChanged += delegate { UpdateXElement(elem); };
			return elem;
		}
		
		void UpdateXElement(XElement elem)
		{
			elem.Name = this.StartTag.Name;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Attributes.Count, this.Children.Count);
		}
	}
	
	public class RawAttribute: RawObject
	{
		public string Name { get; set; }
		public string EqualsSign { get; set; }
		public string Value { get; set; }
		
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
		
		public XAttribute CreateXAttribute(bool autoUpdate)
		{
			XAttribute attr = new XAttribute(string.Empty, string.Empty);
			UpdateXAttribute(attr);
			if (autoUpdate) this.LocalDataChanged += delegate { UpdateXAttribute(attr); };
			return attr;
		}
		
		void UpdateXAttribute(XAttribute attr)
		{
			attr.Value = this.Value;
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
			XText text = new XText(string.Empty);
			UpdateXText(text);
			if (autoUpdate) this.LocalDataChanged += delegate { UpdateXText(text); };
			return text;
		}
		
		void UpdateXText(XText text)
		{
			text.Value = this.Value;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} Text.Length={1}]", base.ToString(), this.Value.Length);
		}
	}
	
	public static class RawUtils
	{
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
					RawObject clone = srcList[i].Clone();
					clone.OnInserting(dstListOwner, i);
					dstList.Insert(i, clone);
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
							RawObject clone = srcList[j].Clone();
							clone.OnInserting(dstListOwner, j);
							dstList.Insert(j, clone);
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
					RawObject clone = srcList[i].Clone();
					clone.OnInserting(dstListOwner, i);
					dstList.Insert(i, clone);
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
