// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace Grunwald.BooBinding
{
	public class BooProject : CompilableProject
	{
		public static readonly string BooBinPath = Path.GetDirectoryName(typeof(BooProject).Assembly.Location);
		
		void Init()
		{
			reparseCodeSensitiveProperties.Add("Ducky");
			reparseCodeSensitiveProperties.Add("Strict");
			reparseReferencesSensitiveProperties.Add("TargetFrameworkVersion");
		}
		
		public override string Language {
			get { return BooProjectBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return BooLanguageProperties.Instance; }
		}
		
		public BooProject(ProjectLoadInformation info)
			: base(info)
		{
			Init();
		}
		
		public BooProject(ProjectCreateInformation info)
			: base(info)
		{
			Init();
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG;TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
			SetProperty("Release", null, "DefineConstants", "TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
			
			string rspFile = Path.Combine(BooBinPath, "booc.rsp");
			if (File.Exists(rspFile)) {
				using (StreamReader r = new StreamReader(rspFile)) {
					string line;
					while ((line = r.ReadLine()) != null) {
						line = line.Trim();
						if (line.StartsWith("-r:")) {
							AddReference(line.Substring(3));
						}
					}
				}
			}
			this.AddImport("$(BooBinPath)\\Boo.Microsoft.Build.targets", null);
		}
		
		void AddReference(string assembly)
		{
			foreach (ProjectItem item in this.Items) {
				if (item.ItemType == ItemType.Reference && item.Include == assembly)
					return;
			}
			((IProjectItemListProvider)this).AddProjectItem(new ReferenceProjectItem(this, assembly));
		}
		
		internal static IProjectContent BooCompilerPC;
		
		protected override ParseProjectContent CreateProjectContent()
		{
			if (BooCompilerPC == null) {
				ReferenceProjectItem booCompilerItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Compiler.AbstractAstAttribute).Assembly.Location);
				BooCompilerPC = AssemblyParserService.GetProjectContentForReference(booCompilerItem);
			}
			
			ParseProjectContent pc = base.CreateProjectContent();
			pc.DefaultImports = new DefaultUsing(pc);
			pc.DefaultImports.Usings.Add("Boo.Lang");
			pc.DefaultImports.Usings.Add("Boo.Lang.Builtins");
			pc.DefaultImports.Usings.Add("Boo.Lang.Extensions");
			return pc;
		}
		
		public override IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
			ReferenceProjectItem[] additionalReferences = {
				new ReferenceProjectItem(this, "mscorlib"),
				new ReferenceProjectItem(this, "System")
			};
			ReferenceProjectItem[] booReferences = {
				new ReferenceProjectItem(this, "Boo.Lang") { FileName = typeof(Boo.Lang.Builtins).Assembly.Location },
				new ReferenceProjectItem(this, "Boo.Extensions") { FileName = typeof(Boo.Lang.Extensions.PropertyAttribute).Assembly.Location }
			};
			return MSBuildInternals.ResolveAssemblyReferences(this, additionalReferences).Concat(booReferences);
		}
		
		public override IAmbience GetAmbience()
		{
			return new BooAmbience();
		}
		
		[Browsable(false)]
		public bool Ducky {
			get {
				bool val;
				if (bool.TryParse(GetEvaluatedProperty("Ducky"), out val))
					return val;
				else
					return false;
			}
		}
		
		[Browsable(false)]
		public bool Strict {
			get {
				bool val;
				if (bool.TryParse(GetEvaluatedProperty("Strict"), out val))
					return val;
				else
					return false;
			}
		}
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new BooProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class BooProjectBehavior : ProjectBehavior
	{
		public BooProjectBehavior(BooProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".boo", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else
				return base.GetDefaultItemType(fileName);
		}
	}
}
