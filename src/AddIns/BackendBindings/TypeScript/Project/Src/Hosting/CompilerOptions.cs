// 
// CompilerSettings.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
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

namespace ICSharpCode.TypeScriptBinding.Hosting
{
	public class CompilerOptions
	{
		public CompilerOptions(ITypeScriptOptions options)
			: this()
		{
			removeComments = options.RemoveComments;
			sourceMap = options.GenerateSourceMap;
			noImplicitAny = options.NoImplicitAny;
			target = options.GetScriptTarget();
			module = options.GetModuleTarget();
			@out = options.GetOutputFileFullPath();
			outDir = options.GetOutputDirectoryFullPath();
			emitBOM = false;
		}
		
		public CompilerOptions()
		{
			module = ModuleKind.None;
			target = ScriptTarget.Latest;
		}
		
		public bool? allowNonTsExtensions { get; set; }
		public string charset { get; set; }
		public bool? declaration { get; set; }
		public bool? diagnostics { get; set; }
		public bool? emitBOM { get; set; }
		public bool? help { get; set; }
		public bool? init { get; set; }
		public bool? inlineSourceMap { get; set; }
		public bool? inlineSources { get; set; }
		public JsxEmit? jsx { get; set; }
		public string reactNamespace { get; set; }
		public bool? listFiles { get; set; }
		public string locale { get; set; }
		public string mapRoot { get; set; }
		public ModuleKind? module { get; set; }
		public NewLineKind? newLine { get; set; }
		public bool? noEmit { get; set; }
		public bool? noEmitHelpers { get; set; }
		public bool? noEmitOnError { get; set; }
		public bool? noErrorTruncation { get; set; }
		public bool? noImplicitAny { get; set; }
		public bool? noLib { get; set; }
		public bool? noResolve { get; set; }
		public string @out { get; set; }
		public string outFile { get; set; }
		public string outDir { get; set; }
		public bool? preserveConstEnums { get; set; }
		public string project { get; set; }
		public bool? removeComments { get; set; }
		public string rootDir { get; set; }
		public bool? sourceMap { get; set; }
		public string sourceRoot { get; set; }
		public bool? suppressExcessPropertyErrors { get; set; }
		public bool? suppressImplicitAnyIndexErrors { get; set; }
		public ScriptTarget? target { get; set; }
		public bool? version { get; set; }
		public bool? watch { get; set; }
		public bool? isolatedModules { get; set; }
		public bool? experimentalDecorators { get; set; }
		public bool? emitDecoratorMetadata { get; set; }
		public ModuleResolutionKind? moduleResolution { get; set; }
		public bool? allowUnusedLabels { get; set; }
		public bool? allowUnreachableCode { get; set; }
		public bool? noImplicitReturns { get; set; }
		public bool? noFallthroughCasesInSwitch { get; set; }
		public bool? forceConsistentCasingInFileNames { get; set; }
		public bool? allowSyntheticDefaultImports { get; set; }
		public bool? allowJs { get; set; }
		public bool? noImplicitUseStrict { get; set; }
//		[option: string]: string | number | boolean;
	}
}