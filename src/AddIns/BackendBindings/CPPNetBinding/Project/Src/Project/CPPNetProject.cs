// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace CPPBinding
{
	/// <summary>
	/// This class describes a C Sharp project and it compilation options.
	/// </summary>
	public class CPPProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return CPPLanguageBinding.LanguageName;
			}
		}
		
		public CPPProject()
		{
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new CPPCompilerParameters();
		}
		
		public CPPProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name = info.ProjectName;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (CPPCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.IntermediateDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
					if (projectOptions != null) {
						if (projectOptions.Attributes["ConfigurationType"] != null) {
							parameter.ConfigurationType = (ConfigurationType)Enum.Parse(typeof(ConfigurationType), projectOptions.Attributes["ConfigurationType"].InnerText);
						}
					}
					parameter.OutputFile = parameter.OutputDirectory + Path.DirectorySeparatorChar + Name + (parameter.ConfigurationType == ConfigurationType.Dll ? ".dll" : ".exe");
				}
			}
		}
	}
}
