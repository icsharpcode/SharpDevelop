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
	/// Base class for change groups.
	/// </summary>
	public abstract class ChangeGroup : IDisposable
	{
		string title;
		
		/// <summary>
		/// Gets/Sets the title of the change group.
		/// </summary>
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		/// <summary>
		/// Commits the change group.
		/// </summary>
		public abstract void Commit();
		
		/// <summary>
		/// Aborts the change group.
		/// </summary>
		public abstract void Abort();
		
		/// <summary>
		/// Is called when the change group is disposed. Should Abort the change group if it is not already committed.
		/// </summary>
		protected abstract void Dispose();
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		void IDisposable.Dispose()
		{
			Dispose();
		}
	}
}
