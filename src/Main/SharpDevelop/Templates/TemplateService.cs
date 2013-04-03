// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Templates
{
	sealed class TemplateService : ITemplateService
	{
		const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		Lazy<IReadOnlyList<TemplateBase>> allTemplates;
		
		public TemplateService()
		{
			allTemplates = new Lazy<IReadOnlyList<TemplateBase>>(LoadTemplates);
		}
		
		public IEnumerable<FileTemplate> FileTemplates {
			get { return allTemplates.Value.OfType<FileTemplate>(); }
		}
		
		public IEnumerable<ProjectTemplate> ProjectTemplates {
			get { return allTemplates.Value.OfType<ProjectTemplate>(); }
		}
		
		public void UpdateTemplates()
		{
			var newTemplates = LoadTemplates();
			allTemplates = new Lazy<IReadOnlyList<TemplateBase>>(() => newTemplates);
		}
		
		IReadOnlyList<TemplateBase> LoadTemplates()
		{
			return SD.AddInTree.BuildItems<TemplateBase>(TemplatePath, this, false);
		}
		
		public TemplateBase LoadTemplate(FileName fileName)
		{
			var fileSystem = SD.FileSystem;
			using (Stream stream = fileSystem.OpenRead(fileName)) {
				return LoadTemplate(stream, new ReadOnlyChrootFileSystem(fileSystem, fileName.GetParentDirectory()));
			}
		}
		
		public TemplateBase LoadTemplate(Stream stream, IReadOnlyFileSystem fileSystem)
		{
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);
				if (doc.DocumentElement["Files"] != null)
					return new FileTemplateImpl(doc, fileSystem);
				else if (doc.DocumentElement["Project"] != null || doc.DocumentElement["Solution"] != null)
					return new ProjectTemplateImpl(doc, fileSystem);
				else
					throw new TemplateLoadException("Unknown file format");
			} catch (XmlException ex) {
				throw new TemplateLoadException(ex.Message, ex);
			}
		}
	}
}
