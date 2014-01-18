// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class BinaryFileModelProvider : IFileModelProvider<IBinaryModel>
	{
		sealed class OnDiskBinaryModel : IBinaryModel
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
		
		public IBinaryModel Load(OpenedFile file)
		{
			return new OnDiskBinaryModel(file);
		}
		
		public void Save(OpenedFile file, IBinaryModel model)
		{
			SaveCopyAs(file, model, file.FileName);
			file.ReplaceModel(this, model, ReplaceModelMode.SetAsValid); // remove dirty flag
		}
		
		public void SaveCopyAs(OpenedFile file, IBinaryModel model, FileName outputFileName)
		{
			var onDisk = model as OnDiskBinaryModel;
			if (onDisk != null) {
				// We can just copy the file (but avoid copying to itself)
				if (onDisk.file.FileName != outputFileName) {
					SD.FileSystem.CopyFile(onDisk.file.FileName, outputFileName);
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

		public void NotifyRename(OpenedFile file, IBinaryModel model, FileName oldName, FileName newName)
		{
		}
		
		public void NotifyStale(OpenedFile file, IBinaryModel model)
		{
		}
		
		public void NotifyUnloaded(OpenedFile file, IBinaryModel model)
		{
		}
	}
}
