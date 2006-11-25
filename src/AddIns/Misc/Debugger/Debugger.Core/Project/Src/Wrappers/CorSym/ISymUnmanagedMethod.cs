// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public partial class ISymUnmanagedMethod
	{
		public SequencePoint[] SequencePoints {
			get {
				uint count = this.SequencePointCount;
				
				ISymUnmanagedDocument[] documents = new ISymUnmanagedDocument[count];
				uint[] offsets    = new uint[count];
				uint[] lines      = new uint[count];
				uint[] columns    = new uint[count];
				uint[] endLines   = new uint[count];
				uint[] endColumns = new uint[count];
				                  
				GetSequencePoints(count,
				                  out count,
				                  offsets,
				                  documents,
				                  lines,
				                  columns,
				                  endLines,
				                  endColumns);
				
				SequencePoint[] sequencePoints = new SequencePoint[count];
				
				for(int i = 0; i < count; i++) {
					sequencePoints[i] = new SequencePoint(documents[i],
					                                      offsets[i],
					                                      lines[i],
					                                      columns[i],
					                                      endLines[i],
					                                      endColumns[i]);
				}
				
				return sequencePoints;
			}
		}
	}
}

#pragma warning restore 1591
