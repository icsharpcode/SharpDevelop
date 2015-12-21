// 
// TypeScriptOptions.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
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
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptOptions : ITypeScriptOptions
	{
		public TypeScriptOptions()
		{
		}
		
		public TypeScriptOptions(ITypeScriptOptions options)
		{
			RemoveComments = options.RemoveComments;
			GenerateSourceMap = options.GenerateSourceMap;
			NoImplicitAny = options.NoImplicitAny;
			ModuleKind = options.ModuleKind;
			EcmaScriptVersion = options.EcmaScriptVersion;
			ModuleTarget = options.GetModuleTarget();
			ScriptTarget = options.GetScriptTarget();
			OutputFileName = options.GetOutputFileFullPath();
			OutputDirectory = options.GetOutputDirectoryFullPath();
		}
		
		public bool RemoveComments { get; set; }
		public bool GenerateSourceMap { get; set; }
		public bool NoImplicitAny { get; set; }
		public string ModuleKind { get; set; }
		public string EcmaScriptVersion { get; set; }
		public string OutputFileName { get; set; }
		public string OutputDirectory { get; set; }
		public ModuleKind ModuleTarget { get; set; }
		public ScriptTarget ScriptTarget { get; set; }
		
		public ScriptTarget GetScriptTarget()
		{
			return ScriptTarget;
		}
		
		public ModuleKind GetModuleTarget()
		{
			return ModuleTarget;
		}
		
		public string GetOutputFileFullPath()
		{
			return OutputFileName;
		}
		
		public string GetOutputDirectoryFullPath()
		{
			return OutputDirectory;
		}
	}
}
