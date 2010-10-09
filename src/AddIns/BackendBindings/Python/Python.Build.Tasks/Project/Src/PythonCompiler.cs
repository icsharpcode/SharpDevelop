// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Resources;

using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace ICSharpCode.Python.Build.Tasks
{
	/// <summary>
	/// Wraps the IronPython.Hosting.PythonCompiler class so it 
	/// implements the IPythonCompiler interface.
	/// </summary>
	public class PythonCompiler : IPythonCompiler
	{
		IList<string> sourceFiles;
		IList<string> referencedAssemblies;
		IList<ResourceFile> resourceFiles;
		PEFileKinds targetKind = PEFileKinds.Dll;
		PortableExecutableKinds executableKind = PortableExecutableKinds.ILOnly;
		ImageFileMachine machine = ImageFileMachine.I386;
		string mainFile = String.Empty;
		bool includeDebugInformation;
		string outputAssembly = String.Empty;
		
		public PythonCompiler()
		{
		}
		
		public IList<string> SourceFiles {
			get { return sourceFiles; }
			set { sourceFiles = value; }
		}
		
		public IList<string> ReferencedAssemblies {
			get { return referencedAssemblies; }
			set { referencedAssemblies = value; }
		}
		
		public IList<ResourceFile> ResourceFiles {
			get { return resourceFiles; }
			set { resourceFiles = value; }
		}
		
		public PEFileKinds TargetKind {
			get { return targetKind; }
			set { targetKind = value; }
		}
		
		public PortableExecutableKinds ExecutableKind {
			get { return executableKind; }
			set { executableKind = value; }
		}
		
		public ImageFileMachine Machine {
			get { return machine; }
			set { machine = value; }
		}
		
		public string MainFile {
			get { return mainFile; }
			set { mainFile = value; }
		}
		
		public string OutputAssembly {
			get { return outputAssembly; }
			set { outputAssembly = value; }
		}
		
		public bool IncludeDebugInformation {
			get { return includeDebugInformation; }
			set { includeDebugInformation = value; }
		}
		
		/// <summary>
		/// The compilation requires us to change into the compile output folder since the
		/// AssemblyBuilder.Save does not use a full path when generating the assembly.
		/// </summary>
		public void Compile()
		{
			VerifyParameters();
			
			// Compile the source files to a dll first.
			ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("mainModule", mainFile);
			string outputAssemblyDll = Path.ChangeExtension(outputAssembly, ".dll");
			ClrModule.CompileModules(DefaultContext.Default, outputAssemblyDll, dictionary, ToStringArray(sourceFiles));
	
			// Generate an executable if required.
			if (targetKind != PEFileKinds.Dll) {
				// Change into compilation folder.
				string originalFolder = Directory.GetCurrentDirectory();
				try {
					string compileFolder = Path.Combine(originalFolder, Path.GetDirectoryName(outputAssembly));
					Directory.SetCurrentDirectory(compileFolder);
					GenerateExecutable(outputAssemblyDll);
				} finally {
					Directory.SetCurrentDirectory(originalFolder);
				}
			}
		}
		
		/// <summary>
		/// Verifies the compiler parameters that have been set correctly.
		/// </summary>
		public void VerifyParameters()
		{
			if ((mainFile == null) && (targetKind != PEFileKinds.Dll)) {
				throw new PythonCompilerException(Resources.NoMainFileSpecified);
			}
		}
		
		public void Dispose()
		{
		}
		
		/// <summary>
		/// Generates an executable from the already compiled dll.
		/// </summary>
		void GenerateExecutable(string outputAssemblyDll)
		{
			string outputAssemblyFileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputAssembly);
			AssemblyName assemblyName = new AssemblyName(outputAssemblyFileNameWithoutExtension); 
			AssemblyBuilder assemblyBuilder = PythonOps.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(outputAssemblyFileNameWithoutExtension, assemblyName.Name + ".exe");
			TypeBuilder typeBuilder = moduleBuilder.DefineType("PythonMain", TypeAttributes.Public);
			MethodBuilder mainMethod = typeBuilder.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[0]);
			
			MarkMainMethodAsSTA(mainMethod);
			GenerateMainMethodBody(mainMethod, outputAssemblyDll);
			
			// Add resources.
			AddResources(moduleBuilder);

			// Create executable.
			typeBuilder.CreateType();
			assemblyBuilder.SetEntryPoint(mainMethod, targetKind);
			assemblyBuilder.Save(assemblyName.Name + ".exe", executableKind, machine);
		}
		
		void MarkMainMethodAsSTA(MethodBuilder mainMethod)
		{
			mainMethod.SetCustomAttribute(typeof(STAThreadAttribute).GetConstructor(Type.EmptyTypes), new byte[0]);
		}
		
		void GenerateMainMethodBody(MethodBuilder mainMethod, string outputAssemblyDll)
		{
			ILGenerator generator = mainMethod.GetILGenerator();
			LocalBuilder exeAssemblyLocalVariable = generator.DeclareLocal(typeof(Assembly));
			LocalBuilder directoryLocalVariable = generator.DeclareLocal(typeof(string));
			LocalBuilder fileNameLocalVariable = generator.DeclareLocal(typeof(string));
			
			generator.EmitCall(OpCodes.Call, typeof(Assembly).GetMethod("GetExecutingAssembly", new Type[0], new ParameterModifier[0]), null);
			generator.Emit(OpCodes.Stloc_0);
			
			generator.Emit(OpCodes.Ldloc_0);
			generator.EmitCall(OpCodes.Callvirt, typeof(Assembly).GetMethod("get_Location"), null);
			generator.EmitCall(OpCodes.Call, typeof(Path).GetMethod("GetDirectoryName", new Type[] {typeof(String)}, new ParameterModifier[0]), null);
			generator.Emit(OpCodes.Stloc_1);
			
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldstr, Path.GetFileName(outputAssemblyDll));
			generator.EmitCall(OpCodes.Call, typeof(Path).GetMethod("Combine", new Type[] {typeof(String), typeof(String)}, new ParameterModifier[0]), null);
			generator.Emit(OpCodes.Stloc_2);
			
			generator.Emit(OpCodes.Ldloc_2);
			generator.EmitCall(OpCodes.Call, typeof(Assembly).GetMethod("LoadFile", new Type[] {typeof(String)}, new ParameterModifier[0]), null);			
			generator.Emit(OpCodes.Ldstr, Path.GetFileNameWithoutExtension(mainFile));

			// Add referenced assemblies.
			AddReferences(generator);

			generator.EmitCall(OpCodes.Call, typeof(PythonOps).GetMethod("InitializeModule"), new Type[0]);
			generator.Emit(OpCodes.Ret);
		}
		
		/// <summary>
		/// Converts an IList<string> into a string[].
		/// </summary>
		string[] ToStringArray(IList<string> items)
		{
			string[] array = new string[items.Count];
			items.CopyTo(array, 0);
			return array;
		}

		/// <summary>
		/// Adds reference information to the IL.
		/// </summary>
		void AddReferences(ILGenerator generator)
		{
			if (referencedAssemblies.Count > 0) {
				generator.Emit(OpCodes.Ldc_I4, referencedAssemblies.Count);
				generator.Emit(OpCodes.Newarr, typeof(String));
				
				for (int i = 0; i < referencedAssemblies.Count; ++i) {
					generator.Emit(OpCodes.Dup);
					generator.Emit(OpCodes.Ldc_I4, i);
					string assemblyFileName = referencedAssemblies[i];
					Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFileName);
					generator.Emit(OpCodes.Ldstr, assembly.FullName);
					generator.Emit(OpCodes.Stelem_Ref);
				}
			} else {
				generator.Emit(OpCodes.Ldnull);
			}			
		}
		
		/// <summary>
		/// Embeds resources into the assembly.
		/// </summary>
		void AddResources(ModuleBuilder moduleBuilder)
		{
			foreach (ResourceFile resourceFile in resourceFiles) {
				AddResource(moduleBuilder, resourceFile);
			}
		}
		
		/// <summary>
		/// Embeds a single resource into the assembly.
		/// </summary>
		void AddResource(ModuleBuilder moduleBuilder, ResourceFile resourceFile)
		{
			string fileName = resourceFile.FileName;
			string extension = Path.GetExtension(fileName).ToLowerInvariant();
			if (extension == ".resources") {
				string fullFileName = Path.GetFileName(fileName);
				IResourceWriter resourceWriter = moduleBuilder.DefineResource(fullFileName, resourceFile.Name, ResourceAttributes.Public);
				AddResources(resourceWriter, fileName);
			} else {
				moduleBuilder.DefineManifestResource(resourceFile.Name, new FileStream(fileName, FileMode.Open), ResourceAttributes.Public);
			}
		}
		
		void AddResources(IResourceWriter resourceWriter, string fileName)
		{
			ResourceReader resourceReader = new ResourceReader(fileName);
			using (resourceReader) {
				IDictionaryEnumerator enumerator = resourceReader.GetEnumerator();
				while (enumerator.MoveNext()) {
					string key = enumerator.Key as string;
					Stream resourceStream = enumerator.Value as Stream;
					if (resourceStream != null) {
						BinaryReader reader = new BinaryReader(resourceStream);
						MemoryStream stream = new MemoryStream();						
						byte[] bytes = reader.ReadBytes((int)resourceStream.Length);
						stream.Write(bytes, 0, bytes.Length);
						resourceWriter.AddResource(key, stream);
					} else {
						resourceWriter.AddResource(key, enumerator.Value);
					}
				}
			}
		}
	}
}
