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
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// View content base class that is compatible with the old SD-1234 OpenedFile model.
	/// </summary>
	public abstract class AbstractViewContentSD1234 : AbstractViewContent
	{
		// The old behavior where view contents are loaded/saved without the use of a model instance is achieved by
		// making the IViewContent itself the model. 
		class Provider : IFileModelProvider<AbstractViewContentSD1234>
		{
			AbstractViewContentSD1234 IFileModelProvider<AbstractViewContentSD1234>.Load(OpenedFile file)
			{
				throw new NotSupportedException();
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.Save(OpenedFile file, AbstractViewContentSD1234 model, FileSaveOptions options)
			{
				MemoryStream ms = new MemoryStream();
				model.Save(file, ms);
				file.ReplaceModel(FileModels.Binary, new BinaryFileModel(ms.ToArray()), ReplaceModelMode.TransferDirty);
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.SaveCopyAs(OpenedFile file, AbstractViewContentSD1234 model, FileName outputFileName, FileSaveOptions options)
			{
				using (Stream s = SD.FileSystem.OpenWrite(outputFileName)) {
					model.Save(file, s);
				}
			}
			
			bool IFileModelProvider<AbstractViewContentSD1234>.CanLoadFrom<U>(IFileModelProvider<U> otherProvider)
			{
				return false;
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.NotifyRename(OpenedFile file, AbstractViewContentSD1234 model, FileName oldName, FileName newName)
			{
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.NotifyStale(OpenedFile file, AbstractViewContentSD1234 model)
			{
				model.isStale = true;
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.NotifyLoaded(OpenedFile file, AbstractViewContentSD1234 model)
			{
			}
			
			void IFileModelProvider<AbstractViewContentSD1234>.NotifyUnloaded(OpenedFile file, AbstractViewContentSD1234 model)
			{
			}
		}
		
		Provider provider = new Provider(); // each AbstractViewContent gets its own provider instance
		bool isStale = true;
		
		public abstract void Save(OpenedFile file, Stream stream);
		public abstract void Load(OpenedFile file, Stream stream);
		
		public override void LoadModel()
		{
			base.LoadModel();
			if (isStale) {
				foreach (var file in this.Files) {
					using (Stream s = file.GetModel(FileModels.Binary).OpenRead()) {
						Load(file, s);
					}
					file.ReplaceModel(provider, this, ReplaceModelMode.TransferDirty);
				}
				isStale = false;
			}
		}
		
		internal override void OnFileRemoved(OpenedFile oldItem)
		{
			// When a file is removed from the Files collection, unload our 'model' to remove the reference to the view content. 
			oldItem.UnloadModel(provider);
			// This method also gets called for each OpenedFile when the view content is disposed; so we don't need to override Dispose for cleanup.
			base.OnFileRemoved(oldItem);
		}
	}
}
