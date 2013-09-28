// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of StringHelper.
	/// </summary>
	class StringHelper
	{
		/// <summary>
		/// Left of the first occurance of c
		/// </summary>
		/// <param name="src"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string LeftOf(string src, char c)
		{
			int idx=src.IndexOf(c);
			if (idx==-1)
			{
				return src;
			}

			return src.Substring(0, idx);
		}

		/// <summary>
		/// Left of the n'th occurance of c
		/// </summary>
		/// <param name="src"></param>
		/// <param name="c"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static string LeftOf(string src, char c, int n)
		{
			int idx=-1;
			while (n != 0)
			{
				idx=src.IndexOf(c, idx+1);
				if (idx==-1)
				{
					return src;
				}
				--n;
			}
			return src.Substring(0, idx);
		}

		/// <summary>
		/// Right of the first occurance of c
		/// </summary>
		/// <param name="src"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string RightOf(string src, char c)
		{
			int idx=src.IndexOf(c);
			if (idx==-1)
			{
				return "";
			}
			
			return src.Substring(idx+1);
		}

		/// <summary>
		/// Right of the n'th occurance of c
		/// </summary>
		/// <param name="src"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string RightOf(string src, char c, int n)
		{
			int idx=-1;
			while (n != 0)
			{
				idx=src.IndexOf(c, idx+1);
				if (idx==-1)
				{
					return "";
				}
				--n;
			}
			
			return src.Substring(idx+1);
		}

		public static string LeftOfRightmostOf(string src, char c)
		{
			int idx=src.LastIndexOf(c);
			if (idx==-1)
			{
				return src;
			}
			return src.Substring(0, idx);
		}

		public static string RightOfRightmostOf(string src, char c)
		{
			int idx=src.LastIndexOf(c);
			if (idx==-1)
			{
				return src;
			}
			return src.Substring(idx+1);
		}

		public static string Between(string src, char start, char end)
		{
			string res=String.Empty;
			int idxStart=src.IndexOf(start);
			if (idxStart != -1)
			{
				++idxStart;
				int idxEnd=src.IndexOf(end, idxStart);
				if (idxEnd != -1)
				{
					res=src.Substring(idxStart, idxEnd-idxStart);
				}
			}
			return res;
		}

		public static int Count(string src, char find)
		{
			int ret=0;
			foreach(char s in src)
			{
				if (s==find)
				{
					++ret;
				}
			}
			return ret;
		}
	}
}
