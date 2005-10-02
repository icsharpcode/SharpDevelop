// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// This class can write Dom entity into a binary file for fast loading.
	/// </summary>
	public static class DomPersistence
	{
		public const long FileMagic = 0x11635233ED2F428C;
		public const long IndexFileMagic = 0x11635233ED2F427D;
		public const short FileVersion = 3;
		
		#region Cache management
		#if DEBUG
		const string tempPathName = "SharpDevelop/DomCacheDebug";
		#else
		const string tempPathName = "SharpDevelop/DomCache";
		#endif
		
		static string MakeTempPath()
		{
			string tempPath = Path.Combine(Path.GetTempPath(), tempPathName);
			if (!Directory.Exists(tempPath))
				Directory.CreateDirectory(tempPath);
			return tempPath;
		}
		
		public static string SaveProjectContent(ReflectionProjectContent pc)
		{
			string assemblyFullName = pc.AssemblyFullName;
			int pos = assemblyFullName.IndexOf(',');
			string fileName = Path.Combine(MakeTempPath(),
			                               assemblyFullName.Substring(0, pos)
			                               + "." + assemblyFullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture)
			                               + "." + pc.AssemblyLocation.GetHashCode().ToString("x", CultureInfo.InvariantCulture)
			                               + ".dat");
			AddFileNameToCacheIndex(Path.GetFileName(fileName), pc);
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				using (BinaryWriter writer = new BinaryWriter(fs)) {
					new ReadWriteHelper(writer).WriteProjectContent(pc);
				}
			}
			return fileName;
		}
		
		public static ReflectionProjectContent LoadProjectContentByAssemblyName(string assemblyName)
		{
			string cacheFileName;
			if (CacheIndex.TryGetValue(assemblyName, out cacheFileName)) {
				cacheFileName = Path.Combine(MakeTempPath(), cacheFileName);
				if (File.Exists(cacheFileName)) {
					return LoadProjectContent(cacheFileName);
				}
			}
			return null;
		}
		
		public static ReflectionProjectContent LoadProjectContent(string cacheFileName)
		{
			ReflectionProjectContent pc;
			using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read)) {
				using (BinaryReader reader = new BinaryReader(fs)) {
					pc = new ReadWriteHelper(reader).ReadProjectContent();
				}
			}
			if (pc != null) {
				pc.InitializeSpecialClasses();
			}
			return pc;
		}
		#endregion
		
		#region Cache index
		static string GetIndexFileName() { return Path.Combine(MakeTempPath(), "index.dat"); }
		
		static Dictionary<string, string> cacheIndex;
		
		static Dictionary<string, string> CacheIndex {
			get {
				if (cacheIndex == null) {
					cacheIndex = LoadCacheIndex();
				}
				return cacheIndex;
			}
		}
		
		static Dictionary<string, string> LoadCacheIndex()
		{
			string indexFile = GetIndexFileName();
			Dictionary<string, string> list = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			if (File.Exists(indexFile)) {
				using (FileStream fs = new FileStream(indexFile, FileMode.Open, FileAccess.Read)) {
					using (BinaryReader reader = new BinaryReader(fs)) {
						if (reader.ReadInt64() != IndexFileMagic) {
							LoggingService.Warn("Index cache has wrong file magic");
							return list;
						}
						if (reader.ReadInt16() != FileVersion) {
							LoggingService.Warn("Index cache has wrong file version");
							return list;
						}
						int count = reader.ReadInt32();
						for (int i = 0; i < count; i++) {
							string key = reader.ReadString();
							list[key] = reader.ReadString();
						}
						return list;
					}
				}
			} else {
				return list;
			}
		}
		
		static void SaveCacheIndex(Dictionary<string, string> cacheIndex)
		{
			string indexFile = GetIndexFileName();
			using (FileStream fs = new FileStream(indexFile, FileMode.Create, FileAccess.Write)) {
				using (BinaryWriter writer = new BinaryWriter(fs)) {
					writer.Write(IndexFileMagic);
					writer.Write(FileVersion);
					writer.Write(cacheIndex.Count);
					foreach (KeyValuePair<string, string> e in cacheIndex) {
						writer.Write(e.Key);
						writer.Write(e.Value);
					}
				}
			}
		}
		
		static void AddFileNameToCacheIndex(string cacheFile, ReflectionProjectContent pc)
		{
			Dictionary<string, string> l = LoadCacheIndex();
			l[pc.AssemblyLocation] = cacheFile;
			string txt = pc.AssemblyFullName;
			l[txt] = cacheFile;
			int pos = txt.LastIndexOf(',');
			do {
				txt = txt.Substring(0, pos);
				if (l.ContainsKey(txt))
					break;
				l[txt] = cacheFile;
				pos = txt.LastIndexOf(',');
			} while (pos >= 0);
			SaveCacheIndex(l);
			cacheIndex = l;
		}
		#endregion
		
		private struct ClassNameTypeCountPair {
			public readonly string ClassName;
			public readonly byte TypeParameterCount;
			
			public ClassNameTypeCountPair(IClass c) {
				this.ClassName = c.FullyQualifiedName;
				this.TypeParameterCount = (byte)c.TypeParameters.Count;
			}
			
			public ClassNameTypeCountPair(IReturnType rt) {
				this.ClassName = rt.FullyQualifiedName;
				this.TypeParameterCount = (byte)rt.TypeParameterCount;
			}
			
			public override bool Equals(object obj) {
				if (!(obj is ClassNameTypeCountPair)) return false;
				ClassNameTypeCountPair myClassNameTypeCountPair = (ClassNameTypeCountPair)obj;
				if (!ClassName.Equals(myClassNameTypeCountPair.ClassName, StringComparison.InvariantCultureIgnoreCase)) return false;
				if (TypeParameterCount != myClassNameTypeCountPair.TypeParameterCount) return false;
				return true;
			}
			
			public override int GetHashCode() {
				return StringComparer.InvariantCultureIgnoreCase.GetHashCode(ClassName) ^ ((int)TypeParameterCount * 5);
			}
		}
		
		private sealed class ReadWriteHelper
		{
			ReflectionProjectContent pc;
			
			readonly BinaryWriter writer;
			readonly Dictionary<ClassNameTypeCountPair, int> classIndices = new Dictionary<ClassNameTypeCountPair, int>();
			
			readonly BinaryReader reader;
			IReturnType[] types;
			
			#region Write/Read ProjectContent
			public ReadWriteHelper(BinaryWriter writer)
			{
				this.writer = writer;
			}
			
			public void WriteProjectContent(ReflectionProjectContent pc)
			{
				this.pc = pc;
				writer.Write(FileMagic);
				writer.Write(FileVersion);
				writer.Write(pc.AssemblyFullName);
				writer.Write(pc.AssemblyLocation);
				long time = 0;
				try {
					time = File.GetLastWriteTimeUtc(pc.AssemblyLocation).ToFileTime();
				} catch {}
				writer.Write(time);
				writer.Write(pc.ReferencedAssemblies.Length);
				foreach (AssemblyName name in pc.ReferencedAssemblies) {
					writer.Write(name.FullName);
				}
				WriteClasses();
			}
			
			public ReadWriteHelper(BinaryReader reader)
			{
				this.reader = reader;
			}
			
			public ReflectionProjectContent ReadProjectContent()
			{
				if (reader.ReadInt64() != FileMagic) {
					LoggingService.Warn("Read dom: wrong magic");
					return null;
				}
				if (reader.ReadInt16() != FileVersion) {
					LoggingService.Warn("Read dom: wrong version");
					return null;
				}
				string assemblyName = reader.ReadString();
				string assemblyLocation = reader.ReadString();
				long time = 0;
				try {
					time = File.GetLastWriteTimeUtc(assemblyLocation).ToFileTime();
				} catch {}
				if (reader.ReadInt64() != time) {
					LoggingService.Warn("Read dom: assembly changed since cache was created");
					return null;
				}
				AssemblyName[] referencedAssemblies = new AssemblyName[reader.ReadInt32()];
				for (int i = 0; i < referencedAssemblies.Length; i++) {
					referencedAssemblies[i] = new AssemblyName(reader.ReadString());
				}
				this.pc = new ReflectionProjectContent(assemblyName, assemblyLocation, referencedAssemblies);
				ReadClasses();
				return pc;
			}
			
			void WriteClasses()
			{
				ICollection<IClass> classes = pc.Classes;
				
				classIndices.Clear();
				int i = 0;
				foreach (IClass c in classes) {
					classIndices[new ClassNameTypeCountPair(c)] = i;
					i += 1;
				}
				
				List<ClassNameTypeCountPair> externalTypes = new List<ClassNameTypeCountPair>();
				CreateExternalTypeList(externalTypes, classes.Count, classes);
				
				writer.Write(classes.Count);
				writer.Write(externalTypes.Count);
				foreach (IClass c in classes) {
					writer.Write(c.FullyQualifiedName);
				}
				foreach (ClassNameTypeCountPair type in externalTypes) {
					writer.Write(type.ClassName);
					writer.Write(type.TypeParameterCount);
				}
				foreach (IClass c in classes) {
					WriteClass(c);
				}
			}
			
			void ReadClasses()
			{
				int classCount = reader.ReadInt32();
				int externalTypeCount = reader.ReadInt32();
				types = new IReturnType[classCount + externalTypeCount];
				DefaultClass[] classes = new DefaultClass[classCount];
				for (int i = 0; i < classes.Length; i++) {
					DefaultClass c = new DefaultClass(pc.AssemblyCompilationUnit, reader.ReadString());
					classes[i] = c;
					types[i] = c.DefaultReturnType;
				}
				for (int i = classCount; i < types.Length; i++) {
					string name = reader.ReadString();
					types[i] = new GetClassReturnType(pc, name, reader.ReadByte());
				}
				for (int i = 0; i < classes.Length; i++) {
					ReadClass(classes[i]);
					pc.AddClassToNamespaceList(classes[i]);
				}
			}
			#endregion
			
			#region Write/Read Class
			IClass currentClass;
			
			void WriteClass(IClass c)
			{
				this.currentClass = c;
				WriteTemplates(c.TypeParameters);
				writer.Write(c.BaseTypes.Count);
				foreach (IReturnType type in c.BaseTypes) {
					WriteType(type);
				}
				writer.Write((int)c.Modifiers);
				writer.Write((byte)c.ClassType);
				WriteAttributes(c.Attributes);
				writer.Write(c.InnerClasses.Count);
				foreach (IClass innerClass in c.InnerClasses) {
					WriteString(innerClass.FullyQualifiedName);
					WriteClass(innerClass);
				}
				this.currentClass = c;
				writer.Write(c.Methods.Count);
				foreach (IMethod method in c.Methods) {
					WriteMethod(method);
				}
				writer.Write(c.Properties.Count);
				foreach (IProperty property in c.Properties) {
					WriteProperty(property);
				}
				writer.Write(c.Events.Count);
				foreach (IEvent evt in c.Events) {
					WriteEvent(evt);
				}
				writer.Write(c.Fields.Count);
				foreach (IField field in c.Fields) {
					WriteField(field);
				}
				this.currentClass = null;
			}
			
			void WriteTemplates(List<ITypeParameter> list)
			{
				// read code exists twice: in ReadClass and ReadMethod
				writer.Write((byte)list.Count);
				foreach (ITypeParameter typeParameter in list) {
					WriteString(typeParameter.Name);
				}
				foreach (ITypeParameter typeParameter in list) {
					writer.Write(typeParameter.Constraints.Count);
					foreach (IReturnType type in typeParameter.Constraints) {
						WriteType(type);
					}
				}
			}
			
			void ReadClass(DefaultClass c)
			{
				this.currentClass = c;
				int count;
				count = reader.ReadByte();
				for (int i = 0; i < count; i++) {
					c.TypeParameters.Add(new DefaultTypeParameter(c, ReadString(), i));
				}
				foreach (ITypeParameter typeParameter in c.TypeParameters) {
					count = reader.ReadInt32();
					for (int i = 0; i < count; i++) {
						typeParameter.Constraints.Add(ReadType());
					}
				}
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					c.BaseTypes.Add(ReadType());
				}
				c.Modifiers = (ModifierEnum)reader.ReadInt32();
				c.ClassType = (ClassType)reader.ReadByte();
				ReadAttributes(c.Attributes);
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					DefaultClass innerClass = new DefaultClass(c.CompilationUnit, c);
					innerClass.FullyQualifiedName = ReadString();
					c.InnerClasses.Add(innerClass);
					ReadClass(innerClass);
				}
				this.currentClass = c;
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					c.Methods.Add(ReadMethod());
				}
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					c.Properties.Add(ReadProperty());
				}
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					c.Events.Add(ReadEvent());
				}
				count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					c.Fields.Add(ReadField());
				}
				this.currentClass = null;
			}
			#endregion
			
			#region Write/Read return types
			/// <summary>
			/// Finds all return types used in the class collection and adds the unknown ones
			/// to the externalTypeIndices and externalTypes collections.
			/// </summary>
			void CreateExternalTypeList(List<ClassNameTypeCountPair> externalTypes,
			                            int classCount, ICollection<IClass> classes)
			{
				foreach (IClass c in classes) {
					CreateExternalTypeList(externalTypes, classCount, c.InnerClasses);
					foreach (IReturnType returnType in c.BaseTypes) {
						AddExternalType(returnType, externalTypes, classCount);
					}
					foreach (ITypeParameter tp in c.TypeParameters) {
						foreach (IReturnType returnType in tp.Constraints) {
							AddExternalType(returnType, externalTypes, classCount);
						}
					}
					foreach (IField f in c.Fields) {
						AddExternalType(f.ReturnType, externalTypes, classCount);
					}
					foreach (IEvent f in c.Events) {
						AddExternalType(f.ReturnType, externalTypes, classCount);
					}
					foreach (IProperty p in c.Properties) {
						AddExternalType(p.ReturnType, externalTypes, classCount);
						foreach (IParameter parameter in p.Parameters) {
							AddExternalType(parameter.ReturnType, externalTypes, classCount);
						}
					}
					foreach (IMethod m in c.Methods) {
						AddExternalType(m.ReturnType, externalTypes, classCount);
						foreach (IParameter parameter in m.Parameters) {
							AddExternalType(parameter.ReturnType, externalTypes, classCount);
						}
						foreach (ITypeParameter tp in m.TypeParameters) {
							foreach (IReturnType returnType in tp.Constraints) {
								AddExternalType(returnType, externalTypes, classCount);
							}
						}
					}
				}
			}
			
			void AddExternalType(IReturnType rt, List<ClassNameTypeCountPair> externalTypes, int classCount)
			{
				if (rt.IsDefaultReturnType) {
					ClassNameTypeCountPair pair = new ClassNameTypeCountPair(rt);
					if (!classIndices.ContainsKey(pair)) {
						classIndices.Add(pair, externalTypes.Count + classCount);
						externalTypes.Add(pair);
					}
				} else if (rt is ArrayReturnType) {
					AddExternalType(((ArrayReturnType)rt).ElementType, externalTypes, classCount);
				} else if (rt is ConstructedReturnType) {
					ConstructedReturnType crt = (ConstructedReturnType)rt;
					AddExternalType(crt.BaseType, externalTypes, classCount);
					foreach (IReturnType typeArgument in crt.TypeArguments) {
						AddExternalType(typeArgument, externalTypes, classCount);
					}
				} else if (rt is GenericReturnType) {
					// ignore
				} else {
					LoggingService.Warn("Unknown return type: " + rt.ToString());
				}
			}
			
			const int ArrayRTCode         = -1;
			const int ConstructedRTCode   = -2;
			const int TypeGenericRTCode   = -3;
			const int MethodGenericRTCode = -4;
			const int NullRTReferenceCode = -5;
			const int VoidRTCode          = -6;
			
			void WriteType(IReturnType rt)
			{
				if (rt == null) {
					writer.Write(NullRTReferenceCode);
					return;
				}
				if (rt.IsDefaultReturnType) {
					string name = rt.FullyQualifiedName;
					if (name == "System.Void") {
						writer.Write(VoidRTCode);
					} else {
						writer.Write(classIndices[new ClassNameTypeCountPair(rt)]);
					}
				} else if (rt is ArrayReturnType) {
					ArrayReturnType art = (ArrayReturnType)rt;
					writer.Write(ArrayRTCode);
					writer.Write(art.ArrayDimensions);
					WriteType(art.ElementType);
				} else if (rt is ConstructedReturnType) {
					ConstructedReturnType crt = (ConstructedReturnType)rt;
					writer.Write(ConstructedRTCode);
					WriteType(crt.BaseType);
					writer.Write((byte)crt.TypeArguments.Count);
					foreach (IReturnType typeArgument in crt.TypeArguments) {
						WriteType(typeArgument);
					}
				} else if (rt is GenericReturnType) {
					GenericReturnType grt = (GenericReturnType)rt;
					if (grt.TypeParameter.Method != null) {
						writer.Write(MethodGenericRTCode);
					} else {
						writer.Write(TypeGenericRTCode);
					}
					writer.Write(grt.TypeParameter.Index);
				} else {
					writer.Write(NullRTReferenceCode);
					LoggingService.Warn("Unknown return type: " + rt.ToString());
				}
			}
			
			// outerClass and outerMethod are required for generic return types
			IReturnType ReadType()
			{
				int index = reader.ReadInt32();
				switch (index) {
					case ArrayRTCode:
						int dimensions = reader.ReadInt32();
						return new ArrayReturnType(ReadType(), dimensions);
					case ConstructedRTCode:
						IReturnType baseType = ReadType();
						IReturnType[] typeArguments = new IReturnType[reader.ReadByte()];
						for (int i = 0; i < typeArguments.Length; i++) {
							typeArguments[i] = ReadType();
						}
						return new ConstructedReturnType(baseType, typeArguments);
					case TypeGenericRTCode:
						return new GenericReturnType(currentClass.TypeParameters[reader.ReadInt32()]);
					case MethodGenericRTCode:
						return new GenericReturnType(currentMethod.TypeParameters[reader.ReadInt32()]);
					case NullRTReferenceCode:
						return null;
					case VoidRTCode:
						return ReflectionReturnType.Void;
					default:
						return types[index];
				}
			}
			#endregion
			
			#region Write/Read class member
			void WriteString(string text)
			{
				writer.Write(text ?? string.Empty);
			}
			
			string ReadString()
			{
				return reader.ReadString();
			}
			
			void WriteMember(IMember m)
			{
				WriteString(m.Name);
				writer.Write((int)m.Modifiers);
				WriteAttributes(m.Attributes);
				if (!(m is IMethod)) {
					// method must store ReturnType AFTER Template definitions
					WriteType(m.ReturnType);
				}
			}
			
			void ReadMember(IMember m)
			{
				// name is already read by the method that calls the member constructor
				m.Modifiers = (ModifierEnum)reader.ReadInt32();
				ReadAttributes(m.Attributes);
				if (!(m is IMethod)) {
					m.ReturnType = ReadType();
				}
			}
			#endregion
			
			#region Write/Read attributes
			void WriteAttributes(IList<IAttribute> attributes)
			{
				writer.Write((ushort)attributes.Count);
				foreach (IAttribute a in attributes) {
					WriteString(a.Name);
					writer.Write((byte)a.AttributeTarget);
				}
			}
			
			void ReadAttributes(IList<IAttribute> attributes)
			{
				int count = reader.ReadUInt16();
				for (int i = 0; i < count; i++) {
					string name = ReadString();
					attributes.Add(new DefaultAttribute(name, (AttributeTarget)reader.ReadByte()));
				}
			}
			#endregion
			
			#region Write/Read parameters
			void WriteParameters(IList<IParameter> parameters)
			{
				writer.Write((ushort)parameters.Count);
				foreach (IParameter p in parameters) {
					WriteString(p.Name);
					WriteType(p.ReturnType);
					writer.Write((byte)p.Modifiers);
					WriteAttributes(p.Attributes);
				}
			}
			
			void ReadParameters(IList<IParameter> parameters)
			{
				int count = reader.ReadUInt16();
				for (int i = 0; i < count; i++) {
					string name = ReadString();
					DefaultParameter p = new DefaultParameter(name, ReadType(), DomRegion.Empty);
					p.Modifiers = (ParameterModifiers)reader.ReadByte();
					ReadAttributes(p.Attributes);
					parameters.Add(p);
				}
			}
			#endregion
			
			#region Write/Read Method
			IMethod currentMethod;
			
			void WriteMethod(IMethod m)
			{
				currentMethod = m;
				WriteMember(m);
				WriteTemplates(m.TypeParameters);
				WriteType(m.ReturnType);
				WriteParameters(m.Parameters);
				currentMethod = null;
			}
			
			IMethod ReadMethod()
			{
				DefaultMethod m = new DefaultMethod(currentClass, ReadString());
				currentMethod = m;
				ReadMember(m);
				int count = reader.ReadByte();
				for (int i = 0; i < count; i++) {
					m.TypeParameters.Add(new DefaultTypeParameter(m, ReadString(), i));
				}
				foreach (ITypeParameter typeParameter in m.TypeParameters) {
					count = reader.ReadInt32();
					for (int i = 0; i < count; i++) {
						typeParameter.Constraints.Add(ReadType());
					}
				}
				m.ReturnType = ReadType();
				ReadParameters(m.Parameters);
				currentMethod = null;
				return m;
			}
			#endregion
			
			#region Write/Read Property
			void WriteProperty(IProperty p)
			{
				WriteMember(p);
				writer.Write(((DefaultProperty)p).accessFlags);
				WriteParameters(p.Parameters);
			}
			
			IProperty ReadProperty()
			{
				DefaultProperty p = new DefaultProperty(currentClass, ReadString());
				ReadMember(p);
				p.accessFlags = reader.ReadByte();
				ReadParameters(p.Parameters);
				return p;
			}
			#endregion
			
			#region Write/Read Event
			void WriteEvent(IEvent p)
			{
				WriteMember(p);
			}
			
			IEvent ReadEvent()
			{
				DefaultEvent p = new DefaultEvent(currentClass, ReadString());
				ReadMember(p);
				return p;
			}
			#endregion
			
			#region Write/Read Field
			void WriteField(IField p)
			{
				WriteMember(p);
			}
			
			IField ReadField()
			{
				DefaultField p = new DefaultField(currentClass, ReadString());
				ReadMember(p);
				return p;
			}
			#endregion
		}
	}
}
