// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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

namespace VBBinding
{
	/// <summary>
	/// This class describes a VB.NET project and it compilation options.
	/// </summary>
	public class VBProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return VBLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new VBCompilerParameters();
		}
		
		public VBProject()
		{
		}
		
		public VBProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name              = info.ProjectName;
				
				VBCompilerParameters debug = (VBCompilerParameters)CreateConfiguration("Debug");
				debug.Optimize = false;
				Configurations.Add(debug);
				
				VBCompilerParameters release = (VBCompilerParameters)CreateConfiguration("Release");
				debug.Optimize = true;
				release.Debugmode = false;
				release.GenerateOverflowChecks = false;
				release.TreatWarningsAsErrors = false;
				Configurations.Add(release);

				XmlElement el = projectOptions;
				
				foreach (VBCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
					
					if (el != null) {
						if (el.Attributes["Target"] != null) {
							parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), el.Attributes["Target"].InnerText);
						}
						if (el.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(el.Attributes["PauseConsoleOutput"].InnerText);
						}
					}
				}
			}
		}
	}
}
