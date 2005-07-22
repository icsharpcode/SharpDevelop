// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David McCloskey" email="dave_a_mccloskey@hotmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.TextEditor.Document
{
	// Currently not in use and doesn't compile because SetContent is not implemented.
	// If anyone wants to use this, it should be easy to fix it.
	/*
	public class PieceTableTextBufferStrategy : ITextBufferStrategy
	{
		protected struct PieceTableDescriptor
		{
			public bool Buffer;			// False for original, True for modified
			
			public int Offset;
			public int Length;
		}

		protected string OriginalBuffer = "";
		protected string ModifiedBuffer = ""; // Use StringBuilder

		// Use List<PieceTableDescriptor>
		protected ArrayList Descriptors = new ArrayList();

		public int Length
		{
			get
			{
				int length = 0;

				for(int i = 0; i < Descriptors.Count; i++)
				{
					length += ((PieceTableDescriptor)Descriptors[i]).Length;
				}

				return length;
			}
		}

		public void Insert(int offset, string text)
		{
			int Len = 0;
			int CurrentDesc = 0;

			while(true)
			{
				PieceTableDescriptor desc = (PieceTableDescriptor)Descriptors[CurrentDesc];

				// Is the offset in this descriptor
				if((Len + desc.Length) >= offset)
				{
					int gap = offset - Len;
					int newoffset = ModifiedBuffer.Length;

					// Add the text to the end of the buffer
					ModifiedBuffer += text;

					// Set up descriptor for the new text
					PieceTableDescriptor newtext = new PieceTableDescriptor();
					newtext.Offset = newoffset;
					newtext.Length = text.Length;
					newtext.Buffer = true;

					// Is the offset in the middle of the descriptor
					if(gap != 0 && gap != desc.Length)
					{
						int end = desc.Offset + desc.Length;

						desc.Length = gap;

						// Set up descriptor for the end of the current descriptor
						PieceTableDescriptor newdesc = new PieceTableDescriptor();
						newdesc.Offset = desc.Offset + desc.Length;
						newdesc.Length = end - newdesc.Offset;

						Descriptors[CurrentDesc] = desc;
						Descriptors.Insert(CurrentDesc + 1, newtext);
						Descriptors.Insert(CurrentDesc + 2, newdesc);
					}
					else if(gap == desc.Length)	// Is it at the end
					{
						Descriptors.Insert(CurrentDesc + 1, newtext);
					}
					else // Is it at the beginning
					{
						Descriptors.Insert(CurrentDesc, newtext);
					}

					break;
				}
				else
				{
					CurrentDesc++;
					
					Len += desc.Length;

					if(CurrentDesc == Descriptors.Count)
						break;
				}
			}
		}

		public void Remove(int offset, int length)
		{
			int Len = 0;
			int CurrentDesc = 0;

			while(true)
			{
				// Does the descriptor contain the offset
				if((Len + ((PieceTableDescriptor)Descriptors[CurrentDesc]).Length) >= offset)
				{
					// Remove the text from the descriptor
					RemoveInternal(CurrentDesc, Len, offset, length);

					break;
				}
				else
				{
					CurrentDesc++;
					
					Len += ((PieceTableDescriptor)Descriptors[CurrentDesc]).Length;

					if(CurrentDesc == Descriptors.Count)
						break;
				}
			}
		}

		protected void RemoveInternal(int descriptor, int lentodesc, int offset, int length)
		{
			PieceTableDescriptor desc = (PieceTableDescriptor)Descriptors[descriptor];

			int gap = offset - lentodesc;

			// Is all the text we want to remove span over multiple descriptors
			if((offset + length) > (lentodesc + desc.Length))
			{
				lentodesc += desc.Length;
				length -= lentodesc - offset;
				offset = lentodesc;

				desc.Length = gap;

				// Does the text we want to remove encompass all of this descriptor
				if(gap != 0)
				{
					Descriptors[descriptor] = desc;

					RemoveInternal(descriptor + 1, lentodesc, offset, length);
				}
				else // It does encompass all of this descriptor so remove it
				{
					Descriptors.RemoveAt(descriptor);

					RemoveInternal(descriptor, lentodesc, offset, length);
				}
			}
			else
			{
				// Set up new descriptor to reflect removed text
				PieceTableDescriptor newdesc = new PieceTableDescriptor();
				newdesc.Buffer = desc.Buffer;
				newdesc.Offset = desc.Offset + gap + length;
				newdesc.Length = (desc.Offset + desc.Length) - newdesc.Offset;
				
				desc.Length = gap;

				// Does the text we want to remove encompass all of this descriptor
				if(gap != 0)
				{
					Descriptors.Insert(descriptor + 1, newdesc);
					Descriptors[descriptor] = desc;
				}
				else
				{
					// Instead of removing the old and inserting the new, just set the old to the new inside the array
					Descriptors[descriptor] = newdesc;
				}
			}
		}

		public void Replace(int offset, int length, string text)
		{
			Remove(offset, length);
			Insert(offset, text);
		}

		public void SetText(string text)
		{
			Descriptors.Clear();

			ModifiedBuffer = "";
			OriginalBuffer = text;

			PieceTableDescriptor desc = new PieceTableDescriptor();
			desc.Buffer = false;
			desc.Offset = 0;
			desc.Length = text.Length;

			Descriptors.Add(desc);
		}

		public char GetCharAt(int offset)
		{
			int off = 0;
			int currdesc = 0;

			while(true)
			{
				PieceTableDescriptor desc = (PieceTableDescriptor)Descriptors[currdesc];

				// Is the offset in the current descriptor
				if((off + desc.Length) > offset)
				{
					// Find the difference between the beginning of the descriptor and the offset
					int gap = offset - off;

					if(desc.Buffer == false)	// Original Buffer
					{
						return OriginalBuffer[desc.Offset + gap];
					}
					else						// Modified Buffer
					{
						return ModifiedBuffer[desc.Offset + gap];
					}
				}
				else
				{
					off += desc.Length;
				}

				currdesc++;

				if(currdesc == Descriptors.Count) return '\0';
			}
		}

		public string GetText(int offset, int length)
		{
			string text = "";

			int off = 0;
			int currdesc = 0;

			while(true)
			{
				// Does the descriptor contain the offset
				if((off + ((PieceTableDescriptor)Descriptors[currdesc]).Length) > offset)
				{
					// Get the text
					text += GetTextInternal(currdesc, off, offset, length);

					break;
				}
				else
				{
					currdesc++;
					
					off += ((PieceTableDescriptor)Descriptors[currdesc]).Length;

					if(currdesc == Descriptors.Count)
						break;
				}
			}

			return text;
		}

		protected string GetTextInternal(int descriptor, int lentodesc, int offset, int length)
		{
			PieceTableDescriptor desc = (PieceTableDescriptor)Descriptors[descriptor];

			int gap = offset - lentodesc;

			string text = "";

			// Is the text we want greater than this descriptor
			if((offset + length) > (lentodesc + desc.Length))
			{
				if(desc.Buffer)
				{
					text += ModifiedBuffer.Substring(desc.Offset + gap, desc.Length);
				}
				else
				{
					text += OriginalBuffer.Substring(desc.Offset + gap, desc.Length);
				}
				
				lentodesc += desc.Length;
				length -= lentodesc - offset;
				offset = lentodesc;

				// Get the text from the next descriptor
				text += GetTextInternal(descriptor + 1, lentodesc, offset, length);
			}
			else
			{
				// The text we want is in this descriptor so get it
				if(desc.Buffer)
				{
					text += ModifiedBuffer.Substring(desc.Offset + gap, length);
				}
				else
				{
					text += OriginalBuffer.Substring(desc.Offset + gap, length);
				}
			}

			return text;
		}
	}
	*/
}
