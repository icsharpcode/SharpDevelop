using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis
{
	public class Instruction
	{
		public Method DeclaringMethod { get; set; }
		public string Operand { get; set; }
	}
}
