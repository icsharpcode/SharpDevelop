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
using System.Windows;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Interface for clipboard access.
	/// </summary>
	[SDService("SD.Clipboard")]
	public interface IClipboard
	{
		/// <inheritdoc cref="System.Windows.Clipboard.Clear"/>
		void Clear();
		
		/// <inheritdoc cref="System.Windows.Clipboard.GetDataObject"/>
		IDataObject GetDataObject();
		
		/// <inheritdoc cref="System.Windows.Clipboard.SetDataObject(object)"/>
		void SetDataObject(object data);
		
		/// <inheritdoc cref="System.Windows.Clipboard.SetDataObject(object, bool)"/>
		void SetDataObject(object data, bool copy);
		
		/// <inheritdoc cref="System.Windows.Clipboard.ContainsText"/>
		bool ContainsText();
		/// <inheritdoc cref="System.Windows.Clipboard.GetText"/>
		string GetText();
		/// <inheritdoc cref="System.Windows.Clipboard.SetText(string)"/>
		void SetText(string text);
	}
}
