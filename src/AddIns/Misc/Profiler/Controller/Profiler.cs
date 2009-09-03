// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Interprocess;
using Microsoft.Win32;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// The core class of the profiler.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
	public unsafe sealed partial class Profiler : IDisposable
	{
		[DllImport("kernel32", EntryPoint = "RtlZeroMemory"), System.Security.SuppressUnmanagedCodeSecurityAttribute]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		static extern void ZeroMemory(IntPtr dest, IntPtr size);

		[DllImport("Hook32.dll", EntryPoint = "rdtsc"), System.Security.SuppressUnmanagedCodeSecurityAttribute]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		static extern void Rdtsc32(out ulong value);

		static ulong GetRdtsc()
		{
			ulong value;
			if (UIntPtr.Size == 8)
				Rdtsc64(out value);
			else
				Rdtsc32(out value);
			return value;
		}

		bool is64Bit;
		bool isRunning;
		volatile bool stopDC;
		volatile bool enableDC;
		volatile bool isFirstDC;
		
		/// <summary>
		/// Gets whether the profiler is running inside a 64-bit profilee process or not.
		/// </summary>
		public bool Is64Bit
		{
			get { return is64Bit; }
		}
		
		/// <summary>
		/// Gets whether the profiler is running not.
		/// </summary>
		public bool IsRunning
		{
			get { return isRunning; }
		}
		
		/// <summary>
		/// Gets the processor frequency read from the registry of the computer the profiler is running on.
		/// </summary>
		public int ProcessorFrequency
		{
			get {
				if (this.is64Bit)
					return this.memHeader64->ProcessorFrequency;
				else
					return this.memHeader32->ProcessorFrequency;
			}
		}

		readonly string SharedMemoryId = "Local\\Profiler.SharedMemory.{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
		readonly string MutexId = "Local\\Profiler.Mutex.{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
		readonly string AccessEventId = "Local\\Profiler.Events.{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
		
		/// <summary>
		/// The Guid of the CProfiler class in the Hook.
		/// </summary>
		public const string ProfilerGuid = "{E7E2C111-3471-4AC7-B278-11F4C26EDBCF}";

		const int bufferSize = 4 * 1024; // 4 kb
		const int threadDataSize = 16 * 1024 * 1024; // 16 mb

		Process profilee;
		ProcessStartInfo psi;
		Mutex threadListMutex;
		EventWaitHandle accessEventHandle;
		Thread logger;
		Thread dataCollector;
		UnmanagedCircularBuffer nativeToManagedBuffer;
		IProfilingDataWriter dataWriter;
		
		Dictionary<string, PerformanceCounter> performanceCounters;
		PerformanceCounter cpuUsageCounter;
		
		/// <summary>
		/// The currently used data provider.
		/// </summary>
		public IProfilingDataWriter DataWriter
		{
			get { return dataWriter; }
		}
		
		UnmanagedMemory fullView;

		// needs to be stored in member variable to prevent GC from cleaning up the memory mapped file
		// before the profilee can open it.
		MemoryMappedFile file;
		
		ProfilerOptions profilerOptions = new ProfilerOptions();
		
		/// <summary>
		/// Gets all settings used by this profiler instance.
		/// </summary>
		public ProfilerOptions ProfilerOptions {
			get { return profilerOptions; }
		}

		SharedMemoryHeader32* memHeader32;
		SharedMemoryHeader64* memHeader64;

		StringBuilder profilerOutput;
		
		/// <summary>
		/// Invoked when the Hook could not be registered as a COM component.
		/// </summary>
		public event EventHandler RegisterFailed;

		void OnRegisterFailed(EventArgs e)
		{
			if (RegisterFailed != null) {
				RegisterFailed(this, e);
			}
		}
		
		/// <summary>
		/// Invoked when the Hook could not be deregistered from COM.
		/// </summary>
		public event EventHandler DeregisterFailed;

		void OnDeregisterFailed(EventArgs e)
		{
			if (DeregisterFailed != null) {
				DeregisterFailed(this, e);
			}
		}
		
		/// <summary>
		/// Invoked when any new output has been sent to the Controller.
		/// </summary>
		public event EventHandler OutputUpdated;
		
		/// <summary>
		/// Invoked when the session has started.
		/// </summary>
		public event EventHandler SessionStarted;

		void OnSessionStarted(EventArgs e)
		{
			if (SessionStarted != null) {
				SessionStarted(this, e);
			}
		}
		
		/// <summary>
		/// Invoked when the session has ended.
		/// </summary>
		public event EventHandler SessionEnded;

		void OnSessionEnded(EventArgs e)
		{
			if (SessionEnded != null) {
				SessionEnded(this, e);
			}
		}

		void OnOutputUpdated(EventArgs e)
		{
			if (OutputUpdated != null) {
				OutputUpdated(this, e);
			}
		}
		
		/// <summary>
		/// Contains the whole Profiler output during the last session.
		/// </summary>
		public string ProfilerOutput
		{
			get { return profilerOutput.ToString(); }
		}
		
		public void EnableDataCollection()
		{
			this.enableDC = true;
		}
		
		public void DisableDataCollection()
		{
			this.enableDC = false;
			this.isFirstDC = true;
		}
		
		/// <summary>
		/// Creates a new profiler using the path to an executable to profile and a data writer.
		/// </summary>
		public Profiler(string pathToExecutable, IProfilingDataWriter dataWriter, ProfilerOptions options)
			: this(new ProcessStartInfo(pathToExecutable), dataWriter, options)
		{
			if (!File.Exists(pathToExecutable))
				throw new FileNotFoundException("File not found!", pathToExecutable);

			this.psi.WorkingDirectory = Path.GetDirectoryName(pathToExecutable);
		}
		
		/// <summary>
		/// Creates a new profiler using a process start info of an executable and a data writer.
		/// </summary>
		public Profiler(ProcessStartInfo info, IProfilingDataWriter dataWriter, ProfilerOptions options)
		{
			if (dataWriter == null)
				throw new ArgumentNullException("dataWriter");

			if (info == null)
				throw new ArgumentNullException("info");
			
			if (!DetectBinaryType.IsDotNetExecutable(info.FileName))
				throw new ProfilerException("File is not a valid .NET executable file!");
			
			this.profilerOptions = options;

			this.is64Bit = DetectBinaryType.RunsAs64Bit(info.FileName);

			this.profilerOutput = new StringBuilder();
			this.performanceCounters = new Dictionary<string, PerformanceCounter>();
			this.dataWriter = dataWriter;

			this.threadListMutex = new Mutex(false, MutexId);
			this.accessEventHandle = new EventWaitHandle(true, EventResetMode.ManualReset, this.AccessEventId);

			this.psi = info;
			this.psi.UseShellExecute = false; // needed to get env vars working!
			this.psi.EnvironmentVariables["SharedMemoryName"] = SharedMemoryId;
			this.psi.EnvironmentVariables["MutexName"] = MutexId; // mutex for process pause/continue sychronization
			this.psi.EnvironmentVariables["AccessEventName"] = AccessEventId; // name for access event of controller
			this.psi.EnvironmentVariables["COR_ENABLE_PROFILING"] = "1"; // enable profiling; 0 = disable
			this.psi.EnvironmentVariables["COR_PROFILER"] = ProfilerGuid; // GUID for the profiler
			
			file = MemoryMappedFile.CreateSharedMemory(SharedMemoryId, profilerOptions.SharedMemorySize);

			fullView = file.MapView(0, profilerOptions.SharedMemorySize);
			
			this.dataWriter.ProcessorFrequency = GetProcessorFrequency();
			
			this.logger = new Thread(new ParameterizedThreadStart(Logging));
			this.logger.Name = "Logger";
			this.logger.IsBackground = true; // don't let the logger thread prevent our process from exiting

			this.dataCollector = new Thread(new ThreadStart(DataCollection));
			this.dataCollector.Name = "DataCollector";
			this.dataCollector.IsBackground = true;
		}

		void InitializeHeader32()
		{
			memHeader32 = (SharedMemoryHeader32*)fullView.Pointer;
			#if DEBUG
			// '~DBG'
			memHeader32->Magic = 0x7e444247;
			#else
			// '~SM1'
			memHeader32->Magic = 0x7e534d31;
			#endif
			memHeader32->TotalLength = profilerOptions.SharedMemorySize;
			memHeader32->NativeToManagedBufferOffset = Align(sizeof(SharedMemoryHeader32));
			memHeader32->ThreadDataOffset = Align(memHeader32->NativeToManagedBufferOffset + bufferSize);
			memHeader32->ThreadDataLength = threadDataSize;
			memHeader32->HeapOffset = Align(memHeader32->ThreadDataOffset + threadDataSize);
			memHeader32->HeapLength = profilerOptions.SharedMemorySize - memHeader32->HeapOffset;
			memHeader32->ProcessorFrequency = GetProcessorFrequency();
			memHeader32->DoNotProfileDotnetInternals = profilerOptions.DoNotProfileDotNetInternals;
			memHeader32->CombineRecursiveFunction = profilerOptions.CombineRecursiveFunction;
			
			if ((Int32)(fullView.Pointer + memHeader32->HeapOffset) % 8 != 0) {
				throw new DataMisalignedException("Heap is not aligned properly: " + ((Int32)(fullView.Pointer + memHeader32->HeapOffset)).ToString(CultureInfo.InvariantCulture) + "!");
			}
		}
		
		static int Align(int address)
		{
			return (address + 7) & ~(Int32)7;
		}

		static int GetProcessorFrequency()
		{
			int freq = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0", "~MHz", -1);
			if (freq == -1)
				throw new ArgumentException("The Processor Frequency could not be read!");
			return freq;
		}

		void DataCollection()
		{
			while (!stopDC) {
				this.Pause();
				this.threadListMutex.WaitOne();

				if (this.is64Bit)
					CollectData64();
				else
					CollectData32();

				this.threadListMutex.ReleaseMutex();
				this.Continue();
				Thread.Sleep(500);
			}
		}

		void CollectData32()
		{
			if (TranslatePointer(memHeader32->RootFuncInfoAddress) == null)
				return;

			ulong now = GetRdtsc();

			ThreadLocalData32* item = (ThreadLocalData32*)TranslatePointer(this.memHeader32->LastThreadListItem);

			List<Stack<int>> stackList = new List<Stack<int>>();

			while (item != null) {
				StackEntry32* entry = (StackEntry32*)TranslatePointer(item->Stack.Array);
				Stack<int> itemIDs = new Stack<int>();
				while (entry != null && entry <= (StackEntry32*)TranslatePointer(item->Stack.TopPointer)) {
					FunctionInfo* function = (FunctionInfo*)TranslatePointer(entry->Function);
					itemIDs.Push(function->Id);

					function->TimeSpent += now - entry->StartTime;

					entry++;
				}

				stackList.Add(itemIDs);

				item = (ThreadLocalData32*)TranslatePointer(item->Predecessor);
			}
			if (this.enableDC) {
				this.AddDataset(fullView.Pointer,
				                memHeader32->NativeAddress + memHeader32->HeapOffset,
				                memHeader32->Allocator.startPos - memHeader32->NativeAddress,
				                memHeader32->Allocator.pos - memHeader32->Allocator.startPos,
				                (cpuUsageCounter == null) ? 0 : cpuUsageCounter.NextValue(),
				                isFirstDC,
				                memHeader32->RootFuncInfoAddress);
				isFirstDC = false;
			}

			ZeroMemory(new IntPtr(TranslatePointer(memHeader32->Allocator.startPos)), new IntPtr(memHeader32->Allocator.pos - memHeader32->Allocator.startPos));

			memHeader32->Allocator.pos = memHeader32->Allocator.startPos;
			Allocator32.ClearFreeList(&memHeader32->Allocator);

			FunctionInfo* root = CreateFunctionInfo(0, 0, stackList.Count);

			memHeader32->RootFuncInfoAddress = TranslatePointerBack32(root);

			item = (ThreadLocalData32*)TranslatePointer(this.memHeader32->LastThreadListItem);

			now = GetRdtsc();

			foreach (Stack<int> thread in stackList) {
				FunctionInfo* child = null;

				StackEntry32* entry = (StackEntry32*)TranslatePointer(item->Stack.TopPointer);

				while (thread.Count > 0) {
					FunctionInfo* stackItem = CreateFunctionInfo(thread.Pop(), 0, child != null ? 1 : 0);

					if (child != null)
						FunctionInfo.AddOrUpdateChild32(stackItem, child, this);

					entry->Function = TranslatePointerBack32(stackItem);
					entry->StartTime = now;
					entry--;

					child = stackItem;
				}
				
				if (child != null)
					FunctionInfo.AddOrUpdateChild32(root, child, this);

				item = (ThreadLocalData32*)TranslatePointer(item->Predecessor);
			}
		}
		
		unsafe void AddDataset(byte *ptr, TargetProcessPointer nativeStartPosition, long offset, long length, double cpuUsage, bool isFirst, TargetProcessPointer nativeRootFuncInfoPosition)
		{
			using (DataSet dataSet = new DataSet(this, ptr + offset, length, nativeStartPosition, nativeRootFuncInfoPosition, cpuUsage, isFirst, is64Bit)) {
				lock (this.dataWriter) {
					this.dataWriter.WriteDataSet(dataSet);
				}
			}
		}

		FunctionInfo* CreateFunctionInfo(int id, int indexInParent, int elementCount)
		{
			int tableSize = 4;
			while (elementCount * 4 >= tableSize * 3)
				tableSize *= 2;

			// Allocate the child in memory
			FunctionInfo* newFunction = (FunctionInfo*)Malloc(sizeof(FunctionInfo) + tableSize * (is64Bit ? 8 : 4));

			// the allocater takes care of zeroing the memory

			// Set field values
			newFunction->Id = id;
			newFunction->TimeSpent = (ulong)indexInParent << 56;
			newFunction->TimeSpent |= (ulong)1 << 55;
			newFunction->FillCount = elementCount;
			// Initialize the table
			newFunction->LastChildIndex = tableSize - 1;
			// Return pointer to the created child

			return newFunction;
		}

		unsafe void* Malloc(int bytes)
		{
			return is64Bit ? Malloc64(bytes) : Malloc32(bytes);
		}

		unsafe void* Malloc32(int bytes)
		{
			#if DEBUG
			const int debuggingInfoSize = 8;
			bytes += debuggingInfoSize;
			#endif
			void* t = TranslatePointer(memHeader32->Allocator.pos);
			memHeader32->Allocator.pos += bytes;
			#if DEBUG
			t = (byte*)t + debuggingInfoSize;
			((Int32*)t)[-1] = bytes - debuggingInfoSize;
			#endif
			return t;
		}

		void Logging(object reader)
		{
			Stream stream = reader as Stream;
			string readString = ReadString(stream);
			while (readString != null) {
				readString = ReadString(stream);
				if (readString != null && !ProcessCommand(readString))
					this.LogString(readString);
			}
		}
		
		/// <summary>
		/// Starts a new profiling session. Prepares IPC and starts the process and logging.
		/// </summary>
		/// <returns>The process information of the profilee.</returns>
		public Process Start()
		{
			VerifyAccess();
			
			if (is64Bit)
				InitializeHeader64();
			else
				InitializeHeader32();
			
			isRunning = true;
			
			RegisterProfiler();

			if (is64Bit) {
				nativeToManagedBuffer = UnmanagedCircularBuffer.Create(
					new IntPtr(fullView.Pointer + memHeader64->NativeToManagedBufferOffset), bufferSize);
				if ((Int64)(fullView.Pointer + memHeader64->NativeToManagedBufferOffset) % 8 != 0) {
					throw new DataMisalignedException("nativeToManagedBuffer is not properly aligned!");
				}
			} else {
				nativeToManagedBuffer = UnmanagedCircularBuffer.Create(
					new IntPtr(fullView.Pointer + memHeader32->NativeToManagedBufferOffset), bufferSize);
				if ((Int32)(fullView.Pointer + memHeader32->NativeToManagedBufferOffset) % 8 != 0) {
					throw new DataMisalignedException("nativeToManagedBuffer is not properly aligned!");
				}
			}

			if (is64Bit)
				LogString("Using 64-bit hook.");
			LogString("Starting process, waiting for profiler hook...");

			this.profilee = new Process();

			this.profilee.EnableRaisingEvents = true;
			this.profilee.StartInfo = this.psi;
			this.profilee.Exited += new EventHandler(ProfileeExited);
			
			this.enableDC = this.profilerOptions.EnableDCAtStart;
			this.isFirstDC = true;
			
			Debug.WriteLine("Launching profiler for " + this.psi.FileName + "...");
			this.profilee.Start();
			
			this.cpuUsageCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", ".");
			
			this.logger.Start(nativeToManagedBuffer.CreateReadingStream());

			// GC references currentSession
			if (this.profilerOptions.EnableDC) {
				this.dataCollector.Start();
			}

			OnSessionStarted(EventArgs.Empty);
			return profilee;
		}
		
		/// <summary>
		/// Halts the profilee process.
		/// </summary>
		void Pause()
		{
			this.accessEventHandle.Reset();
			if (is64Bit)
				this.memHeader64->ExclusiveAccess = 1;
			else
				this.memHeader32->ExclusiveAccess = 1;
			Thread.MemoryBarrier();
			if (is64Bit)
				while (!AllThreadsWait64()) ;
			else
				while (!AllThreadsWait32()) ;
		}

		bool AllThreadsWait32()
		{
			this.threadListMutex.WaitOne();

			bool isWaiting = true;

			ThreadLocalData32* item = (ThreadLocalData32*)TranslatePointer(this.memHeader32->LastThreadListItem);

			while (item != null) {
				if (item->InLock == 1)
					isWaiting = false;

				item = (ThreadLocalData32*)TranslatePointer(item->Predecessor);
			}

			this.threadListMutex.ReleaseMutex();

			return isWaiting;
		}
		
		/// <summary>
		/// Continues execution of the profilee.
		/// </summary>
		void Continue()
		{
			if (is64Bit)
				this.memHeader64->ExclusiveAccess = 0;
			else
				this.memHeader32->ExclusiveAccess = 0;
			this.accessEventHandle.Set();
		}

		unsafe void ProfileeExited(object sender, EventArgs e)
		{
			if (isDisposed)
				return;

			DeregisterProfiler();

			this.stopDC = true;

			Debug.WriteLine("Closing native to managed buffer");
			nativeToManagedBuffer.Close();

			Debug.WriteLine("Joining logger thread...");
			this.logger.Join();
			Debug.WriteLine("Logger thread joined!");
			if (this.profilerOptions.EnableDC)
				this.dataCollector.Join();
			
			// unload all counters to prevent exception during last collection!
			this.cpuUsageCounter = null;
			this.performanceCounters = null;
			
			// Take last shot
			if (this.is64Bit)
				CollectData64();
			else
				CollectData32();
			
			isRunning = false;
			
			this.dataWriter.Close();
			
			OnSessionEnded(EventArgs.Empty);
		}

		internal void LogString(string text)
		{
			this.profilerOutput.AppendLine(text);
			OnOutputUpdated(EventArgs.Empty);
		}

		internal unsafe void* TranslatePointer32(TargetProcessPointer32 ptr)
		{
			if (ptr.Pointer == 0)
				return null;
			// Use Int32 instead of int because of preprocessor!
			unchecked {
				Int32 spaceDiff = (Int32)(new IntPtr(fullView.Pointer)) - (Int32)memHeader32->NativeAddress.Pointer;
				return new IntPtr((Int32)ptr.Pointer + spaceDiff).ToPointer();
			}
		}

		internal unsafe void* TranslatePointer(TargetProcessPointer ptr)
		{
			if (this.is64Bit)
				return TranslatePointer64(ptr.To64());
			else
				return TranslatePointer32(ptr.To32());
		}

		internal unsafe TargetProcessPointer32 TranslatePointerBack32(void* ptr)
		{
			// Use Int32 instead of int because of preprocessor!
			if (ptr == null)
				return new TargetProcessPointer32();
			unchecked {
				Int32 spaceDiff = (Int32)(new IntPtr(fullView.Pointer)) - (Int32)memHeader32->NativeAddress.Pointer;
				TargetProcessPointer32 pointer = new TargetProcessPointer32();
				pointer.Pointer = (UInt32)((Int32)ptr - spaceDiff);
				return pointer;
			}
		}

		/// <summary>
		/// Verifies that the profiler is not disposed (and the shared memory is still available)
		/// </summary>
		void VerifyAccess()
		{
			if (isDisposed)
				throw new ObjectDisposedException("Profiler");
		}

		bool ProcessCommand(string readString)
		{
			if (readString == null)
				return false;
			
			if (readString.StartsWith("map ", StringComparison.Ordinal)) {
				IList<string> parts = readString.SplitSeparatedString(' ');
				
				if (parts.Count < 3)
					return false;
				
				int id = int.Parse(parts[1], CultureInfo.InvariantCulture);
				string name = parts[4];
				string returnType = parts[3];
				IList<string> parameters = parts.Skip(5).ToList();

				lock (this.dataWriter) {
					this.dataWriter.WriteMappings(new NameMapping[] {new NameMapping(id, returnType, name, parameters)});
				}

				return true;
			} else if (readString.StartsWith("mapthread ", StringComparison.Ordinal)) {
				IList<string> parts = readString.SplitSeparatedString(' ');
				
				if (parts.Count < 5)
					return false;
				
				int id = int.Parse(parts[1], CultureInfo.InvariantCulture);
				string name = parts[3] + ((string.IsNullOrEmpty(parts[4])) ? "" : " - " + parts[4]);
				
				lock (this.dataWriter) {
					this.dataWriter.WriteMappings(new NameMapping[] {new NameMapping(id, null, name, null)});
				}

				return true;
			} else {
				if (readString.StartsWith("error-", StringComparison.Ordinal)) {
					string[] parts = readString.Split('-');
					if (parts.Length > 1)
						throw new ProfilerException(parts[1]);
					
					throw new ProfilerException("unknown error");
				}
			}

			return false;
		}
		
		/// <summary>
		/// Stops execution of the profilee.
		/// </summary>
		public void Stop()
		{
			try {
				if (profilee != null)
					profilee.Kill();
			} catch (InvalidOperationException) { }
		}

		void RegisterProfiler()
		{
			string hookDll = is64Bit ? "Hook64.dll" : "Hook32.dll";
			string path = Path.Combine(Path.GetDirectoryName(typeof(Profiler).Assembly.Location), hookDll);
			if (!Registrar.Register(ProfilerGuid, "ProfilerLib", "CProfiler", path, is64Bit))
				OnRegisterFailed(EventArgs.Empty);
		}

		void DeregisterProfiler()
		{
			if (!Registrar.Deregister(ProfilerGuid, is64Bit))
				OnDeregisterFailed(EventArgs.Empty);
		}

		static int ReadInt(Stream s)
		{
			int value = 0;
			int tmp = s.ReadByte();
			if (tmp < 0)
				return -1;
			value = tmp;
			tmp = s.ReadByte();
			if (tmp < 0)
				return -1;
			value += (tmp << 8);
			tmp = s.ReadByte();
			if (tmp < 0)
				return -1;
			value += (tmp << 16);
			tmp = s.ReadByte();
			if (tmp < 0)
				return -1;
			value += (tmp << 24);

			return value;
		}

		static string ReadString(Stream s)
		{
			int len = ReadInt(s);
			if (len == -1)
				return null;
			byte[] bytes = new byte[len];
			int offset = 0;
			while (offset < len) {
				int r = s.Read(bytes, offset, len - offset);
				offset += r;
				if (r == 0)
					break;
			}
			return Encoding.Unicode.GetString(bytes, 0, offset);
		}

		#region IDisposable Member
		bool isDisposed;
		
		/// <summary>
		/// Shuts down the profilee and stops logging and closes and IPC.
		/// </summary>
		public void Dispose()
		{
			if (!isDisposed) {
				isDisposed = true;
				stopDC = true;
				nativeToManagedBuffer.Close();
				try {
					this.profilee.Kill();
				} catch (InvalidOperationException) {
					// can happen if profilee has already exited
				}
				if (logger != null && logger.IsAlive) {
					this.logger.Join();
				}

				if (dataCollector != null && dataCollector.IsAlive) {
					this.dataCollector.Join();
				}

				this.fullView.Dispose();
				this.file.Close();
				
				this.threadListMutex.Close();
				this.accessEventHandle.Close();
				
				this.dataWriter.Close();

				this.profilee.Dispose();
			}
		}

		#endregion
		
		#region UnmanagedProfilingDataSet implementation
		sealed class DataSet : UnmanagedProfilingDataSet
		{
			Profiler profiler;
			
			public DataSet(Profiler profiler, byte *startPtr, long length, TargetProcessPointer nativeStartPosition,
			               TargetProcessPointer nativeRootFuncInfoPosition,
			               double cpuUsage, bool isFirst, bool is64Bit)
				: base(nativeStartPosition, nativeRootFuncInfoPosition, startPtr, length, cpuUsage, isFirst, is64Bit)
			{
				this.profiler = profiler;
			}
			
			public override NameMapping GetMapping(int nameId)
			{
				return new NameMapping(nameId);
			}
			
			public override int ProcessorFrequency {
				get {
					return this.profiler.ProcessorFrequency;
				}
			}
			
			internal unsafe override void* TranslatePointer(TargetProcessPointer ptr)
			{
				return this.profiler.TranslatePointer(ptr);
			}
		}
		#endregion
	}
}