// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Expressions.Irony;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Expressions
{
	[TestFixture]
	public class ParametersHandlingFixture
	{
		ReportingLanguageGrammer grammar;
		ReportingExpressionEvaluator evaluator;
		
		[Test]
		public void CanEvaluateOneParameter()
		{
			var resultValue =  "Hi from param1";
			var parameters = new ParameterCollection();
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param1",
			               	ParameterValue = resultValue
			               }
			              );
			evaluator.App.Globals.Add("parameters",parameters);
			var script = "Parameters!param1";
			var result = evaluator.Evaluate(script);
			Assert.That (result,Is.EqualTo(resultValue));
		}
		
		
		[Test]
		public void CanFindParameter () {
			var resultValue =  "Hi from param2";
			var parameters = new ParameterCollection();
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param1",
			               	ParameterValue = "Value for parameter1"
			               }
			              );
			
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param2",
			               	ParameterValue = resultValue
			               }
			              );
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param3",
			               	ParameterValue = "Value for parameter2"
			               }
			              );
			evaluator.App.Globals.Add("parameters",parameters);
			var script = "Parameters!param2";
			var result = evaluator.Evaluate(script);
			Assert.That (result,Is.EqualTo(resultValue));
		}
		
		
		[Test]
		public void CanConcatParameter () {
			var parameters = new ParameterCollection();
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param1",
			               	ParameterValue = "SharpDevelop"
			               }
			              );
			
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param2",
			               	ParameterValue = " is "
			               }
			              );
			parameters.Add(new BasicParameter() {
			               	ParameterName = "param3",
			               	ParameterValue = "great"
			               }
			              );
			evaluator.App.Globals.Add("parameters",parameters);
			var script = "Parameters!param1 + Parameters!param2 + Parameters!param3";
			var result = evaluator.Evaluate(script);
			Assert.That (result,Is.EqualTo("SharpDevelop is great"));
		}
		
		
		[SetUp]
		public void Initialize() {
			grammar = new ReportingLanguageGrammer();
			evaluator = new ReportingExpressionEvaluator(grammar);
		}
			
		
		[TestFixtureSetUp]
		public void Init()
		{
			// TODO: Add Init code.
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
