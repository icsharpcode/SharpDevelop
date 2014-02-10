// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class XDocumentFileModelProvider : IFileModelProvider<XDocument>
	{
		public XDocument Load(OpenedFile file)
		{
			XDocument document;
			using (Stream stream = file.GetModel(FileModels.Binary).OpenRead()) {
				document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
			}
			document.Changed += delegate {
				file.MakeDirty(this);
			};
			return document;
		}
		
		public void Save(OpenedFile file, XDocument model, FileSaveOptions options)
		{
			MemoryStream ms = new MemoryStream();
			model.Save(ms, SaveOptions.DisableFormatting);
			file.ReplaceModel(FileModels.Binary, new BinaryFileModel(ms.ToArray()));
		}
		
		public void SaveCopyAs(OpenedFile file, XDocument model, FileName outputFileName, FileSaveOptions options)
		{
			model.Save(outputFileName, SaveOptions.DisableFormatting);
		}
		
		public bool CanLoadFrom<U>(IFileModelProvider<U> otherProvider) where U : class
		{
			return otherProvider == FileModels.Binary || FileModels.Binary.CanLoadFrom(otherProvider);
		}
		
		public void NotifyRename(OpenedFile file, XDocument model, FileName oldName, FileName newName)
		{
		}
		
		public void NotifyStale(OpenedFile file, XDocument model)
		{
		}
		
		public void NotifyUnloaded(OpenedFile file, XDocument model)
		{
		}
	}
}
