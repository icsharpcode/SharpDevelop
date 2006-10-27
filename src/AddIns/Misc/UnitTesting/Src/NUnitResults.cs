// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Reads the NUnit results file.
	/// </summary>
	public class NUnitResults
	{
		static readonly string TestCaseElementName = "test-case";
		static readonly string MessageElementName = "message";
		static readonly string StackTraceElementName= "stack-trace";
		
		readonly List<Task> tasks = new List<Task>();
		
		public List<Task> Tasks {
			get {
				return tasks;
			}
		}
		
		public NUnitResults(string fileName) : this(new StreamReader(fileName, true))
		{
		}
		
		public NUnitResults(TextReader reader) : this(new XmlTextReader(reader))
		{
		}
		
		public NUnitResults(XmlReader reader)
		{
			using (reader) {
				ReadResults(reader);
			}
		}
		
		void ReadResults(XmlReader reader)
		{
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element) {
					if (reader.Name == TestCaseElementName) {
						if (bool.Parse(reader.GetAttribute("executed")) == false) {
							// test was ignored.
							AddTest(reader, TaskType.Warning, "${res:NUnitPad.NUnitPadContent.TestTreeView.TestNotExecutedMessage}");
						} else if (bool.Parse(reader.GetAttribute("success")) == false) {
							// test failed.
							AddTest(reader, TaskType.Error, "${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}");
						}
					}
				}
			}
		}
		
		void AddTest(XmlReader reader, TaskType type, string message)
		{
			string testName = reader.GetAttribute("name");
			string msg, stackTrace;
			GetDescription(reader, out msg, out stackTrace);
			
			message = StringParser.Parse(message, new string[,] {
			                             	{"TestCase", testName},
			                             	{"Message", msg}
			                             });
			Task task = TestTreeView.CreateTask(type, message, testName, stackTrace);
			if (task != null) {
				tasks.Add(task);
			}
		}
		
		void GetDescription(XmlReader reader, out string message, out string stackTrace)
		{
			message = "";
			stackTrace = "";
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element) {
					if (reader.Name == MessageElementName) {
						message = reader.ReadElementString();
					} else if (reader.Name == StackTraceElementName) {
						stackTrace = reader.ReadElementString();
					}
				} else if (reader.NodeType == XmlNodeType.EndElement) {
					if (reader.Name == TestCaseElementName) {
						return;
					}
				}
			}
		}
	}
}
