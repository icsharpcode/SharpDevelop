// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Describes how a placement is done.
	/// </summary>
	public sealed class PlacementType
	{
		/// <summary>
		/// Placement is done by Moving a inner Point (for Example on Path, Line, ...)
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType MovePoint = Register("MovePoint");

		/// <summary>
		/// Placement is done by resizing an element in a drag'n'drop operation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Resize = Register("Resize");
		
		/// <summary>
		/// Placement is done by moving an element in a drag'n'drop operation.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Move = Register("Move");
		
		/// <summary>
		/// Adding an element to a specified position in the container.
		/// AddItem is used when dragging a toolbox item to the design surface.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType AddItem = Register("AddItem");
		
		/// <summary>
		/// Not a "real" placement, but deleting the element.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType Delete = Register("Delete");
		
		/// <summary>
		/// Inserting from Cliboard
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly PlacementType PasteItem = Register("PasteItem");

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
		
		/// <summary>
		/// Gets the name used to register this PlacementType.
		/// </summary>
		public override string ToString()
		{
			return name;
		}
	}
}
