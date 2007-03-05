// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Describes how a placement is done.
	/// </summary>
	public sealed class PlacementType
	{
		/// <summary>
		/// Placement is done by resizing an element.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Resize = Register("Resize");
		
		/// <summary>
		/// Placement is done by moving an element.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Move = Register("Move");
		
		/// <summary>
		/// Not a "real" placement, but deleting the element.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Delete = Register("Delete");
		
		readonly string name;
		
		private PlacementType(string name)
		{
			this.name = name;
		}
		
		/// <summary>
		/// Creates a new unique PlacementKind.
		/// </summary>
		/// <param name="name">The name to return from a ToString() call.
		/// Note that two PlacementTypes with the same name are NOT equal!</param>
		public static PlacementType Register(string name)
		{
			return new PlacementType(name);
		}
		
		/// <inherit/>
		public override string ToString()
		{
			return name;
		}
	}
}
