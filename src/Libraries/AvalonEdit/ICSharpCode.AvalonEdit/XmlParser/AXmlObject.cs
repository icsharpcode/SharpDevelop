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
	/// Abstact base class for all types
	/// </summary>
	public abstract class AXmlObject: TextSegment
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
		public AXmlObject Parent { get; set; }
		
		/// <summary> Gets the document owning this object or null if orphaned </summary>
		public AXmlDocument Document {
			get {
				if (this.Parent != null) {
					return this.Parent.Document;
				} else if (this is AXmlDocument) {
					return (AXmlDocument)this;
				} else {
					return null;
				}
			}
		}
		
		/// <summary> Creates new object </summary>
		public AXmlObject()
		{
			this.LastUpdatedFrom = this;
		}
		
		/// <summary> Occurs before the value of any local properties changes.  Nested changes do not cause the event to occur </summary>
		public event EventHandler<AXmlObjectEventArgs> Changing;
		
		/// <summary> Occurs after the value of any local properties changed.  Nested changes do not cause the event to occur </summary>
		public event EventHandler<AXmlObjectEventArgs> Changed;
		
		/// <summary> Raises Changing event </summary>
		protected void OnChanging()
		{
			LogDom("Changing {0}", this);
			if (Changing != null) {
				Changing(this, new AXmlObjectEventArgs() { Object = this } );
			}
			AXmlDocument doc = this.Document;
			if (doc != null) {
				doc.OnObjectChanging(this);
			}
		}
		
		/// <summary> Raises Changed event </summary>
		protected void OnChanged()
		{
			LogDom("Changed {0}", this);
			if (Changed != null) {
				Changed(this, new AXmlObjectEventArgs() { Object = this } );
			}
			AXmlDocument doc = this.Document;
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
		public virtual IEnumerable<AXmlObject> GetSelfAndAllChildren()
		{
			return new AXmlObject[] { this };
		}
		
		/// <summary> Get all ancestors of this node </summary>
		public IEnumerable<AXmlObject> GetAncestors()
		{
			AXmlObject curr = this.Parent;
			while(curr != null) {
				yield return curr;
				curr = curr.Parent;
			}
		}
		
		/// <summary> Call appropriate visit method on the given visitor </summary>
		public abstract void AcceptVisitor(IAXmlVisitor visitor);
		
		/// <summary> The parser tree object this object was updated from </summary>
		internal object LastUpdatedFrom { get; private set; }
		
		internal bool IsInCache { get; set; }
		
		/// <summary> Is call to UpdateDataFrom is allowed? </summary>
		internal bool CanUpdateDataFrom(AXmlObject source)
		{
			return
				this.GetType() == source.GetType() &&
				this.StartOffset == source.StartOffset &&
				(this.LastUpdatedFrom == source || !this.IsInCache);
		}
		
		/// <summary> Copy all data from the 'source' to this object </summary>
		internal virtual void UpdateDataFrom(AXmlObject source)
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
}
