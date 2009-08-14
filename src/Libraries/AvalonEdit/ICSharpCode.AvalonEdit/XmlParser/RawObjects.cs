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
	/// <summary> Holds event args for event caused by <see cref="RawObject"/> </summary>
	public class RawObjectEventArgs: EventArgs
	{
		/// <summary> The object that caused the event </summary>
		public RawObject Object { get; set; }
	}
	
	/// <summary>
	/// Abstact base class for all types
	/// </summary>
	public abstract class RawObject: TextSegment
	{
		/// <summary> Empty string.  The namespace used if there is no "xmlns" specified </summary>
		public static readonly string NoNamespace = string.Empty;
		
		/// <summary> Namespace for "xml:" prefix: "http://www.w3.org/XML/1998/namespace" </summary>
		public static readonly string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
		
		/// <summary> Namesapce for "xmlns:" prefix: "http://www.w3.org/2000/xmlns/" </summary>
		public static readonly string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
		
		/// <summary> Parent node. </summary>
		/// <remarks>
		/// New cached items start with null parent.
		/// Cache constraint:
		///   If cached item has parent set, then the whole subtree must be consistent
		/// </remarks>
		public RawObject Parent { get; set; }
		
		/// <summary> Gets the document owning this object or null if orphaned </summary>
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
		
		/// <summary> Creates new object </summary>
		public RawObject()
		{
			this.LastUpdatedFrom = this;
		}
		
		/// <summary> Occurs before the value of any local properties changes.  Nested changes do not cause the event to occur </summary>
		public event EventHandler<RawObjectEventArgs> Changing;
		
		/// <summary> Occurs after the value of any local properties changed.  Nested changes do not cause the event to occur </summary>
		public event EventHandler<RawObjectEventArgs> Changed;
		
		/// <summary> Raises Changing event </summary>
		protected void OnChanging()
		{
			LogDom("Changing {0}", this);
			if (Changing != null) {
				Changing(this, new RawObjectEventArgs() { Object = this } );
			}
			RawDocument doc = this.Document;
			if (doc != null) {
				doc.OnObjectChanging(this);
			}
		}
		
		/// <summary> Raises Changed event </summary>
		protected void OnChanged()
		{
			LogDom("Changed {0}", this);
			if (Changed != null) {
				Changed(this, new RawObjectEventArgs() { Object = this } );
			}
			RawDocument doc = this.Document;
			if (doc != null) {
				doc.OnObjectChanged(this);
			}
		}
		
		List<SyntaxError> syntaxErrors;
		
		/// <summary>
		/// The error that occured in the context of this node (excluding nested nodes)
		/// </summary>
		public IEnumerable<SyntaxError> SyntaxErrors {
			get {
				if (syntaxErrors == null) {
					return new SyntaxError[] {};
				} else {
					return syntaxErrors;
				}
			}
		}
		
		internal void AddSyntaxError(SyntaxError error)
		{
			DebugAssert(error.Object == this, "Must own the error");
			if (this.syntaxErrors == null) this.syntaxErrors = new List<SyntaxError>();
			syntaxErrors.Add(error);
		}
		
		/// <summary> Throws exception if condition is false </summary>
		/// <remarks> Present in release mode - use only for very cheap aserts </remarks>
		protected static void Assert(bool condition, string message)
		{
			if (!condition) {
				throw new Exception("Assertion failed: " + message);
			}
		}
		
		/// <summary> Throws exception if condition is false </summary>
		[Conditional("DEBUG")]
		protected static void DebugAssert(bool condition, string message)
		{
			if (!condition) {
				throw new Exception("Assertion failed: " + message);
			}
		}
		
		/// <summary> Recursively gets self and all nested nodes. </summary>
		public virtual IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return new RawObject[] { this };
		}
		
		/// <summary> Get all ancestors of this node </summary>
		public IEnumerable<RawObject> GetAncestors()
		{
			RawObject curr = this.Parent;
			while(curr != null) {
				yield return curr;
				curr = curr.Parent;
			}
		}
		
		/// <summary> Call appropriate visit method on the given visitor </summary>
		public abstract void AcceptVisitor(IXmlVisitor visitor);
		
		/// <summary> The parser tree object this object was updated from </summary>
		internal object LastUpdatedFrom { get; private set; }
		
		internal bool IsInCache { get; set; }
		
		/// <summary> Is call to UpdateDataFrom is allowed? </summary>
		internal bool CanUpdateDataFrom(RawObject source)
		{
			return
				this.GetType() == source.GetType() &&
				this.StartOffset == source.StartOffset &&
				(this.LastUpdatedFrom == source || !this.IsInCache);
		}
		
		/// <summary> Copy all data from the 'source' to this object </summary>
		internal virtual void UpdateDataFrom(RawObject source)
		{
			Assert(this.GetType() == source.GetType(), "Source has different type");
			DebugAssert(this.StartOffset == source.StartOffset, "Source has different StartOffset");
			
			if (this.LastUpdatedFrom == source) {
				DebugAssert(this.EndOffset == source.EndOffset, "Source has different EndOffset");
				return;
			}
			
			Assert(!this.IsInCache, "Can not update cached item");
			Assert(source.IsInCache, "Must update from cache");
			
			this.LastUpdatedFrom = source;
			this.StartOffset = source.StartOffset;
			// In some cases we are just updating objects of that same
			// type and position and hoping to be luckily right
			this.EndOffset = source.EndOffset;
			
			// Do not bother comparing - assume changed if non-null
			if (this.syntaxErrors != null || source.syntaxErrors != null) {
				// May be called again in derived class - oh, well, nevermind
				OnChanging();
				if (source.syntaxErrors == null) {
					this.syntaxErrors = null;
				} else {
					this.syntaxErrors = new List<SyntaxError>();
					foreach(var error in source.SyntaxErrors) {
						// The object differs, so create our own copy
						// The source still might need it in the future and we do not want to break it
						this.AddSyntaxError(error.Clone(this));
					}
				}
				OnChanged();
			}
		}
		
		/// <summary> Verify that the item is consistent.  Only in debug build. </summary>
		[Conditional("DEBUG")]
		internal virtual void DebugCheckConsistency(bool allowNullParent)
		{
			
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("{0}({1}-{2})", this.GetType().Name.Remove(0, 3), this.StartOffset, this.EndOffset);
		}
		
		internal static void LogDom(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("XML DOM: " + format, args));
		}
		
		#region Helpper methods
		
		/// <summary> The part of name before ":" </summary>
		/// <returns> Empty string if not found </returns>
		protected static string GetNamespacePrefix(string name)
		{
			if (string.IsNullOrEmpty(name)) return string.Empty;
			int colonIndex = name.IndexOf(':');
			if (colonIndex != -1) {
				return name.Substring(0, colonIndex);
			} else {
				return string.Empty;
			}
		}
		
		/// <summary> The part of name after ":" </summary>
		/// <returns> Whole name if ":" not found </returns>
		protected static string GetLocalName(string name)
		{
			if (string.IsNullOrEmpty(name)) return string.Empty;
			int colonIndex = name.IndexOf(':');
			if (colonIndex != -1) {
				return name.Remove(0, colonIndex + 1);
			} else {
				return name ?? string.Empty;
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
		
		/// <summary> Create new container </summary>
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
		
		internal RawObject FirstChild {
			get {
				return this.Children[0];
			}
		}
		
		internal RawObject LastChild {
			get {
				return this.Children[this.Children.Count - 1];
			}
		}
		
		#endregion
		
		/// <inheritdoc/>
		public override IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return new RawObject[] { this }.Flatten(i => i is RawContainer ? ((RawContainer)i).Children : null);
		}
		
		/// <summary>
		/// Gets a child fully containg the given offset.
		/// Goes recursively down the tree.
		/// Specail case if at the end of attribute or text
		/// </summary>
		public RawObject GetChildAtOffset(int offset)
		{
			foreach(RawObject child in this.Children) {
				if ((child is RawAttribute || child is RawText) && offset == child.EndOffset) return child;
				if (child.StartOffset < offset && offset < child.EndOffset) {
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
		
		/// <summary> To be used exlucively by the parser </summary>
		internal void AddChild(RawObject item)
		{
			// Childs can be only added to newly parsed items
			Assert(this.Parent == null, "I have to be new");
			Assert(item.IsInCache, "Added item must be in cache");
			// Do not set parent pointer
			this.Children.InsertItemAt(this.Children.Count, item);
		}
		
		/// <summary> To be used exlucively by the parser </summary>
		internal void AddChildren(IEnumerable<RawObject> items)
		{
			// Childs can be only added to newly parsed items
			Assert(this.Parent == null, "I have to be new");
			// Do not set parent pointer
			this.Children.InsertItemsAt(this.Children.Count, items.ToList());
		}
		
		/// <summary>
		/// To be used exclusively by the children update algorithm.
		/// Insert child and keep links consistent.
		/// </summary>
		void InsertChild(int index, RawObject item)
		{
			LogDom("Inserting {0} at index {1}", item, index);
			
			RawDocument document = this.Document;
			Assert(document != null, "Can not insert to dangling object");
			Assert(item.Parent != this, "Can not own item twice");
			
			SetParentPointersInTree(item);
			
			this.Children.InsertItemAt(index, item);
			
			document.OnObjectInserted(index, item);
		}
		
		/// <summary> Recursively fix all parent pointer in a tree </summary>
		/// <remarks>
		/// Cache constraint:
		///    If cached item has parent set, then the whole subtree must be consistent
		/// </remarks>
		void SetParentPointersInTree(RawObject item)
		{
			// All items come from the parser cache
			
			if (item.Parent == null) {
				// Dangling object - either a new parser object or removed tree (still cached)
				item.Parent = this;
				if (item is RawContainer) {
					foreach(RawObject child in ((RawContainer)item).Children) {
						((RawContainer)item).SetParentPointersInTree(child);
					}
				}
			} else if (item.Parent == this) {
				// If node is attached and then deattached, it will have null parent pointer
				//   but valid subtree - so its children will alredy have correct parent pointer
				//   like in this case
				item.DebugCheckConsistency(false);
				// Rest of the tree is consistent - do not recurse
			} else {
				// From cache & parent set => consitent subtree
				item.DebugCheckConsistency(false);
				// The parent (or any futher parents) can not be part of parsed document
				//   becuase otherwise this item would be included twice => safe to change parents
				DebugAssert(item.Parent.Document == null, "Old parent is part of document as well");
				// Maintain cache constraint by setting parents to null
				foreach(RawObject ancest in item.GetAncestors().ToList()) {
					ancest.Parent = null; 
				}
				item.Parent = this;
				// Rest of the tree is consistent - do not recurse
			}
		}
		
		/// <summary>
		/// To be used exclusively by the children update algorithm.
		/// Remove child, set parent to null and notify the document
		/// </summary>
		void RemoveChild(int index)
		{
			RawObject removed = this.Children[index];
			LogDom("Removing {0} at index {1}", removed, index);
			
			// Null parent pointer
			Assert(removed.Parent == this, "Inconsistent child");
			removed.Parent = null;
			
			this.Children.RemoveItemAt(index);
			
			this.Document.OnObjectRemoved(index, removed);
		}
		
		/// <summary> Verify that the subtree is consistent.  Only in debug build. </summary>
		internal override void DebugCheckConsistency(bool allowNullParent)
		{
			base.DebugCheckConsistency(allowNullParent);
			RawObject prevChild = null;
			int myStartOffset = this.StartOffset;
			int myEndOffset = this.EndOffset;
			foreach(RawObject child in this.Children) {
				Assert(child.Length != 0, "Empty child");
				if (!allowNullParent) {
					Assert(child.Parent != null, "Null parent reference");
				}
				Assert(child.Parent == null || child.Parent == this, "Inccorect parent reference");
				Assert(myStartOffset <= child.StartOffset && child.EndOffset <= myEndOffset, "Child not within parent text range");
				if (this.IsInCache)
					Assert(child.IsInCache, "Child not in cache");
				if (prevChild != null)
					Assert(prevChild.EndOffset <= child.StartOffset, "Overlaping childs");
				child.DebugCheckConsistency(allowNullParent);
				prevChild = child;
			}
		}
		
		internal void UpdateTreeFrom(RawContainer srcContainer)
		{
			RemoveChildrenNotIn(srcContainer.Children);
			InsertAndUpdateChildrenFrom(srcContainer.Children);
		}
		
		void RemoveChildrenNotIn(IList<RawObject> srcList)
		{
			Dictionary<int, RawObject> srcChildren = srcList.ToDictionary(i => i.StartOffset);
			for(int i = 0; i < this.Children.Count;) {
				RawObject child = this.Children[i];
				RawObject srcChild;
				
				if (srcChildren.TryGetValue(child.StartOffset, out srcChild) && child.CanUpdateDataFrom(srcChild)) {
					// Keep only one item with given offset (we might have several due to deletion)
					srcChildren.Remove(child.StartOffset);
					if (child is RawContainer)
						((RawContainer)child).RemoveChildrenNotIn(((RawContainer)srcChild).Children);
					i++;
				} else {
					RemoveChild(i);
				}
			}
		}
		
		void InsertAndUpdateChildrenFrom(IList<RawObject> srcList)
		{
			for(int i = 0; i < srcList.Count; i++) {
				// End of our list?
				if (i == this.Children.Count) {
					InsertChild(i, srcList[i]);
					continue;
				}
				RawObject child = this.Children[i];
				RawObject srcChild = srcList[i];
				
				if (child.CanUpdateDataFrom(srcChild) /* includes offset test */) {
					child.UpdateDataFrom(srcChild);
					if (child is RawContainer)
						((RawContainer)child).InsertAndUpdateChildrenFrom(((RawContainer)srcChild).Children);
				} else {
					InsertChild(i, srcChild);
				}
			}
			Assert(this.Children.Count == srcList.Count, "List lengths differ after update");
		}
	}
	
	/// <summary> Information about syntax error that occured during parsing </summary>
	public class SyntaxError: TextSegment
	{
		/// <summary> Object for which the error occured </summary>
		public RawObject Object { get; internal set; }
		/// <summary> Textual description of the error </summary>
		public string Message { get; internal set; }
		/// <summary> Any user data </summary>
		public object Tag { get; set; }
		
		internal SyntaxError Clone(RawObject newOwner)
		{
			return new SyntaxError {
				Object = newOwner,
				Message = Message,
				Tag = Tag,
				StartOffset = StartOffset,
				EndOffset = EndOffset,
			};
		}
	}
	
	/// <summary>
	/// The root object of the XML document
	/// </summary>
	public class RawDocument: RawContainer
	{
		/// <summary> Parser that produced this document </summary>
		internal XmlParser Parser { get; set; }
		
		/// <summary> Occurs when object is added to any part of the document </summary>
		public event EventHandler<NotifyCollectionChangedEventArgs> ObjectInserted;
		/// <summary> Occurs when object is removed from any part of the document </summary>
		public event EventHandler<NotifyCollectionChangedEventArgs> ObjectRemoved;
		/// <summary> Occurs before local data of any object in the document changes </summary>
		public event EventHandler<RawObjectEventArgs> ObjectChanging;
		/// <summary> Occurs after local data of any object in the document changed </summary>
		public event EventHandler<RawObjectEventArgs> ObjectChanged;
		
		internal void OnObjectInserted(int index, RawObject obj)
		{
			if (ObjectInserted != null)
				ObjectInserted(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new RawObject[] { obj }.ToList(), index));
		}
		
		internal void OnObjectRemoved(int index, RawObject obj)
		{
			if (ObjectRemoved != null)
				ObjectRemoved(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new RawObject[] { obj }.ToList(), index));
		}
		
		internal void OnObjectChanging(RawObject obj)
		{
			if (ObjectChanging != null)
				ObjectChanging(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectChanged(RawObject obj)
		{
			if (ObjectChanged != null)
				ObjectChanged(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitDocument(this);
		}
		
		/// <inheritdoc/>
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
	
	/// <summary>
	/// Logical grouping of other nodes together.
	/// </summary>
	public class RawElement: RawContainer
	{
		/// <summary> No tags are missing anywhere within this element (recursive) </summary>
		public bool IsProperlyNested { get; set; }
		/// <returns> True in wellformed XML </returns>
		public bool HasStartOrEmptyTag { get; set; }
		/// <returns> True in wellformed XML </returns>
		public bool HasEndTag { get; set; }
		
		/// <summary>  StartTag of an element.  </summary>
		public RawTag StartTag {
			get {
				return (RawTag)this.Children[0];
			}
		}
		
		internal override void DebugCheckConsistency(bool allowNullParent)
		{
			DebugAssert(Children.Count > 0, "No children");
			base.DebugCheckConsistency(allowNullParent);
		}
		
		#region Helpper methods
		
		AttributeCollection attributes;
		
		/// <summary> Gets attributes of the element </summary>
		public AttributeCollection Attributes {
			get {
				if (attributes == null) {
					attributes = new AttributeCollection(this.StartTag.Children);
				}
				return attributes;
			}
		}
		
		ObservableCollection<RawObject> attributesAndElements;
		
		// TODO: Identity
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
		
		/// <summary> Name with namespace prefix - exactly as in source </summary>
		public string Name {
			get {
				return this.StartTag.Name;
			}
		}
		
		/// <summary> The part of name before ":" </summary>
		/// <returns> Empty string if not found </returns>
		public string Prefix {
			get {
				return GetNamespacePrefix(this.StartTag.Name);
			}
		}
		
		/// <summary> The part of name after ":" </summary>
		/// <returns> Empty string if not found </returns>
		public string LocalName {
			get {
				return GetLocalName(this.StartTag.Name);
			}
		}
		
		/// <summary> Resolved namespace of the name </summary>
		/// <returns> Empty string if prefix is not found </returns>
		public string Namespace {
			get {
				string prefix = this.Prefix;
				if (string.IsNullOrEmpty(prefix)) {
					return FindDefaultNamesapce();
				} else {
					return ReslovePrefix(prefix);
				}
			}
		}
		
		/// <summary> Find the defualt namesapce for this context </summary>
		public string FindDefaultNamesapce()
		{
			RawElement current = this;
			while(current != null) {
				string namesapce = current.GetAttributeValue(NoNamespace, "xmlns");
				if (namesapce != null) return namesapce;
				current = current.Parent as RawElement;
			}
			return string.Empty; // No namesapce
		}
		
		/// <summary>
		/// Recursively resolve given prefix in this context.  Prefix must have some value.
		/// </summary>
		/// <returns> Empty string if prefix is not found </returns>
		public string ReslovePrefix(string prefix)
		{
			if (string.IsNullOrEmpty(prefix)) throw new ArgumentException("No prefix given", "prefix");
			
			// Implicit namesapces
			if (prefix == "xml") return XmlNamespace;
			if (prefix == "xmlns") return XmlnsNamespace;
			
			RawElement current = this;
			while(current != null) {
				string namesapce = current.GetAttributeValue(XmlnsNamespace, prefix);
				if (namesapce != null) return namesapce;
				current = current.Parent as RawElement;
			}
			return NoNamespace; // Can not find prefix
		}
		
		/// <summary>
		/// Get unquoted value of attribute.
		/// It looks in the no namespace (empty string).
		/// </summary>
		/// <returns>Null if not found</returns>
		public string GetAttributeValue(string localName)
		{
			return GetAttributeValue(NoNamespace, localName);
		}
		
		/// <summary>
		/// Get unquoted value of attribute
		/// </summary>
		/// <param name="namespace">Namespace.  Can be no namepace (empty string), which is the default for attributes.</param>
		/// <param name="localName">Local name - text after ":"</param>
		/// <returns>Null if not found</returns>
		public string GetAttributeValue(string @namespace, string localName)
		{
			@namespace = @namespace ?? string.Empty;
			foreach(RawAttribute attr in this.Attributes.GetByLocalName(localName)) {
				DebugAssert(attr.LocalName == localName, "Bad hashtable");
				if (attr.Namespace == @namespace) {
					return attr.Value;
				}
			}
			return null;
		}
		
		#endregion
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitElement(this);
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5} Nest:{6}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Children.Count, this.Children.Count, this.IsProperlyNested ? "Ok" : "Bad");
		}
	}
	
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
	
	static class ExtensionMethods
	{
		// Copied from ICSharpCode.SharpDevelop.Dom.ExtensionMethods
		/// <summary>
		/// Converts a recursive data structure into a flat list.
		/// </summary>
		/// <param name="input">The root elements of the recursive data structure.</param>
		/// <param name="recursion">The function that gets the children of an element.</param>
		/// <returns>Iterator that enumerates the tree structure in preorder.</returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
		{
			Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
			try {
				stack.Push(input.GetEnumerator());
				while (stack.Count > 0) {
					while (stack.Peek().MoveNext()) {
						T element = stack.Peek().Current;
						yield return element;
						IEnumerable<T> children = recursion(element);
						if (children != null) {
							stack.Push(children.GetEnumerator());
						}
					}
					stack.Pop().Dispose();
				}
			} finally {
				while (stack.Count > 0) {
					stack.Pop().Dispose();
				}
			}
		}
	}
}
