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
				new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "2" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global2", RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "3" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething2", Groups = new BindingGroupCollection { new BindingGroup() } },
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest", OwnerInstanceName = "4" });
			
			var allBindingInfos = dictionary.FindItems(new BindingInfoTemplate(), BindingInfoMatchType.SuperSet);
			Assert.AreEqual(4, allBindingInfos.Count());
			Assert.IsTrue(allBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
    	
    	[Test]
		public void FindSuperSetWithRoutedCommandTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething", Groups = new BindingGroupCollection { new BindingGroup() } },
			    new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomethingOther" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, BindingInfoMatchType.SuperSet);
			Assert.AreEqual(3, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
		
		[Test]
		public void FindSuperSetWithTwoAttributesTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething", Groups = new BindingGroupCollection { new BindingGroup() } },
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomethingOther" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething" },
				BindingInfoMatchType.SuperSet);
			
			Assert.AreEqual(2, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
		
		
		
    	[Test]
		public void FindAllSubSetsTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global2", RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething2", Groups = new BindingGroupCollection { new BindingGroup() } },
			    new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(new BindingInfoTemplate(), BindingInfoMatchType.SubSet);
			Assert.AreEqual(1, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
    	
    	[Test]
		public void FindSubSetWithGroupTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			var group = new BindingGroup();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { Groups = new BindingGroupCollection { group } },
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", Groups = new BindingGroupCollection { group } },
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething", Groups = new BindingGroupCollection { group } },
			    new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", Groups = new BindingGroupCollection { new BindingGroup() } },
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(new BindingInfoTemplate { Groups = new BindingGroupCollection { group } }, BindingInfoMatchType.SubSet);
			Assert.AreEqual(2, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
		
		[Test]
		public void FindSubSetWithTwoAttributesTest()
		{	
			var dictionary = new BindingInfoTemplateDictionary<CommandBindingInfo>();
			
			dictionary.Add(
				new BindingInfoTemplate {  }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething" }, 
				new CommandBindingInfo { RoutedCommandName = "SuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething", Groups = new BindingGroupCollection { new BindingGroup() } },
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			dictionary.Add(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomethingOther" }, 
				new CommandBindingInfo { RoutedCommandName = "UnsuccessfullTest" });
			
			var doSomethingBindingInfos = dictionary.FindItems(
				new BindingInfoTemplate { OwnerTypeName = "Global", RoutedCommandName = "TestCommands.DoSomething" },
				BindingInfoMatchType.SubSet);
			
			Assert.AreEqual(3, doSomethingBindingInfos.Count());
			Assert.IsTrue(doSomethingBindingInfos.All(i => i.RoutedCommandName == "SuccessfullTest"));
		}
    }
}
