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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using ICSharpCode.Profiler.Interprocess;
using NUnit.Framework;

namespace Profiler.Interprocess
{
	[TestFixture]
	public class UnmanagedCircularBufferTest
	{
		IntPtr memoryStart;
		int memoryLength;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			memoryLength = 1024;
			memoryStart = Marshal.AllocHGlobal(1024);
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			Marshal.FreeHGlobal(memoryStart);
		}
		
		[Test]
		[ExpectedException(typeof(TimeoutException))]
		public void ReadFromEmptyBufferCausesTimeout()
		{
			using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(memoryStart, memoryLength)) {
				using (Stream s = ncb.CreateReadingStream()) {
					s.ReadTimeout = 100;
					s.ReadByte();
				}
			}
		}
		
		[Test]
		[ExpectedException(typeof(TimeoutException))]
		public void WriteToFullBufferCausesTimeout()
		{
			using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(memoryStart, UnmanagedCircularBuffer.SynchronizationOverheadSize + 3)) {
				using (Stream s = ncb.CreateWritingStream()) {
					s.WriteTimeout = 100;
					try {
						s.WriteByte(1);
						s.WriteByte(2);
					} catch (TimeoutException) {
						Assert.Fail("The first two calls should work");
					}
					s.WriteByte(3);
				}
			}
		}
		
		[Test]
		public void WriteAndReadFromBuffer()
		{
			using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(memoryStart, memoryLength)) {
				using (Stream ws = ncb.CreateWritingStream()) {
					using (Stream rs = ncb.CreateReadingStream()) {
						ws.WriteByte(0x42);
						Assert.AreEqual(0x42, rs.ReadByte());
					}
				}
			}
		}
		
		[Test]
		public void WriteAndReadFromBuffer2()
		{
			using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(memoryStart, memoryLength)) {
				using (Stream ws = ncb.CreateWritingStream()) {
					ws.WriteByte(0x42);
				}
				using (UnmanagedCircularBuffer ncb2 = UnmanagedCircularBuffer.Open(memoryStart)) {
					using (Stream rs = ncb2.CreateReadingStream()) {
						Assert.AreEqual(0x42, rs.ReadByte());
					}
				}
			}
		}
		
		[Test]
		public void WriteAndReadFromBufferUsingMemoryMappedFile()
		{
			using (MemoryMappedFile mmf1 = MemoryMappedFile.CreateSharedMemory("Local\\TestMemory", 1024)) {
				using (UnmanagedMemory view1 = mmf1.MapView(0, 1024)) {
					using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(view1.Start, (int)view1.Length)) {
						using (Stream ws = ncb.CreateWritingStream()) {
							ws.WriteByte(0x42);
						}
						
						using (MemoryMappedFile mmf2 = MemoryMappedFile.Open("Local\\TestMemory")) {
							using (UnmanagedMemory view2 = mmf1.MapView(0, 1024)) {
								Assert.AreNotEqual(view1.Start, view2.Start);
								using (UnmanagedCircularBuffer ncb2 = UnmanagedCircularBuffer.Open(view2.Start)) {
									using (Stream rs = ncb2.CreateReadingStream()) {
										Assert.AreEqual(0x42, rs.ReadByte());
									}
								}
							}
						}
					}
				}
			}
		}
		
		[Test]
		public void PumpLargeDataSetThroughTinyBuffer()
		{
			using (UnmanagedCircularBuffer ncb = UnmanagedCircularBuffer.Create(memoryStart, UnmanagedCircularBuffer.SynchronizationOverheadSize + 3)) {
				byte[] data = new byte[10000];
				byte[] data2 = new byte[10000];
				for (int i = 0; i < data.Length; i++) {
					data[i] = unchecked( (byte)(i + 1) );
				}
				DoParallel(
					delegate {
						using (Stream w = ncb.CreateWritingStream()) {
							w.Write(data, 0, data.Length);
						}
					},
					delegate {
						using (Stream r = ncb.CreateReadingStream()) {
							int count = 0;
							while (count < data2.Length) {
								count += r.Read(data2, count, data2.Length - count);
							}
						}
					});
				Assert.AreEqual(data, data2);
			}
		}
		
		void DoParallel(ThreadStart a1, ThreadStart a2)
		{
			Thread t1 = new Thread(a1);
			t1.Start();
			a2();
			t1.Join();
		}
	}
}
