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
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class SysModuleCompletionItems : PythonModuleCompletionItems
	{
		public SysModuleCompletionItems(PythonStandardModuleType moduleType)
			: base(moduleType)
		{
			AddCompletionItems();
		}
		
		void AddCompletionItems()
		{
			AddField("__stderr__");
			AddField("__stdin__");
			AddField("__stdout__");
			AddField("argv");
			AddField("builtin_module_names");
			AddField("dont_write_byte_code");
			AddField("executable");
			AddField("exec_prefix");
			AddField("flags");
			AddField("hexversion");
			AddField("last_type");
			AddField("last_value");
			AddField("last_traceback");
			AddField("meta_path");
			AddField("modules");
			AddField("path");
			AddField("path_hooks");
			AddField("path_importer_cache");
			AddField("ps1");
			AddField("ps2");
			AddField("py3kwarning");
			AddField("stderr");
			AddField("stdin");
			AddField("stdout");
			AddField("version");
			AddField("version_info");
			AddField("warnoptions");
		}
	}
}
