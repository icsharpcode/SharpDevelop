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
using System.Linq;
using System.Runtime.InteropServices;

namespace Debugger.Interop.CorSym
{
	public static partial class CorSymExtensionMethods
	{
		static void ProcessOutParameter(object parameter)
		{
			TrackedComObjects.ProcessOutParameter(parameter);
		}
		
		// ISymUnmanagedBinder
		
		public static ISymUnmanagedReader GetReaderForFile(this ISymUnmanagedBinder symBinder, object importer, string filename, string searchPath)
		{
			IntPtr pfilename = Marshal.StringToCoTaskMemUni(filename);
			IntPtr psearchPath = Marshal.StringToCoTaskMemUni(searchPath);
			object res = null;
			// The method will create the object anyway so we have to use preservesig so that we can release it
			// failing to do so would lock the assembly
			int code = symBinder.GetReaderForFile(importer, pfilename, psearchPath, ref res);
			Marshal.FreeCoTaskMem(pfilename);
			Marshal.FreeCoTaskMem(psearchPath);
			if (code != 0) {
				Marshal.FinalReleaseComObject(res);
				throw new COMException("", code);
			}
			return (ISymUnmanagedReader)res;
		}
		
		// ISymUnmanagedDocument
		
		public static string GetURL(this ISymUnmanagedDocument symDoc)
		{
			return Util.GetCorSymString(symDoc.GetURL);
		}
		
		public static unsafe byte[] GetCheckSum(this ISymUnmanagedDocument symDoc)
		{
			uint actualLength;
			byte[] checkSum = new byte[20];
			fixed(byte* pCheckSum = checkSum)
				symDoc.GetCheckSum((uint)checkSum.Length, out actualLength, new IntPtr(pCheckSum));
			if (actualLength > checkSum.Length) {
				checkSum = new byte[actualLength];
				fixed(byte* pCheckSum = checkSum)
					symDoc.GetCheckSum((uint)checkSum.Length, out actualLength, new IntPtr(pCheckSum));
			}
			if (actualLength == 0) return null;
			Array.Resize(ref checkSum, (int)actualLength);
			return checkSum;
		}
		
		// ISymUnmanagedMethod
		
		public static SequencePoint[] GetSequencePoints(this ISymUnmanagedMethod symMethod, int codesize)
		{
			uint count = symMethod.GetSequencePointCount();
			
			ISymUnmanagedDocument[] documents = new ISymUnmanagedDocument[count];
			uint[] offsets    = new uint[count];
			uint[] lines      = new uint[count];
			uint[] columns    = new uint[count];
			uint[] endLines   = new uint[count];
			uint[] endColumns = new uint[count];
			                  
			symMethod.GetSequencePoints(
				count,
				out count,
				offsets,
				documents,
				lines,
				columns,
				endLines,
				endColumns
			);
			
			var sequencePoints = new SequencePoint[count];
			var urls = documents.Distinct().ToDictionary(d => d, d => d.GetURL());
			var sums = documents.Distinct().ToDictionary(d => d, d => d.GetCheckSum());
			
			uint token = symMethod.GetToken();
			for(int i = 0; i < count; i++) {
				sequencePoints[i] = new SequencePoint() {
					MethodDefToken = token,
					ILRanges = new [] { new ILRange((int)offsets[i], i + 1 < count ? (int)offsets[i + 1] : codesize) },
					Filename = urls[documents[i]],
					FileCheckSum = sums[documents[i]],
					StartLine = (int)lines[i],
					StartColumn = (int)columns[i],
					EndLine = (int)endLines[i],
					EndColumn = (int)endColumns[i]
				};
			}
			
			return sequencePoints;
		}
		
		// ISymUnmanagedReader
		
		public static ISymUnmanagedDocument GetDocument(this ISymUnmanagedReader symReader, string url, System.Guid language, System.Guid languageVendor, System.Guid documentType)
		{
			IntPtr p = Marshal.StringToCoTaskMemUni(url);
			ISymUnmanagedDocument res = symReader.GetDocument(p, language, languageVendor, documentType);
			Marshal.FreeCoTaskMem(p);
			return res;
		}
		
		// ISymUnmanagedScope
		
		public static ISymUnmanagedScope[] GetChildren(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetChildren(0, out count, new ISymUnmanagedScope[0]);
			ISymUnmanagedScope[] children = new ISymUnmanagedScope[count];
			symScope.GetChildren(count, out count, children);
			return children;
		}
		
		public static ISymUnmanagedVariable[] GetLocals(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetLocals(0, out count, new ISymUnmanagedVariable[0]);
			ISymUnmanagedVariable[] locals = new ISymUnmanagedVariable[count];
			symScope.GetLocals(count, out count, locals);
			return locals;
		}
		
		public static ISymUnmanagedNamespace[] GetNamespaces(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetNamespaces(0, out count, new ISymUnmanagedNamespace[0]);
			ISymUnmanagedNamespace[] namespaces = new ISymUnmanagedNamespace[count];
			symScope.GetNamespaces(count, out count, namespaces);
			return namespaces;
		}
		
		// ISymUnmanagedNamespace
		
		public static string GetName(this ISymUnmanagedNamespace symNs)
		{
			return Util.GetCorSymString(symNs.GetName);
		}
		
		// ISymUnmanagedVariable
		
		public static string GetName(this ISymUnmanagedVariable symVar)
		{
			return Util.GetCorSymString(symVar.GetName);
		}
		
		const int defaultSigSize = 8;
		
		public static unsafe byte[] GetSignature(this ISymUnmanagedVariable symVar)
		{
			byte[] sig = new byte[defaultSigSize];
			uint acualSize;
			fixed(byte* pSig = sig)
				symVar.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
			Array.Resize(ref sig, (int)acualSize);
			if (acualSize > defaultSigSize)
				fixed(byte* pSig = sig)
					symVar.GetSignature((uint)sig.Length, out acualSize, new IntPtr(pSig));
			return sig;
		}
		
		// ISymUnmanagedReader
		
		public static ISymUnmanagedNamespace[] GetNamespaces(this ISymUnmanagedReader symReader)
		{
			uint count;
			symReader.GetNamespaces(0, out count, new ISymUnmanagedNamespace[0]);
			ISymUnmanagedNamespace[] namespaces = new ISymUnmanagedNamespace[count];
			symReader.GetNamespaces(count, out count, namespaces);
			return namespaces;
		}
	}
}
