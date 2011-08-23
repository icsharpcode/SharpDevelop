// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace Grunwald.BooBinding
{
	public class BooProjectBinding : IProjectBinding
	{
		public const string LanguageName = "Boo";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(ProjectLoadInformation loadInformation)
		{
			return new BooProject(loadInformation);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new BooProject(info);
		}
		
		public LanguageProperties LanguageProperties {
			get {
				return BooLanguageProperties.Instance;
			}
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
