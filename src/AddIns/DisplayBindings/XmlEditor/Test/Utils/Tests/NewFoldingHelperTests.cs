// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class NewFoldingHelperTests
	{
		List<NewFolding> folds;
		
		[Test]
		public void ConvertToStrings_OneFoldInList_ReturnsStringArrayWithOneItem()
		{
			folds = NewFoldingHelper.CreateFoldList();
			NewFolding fold = NewFoldingHelper.CreateNewFold();
			fold.Name = "test";
			fold.DefaultClosed = true;
			fold.StartOffset = 3;
			fold.EndOffset = 5;
			
			folds.Add(fold);
			
			string[] actualStrings = NewFoldingHelper.ConvertToStrings(folds);
			
			string[] expectedStrings = new string[] {
				"Name: 'test' StartOffset: 3 EndOffset: 5 DefaultClosed: True"
			};
			
			Assert.AreEqual(expectedStrings, actualStrings);
		}
	}
}
