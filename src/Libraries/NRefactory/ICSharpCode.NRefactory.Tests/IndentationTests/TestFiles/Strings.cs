namespace Strings
{
	class Strings
	{
		void Strings(string s = "")
		{
			s = @"""
can't escape a verbatim string \";
			s = @"" "not in verbatim anymore
				;
			s = "";
			s = "\\";
			s = "\\\\\\\"";
			s = " // syntax error, but on the next line we start with the previous state
				;
			s = "'c\'";
			string concat = "line 1" +
				"line 2" +
				"line 3";
			
			var c = '\\';
			c = '\'';
			c = ' // syntax error, but on the next line we start with the previous state
				;
			c = ';';
		}
	}
}
