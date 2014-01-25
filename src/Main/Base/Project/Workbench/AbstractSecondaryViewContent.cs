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
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// A view content that is based on another (primary) view content.
	/// Loading works by getting the content from the primary view content,
	/// saving by merging it back to the primary view content and then saving the
	/// primary view content.
	/// AbstractSecondaryViewContent implements the [Support]Switch* methods to make switching
	/// between the primary view content and the secondary possible without having to save/load the
	/// primary view content.
	/// </summary>
	public abstract class AbstractSecondaryViewContent : AbstractViewContent
	{
		readonly IViewContent primaryViewContent;
		readonly OpenedFile primaryFile;
		
		public IViewContent PrimaryViewContent {
			get { return primaryViewContent; }
		}
		
		public sealed override OpenedFile PrimaryFile {
			get { return primaryFile; }
		}
		
		protected AbstractSecondaryViewContent(IViewContent primaryViewContent)
		{
			if (primaryViewContent == null)
				throw new ArgumentNullException("primaryViewContent");
			if (primaryViewContent.PrimaryFile == null)
				throw new ArgumentException("primaryViewContent.PrimaryFile must not be null");
			this.primaryViewContent = primaryViewContent;
			
			primaryFile = primaryViewContent.PrimaryFile;
			this.Files.Add(primaryFile);
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != this.PrimaryFile)
				throw new ArgumentException("file must be the primary file of the primary view content, override Load() to handle other files");
			primaryViewContent.Load(file, stream);
			LoadFromPrimary();
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != this.PrimaryFile)
				throw new ArgumentException("file must be the primary file of the primary view content, override Save() to handle other files");
			SaveToPrimary();
			primaryViewContent.Save(file, stream);
		}
		
		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			if (file == this.PrimaryFile)
				return newView.SupportsSwitchToThisWithoutSaveLoad(file, primaryViewContent);
			else
				return base.SupportsSwitchFromThisWithoutSaveLoad(file, newView);
		}
		
		public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			if (file == this.PrimaryFile)
				return oldView.SupportsSwitchToThisWithoutSaveLoad(file, primaryViewContent);
			else
				return base.SupportsSwitchFromThisWithoutSaveLoad(file, oldView);
		}
		
		public override void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			if (file == this.PrimaryFile && this != newView) {
				SaveToPrimary();
				primaryViewContent.SwitchFromThisWithoutSaveLoad(file, newView);
			}
		}
		
		public override void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			if (file == this.PrimaryFile && oldView != this) {
				primaryViewContent.SwitchToThisWithoutSaveLoad(file, oldView);
				LoadFromPrimary();
			}
		}
		
		protected abstract void LoadFromPrimary();
		protected abstract void SaveToPrimary();
		
		/// <summary>
		/// Gets the list of sibling secondary view contents.
		/// </summary>
		public override ICollection<IViewContent> SecondaryViewContents {
			get { return primaryViewContent.SecondaryViewContents; }
		}
	}
}
