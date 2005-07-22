// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class TopologicalSortTest
	{
		[Test, Ignore("TODO: Write test")]
		public void Test()
		{
			List<Codon> codons = new List<Codon>();
			for (int i = 0; i < 5; ++i) {
				codons.Add(new Codon(null, "codon" + i, null, null));
			}
		}
	}
}
