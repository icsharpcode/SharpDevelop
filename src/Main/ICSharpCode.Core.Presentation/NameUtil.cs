using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Core.Presentation
{
	public static class NameUtil
	{
		public static string CleanName(string name)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
			string ret = regex.Replace(name, "_"); //The identifier must start with a character if (!char.IsLetter(ret, 0))

			return ret;
		}

		public static string ControlName(string header, string prefixId, string subfixName)
		{
			if (header != null)
				header = header.Trim();

			if (prefixId != null)
				prefixId = prefixId.Trim();

			if (subfixName != null)
				subfixName = subfixName.Trim();

			string name = string.Concat(header, prefixId, subfixName);
			return CleanName(name);
		}
	}
}
