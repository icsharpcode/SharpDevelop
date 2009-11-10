// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlCompletionItemList : DefaultCompletionItemList
	{
		List<char> normalKeys = new List<char>();
		
		public XmlCompletionItemList()
		{
			normalKeys.AddRange(new char[] { ' ', ':', '.' });
		}
		
		public override CompletionItemListKeyResult ProcessInput(char key)
		{
			if (normalKeys.Contains(key)) {
				return CompletionItemListKeyResult.NormalKey;
			}
			return base.ProcessInput(key);
		}
	}
}
