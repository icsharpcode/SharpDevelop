// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests
{
	[TestFixture]
	public class TopologicalSortTest
	{
		[Test]
		public void Test()
		{
			var input = new List<List<Codon>> {
				new List<Codon> {
					CreateCodon("1"),
					CreateCodon("2"),
					CreateCodon("3")
				},
				new List<Codon> {
					CreateCodon("4"),
					CreateCodon("5"),
					CreateCodon("6"),
				},
				new List<Codon> {
					CreateCodon("7", insertAfter: "3"),
					CreateCodon("8"),
					CreateCodon("9", insertBefore: "4")
				}
			};
			
			Assert.AreEqual(new string[] { "1", "2", "3", "7", "8", "9", "4", "5", "6" },
			                TopologicalSort.Sort(input).ConvertAll(c => c.Id).ToArray());
		}
		
		Codon CreateCodon(string id, string insertBefore = "", string insertAfter = "")
		{
			Properties p = new Properties();
			p["id"] = id;
			p["insertbefore"] = insertBefore;
			p["insertafter"] = insertAfter;
			return new Codon(null, id, p, null);
		}
	}
}
