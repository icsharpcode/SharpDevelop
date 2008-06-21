// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Struct for strongly-typed passing of item types
	/// - we don't want to use strings everywhere.
	/// Basically this is something like a typedef for C# (without implicit conversions).
	/// </summary>
	public struct ItemType : IEquatable<ItemType>, IComparable<ItemType>
	{
		// ReferenceProjectItem
		public static readonly ItemType Reference = new ItemType("Reference");
		public static readonly ItemType ProjectReference = new ItemType("ProjectReference");
		public static readonly ItemType COMReference = new ItemType("COMReference");
		
		/// <summary>
		/// Item type for imported VB namespaces
		/// </summary>
		public static readonly ItemType Import = new ItemType("Import");
		
		public static readonly ItemType WebReferenceUrl = new ItemType("WebReferenceUrl");
		
		// FileProjectItem
		public static readonly ItemType Compile = new ItemType("Compile");
		public static readonly ItemType EmbeddedResource = new ItemType("EmbeddedResource");
		public static readonly ItemType None = new ItemType("None");
		public static readonly ItemType Content = new ItemType("Content");
		public static readonly ItemType ApplicationDefinition = new ItemType("ApplicationDefinition");
		public static readonly ItemType Page = new ItemType("Page");
		public static readonly ItemType BootstrapperFile = new ItemType("BootstrapperFile");
		public static readonly ItemType Header = new ItemType("Header");
		
		/// <summary>
		/// Gets a collection of item types that are used for files.
		/// </summary>
		public static readonly ReadOnlyCollectionWrapper<ItemType> DefaultFileItems
			= new Set<ItemType>(Compile, EmbeddedResource, None, Content).AsReadOnly();
		
		public static readonly ItemType Resource = new ItemType("Resource");
		public static readonly ItemType Folder = new ItemType("Folder");
		public static readonly ItemType WebReferences = new ItemType("WebReferences");
		
		
		readonly string itemName;
		
		public string ItemName {
			get { return itemName; }
		}
		
		public ItemType(string itemName)
		{
			if (itemName == null)
				throw new ArgumentNullException("itemName");
			this.itemName = itemName;
		}
		
		public override string ToString()
		{
			return itemName;
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<ItemType>" declaration.
		
		public override bool Equals(object obj)
		{
			if (obj is ItemType)
				return Equals((ItemType)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(ItemType other)
		{
			// add comparisions for all members here
			return this.itemName == other.itemName;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return itemName.GetHashCode();
		}
		
		public static bool operator ==(ItemType lhs, ItemType rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(ItemType lhs, ItemType rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		#endregion
		
		public int CompareTo(ItemType other)
		{
			return itemName.CompareTo(other.itemName);
		}
	}
}
