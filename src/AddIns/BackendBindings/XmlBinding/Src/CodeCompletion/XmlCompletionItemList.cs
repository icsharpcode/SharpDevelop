// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;

namespace ICSharpCode.XmlBinding
{
	/// <summary>
	/// Description of XmlCompletionItemList.
	/// </summary>
	sealed class XmlCompletionItemList : DefaultCompletionItemList
	{
		public XmlCompletionItemList()
		{
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == ':' || key == '.')
				return CompletionItemListKeyResult.NormalKey;
			
			return base.ProcessInput(key);
		}
	}
}
