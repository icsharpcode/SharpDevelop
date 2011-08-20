// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace ICSharpCode.FormsDesigner.Services
{
	public interface IProjectResourceService
	{
		IProjectResourceInfo GetProjectResource(CodePropertyReferenceExpression propRef);
		bool DesignerSupportsProjectResources { get; set; }
		string ProjectResourceKey { get; }
		bool FindResourceClassNames(IProjectResourceInfo resourceInfo, out string resourceClassFullyQualifiedName, out string resourcePropertyName);
	}
	
	public interface IMessageService
	{
		void ShowOutputPad();
		void ShowPropertiesPad();
		void AppendTextToBuildMessages(string text);
		void ShowError(string message);
		void ShowException(Exception ex, string message);
		string CodeStatementToString(CodeStatement statement);
	}
	
	public interface IProjectResourceInfo
	{
		string ResourceFile { get; }
		string ResourceKey { get; }
		object OriginalValue { get; }
	}
	
	public interface ICommandProvider
	{
		void ShowContextMenu(CommandID menuID, int x, int y);
	}
	
	public static class CommandIDEnumConverter
	{
		public static CommandID ToCommandID(CommandIDEnum key)
		{
			switch (key) {
				case CommandIDEnum.AlignBottom:
					return StandardCommands.AlignBottom;
				case CommandIDEnum.AlignHorizontalCenters:
					return StandardCommands.AlignHorizontalCenters;
				case CommandIDEnum.AlignLeft:
					return StandardCommands.AlignLeft;
				case CommandIDEnum.AlignRight:
					return StandardCommands.AlignRight;
				case CommandIDEnum.AlignToGrid:
					return StandardCommands.AlignToGrid;
				case CommandIDEnum.AlignTop:
					return StandardCommands.AlignTop;
				case CommandIDEnum.AlignVerticalCenters:
					return StandardCommands.AlignVerticalCenters;
				case CommandIDEnum.ArrangeBottom:
					return StandardCommands.ArrangeBottom;
				case CommandIDEnum.ArrangeRight:
					return StandardCommands.ArrangeRight;
				case CommandIDEnum.BringForward:
					return StandardCommands.BringForward;
				case CommandIDEnum.BringToFront:
					return StandardCommands.BringToFront;
				case CommandIDEnum.CenterHorizontally:
					return StandardCommands.CenterHorizontally;
				case CommandIDEnum.CenterVertically:
					return StandardCommands.CenterVertically;
				case CommandIDEnum.ViewCode:
					return StandardCommands.ViewCode;
				case CommandIDEnum.DocumentOutline:
					return StandardCommands.DocumentOutline;
				case CommandIDEnum.Copy:
					return StandardCommands.Copy;
				case CommandIDEnum.Cut:
					return StandardCommands.Cut;
				case CommandIDEnum.Delete:
					return StandardCommands.Delete;
				case CommandIDEnum.Group:
					return StandardCommands.Group;
				case CommandIDEnum.HorizSpaceConcatenate:
					return StandardCommands.HorizSpaceConcatenate;
				case CommandIDEnum.HorizSpaceDecrease:
					return StandardCommands.HorizSpaceDecrease;
				case CommandIDEnum.HorizSpaceIncrease:
					return StandardCommands.HorizSpaceIncrease;
				case CommandIDEnum.HorizSpaceMakeEqual:
					return StandardCommands.HorizSpaceMakeEqual;
				case CommandIDEnum.Paste:
					return StandardCommands.Paste;
				case CommandIDEnum.Properties:
					return StandardCommands.Properties;
				case CommandIDEnum.Redo:
					return StandardCommands.Redo;
				case CommandIDEnum.MultiLevelRedo:
					return StandardCommands.MultiLevelRedo;
				case CommandIDEnum.SelectAll:
					return StandardCommands.SelectAll;
				case CommandIDEnum.SendBackward:
					return StandardCommands.SendBackward;
				case CommandIDEnum.SendToBack:
					return StandardCommands.SendToBack;
				case CommandIDEnum.SizeToControl:
					return StandardCommands.SizeToControl;
				case CommandIDEnum.SizeToControlHeight:
					return StandardCommands.SizeToControlHeight;
				case CommandIDEnum.SizeToControlWidth:
					return StandardCommands.SizeToControlWidth;
				case CommandIDEnum.SizeToFit:
					return StandardCommands.SizeToFit;
				case CommandIDEnum.SizeToGrid:
					return StandardCommands.SizeToGrid;
				case CommandIDEnum.SnapToGrid:
					return StandardCommands.SnapToGrid;
				case CommandIDEnum.TabOrder:
					return StandardCommands.TabOrder;
				case CommandIDEnum.Undo:
					return StandardCommands.Undo;
				case CommandIDEnum.MultiLevelUndo:
					return StandardCommands.MultiLevelUndo;
				case CommandIDEnum.Ungroup:
					return StandardCommands.Ungroup;
				case CommandIDEnum.VertSpaceConcatenate:
					return StandardCommands.VertSpaceConcatenate;
				case CommandIDEnum.VertSpaceDecrease:
					return StandardCommands.VertSpaceDecrease;
				case CommandIDEnum.VertSpaceIncrease:
					return StandardCommands.VertSpaceIncrease;
				case CommandIDEnum.VertSpaceMakeEqual:
					return StandardCommands.VertSpaceMakeEqual;
				case CommandIDEnum.ShowGrid:
					return StandardCommands.ShowGrid;
				case CommandIDEnum.ViewGrid:
					return StandardCommands.ViewGrid;
				case CommandIDEnum.Replace:
					return StandardCommands.Replace;
				case CommandIDEnum.PropertiesWindow:
					return StandardCommands.PropertiesWindow;
				case CommandIDEnum.LockControls:
					return StandardCommands.LockControls;
				case CommandIDEnum.F1Help:
					return StandardCommands.F1Help;
				case CommandIDEnum.ArrangeIcons:
					return StandardCommands.ArrangeIcons;
				case CommandIDEnum.LineupIcons:
					return StandardCommands.LineupIcons;
				case CommandIDEnum.ShowLargeIcons:
					return StandardCommands.ShowLargeIcons;
				case CommandIDEnum.VerbFirst:
					return StandardCommands.VerbFirst;
				case CommandIDEnum.VerbLast:
					return StandardCommands.VerbLast;
				default:
					throw new Exception("Invalid value for CommandIDEnum");
			}
		}
	}
	
	public enum CommandIDEnum
	{
		AlignBottom,
		AlignHorizontalCenters,
		AlignLeft,
		AlignRight,
		AlignToGrid,
		AlignTop,
		AlignVerticalCenters,
		ArrangeBottom,
		ArrangeRight,
		BringForward,
		BringToFront,
		CenterHorizontally,
		CenterVertically,
		ViewCode,
		DocumentOutline,
		Copy,
		Cut,
		Delete,
		Group,
		HorizSpaceConcatenate,
		HorizSpaceDecrease,
		HorizSpaceIncrease,
		HorizSpaceMakeEqual,
		Paste,
		Properties,
		Redo,
		MultiLevelRedo,
		SelectAll,
		SendBackward,
		SendToBack,
		SizeToControl,
		SizeToControlHeight,
		SizeToControlWidth,
		SizeToFit,
		SizeToGrid,
		SnapToGrid,
		TabOrder,
		Undo,
		MultiLevelUndo,
		Ungroup,
		VertSpaceConcatenate,
		VertSpaceDecrease,
		VertSpaceIncrease,
		VertSpaceMakeEqual ,
		ShowGrid,
		ViewGrid,
		Replace,
		PropertiesWindow,
		LockControls,
		F1Help,
		ArrangeIcons,
		LineupIcons,
		ShowLargeIcons,
		VerbFirst,
		VerbLast
	}
}
