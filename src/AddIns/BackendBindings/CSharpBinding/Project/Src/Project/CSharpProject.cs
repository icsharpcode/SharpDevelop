// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpProject.
	/// </summary>
	public class CSharpProject : MSBuildProject
	{
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return CSharpAmbience.Instance;
			}
		}
		
		void Init()
		{
			Language = "C#";
			reparseSensitiveProperties.Add("TargetFrameworkVersion");
			reparseSensitiveProperties.Add("DefineConstants");
		}
		
		public CSharpProject(string fileName, string projectName)
		{
			this.Name = projectName;
			Init();
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public CSharpProject(ProjectCreateInformation info)
		{
			Init();
			Create(info);
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.CSharp.Targets";
		
		protected override void Create(ProjectCreateInformation information)
		{
			base.Create(information);
			this.Imports.Add(new MSBuildImport(DefaultTargetsFile));
			SetProperty("Debug", null, "CheckForOverflowUnderflow", "True", PropertyStorageLocations.ConfigurationSpecific);
			SetProperty("Release", null, "CheckForOverflowUnderflow", "False", PropertyStorageLocations.ConfigurationSpecific);
		}
		
		public override bool CanCompile(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), ".cs", StringComparison.OrdinalIgnoreCase);
		}
	}
}
