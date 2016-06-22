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
		/// <param name="displayName">The title of the category. This is parsed using StringParser and shown to the user.</param>
		IOutputCategory CreateCategory(string displayName);
		
		/// <summary>
		/// Gets an output category, or creates a new output category if no category with the given
		/// title already exists.
		/// </summary>
		/// <param name="displayName">The title of the category. This is parsed using StringParser and shown to the user.</param>
		IOutputCategory GetOrCreateCategory(string displayName);
		
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
		/// May contain StringParser-tags ($res) for localization; these will be replaced
		/// by the UI showing the display name.
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
