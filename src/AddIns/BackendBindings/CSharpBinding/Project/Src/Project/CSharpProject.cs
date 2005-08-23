// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

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
		
		public CSharpProject(string fileName, string projectName)
		{
			this.Name = projectName;
			Language = "C#";
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public CSharpProject(ProjectCreateInformation info)
		{
			Language = "C#";
			Create(info);
		}
		
		protected override void Create(ProjectCreateInformation information)
		{
			base.Create(information);
			imports.Add(@"$(MSBuildBinPath)\Microsoft.CSharp.Targets");
			configurations["Debug|AnyCPU"]["CheckForOverflowUnderflow"] = "True";
			configurations["Release|AnyCPU"]["CheckForOverflowUnderflow"] = "False";
		}
		
		public override bool CanCompile(string fileName)
		{
			return new CSharpLanguageBinding().CanCompile(fileName);
		}
	}
}
