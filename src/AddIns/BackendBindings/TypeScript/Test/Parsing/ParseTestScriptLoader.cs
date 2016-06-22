
using System;
using System.IO;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace TypeScriptBinding.Tests.Parsing
{
	public class ParseTestScriptLoader : IScriptLoader
	{
		public string RootFolder {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string TypeScriptCompilerFileName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string GetTypeScriptServicesScript()
		{
			return GetScriptFromResource("typescriptServices.js");
		}
		
		string GetScriptFromResource(string scriptName)
		{
			Stream stream = this.GetType().Assembly.GetManifestResourceStream(scriptName);
			return new StreamReader(stream).ReadToEnd();
		}
		
		public string GetMainScript()
		{
			return GetScriptFromResource("main.js");
		}
		
		public string GetMemberCompletionScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetTypeScriptCompilerScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetFunctionSignatureScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetLibScript()
		{
			return GetScriptFromResource("lib.d.ts");
		}
		
		public string GetFindReferencesScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetDefinitionScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetNavigationScript()
		{
			return GetScriptFromResource("navigation.js");
		}
		
		public string LibScriptFileName {
			get { return "lib.d.ts"; }
		}
		
		public string GetCompletionDetailsScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetLanguageServicesCompileScript()
		{
			throw new NotImplementedException();
		}
		
		public string GetDiagnosticsScript()
		{
			throw new NotImplementedException();
		}
	}
}
