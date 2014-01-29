// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// The 'Output' pad.
	/// Allows showing a text log to the user.
	/// </summary>
	/// <remarks>This service is thread-safe.</remarks>
	[SDService("SD.OutputPad")]
	public interface IOutputPad
	{
		/// <summary>
		/// Opens the pad.
		/// </summary>
		void BringToFront();
		
		/// <summary>
		/// Creates a new output category.
		/// </summary>
		IOutputCategory CreateCategory(string displayName);
		
		/// <summary>
		/// Removes an existing output category.
		/// </summary>
		void RemoveCategory(IOutputCategory category);
		
		/// <summary>
		/// Gets/Sets the current category.
		/// This property is thread-safe.
		/// </summary>
		IOutputCategory CurrentCategory { get; set; }
		
		/// <summary>
		/// The "Build" category.
		/// </summary>
		IOutputCategory BuildCategory { get; }
	}
	
	/// <summary>
	/// Represents a category in the <see cref="IOutputPad"/>.
	/// </summary>
	/// <remarks>This interface is thread-safe.</remarks>
	public interface IOutputCategory
	{
		/// <summary>
		/// Gets the display name of this category.
		/// </summary>
		string DisplayName { get; }
		
		/// <summary>
		/// Activates this output category in the UI.
		/// </summary>
		void Activate(bool bringPadToFront = false);
		
		/// <summary>
		/// Clears all text in the category.
		/// </summary>
		void Clear();
		
		/// <summary>
		/// Appends text to this category.
		/// </summary>
		void AppendText(RichText text);
		
		/// <summary>
		/// Appends text to this category, followed by a newline.
		/// </summary>
		void AppendLine(RichText text);
	}
}
