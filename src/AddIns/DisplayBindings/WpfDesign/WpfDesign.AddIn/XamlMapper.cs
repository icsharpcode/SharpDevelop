using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Xaml;
using ICSharpCode.SharpDevelop.Dom;
using System.IO;
using System.Windows.Markup;
using ICSharpCode.SharpDevelop;
using System.Reflection;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.WpfDesign.Designer.XamlBackend;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class XamlMapper
	{
		static XamlMapper()
		{
			ProjectService.SolutionLoaded += new EventHandler<SolutionEventArgs>(ProjectService_SolutionLoaded);
		}

		static void ProjectService_SolutionLoaded(object sender, SolutionEventArgs e)
		{
			//throw new NotImplementedException();
		}

		//TODO xamlDocs
		static Dictionary<IProject, XamlProject> xamlProjects = new Dictionary<IProject, XamlProject>();

		public static TypeResolutionService TypeResolutionServiceInstance = new TypeResolutionService();

		public static XamlProject GetXamlProject(IProject p)
		{
			XamlProject result;
			if (!xamlProjects.TryGetValue(p, out result)) {
				result = CreateXamlProject(p);
				xamlProjects[p] = result;
			}
			return result;
		}

		static XamlProject CreateXamlProject(IProject domProject)
		{
			var result = new WpfProject();

			var projectContent = ParserService.GetProjectContent(domProject);
			result.ProjectAssembly = GetXamlAssembly(projectContent);

			foreach (var referenceContent in projectContent.ReferencedContents) {
				result.AddReference(GetXamlAssembly(referenceContent));
			}

			foreach (var item in domProject.Items) {
				if (item.ItemType == ItemType.Page) {
					if (XamlConstants.HasXamlExtension(item.FileName)) {
						result.LoadDocument(item.FileName);
					}
				}
				//else if (item.ItemType == ItemType.ApplicationDefinition) {
				//    result.ApplicationDefinition = result.LoadDocument(item.FileName);
				//}
			}

			return result;
		}

		public static XamlAssembly GetXamlAssembly(IProjectContent projectContent)
		{
			if (projectContent is ReflectionProjectContent) {
				var assembly = TypeResolutionServiceInstance.LoadAssembly(projectContent);
				return ReflectionMapper.GetXamlAssembly(assembly);
			}
			return new IdeXamlAssembly(projectContent);
		}

		public static XamlDocument GetXamlDocument(string filePath)
		{
			if (ProjectService.OpenSolution != null) {
				var domProject = ProjectService.OpenSolution.FindProjectContainingFile(filePath);
				if (domProject != null) {
					var xamlProject = GetXamlProject(domProject);
					if (xamlProject != null) {
						return xamlProject.LoadDocument(filePath);
					}
				}
			}
			return null;
		}

		public static XamlType GetXamlType(IClass c)
		{
			throw new NotImplementedException();
			//return c.UserData as XamlType;
		}

		public static XamlMember GetXamlMember(IMember m)
		{
			throw new NotImplementedException();
		}

		public static IProject GetDomProject(XamlProject p)
		{
			throw new NotImplementedException();
		}

		public static IClass GetDomClass(XamlType t)
		{
			throw new NotImplementedException();
		}

		public static IMember GetDomMember(XamlMember m)
		{
			throw new NotImplementedException();
		}

		//static IProjectContent GetProjectContent(OpenedFile file)
		//{
		//    if ((ProjectService.OpenSolution != null) && (file != null))
		//    {
		//        IProject p = ProjectService.OpenSolution.FindProjectContainingFile(file.FileName);
		//        if (p != null)
		//        {
		//            return ParserService.GetProjectContent(p);
		//        }
		//    }
		//    return ParserService.DefaultProjectContent;
		//}

		//IProjectContent pc = GetProjectContent(file);
		//if (pc != null) {
		//    IClass c = pc.GetClassByReflectionName(property.DeclaringType.FullName, true);
		//    if (c != null) {
		//        IMember m = DefaultProjectContent.GetMemberByReflectionName(c, property.Name);
		//        if (m != null)
		//            return CodeCompletionData.GetDocumentation(m.Documentation);
		//    }
		//}
		//return null;
	}

	class IdeXamlAssembly : XamlAssembly
	{
		public IdeXamlAssembly(IProjectContent projectContent)
		{
			this.ProjectContent = projectContent;
		}

		public IProjectContent ProjectContent { get; private set; }

		public override IEnumerable<XmlnsDefinitionAttribute> XmlnsDefinitions
		{
			get { yield break; }
		}

		public override string Name
		{
			get { return GetName(ProjectContent); }
		}

		public override XamlType GetType(string fullName)
		{
			var domClass = ProjectContent.GetClass(fullName, 0);
			return new IdeXamlType(domClass);
		}

		static string GetName(IProjectContent projectContent)
		{
			var reflection = projectContent as ReflectionProjectContent;
			if (reflection != null) {
				return new AssemblyName(reflection.AssemblyFullName).Name;
			}
			var project = projectContent.Project as IProject;
			if (project != null) {
				return project.Name;
			}
			return null;
		}
	}

	class IdeXamlType : DefaultXamlType
	{
		public IdeXamlType(IClass domClass)
		{
			this.Class = domClass;
		}

		public IClass Class { get; private set; }
	}

	class IdeXamlMember : DefaultXamlMember
	{
		public IdeXamlMember(IMember domMember)
		{
			this.Member = domMember;
		}

		public IMember Member { get; private set; }
	}
}
