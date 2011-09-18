// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateRepository : IMvcTextTemplateRepository
	{
		string textTemplatesRootDirectory;
		IFileSystem fileSystem;
		
		public MvcTextTemplateRepository(string mvcAddInPath, IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			GetTextTemplatesRootDirectory(mvcAddInPath);
		}
		
		public MvcTextTemplateRepository(string mvcAddInPath)
			: this(mvcAddInPath, new FileSystem())
		{
		}
		
		public MvcTextTemplateRepository()
			: this(StringParser.GetValue("addinpath:ICSharpCode.AspNet.Mvc"))
		{
		}
		
		void GetTextTemplatesRootDirectory(string mvcAddInPath)
		{
			this.textTemplatesRootDirectory = Path.Combine(mvcAddInPath, "ItemTemplates");
		}
		
		public string GetMvcViewTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcViewTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
		
		public string GetMvcControllerTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcControllerTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
		
		public IEnumerable<MvcControllerTextTemplate> GetMvcControllerTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			string templateFileName = GetDefaultMvcControllerTextTemplateFileName(templateCriteria);
			yield return CreateEmptyControllerTemplate(templateFileName);
			yield return CreateEmptyReadWriteControllerTemplate(templateFileName);
		}
		
		string GetDefaultMvcControllerTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var defaultControllerTemplateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = templateCriteria.TemplateLanguage,
				TemplateName = "Controller",
				TemplateType = templateCriteria.TemplateType
			};
			return GetMvcControllerTextTemplateFileName(defaultControllerTemplateCriteria);
		}
		
		MvcControllerTextTemplate CreateEmptyControllerTemplate(string templateFileName)
		{
			return new MvcControllerTextTemplate() {
				Name = "Empty",
				Description = "Empty controller",
				FileName = templateFileName,
				AddActionMethods = false
			};
		}
		
		MvcControllerTextTemplate CreateEmptyReadWriteControllerTemplate(string templateFileName)
		{
			return new MvcControllerTextTemplate() {
				Name = "EmptyReadWrite",
				Description = "Controller with create, read, update and delete actions",
				FileName = templateFileName,
				AddActionMethods = true
			};
		}
		
		public IEnumerable<MvcViewTextTemplate> GetMvcViewTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			foreach (string templateFileName in GetMvcViewTemplateFileNamesInFolder(templateCriteria)) {
				yield return new MvcViewTextTemplate(templateFileName);
			}
		}
		
		IEnumerable<string> GetMvcViewTemplateFileNamesInFolder(MvcTextTemplateCriteria templateCriteria)
		{
			string templatePath = GetMvcViewTemplatePath(templateCriteria);
			return fileSystem.GetFiles(templatePath, "*.tt");
		}
		
		string GetMvcViewTemplatePath(MvcTextTemplateCriteria templateCriteria)
		{
			var emptyViewTemplateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = templateCriteria.TemplateLanguage,
				TemplateName = "Empty",
				TemplateType = templateCriteria.TemplateType
			};
			string emptyViewTemplateFileName = GetMvcViewTextTemplateFileName(emptyViewTemplateCriteria);
			return Path.GetDirectoryName(emptyViewTemplateFileName);
		}
	}
}
