using ICSharpCode.Core.Presentation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
    public class BindingInfoTemplateDictionary
    {
    	[Test]
		public void FindAllSuperSetsTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "1" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, null, "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "2" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, "Global2", "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "3" });
			
			var allBindingInfos = dictionary.FindItems(new BindingInfoTemplate());
			Assert.AreEqual(3, allBindingInfos.Count());
			Assert.IsTrue(allBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
    	
    	[Test]
		public void FindSuperSetWithRoutedCommandTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, null, null),
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, null, "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, "Global", "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, "Global", "TestCommands.DoSomethingOther"),
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(BindingInfoTemplate.Create(null, null, "TestCommands.DoSomething"));
			Assert.AreEqual(2, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
		
		[Test]
		public void FindSuperSetWithTwoAttributesTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, null, null),
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, null, "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, "Global", "TestCommands.DoSomething"),
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				BindingInfoTemplate.Create(null, "Global", "TestCommands.DoSomethingOther"),
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(BindingInfoTemplate.Create(null, "Global", "TestCommands.DoSomething"));
			
			Assert.AreEqual(1, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
    	
    }
}
