// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilCommandLine
	{
		List<string> arguments = new List<string>();
		
		public SvcUtilCommandLine(ServiceReferenceGeneratorOptions options)
		{
			GenerateCommandLine(options);
		}
		
		public string Command { get; set; }
		
		public string[] GetArguments()
		{
			return arguments.ToArray();
		}
		
		void GenerateCommandLine(ServiceReferenceGeneratorOptions options)
		{
			AppendIfNotEmpty("/o:", options.OutputFileName);
			AppendIfNotEmpty("/n:", options.GetNamespaceMapping());
			AppendIfNotEmpty("/language:", options.Language);
			AppendIfTrue("/noConfig", options.NoAppConfig);
			AppendIfTrue("/i", options.GenerateInternalClasses);
			AppendIfTrue("/a", options.GenerateAsyncOperations);
			AppendIfTrue("/mergeConfig", options.MergeAppConfig);
			AppendIfNotEmpty("/config:", options.AppConfigFileName);
			AppendIfNotEmpty("/ct:", options.GetArrayCollectionTypeDescription());
			AppendIfNotEmpty("/ct:", options.GetDictionaryCollectionTypeDescription());
			AppendAssemblyReferences(options.Assemblies);
			AppendIfNotEmpty(options.Url);
		}
		
		void AppendIfTrue(string argument, bool flag)
		{
			if (flag) {
				Append(argument);
			}
		}
		
		void AppendIfNotEmpty(string argumentName, string argumentValue)
		{
			if (!String.IsNullOrEmpty(argumentValue)) {
				Append(argumentName + argumentValue);
			}
		}
		
		void AppendIfNotEmpty(string argument)
		{
			if (!String.IsNullOrEmpty(argument)) {
				Append(argument);
			}
		}
		
		void Append(string argument)
		{
			arguments.Add(argument);
		}
		
		bool ContainsSpaceCharacter(string text)
		{
			return text.IndexOf(' ') >= 0;
		}
		
		void AppendAssemblyReferences(IEnumerable<string> assemblies)
		{
			foreach (string assembly in assemblies) {
				AppendIfNotEmpty("/r:", assembly);
			}
		}
	}
}
