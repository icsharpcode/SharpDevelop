// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
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

namespace ICSharpCode.WixBinding
{
	public class WixProject : AbstractProject
	{
		public override string ProjectType {
			get {
				return WixLanguageBinding.LanguageName;
			}
		}
		
		public WixProject()
		{
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new WixCompilerParameters();
		}
		
		public WixProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name = info.ProjectName;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				foreach (WixCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
				}
				
			}
		}
	}
}
