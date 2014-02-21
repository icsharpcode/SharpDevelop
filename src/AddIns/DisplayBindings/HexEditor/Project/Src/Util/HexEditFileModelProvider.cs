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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace HexEditor.Util
{
	public class HexEditFileModelProvider : IFileModelProvider<BufferManager>
	{
		static readonly HexEditFileModelProvider instance = new HexEditFileModelProvider();

		HexEditFileModelProvider()
		{
		}

		public static HexEditFileModelProvider Instance {
			get {
				return instance;
			}
		}

		public BufferManager Load(OpenedFile file)
		{
			SD.AnalyticsMonitor.TrackFeature(typeof(HexEditor.View.HexEditView), "Load");
			return new BufferManager(file);
		}

		public void Save(OpenedFile file, BufferManager model, FileSaveOptions options)
		{
			SD.AnalyticsMonitor.TrackFeature(typeof(HexEditor.View.HexEditView), "Save");
			MemoryStream ms = new MemoryStream();
			model.Save(ms);
			file.ReplaceModel(FileModels.Binary, new BinaryFileModel(ms.ToArray()));
		}

		public void SaveCopyAs(OpenedFile file, BufferManager model, FileName outputFileName, FileSaveOptions options)
		{
			using (var fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write)) {
				model.Save(fileStream);
			}
		}

		public bool CanLoadFrom<U>(IFileModelProvider<U> otherProvider) where U : class
		{
			return otherProvider == FileModels.Binary || FileModels.Binary.CanLoadFrom(otherProvider);
		}

		public void NotifyRename(OpenedFile file, BufferManager model, FileName oldName, FileName newName)
		{
		}

		public void NotifyStale(OpenedFile file, BufferManager model)
		{
			file.UnloadModel(this);
		}

		public void NotifyLoaded(OpenedFile file, BufferManager model)
		{
		}

		public void NotifyUnloaded(OpenedFile file, BufferManager model)
		{
		}
	}
}