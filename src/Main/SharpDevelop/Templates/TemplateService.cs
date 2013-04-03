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
		
		IReadOnlyList<TemplateBase> LoadTemplates()
		{
			return SD.AddInTree.BuildItems<TemplateBase>(TemplatePath, this, false);
		}
		
		public TemplateBase LoadTemplate(FileName fileName)
		{
			var fileSystem = SD.FileSystem;
			using (TextReader reader = fileSystem.OpenText(fileName)) {
				return LoadTemplate(reader, new ReadOnlyChrootFileSystem(fileSystem, fileName.GetParentDirectory()));
			}
		}
		
		public TemplateBase LoadTemplate(TextReader textReader, IReadOnlyFileSystem fileSystem)
		{
			try {
				throw new NotImplementedException();
			} catch (XmlException ex) {
				throw new TemplateLoadException(ex.Message, ex);
			}
		}
	}
}
