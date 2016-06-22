// 
// LanguageServiceShimHost.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013-2015 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.TypeScriptBinding;
using ICSharpCode.TypeScriptBinding.Hosting;
using Newtonsoft.Json;

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class LanguageServiceShimHost : ILanguageServiceShimHost
	{
		Dictionary<string, Script> scripts = new Dictionary<string, Script>(StringComparer.OrdinalIgnoreCase);
		ILogger logger;
		FileName defaultLibScriptFileName;
		CompilerOptions compilerSettings = new CompilerOptions();
		
		public LanguageServiceShimHost(ILogger logger)
		{
			this.logger = logger;
		}
		
		internal void AddDefaultLibScript(FileName fileName, string text)
		{
			defaultLibScriptFileName = fileName;
			AddFile(fileName, text);
		}
		
		internal void AddFile(FileName fileName, string text)
		{
			if (!scripts.ContainsKey(fileName)) {
				scripts.Add(fileName, new Script(fileName, text));
			}
		}
		
		internal void UpdateFile(FileName fileName, string text)
		{
			Script script = FindScript(fileName);
			if (script != null) {
				script.Update(text);
			} else {
				AddFile(fileName, text);
			}
		}
		
		Script FindScript(FileName fileName)
		{
			Script script = null;
			if (scripts.TryGetValue(fileName, out script)) {
				return script;
			}
			return null;
		}
		
		internal void UpdateFile(string fileName, string text)
		{
			scripts[fileName].Update(text);
		}
		
		public int position { get; set; }
		public string fileName { get; set; }
		public bool isMemberCompletion { get; set; }
		public string completionEntry { get; set; }
		
		public void updateCompletionInfoAtCurrentPosition(string completionInfo)
		{
			LogDebug(completionInfo);
			CompletionResult = JsonConvert.DeserializeObject<CompletionResult>(completionInfo);
		}
		
		internal CompletionResult CompletionResult { get; private set; }
		
		public void updateCompletionEntryDetailsAtCurrentPosition(string completionEntryDetails)
		{
			LogDebug(completionEntryDetails);
			CompletionEntryDetailsResult = JsonConvert.DeserializeObject<CompletionEntryDetailsResult>(completionEntryDetails);
		}
		
		internal CompletionEntryDetailsResult CompletionEntryDetailsResult { get; private set; }
		
		public void updateSignatureAtPosition(string signature)
		{
			LogDebug(signature);
			SignatureResult = JsonConvert.DeserializeObject<SignatureResult>(signature);
		}
		
		internal SignatureResult SignatureResult { get; private set; }
		
		public void updateReferencesAtPosition(string references)
		{
			LogDebug(references);
			ReferencesResult = JsonConvert.DeserializeObject<ReferencesResult>(references);
		}
		
		internal ReferencesResult ReferencesResult { get; private set; }
		
		public void updateDefinitionAtPosition(string definition)
		{
			LogDebug(definition);
			DefinitionResult = JsonConvert.DeserializeObject<DefinitionResult>(definition);
		}
		
		internal DefinitionResult DefinitionResult { get; private set; }
		
		public void updateNavigationBarItems(string items)
		{
			LogDebug(items);
			NavigationResult = JsonConvert.DeserializeObject<NavigationResult>(items);
		}
		
		internal NavigationResult NavigationResult { get; private set; }
		
		public void log(string s)
		{
			logger.log(s);
		}
		
		public void trace(string s)
		{
			logger.trace(s);
		}
		
		public void error(string s)
		{
			logger.error(s);
		}
		
		void LogDebug(string format, params object[] args)
		{
			LogDebug(String.Format(format, args));
		}
		
		void LogDebug(string s)
		{
			log(s);
		}
		
		public string getCompilationSettings()
		{
			LogDebug("Host.getCompilationSettings");
			return JsonConvert.SerializeObject(compilerSettings);
		}
		
		internal void UpdateCompilerSettings(ITypeScriptOptions options)
		{
			compilerSettings = new CompilerOptions(options);
		}
		
		public string getScriptVersion(string fileName)
		{
			LogDebug("Host.getScriptVersion: " + fileName);
			return scripts[fileName].Version.ToString();
		}
		
		internal void UpdateFileName(FileName fileName)
		{
			this.fileName = fileName;
		}
		
		internal void RemoveFile(FileName fileName)
		{
			scripts.Remove(fileName);
		}
		
		internal IEnumerable<string> GetFileNames()
		{
			return scripts.Keys.AsEnumerable();
		}
		
		public IScriptSnapshotShim getScriptSnapshot(string fileName)
		{
			log("Host.getScriptSnapshot: " + fileName);
			Script script = scripts[fileName];
			return new ScriptSnapshotShim(logger, script);
		}
		
		public string getScriptFileNames()
		{
			log("Host.getScriptFileNames");
			
			string json = JsonConvert.SerializeObject(scripts.Select(keyPair => keyPair.Value.FileName));
			
			log("Host.getScriptFileNames: " + json);
			
			return json;
		}
		
		public string getCurrentDirectory()
		{
			log("Host.getCurrentDirectory");
			return String.Empty;
		}
		
		public string getLocalizedDiagnosticMessages()
		{
			log("Host.getLocalizedDiagnosticMessages");
			return null;
		}
		
		public string getDefaultLibFilename(string options)
		{
			log("Host.getDefaultLibFilename: " + options);
			return String.Empty;
		}
		
		public ICancellationToken getCancellationToken()
		{
			log("Host.getCancellationToken");
			return new LanguageServiceCancellationToken();
		}
		
		public void updateCompilerResult(string result)
		{
			log(result);
			CompilerResult = JsonConvert.DeserializeObject<CompilerResult>(result);
		}
		
		internal CompilerResult CompilerResult { get; private set; }
		
		public void updateSemanticDiagnosticsResult(string result)
		{
			log(result);
			SemanticDiagnosticsResult = JsonConvert.DeserializeObject<DiagnosticsResult>(result);
		}
		
		internal DiagnosticsResult SemanticDiagnosticsResult { get; private set; }
		
		public void updateSyntacticDiagnosticsResult(string result)
		{
			log(result);
			SyntacticDiagnosticsResult = JsonConvert.DeserializeObject<DiagnosticsResult>(result);
		}
		
		internal DiagnosticsResult SyntacticDiagnosticsResult { get; private set; }
		
		public string getNewLine()
		{
			return Environment.NewLine;
		}
		
		int projectVersion;
		
		public string getProjectVersion()
		{
			projectVersion++;
			return projectVersion.ToString();
		}
		
		public bool useCaseSensitiveFileNames()
		{
			return false;
		}
		
		public string getModuleResolutionsForFile(string fileName)
		{
			log("Host.getModuleResolutionsForFile: " + fileName);
			return null;
		}

		public bool directoryExists(string directoryName)
		{
			log("Host.directoryExists: " + directoryName);
			return true;
		}
	}
}
