/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 21.12.2004
 * Time: 18:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class TopologicalSortTest
	{
		[Test]
		public void Test()
		{
			
			List<Codon> codons = new List<Codon>(5);
			for (int i = 0; i < 5; ++i) {
				codons[i] = new Codon(null, "codon" + i, null, null);
			}
		}
	}
}
