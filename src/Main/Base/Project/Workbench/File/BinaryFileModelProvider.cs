// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class BinaryFileModelProvider : IFileModelProvider<IBinaryFileModel>
	{
		sealed class OnDiskBinaryModel : IBinaryFileModel
		{
			internal readonly OpenedFile file;
			
			public OnDiskBinaryModel(OpenedFile file)
			{
				this.file = file;
			}
			
			public Stream OpenRead()
			{
				return SD.FileSystem.OpenRead(file.FileName);
			}
		}
		
		public IBinaryFileModel Load(OpenedFile file)
		{
			return new OnDiskBinaryModel(file);
		}
		
		public void Save(OpenedFile file, IBinaryFileModel model)
		{
			SaveCopyAs(file, model, file.FileName);
			file.ReplaceModel(this, model, ReplaceModelMode.SetAsValid); // remove dirty flag
		}
		
		public void SaveCopyAs(OpenedFile file, IBinaryFileModel model, FileName outputFileName)
		{
			var onDisk = model as OnDiskBinaryModel;
			if (onDisk != null) {
				// We can just copy the file (but avoid copying to itself)
				if (onDisk.file.FileName != outputFileName) {
					SD.FileSystem.CopyFile(onDisk.file.FileName, outputFileName, true);
				}
			} else {
				using (var inputStream = model.OpenRead()) {
					using (var outputStream = SD.FileSystem.OpenWrite(outputFileName)) {
						inputStream.CopyTo(outputStream);
					}
				}
			}
		}
		
		public bool CanLoadFrom<U>(IFileModelProvider<U> otherProvider) where U : class
		{
			return false;
		}

		public void NotifyRename(OpenedFile file, IBinaryFileModel model, FileName oldName, FileName newName)
		{
		}
		
		public void NotifyStale(OpenedFile file, IBinaryFileModel model)
		{
		}
		
		public void NotifyUnloaded(OpenedFile file, IBinaryFileModel model)
		{
		}
	}
}
