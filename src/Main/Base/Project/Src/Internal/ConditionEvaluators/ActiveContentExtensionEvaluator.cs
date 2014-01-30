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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests the file extension of the file edited in the active window content.
	/// </summary>
	/// <attribute name="activeextension">
	/// The file extension the file should have.
	/// </attribute>
	/// <example title="Test if a C# file is being edited">
	/// &lt;Condition name = "ActiveContentExtension" activeextension=".cs"&gt;
	/// </example>
	public class ActiveContentExtensionConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (SD.Workbench == null || SD.Workbench.ActiveWorkbenchWindow == null || SD.Workbench.ActiveViewContent == null) {
				return false;
			}
			try {
				string name = SD.Workbench.ActiveViewContent.PrimaryFileName;
				
				if (name == null) {
					return false;
				}
				
				string extension = Path.GetExtension(name);
				return extension.ToUpperInvariant() == condition.Properties["activeextension"].ToUpperInvariant();
			} catch (Exception) {
				return false;
			}
		}
	}
}
