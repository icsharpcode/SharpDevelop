// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary> Holds event args for event caused by <see cref="RawObject"/> </summary>
	public class RawObjectEventArgs: EventArgs
	{
		/// <summary> The object that cause the event </summary>
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
		
		/// <summary>
		/// Gets the document owning this object or null if orphaned
		/// </summary>
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
			Assert(error.Object == this);
			if (this.syntaxErrors == null) this.syntaxErrors = new List<SyntaxError>();
			syntaxErrors.Add(error);
		}
		
		/// <summary> Create new object </summary>
		protected RawObject()
		{
			this.ReadCallID = new object();
		}
		
		/// <summary> Throws exception if condition is false </summary>
		[Conditional("Debug")]
		protected static void Assert(bool condition)
		{
			if (!condition) {
				throw new Exception("Assertion failed");
			}
		}
		
		/// <summary> Throws exception if condition is false </summary>
		[Conditional("Debug")]
		protected static void Assert(bool condition, string message)
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
		
		/// <summary> Call appropriate visit method on the given visitor </summary>
		public abstract void AcceptVisitor(IXmlVisitor visitor);
		
		/// <summary> Copy all data from the 'source' to this object </summary>
		internal virtual void UpdateDataFrom(RawObject source)
		{
			this.ReadCallID = source.ReadCallID;
			// In some cases we are just updating objects of that same
			// type and sequential position hoping to be luckily right
			this.StartOffset = source.StartOffset;
			this.EndOffset = source.EndOffset;
			
			// Do not bother comparing - assume changed if non-null
			if (this.syntaxErrors != null || source.syntaxErrors != null) {
				// May be called again in derived class - oh, well, nevermind
				OnChanging();
				this.syntaxErrors = new List<SyntaxError>();
				foreach(var error in source.SyntaxErrors) {
					// The object differs, so create our own copy
					// The source still might need it in the future and we do not want to break it
					this.AddSyntaxError(error.Clone(this));
				}
				OnChanged();
			}
		}
		
		[Conditional("Debug")]
		internal virtual void CheckConsistency()
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
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
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
		
		/// <summary> The part of name after ":".  Whole name if not found </summary>
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
		internal override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
			RawContainer src = (RawContainer)source;
			UpdateChildrenFrom(src.Children);
		}
		
		/// <inheritdoc/>
		public override IEnumerable<RawObject> GetSelfAndAllChildren()
		{
			return Enumerable.Union(
				new RawContainer[] { this },
				this.Children.SelectMany(x => x.GetSelfAndAllChildren())
			);
		}
		
		/// <summary>
		/// Gets a child fully containg the given offset.
		/// Goes recursively down the tree.
		/// Specail case if at the end of attribute
		/// </summary>
		public RawObject GetChildAtOffset(int offset)
		{
			foreach(RawObject child in this.Children) {
				if (child is RawAttribute && offset == child.EndOffset) return child;
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
				EnsureOwing(document, item, attachedObjects);
			}
			
			// Add it
			this.Children.InsertItems(index, items);
			
			// Notify document - do last so that the handler sees up-to-date tree
			foreach(RawObject item in attachedObjects)  {
				document.OnObjectAttached(item);
			}
		}
		
		/// <summary>
		/// Make sure that you own the item and no-one else does
		/// </summary>
		void EnsureOwing(RawDocument myDocument, RawObject item, List<RawObject> attachedObjects)
		{
			if (item.Document == null) {
				// TODO: Maybe taking only part of dangling tree
				// Dangling object - it was probably just removed from the document during update
				LogDom("Inserting dangling {0}", item);
				attachedObjects.AddRange(item.GetSelfAndAllChildren());
				item.Parent = this;
			} else if (item.Document.IsParsed) {
				// Adding from parser tree - steal pointer; keep in the parser tree
				LogDom("Inserting {0} from parser tree", item);
				attachedObjects.Add(item);
				if (item is RawContainer) {
					foreach(RawObject child in ((RawContainer)item).Children) {
						((RawContainer)item).EnsureOwing(myDocument, child, attachedObjects);
					}
				}
				item.Parent = this;  // Do after recursion so the Document.IsParser == true for children
			} else {
				// Adding from user other document location
				Assert(item.Document == myDocument);  // The parser was reusing object from other document?
				LogDom("Inserting {0} from other document location", item);
				// Remove from other location
				var owingList = ((RawContainer)item.Parent).Children;
				owingList.RemoveItems(owingList.IndexOf(item), 1);
				// No detach / attach notifications
				item.Parent = this;
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
		
		internal override void CheckConsistency()
		{
			base.CheckConsistency();
			foreach(RawObject child in this.Children) {
				Assert(child.Parent != null, "Null parent reference");
				Assert(child.Parent == this, "Inccorect parent reference");
				child.CheckConsistency();
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
		/// <summary>
		/// Parser tree (as opposed to some user tree).
		/// Parser tree can reuse data of other trees as long as it does not modify them
		/// (that, it can not set parent pointer if non-null)
		/// </summary>
		internal bool IsParsed { get; set; }
		
		/// <summary>
		/// All syntax errors in the document
		/// </summary>
		public new TextSegmentCollection<SyntaxError> SyntaxErrors { get; private set; }
		
		/// <summary> Occurs when object is added to the document </summary>
		public event EventHandler<RawObjectEventArgs> ObjectAttached;
		/// <summary> Occurs when object is removed from the document </summary>
		public event EventHandler<RawObjectEventArgs> ObjectDettached;
		/// <summary> Occurs before local data of object changes </summary>
		public event EventHandler<RawObjectEventArgs> ObjectChanging;
		/// <summary> Occurs after local data of object changed </summary>
		public event EventHandler<RawObjectEventArgs> ObjectChanged;
		
		/// <summary> The total number of objects in this document including the document itself </summary>
		public int ObjectCount { get; private set; }
		
		internal void OnObjectAttached(RawObject obj)
		{
			Assert(!IsParsed);
			ObjectCount++;
			foreach(SyntaxError error in obj.SyntaxErrors) {
				this.SyntaxErrors.Add(error);
			}
			if (ObjectAttached != null) ObjectAttached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectDettached(RawObject obj)
		{
			Assert(!IsParsed);
			ObjectCount--;
			foreach(SyntaxError error in obj.SyntaxErrors) {
				this.SyntaxErrors.Remove(error);
			}
			if (ObjectDettached != null) ObjectDettached(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectChanging(RawObject obj)
		{
			Assert(!IsParsed);
			foreach(SyntaxError error in obj.SyntaxErrors) {
				this.SyntaxErrors.Remove(error);
			}
			if (ObjectChanging != null) ObjectChanging(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		internal void OnObjectChanged(RawObject obj)
		{
			Assert(!IsParsed);
			foreach(SyntaxError error in obj.SyntaxErrors) {
				this.SyntaxErrors.Add(error);
			}
			if (ObjectChanged != null) ObjectChanged(this, new RawObjectEventArgs() { Object = obj } );
		}
		
		/// <summary> Create new document </summary>
		public RawDocument()
		{
			this.SyntaxErrors = new TextSegmentCollection<SyntaxError>();
		}
		
		internal override void CheckConsistency()
		{
			base.CheckConsistency();
			Assert(ObjectCount == this.GetSelfAndAllChildren().Count(), "Number of attach/detach calls incorrect");
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
		public string OpeningBracket { get; set; }
		/// <summary> Name following the opening bracket </summary>
		public string Name { get; set; }
		/// <summary> Opening bracket - usually "&gt;" </summary>
		public string ClosingBracket { get; set; }
		
		// Exactly one of the folling will be true
		
		/// <summary> True if tag starts with "&lt;" </summary>
		public bool IsStartTag              { get { return OpeningBracket == "<"; } }
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
		
		internal override void CheckConsistency()
		{
			Assert(OpeningBracket != null);
			Assert(Name != null);
			Assert(ClosingBracket != null);
			base.CheckConsistency();
		}
		
		/// <inheritdoc/>
		public override void AcceptVisitor(IXmlVisitor visitor)
		{
			visitor.VisitTag(this);
		}
		
		/// <inheritdoc/>
		internal override void UpdateDataFrom(RawObject source)
		{
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
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
		
		internal override void CheckConsistency()
		{
			Assert(Children.Count > 0, "No children");
			base.CheckConsistency();
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
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
		public string Prefix {
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
		
		/// <summary> Resolved namespace of the name.  Empty string if prefix not found </summary>
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
		
		/// <summary>
		/// Find the defualt namesapce for this context
		/// </summary>
		public string FindDefaultNamesapce()
		{
			RawElement current = this;
			while(current != null) {
				string namesapce = current.GetAttributeValue(NoNamespace, "xmlns");
				if (namesapce != null) return namesapce;
				current = current.Parent as RawElement;
			}
			return string.Empty; // "" namesapce
		}
		
		/// <summary>
		/// Recursively resolve given prefix in this context.  Prefix must have some value.
		/// Returns empty string if prefix is not found.
		/// </summary>
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
		/// Get unquoted value of attribute or null if not found.
		/// It looks in the empty "" namespace.
		/// </summary>
		public string GetAttributeValue(string localName)
		{
			return GetAttributeValue(NoNamespace, localName);
		}
		
		/// <summary>
		/// Get unquoted value of attribute or null if not found
		/// </summary>
		/// <param name="namespace">Namespace.  Can be the "" no namepace, which is default for attributes.</param>
		/// <param name="localName">Local name - text after ":"</param>
		/// <returns></returns>
		public string GetAttributeValue(string @namespace, string localName)
		{
			@namespace = @namespace ?? string.Empty;
			foreach(RawAttribute attr in this.Attributes.GetByLocalName(localName)) {
				Assert(attr.LocalName == localName);
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
			return string.Format("[{0} '{1}{2}{3}' Attr:{4} Chld:{5}]", base.ToString(), this.StartTag.OpeningBracket, this.StartTag.Name, this.StartTag.ClosingBracket, this.StartTag.Children.Count, this.Children.Count);
		}
	}
	
	/// <summary>
	/// Name-value pair in a tag
	/// </summary>
	public class RawAttribute: RawObject
	{
		/// <summary> Name with namespace prefix - exactly as in source file </summary>
		public string Name { get; set; }
		/// <summary> Equals sign and surrounding whitespace </summary>
		public string EqualsSign { get; set; }
		/// <summary> The raw value - exactly as in source file (*probably* quoted and escaped) </summary>
		public string QuotedValue { get; set; }
		/// <summary> Unquoted and dereferenced value of the attribute </summary>
		public string Value { get; set; }
		
		internal override void CheckConsistency()
		{
			Assert(Name != null);
			Assert(EqualsSign != null);
			Assert(QuotedValue != null);
			base.CheckConsistency();
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
		
		/// <summary> The part of name before ":".  Empty string if not found </summary>
		public string Prefix {
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
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
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
			if (this.ReadCallID == source.ReadCallID) return;
			base.UpdateDataFrom(source);
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
}
