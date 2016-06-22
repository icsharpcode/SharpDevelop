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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Debugger.Tests
{
	[TestFixture]
	public partial class DebuggerTests: DebuggerTestsBase
	{
		
	}
	
	public class DebuggerTestsBase
	{
		string expetedOutputEnvVar = "SD_TESTS_DEBUGGER_XML_OUT";
		
		protected NDebugger   debugger;
		protected Process     process;
		protected string      log;
		protected string      lastLogMessage;
		protected string      testName;
		protected XmlDocument testDoc;
		protected XmlElement  testNode;
		protected XmlElement  snapshotNode;
		protected int         shapshotID;
		
		public Thread CurrentThread { get; private set; }
		public StackFrame CurrentStackFrame { get; private set; }
		public Thread EvalThread { get { return this.CurrentThread; } }
		
		public void Continue()
		{
			process.Continue();
		}
		
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			debugger = new NDebugger();
			debugger.MTA2STA.CallMethod = CallMethod.Manual;
		}
		
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			
		}
		
		[SetUp]
		public virtual void SetUp()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			
			testName = null;
			
			expandProperties = new List<string>();
			ignoreProperties = new List<string>();
			ignoreProperties.AddRange(new string [] {
			                          	"*.GetType",
			                          	"*.GetHashCode",
			                          	"*.GetEnumerator",
			                          	"*.AppDomain",
			                          	"*.Process",
			                          	"MemberInfo.DebugModule",
			                          	"MemberInfo.IsStatic",
			                          	"MemberInfo.IsPublic",
			                          	"MemberInfo.IsAssembly",
			                          	"MemberInfo.IsFamily",
			                          	"MemberInfo.IsPrivate",
			                          	"MemberInfo.MetadataToken",
			                          	"MemberInfo.MemberType",
			                          	"Type.DeclaringType",
			                          	"Type.IsAbstract",
			                          	"Type.IsAnsiClass",
			                          	"Type.IsAutoLayout",
			                          	"Type.IsLayoutSequential",
			                          	"Type.IsNestedPublic",
			                          	"Type.IsSealed",
			                          	"Type.IsSerializable",
			                          	"Type.IsVisible",
			                          	"Type.IsNotPublic",
			                          	"Type.Name",
			                          	"Type.Namespace",
			                          	"Type.UnderlyingSystemType",
			                          	"MethodBase.CallingConvention",
			                          	"MethodBase.GetMethodBody",
			                          	"MethodBase.GetMethodImplementationFlags",
			                          	"MethodBase.GetParameters",
			                          	"MethodBase.IsFinal",
			                          	"MethodBase.IsHideBySig",
			                          	"MethodBase.IsVirtual",
			                          	"MethodBase.IsSpecialName",
			                          	"MethodBase.ReturnParameter",
			                          	"MethodBase.ParameterCount",
			                          	"PropertyInfo.CanRead",
			                          	"PropertyInfo.CanWrite",
			                          	"PropertyInfo.GetGetMethod",
			                          	"PropertyInfo.GetSetMethod",
			                          });
			
			testDoc = new XmlDocument();
			testDoc.AppendChild(testDoc.CreateXmlDeclaration("1.0","utf-8",null));
			testDoc.AppendChild(testDoc.CreateElement("DebuggerTests"));
			testNode = testDoc.CreateElement("Test");
			testDoc.DocumentElement.AppendChild(testNode);
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			
		}
		
		protected void EndTest(bool hasXml = true)
		{
			if (!process.HasExited) {
				process.AsyncContinue();
				process.WaitForExit();
			}
			if (hasXml)
				CheckXmlOutput();
		}
		
		protected void CheckXmlOutput()
		{
			string startMark = "#if EXPECTED_OUTPUT\r\n";
			string endMark = "#endif // EXPECTED_OUTPUT";
			
			MemoryStream newXmlStream = new MemoryStream();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			XmlWriter writer = XmlTextWriter.Create(newXmlStream, settings);
			testDoc.Save(writer);
			newXmlStream.Seek(0, SeekOrigin.Begin);
			string actualXml = new StreamReader(newXmlStream).ReadToEnd() + "\r\n";
			
			string sourceCode = GetResource(testName);
			// Normalize the line endings
			sourceCode = sourceCode.Replace("\r\n", "\n").Replace("\n", "\r\n");
			int startIndex = sourceCode.IndexOf(startMark);
			int endIndex = sourceCode.IndexOf(endMark);
			if (startIndex == -1 || endIndex == -1) {
				Assert.Fail("Test " + testName + " failed.  Expected XML output not found.");
			}
			string expectedXml = sourceCode.Substring(startIndex + startMark.Length, endIndex - (startIndex + startMark.Length));
			
			if (actualXml != expectedXml) {
				// Update the source code file with the new output
				string path = Environment.GetEnvironmentVariable(expetedOutputEnvVar);
				if (path != null) {
					string filename = Path.Combine(path, testName);
					string newSourceCode = File.ReadAllText(filename, Encoding.UTF8);
					startIndex = newSourceCode.IndexOf(startMark);
					endIndex = newSourceCode.IndexOf(endMark);
					newSourceCode =
						newSourceCode.Substring(0, startIndex + startMark.Length) +
						actualXml +
						newSourceCode.Substring(endIndex);
					File.WriteAllText(filename, newSourceCode, Encoding.UTF8);
				}
				
				//Assert.Fail("Test " + testName + " failed.  XML output differs from expected.");
				Assert.AreEqual(expectedXml, actualXml, "Test " + testName + " failed.  XML output differs from expected.");
			}
		}
		
		protected void StartTest()
		{
			string testName = Path.GetFileName(new System.Diagnostics.StackTrace(true).GetFrame(1).GetFileName());
			StartTest(testName, true);
		}
		
		protected void StartTestNoWait()
		{
			string testName = Path.GetFileName(new System.Diagnostics.StackTrace(true).GetFrame(1).GetFileName());
			StartTest(testName, false);
		}
		
		void StartTest(string testName, bool wait)
		{
			if (!IsRuntimeCompatible()) {
				Assert.Ignore();
				return;
			}
			this.testName = testName;
			string exeFilename = CompileTest(testName);
			
			testNode.SetAttribute("name", testName);
			shapshotID = 0;
			
			debugger.Options = new Options();
			debugger.Options.EnableJustMyCode = true;
			debugger.Options.SuppressJITOptimization = true;
			debugger.Options.StepOverDebuggerAttributes = true;
			debugger.Options.StepOverAllProperties = false;
			debugger.Options.StepOverFieldAccessProperties = true;
			debugger.Options.SymbolsSearchPaths = new string[0];
			debugger.Options.PauseOnHandledExceptions = false;
			
			log = "";
			lastLogMessage = null;
			process = debugger.Start(exeFilename, Path.GetDirectoryName(exeFilename), testName, false);
			process.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
				LogEvent("LogMessage", e.Message.Replace("\r",@"\r").Replace("\n",@"\n"));
			};
			process.ModuleLoaded += delegate(object sender, ModuleEventArgs e) {
				LogEvent("ModuleLoaded", e.Module.Name + (e.Module.HasSymbols ? " (Has symbols)" : " (No symbols)"));
			};
			process.Paused += delegate(object sender, DebuggerPausedEventArgs e) {
				this.CurrentThread = e.Thread;
				if (e.Thread != null && e.Thread.IsInValidState) {
					this.CurrentStackFrame = e.Thread.MostRecentStackFrame;
				} else {
					this.CurrentStackFrame = null;
				}
				foreach(Thread exceptionThread in e.ExceptionsThrown) {
					Value exception = exceptionThread.CurrentException;
					LogEvent("ExceptionThrown", exception.Type.FullName);
					if (!exceptionThread.InterceptException()) {
						LogEvent("CanNotInterceptException", exception.Type.FullName);
					}
				}
				LogEvent("Paused", CurrentStackFrame != null && CurrentStackFrame.NextStatement != null ? CurrentStackFrame.NextStatement.ToString() : string.Empty);
			};
			process.Exited += delegate(object sender, DebuggerEventArgs e) {
				LogEvent("Exited", null);
			};
			
			LogEvent("Started", null);
			
			if (wait) {
				process.WaitForPause();
			}
		}
		
		protected XmlElement LogEvent(string name, string content)
		{
			XmlElement eventNode = testDoc.CreateElement(name);
			if (content != null) {
				eventNode.AppendChild(testDoc.CreateTextNode(content));
			}
			testNode.AppendChild(eventNode);
			return eventNode;
		}
		
		public void ObjectDump(object obj)
		{
			Serialize(testNode, obj, 16, new List<object>());
		}
		
		public void ObjectDump(string name, object obj)
		{
			XmlElement dumpNode = testDoc.CreateElement(XmlConvert.EncodeName(name.Replace(" ", "_")));
			testNode.AppendChild(dumpNode);
			Serialize(dumpNode, obj, 16, new List<object>());
		}
		
		public void ObjectDumpToString(string name, object obj)
		{
			XmlElement dumpNode = testDoc.CreateElement(XmlConvert.EncodeName(name.Replace(" ", "_")));
			testNode.AppendChild(dumpNode);
			if (obj == null) {
				dumpNode.AppendChild(dumpNode.OwnerDocument.CreateTextNode("null"));
			} else {
				dumpNode.AppendChild(dumpNode.OwnerDocument.CreateTextNode(obj.ToString()));
			}
		}
		
		class LocalVariable
		{
			public string Name { get; set; }
			public IType Type { get; set; }
			public Value Value { get; set; }
		}
		
		public void DumpLocalVariables()
		{
			DumpLocalVariables("LocalVariables");
		}
		
		public void DumpLocalVariables(string msg)
		{
			ObjectDump(
				msg,
				this.CurrentStackFrame.GetLocalVariables(this.CurrentStackFrame.IP).Select(v => new LocalVariable() { Name = v.Name, Type = v.Type, Value = v.GetValue(this.CurrentStackFrame)})
			);
		}
		
		List<string> expandProperties;
		List<string> ignoreProperties;
		
		protected void ExpandProperties(params string[] props)
		{
			expandProperties = new List<string>(props);
		}
		
		bool ListContains(List<string> list, MemberInfo memberInfo)
		{
			Type declaringType = memberInfo.DeclaringType;
			while(declaringType != null) {
				if (list.Contains(declaringType.Name + "." + memberInfo.Name) ||
				    list.Contains(declaringType.Name + ".*") ||
				    list.Contains("*." + memberInfo.Name) ||
				    list.Contains("*.*") ||
				    list.Contains("*"))
					return true;
				declaringType = declaringType.BaseType;
			}
			return false;
		}
		
		bool ExpandProperty(MemberInfo memberInfo)
		{
			return (memberInfo.IsDefined(typeof(Debugger.Tests.ExpandAttribute), true)) ||
				ListContains(expandProperties, memberInfo);
		}
		
		bool IgnoreProperty(MemberInfo memberInfo)
		{
			return (memberInfo.IsDefined(typeof(Debugger.Tests.IgnoreAttribute), true)) ||
				ListContains(ignoreProperties, memberInfo);
		}
		
		public void Serialize(XmlElement container, object obj, int maxDepth, List<object> parents)
		{
			XmlDocument doc = container.OwnerDocument;
			
			if (obj == null) {
				container.AppendChild(doc.CreateTextNode("null"));
				return;
			}
			if (maxDepth == -1) {
				container.AppendChild(doc.CreateTextNode("{Max depth reached}"));
				return;
			}
			if (parents.Contains(obj)) {
				container.AppendChild(doc.CreateTextNode("{Recusion detected}"));
				return;
			}
			if (obj.GetType().Namespace == "System") {
				container.AppendChild(doc.CreateTextNode(obj.ToString()));
				return;
			}
			
			parents = new List<object>(parents); // Clone
			parents.Add(obj);
			
			Type type = obj.GetType();
			
			if (!(obj is IEnumerable)) {
				string name = XmlConvert.EncodeName(type.Name);
				if (name.Contains("__AnonymousType"))
					name = "AnonymousType";
				XmlElement newContainer = doc.CreateElement(name);
				container.AppendChild(newContainer);
				container = newContainer;
			}
			
			List<MemberInfo> members = new List<MemberInfo>();
			members.AddRange(type.GetMembers());
			members.Sort(delegate(MemberInfo a, MemberInfo b) { return a.Name.CompareTo(b.Name);});
			
			foreach(MemberInfo member in members) {
				if (type.BaseType == typeof(Array)) continue;
				
				MethodInfo method;
				object val;
				
				PropertyInfo propertyInfo = member as PropertyInfo;
				MethodInfo methodInfo = member as MethodInfo;
				if (propertyInfo != null) {
					if (propertyInfo.GetGetMethod() == null) continue;
					method = propertyInfo.GetGetMethod();
				} else if (methodInfo != null) {
					if (!methodInfo.Name.StartsWith("Get")) continue;
					method = methodInfo;
				} else {
					continue;
				}
				if (method.GetParameters().Length > 0) continue;
				if (IgnoreProperty(member)) continue;
				
				try {
					val = method.Invoke(obj, new object[] {});
				} catch (System.Exception e) {
					while(e.InnerException != null) e = e.InnerException;
					if (e is NotImplementedException || e is NotSupportedException)
						continue;
					if (type.IsDefined(typeof(Debugger.Tests.IgnoreOnExceptionAttribute), true) ||
					    member.IsDefined(typeof(Debugger.Tests.IgnoreOnExceptionAttribute), true))
						continue;
					val = "{Exception: " + e.Message + "}";
				}
				
				if (val is IEnumerable && !(val is string)) {
					List<string> vals = new List<string>();
					foreach(object o in (IEnumerable)val) {
						vals.Add(o.ToString());
					}
					if (vals.Count != 0) {
						container.SetAttribute(member.Name, "{" + string.Join(", ", vals.ToArray()) + "}");
					}
				} else {
					bool isDefault = false;
					
					if (method.ReturnType == typeof(bool)) {
						isDefault = false.Equals(val);
					} else if (method.ReturnType == typeof(string)) {
						isDefault = val == null;
					} else if (method.ReturnType == typeof(int)) {
						isDefault = 0.Equals(val);
					} else if (method.ReturnType == typeof(uint)) {
						isDefault = ((uint)0).Equals(val);
					} else {
						isDefault = val == null;
					}

					if (val == null) val = "null";
					if (!isDefault) {
						container.SetAttribute(member.Name, val.ToString());
					}
				}
				
				
				if (ExpandProperty(member)) {
					XmlElement propertyNode = doc.CreateElement(member.Name);
					container.AppendChild(propertyNode);
					Serialize(propertyNode, val, maxDepth - 1, parents);
				}
			}
			
			// Save all objects of an enumerable object
			if (obj is IEnumerable) {
				int id = 1;
				foreach(object enumObject in (IEnumerable)obj) {
					XmlElement enumRoot = doc.CreateElement("Item");
					container.AppendChild(enumRoot);
					Serialize(enumRoot, enumObject, maxDepth - 1, parents);
					id++;
				}
			}
		}
		
		protected string GetResource(string filename)
		{
			string resourcePrefix = "Debugger.Tests.Tests.";
			
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePrefix + filename);
			if (stream == null) throw new System.Exception("Resource " + filename + " not found");
			return new StreamReader(stream).ReadToEnd();
		}
		
		string CompileTest(string testName)
		{
			string code = GetResource(testName);
			
			string md5 = ToHexadecimal(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(code)));
			
			string path = Path.GetTempPath();
			path = Path.Combine(path, "SharpDevelop4.0");
			path = Path.Combine(path, "DebuggerTestsX86");
			path = Path.Combine(path, testName + "." + md5);
			Directory.CreateDirectory(path);
			
			string codeFilename = Path.Combine(path, testName);
			string exeFilename = Path.Combine(path, testName.Replace(".cs", ".exe"));
			
			if (File.Exists(exeFilename)) {
				return exeFilename;
			}
			
			StreamWriter file = new StreamWriter(codeFilename);
			file.Write(code);
			file.Close();
			
			CompilerParameters compParams = new CompilerParameters();
			compParams.GenerateExecutable = true;
			compParams.GenerateInMemory = false;
			compParams.TreatWarningsAsErrors = false;
			compParams.IncludeDebugInformation = true;
			compParams.ReferencedAssemblies.Add("System.dll");
			compParams.ReferencedAssemblies.Add("System.Core.dll");
			compParams.OutputAssembly = exeFilename;
			compParams.CompilerOptions = "/unsafe /platform:x86 /target:winexe";
			compParams.ReferencedAssemblies.Add(typeof(TestFixtureAttribute).Assembly.Location);
			
			CSharpCodeProvider compiler = new CSharpCodeProvider();
			CompilerResults result = compiler.CompileAssemblyFromFile(compParams, codeFilename);
			
			if (result.Errors.Count > 0) {
				throw new System.Exception("There was an error(s) during compilation of test program:\n" + result.Errors[0].ToString());
			}
			
			return exeFilename;
		}
		
		string CopyThisAssembly()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string md5 = ToHexadecimal(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(assembly.Location)));
			
			string exeName = Path.GetFileName(assembly.Location);
			string pdbName = Path.GetFileNameWithoutExtension(assembly.Location) + ".pdb";
			
			string oldDir = Path.GetDirectoryName(assembly.Location);
			
			string newDir = Path.GetTempPath();
			newDir = Path.Combine(newDir, "SharpDevelop3.0");
			newDir = Path.Combine(newDir, "DebuggerTests");
			newDir = Path.Combine(newDir, md5);
			Directory.CreateDirectory(newDir);
			
			if (!File.Exists(Path.Combine(newDir, exeName))) {
				File.Copy(Path.Combine(oldDir, exeName), Path.Combine(newDir, exeName));
			}
			
			if (!File.Exists(Path.Combine(newDir, pdbName))) {
				File.Copy(Path.Combine(oldDir, pdbName), Path.Combine(newDir, pdbName));
			}
			
			return Path.Combine(newDir, exeName);
		}
		
		void CopyPdb()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string dir = Path.GetDirectoryName(assembly.Location);
			string iniFilePath = Path.Combine(dir, "__AssemblyInfo__.ini");
			string iniFileContent = File.ReadAllText(iniFilePath, Encoding.Unicode);
			
			string originalExePath = iniFileContent.Remove(0, iniFileContent.IndexOf("file:///") + "file:///".Length).TrimEnd(' ', '\0');
			string originalDir = Path.GetDirectoryName(originalExePath);
			string originalPdbPath = Path.Combine(originalDir, Path.GetFileNameWithoutExtension(originalExePath) + ".pdb");
			
			string pdbPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(originalExePath) + ".pdb");
			
			if (!File.Exists(pdbPath)) {
				File.Copy(originalPdbPath, pdbPath);
			}
		}
		
		static string ToHexadecimal(byte[] bytes)
		{
			char[] chars = new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F',};
			string hex = "";
			foreach(byte b in bytes) {
				hex += chars[b >> 4];
				hex += chars[b & 0x0F];
			}
			return hex;
		}
		
		public static void Main(string[] args)
		{
			if (args.Length != 1) throw new System.Exception("Needs test name as argument");
			string testName = args[0];
			Type type = Type.GetType(testName);
			type.GetMethod("Main").Invoke(null, new object[0]);
		}
		
		protected static bool IsDotnet45Installed()
		{
			//Version dotnet45Beta = new Version(4, 0, 30319, 17379);
			//return Environment.Version >= dotnet45Beta;
			// SharpDevelop 5.0 requires .NET 4.5, and the detection with Environment.Version is no longer reliable
			// (see https://github.com/icsharpcode/SharpDevelop/issues/581)
			return true;
		}
		
		/// <summary>
		/// Debugger Tests currently require .NET 4.6
		/// </summary>
		protected static bool IsRuntimeCompatible()	
		{
			return DotnetDetection.IsDotnet46Installed();
		}
	}
}
