// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using ICSharpCode.Profiler.Interprocess;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Stores recorded profiling data in a temporary file, which is deleted when the file database is closed.
	/// Instance members of this class are not thread-safe.
	/// </summary>
	public class TempFileDatabase : IDisposable
	{
		FileStream file;
		int processorFrequency;
		bool is64Bit;
		
		Dictionary<int, NameMapping> nameMappings = new Dictionary<int, NameMapping>();
		
		/// <summary>
		/// The list of NameMappings in the database.
		/// </summary>
		/// <remarks>NameMappings are held in memory and not written to the file.</remarks>
		public ReadOnlyCollection<NameMapping> NameMappings {
			get { return nameMappings.Select(i => i.Value).ToList().AsReadOnly(); }
		}
		
		struct StreamInfo
		{
			public TargetProcessPointer NativeStartPosition { get; set; }
			public TargetProcessPointer NativeRootFuncInfoStartPosition { get; set; }
			public long StreamStartPosition { get; set; }
			public long StreamLength { get; set; }
			public double CpuUsage { get; set; }
			public bool IsFirst { get; set; }
		}
		
		#region TempFileDatabase DataSet and DataWriter implementation
		sealed unsafe class DataSet : UnmanagedProfilingDataSet
		{
			TempFileDatabase database;
			UnmanagedMemory view;
			
			public DataSet(TempFileDatabase database, UnmanagedMemory view,
			               TargetProcessPointer nativeStartPosition, TargetProcessPointer nativeRootFuncInfoStartPosition,
			               double cpuUsage, bool isFirst)
				: base(nativeStartPosition, nativeRootFuncInfoStartPosition, view.Pointer, view.Length, cpuUsage, isFirst, database.is64Bit)
			{
				this.database = database;
				this.view = view;
			}
			
			public override int ProcessorFrequency {
				get {
					return this.database.processorFrequency;
				}
			}
			
			unsafe internal override void* TranslatePointer(TargetProcessPointer ptr)
			{
				if (ptr.To64().Pointer == 0)
					return null;
				unchecked {
					long spaceDiff = (long)(new IntPtr(view.Pointer)) - (long)NativeStartPosition.To64().Pointer;
					return new IntPtr((long)ptr.To64().Pointer + spaceDiff).ToPointer();
				}
			}
			
			public override NameMapping GetMapping(int nameId)
			{
				if (nameId == 0)
					return new NameMapping(0);
				return this.database.nameMappings[nameId];
			}
			
			public override void Dispose()
			{
				base.Dispose();
				this.view.Dispose();
			}
		}
		
		sealed class TempFileDatabaseWriter : IProfilingDataWriter
		{
			TempFileDatabase database;
			bool isClosed;
			
			public TempFileDatabaseWriter(TempFileDatabase database)
			{
				this.database = database;
			}
			
			public int ProcessorFrequency {
				get {
					return this.database.processorFrequency;
				}
				set {
					this.database.processorFrequency = value;
				}
			}
			
			public unsafe void WriteDataSet(IProfilingDataSet dataSet)
			{
				UnmanagedProfilingDataSet uDataSet = dataSet as UnmanagedProfilingDataSet;
				if (dataSet == null)
					throw new InvalidOperationException("TempFileDatabase cannot write DataSets other than UnmanagedProfilingDataSet!");
				
				database.AddDataset((byte *)uDataSet.StartPtr.ToPointer(), uDataSet.Length, uDataSet.NativeStartPosition, uDataSet.NativeRootFuncInfoPosition, uDataSet.CpuUsage, uDataSet.IsFirst);
				this.database.is64Bit = uDataSet.Is64Bit;
			}
			
			public void Close()
			{
				if (!isClosed)
					database.NotifyFinish();
				isClosed = true;
			}
			
			public void WriteMappings(IEnumerable<NameMapping> mappings)
			{
				foreach (NameMapping nm in mappings)
					this.database.nameMappings.Add(nm.Id, nm);
			}
		}
		#endregion
		
		readonly List<StreamInfo> streamInfos = new List<StreamInfo>();
		IAsyncResult currentWrite;
		MemoryMappedFile mmf;
		string fileName;
		
		/// <summary>
		/// Creates a new TempFileDatabase.
		/// </summary>
		public TempFileDatabase()
		{
			this.fileName = Path.GetTempFileName();
			this.file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
		}
		
		unsafe void AddDataset(byte *ptr, long length, TargetProcessPointer nativeStartPosition, TargetProcessPointer nativeRootFuncInfoStartPosition, double cpuUsage, bool isFirst)
		{
			byte[] data = new byte[length];
			Marshal.Copy(new IntPtr(ptr), data, 0, (int)length);
			if (this.currentWrite != null)
				this.file.EndWrite(this.currentWrite);
			this.streamInfos.Add(new StreamInfo { NativeStartPosition = nativeStartPosition, NativeRootFuncInfoStartPosition = nativeRootFuncInfoStartPosition,
			                     	StreamStartPosition = this.file.Length, StreamLength = length, CpuUsage = cpuUsage, IsFirst = isFirst });
			this.currentWrite = this.file.BeginWrite(data, 0, (int)length, null, null);
		}
		
		void NotifyFinish()
		{
			if (this.currentWrite != null) {
				this.file.EndWrite(this.currentWrite);
				this.currentWrite = null;
			}
			this.file.Flush();
			
			if (this.streamInfos.Count > 0)
				this.mmf = MemoryMappedFile.Open(file);//Name, FileAccess.Read, FileShare.ReadWrite);
		}
		
		/// <summary>
		/// Closes the TempFileDatabase. After calling this method, the data recorded is lost, because the temporary file is deleted.
		/// </summary>
		public void Close()
		{
			if (this.mmf != null)
				this.mmf.Close();
			this.file.Close();
		}
		
		/// <summary>
		/// Creates a new Writer to allow writing access to the database.
		/// </summary>
		public IProfilingDataWriter GetWriter()
		{
			return new TempFileDatabaseWriter(this);
		}
		
		/// <summary>
		/// Loads a dataset into memory.
		/// </summary>
		/// <remarks>You can not load any datasets before all writers are closed.</remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
		public UnmanagedProfilingDataSet LoadDataSet(int index)
		{
			if (index < 0 || index >= streamInfos.Count)
				throw new IndexOutOfRangeException("index needs to be between 0 and " + (streamInfos.Count - 1)
				                                   + "\nActual value: " + index);
			
			if (this.mmf == null)
				throw new InvalidOperationException("All writers have to be closed before reading the data from the database!");
			
			return new DataSet(this, mmf.MapView(streamInfos[index].StreamStartPosition, streamInfos[index].StreamLength), streamInfos[index].NativeStartPosition,
			                   streamInfos[index].NativeRootFuncInfoStartPosition,
			                   streamInfos[index].CpuUsage, streamInfos[index].IsFirst);
		}
		
		/// <summary>
		/// Copies all data to a different writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="progressReport">Used to report the progress of writing all DataSets, returning false tells WriteTo to cancel the process.</param>
		public void WriteTo(IProfilingDataWriter writer, Predicate<double> progressReport)
		{
			writer.ProcessorFrequency = this.processorFrequency;
			writer.WriteMappings(this.nameMappings.Values);
			for (int i = 0; i < this.DataSetCount; i++) {
				using (UnmanagedProfilingDataSet dataSet = this.LoadDataSet(i))
					writer.WriteDataSet(dataSet);
				
				if (!progressReport.Invoke((i + 1) / (double)this.DataSetCount))
					break;
			}
		}
		
		/*
		 * DOES NOT WORK:
		 * WE CANNOT GUARANTEE THAT THE DATASETS STAY IN THE CORRECT ORDER!
		/// <summary>
		/// Copies all data to a different writer using multiple threads. The number of threads used is the number of processors or cores available on the system.
		/// </summary>
		void ParallelWriteTo(IProfilingDataWriter writer, Predicate<double> progressReport)
		{
			Worker worker = new Worker();
			worker.DoWork(writer, this, progressReport);
		}
		
		sealed class Worker
		{
			int dataSetsFinished;
			int dataSetCount;
			
			public void DoWork(IProfilingDataWriter writer, TempFileDatabase database, Predicate<double> progressReport)
			{
				writer.ProcessorFrequency = database.processorFrequency;
				writer.WriteMappings(database.nameMappings.Values);
				
				Thread[] threads = new Thread[Environment.ProcessorCount];
				
				this.dataSetCount = database.DataSetCount;
				
				for (int i = 0; i < threads.Length; i++) {
					threads[i] = new Thread(new ThreadStart(() => WorkerThread(writer, database, progressReport)));
					threads[i].Start();
				}
				
				for (int i = 0; i < threads.Length; i++) {
					threads[i].Join();
				}
			}
			
			void WorkerThread(IProfilingDataWriter writer, TempFileDatabase database, Predicate<double> progressReport)
			{
				while (true) {
					int item;
					UnmanagedProfilingDataSet dataSet;
					lock (this) {
						if (dataSetsFinished >= database.DataSetCount)
							return;
						item = dataSetsFinished;
						dataSet = database.LoadDataSet(item);
					}
					
					writer.WriteDataSet(dataSet);
					dataSet.Dispose();
					
					lock (this) {
						dataSetsFinished++;
						if (!progressReport.Invoke(dataSetsFinished / (double)dataSetCount))
							break;
					}
				}
			}
		}*/
		
		/// <summary>
		/// Returns the number of DataSets stored in the database.
		/// </summary>
		public int DataSetCount {
			get { return this.streamInfos.Count; }
		}
		
		/// <summary>
		/// Disposes all unmanaged memory and files opened by the dataset.
		/// </summary>
		public void Dispose()
		{
			Close();
		}
	}
}
