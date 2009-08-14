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
}
