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
using System.Runtime.Serialization;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Exception used when loading of a project fails.
	/// </summary>
	[Serializable()]
	public class ProjectLoadException : FormatException
	{
		public ProjectLoadException() : base()
		{
		}
		
		public ProjectLoadException(string message) : base(message)
		{
		}
		
		public ProjectLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ProjectLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		
		public virtual void WriteToOutputPad(IOutputCategory category)
		{
			category.AppendLine(this.Message);
		}
		
		/// <summary>
		/// Gets whether the exception supports showing a dialog that has additional information
		/// not contained in the error message.
		/// </summary>
		public virtual bool CanShowDialog {
			get {
				return false;
			}
		}
		
		public virtual void ShowDialog()
		{
		}
	}
	
	[Serializable()]
	public class ToolNotFoundProjectLoadException : ProjectLoadException
	{
		/// <summary>
		/// The description text
		/// </summary>
		public string Description { get; set; }
		
		/// <summary>
		/// The link target (with leading http://)
		/// </summary>
		public string LinkTarget { get; set; }
		
		/// <summary>
		/// 32x32 icon to display next to the description. May be null.
		/// </summary>
		public IImage Icon { get; set; }
		
		public ToolNotFoundProjectLoadException() : base()
		{
		}
		
		public ToolNotFoundProjectLoadException(string message) : base(message)
		{
		}
		
		public ToolNotFoundProjectLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ToolNotFoundProjectLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		
		public override bool CanShowDialog {
			get { return true; }
		}
		
		public override void ShowDialog()
		{
			using (var dlg = new ToolNotFoundDialog(this.Description, this.LinkTarget, this.Icon)) {
				dlg.ShowDialog();
			}
		}
	}
}
