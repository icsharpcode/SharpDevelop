// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	///   A collection that stores <see cref='QualifiedName'/> objects.
	/// </summary>
	[Serializable()]
	public class QualifiedNameCollection : Collection<QualifiedName> {	
		/// <summary>
		///   Initializes a new instance of <see cref='QualifiedNameCollection'/>.
		/// </summary>
		public QualifiedNameCollection()
		{
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='QualifiedNameCollection'/> based on another <see cref='QualifiedNameCollection'/>.
		/// </summary>
		/// <param name='val'>
		///   A <see cref='QualifiedNameCollection'/> from which the contents are copied
		/// </param>
		public QualifiedNameCollection(QualifiedNameCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///   Initializes a new instance of <see cref='QualifiedNameCollection'/> containing any array of <see cref='QualifiedName'/> objects.
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='QualifiedName'/> objects with which to intialize the collection
		/// </param>
		public QualifiedNameCollection(QualifiedName[] val)
		{
			this.AddRange(val);
		}
		
		public override string ToString()
		{
			string text = String.Empty;
			
			for (int i = 0; i < this.Count; i++) {
				text += (i == 0) ? this[i] + "" : " > " + this[i];
			}
			     
			return text;
		}
		
		/// <summary>
		///   Copies the elements of an array to the end of the <see cref='QualifiedNameCollection'/>.
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='QualifiedName'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='QualifiedNameCollection.Add'/>
		public void AddRange(QualifiedName[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///   Adds the contents of another <see cref='QualifiedNameCollection'/> to the end of the collection.
		/// </summary>
		/// <param name='val'>
		///    A <see cref='QualifiedNameCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <seealso cref='QualifiedNameCollection.Add'/>
		public void AddRange(QualifiedNameCollection val)
		{
			for (int i = 0; i < val.Count; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		/// Removes the last item in this collection.
		/// </summary>
		public void RemoveLast()
		{
			if (Count > 0) {
				RemoveAt(Count - 1);
			}
		}
		
		/// <summary>
		/// Removes the first item in the collection.
		/// </summary>
		public void RemoveFirst()
		{
			if (Count > 0) {
				RemoveAt(0);
			}
		}
		
		/// <summary>
		/// Gets the namespace prefix of the last item.
		/// </summary>
		public string LastPrefix {
			get {
				if (Count > 0) {
					QualifiedName name = this[Count - 1];
					return name.Prefix;
				}
				return String.Empty;
			}
		}
	}
}
