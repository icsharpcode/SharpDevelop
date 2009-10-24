// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Interop.CorDebug
{
	public static partial class CorSymExtensionMethods
	{
		// ISymUnmanagedBinder
		
		public static ISymUnmanagedReader GetReaderForFile(this ISymUnmanagedBinder symBinder, object importer, string filename, string searchPath)
		{
			IntPtr pfilename = Marshal.StringToCoTaskMemUni(filename);
			IntPtr psearchPath = Marshal.StringToCoTaskMemUni(searchPath);
			ISymUnmanagedReader res = symBinder.GetReaderForFile(importer, pfilename, psearchPath);
			Marshal.FreeCoTaskMem(pfilename);
			Marshal.FreeCoTaskMem(psearchPath);
			return res;
		}
		
		// ISymUnmanagedDocument
		
		public static string GetURL(this ISymUnmanagedDocument symDoc)
		{
			return Util.GetString(symDoc.GetURL, 256, true);
		}
		
		public static byte[] GetCheckSum(this ISymUnmanagedDocument symDoc)
		{
			uint checkSumLength = 0;
			symDoc.GetCheckSum(checkSumLength, out checkSumLength, IntPtr.Zero);
			IntPtr checkSumPtr = Marshal.AllocHGlobal((int)checkSumLength);
			symDoc.GetCheckSum(checkSumLength, out checkSumLength, checkSumPtr);
			byte[] checkSumBytes = new byte[checkSumLength];
			Marshal.Copy(checkSumPtr, checkSumBytes, 0, (int)checkSumLength);
			Marshal.FreeHGlobal(checkSumPtr);
			return checkSumBytes;
		}
		
		// ISymUnmanagedMethod
		
		public static SequencePoint[] GetSequencePoints(this ISymUnmanagedMethod symMethod)
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
			
			SequencePoint[] sequencePoints = new SequencePoint[count];
			
			for(int i = 0; i < count; i++) {
				sequencePoints[i] = new SequencePoint() {
					Document = documents[i],
					Offset = offsets[i],
					Line = lines[i],
					Column = columns[i],
					EndLine = endLines[i],
					EndColumn = endColumns[i]
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
			symScope.GetNamespaces(0, out count, namespaces);
			return namespaces;
		}
		
		// ISymUnmanagedVariable
		
		public static string GetName(this ISymUnmanagedVariable symVar)
		{
			return Util.GetString(symVar.GetName);
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
	}
	
	public class SequencePoint: IComparable<SequencePoint>
	{
		public ISymUnmanagedDocument Document { get; private set; }
		public uint Offset { get; private set; }
		public uint Line { get; private set; }
		public uint Column { get; private set; }
		public uint EndLine { get; private set; }
		public uint EndColumn { get; private set; }
		
		public int CompareTo(SequencePoint other)
		{
			if (this.Line == other.Line) {
				return this.Column.CompareTo(other.Column);
			} else {
				return this.Line.CompareTo(other.Line);
			}
		}
	}
}
