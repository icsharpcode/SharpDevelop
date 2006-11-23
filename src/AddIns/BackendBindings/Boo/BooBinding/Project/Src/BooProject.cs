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
		static bool initialized = false;
		public static readonly string BooBinPath = Path.GetDirectoryName(typeof(BooProject).Assembly.Location);
		
		void Init()
		{
			reparseCodeSensitiveProperties.Add("Ducky");
			
			if (!initialized) {
				initialized = true;
				MSBuildEngine.MSBuildProperties.Add("BooBinPath", BooBinPath);
			}
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
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem systemItem = new ReferenceProjectItem(this, "System");
			pc.AddReferencedContent(ParserService.GetProjectContentForReference(systemItem));
			ReferenceProjectItem booLangItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Builtins).Assembly.Location);
			pc.AddReferencedContent(ParserService.GetProjectContentForReference(booLangItem));
			if (BooCompilerPC == null) {
				ReferenceProjectItem booCompilerItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Compiler.AbstractAstAttribute).Assembly.Location);
				BooCompilerPC = ParserService.GetProjectContentForReference(booCompilerItem);
			}
			if (BooUsefulPC == null) {
				ReferenceProjectItem booUsefulItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Useful.Attributes.SingletonAttribute).Assembly.Location);
				BooUsefulPC = ParserService.GetProjectContentForReference(booUsefulItem);
			}
			pc.DefaultImports = new DefaultUsing(pc);
			pc.DefaultImports.Usings.Add("Boo.Lang");
			pc.DefaultImports.Usings.Add("Boo.Lang.Builtins");
			return pc;
		}
		
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return BooAmbience.Instance;
			}
		}
		
		[Browsable(false)]
		public bool Ducky {
			get {
				bool val;
				bool.TryParse(GetEvaluatedProperty("Ducky"), out val);
				return val;
			}
		}
	}
}
