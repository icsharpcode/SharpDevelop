// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.SharpDevelop.Templates
{
	sealed class TemplateService : ITemplateService
	{
		const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		Lazy<IReadOnlyList<TemplateCategory>> templateCategories;
		Lazy<IReadOnlyList<TextTemplateGroup>> textTemplates;
		
		public TemplateService()
		{
			templateCategories = new Lazy<IReadOnlyList<TemplateCategory>>(LoadProjectAndFileTemplates);
			textTemplates = new Lazy<IReadOnlyList<TextTemplateGroup>>(LoadTextTemplates);
		}
		
		public IReadOnlyList<TemplateCategory> TemplateCategories {
			get { return templateCategories.Value; }
		}
		
		public void UpdateTemplates()
		{
			var newTemplates = LoadProjectAndFileTemplates();
			templateCategories = new Lazy<IReadOnlyList<TemplateCategory>>(() => newTemplates);
		}
		
		IReadOnlyList<TemplateCategory> LoadProjectAndFileTemplates()
		{
			var items = SD.AddInTree.BuildItems<TemplateBase>(TemplatePath, this, false);
			var categories = items.OfType<TemplateCategory>().ToList();
			foreach (var classicTemplate in items.Except(categories)) {
				// compatibility with the SD <=4.x way of adding templates:
				// define category+subcategory in the .xft/.xpt file,
				// and the category gets created automatically
				ICategory classicCategory = (ICategory)classicTemplate;
				var cat = GetOrCreateClassicCategory(categories, classicCategory.Category);
				if (!string.IsNullOrEmpty(classicCategory.Subcategory))
					cat = GetOrCreateClassicCategory(cat.Subcategories, classicCategory.Subcategory);
				cat.Templates.Add(classicTemplate);
			}
			return categories;
		}
		
		TemplateCategory GetOrCreateClassicCategory(IList<TemplateCategory> categories, string name)
		{
			var cat = categories.FirstOrDefault(c => c.Name == name);
			if (cat == null) {
				cat = new TemplateCategory(name);
				categories.Add(cat);
			}
			return cat;
		}
		
		public IEnumerable<TextTemplateGroup> TextTemplates {
			get { return textTemplates.Value; }
		}
		
		IReadOnlyList<TextTemplateGroup> LoadTextTemplates()
		{
			var dir = SD.PropertyService.DataDirectory.CombineDirectory("options/textlib");
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
