// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Xml
{
	/// <summary>
	/// Specailized attribute collection with attribute name caching
	/// </summary>
	public class AXmlAttributeCollection: FilteredCollection<AXmlAttribute, AXmlObjectCollection<AXmlObject>>
	{
		/// <summary> Wrap the given collection.  Non-attributes are filtered </summary>
		public AXmlAttributeCollection(AXmlObjectCollection<AXmlObject> source): base(source) {}
		
		/// <summary> Wrap the given collection.  Non-attributes are filtered.  Items not matching the condition are filtered. </summary>
		public AXmlAttributeCollection(AXmlObjectCollection<AXmlObject> source, Predicate<object> condition): base(source, condition) {}
		
		Dictionary<string, List<AXmlAttribute>> hashtable = new Dictionary<string, List<AXmlAttribute>>();
		
		void AddToHashtable(AXmlAttribute attr)
		{
			string localName = attr.LocalName;
			if (!hashtable.ContainsKey(localName)) {
				hashtable[localName] = new List<AXmlAttribute>(1);
			}
			hashtable[localName].Add(attr);
		}
		
		void RemoveFromHashtable(AXmlAttribute attr)
		{
			string localName = attr.LocalName;
			hashtable[localName].Remove(attr);
		}
		
		static List<AXmlAttribute> NoAttributes = new List<AXmlAttribute>();
		
		/// <summary>
		/// Get all attributes with given local name.
		/// Hash table is used for lookup so this is cheap.
		/// </summary>
		public IEnumerable<AXmlAttribute> GetByLocalName(string localName)
		{
			if (hashtable.ContainsKey(localName)) {
				return hashtable[localName];
			} else {
				return NoAttributes;
			}
		}
		
		/// <inheritdoc/>
		protected override void ClearItems()
		{
			foreach(AXmlAttribute item in this) {
				RemoveFromHashtable(item);
				item.Changing -= item_Changing;
				item.Changed  -= item_Changed;
			}
			base.ClearItems();
		}
		
		/// <inheritdoc/>
		protected override void InsertItem(int index, AXmlAttribute item)
		{
			AddToHashtable(item);
			item.Changing += item_Changing;
			item.Changed  += item_Changed;
			base.InsertItem(index, item);
		}
		
		/// <inheritdoc/>
		protected override void RemoveItem(int index)
		{
			RemoveFromHashtable(this[index]);
			this[index].Changing -= item_Changing;
			this[index].Changed  -= item_Changed;
			base.RemoveItem(index);
		}
		
		/// <inheritdoc/>
		protected override void SetItem(int index, AXmlAttribute item)
		{
			throw new NotSupportedException();
		}
		
		// Every item in the collectoin should be registered to these handlers
		// so that we can handle renames
		
		void item_Changing(object sender, AXmlObjectEventArgs e)
		{
			RemoveFromHashtable((AXmlAttribute)e.Object);
		}
		
		void item_Changed(object sender, AXmlObjectEventArgs e)
		{
			AddToHashtable((AXmlAttribute)e.Object);
		}
	}
}
