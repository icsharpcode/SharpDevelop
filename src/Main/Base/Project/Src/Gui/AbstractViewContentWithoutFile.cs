// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Base class for view contents that are not based on a file.
	/// </summary>
	public abstract class AbstractViewContentWithoutFile : AbstractViewContent
	{
		public override bool IsViewOnly {
			get { return false; }
		}
		
		[Obsolete("AbstractViewContentWithoutFile.PrimaryFile is always null")]
		public override OpenedFile PrimaryFile { get { return null; } }
		
		[Obsolete("This method is not supported on an AbstractViewContentWithoutFile")]
		public sealed override void Load(OpenedFile file, System.IO.Stream stream)
		{
			throw new NotSupportedException();
		}
		
		[Obsolete("This method is not supported on an AbstractViewContentWithoutFile")]
		public sealed override void Save(OpenedFile file, System.IO.Stream stream)
		{
			throw new NotSupportedException();
		}
		
		/// <summary>
		/// Load the view content.
		/// </summary>
		public abstract void Load();
		
		/// <summary>
		/// Save the view content.
		/// </summary>
		public abstract void Save();
	}
}
