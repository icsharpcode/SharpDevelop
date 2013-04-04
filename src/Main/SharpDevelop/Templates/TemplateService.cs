// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	sealed class TemplateService : ITemplateService
	{
		const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		Lazy<IReadOnlyList<TemplateBase>> projectAndFileTemplates;
		Lazy<IReadOnlyList<TextTemplateGroup>> textTemplates;
		
		public TemplateService()
		{
			projectAndFileTemplates = new Lazy<IReadOnlyList<TemplateBase>>(LoadProjectAndFileTemplates);
		}
		
		public IEnumerable<FileTemplate> FileTemplates {
			get { return projectAndFileTemplates.Value.OfType<FileTemplate>(); }
		}
		
		public IEnumerable<ProjectTemplate> ProjectTemplates {
			get { return projectAndFileTemplates.Value.OfType<ProjectTemplate>(); }
		}
		
		public void UpdateTemplates()
		{
			var newTemplates = LoadProjectAndFileTemplates();
			projectAndFileTemplates = new Lazy<IReadOnlyList<TemplateBase>>(() => newTemplates);
		}
		
		IReadOnlyList<TemplateBase> LoadProjectAndFileTemplates()
		{
			return SD.AddInTree.BuildItems<TemplateBase>(TemplatePath, this, false);
		}
		
		public IEnumerable<TextTemplateGroup> TextTemplates {
			get { return textTemplates.Value; }
		}
		
		IReadOnlyList<TextTemplateGroup> LoadTextTemplates()
		{
			var dir = PropertyService.DataDirectory.Combine(DirectoryName.Create("options/textlib"));
			return SD.FileSystem.GetFiles(dir, "*.xml")
				.Select(file => TextTemplateGroup.Load(file))
				.ToList();
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
