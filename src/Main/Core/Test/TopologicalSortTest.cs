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
