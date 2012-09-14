// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.SharpDevelop
{
	sealed class ClipboardWrapper : IClipboard
	{
		void IClipboard.Clear()
		{
			Clipboard.Clear();
		}
		
		IDataObject IClipboard.GetDataObject()
		{
			return Clipboard.GetDataObject();
		}
		
		void IClipboard.SetDataObject(object data)
		{
			Clipboard.SetDataObject(data);
		}
		
		void IClipboard.SetDataObject(object data, bool copy)
		{
			Clipboard.SetDataObject(data, copy);
		}
		
		string IClipboard.GetText()
		{
			return Clipboard.GetText();
		}
		
		void IClipboard.SetText(string text)
		{
			Clipboard.SetText(text);
		}
		
		bool IClipboard.ContainsText()
		{
			return Clipboard.ContainsText();
		}
	}
}
