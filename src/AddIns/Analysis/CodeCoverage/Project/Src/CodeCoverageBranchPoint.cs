/*
 * Created by SharpDevelop.
 * User: ddur
 * Date: 26/11/2013
 * Time: 02:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Description of CodeCoverageBranchPoint.
	/// </summary>
	public class CodeCoverageBranchPoint
	{
		public int VisitCount { get; set; }
		public int Offset { get; set; }
		public int Path { get; set; }
	}

}
