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
		
		/// <summary>
		/// Parent node.
		/// 
		/// Some constraints:
		///  - Reachable childs shall have parent pointer (except Document)
		///  - Parser tree can reuse data of other trees as long as it does not modify them
		///    (that, it can not set parent pointer if non-null)
		/// </summary>
		public RawObject Parent { get; set; }
		
		// TODO: Performance
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
		public event EventHandler Changed;
		
		protected void OnChanged()
		{
			LogDom("Changed {0}", this);
			if (Changed != null) {
				Changed(this, EventArgs.Empty);
			}
			RawDocument doc = this.Document;
			if (doc != null) {
				Document.OnObjectChanged(this);
			}
		}
		
		List<XmlParser.SyntaxError> syntaxErrors;
		
		/// <summary>
		/// The error that occured in the context of this node (excluding nested nodes)
		/// </summary>
		public IEnumerable<XmlParser.SyntaxError> SyntaxErrors {
			get {
				if (syntaxErrors == null) {
					return new XmlParser.SyntaxError[] {};
				} else {
					return syntaxErrors;
				}
			}
		}
		
		internal void AddSyntaxError(XmlParser.SyntaxError error)
		{
			Assert(error.Object == this);
			if (this.syntaxErrors == null) this.syntaxErrors = new List<XmlParser.SyntaxError>();
			syntaxErrors.Add(error);
		}
		
		public RawObject()
		{
			this.ReadCallID = new object();
		}
		
		protected static void Assert(bool condition)
		{
			if (!condition) {
				throw new Exception("Consistency assertion failed");
			}
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
			
			// Do not bother comparing - assume changed if non-null
			if (this.syntaxErrors != null || source.syntaxErrors != null) {
				this.syntaxErrors = new List<XmlParser.SyntaxError>();
				foreach(var error in source.SyntaxErrors) {
					// The object differs, so create our own copy
					// The source still might need it in the future and we do not want to break it
					this.AddSyntaxError(error.Clone(this));
				}
				// May be called again in derived class - oh, well, nevermind
				OnChanged();
			}
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
		
		#region Helpper methods
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
		protected static string GetNamespacePrefix(string name)
		{
			int colonIndex = name.IndexOf(':');
			if (colonIndex != -1) {
				return name.Substring(0, colonIndex);
			} else {
				return string.Empty;
			}
		}
		
		/// <summary> The part of name after ":".  Whole name if not found </summary>
		protected static string GetLocalName(string name)
		{
			int colonIndex = name.IndexOf(':');
			if (colonIndex != -1) {
				return name.Remove(0, colonIndex + 1);
			} else {
				return name ?? string.Empty;
			}
		}
		
		/// <summary> Remove quoting from the given string </summary>
		protected static string Unquote(string quoted)
		{
			if (string.IsNullOrEmpty(quoted)) return string.Empty;
			char first = quoted[0];
			if (quoted.Length == 1) return (first == '"' || first == '\'') ? string.Empty : quoted;
			char last  = quoted[quoted.Length - 1];
			if (first == '"' || first == '\'') {
				if (first == last) {
					// Remove both quotes
					return quoted.Substring(1, quoted.Length - 2);
				} else {
					// Remove first quote
					return quoted.Remove(0, 1);
				}
			} else {
				if (last == '"' || last == '\'') {
					// Remove last quote
					return quoted.Substring(0, quoted.Length - 1);
				} else {
					// Keep whole string
					return quoted;
				}
			}
		}
		
		#endregion
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
		
		#region Helpper methods
		
		ObservableCollection<RawElement> elements;
		
		/// <summary> Gets direcly nested elements (non-recursive) </summary>
		public ObservableCollection<RawElement> Elements {
			get {
				if (elements == null) {
					elements = new FilteredCollection<RawElement, ChildrenCollection<RawObject>>(this.Children);
				}
				return elements;
			}
		}
		
		#endregion
		
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
		
		/// <summary>
		/// Gets a child at the given document offset.
		/// Goes recursively down the tree.
		/// </summary>
		public RawObject GetChildAtOffset(int offset)
		{
			foreach(RawObject child in this.Children) {
				if (child.StartOffset <= offset && offset <= child.EndOffset) {
					if (child is RawContainer) {
						return ((RawContainer)child).GetChildAtOffset(offset);
					} else {
						return child;
					}
				}
			}
			return this; // No childs at offset
		}
		
		// Only these four methods should be used to modify the collection
		
		// See constriants of Parent pointer
		
		/// <summary>
		/// To be used exlucively by the parser
		/// </summary>
		internal void AddChild(RawObject item)
		{
			AddChildren(new RawObject[] {item}.ToList());
		}
		
		/// <summary>
		/// To be used exlucively by the parser
		/// </summary>
		internal void AddChildren(IEnumerable<RawObject> items)
		{
			// Childs can be only added to newly parsed items
			Assert(this.Parent == null);
			
			// Read the list just once
			items = items.ToList();
			
			foreach(RawObject item in items) {
				// Are we adding some cached item?
				// We can *not* modify data of other tree
				// It might resurect user deleted nodes, but that is fine
				if (item.Parent == null) item.Parent = this;
			}
			
			this.Children.InsertItems(this.Children.Count, items.ToList());
		}
		
		/// <summary>
		/// To be used exclusively by UpdateChildrenFrom.
		/// Insert children and keep links consistent.
		/// Note: If the nodes are in other part of the document, they will be moved
		/// </summary>
		void InsertChildren(int index, IList<RawObject> items)
		{
			RawDocument document = this.Document;
			Assert(document != null);
			Assert(!document.IsParsed);
			
			List<RawObject> attachedObjects = new List<RawObject>();
			
			// Remove from the old location and set parent
			foreach(RawObject item in items) {
				if (item.Parent == null) {
					// Dangling object - it was probably just removed from the document during update
					LogDom("Inserting dangling {0}", item);
					item.Parent = this;
					attachedObjects.Add(item);
				} else if (item.Document.IsParsed) {
					// Adding from parser tree - steal pointer; keep in the parser tree
					LogDom("Inserting {0} from parser tree", item);
					item.Parent = this;
					attachedObjects.Add(item);
				} else {
					// Adding from user other document location
					Assert(item.Document == document);  // The parser was reusing object from other document?
					LogDom("Inserting {0} from other document location", item);
					// Remove from other location
					var owingList = ((RawContainer)item.Parent).Children;
					owingList.RemoveItems(owingList.IndexOf(item), 1);
					// No detach / attach notifications
					item.Parent = this;
				}
			}
			
			// Add it
			this.Children.InsertItems(index, items);
			
			// Notify document - do last so that the handler sees up-to-date tree
			foreach(RawObject item in attachedObjects)  {
				foreach(RawObject obj in item.GetSelfAndAllChildren()) {
					document.OnObjectAttached(obj);
				}
			}
		}
		
		/// <summary>
		/// To be used exclusively by UpdateChildrenFrom.
		/// Remove children, set parent to null for them and notify the document
		/// </summary>
		void RemoveChildrenAt(int index, int count)
		{
			RawDocument document = this.Document;
			Assert(document != null);
			Assert(!document.IsParsed);
			
			List<RawObject> removed = new List<RawObject>(count);
			for(int i = 0; i < count; i++) {
				removed.Add(this.Children[index + i]);
			}
			
			// Log the action
			if (count == 1) {
				LogDom("Removing {0} at index {1}", removed[0], index);
			} else {
				LogDom("Removing at index {0}:", index);
				foreach(RawObject item in removed) LogDom("  {0}", item);
			}
			
			// Null parent pointer
			foreach(RawObject item in removed) {
				Assert(item.Parent != null);
				item.Parent = null;
			}
			
			// Remove
			this.Children.RemoveItems(index, count);
			
			// Notify document - do last so that the handler sees up-to-date tree
			foreach(RawObject item in removed) {
				foreach(RawObject obj in item.GetSelfAndAllChildren()) {
					document.OnObjectDettached(obj);
				}
			}
		}
		
		internal void CheckLinksConsistency()
		{
			foreach(RawObject child in this.Children) {
				if (child.Parent == null) throw new Exception("Null parent reference");
				if (!(child.Parent == this)) throw new Exception("Inccorect parent reference");
				if (child is RawContainer) {
					((RawContainer)child).CheckLinksConsistency();
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
		/// <summary>
		/// Parser tree (as opposed to some user tree).
		/// Parser tree can reuse data of other trees as long as it does not modify them
		/// (that, it can not set parent pointer if non-null)
		/// </summary>
		internal bool IsParsed { get; set; }
		
		/// <summary> Occurs when object is added to the document </summary>
		public event EventHandler<RawObjectEventArgs> ObjectAttached;
		/// <summary> Occurs when object is removed from the document </summary>
		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		/// <summary> Occurs when local data of object changes </summary>
		public event EventHandler<RawObjectEventArgs> ObjectChanged;
		
		internal void OnObjectAttached(RawObject obj)
		{
			if (ObjectAttached != null) ObjectAttached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectDettached(RawObject obj)
		{
			if (ObjectDettached != null) ObjectDettached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectChanged(RawObject obj)
		{
			if (ObjectChanged != null) ObjectChanged(this, new RawObjectEventArgs() { Object = obj } );
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
				OnChanged();
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
		
		#region Helpper methods
		
		ObservableCollection<RawAttribute> attributes;
		
		/// <summary> Gets attributes of the element </summary>
		public ObservableCollection<RawAttribute> Attributes {
			get {
				if (attributes == null) {
					attributes = new FilteredCollection<RawAttribute, ChildrenCollection<RawObject>>(this.StartTag.Children);
				}
				return attributes;
			}
		}
		
		ObservableCollection<RawObject> attributesAndElements;
		
		/// <summary> Gets both attributes and elements </summary>
		public ObservableCollection<RawObject> AttributesAndElements {
			get {
				if (attributesAndElements == null) {
					attributesAndElements = new MergedCollection<RawObject, ObservableCollection<RawObject>> (
						// New wrapper with RawObject types
						new FilteredCollection<RawObject, ChildrenCollection<RawObject>>(this.StartTag.Children, x => x is RawAttribute),
						new FilteredCollection<RawObject, ChildrenCollection<RawObject>>(this.Children, x => x is RawElement)
					);
				}
				return attributesAndElements;
			}
		}
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
		public string NamespacePrefix {
			get {
				return GetNamespacePrefix(this.StartTag.Name);
			}
		}
		
		/// <summary> The part of name after ":".  Whole name if not found </summary>
		public string LocalName {
			get {
				return GetLocalName(this.StartTag.Name);
			}
		}
		
		/// <summary> Resolved namespace of the name.  String empty if not found </summary>
		public string Namespace {
			get {
				return ResloveNamespacePrefix(this.NamespacePrefix);
			}
		}
		
		/// <summary>
		/// Recursively resolve given prefix in this context.
		/// If prefix is empty find "xmlns" namespace.
		/// </summary>
		public string ResloveNamespacePrefix(string prefix)
		{
			string definition = string.IsNullOrEmpty(prefix) ? "xmlns" : "xmlns:" + prefix;
			RawElement current = this;
			while(current != null) {
				string namesapce = current.GetAttributeValue(definition);
				if (namesapce != null) return namesapce;
				current = current.Parent as RawElement;
			}
			return string.Empty; // Default or undefined
		}
		
		/// <summary>
		/// Get unqoted value of attribute or null if not found.
		/// It will match the local name in any namesapce.
		/// </summary>
		public string GetAttributeValue(string localName)
		{
			return GetAttributeValue(null, localName);
		}
		
		/// <summary>
		/// Get unqoted value of attribute or null if not found
		/// </summary>
		/// <param name="namespace">Namespace.  Empty stirng to match default.  Null to match any.</param>
		/// <param name="localName">Local name - text after ":"</param>
		/// <returns></returns>
		public string GetAttributeValue(string @namespace, string localName)
		{
			// TODO: More efficient
			foreach(RawAttribute attr in this.Attributes) {
				if (attr.LocalName == localName && (@namespace == null || attr.Namespace == @namespace)) {
					return attr.Value;
				}
			}
			return null;
		}
		
		#endregion
		
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
				this.StartTag.Changed += delegate { UpdateXElement(false); };
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
		/// <summary> The raw name - exactly as in source file </summary>
		public string Name { get; set; }
		/// <summary> Equals sign and surrounding whitespace </summary>
		public string EqualsSign { get; set; }
		/// <summary> The raw value - exactly as in source file (*probably* quoted) </summary>
		public string QuotedValue { get; set; }
		
		#region Helpper methods
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
		public string NamespacePrefix {
			get {
				return GetNamespacePrefix(this.Name);
			}
		}
		
		/// <summary> The part of name after ":".  Whole name if not found </summary>
		public string LocalName {
			get {
				return GetLocalName(this.Name);
			}
		}
		
		/// <summary> Resolved namespace of the name.  String empty if not found </summary>
		public string Namespace {
			get {
				RawTag tag = this.Parent as RawTag;
				if (tag != null) {
					RawElement elem = tag.Parent as RawElement;
					if (elem != null) {
						return elem.ResloveNamespacePrefix(this.NamespacePrefix);
					}
				}
				return string.Empty; // Orphaned
			}
		}
		
		/// <summary> Attribute is declaring namespace ("xmlns" or "xmlns:*") </summary>
		public bool IsNamespaceDeclaration {
			get {
				return this.Name == "xmlns" || this.NamespacePrefix == "xmlns";
			}
		}
		
		/// <summary> Unquoted value of the attribute </summary>
		public string Value {
			get {
				return Unquote(this.QuotedValue);
			}
		}
		
		#endregion
		
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
				this.QuotedValue != src.QuotedValue)
			{
				this.Name = src.Name;
				this.EqualsSign = src.EqualsSign;
				this.QuotedValue = src.QuotedValue;
				OnChanged();
			}
		}
		
		XAttribute xAttr;
		
		public XAttribute GetXAttribute()
		{
			if (xAttr == null) {
				LogLinq("Creating XAttribute '{0}={1}'", this.Name, this.QuotedValue);
				xAttr = new XAttribute(EncodeXName(this.Name), string.Empty);
				xAttr.AddAnnotation(this);
				bool deleted = false;
				UpdateXAttribute(true, ref deleted);
				this.Changed += delegate { if (!deleted) UpdateXAttribute(false, ref deleted); };
			}
			return xAttr;
		}
		
		void UpdateXAttribute(bool firstUpdate, ref bool deleted)
		{
			if (!firstUpdate) LogLinq("Updating XAttribute '{0}={1}'", this.Name, this.QuotedValue);
			
			if (xAttr.Name == EncodeXName(this.Name)) {
				xAttr.Value = this.QuotedValue ?? string.Empty;
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
			return string.Format("[{0} '{1}{2}{3}']", base.ToString(), this.Name, this.EqualsSign, this.QuotedValue);
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
				OnChanged();
			}
		}
		
		public XText CreateXText(bool autoUpdate)
		{
			LogLinq("Creating XText Length={0}", this.Value.Length);
			XText text = new XText(string.Empty);
			text.AddAnnotation(this);
			UpdateXText(text, autoUpdate);
			if (autoUpdate) this.Changed += delegate { UpdateXText(text, autoUpdate); };
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
