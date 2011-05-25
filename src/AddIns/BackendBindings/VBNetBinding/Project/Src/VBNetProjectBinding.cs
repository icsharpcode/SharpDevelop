// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetProjectBinding : IProjectBinding
	{
		public const string LanguageName = "VBNet";
		
		public string Language
		{
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(ProjectLoadInformation info)
		{
			return new VBNetProject(info);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new VBNetProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
