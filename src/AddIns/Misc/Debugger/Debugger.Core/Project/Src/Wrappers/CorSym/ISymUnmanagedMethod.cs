// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorSym
{
	using System;
	
	
	public static partial class CorSymExtensionMethods
	{
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
				sequencePoints[i] = new SequencePoint(
					documents[i],
					offsets[i],
					lines[i],
					columns[i],
					endLines[i],
					endColumns[i]
				);
			}
			
			return sequencePoints;
		}
	}
}

#pragma warning restore 1591
