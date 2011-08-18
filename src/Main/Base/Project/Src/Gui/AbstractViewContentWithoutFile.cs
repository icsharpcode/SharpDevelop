// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Base class for view contents that are not based on a file, but can be loaded/saved.
	/// If you need a view content that cannot save (IsViewOnly==true), you should instead derive
	/// directly from AbstractViewContent and leave the Files collection empty.
	/// 
	/// AbstractViewContentWithoutFile implements ICustomizedCommands to make "File > Save" work
	/// without requiring an OpenedFile. "File > Save as" will also cause Save() to be called, without
	/// showing a "Save as" dialog.
	/// </summary>
	public abstract class AbstractViewContentWithoutFile : AbstractViewContent, ICustomizedCommands
	{
		public override bool IsViewOnly {
			get { return false; }
		}
		
		[Obsolete("AbstractViewContentWithoutFile.PrimaryFile is always null")]
		public sealed override OpenedFile PrimaryFile { get { return null; } }
		
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
		
		bool ICustomizedCommands.SaveCommand()
		{
			Save();
			return true;
		}
		
		bool ICustomizedCommands.SaveAsCommand()
		{
			Save();
			return true;
		}
	}
}
