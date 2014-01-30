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

namespace ICSharpCode.SharpDevelop.Workbench
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
