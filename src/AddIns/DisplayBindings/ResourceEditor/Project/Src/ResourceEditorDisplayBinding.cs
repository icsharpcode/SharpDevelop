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
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ResourceEditor.ViewModels;
using ResourceEditor.Views;

namespace ResourceEditor
{
	public class ResourceEditorDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true; // definition in .addin does extension-based filtering
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new ResourceEditViewContent(file);
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
	
	public class ResourceEditViewContent : AbstractViewContentHandlingLoadErrors
	{
		readonly ResourceEditorViewModel resourceEditor = new ResourceEditorViewModel();
		readonly ResourceEditorView resourceEditorView = new ResourceEditorView();
		
		public ResourceEditViewContent(OpenedFile file)
		{
			this.TabPageText = "Resource editor";
			resourceEditor.View = resourceEditorView;
			
			// Register different resource item viewers
			resourceEditor.AddItemView(ResourceItemEditorType.String, new TextView());
			resourceEditor.AddItemView(ResourceItemEditorType.Bitmap, new ImageViewBase());
			resourceEditor.AddItemView(ResourceItemEditorType.Icon, new ImageViewBase());
			resourceEditor.AddItemView(ResourceItemEditorType.Cursor, new ImageViewBase());
			resourceEditor.AddItemView(ResourceItemEditorType.Binary, new BinaryView());
			resourceEditor.AddItemView(ResourceItemEditorType.Boolean, new BooleanView());
			resourceEditor.DirtyStateChanged += (sender, e) => {
				if (e.IsDirty)
					SetDirty(sender, new EventArgs());
			};
			
			UserContent = resourceEditorView;
			this.Files.Add(file);
		}
		
		public ResourceEditorViewModel ResourceEditor {
			get { return resourceEditor; }
		}

		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		void SetDirty(object sender, EventArgs e)
		{
			PrimaryFile.MakeDirty();
		}
		
		protected override void LoadInternal(OpenedFile file, Stream stream)
		{
			resourceEditor.LoadFile(file.FileName, stream);
		}
		
		protected override void SaveInternal(OpenedFile file, Stream stream)
		{
			resourceEditor.SaveFile(file.FileName, stream);
		}
	}
}
