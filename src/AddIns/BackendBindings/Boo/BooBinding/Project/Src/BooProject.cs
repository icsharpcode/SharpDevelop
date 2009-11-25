// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

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
			get { return BooLanguageBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return BooLanguageProperties.Instance; }
		}
		
		public BooProject(IMSBuildEngineProvider provider, string fileName, string projectName)
			: base(provider)
		{
			this.Name = projectName;
			Init();
			LoadProject(fileName);
		}
		
		public BooProject(ProjectCreateInformation info)
			: base(info.Solution)
		{
			Init();
			Create(info);
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
		
		protected override void Create(ProjectCreateInformation information)
		{
			base.Create(information);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG;TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
			SetProperty("Release", null, "DefineConstants", "TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
		}
		
		void AddReference(string assembly)
		{
			foreach (ProjectItem item in this.Items) {
				if (item.ItemType == ItemType.Reference && item.Include == assembly)
					return;
			}
			((IProjectItemListProvider)this).AddProjectItem(new ReferenceProjectItem(this, assembly));
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".boo", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else
				return base.GetDefaultItemType(fileName);
		}
		
		internal static IProjectContent BooCompilerPC;
		internal static IProjectContent BooUsefulPC;
		
		protected override ParseProjectContent CreateProjectContent()
		{
			if (BooCompilerPC == null) {
				ReferenceProjectItem booCompilerItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Compiler.AbstractAstAttribute).Assembly.Location);
				BooCompilerPC = ParserService.GetProjectContentForReference(booCompilerItem);
			}
			if (BooUsefulPC == null) {
				ReferenceProjectItem booUsefulItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Useful.Attributes.SingletonAttribute).Assembly.Location);
				BooUsefulPC = ParserService.GetRegistryForReference(booUsefulItem).GetProjectContentForReference("Boo.Lang.Useful", booUsefulItem.Include);
			}
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem systemItem = new ReferenceProjectItem(this, "System");
			pc.AddReferencedContent(ParserService.GetProjectContentForReference(systemItem));
			ReferenceProjectItem booLangItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Builtins).Assembly.Location);
			pc.AddReferencedContent(ParserService.GetProjectContentForReference(booLangItem));
			ReferenceProjectItem booExtensionsItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Extensions.PropertyAttribute).Assembly.Location);
			pc.AddReferencedContent(ParserService.GetProjectContentForReference(booExtensionsItem));
			pc.DefaultImports = new DefaultUsing(pc);
			pc.DefaultImports.Usings.Add("Boo.Lang");
			pc.DefaultImports.Usings.Add("Boo.Lang.Builtins");
			pc.DefaultImports.Usings.Add("Boo.Lang.Extensions");
			return pc;
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
	}
}
