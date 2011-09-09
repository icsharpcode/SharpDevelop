// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

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
		void ShowContextMenu(CommandIDEnum menuID, int x, int y);
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
				case CommandIDEnum.SelectionMenu:
					return MenuCommands.SelectionMenu;
				case CommandIDEnum.ContainerMenu:
					return MenuCommands.ContainerMenu;
				case CommandIDEnum.TraySelectionMenu:
					return MenuCommands.TraySelectionMenu;
				case CommandIDEnum.ComponentTrayMenu:
					return MenuCommands.ComponentTrayMenu;
				case CommandIDEnum.DesignerProperties:
					return MenuCommands.DesignerProperties;
				case CommandIDEnum.KeyCancel:
					return MenuCommands.KeyCancel;
				case CommandIDEnum.KeyReverseCancel:
					return MenuCommands.KeyReverseCancel;
				case CommandIDEnum.KeyInvokeSmartTag:
					return MenuCommands.KeyInvokeSmartTag;
				case CommandIDEnum.KeyDefaultAction:
					return MenuCommands.KeyDefaultAction;
				case CommandIDEnum.KeyMoveUp:
					return MenuCommands.KeyMoveUp;
				case CommandIDEnum.KeyMoveDown:
					return MenuCommands.KeyMoveDown;
				case CommandIDEnum.KeyMoveLeft:
					return MenuCommands.KeyMoveLeft;
				case CommandIDEnum.KeyMoveRight:
					return MenuCommands.KeyMoveRight;
				case CommandIDEnum.KeyNudgeUp:
					return MenuCommands.KeyNudgeUp;
				case CommandIDEnum.KeyNudgeDown:
					return MenuCommands.KeyNudgeDown;
				case CommandIDEnum.KeyNudgeLeft:
					return MenuCommands.KeyNudgeLeft;
				case CommandIDEnum.KeyNudgeRight:
					return MenuCommands.KeyNudgeRight;
				case CommandIDEnum.KeySizeWidthIncrease:
					return MenuCommands.KeySizeWidthIncrease;
				case CommandIDEnum.KeySizeHeightIncrease:
					return MenuCommands.KeySizeHeightIncrease;
				case CommandIDEnum.KeySizeWidthDecrease:
					return MenuCommands.KeySizeWidthDecrease;
				case CommandIDEnum.KeySizeHeightDecrease:
					return MenuCommands.KeySizeHeightDecrease;
				case CommandIDEnum.KeyNudgeWidthIncrease:
					return MenuCommands.KeyNudgeWidthIncrease;
				case CommandIDEnum.KeyNudgeHeightIncrease:
					return MenuCommands.KeyNudgeHeightIncrease;
				case CommandIDEnum.KeyNudgeWidthDecrease:
					return MenuCommands.KeyNudgeWidthDecrease;
				case CommandIDEnum.KeyNudgeHeightDecrease:
					return MenuCommands.KeyNudgeHeightDecrease;
				case CommandIDEnum.KeySelectNext:
					return MenuCommands.KeySelectNext;
				case CommandIDEnum.KeySelectPrevious:
					return MenuCommands.KeySelectPrevious;
				case CommandIDEnum.KeyTabOrderSelect:
					return MenuCommands.KeyTabOrderSelect;
				case CommandIDEnum.EditLabel:
					return MenuCommands.EditLabel;
				case CommandIDEnum.KeyHome:
					return MenuCommands.KeyHome;
				case CommandIDEnum.KeyEnd:
					return MenuCommands.KeyEnd;
				case CommandIDEnum.KeyShiftHome:
					return MenuCommands.KeyShiftHome;
				case CommandIDEnum.KeyShiftEnd:
					return MenuCommands.KeyShiftEnd;
				case CommandIDEnum.SetStatusText:
					return MenuCommands.SetStatusText;
				case CommandIDEnum.SetStatusRectangle:
					return MenuCommands.SetStatusRectangle;
				default:
					throw new Exception("Invalid value for CommandIDEnum");
			}
		}
		
		public static CommandIDEnum ToCommandIDEnum(CommandID key)
		{
			if (key.Equals(StandardCommands.AlignBottom))
				return CommandIDEnum.AlignBottom;
			if (key.Equals(StandardCommands.AlignHorizontalCenters))
				return CommandIDEnum.AlignHorizontalCenters;
			if (key.Equals(StandardCommands.AlignLeft))
				return CommandIDEnum.AlignLeft;
			if (key.Equals(StandardCommands.AlignRight))
				return CommandIDEnum.AlignRight;
			if (key.Equals(StandardCommands.AlignToGrid))
				return CommandIDEnum.AlignToGrid;
			if (key.Equals(StandardCommands.AlignTop))
				return CommandIDEnum.AlignTop;
			if (key.Equals(StandardCommands.AlignVerticalCenters))
				return CommandIDEnum.AlignVerticalCenters;
			if (key.Equals(StandardCommands.ArrangeBottom))
				return CommandIDEnum.ArrangeBottom;
			if (key.Equals(StandardCommands.ArrangeIcons))
				return CommandIDEnum.ArrangeIcons;
			if (key.Equals(StandardCommands.ArrangeRight))
				return CommandIDEnum.ArrangeRight;
			if (key.Equals(StandardCommands.BringForward))
				return CommandIDEnum.BringForward;
			if (key.Equals(StandardCommands.BringToFront))
				return CommandIDEnum.BringToFront;
			if (key.Equals(StandardCommands.CenterHorizontally))
				return CommandIDEnum.CenterHorizontally;
			if (key.Equals(StandardCommands.CenterVertically))
				return CommandIDEnum.CenterVertically;
			if (key.Equals(StandardCommands.Copy))
				return CommandIDEnum.Copy;
			if (key.Equals(StandardCommands.Cut))
				return CommandIDEnum.Cut;
			if (key.Equals(StandardCommands.Delete))
				return CommandIDEnum.Delete;
			if (key.Equals(StandardCommands.DocumentOutline))
				return CommandIDEnum.DocumentOutline;
			if (key.Equals(StandardCommands.F1Help))
				return CommandIDEnum.F1Help;
			if (key.Equals(StandardCommands.Group))
				return CommandIDEnum.Group;
			if (key.Equals(StandardCommands.HorizSpaceConcatenate))
				return CommandIDEnum.HorizSpaceConcatenate;
			if (key.Equals(StandardCommands.HorizSpaceDecrease))
				return CommandIDEnum.HorizSpaceDecrease;
			if (key.Equals(StandardCommands.HorizSpaceIncrease))
				return CommandIDEnum.HorizSpaceIncrease;
			if (key.Equals(StandardCommands.HorizSpaceMakeEqual))
				return CommandIDEnum.HorizSpaceMakeEqual;
			if (key.Equals(StandardCommands.LineupIcons))
				return CommandIDEnum.LineupIcons;
			if (key.Equals(StandardCommands.LockControls))
				return CommandIDEnum.LockControls;
			if (key.Equals(StandardCommands.MultiLevelRedo))
				return CommandIDEnum.MultiLevelRedo;
			if (key.Equals(StandardCommands.MultiLevelUndo))
				return CommandIDEnum.MultiLevelUndo;
			if (key.Equals(StandardCommands.Paste))
				return CommandIDEnum.Paste;
			if (key.Equals(StandardCommands.Properties))
				return CommandIDEnum.Properties;
			if (key.Equals(StandardCommands.PropertiesWindow))
				return CommandIDEnum.PropertiesWindow;
			if (key.Equals(StandardCommands.Redo))
				return CommandIDEnum.Redo;
			if (key.Equals(StandardCommands.Replace))
				return CommandIDEnum.Replace;
			if (key.Equals(StandardCommands.SelectAll))
				return CommandIDEnum.SelectAll;
			if (key.Equals(StandardCommands.SendBackward))
				return CommandIDEnum.SendBackward;
			if (key.Equals(StandardCommands.SendToBack))
				return CommandIDEnum.SendToBack;
			if (key.Equals(StandardCommands.ShowGrid))
				return CommandIDEnum.ShowGrid;
			if (key.Equals(StandardCommands.ShowLargeIcons))
				return CommandIDEnum.ShowLargeIcons;
			if (key.Equals(StandardCommands.SizeToControl))
				return CommandIDEnum.SizeToControl;
			if (key.Equals(StandardCommands.SizeToControlHeight))
				return CommandIDEnum.SizeToControlHeight;
			if (key.Equals(StandardCommands.SizeToControlWidth))
				return CommandIDEnum.SizeToControlWidth;
			if (key.Equals(StandardCommands.SizeToFit))
				return CommandIDEnum.SizeToFit;
			if (key.Equals(StandardCommands.SizeToGrid))
				return CommandIDEnum.SizeToGrid;
			if (key.Equals(StandardCommands.SnapToGrid))
				return CommandIDEnum.SnapToGrid;
			if (key.Equals(StandardCommands.TabOrder))
				return CommandIDEnum.TabOrder;
			if (key.Equals(StandardCommands.Undo))
				return CommandIDEnum.Undo;
			if (key.Equals(StandardCommands.Ungroup))
				return CommandIDEnum.Ungroup;
			if (key.Equals(StandardCommands.VerbFirst))
				return CommandIDEnum.VerbFirst;
			if (key.Equals(StandardCommands.VerbLast))
				return CommandIDEnum.VerbLast;
			if (key.Equals(StandardCommands.VertSpaceConcatenate))
				return CommandIDEnum.VertSpaceConcatenate;
			if (key.Equals(StandardCommands.VertSpaceDecrease))
				return CommandIDEnum.VertSpaceDecrease;
			if (key.Equals(StandardCommands.VertSpaceIncrease))
				return CommandIDEnum.VertSpaceIncrease;
			if (key.Equals(StandardCommands.VertSpaceMakeEqual))
				return CommandIDEnum.VertSpaceMakeEqual;
			if (key.Equals(StandardCommands.ViewCode))
				return CommandIDEnum.ViewCode;
			if (key.Equals(StandardCommands.ViewGrid))
				return CommandIDEnum.ViewGrid;
			if (key.Equals(MenuCommands.SelectionMenu))
				return CommandIDEnum.SelectionMenu;
			if (key.Equals(MenuCommands.ContainerMenu))
				return CommandIDEnum.ContainerMenu;
			if (key.Equals(MenuCommands.TraySelectionMenu))
				return CommandIDEnum.TraySelectionMenu;
			if (key.Equals(MenuCommands.ComponentTrayMenu))
				return CommandIDEnum.ComponentTrayMenu;
			if (key.Equals(MenuCommands.DesignerProperties))
				return CommandIDEnum.DesignerProperties;
			if (key.Equals(MenuCommands.KeyCancel))
				return CommandIDEnum.KeyCancel;
			if (key.Equals(MenuCommands.KeyReverseCancel))
				return CommandIDEnum.KeyReverseCancel;
			if (key.Equals(MenuCommands.KeyInvokeSmartTag))
				return CommandIDEnum.KeyInvokeSmartTag;
			if (key.Equals(MenuCommands.KeyDefaultAction))
				return CommandIDEnum.KeyDefaultAction;
			if (key.Equals(MenuCommands.KeyMoveUp))
				return CommandIDEnum.KeyMoveUp;
			if (key.Equals(MenuCommands.KeyMoveDown))
				return CommandIDEnum.KeyMoveDown;
			if (key.Equals(MenuCommands.KeyMoveLeft))
				return CommandIDEnum.KeyMoveLeft;
			if (key.Equals(MenuCommands.KeyMoveRight))
				return CommandIDEnum.KeyMoveRight;
			if (key.Equals(MenuCommands.KeyNudgeUp))
				return CommandIDEnum.KeyNudgeUp;
			if (key.Equals(MenuCommands.KeyNudgeDown))
				return CommandIDEnum.KeyNudgeDown;
			if (key.Equals(MenuCommands.KeyNudgeLeft))
				return CommandIDEnum.KeyNudgeLeft;
			if (key.Equals(MenuCommands.KeyNudgeRight))
				return CommandIDEnum.KeyNudgeRight;
			if (key.Equals(MenuCommands.KeySizeWidthIncrease))
				return CommandIDEnum.KeySizeWidthIncrease;
			if (key.Equals(MenuCommands.KeySizeHeightIncrease))
				return CommandIDEnum.KeySizeHeightIncrease;
			if (key.Equals(MenuCommands.KeySizeWidthDecrease))
				return CommandIDEnum.KeySizeWidthDecrease;
			if (key.Equals(MenuCommands.KeySizeHeightDecrease))
				return CommandIDEnum.KeySizeHeightDecrease;
			if (key.Equals(MenuCommands.KeyNudgeWidthIncrease))
				return CommandIDEnum.KeyNudgeWidthIncrease;
			if (key.Equals(MenuCommands.KeyNudgeHeightIncrease))
				return CommandIDEnum.KeyNudgeHeightIncrease;
			if (key.Equals(MenuCommands.KeyNudgeWidthDecrease))
				return CommandIDEnum.KeyNudgeWidthDecrease;
			if (key.Equals(MenuCommands.KeyNudgeHeightDecrease))
				return CommandIDEnum.KeyNudgeHeightDecrease;
			if (key.Equals(MenuCommands.KeySelectNext))
				return CommandIDEnum.KeySelectNext;
			if (key.Equals(MenuCommands.KeySelectPrevious))
				return CommandIDEnum.KeySelectPrevious;
			if (key.Equals(MenuCommands.KeyTabOrderSelect))
				return CommandIDEnum.KeyTabOrderSelect;
			if (key.Equals(MenuCommands.EditLabel))
				return CommandIDEnum.EditLabel;
			if (key.Equals(MenuCommands.KeyHome))
				return CommandIDEnum.KeyHome;
			if (key.Equals(MenuCommands.KeyEnd))
				return CommandIDEnum.KeyEnd;
			if (key.Equals(MenuCommands.KeyShiftHome))
				return CommandIDEnum.KeyShiftHome;
			if (key.Equals(MenuCommands.KeyShiftEnd))
				return CommandIDEnum.KeyShiftEnd;
			if (key.Equals(MenuCommands.SetStatusText))
				return CommandIDEnum.SetStatusText;
			if (key.Equals(MenuCommands.SetStatusRectangle))
				return CommandIDEnum.SetStatusRectangle;
			throw new Exception("Invalid value for CommandID");
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
		VerbLast,
		SelectionMenu,
		ContainerMenu,
		TraySelectionMenu,
		ComponentTrayMenu,
		DesignerProperties,
		KeyCancel,
		KeyReverseCancel,
		KeyInvokeSmartTag,
		KeyDefaultAction,
		KeyMoveUp,
		KeyMoveDown,
		KeyMoveLeft,
		KeyMoveRight,
		KeyNudgeUp,
		KeyNudgeDown,
		KeyNudgeLeft,
		KeyNudgeRight,
		KeySizeWidthIncrease,
		KeySizeHeightIncrease,
		KeySizeWidthDecrease,
		KeySizeHeightDecrease,
		KeyNudgeWidthIncrease,
		KeyNudgeHeightIncrease,
		KeyNudgeWidthDecrease,
		KeyNudgeHeightDecrease,
		KeySelectNext,
		KeySelectPrevious,
		KeyTabOrderSelect,
		EditLabel,
		KeyHome,
		KeyEnd,
		KeyShiftHome,
		KeyShiftEnd,
		SetStatusText,
		SetStatusRectangle
	}
}