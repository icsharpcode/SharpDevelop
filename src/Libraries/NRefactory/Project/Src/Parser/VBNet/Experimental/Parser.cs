using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



namespace ICSharpCode.NRefactory.Parser.VBNet.Experimental {



partial class ExpressionFinder {

	const bool T = true;
	const bool x = false;

int currentState = 1;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token t)
	{
		if (t.kind != expectedKind)
			Error(t);
	}
	
	void Error(Token t) 
	{
	}

	public void InformToken(Token t) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		ApplyToken(t);
		switchlbl: switch (currentState) {
			case 0: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 1: { // start of ExpressionFinder
				PushContext(Context.Global, t);
				goto case 3;
			}
			case 2: {
				stateStack.Push(3);
				goto case 15; // OptionStatement
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 172) {
					goto case 2;
				} else {
					goto case 5;
				}
			}
			case 4: {
				stateStack.Push(5);
				goto case 19; // ImportsStatement
			}
			case 5: {
				if (t == null) { currentState = 5; break; }
				if (t.kind == 136) {
					goto case 4;
				} else {
					goto case 7;
				}
			}
			case 6: {
				stateStack.Push(7);
				goto case 24; // AttributeBlock
			}
			case 7: {
				if (t == null) { currentState = 7; break; }
				if (t.kind == 39) {
					goto case 6;
				} else {
					goto case 9;
				}
			}
			case 8: {
				stateStack.Push(9);
				goto case 34; // NamespaceMemberDeclaration
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[0, t.kind]) {
					goto case 8;
				} else {
					goto case 10;
				}
			}
			case 10: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 11: {
				if (t == null) { currentState = 11; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 12: {
				if (t == null) { currentState = 12; break; }
				Expect(22, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 13: { // start of StatementTerminator
				if (t == null) { currentState = 13; break; }
				if (t.kind == 1) {
					goto case 11;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (t == null) { currentState = 14; break; }
				if (t.kind == 22) {
					goto case 12;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 15: { // start of OptionStatement
				if (t == null) { currentState = 15; break; }
				Expect(172, t); // "Option"
				currentState = 17;
				break;
			}
			case 16: {
				if (t == null) { currentState = 16; break; }
				currentState = 17;
				break;
			}
			case 17: {
				if (t == null) { currentState = 17; break; }
				if (set[1, t.kind]) {
					goto case 16;
				} else {
					goto case 18;
				}
			}
			case 18: {
				goto case 13; // StatementTerminator
			}
			case 19: { // start of ImportsStatement
				if (t == null) { currentState = 19; break; }
				Expect(136, t); // "Imports"
				currentState = 20;
				break;
			}
			case 20: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 22;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				if (set[1, t.kind]) {
					goto case 21;
				} else {
					goto case 23;
				}
			}
			case 23: {
				goto case 13; // StatementTerminator
			}
			case 24: { // start of AttributeBlock
				if (t == null) { currentState = 24; break; }
				Expect(39, t); // "<"
				currentState = 25;
				break;
			}
			case 25: {
				PushContext(Context.Attribute, t);
				goto case 27;
			}
			case 26: {
				if (t == null) { currentState = 26; break; }
				currentState = 27;
				break;
			}
			case 27: {
				if (t == null) { currentState = 27; break; }
				if (set[2, t.kind]) {
					goto case 26;
				} else {
					goto case 28;
				}
			}
			case 28: {
				if (t == null) { currentState = 28; break; }
				Expect(38, t); // ">"
				currentState = 29;
				break;
			}
			case 29: {
				PopContext();
				goto case 31;
			}
			case 30: {
				if (t == null) { currentState = 30; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 31: {
				if (t == null) { currentState = 31; break; }
				if (t.kind == 1) {
					goto case 30;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 32: {
				goto case 36; // NamespaceDeclaration
			}
			case 33: {
				goto case 46; // TypeDeclaration
			}
			case 34: { // start of NamespaceMemberDeclaration
				if (t == null) { currentState = 34; break; }
				if (t.kind == 159) {
					goto case 32;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (set[3, t.kind]) {
					goto case 33;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: { // start of NamespaceDeclaration
				if (t == null) { currentState = 36; break; }
				Expect(159, t); // "Namespace"
				currentState = 38;
				break;
			}
			case 37: {
				if (t == null) { currentState = 37; break; }
				currentState = 38;
				break;
			}
			case 38: {
				if (t == null) { currentState = 38; break; }
				if (set[1, t.kind]) {
					goto case 37;
				} else {
					goto case 39;
				}
			}
			case 39: {
				stateStack.Push(41);
				goto case 13; // StatementTerminator
			}
			case 40: {
				stateStack.Push(41);
				goto case 34; // NamespaceMemberDeclaration
			}
			case 41: {
				if (t == null) { currentState = 41; break; }
				if (set[4, t.kind]) {
					goto case 40;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				Expect(112, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				if (t == null) { currentState = 43; break; }
				Expect(159, t); // "Namespace"
				currentState = 44;
				break;
			}
			case 44: {
				goto case 13; // StatementTerminator
			}
			case 45: {
				stateStack.Push(46);
				goto case 24; // AttributeBlock
			}
			case 46: { // start of TypeDeclaration
				if (t == null) { currentState = 46; break; }
				if (t.kind == 39) {
					goto case 45;
				} else {
					goto case 48;
				}
			}
			case 47: {
				stateStack.Push(48);
				goto case 531; // TypeModifier
			}
			case 48: {
				if (t == null) { currentState = 48; break; }
				if (set[5, t.kind]) {
					goto case 47;
				} else {
					goto case 51;
				}
			}
			case 49: {
				if (t == null) { currentState = 49; break; }
				Expect(154, t); // "Module"
				currentState = 54;
				break;
			}
			case 50: {
				if (t == null) { currentState = 50; break; }
				Expect(83, t); // "Class"
				currentState = 54;
				break;
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				if (t.kind == 154) {
					goto case 49;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				if (t.kind == 83) {
					goto case 50;
				} else {
					Error(t);
					goto case 54;
				}
			}
			case 53: {
				if (t == null) { currentState = 53; break; }
				currentState = 54;
				break;
			}
			case 54: {
				if (t == null) { currentState = 54; break; }
				if (set[1, t.kind]) {
					goto case 53;
				} else {
					goto case 55;
				}
			}
			case 55: {
				stateStack.Push(56);
				goto case 13; // StatementTerminator
			}
			case 56: {
				PushContext(Context.Type, t);
				goto case 58;
			}
			case 57: {
				stateStack.Push(58);
				goto case 66; // MemberDeclaration
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				if (set[6, t.kind]) {
					goto case 57;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (t == null) { currentState = 59; break; }
				Expect(112, t); // "End"
				currentState = 62;
				break;
			}
			case 60: {
				if (t == null) { currentState = 60; break; }
				Expect(154, t); // "Module"
				currentState = 64;
				break;
			}
			case 61: {
				if (t == null) { currentState = 61; break; }
				Expect(83, t); // "Class"
				currentState = 64;
				break;
			}
			case 62: {
				if (t == null) { currentState = 62; break; }
				if (t.kind == 154) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				if (t.kind == 83) {
					goto case 61;
				} else {
					Error(t);
					goto case 64;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 13; // StatementTerminator
			}
			case 65: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 66: { // start of MemberDeclaration
				PushContext(Context.Member, t);
				goto case 68;
			}
			case 67: {
				stateStack.Push(68);
				goto case 24; // AttributeBlock
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 39) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 535; // MemberModifier
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				if (set[7, t.kind]) {
					goto case 69;
				} else {
					goto case 73;
				}
			}
			case 71: {
				stateStack.Push(83);
				goto case 191; // MemberVariableOrConstantDeclaration
			}
			case 72: {
				stateStack.Push(83);
				goto case 86; // SubOrFunctionDeclaration
			}
			case 73: {
				if (t == null) { currentState = 73; break; }
				if (set[8, t.kind]) {
					goto case 71;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				if (t.kind == 126 || t.kind == 208) {
					goto case 72;
				} else {
					goto case 76;
				}
			}
			case 75: {
				stateStack.Push(83);
				goto case 106; // ExternalMemberDeclaration
			}
			case 76: {
				if (t == null) { currentState = 76; break; }
				if (t.kind == 100) {
					goto case 75;
				} else {
					goto case 78;
				}
			}
			case 77: {
				stateStack.Push(83);
				goto case 130; // EventMemberDeclaration
			}
			case 78: {
				if (t == null) { currentState = 78; break; }
				if (t.kind == 118) {
					goto case 77;
				} else {
					goto case 80;
				}
			}
			case 79: {
				stateStack.Push(83);
				goto case 148; // CustomEventMemberDeclaration
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (t.kind == 97) {
					goto case 79;
				} else {
					goto case 82;
				}
			}
			case 81: {
				stateStack.Push(83);
				goto case 175; // OperatorDeclaration
			}
			case 82: {
				if (t == null) { currentState = 82; break; }
				if (t.kind == 171) {
					goto case 81;
				} else {
					Error(t);
					goto case 83;
				}
			}
			case 83: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				Expect(208, t); // "Sub"
				currentState = 88;
				break;
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				Expect(126, t); // "Function"
				currentState = 88;
				break;
			}
			case 86: { // start of SubOrFunctionDeclaration
				if (t == null) { currentState = 86; break; }
				if (t.kind == 208) {
					goto case 84;
				} else {
					goto case 87;
				}
			}
			case 87: {
				if (t == null) { currentState = 87; break; }
				if (t.kind == 126) {
					goto case 85;
				} else {
					Error(t);
					goto case 88;
				}
			}
			case 88: {
				PushContext(Context.IdentifierExpected, t);
				goto case 89;
			}
			case 89: {
				if (t == null) { currentState = 89; break; }
				currentState = 90;
				break;
			}
			case 90: {
				PopContext();
				goto case 95;
			}
			case 91: {
				if (t == null) { currentState = 91; break; }
				Expect(36, t); // "("
				currentState = 93;
				break;
			}
			case 92: {
				stateStack.Push(94);
				goto case 200; // ParameterList
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				if (set[9, t.kind]) {
					goto case 92;
				} else {
					goto case 94;
				}
			}
			case 94: {
				if (t == null) { currentState = 94; break; }
				Expect(37, t); // ")"
				currentState = 98;
				break;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 36) {
					goto case 91;
				} else {
					goto case 98;
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				Expect(62, t); // "As"
				currentState = 97;
				break;
			}
			case 97: {
				stateStack.Push(99);
				goto case 310; // TypeName
			}
			case 98: {
				if (t == null) { currentState = 98; break; }
				if (t.kind == 62) {
					goto case 96;
				} else {
					goto case 99;
				}
			}
			case 99: {
				stateStack.Push(100);
				goto case 215; // Block
			}
			case 100: {
				if (t == null) { currentState = 100; break; }
				Expect(112, t); // "End"
				currentState = 103;
				break;
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				Expect(208, t); // "Sub"
				currentState = 105;
				break;
			}
			case 102: {
				if (t == null) { currentState = 102; break; }
				Expect(126, t); // "Function"
				currentState = 105;
				break;
			}
			case 103: {
				if (t == null) { currentState = 103; break; }
				if (t.kind == 208) {
					goto case 101;
				} else {
					goto case 104;
				}
			}
			case 104: {
				if (t == null) { currentState = 104; break; }
				if (t.kind == 126) {
					goto case 102;
				} else {
					Error(t);
					goto case 105;
				}
			}
			case 105: {
				goto case 13; // StatementTerminator
			}
			case 106: { // start of ExternalMemberDeclaration
				if (t == null) { currentState = 106; break; }
				Expect(100, t); // "Declare"
				currentState = 113;
				break;
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				Expect(61, t); // "Ansi"
				currentState = 116;
				break;
			}
			case 108: {
				if (t == null) { currentState = 108; break; }
				Expect(221, t); // "Unicode"
				currentState = 116;
				break;
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				if (t.kind == 61) {
					goto case 107;
				} else {
					goto case 110;
				}
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				if (t.kind == 221) {
					goto case 108;
				} else {
					goto case 112;
				}
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				Expect(65, t); // "Auto"
				currentState = 116;
				break;
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				if (t.kind == 65) {
					goto case 111;
				} else {
					Error(t);
					goto case 116;
				}
			}
			case 113: {
				if (t == null) { currentState = 113; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					goto case 109;
				} else {
					goto case 116;
				}
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				Expect(208, t); // "Sub"
				currentState = 118;
				break;
			}
			case 115: {
				if (t == null) { currentState = 115; break; }
				Expect(126, t); // "Function"
				currentState = 118;
				break;
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				if (t.kind == 208) {
					goto case 114;
				} else {
					goto case 117;
				}
			}
			case 117: {
				if (t == null) { currentState = 117; break; }
				if (t.kind == 126) {
					goto case 115;
				} else {
					Error(t);
					goto case 118;
				}
			}
			case 118: {
				stateStack.Push(119);
				goto case 459; // Identifier
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				Expect(148, t); // "Lib"
				currentState = 120;
				break;
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				Expect(3, t); // LiteralString
				currentState = 123;
				break;
			}
			case 121: {
				if (t == null) { currentState = 121; break; }
				Expect(58, t); // "Alias"
				currentState = 122;
				break;
			}
			case 122: {
				if (t == null) { currentState = 122; break; }
				Expect(3, t); // LiteralString
				currentState = 128;
				break;
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				if (t.kind == 58) {
					goto case 121;
				} else {
					goto case 128;
				}
			}
			case 124: {
				if (t == null) { currentState = 124; break; }
				Expect(36, t); // "("
				currentState = 126;
				break;
			}
			case 125: {
				stateStack.Push(127);
				goto case 200; // ParameterList
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (set[9, t.kind]) {
					goto case 125;
				} else {
					goto case 127;
				}
			}
			case 127: {
				if (t == null) { currentState = 127; break; }
				Expect(37, t); // ")"
				currentState = 129;
				break;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 36) {
					goto case 124;
				} else {
					goto case 129;
				}
			}
			case 129: {
				goto case 13; // StatementTerminator
			}
			case 130: { // start of EventMemberDeclaration
				if (t == null) { currentState = 130; break; }
				Expect(118, t); // "Event"
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(139);
				goto case 459; // Identifier
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				Expect(62, t); // "As"
				currentState = 133;
				break;
			}
			case 133: {
				stateStack.Push(146);
				goto case 310; // TypeName
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				Expect(36, t); // "("
				currentState = 136;
				break;
			}
			case 135: {
				stateStack.Push(137);
				goto case 200; // ParameterList
			}
			case 136: {
				if (t == null) { currentState = 136; break; }
				if (set[9, t.kind]) {
					goto case 135;
				} else {
					goto case 137;
				}
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				Expect(37, t); // ")"
				currentState = 146;
				break;
			}
			case 138: {
				if (t == null) { currentState = 138; break; }
				if (t.kind == 36) {
					goto case 134;
				} else {
					goto case 146;
				}
			}
			case 139: {
				if (t == null) { currentState = 139; break; }
				if (t.kind == 62) {
					goto case 132;
				} else {
					goto case 140;
				}
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				if (set[10, t.kind]) {
					goto case 138;
				} else {
					Error(t);
					goto case 146;
				}
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				Expect(135, t); // "Implements"
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(145);
				goto case 310; // TypeName
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				Expect(23, t); // ","
				currentState = 144;
				break;
			}
			case 144: {
				stateStack.Push(145);
				goto case 310; // TypeName
			}
			case 145: {
				if (t == null) { currentState = 145; break; }
				if (t.kind == 23) {
					goto case 143;
				} else {
					goto case 147;
				}
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (t.kind == 135) {
					goto case 141;
				} else {
					goto case 147;
				}
			}
			case 147: {
				goto case 13; // StatementTerminator
			}
			case 148: { // start of CustomEventMemberDeclaration
				if (t == null) { currentState = 148; break; }
				Expect(97, t); // "Custom"
				currentState = 149;
				break;
			}
			case 149: {
				stateStack.Push(171);
				goto case 130; // EventMemberDeclaration
			}
			case 150: {
				stateStack.Push(151);
				goto case 24; // AttributeBlock
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				if (t.kind == 39) {
					goto case 150;
				} else {
					goto case 154;
				}
			}
			case 152: {
				if (t == null) { currentState = 152; break; }
				Expect(55, t); // "AddHandler"
				currentState = 158;
				break;
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				Expect(191, t); // "RemoveHandler"
				currentState = 158;
				break;
			}
			case 154: {
				if (t == null) { currentState = 154; break; }
				if (t.kind == 55) {
					goto case 152;
				} else {
					goto case 155;
				}
			}
			case 155: {
				if (t == null) { currentState = 155; break; }
				if (t.kind == 191) {
					goto case 153;
				} else {
					goto case 157;
				}
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 158;
				break;
			}
			case 157: {
				if (t == null) { currentState = 157; break; }
				if (t.kind == 187) {
					goto case 156;
				} else {
					Error(t);
					goto case 158;
				}
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				Expect(36, t); // "("
				currentState = 159;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 200; // ParameterList
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				Expect(37, t); // ")"
				currentState = 161;
				break;
			}
			case 161: {
				if (t == null) { currentState = 161; break; }
				Expect(1, t); // EOL
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 215; // Block
			}
			case 163: {
				if (t == null) { currentState = 163; break; }
				Expect(112, t); // "End"
				currentState = 166;
				break;
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				Expect(55, t); // "AddHandler"
				currentState = 170;
				break;
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				Expect(191, t); // "RemoveHandler"
				currentState = 170;
				break;
			}
			case 166: {
				if (t == null) { currentState = 166; break; }
				if (t.kind == 55) {
					goto case 164;
				} else {
					goto case 167;
				}
			}
			case 167: {
				if (t == null) { currentState = 167; break; }
				if (t.kind == 191) {
					goto case 165;
				} else {
					goto case 169;
				}
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 170;
				break;
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				if (t.kind == 187) {
					goto case 168;
				} else {
					Error(t);
					goto case 170;
				}
			}
			case 170: {
				stateStack.Push(171);
				goto case 13; // StatementTerminator
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				if (set[11, t.kind]) {
					goto case 151;
				} else {
					goto case 172;
				}
			}
			case 172: {
				if (t == null) { currentState = 172; break; }
				Expect(112, t); // "End"
				currentState = 173;
				break;
			}
			case 173: {
				if (t == null) { currentState = 173; break; }
				Expect(118, t); // "Event"
				currentState = 174;
				break;
			}
			case 174: {
				goto case 13; // StatementTerminator
			}
			case 175: { // start of OperatorDeclaration
				if (t == null) { currentState = 175; break; }
				Expect(171, t); // "Operator"
				currentState = 176;
				break;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				currentState = 177;
				break;
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				Expect(36, t); // "("
				currentState = 178;
				break;
			}
			case 178: {
				stateStack.Push(179);
				goto case 200; // ParameterList
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				Expect(37, t); // ")"
				currentState = 184;
				break;
			}
			case 180: {
				if (t == null) { currentState = 180; break; }
				Expect(62, t); // "As"
				currentState = 182;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 24; // AttributeBlock
			}
			case 182: {
				if (t == null) { currentState = 182; break; }
				if (t.kind == 39) {
					goto case 181;
				} else {
					goto case 183;
				}
			}
			case 183: {
				stateStack.Push(185);
				goto case 310; // TypeName
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				if (t.kind == 62) {
					goto case 180;
				} else {
					goto case 185;
				}
			}
			case 185: {
				if (t == null) { currentState = 185; break; }
				Expect(1, t); // EOL
				currentState = 186;
				break;
			}
			case 186: {
				stateStack.Push(187);
				goto case 215; // Block
			}
			case 187: {
				if (t == null) { currentState = 187; break; }
				Expect(112, t); // "End"
				currentState = 188;
				break;
			}
			case 188: {
				if (t == null) { currentState = 188; break; }
				Expect(171, t); // "Operator"
				currentState = 189;
				break;
			}
			case 189: {
				goto case 13; // StatementTerminator
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				Expect(87, t); // "Const"
				currentState = 192;
				break;
			}
			case 191: { // start of MemberVariableOrConstantDeclaration
				if (t == null) { currentState = 191; break; }
				if (t.kind == 87) {
					goto case 190;
				} else {
					goto case 192;
				}
			}
			case 192: {
				stateStack.Push(195);
				goto case 467; // IdentifierForFieldDeclaration
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				Expect(62, t); // "As"
				currentState = 194;
				break;
			}
			case 194: {
				stateStack.Push(198);
				goto case 310; // TypeName
			}
			case 195: {
				if (t == null) { currentState = 195; break; }
				if (t.kind == 62) {
					goto case 193;
				} else {
					goto case 198;
				}
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				Expect(21, t); // "="
				currentState = 197;
				break;
			}
			case 197: {
				stateStack.Push(199);
				goto case 222; // Expression
			}
			case 198: {
				if (t == null) { currentState = 198; break; }
				if (t.kind == 21) {
					goto case 196;
				} else {
					goto case 199;
				}
			}
			case 199: {
				goto case 13; // StatementTerminator
			}
			case 200: { // start of ParameterList
				stateStack.Push(203);
				goto case 205; // Parameter
			}
			case 201: {
				if (t == null) { currentState = 201; break; }
				Expect(23, t); // ","
				currentState = 202;
				break;
			}
			case 202: {
				stateStack.Push(203);
				goto case 205; // Parameter
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (t.kind == 23) {
					goto case 201;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 24; // AttributeBlock
			}
			case 205: { // start of Parameter
				if (t == null) { currentState = 205; break; }
				if (t.kind == 39) {
					goto case 204;
				} else {
					goto case 207;
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 561; // ParameterModifier
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				if (set[12, t.kind]) {
					goto case 206;
				} else {
					goto case 208;
				}
			}
			case 208: {
				stateStack.Push(211);
				goto case 459; // Identifier
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				Expect(62, t); // "As"
				currentState = 210;
				break;
			}
			case 210: {
				stateStack.Push(214);
				goto case 310; // TypeName
			}
			case 211: {
				if (t == null) { currentState = 211; break; }
				if (t.kind == 62) {
					goto case 209;
				} else {
					goto case 214;
				}
			}
			case 212: {
				if (t == null) { currentState = 212; break; }
				Expect(21, t); // "="
				currentState = 213;
				break;
			}
			case 213: {
				goto case 222; // Expression
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				if (t.kind == 21) {
					goto case 212;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 215: { // start of Block
				PushContext(Context.Body, t);
				goto case 216;
			}
			case 216: {
				stateStack.Push(220);
				goto case 13; // StatementTerminator
			}
			case 217: {
				stateStack.Push(219);
				goto case 361; // Statement
			}
			case 218: {
				if (t == null) { currentState = 218; break; }
				if (set[13, t.kind]) {
					goto case 217;
				} else {
					goto case 219;
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 13; // StatementTerminator
			}
			case 220: {
				if (t == null) { currentState = 220; break; }
				if (set[14, t.kind]) {
					goto case 218;
				} else {
					goto case 221;
				}
			}
			case 221: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 222: { // start of Expression
				goto case 223; // SimpleExpressionWithSuffix
			}
			case 223: { // start of SimpleExpressionWithSuffix
				stateStack.Push(225);
				goto case 226; // SimpleExpression
			}
			case 224: {
				stateStack.Push(225);
				goto case 251; // ExpressionSuffix
			}
			case 225: {
				if (t == null) { currentState = 225; break; }
				if (t.kind == 27 || t.kind == 36) {
					goto case 224;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: { // start of SimpleExpression
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 231;
			}
			case 227: {
				goto case 335; // Literal
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				Expect(36, t); // "("
				currentState = 229;
				break;
			}
			case 229: {
				stateStack.Push(230);
				goto case 222; // Expression
			}
			case 230: {
				if (t == null) { currentState = 230; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 231: {
				if (t == null) { currentState = 231; break; }
				if (set[15, t.kind]) {
					goto case 227;
				} else {
					goto case 232;
				}
			}
			case 232: {
				if (t == null) { currentState = 232; break; }
				if (t.kind == 36) {
					goto case 228;
				} else {
					goto case 234;
				}
			}
			case 233: {
				goto case 459; // Identifier
			}
			case 234: {
				if (t == null) { currentState = 234; break; }
				if (set[16, t.kind]) {
					goto case 233;
				} else {
					goto case 236;
				}
			}
			case 235: {
				goto case 253; // XmlLiteral
			}
			case 236: {
				if (t == null) { currentState = 236; break; }
				if (t.kind == 10 || t.kind == 17) {
					goto case 235;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 237: {
				if (t == null) { currentState = 237; break; }
				Expect(36, t); // "("
				currentState = 238;
				break;
			}
			case 238: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 247;
			}
			case 239: {
				if (t == null) { currentState = 239; break; }
				Expect(168, t); // "Of"
				currentState = 240;
				break;
			}
			case 240: {
				stateStack.Push(243);
				goto case 310; // TypeName
			}
			case 241: {
				if (t == null) { currentState = 241; break; }
				Expect(23, t); // ","
				currentState = 242;
				break;
			}
			case 242: {
				stateStack.Push(243);
				goto case 310; // TypeName
			}
			case 243: {
				if (t == null) { currentState = 243; break; }
				if (t.kind == 23) {
					goto case 241;
				} else {
					goto case 244;
				}
			}
			case 244: {
				if (t == null) { currentState = 244; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 245: {
				stateStack.Push(246);
				goto case 451; // ArgumentList
			}
			case 246: {
				if (t == null) { currentState = 246; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				if (t.kind == 168) {
					goto case 239;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (t == null) { currentState = 248; break; }
				if (set[17, t.kind]) {
					goto case 245;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				Expect(27, t); // "."
				currentState = 250;
				break;
			}
			case 250: {
				goto case 332; // IdentifierOrKeyword
			}
			case 251: { // start of ExpressionSuffix
				if (t == null) { currentState = 251; break; }
				if (t.kind == 36) {
					goto case 237;
				} else {
					goto case 252;
				}
			}
			case 252: {
				if (t == null) { currentState = 252; break; }
				if (t.kind == 27) {
					goto case 249;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 253: { // start of XmlLiteral
				PushContext(Context.Xml, t);
				goto case 255;
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				Expect(17, t); // XmlComment
				currentState = 255;
				break;
			}
			case 255: {
				if (t == null) { currentState = 255; break; }
				if (t.kind == 17) {
					goto case 254;
				} else {
					goto case 256;
				}
			}
			case 256: {
				stateStack.Push(258);
				goto case 260; // XmlElement
			}
			case 257: {
				if (t == null) { currentState = 257; break; }
				Expect(17, t); // XmlComment
				currentState = 258;
				break;
			}
			case 258: {
				if (t == null) { currentState = 258; break; }
				if (t.kind == 17) {
					goto case 257;
				} else {
					goto case 259;
				}
			}
			case 259: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 260: { // start of XmlElement
				if (t == null) { currentState = 260; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 262;
				break;
			}
			case 261: {
				if (t == null) { currentState = 261; break; }
				currentState = 262;
				break;
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				if (set[18, t.kind]) {
					goto case 261;
				} else {
					goto case 274;
				}
			}
			case 263: {
				if (t == null) { currentState = 263; break; }
				Expect(14, t); // XmlCloseTagEmptyElement
				currentState = stateStack.Pop();
				break;
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				Expect(11, t); // XmlCloseTag
				currentState = 269;
				break;
			}
			case 265: {
				if (t == null) { currentState = 265; break; }
				currentState = 269;
				break;
			}
			case 266: {
				stateStack.Push(269);
				goto case 260; // XmlElement
			}
			case 267: {
				if (t == null) { currentState = 267; break; }
				if (set[19, t.kind]) {
					goto case 265;
				} else {
					goto case 268;
				}
			}
			case 268: {
				if (t == null) { currentState = 268; break; }
				if (t.kind == 10) {
					goto case 266;
				} else {
					Error(t);
					goto case 269;
				}
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				if (set[20, t.kind]) {
					goto case 267;
				} else {
					goto case 270;
				}
			}
			case 270: {
				if (t == null) { currentState = 270; break; }
				Expect(15, t); // XmlOpenEndTag
				currentState = 272;
				break;
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				currentState = 272;
				break;
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				if (set[21, t.kind]) {
					goto case 271;
				} else {
					goto case 273;
				}
			}
			case 273: {
				if (t == null) { currentState = 273; break; }
				Expect(11, t); // XmlCloseTag
				currentState = stateStack.Pop();
				break;
			}
			case 274: {
				if (t == null) { currentState = 274; break; }
				if (t.kind == 14) {
					goto case 263;
				} else {
					goto case 275;
				}
			}
			case 275: {
				if (t == null) { currentState = 275; break; }
				if (t.kind == 11) {
					goto case 264;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 276: {
				if (t == null) { currentState = 276; break; }
				Expect(70, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				Expect(194, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 278: { // start of PrimitiveTypeName
				if (t == null) { currentState = 278; break; }
				if (t.kind == 70) {
					goto case 276;
				} else {
					goto case 279;
				}
			}
			case 279: {
				if (t == null) { currentState = 279; break; }
				if (t.kind == 194) {
					goto case 277;
				} else {
					goto case 281;
				}
			}
			case 280: {
				if (t == null) { currentState = 280; break; }
				Expect(223, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				if (t == null) { currentState = 281; break; }
				if (t.kind == 223) {
					goto case 280;
				} else {
					goto case 283;
				}
			}
			case 282: {
				if (t == null) { currentState = 282; break; }
				Expect(199, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 283: {
				if (t == null) { currentState = 283; break; }
				if (t.kind == 199) {
					goto case 282;
				} else {
					goto case 285;
				}
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(219, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 285: {
				if (t == null) { currentState = 285; break; }
				if (t.kind == 219) {
					goto case 284;
				} else {
					goto case 287;
				}
			}
			case 286: {
				if (t == null) { currentState = 286; break; }
				Expect(140, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 287: {
				if (t == null) { currentState = 287; break; }
				if (t.kind == 140) {
					goto case 286;
				} else {
					goto case 289;
				}
			}
			case 288: {
				if (t == null) { currentState = 288; break; }
				Expect(220, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 289: {
				if (t == null) { currentState = 289; break; }
				if (t.kind == 220) {
					goto case 288;
				} else {
					goto case 291;
				}
			}
			case 290: {
				if (t == null) { currentState = 290; break; }
				Expect(150, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 291: {
				if (t == null) { currentState = 291; break; }
				if (t.kind == 150) {
					goto case 290;
				} else {
					goto case 293;
				}
			}
			case 292: {
				if (t == null) { currentState = 292; break; }
				Expect(200, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 293: {
				if (t == null) { currentState = 293; break; }
				if (t.kind == 200) {
					goto case 292;
				} else {
					goto case 295;
				}
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				Expect(108, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				if (t.kind == 108) {
					goto case 294;
				} else {
					goto case 297;
				}
			}
			case 296: {
				if (t == null) { currentState = 296; break; }
				Expect(99, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 297: {
				if (t == null) { currentState = 297; break; }
				if (t.kind == 99) {
					goto case 296;
				} else {
					goto case 299;
				}
			}
			case 298: {
				if (t == null) { currentState = 298; break; }
				Expect(67, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 299: {
				if (t == null) { currentState = 299; break; }
				if (t.kind == 67) {
					goto case 298;
				} else {
					goto case 301;
				}
			}
			case 300: {
				if (t == null) { currentState = 300; break; }
				Expect(98, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 301: {
				if (t == null) { currentState = 301; break; }
				if (t.kind == 98) {
					goto case 300;
				} else {
					goto case 303;
				}
			}
			case 302: {
				if (t == null) { currentState = 302; break; }
				Expect(81, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 303: {
				if (t == null) { currentState = 303; break; }
				if (t.kind == 81) {
					goto case 302;
				} else {
					goto case 305;
				}
			}
			case 304: {
				if (t == null) { currentState = 304; break; }
				Expect(206, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 305: {
				if (t == null) { currentState = 305; break; }
				if (t.kind == 206) {
					goto case 304;
				} else {
					goto case 307;
				}
			}
			case 306: {
				if (t == null) { currentState = 306; break; }
				Expect(167, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 307: {
				if (t == null) { currentState = 307; break; }
				if (t.kind == 167) {
					goto case 306;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 308: {
				if (t == null) { currentState = 308; break; }
				Expect(129, t); // "Global"
				currentState = 315;
				break;
			}
			case 309: {
				stateStack.Push(315);
				goto case 459; // Identifier
			}
			case 310: { // start of TypeName
				if (t == null) { currentState = 310; break; }
				if (t.kind == 129) {
					goto case 308;
				} else {
					goto case 311;
				}
			}
			case 311: {
				if (t == null) { currentState = 311; break; }
				if (set[16, t.kind]) {
					goto case 309;
				} else {
					goto case 313;
				}
			}
			case 312: {
				stateStack.Push(315);
				goto case 278; // PrimitiveTypeName
			}
			case 313: {
				if (t == null) { currentState = 313; break; }
				if (set[22, t.kind]) {
					goto case 312;
				} else {
					Error(t);
					goto case 315;
				}
			}
			case 314: {
				stateStack.Push(315);
				goto case 321; // TypeSuffix
			}
			case 315: {
				if (t == null) { currentState = 315; break; }
				if (t.kind == 36) {
					goto case 314;
				} else {
					goto case 320;
				}
			}
			case 316: {
				if (t == null) { currentState = 316; break; }
				Expect(27, t); // "."
				currentState = 317;
				break;
			}
			case 317: {
				stateStack.Push(319);
				goto case 332; // IdentifierOrKeyword
			}
			case 318: {
				stateStack.Push(319);
				goto case 321; // TypeSuffix
			}
			case 319: {
				if (t == null) { currentState = 319; break; }
				if (t.kind == 36) {
					goto case 318;
				} else {
					goto case 320;
				}
			}
			case 320: {
				if (t == null) { currentState = 320; break; }
				if (t.kind == 27) {
					goto case 316;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 321: { // start of TypeSuffix
				if (t == null) { currentState = 321; break; }
				Expect(36, t); // "("
				currentState = 329;
				break;
			}
			case 322: {
				if (t == null) { currentState = 322; break; }
				Expect(168, t); // "Of"
				currentState = 323;
				break;
			}
			case 323: {
				stateStack.Push(326);
				goto case 310; // TypeName
			}
			case 324: {
				if (t == null) { currentState = 324; break; }
				Expect(23, t); // ","
				currentState = 325;
				break;
			}
			case 325: {
				stateStack.Push(326);
				goto case 310; // TypeName
			}
			case 326: {
				if (t == null) { currentState = 326; break; }
				if (t.kind == 23) {
					goto case 324;
				} else {
					goto case 331;
				}
			}
			case 327: {
				if (t == null) { currentState = 327; break; }
				Expect(23, t); // ","
				currentState = 328;
				break;
			}
			case 328: {
				if (t == null) { currentState = 328; break; }
				if (t.kind == 23) {
					goto case 327;
				} else {
					goto case 331;
				}
			}
			case 329: {
				if (t == null) { currentState = 329; break; }
				if (t.kind == 168) {
					goto case 322;
				} else {
					goto case 330;
				}
			}
			case 330: {
				if (t == null) { currentState = 330; break; }
				if (t.kind == 23 || t.kind == 37) {
					goto case 328;
				} else {
					Error(t);
					goto case 331;
				}
			}
			case 331: {
				if (t == null) { currentState = 331; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 332: { // start of IdentifierOrKeyword
				if (t == null) { currentState = 332; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 333: {
				if (t == null) { currentState = 333; break; }
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 334: {
				if (t == null) { currentState = 334; break; }
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 335: { // start of Literal
				if (t == null) { currentState = 335; break; }
				if (t.kind == 3) {
					goto case 333;
				} else {
					goto case 336;
				}
			}
			case 336: {
				if (t == null) { currentState = 336; break; }
				if (t.kind == 4) {
					goto case 334;
				} else {
					goto case 338;
				}
			}
			case 337: {
				if (t == null) { currentState = 337; break; }
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 338: {
				if (t == null) { currentState = 338; break; }
				if (t.kind == 5) {
					goto case 337;
				} else {
					goto case 340;
				}
			}
			case 339: {
				if (t == null) { currentState = 339; break; }
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 340: {
				if (t == null) { currentState = 340; break; }
				if (t.kind == 6) {
					goto case 339;
				} else {
					goto case 342;
				}
			}
			case 341: {
				if (t == null) { currentState = 341; break; }
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 342: {
				if (t == null) { currentState = 342; break; }
				if (t.kind == 7) {
					goto case 341;
				} else {
					goto case 344;
				}
			}
			case 343: {
				if (t == null) { currentState = 343; break; }
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 344: {
				if (t == null) { currentState = 344; break; }
				if (t.kind == 8) {
					goto case 343;
				} else {
					goto case 346;
				}
			}
			case 345: {
				if (t == null) { currentState = 345; break; }
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 346: {
				if (t == null) { currentState = 346; break; }
				if (t.kind == 9) {
					goto case 345;
				} else {
					goto case 348;
				}
			}
			case 347: {
				if (t == null) { currentState = 347; break; }
				Expect(215, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 348: {
				if (t == null) { currentState = 348; break; }
				if (t.kind == 215) {
					goto case 347;
				} else {
					goto case 350;
				}
			}
			case 349: {
				if (t == null) { currentState = 349; break; }
				Expect(121, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 350: {
				if (t == null) { currentState = 350; break; }
				if (t.kind == 121) {
					goto case 349;
				} else {
					goto case 352;
				}
			}
			case 351: {
				if (t == null) { currentState = 351; break; }
				Expect(164, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 352: {
				if (t == null) { currentState = 352; break; }
				if (t.kind == 164) {
					goto case 351;
				} else {
					goto case 354;
				}
			}
			case 353: {
				if (t == null) { currentState = 353; break; }
				Expect(152, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 354: {
				if (t == null) { currentState = 354; break; }
				if (t.kind == 152) {
					goto case 353;
				} else {
					goto case 356;
				}
			}
			case 355: {
				if (t == null) { currentState = 355; break; }
				Expect(157, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 356: {
				if (t == null) { currentState = 356; break; }
				if (t.kind == 157) {
					goto case 355;
				} else {
					goto case 358;
				}
			}
			case 357: {
				if (t == null) { currentState = 357; break; }
				Expect(158, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 358: {
				if (t == null) { currentState = 358; break; }
				if (t.kind == 158) {
					goto case 357;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 359: {
				goto case 371; // VariableDeclarationStatement
			}
			case 360: {
				goto case 403; // WithOrLockStatement
			}
			case 361: { // start of Statement
				if (t == null) { currentState = 361; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 359;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (t == null) { currentState = 362; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 360;
				} else {
					goto case 364;
				}
			}
			case 363: {
				goto case 415; // AddOrRemoveHandlerStatement
			}
			case 364: {
				if (t == null) { currentState = 364; break; }
				if (t.kind == 55 || t.kind == 191) {
					goto case 363;
				} else {
					goto case 366;
				}
			}
			case 365: {
				goto case 420; // RaiseEventStatement
			}
			case 366: {
				if (t == null) { currentState = 366; break; }
				if (t.kind == 187) {
					goto case 365;
				} else {
					goto case 368;
				}
			}
			case 367: {
				goto case 427; // InvocationStatement
			}
			case 368: {
				if (t == null) { currentState = 368; break; }
				if (set[23, t.kind]) {
					goto case 367;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 369: {
				if (t == null) { currentState = 369; break; }
				Expect(104, t); // "Dim"
				currentState = 375;
				break;
			}
			case 370: {
				if (t == null) { currentState = 370; break; }
				Expect(202, t); // "Static"
				currentState = 375;
				break;
			}
			case 371: { // start of VariableDeclarationStatement
				if (t == null) { currentState = 371; break; }
				if (t.kind == 104) {
					goto case 369;
				} else {
					goto case 372;
				}
			}
			case 372: {
				if (t == null) { currentState = 372; break; }
				if (t.kind == 202) {
					goto case 370;
				} else {
					goto case 374;
				}
			}
			case 373: {
				if (t == null) { currentState = 373; break; }
				Expect(87, t); // "Const"
				currentState = 375;
				break;
			}
			case 374: {
				if (t == null) { currentState = 374; break; }
				if (t.kind == 87) {
					goto case 373;
				} else {
					Error(t);
					goto case 375;
				}
			}
			case 375: {
				stateStack.Push(377);
				goto case 459; // Identifier
			}
			case 376: {
				if (t == null) { currentState = 376; break; }
				Expect(32, t); // "?"
				currentState = 382;
				break;
			}
			case 377: {
				if (t == null) { currentState = 377; break; }
				if (t.kind == 32) {
					goto case 376;
				} else {
					goto case 382;
				}
			}
			case 378: {
				if (t == null) { currentState = 378; break; }
				Expect(36, t); // "("
				currentState = 380;
				break;
			}
			case 379: {
				if (t == null) { currentState = 379; break; }
				Expect(23, t); // ","
				currentState = 380;
				break;
			}
			case 380: {
				if (t == null) { currentState = 380; break; }
				if (t.kind == 23) {
					goto case 379;
				} else {
					goto case 381;
				}
			}
			case 381: {
				if (t == null) { currentState = 381; break; }
				Expect(37, t); // ")"
				currentState = 392;
				break;
			}
			case 382: {
				if (t == null) { currentState = 382; break; }
				if (t.kind == 36) {
					goto case 378;
				} else {
					goto case 392;
				}
			}
			case 383: {
				if (t == null) { currentState = 383; break; }
				Expect(23, t); // ","
				currentState = 384;
				break;
			}
			case 384: {
				stateStack.Push(386);
				goto case 459; // Identifier
			}
			case 385: {
				if (t == null) { currentState = 385; break; }
				Expect(32, t); // "?"
				currentState = 391;
				break;
			}
			case 386: {
				if (t == null) { currentState = 386; break; }
				if (t.kind == 32) {
					goto case 385;
				} else {
					goto case 391;
				}
			}
			case 387: {
				if (t == null) { currentState = 387; break; }
				Expect(36, t); // "("
				currentState = 389;
				break;
			}
			case 388: {
				if (t == null) { currentState = 388; break; }
				Expect(23, t); // ","
				currentState = 389;
				break;
			}
			case 389: {
				if (t == null) { currentState = 389; break; }
				if (t.kind == 23) {
					goto case 388;
				} else {
					goto case 390;
				}
			}
			case 390: {
				if (t == null) { currentState = 390; break; }
				Expect(37, t); // ")"
				currentState = 392;
				break;
			}
			case 391: {
				if (t == null) { currentState = 391; break; }
				if (t.kind == 36) {
					goto case 387;
				} else {
					goto case 392;
				}
			}
			case 392: {
				if (t == null) { currentState = 392; break; }
				if (t.kind == 23) {
					goto case 383;
				} else {
					goto case 397;
				}
			}
			case 393: {
				if (t == null) { currentState = 393; break; }
				Expect(62, t); // "As"
				currentState = 395;
				break;
			}
			case 394: {
				if (t == null) { currentState = 394; break; }
				Expect(161, t); // "New"
				currentState = 396;
				break;
			}
			case 395: {
				if (t == null) { currentState = 395; break; }
				if (t.kind == 161) {
					goto case 394;
				} else {
					goto case 396;
				}
			}
			case 396: {
				stateStack.Push(400);
				goto case 310; // TypeName
			}
			case 397: {
				if (t == null) { currentState = 397; break; }
				if (t.kind == 62) {
					goto case 393;
				} else {
					goto case 400;
				}
			}
			case 398: {
				if (t == null) { currentState = 398; break; }
				Expect(21, t); // "="
				currentState = 399;
				break;
			}
			case 399: {
				goto case 222; // Expression
			}
			case 400: {
				if (t == null) { currentState = 400; break; }
				if (t.kind == 21) {
					goto case 398;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 401: {
				if (t == null) { currentState = 401; break; }
				Expect(231, t); // "With"
				currentState = 405;
				break;
			}
			case 402: {
				if (t == null) { currentState = 402; break; }
				Expect(209, t); // "SyncLock"
				currentState = 405;
				break;
			}
			case 403: { // start of WithOrLockStatement
				if (t == null) { currentState = 403; break; }
				if (t.kind == 231) {
					goto case 401;
				} else {
					goto case 404;
				}
			}
			case 404: {
				if (t == null) { currentState = 404; break; }
				if (t.kind == 209) {
					goto case 402;
				} else {
					Error(t);
					goto case 405;
				}
			}
			case 405: {
				stateStack.Push(406);
				goto case 222; // Expression
			}
			case 406: {
				stateStack.Push(407);
				goto case 13; // StatementTerminator
			}
			case 407: {
				stateStack.Push(408);
				goto case 215; // Block
			}
			case 408: {
				if (t == null) { currentState = 408; break; }
				Expect(112, t); // "End"
				currentState = 411;
				break;
			}
			case 409: {
				if (t == null) { currentState = 409; break; }
				Expect(231, t); // "With"
				currentState = stateStack.Pop();
				break;
			}
			case 410: {
				if (t == null) { currentState = 410; break; }
				Expect(209, t); // "SyncLock"
				currentState = stateStack.Pop();
				break;
			}
			case 411: {
				if (t == null) { currentState = 411; break; }
				if (t.kind == 231) {
					goto case 409;
				} else {
					goto case 412;
				}
			}
			case 412: {
				if (t == null) { currentState = 412; break; }
				if (t.kind == 209) {
					goto case 410;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 413: {
				if (t == null) { currentState = 413; break; }
				Expect(55, t); // "AddHandler"
				currentState = 417;
				break;
			}
			case 414: {
				if (t == null) { currentState = 414; break; }
				Expect(191, t); // "RemoveHandler"
				currentState = 417;
				break;
			}
			case 415: { // start of AddOrRemoveHandlerStatement
				if (t == null) { currentState = 415; break; }
				if (t.kind == 55) {
					goto case 413;
				} else {
					goto case 416;
				}
			}
			case 416: {
				if (t == null) { currentState = 416; break; }
				if (t.kind == 191) {
					goto case 414;
				} else {
					Error(t);
					goto case 417;
				}
			}
			case 417: {
				stateStack.Push(418);
				goto case 222; // Expression
			}
			case 418: {
				if (t == null) { currentState = 418; break; }
				Expect(23, t); // ","
				currentState = 419;
				break;
			}
			case 419: {
				goto case 222; // Expression
			}
			case 420: { // start of RaiseEventStatement
				if (t == null) { currentState = 420; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 421;
				break;
			}
			case 421: {
				stateStack.Push(426);
				goto case 332; // IdentifierOrKeyword
			}
			case 422: {
				if (t == null) { currentState = 422; break; }
				Expect(36, t); // "("
				currentState = 424;
				break;
			}
			case 423: {
				stateStack.Push(425);
				goto case 451; // ArgumentList
			}
			case 424: {
				if (t == null) { currentState = 424; break; }
				if (set[17, t.kind]) {
					goto case 423;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (t == null) { currentState = 425; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 426: {
				if (t == null) { currentState = 426; break; }
				if (t.kind == 36) {
					goto case 422;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 427: { // start of InvocationStatement
				Console.WriteLine("InvocationStatement");
				goto case 429;
			}
			case 428: {
				if (t == null) { currentState = 428; break; }
				Expect(72, t); // "Call"
				currentState = 430;
				break;
			}
			case 429: {
				if (t == null) { currentState = 429; break; }
				if (t.kind == 72) {
					goto case 428;
				} else {
					goto case 430;
				}
			}
			case 430: {
				stateStack.Push(450);
				goto case 222; // Expression
			}
			case 431: {
				if (t == null) { currentState = 431; break; }
				Expect(21, t); // "="
				currentState = 449;
				break;
			}
			case 432: {
				if (t == null) { currentState = 432; break; }
				Expect(46, t); // "^="
				currentState = 449;
				break;
			}
			case 433: {
				if (t == null) { currentState = 433; break; }
				if (t.kind == 21) {
					goto case 431;
				} else {
					goto case 434;
				}
			}
			case 434: {
				if (t == null) { currentState = 434; break; }
				if (t.kind == 46) {
					goto case 432;
				} else {
					goto case 436;
				}
			}
			case 435: {
				if (t == null) { currentState = 435; break; }
				Expect(48, t); // "*="
				currentState = 449;
				break;
			}
			case 436: {
				if (t == null) { currentState = 436; break; }
				if (t.kind == 48) {
					goto case 435;
				} else {
					goto case 438;
				}
			}
			case 437: {
				if (t == null) { currentState = 437; break; }
				Expect(49, t); // "/="
				currentState = 449;
				break;
			}
			case 438: {
				if (t == null) { currentState = 438; break; }
				if (t.kind == 49) {
					goto case 437;
				} else {
					goto case 440;
				}
			}
			case 439: {
				if (t == null) { currentState = 439; break; }
				Expect(50, t); // "\\="
				currentState = 449;
				break;
			}
			case 440: {
				if (t == null) { currentState = 440; break; }
				if (t.kind == 50) {
					goto case 439;
				} else {
					goto case 442;
				}
			}
			case 441: {
				if (t == null) { currentState = 441; break; }
				Expect(45, t); // "+="
				currentState = 449;
				break;
			}
			case 442: {
				if (t == null) { currentState = 442; break; }
				if (t.kind == 45) {
					goto case 441;
				} else {
					goto case 444;
				}
			}
			case 443: {
				if (t == null) { currentState = 443; break; }
				Expect(47, t); // "-="
				currentState = 449;
				break;
			}
			case 444: {
				if (t == null) { currentState = 444; break; }
				if (t.kind == 47) {
					goto case 443;
				} else {
					goto case 446;
				}
			}
			case 445: {
				if (t == null) { currentState = 445; break; }
				Expect(53, t); // "&="
				currentState = 449;
				break;
			}
			case 446: {
				if (t == null) { currentState = 446; break; }
				if (t.kind == 53) {
					goto case 445;
				} else {
					goto case 448;
				}
			}
			case 447: {
				if (t == null) { currentState = 447; break; }
				Expect(41, t); // ">="
				currentState = 449;
				break;
			}
			case 448: {
				if (t == null) { currentState = 448; break; }
				if (t.kind == 41) {
					goto case 447;
				} else {
					Error(t);
					goto case 449;
				}
			}
			case 449: {
				goto case 222; // Expression
			}
			case 450: {
				if (t == null) { currentState = 450; break; }
				if (set[24, t.kind]) {
					goto case 433;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 451: { // start of ArgumentList
				Console.WriteLine("ArgumentList");
				goto case 452;
			}
			case 452: {
				stateStack.Push(458);
				goto case 222; // Expression
			}
			case 453: {
				if (t == null) { currentState = 453; break; }
				Expect(23, t); // ","
				currentState = 454;
				break;
			}
			case 454: {
				stateStack.Push(457);
				goto case 222; // Expression
			}
			case 455: {
				if (t == null) { currentState = 455; break; }
				Expect(54, t); // ":="
				currentState = 456;
				break;
			}
			case 456: {
				stateStack.Push(458);
				goto case 222; // Expression
			}
			case 457: {
				if (t == null) { currentState = 457; break; }
				if (t.kind == 54) {
					goto case 455;
				} else {
					goto case 458;
				}
			}
			case 458: {
				if (t == null) { currentState = 458; break; }
				if (t.kind == 23) {
					goto case 453;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 459: { // start of Identifier
				PushContext(Context.IdentifierExpected, t);
				goto case 462;
			}
			case 460: {
				stateStack.Push(464);
				goto case 467; // IdentifierForFieldDeclaration
			}
			case 461: {
				if (t == null) { currentState = 461; break; }
				Expect(97, t); // "Custom"
				currentState = 464;
				break;
			}
			case 462: {
				if (t == null) { currentState = 462; break; }
				if (set[25, t.kind]) {
					goto case 460;
				} else {
					goto case 463;
				}
			}
			case 463: {
				if (t == null) { currentState = 463; break; }
				if (t.kind == 97) {
					goto case 461;
				} else {
					Error(t);
					goto case 464;
				}
			}
			case 464: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 465: {
				if (t == null) { currentState = 465; break; }
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 466: {
				if (t == null) { currentState = 466; break; }
				Expect(57, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 467: { // start of IdentifierForFieldDeclaration
				if (t == null) { currentState = 467; break; }
				if (t.kind == 2) {
					goto case 465;
				} else {
					goto case 468;
				}
			}
			case 468: {
				if (t == null) { currentState = 468; break; }
				if (t.kind == 57) {
					goto case 466;
				} else {
					goto case 470;
				}
			}
			case 469: {
				if (t == null) { currentState = 469; break; }
				Expect(61, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 470: {
				if (t == null) { currentState = 470; break; }
				if (t.kind == 61) {
					goto case 469;
				} else {
					goto case 472;
				}
			}
			case 471: {
				if (t == null) { currentState = 471; break; }
				Expect(63, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 472: {
				if (t == null) { currentState = 472; break; }
				if (t.kind == 63) {
					goto case 471;
				} else {
					goto case 474;
				}
			}
			case 473: {
				if (t == null) { currentState = 473; break; }
				Expect(64, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 474: {
				if (t == null) { currentState = 474; break; }
				if (t.kind == 64) {
					goto case 473;
				} else {
					goto case 476;
				}
			}
			case 475: {
				if (t == null) { currentState = 475; break; }
				Expect(65, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 476: {
				if (t == null) { currentState = 476; break; }
				if (t.kind == 65) {
					goto case 475;
				} else {
					goto case 478;
				}
			}
			case 477: {
				if (t == null) { currentState = 477; break; }
				Expect(66, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 478: {
				if (t == null) { currentState = 478; break; }
				if (t.kind == 66) {
					goto case 477;
				} else {
					goto case 480;
				}
			}
			case 479: {
				if (t == null) { currentState = 479; break; }
				Expect(69, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 480: {
				if (t == null) { currentState = 480; break; }
				if (t.kind == 69) {
					goto case 479;
				} else {
					goto case 482;
				}
			}
			case 481: {
				if (t == null) { currentState = 481; break; }
				Expect(86, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 482: {
				if (t == null) { currentState = 482; break; }
				if (t.kind == 86) {
					goto case 481;
				} else {
					goto case 484;
				}
			}
			case 483: {
				if (t == null) { currentState = 483; break; }
				Expect(103, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 484: {
				if (t == null) { currentState = 484; break; }
				if (t.kind == 103) {
					goto case 483;
				} else {
					goto case 486;
				}
			}
			case 485: {
				if (t == null) { currentState = 485; break; }
				Expect(106, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 486: {
				if (t == null) { currentState = 486; break; }
				if (t.kind == 106) {
					goto case 485;
				} else {
					goto case 488;
				}
			}
			case 487: {
				if (t == null) { currentState = 487; break; }
				Expect(115, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 488: {
				if (t == null) { currentState = 488; break; }
				if (t.kind == 115) {
					goto case 487;
				} else {
					goto case 490;
				}
			}
			case 489: {
				if (t == null) { currentState = 489; break; }
				Expect(120, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 490: {
				if (t == null) { currentState = 490; break; }
				if (t.kind == 120) {
					goto case 489;
				} else {
					goto case 492;
				}
			}
			case 491: {
				if (t == null) { currentState = 491; break; }
				Expect(125, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 492: {
				if (t == null) { currentState = 492; break; }
				if (t.kind == 125) {
					goto case 491;
				} else {
					goto case 494;
				}
			}
			case 493: {
				if (t == null) { currentState = 493; break; }
				Expect(132, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 494: {
				if (t == null) { currentState = 494; break; }
				if (t.kind == 132) {
					goto case 493;
				} else {
					goto case 496;
				}
			}
			case 495: {
				if (t == null) { currentState = 495; break; }
				Expect(138, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 496: {
				if (t == null) { currentState = 496; break; }
				if (t.kind == 138) {
					goto case 495;
				} else {
					goto case 498;
				}
			}
			case 497: {
				if (t == null) { currentState = 497; break; }
				Expect(142, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 498: {
				if (t == null) { currentState = 498; break; }
				if (t.kind == 142) {
					goto case 497;
				} else {
					goto case 500;
				}
			}
			case 499: {
				if (t == null) { currentState = 499; break; }
				Expect(145, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 500: {
				if (t == null) { currentState = 500; break; }
				if (t.kind == 145) {
					goto case 499;
				} else {
					goto case 502;
				}
			}
			case 501: {
				if (t == null) { currentState = 501; break; }
				Expect(146, t); // "Key"
				currentState = stateStack.Pop();
				break;
			}
			case 502: {
				if (t == null) { currentState = 502; break; }
				if (t.kind == 146) {
					goto case 501;
				} else {
					goto case 504;
				}
			}
			case 503: {
				if (t == null) { currentState = 503; break; }
				Expect(169, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 504: {
				if (t == null) { currentState = 504; break; }
				if (t.kind == 169) {
					goto case 503;
				} else {
					goto case 506;
				}
			}
			case 505: {
				if (t == null) { currentState = 505; break; }
				Expect(175, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 506: {
				if (t == null) { currentState = 506; break; }
				if (t.kind == 175) {
					goto case 505;
				} else {
					goto case 508;
				}
			}
			case 507: {
				if (t == null) { currentState = 507; break; }
				Expect(182, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 508: {
				if (t == null) { currentState = 508; break; }
				if (t.kind == 182) {
					goto case 507;
				} else {
					goto case 510;
				}
			}
			case 509: {
				if (t == null) { currentState = 509; break; }
				Expect(201, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 510: {
				if (t == null) { currentState = 510; break; }
				if (t.kind == 201) {
					goto case 509;
				} else {
					goto case 512;
				}
			}
			case 511: {
				if (t == null) { currentState = 511; break; }
				Expect(210, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 512: {
				if (t == null) { currentState = 512; break; }
				if (t.kind == 210) {
					goto case 511;
				} else {
					goto case 514;
				}
			}
			case 513: {
				if (t == null) { currentState = 513; break; }
				Expect(211, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 514: {
				if (t == null) { currentState = 514; break; }
				if (t.kind == 211) {
					goto case 513;
				} else {
					goto case 516;
				}
			}
			case 515: {
				if (t == null) { currentState = 515; break; }
				Expect(221, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 516: {
				if (t == null) { currentState = 516; break; }
				if (t.kind == 221) {
					goto case 515;
				} else {
					goto case 518;
				}
			}
			case 517: {
				if (t == null) { currentState = 517; break; }
				Expect(222, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 518: {
				if (t == null) { currentState = 518; break; }
				if (t.kind == 222) {
					goto case 517;
				} else {
					goto case 520;
				}
			}
			case 519: {
				if (t == null) { currentState = 519; break; }
				Expect(228, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 520: {
				if (t == null) { currentState = 520; break; }
				if (t.kind == 228) {
					goto case 519;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 521: {
				if (t == null) { currentState = 521; break; }
				Expect(186, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 522: {
				if (t == null) { currentState = 522; break; }
				Expect(124, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 523: { // start of AccessModifier
				if (t == null) { currentState = 523; break; }
				if (t.kind == 186) {
					goto case 521;
				} else {
					goto case 524;
				}
			}
			case 524: {
				if (t == null) { currentState = 524; break; }
				if (t.kind == 124) {
					goto case 522;
				} else {
					goto case 526;
				}
			}
			case 525: {
				if (t == null) { currentState = 525; break; }
				Expect(185, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 526: {
				if (t == null) { currentState = 526; break; }
				if (t.kind == 185) {
					goto case 525;
				} else {
					goto case 528;
				}
			}
			case 527: {
				if (t == null) { currentState = 527; break; }
				Expect(183, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 528: {
				if (t == null) { currentState = 528; break; }
				if (t.kind == 183) {
					goto case 527;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 529: {
				goto case 523; // AccessModifier
			}
			case 530: {
				if (t == null) { currentState = 530; break; }
				Expect(197, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 531: { // start of TypeModifier
				if (t == null) { currentState = 531; break; }
				if (set[26, t.kind]) {
					goto case 529;
				} else {
					goto case 532;
				}
			}
			case 532: {
				if (t == null) { currentState = 532; break; }
				if (t.kind == 197) {
					goto case 530;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 533: {
				goto case 523; // AccessModifier
			}
			case 534: {
				if (t == null) { currentState = 534; break; }
				Expect(197, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 535: { // start of MemberModifier
				if (t == null) { currentState = 535; break; }
				if (set[26, t.kind]) {
					goto case 533;
				} else {
					goto case 536;
				}
			}
			case 536: {
				if (t == null) { currentState = 536; break; }
				if (t.kind == 197) {
					goto case 534;
				} else {
					goto case 538;
				}
			}
			case 537: {
				if (t == null) { currentState = 537; break; }
				Expect(198, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 538: {
				if (t == null) { currentState = 538; break; }
				if (t.kind == 198) {
					goto case 537;
				} else {
					goto case 540;
				}
			}
			case 539: {
				if (t == null) { currentState = 539; break; }
				Expect(178, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 540: {
				if (t == null) { currentState = 540; break; }
				if (t.kind == 178) {
					goto case 539;
				} else {
					goto case 542;
				}
			}
			case 541: {
				if (t == null) { currentState = 541; break; }
				Expect(166, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 542: {
				if (t == null) { currentState = 542; break; }
				if (t.kind == 166) {
					goto case 541;
				} else {
					goto case 544;
				}
			}
			case 543: {
				if (t == null) { currentState = 543; break; }
				Expect(179, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 544: {
				if (t == null) { currentState = 544; break; }
				if (t.kind == 179) {
					goto case 543;
				} else {
					goto case 546;
				}
			}
			case 545: {
				if (t == null) { currentState = 545; break; }
				Expect(177, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 546: {
				if (t == null) { currentState = 546; break; }
				if (t.kind == 177) {
					goto case 545;
				} else {
					goto case 548;
				}
			}
			case 547: {
				if (t == null) { currentState = 547; break; }
				Expect(181, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 548: {
				if (t == null) { currentState = 548; break; }
				if (t.kind == 181) {
					goto case 547;
				} else {
					goto case 550;
				}
			}
			case 549: {
				if (t == null) { currentState = 549; break; }
				Expect(232, t); // "WithEvents"
				currentState = stateStack.Pop();
				break;
			}
			case 550: {
				if (t == null) { currentState = 550; break; }
				if (t.kind == 232) {
					goto case 549;
				} else {
					goto case 552;
				}
			}
			case 551: {
				if (t == null) { currentState = 551; break; }
				Expect(156, t); // "MustOverride"
				currentState = stateStack.Pop();
				break;
			}
			case 552: {
				if (t == null) { currentState = 552; break; }
				if (t.kind == 156) {
					goto case 551;
				} else {
					goto case 554;
				}
			}
			case 553: {
				if (t == null) { currentState = 553; break; }
				Expect(230, t); // "Widening"
				currentState = stateStack.Pop();
				break;
			}
			case 554: {
				if (t == null) { currentState = 554; break; }
				if (t.kind == 230) {
					goto case 553;
				} else {
					goto case 556;
				}
			}
			case 555: {
				if (t == null) { currentState = 555; break; }
				Expect(160, t); // "Narrowing"
				currentState = stateStack.Pop();
				break;
			}
			case 556: {
				if (t == null) { currentState = 556; break; }
				if (t.kind == 160) {
					goto case 555;
				} else {
					goto case 558;
				}
			}
			case 557: {
				if (t == null) { currentState = 557; break; }
				Expect(104, t); // "Dim"
				currentState = stateStack.Pop();
				break;
			}
			case 558: {
				if (t == null) { currentState = 558; break; }
				if (t.kind == 104) {
					goto case 557;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 559: {
				if (t == null) { currentState = 559; break; }
				Expect(71, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 560: {
				if (t == null) { currentState = 560; break; }
				Expect(68, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 561: { // start of ParameterModifier
				if (t == null) { currentState = 561; break; }
				if (t.kind == 71) {
					goto case 559;
				} else {
					goto case 562;
				}
			}
			case 562: {
				if (t == null) { currentState = 562; break; }
				if (t.kind == 68) {
					goto case 560;
				} else {
					goto case 564;
				}
			}
			case 563: {
				if (t == null) { currentState = 563; break; }
				Expect(173, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 564: {
				if (t == null) { currentState = 564; break; }
				if (t.kind == 173) {
					goto case 563;
				} else {
					goto case 566;
				}
			}
			case 565: {
				if (t == null) { currentState = 565; break; }
				Expect(180, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 566: {
				if (t == null) { currentState = 566; break; }
				if (t.kind == 180) {
					goto case 565;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
		}

	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,T,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};

} // end Parser


}