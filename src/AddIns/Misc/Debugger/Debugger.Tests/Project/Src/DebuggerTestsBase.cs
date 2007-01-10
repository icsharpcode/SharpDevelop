// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Xml;

using NUnit.Framework;

using Debugger;
using Debugger.Interop;

namespace Debugger.Tests
{
	public class DebuggerTestsBase
	{
		protected NDebugger   debugger;
		protected Process     process;
		protected string      log;
		protected string      lastLogMessage;
		protected string      testName;
		protected XmlDocument testDoc;
		protected XmlElement  testNode;
		protected XmlElement  snapshotNode;
		protected int         shapshotID;
		
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
			testName = null;
			
			testDoc = new XmlDocument();
			testDoc.AppendChild(testDoc.CreateXmlDeclaration("1.0","utf-8",null));
			testDoc.AppendChild(testDoc.CreateElement("DebuggerTests"));
			testNode = testDoc.CreateElement("Test");
			testDoc.DocumentElement.AppendChild(testNode);
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			while(debugger.Processes.Count > 0) {
				debugger.Processes[0].Terminate();
				debugger.Processes[0].WaitForExit();
			}
			string path = Path.GetTempPath();
			path = Path.Combine(path, "SharpDevelop");
			path = Path.Combine(path, "DebuggerTestResults");
			Directory.CreateDirectory(path);
			testDoc.Save(Path.Combine(path, testName + ".xml"));
			
			string oldXml = GetResource(testName + ".xml");
			MemoryStream newXmlStream = new MemoryStream();
			testDoc.Save(newXmlStream);
			newXmlStream.Seek(0, SeekOrigin.Begin);
			string newXml = new StreamReader(newXmlStream).ReadToEnd();
			Assert.AreEqual(oldXml, newXml);
		}
		
		protected void StartTest(string testName)
		{
			this.testName = testName;
			string exeFilename = CompileTest(testName);
			
			testNode.SetAttribute("name", testName);
			shapshotID = 0;
			
			log = "";
			lastLogMessage = null;
			process = debugger.Start(exeFilename, Path.GetDirectoryName(exeFilename), testName);
			process.LogMessage += delegate(object sender, MessageEventArgs e) {
				log += e.Message;
				lastLogMessage = e.Message;
				LogEvent("LogMessage", e.Message.Replace("\r",@"\r").Replace("\n",@"\n"));
			};
			process.ModuleLoaded += delegate(object sender, ModuleEventArgs e) {
				LogEvent("ModuleLoaded", e.Module.Filename).SetAttribute("symbols", e.Module.SymbolsLoaded.ToString());
			};
			process.DebuggingPaused += delegate(object sender, ProcessEventArgs e) {
				LogEvent("DebuggingPaused", e.Process.PausedReason.ToString());
			};
//			process.DebuggingResumed += delegate(object sender, ProcessEventArgs e) {
//				LogEvent("DebuggingResumed", e.Process.PausedReason.ToString());
//			};
			process.Expired += delegate(object sender, EventArgs e) {
				LogEvent("ProcessExited", null);
			};
			
			LogEvent("ProcessStarted", null);
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
		
		protected void WaitForPause(PausedReason expectedReason)
		{
			process.WaitForPause();
			Assert.AreEqual(true, process.IsPaused);
			Assert.AreEqual(expectedReason, process.PausedReason);
		}	
		
		protected void WaitForPause(PausedReason expectedReason, string expectedLastLogMessage)
		{
			WaitForPause(expectedReason);
			if (expectedLastLogMessage != null) expectedLastLogMessage += "\r\n";
			Assert.AreEqual(expectedLastLogMessage, lastLogMessage);
			
//			snapshotNode = testDoc.CreateElement("Snapshot");
//			snapshotNode.SetAttribute("id", (shapshotID++).ToString());
//			testNode.AppendChild(snapshotNode);
		}
		
		public void ObjectDump(object obj)
		{
			ObjectDump(null, obj);
		}
		
		public void ObjectDump(string name, object obj)
		{
			XmlElement dumpNode = testDoc.CreateElement("ObjectDump");
			if (name != null) dumpNode.SetAttribute("name", name);
			testNode.AppendChild(dumpNode);
			Serialize(dumpNode, obj);
		}
		
		static bool ShouldExpandType(Type type)
		{
			return type.IsSubclassOf(typeof(DebuggerObject)) ||
			       ( typeof(IEnumerable).IsAssignableFrom(type) &&
			         type.Namespace != "System"
			       );
		}
		
		public static void Serialize(XmlNode parent, object obj)
		{
			XmlDocument doc = parent.OwnerDocument;
			
			if (obj == null) {
				parent.AppendChild(doc.CreateElement("Null"));
				return;
			}
			
			Type type = obj.GetType();
			
			XmlElement objectRoot = doc.CreateElement(XmlConvert.EncodeName(type.Name));
			parent.AppendChild(objectRoot);
			
			if (!ShouldExpandType(type)) {
				objectRoot.AppendChild(doc.CreateTextNode(obj.ToString()));
				return;
			}
			
			foreach(System.Reflection.PropertyInfo property in type.GetProperties()) {
				if (property.GetGetMethod().GetParameters().Length > 0) continue;
				if (property.GetCustomAttributes(typeof(Debugger.Tests.IgnoreAttribute), true).Length > 0) continue;
				
				XmlElement propertyNode = doc.CreateElement(property.Name);
				objectRoot.AppendChild(propertyNode);
				
				object val;
				try {
					val = property.GetValue(obj, new object[] {});
				} catch (System.Exception e) {
					while(e.InnerException != null) e = e.InnerException;
					propertyNode.SetAttribute("exception", e.Message);
					continue;
				}
				if (val == null) {
					propertyNode.AppendChild(doc.CreateTextNode("null"));
				} else if (!ShouldExpandType(val.GetType()) || property.GetCustomAttributes(typeof(Debugger.Tests.SummaryOnlyAttribute), true).Length > 0) {
					// Only write ToString() text
					propertyNode.AppendChild(doc.CreateTextNode(val.ToString()));
				} else {
					Serialize(propertyNode, val);
				}
			}
			
			// Save all objects of an enumerable object
			if (obj is IEnumerable) {
				XmlElement enumRoot = doc.CreateElement("Items");
				objectRoot.AppendChild(enumRoot);
				foreach(object enumObject in (IEnumerable)obj) {
					Serialize(enumRoot, enumObject);
				}
			}
		}
		
		string GetResource(string filename)
		{
			string resourcePrefix = "Debugger.Tests.Src.TestPrograms.";
			
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePrefix + filename);
			if (stream == null) throw new System.Exception("Resource " + filename + " not found");
			return new StreamReader(stream).ReadToEnd();
		}
		
		string CompileTest(string testName)
		{
			string code = GetResource(testName + ".cs");
			
			string md5 = ToHexadecimal(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(code)));
			
			string path = Path.GetTempPath();
			path = Path.Combine(path, "SharpDevelop");
			path = Path.Combine(path, "DebuggerTests");
			path = Path.Combine(path, md5);
			Directory.CreateDirectory(path);
			
			string codeFilename = Path.Combine(path, testName + ".cs");
			string exeFilename = Path.Combine(path, testName + ".exe");
			
			StreamWriter file = new StreamWriter(codeFilename);
			file.Write(code);
			file.Close();
			
			CompilerParameters compParams = new CompilerParameters();
			compParams.GenerateExecutable = true;
			compParams.GenerateInMemory = false;
			compParams.TreatWarningsAsErrors = false;
			compParams.IncludeDebugInformation = true;
			compParams.ReferencedAssemblies.Add("System.dll");
			compParams.OutputAssembly = exeFilename;
			
			CSharpCodeProvider compiler = new CSharpCodeProvider();
			CompilerResults result = compiler.CompileAssemblyFromFile(compParams, codeFilename);
			
			if (result.Errors.Count > 0) {
				throw new System.Exception("There was an error(s) during compilation of test program:\n" + result.Errors[0].ToString());
			}
			
			return exeFilename;
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
	}
}
