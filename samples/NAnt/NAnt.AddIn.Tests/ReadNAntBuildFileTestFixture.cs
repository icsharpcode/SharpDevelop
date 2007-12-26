using ICSharpCode.NAnt;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.NAnt.Tests
{
	/// <summary>
	/// Tests the <see cref="NAntBuildFile"/> class.
	/// </summary>
	[TestFixture]
	public class ReadNAntBuildFileTestFixture
	{
		NAntBuildFile buildFile;
		
		[SetUp]
		public void Init()
		{
			StringReader reader = new StringReader(GetBuildFileXml());
			buildFile = new NAntBuildFile(reader);
		}
		
		[Test]
		public void ProjectName()
		{
			Assert.AreEqual("Hello World", buildFile.Name, "Project name is wrong.");
		}
		
		[Test]
		public void DefaultTarget()
		{
			// Check the name.
			Assert.IsNotNull(buildFile.DefaultTarget, "Should have a default target.");
			Assert.AreEqual("test", buildFile.DefaultTarget.Name, "Default target read from build file is wrong.");
		}
		
		[Test]
		public void Targets()
		{
			Assert.AreEqual(3, buildFile.Targets.Count, "Should have 3 targets.");

			NAntBuildTarget target = buildFile.Targets[0];
			Assert.AreEqual("clean", target.Name, "Target name should be 'clean'.");
			Assert.IsFalse(target.IsDefault, "Clean target should not have default target flag set.");
			Assert.AreEqual(4, target.Line, "Clean target line number is incorrect.");
			Assert.AreEqual(5, target.Column, "Clean target column number is incorrect.");

			target = buildFile.Targets[1];
			Assert.IsFalse(target.IsDefault, "Build target should not have default target flag set.");
			Assert.AreEqual("build", target.Name, "Target name should be 'build'.");
			Assert.AreEqual(13, target.Line, "Build target line number is incorrect.");
			Assert.AreEqual(5, target.Column, "Build target column number is incorrect.");
			
			target = buildFile.Targets[2];
			Assert.AreEqual("test", target.Name, "Target name should be 'test'.");
			Assert.IsTrue(target.IsDefault, "Test target should have default target flag set.");
			Assert.AreEqual(31, target.Line, "Test target line number is incorrect.");
			Assert.AreEqual(5, target.Column, "Test target column number is incorrect.");
		}
		
		/// <summary>
		/// Gets the build file xml that will be used in this
		/// test fixture.
		/// </summary>
		string GetBuildFileXml()
		{
			return "<project name=\"Hello World\" default=\"test\">\r\n" +
				"    <property name=\"basename\" value=\"HelloWorld\"/>\r\n" +
				"    <property name=\"debug\" value=\"true\"/>\r\n" +
				"\r\n" +
				"    <target name=\"clean\">\r\n" +
				"        <delete>\r\n" +
				"            <fileset>\r\n" +
				"                <includes name=\"${basename}-??.exe\"/>\r\n" +
				"                <includes name=\"${basename}-??.pdb\"/>\r\n" +
				"            </fileset>\r\n" +
				"        </delete>\r\n" +
				"    </target>\r\n" +
				"\r\n" +
				"    <target name=\"build\">\r\n" +
				"        <csc target=\"exe\" output=\"${basename}-cs.exe\" debug=\"${debug}\">\r\n" +
				"            <sources>\r\n" +
				"                <includes name=\"${basename}.cs\"/>\r\n" +
				"            </sources>\r\n" +
				"        </csc>\r\n" +
				"        <jsc target=\"exe\" output=\"${basename}-js.exe\" debug=\"${debug}\">\r\n" +
				"            <sources>\r\n" +
				"                <includes name=\"${basename}.js\"/>\r\n" +
				"            </sources>\r\n" +
				"        </jsc>\r\n" +
				"        <vbc target=\"exe\" output=\"${basename}-vb.exe\" debug=\"${debug}\">\r\n" +
				"            <sources>\r\n" +
				"                <includes name=\"${basename}.vb\"/>\r\n" +
				"            </sources>\r\n" +
				"        </vbc>\r\n" +
				"    </target>\r\n" +
				"\r\n" +
				"    <target name=\"test\" depends=\"build\">\r\n" +
				"        <exec program=\"${basename}-cs.exe\" basedir=\".\"/>\r\n" +
				"        <exec program=\"${basename}-js.exe\" basedir=\".\"/>\r\n" +
				"        <exec program=\"${basename}-vb.exe\" basedir=\".\"/>\r\n" +
				"    </target>\r\n" +
				"</project>";
		}
	}
}
