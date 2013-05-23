// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
