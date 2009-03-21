// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Contains general fields for verification, synchronisation and important addresses and offsets.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	struct SharedMemoryHeader32
	{
		public int Magic;  // Verfication value, always '~SM1'
		public volatile int ExclusiveAccess;
		public int TotalLength;
		public int NativeToManagedBufferOffset;
		public int ThreadDataOffset;
		public int ThreadDataLength;
		public int HeapOffset;
		public int HeapLength;
		public TargetProcessPointer32 NativeAddress;
		public TargetProcessPointer32 RootFuncInfoAddress;
		public TargetProcessPointer32 LastThreadListItem;
		public int ProcessorFrequency;
		public bool DoNotProfileDotnetInternals;
		public bool CombineRecursiveFunction;
		public Allocator32 Allocator;
	}
	
	/// <summary>
	/// The representation of the unmanaged allocator used by the Hook.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	unsafe struct Allocator32
	{
		public TargetProcessPointer32 startPos;
		public TargetProcessPointer32 pos;
		public TargetProcessPointer32 endPos;

		public fixed UInt32 freeList[32]; // Need UInt32 instead of TargetProcessPointer32
									      // because of fixed
		
		public static void ClearFreeList(Allocator32 *a)
		{
			for (int i = 0; i < 32; i++) {
				a->freeList[i] = 0;
			}
		}
	}
	
	/// <summary>
	/// 32bit pointer used when a 32bit executable is profiled.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	struct TargetProcessPointer32
	{
		public UInt32 Pointer;

		public override string ToString()
		{
			return "0x" + Pointer.ToString("x", CultureInfo.InvariantCulture);
		}

		public static implicit operator TargetProcessPointer(TargetProcessPointer32 p)
		{
			return new TargetProcessPointer(p);
		}

		public static TargetProcessPointer32 operator +(TargetProcessPointer32 p, Int32 offset)
		{
			unchecked { p.Pointer += (UInt32)offset; }
			return p;
		}

		public static TargetProcessPointer32 operator -(TargetProcessPointer32 p, Int32 offset)
		{
			unchecked { p.Pointer -= (UInt32)offset; }
			return p;
		}

		public static Int32 operator -(TargetProcessPointer32 ptr1, TargetProcessPointer32 ptr2)
		{
			unchecked { return (Int32)(ptr1.Pointer - ptr2.Pointer); }
		}
	}
	
	/// <summary>
	/// A pointer that can be both 64 and 32 bit.
	/// </summary>
	struct TargetProcessPointer
	{
		TargetProcessPointer64 pointer;

		internal TargetProcessPointer(TargetProcessPointer32 p)
		{
			this.pointer.Pointer = p.Pointer;
		}

		internal TargetProcessPointer(TargetProcessPointer64 p)
		{
			this.pointer = p;
		}

		internal TargetProcessPointer32 To32()
		{
			TargetProcessPointer32 p = new TargetProcessPointer32();
			p.Pointer = (uint)pointer.Pointer;
			return p;
		}

		internal TargetProcessPointer64 To64()
		{
			return pointer;
		}

		public override string ToString()
		{
			return pointer.ToString();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is TargetProcessPointer) {
				return ((TargetProcessPointer)obj).pointer.Pointer == this.pointer.Pointer;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return pointer.Pointer.GetHashCode();
		}

		public static bool operator ==(TargetProcessPointer ptr, int number)
		{
			return (ptr.pointer.Pointer == (ulong)number);
		}

		public static bool operator !=(TargetProcessPointer ptr, int number)
		{
			return (ptr.pointer.Pointer != (ulong)number);
		}

		public static bool operator ==(TargetProcessPointer ptr, TargetProcessPointer ptr2)
		{
			return (ptr.pointer.Pointer == ptr2.pointer.Pointer);
		}

		public static bool operator !=(TargetProcessPointer ptr, TargetProcessPointer ptr2)
		{
			return (ptr.pointer.Pointer != ptr2.pointer.Pointer);
		}

		public static int operator -(TargetProcessPointer ptr, TargetProcessPointer ptr2)
		{
			return (int)(ptr.pointer.Pointer - ptr2.pointer.Pointer);
		}

		public static int operator +(TargetProcessPointer ptr, TargetProcessPointer ptr2)
		{
			return (int)(ptr.pointer.Pointer + ptr2.pointer.Pointer);
		}

		public static int operator +(TargetProcessPointer ptr, int num)
		{
			return (int)(ptr.pointer.Pointer + (ulong)num);
		}
	}
	
	/// <summary>
	/// The managed version of the FunctionInfo
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	unsafe partial struct FunctionInfo
	{
		public int Id;
		public int CallCount;
		public ulong TimeSpent;
		public int FillCount;
		public int LastChildIndex;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
		public static TargetProcessPointer32* GetChildren32(FunctionInfo* f)
		{
			if (f == null)
				throw new NullReferenceException();
			return (TargetProcessPointer32*)(f + 1);
		}

		public static void AddOrUpdateChild32(FunctionInfo* parent, FunctionInfo* child, Profiler profiler)
		{
			int slot = child->Id;
			while (true)
			{
				slot &= parent->LastChildIndex;
				FunctionInfo* slotContent = (FunctionInfo*)profiler.TranslatePointer(GetChildren32(parent)[slot]);
				if (slotContent == null || slotContent->Id == child->Id)
				{
					GetChildren32(parent)[slot] = profiler.TranslatePointerBack32(child);
					break;
				}
				slot++;
			}
		}
	}
	
	/// <summary>
	/// The managed version of the ThreadLocalData
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	unsafe struct ThreadLocalData32
	{
		public int ThreadID;
		public volatile int InLock;
		public TargetProcessPointer32 Predecessor;
		public TargetProcessPointer32 Follower;
		public LightweightStack32 Stack;
	}
	
	
	[StructLayout(LayoutKind.Sequential)]
	unsafe struct LightweightStack32
	{
		public TargetProcessPointer32 Array;
		public TargetProcessPointer32 TopPointer;
		public TargetProcessPointer32 ArrayEnd;
	}

	[StructLayout(LayoutKind.Sequential)]
	unsafe struct StackEntry32
	{
		public TargetProcessPointer32 Function;
		public ulong StartTime;
		public int FrameCount;
	}
}
