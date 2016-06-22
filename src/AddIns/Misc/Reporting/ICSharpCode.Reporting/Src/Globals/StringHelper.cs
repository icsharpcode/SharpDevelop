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
