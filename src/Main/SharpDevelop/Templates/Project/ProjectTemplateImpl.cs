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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	internal class ProjectTemplateImpl : ProjectTemplate, ICategory
	{
		string originator;
		string created;
		string lastmodified;
		string name;
		string category;
		string languagename;
		string description;
		IImage icon;
		string subcategory;
		TargetFramework[] supportedTargetFrameworks;
		
		public override IEnumerable<TargetFramework> SupportedTargetFrameworks {
			get { return supportedTargetFrameworks; }
		}
		
		bool newProjectDialogVisible = true;
		
		public override bool IsVisible(ISolution solution)
		{
			return newProjectDialogVisible && (solution == null || projectDescriptor != null);
		}
		
		List<Action<ProjectTemplateResult>> openActions = new List<Action<ProjectTemplateResult>>();
		
		SolutionDescriptor solutionDescriptor = null;
		ProjectDescriptor projectDescriptor = null;
		
		#region Template Properties
		public string Originator {
			get {
				return originator;
			}
		}
		
		public string Created {
			get {
				return created;
			}
		}
		
		public string LastModified {
			get {
				return lastmodified;
			}
		}
		
		public override string Name {
			get {
				return name;
			}
		}
		
		public override string DisplayName {
			get {
				return StringParser.Parse(name);
			}
		}
		public string Category {
			get {
				return category;
			}
		}
		
		public string Subcategory {
			get {
				return subcategory;
			}
		}
		
		public string LanguageName {
			get {
				return languagename;
			}
		}
		
		public override string Description {
			get {
				return description;
			}
		}
		
		public override IImage Icon {
			get {
				return icon;
			}
		}
		
		public bool NewProjectDialogVisible {
			get {
				return newProjectDialogVisible;
			}
		}
		
		
		[Browsable(false)]
		public SolutionDescriptor SolutionDescriptor {
			get {
				return solutionDescriptor;
			}
		}
		
		[Browsable(false)]
		public ProjectDescriptor ProjectDescriptor {
			get {
				return projectDescriptor;
			}
		}
		#endregion
		
		public ProjectTemplateImpl(XmlDocument doc, IReadOnlyFileSystem fileSystem)
		{
			var templateElement = doc.DocumentElement;
			originator   = templateElement.GetAttribute("originator");
			created      = templateElement.GetAttribute("created");
			lastmodified = templateElement.GetAttribute("lastModified");
			
			string newProjectDialogVisibleAttr  = templateElement.GetAttribute("newprojectdialogvisible");
			if (string.Equals(newProjectDialogVisibleAttr, "false", StringComparison.OrdinalIgnoreCase))
				newProjectDialogVisible = false;
			
			XmlElement config = templateElement["TemplateConfiguration"];
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			
			if (config["LanguageName"] != null) {
				languagename = config["LanguageName"].InnerText;
				WarnObsoleteNode(config["LanguageName"], "use language attribute on the project node instead");
			}
			
			if (config["Subcategory"] != null) {
				subcategory = config["Subcategory"].InnerText;
			}
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon = SD.ResourceService.GetImage(config["Icon"].InnerText);
			}
			
			if (config["SupportedTargetFrameworks"] != null) {
				var specifiedTargetFrameworks =
					config["SupportedTargetFrameworks"].InnerText.Split(';')
					.Select<string,TargetFramework>(TargetFramework.GetByName).ToArray();
				
				supportedTargetFrameworks = SD.ProjectService.TargetFrameworks.Where(fx => specifiedTargetFrameworks.Any(s => fx.IsBasedOn(s))).ToArray();
			} else {
				supportedTargetFrameworks = new TargetFramework[0];
			}
			
			if (templateElement["Solution"] != null) {
				solutionDescriptor = SolutionDescriptor.CreateSolutionDescriptor(templateElement["Solution"], fileSystem);
			} else if (templateElement["Combine"] != null) {
				solutionDescriptor = SolutionDescriptor.CreateSolutionDescriptor(templateElement["Combine"], fileSystem);
				WarnObsoleteNode(templateElement["Combine"], "Use <Solution> instead!");
			}
			
			if (templateElement["Project"] != null) {
				projectDescriptor = new ProjectDescriptor(templateElement["Project"], fileSystem);
			}
			
			if (solutionDescriptor == null && projectDescriptor == null
			    || solutionDescriptor != null && projectDescriptor != null)
			{
				throw new TemplateLoadException("Template must contain either Project or Solution node!");
			}
			
			// Read Actions;
			if (templateElement["Actions"] != null) {
				foreach (XmlElement el in templateElement["Actions"]) {
					Action<ProjectTemplateResult> action = ReadAction(el);
					if (action != null)
						openActions.Add(action);
				}
			}
		}
		
		static Action<ProjectTemplateResult> ReadAction(XmlElement el)
		{
			switch (el.Name) {
				case "Open":
					if (el.HasAttribute("filename")) {
						string fileName = el.GetAttribute("filename");
						return projectTemplateResult => {
							var projectCreateInformation = projectTemplateResult.Options;
							string unresolvedFileName = StringParser.Parse(fileName, new StringTagPair("ProjectName", projectCreateInformation.ProjectName));
							string path = Path.Combine(projectCreateInformation.ProjectBasePath, unresolvedFileName);
							FileService.OpenFile(path);
						};
					} else {
						WarnAttributeMissing(el, "filename");
						return null;
					}
				case "RunCommand":
					if (el.HasAttribute("path")) {
						try {
							ICommand command = (ICommand)SD.AddInTree.BuildItem(el.GetAttribute("path"), null);
							return command.Execute;
						} catch (TreePathNotFoundException ex) {
							MessageService.ShowWarning(ex.Message + " - in " + el.OwnerDocument.DocumentElement.GetAttribute("fileName"));
							return null;
						}
					} else {
						WarnAttributeMissing(el, "path");
						return null;
					}
				default:
					WarnObsoleteNode(el, "Unknown action element is interpreted as <Open>");
					goto case "Open";
			}
		}
		
		internal static void WarnObsoleteNode(XmlElement element, string message)
		{
			MessageService.ShowWarning("Obsolete node <" + element.Name +
			                           "> used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		internal static void WarnObsoleteAttribute(XmlElement element, string attribute, string message)
		{
			MessageService.ShowWarning("Obsolete attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           "used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		internal static void WarnAttributeMissing(XmlElement element, string attribute)
		{
			MessageService.ShowWarning("Missing attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           " in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "'");
		}
		
		public override ProjectTemplateResult CreateProjects(ProjectTemplateOptions options)
		{
			var result = new ProjectTemplateResult(options);
			StandardHeader.SetHeaders();
			if (solutionDescriptor != null) {
				if (!solutionDescriptor.AddContents(options.SolutionFolder, result, languagename))
					return null;
			}
			if (projectDescriptor != null) {
				bool success = projectDescriptor.CreateProject(result, languagename, options.SolutionFolder);
				if (!success) {
					return null;
				}
			}
			return result;
		}
		
		public override void RunOpenActions(ProjectTemplateResult result)
		{
			foreach (var action in openActions) {
				action(result);
			}
		}
	}
}
