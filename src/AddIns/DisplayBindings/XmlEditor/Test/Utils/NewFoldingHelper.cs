// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils
{
	public static class NewFoldingHelper
	{
		public static NewFolding CreateNewFold()
		{
			return new NewFolding();
		}
		
		public static List<NewFolding> CreateFoldListWithOneFold()
		{
			NewFolding fold = CreateNewFold();
			List<NewFolding> folds = CreateFoldList();
			folds.Add(fold);
			return folds;
		}
		
		public static List<NewFolding> CreateFoldList()
		{
			return new List<NewFolding>();
		}
		
		public static string[] ConvertToStrings(IList<NewFolding> folds)
		{
			List<string> foldsAsStrings = new List<string>();
			foreach (NewFolding fold in folds) {
				foldsAsStrings.Add(ConvertToString(fold));
			}
			return foldsAsStrings.ToArray();
		}
		
		public static string ConvertToString(NewFolding fold)
		{
			return String.Format("Name: '{0}' StartOffset: {1} EndOffset: {2} DefaultClosed: {3}",
				fold.Name, fold.StartOffset, fold.EndOffset, fold.DefaultClosed);
		}
		
		public static void AssertAreEqual(IList<NewFolding> expectedFolds, IList<NewFolding> actualFolds)
		{
			string[] expectedFoldsAsStrings = ConvertToStrings(expectedFolds);
			string[] actualFoldsAsStrings = ConvertToStrings(actualFolds);
			Assert.AreEqual(expectedFoldsAsStrings, actualFoldsAsStrings);
		}
	}
}
