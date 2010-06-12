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
				if (t == null) break;
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
				if (t == null) break;
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
				if (t == null) break;
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
				if (t == null) break;
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
				if (t == null) break;
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 12: {
				if (t == null) break;
				Expect(22, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 13: { // start of StatementTerminator
				if (t == null) break;
				if (t.kind == 1) {
					goto case 11;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (t == null) break;
				if (t.kind == 22) {
					goto case 12;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 15: { // start of OptionStatement
				if (t == null) break;
				Expect(172, t); // "Option"
				currentState = 17;
				break;
			}
			case 16: {
				if (t == null) break;
				currentState = 17;
				break;
			}
			case 17: {
				if (t == null) break;
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
				if (t == null) break;
				Expect(136, t); // "Imports"
				currentState = 20;
				break;
			}
			case 20: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 22;
			}
			case 21: {
				if (t == null) break;
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) break;
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
				if (t == null) break;
				Expect(39, t); // "<"
				currentState = 25;
				break;
			}
			case 25: {
				PushContext(Context.Attribute, t);
				goto case 27;
			}
			case 26: {
				if (t == null) break;
				currentState = 27;
				break;
			}
			case 27: {
				if (t == null) break;
				if (set[2, t.kind]) {
					goto case 26;
				} else {
					goto case 28;
				}
			}
			case 28: {
				if (t == null) break;
				Expect(38, t); // ">"
				currentState = 29;
				break;
			}
			case 29: {
				PopContext();
				goto case 31;
			}
			case 30: {
				if (t == null) break;
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 31: {
				if (t == null) break;
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
				if (t == null) break;
				if (t.kind == 159) {
					goto case 32;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (t == null) break;
				if (set[3, t.kind]) {
					goto case 33;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: { // start of NamespaceDeclaration
				if (t == null) break;
				Expect(159, t); // "Namespace"
				currentState = 38;
				break;
			}
			case 37: {
				if (t == null) break;
				currentState = 38;
				break;
			}
			case 38: {
				if (t == null) break;
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
				if (t == null) break;
				if (set[4, t.kind]) {
					goto case 40;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				if (t == null) break;
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
				if (t == null) break;
				if (t.kind == 39) {
					goto case 45;
				} else {
					goto case 48;
				}
			}
			case 47: {
				stateStack.Push(48);
				goto case 514; // TypeModifier
			}
			case 48: {
				if (t == null) break;
				if (set[5, t.kind]) {
					goto case 47;
				} else {
					goto case 51;
				}
			}
			case 49: {
				if (t == null) break;
				Expect(154, t); // "Module"
				currentState = 54;
				break;
			}
			case 50: {
				if (t == null) break;
				Expect(83, t); // "Class"
				currentState = 54;
				break;
			}
			case 51: {
				if (t == null) break;
				if (t.kind == 154) {
					goto case 49;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (t == null) break;
				if (t.kind == 83) {
					goto case 50;
				} else {
					Error(t);
					goto case 54;
				}
			}
			case 53: {
				if (t == null) break;
				currentState = 54;
				break;
			}
			case 54: {
				if (t == null) break;
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
				if (t == null) break;
				if (set[6, t.kind]) {
					goto case 57;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 62;
				break;
			}
			case 60: {
				if (t == null) break;
				Expect(154, t); // "Module"
				currentState = 64;
				break;
			}
			case 61: {
				if (t == null) break;
				Expect(83, t); // "Class"
				currentState = 64;
				break;
			}
			case 62: {
				if (t == null) break;
				if (t.kind == 154) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t == null) break;
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
				if (t == null) break;
				if (t.kind == 39) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 518; // MemberModifier
			}
			case 70: {
				if (t == null) break;
				if (set[7, t.kind]) {
					goto case 69;
				} else {
					goto case 73;
				}
			}
			case 71: {
				stateStack.Push(81);
				goto case 194; // MemberVariableOrConstantDeclaration
			}
			case 72: {
				stateStack.Push(81);
				goto case 84; // SubOrFunctionDeclaration
			}
			case 73: {
				if (t == null) break;
				if (set[8, t.kind]) {
					goto case 71;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t == null) break;
				if (t.kind == 126 || t.kind == 208) {
					goto case 72;
				} else {
					goto case 76;
				}
			}
			case 75: {
				stateStack.Push(81);
				goto case 104; // ExternalMemberDeclaration
			}
			case 76: {
				if (t == null) break;
				if (t.kind == 100) {
					goto case 75;
				} else {
					goto case 78;
				}
			}
			case 77: {
				stateStack.Push(81);
				goto case 129; // EventMemberDeclaration
			}
			case 78: {
				if (t == null) break;
				if (t.kind == 97 || t.kind == 118) {
					goto case 77;
				} else {
					goto case 80;
				}
			}
			case 79: {
				stateStack.Push(81);
				goto case 178; // OperatorDeclaration
			}
			case 80: {
				if (t == null) break;
				if (t.kind == 171) {
					goto case 79;
				} else {
					Error(t);
					goto case 81;
				}
			}
			case 81: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 82: {
				if (t == null) break;
				Expect(208, t); // "Sub"
				currentState = 86;
				break;
			}
			case 83: {
				if (t == null) break;
				Expect(126, t); // "Function"
				currentState = 86;
				break;
			}
			case 84: { // start of SubOrFunctionDeclaration
				if (t == null) break;
				if (t.kind == 208) {
					goto case 82;
				} else {
					goto case 85;
				}
			}
			case 85: {
				if (t == null) break;
				if (t.kind == 126) {
					goto case 83;
				} else {
					Error(t);
					goto case 86;
				}
			}
			case 86: {
				PushContext(Context.IdentifierExpected, t);
				goto case 87;
			}
			case 87: {
				if (t == null) break;
				currentState = 88;
				break;
			}
			case 88: {
				PopContext();
				goto case 93;
			}
			case 89: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 91;
				break;
			}
			case 90: {
				stateStack.Push(92);
				goto case 203; // ParameterList
			}
			case 91: {
				if (t == null) break;
				if (set[9, t.kind]) {
					goto case 90;
				} else {
					goto case 92;
				}
			}
			case 92: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 96;
				break;
			}
			case 93: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 89;
				} else {
					goto case 96;
				}
			}
			case 94: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 95;
				break;
			}
			case 95: {
				stateStack.Push(97);
				goto case 287; // TypeName
			}
			case 96: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 94;
				} else {
					goto case 97;
				}
			}
			case 97: {
				stateStack.Push(98);
				goto case 218; // Block
			}
			case 98: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 101;
				break;
			}
			case 99: {
				if (t == null) break;
				Expect(208, t); // "Sub"
				currentState = 103;
				break;
			}
			case 100: {
				if (t == null) break;
				Expect(126, t); // "Function"
				currentState = 103;
				break;
			}
			case 101: {
				if (t == null) break;
				if (t.kind == 208) {
					goto case 99;
				} else {
					goto case 102;
				}
			}
			case 102: {
				if (t == null) break;
				if (t.kind == 126) {
					goto case 100;
				} else {
					Error(t);
					goto case 103;
				}
			}
			case 103: {
				goto case 13; // StatementTerminator
			}
			case 104: { // start of ExternalMemberDeclaration
				if (t == null) break;
				Expect(100, t); // "Declare"
				currentState = 111;
				break;
			}
			case 105: {
				if (t == null) break;
				Expect(61, t); // "Ansi"
				currentState = 114;
				break;
			}
			case 106: {
				if (t == null) break;
				Expect(221, t); // "Unicode"
				currentState = 114;
				break;
			}
			case 107: {
				if (t == null) break;
				if (t.kind == 61) {
					goto case 105;
				} else {
					goto case 108;
				}
			}
			case 108: {
				if (t == null) break;
				if (t.kind == 221) {
					goto case 106;
				} else {
					goto case 110;
				}
			}
			case 109: {
				if (t == null) break;
				Expect(65, t); // "Auto"
				currentState = 114;
				break;
			}
			case 110: {
				if (t == null) break;
				if (t.kind == 65) {
					goto case 109;
				} else {
					Error(t);
					goto case 114;
				}
			}
			case 111: {
				if (t == null) break;
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					goto case 107;
				} else {
					goto case 114;
				}
			}
			case 112: {
				if (t == null) break;
				Expect(208, t); // "Sub"
				currentState = 116;
				break;
			}
			case 113: {
				if (t == null) break;
				Expect(126, t); // "Function"
				currentState = 116;
				break;
			}
			case 114: {
				if (t == null) break;
				if (t.kind == 208) {
					goto case 112;
				} else {
					goto case 115;
				}
			}
			case 115: {
				if (t == null) break;
				if (t.kind == 126) {
					goto case 113;
				} else {
					Error(t);
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(117);
				goto case 442; // Identifier
			}
			case 117: {
				if (t == null) break;
				Expect(148, t); // "Lib"
				currentState = 118;
				break;
			}
			case 118: {
				if (t == null) break;
				Expect(3, t); // LiteralString
				currentState = 121;
				break;
			}
			case 119: {
				if (t == null) break;
				Expect(58, t); // "Alias"
				currentState = 120;
				break;
			}
			case 120: {
				if (t == null) break;
				Expect(3, t); // LiteralString
				currentState = 126;
				break;
			}
			case 121: {
				if (t == null) break;
				if (t.kind == 58) {
					goto case 119;
				} else {
					goto case 126;
				}
			}
			case 122: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 124;
				break;
			}
			case 123: {
				stateStack.Push(125);
				goto case 203; // ParameterList
			}
			case 124: {
				if (t == null) break;
				if (set[9, t.kind]) {
					goto case 123;
				} else {
					goto case 125;
				}
			}
			case 125: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 127;
				break;
			}
			case 126: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 122;
				} else {
					goto case 127;
				}
			}
			case 127: {
				goto case 13; // StatementTerminator
			}
			case 128: {
				if (t == null) break;
				Expect(97, t); // "Custom"
				currentState = 130;
				break;
			}
			case 129: { // start of EventMemberDeclaration
				if (t == null) break;
				if (t.kind == 97) {
					goto case 128;
				} else {
					goto case 130;
				}
			}
			case 130: {
				if (t == null) break;
				Expect(118, t); // "Event"
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(139);
				goto case 442; // Identifier
			}
			case 132: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 133;
				break;
			}
			case 133: {
				stateStack.Push(150);
				goto case 287; // TypeName
			}
			case 134: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 136;
				break;
			}
			case 135: {
				stateStack.Push(137);
				goto case 203; // ParameterList
			}
			case 136: {
				if (t == null) break;
				if (set[9, t.kind]) {
					goto case 135;
				} else {
					goto case 137;
				}
			}
			case 137: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 150;
				break;
			}
			case 138: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 134;
				} else {
					goto case 150;
				}
			}
			case 139: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 132;
				} else {
					goto case 140;
				}
			}
			case 140: {
				if (t == null) break;
				if (set[10, t.kind]) {
					goto case 138;
				} else {
					Error(t);
					goto case 150;
				}
			}
			case 141: {
				if (t == null) break;
				Expect(135, t); // "Implements"
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(143);
				goto case 287; // TypeName
			}
			case 143: {
				if (t == null) break;
				Expect(27, t); // "."
				currentState = 144;
				break;
			}
			case 144: {
				stateStack.Push(149);
				goto case 309; // IdentifierOrKeyword
			}
			case 145: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 146;
				break;
			}
			case 146: {
				stateStack.Push(147);
				goto case 287; // TypeName
			}
			case 147: {
				if (t == null) break;
				Expect(27, t); // "."
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 309; // IdentifierOrKeyword
			}
			case 149: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 145;
				} else {
					goto case 151;
				}
			}
			case 150: {
				if (t == null) break;
				if (t.kind == 135) {
					goto case 141;
				} else {
					goto case 151;
				}
			}
			case 151: {
				stateStack.Push(177);
				goto case 13; // StatementTerminator
			}
			case 152: {
				stateStack.Push(153);
				goto case 24; // AttributeBlock
			}
			case 153: {
				if (t == null) break;
				if (t.kind == 39) {
					goto case 152;
				} else {
					goto case 156;
				}
			}
			case 154: {
				if (t == null) break;
				Expect(55, t); // "AddHandler"
				currentState = 160;
				break;
			}
			case 155: {
				if (t == null) break;
				Expect(191, t); // "RemoveHandler"
				currentState = 160;
				break;
			}
			case 156: {
				if (t == null) break;
				if (t.kind == 55) {
					goto case 154;
				} else {
					goto case 157;
				}
			}
			case 157: {
				if (t == null) break;
				if (t.kind == 191) {
					goto case 155;
				} else {
					goto case 159;
				}
			}
			case 158: {
				if (t == null) break;
				Expect(187, t); // "RaiseEvent"
				currentState = 160;
				break;
			}
			case 159: {
				if (t == null) break;
				if (t.kind == 187) {
					goto case 158;
				} else {
					Error(t);
					goto case 160;
				}
			}
			case 160: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 161;
				break;
			}
			case 161: {
				stateStack.Push(162);
				goto case 203; // ParameterList
			}
			case 162: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 163;
				break;
			}
			case 163: {
				if (t == null) break;
				Expect(1, t); // EOL
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 218; // Block
			}
			case 165: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 168;
				break;
			}
			case 166: {
				if (t == null) break;
				Expect(55, t); // "AddHandler"
				currentState = 172;
				break;
			}
			case 167: {
				if (t == null) break;
				Expect(191, t); // "RemoveHandler"
				currentState = 172;
				break;
			}
			case 168: {
				if (t == null) break;
				if (t.kind == 55) {
					goto case 166;
				} else {
					goto case 169;
				}
			}
			case 169: {
				if (t == null) break;
				if (t.kind == 191) {
					goto case 167;
				} else {
					goto case 171;
				}
			}
			case 170: {
				if (t == null) break;
				Expect(187, t); // "RaiseEvent"
				currentState = 172;
				break;
			}
			case 171: {
				if (t == null) break;
				if (t.kind == 187) {
					goto case 170;
				} else {
					Error(t);
					goto case 172;
				}
			}
			case 172: {
				stateStack.Push(173);
				goto case 13; // StatementTerminator
			}
			case 173: {
				if (t == null) break;
				if (set[11, t.kind]) {
					goto case 153;
				} else {
					goto case 174;
				}
			}
			case 174: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 175;
				break;
			}
			case 175: {
				if (t == null) break;
				Expect(118, t); // "Event"
				currentState = 176;
				break;
			}
			case 176: {
				goto case 13; // StatementTerminator
			}
			case 177: {
				if (t == null) break;
				if (set[12, t.kind]) {
					goto case 173;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 178: { // start of OperatorDeclaration
				if (t == null) break;
				Expect(171, t); // "Operator"
				currentState = 179;
				break;
			}
			case 179: {
				if (t == null) break;
				currentState = 180;
				break;
			}
			case 180: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 181;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 203; // ParameterList
			}
			case 182: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 187;
				break;
			}
			case 183: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 185;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 24; // AttributeBlock
			}
			case 185: {
				if (t == null) break;
				if (t.kind == 39) {
					goto case 184;
				} else {
					goto case 186;
				}
			}
			case 186: {
				stateStack.Push(188);
				goto case 287; // TypeName
			}
			case 187: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 183;
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (t == null) break;
				Expect(1, t); // EOL
				currentState = 189;
				break;
			}
			case 189: {
				stateStack.Push(190);
				goto case 218; // Block
			}
			case 190: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 191;
				break;
			}
			case 191: {
				if (t == null) break;
				Expect(171, t); // "Operator"
				currentState = 192;
				break;
			}
			case 192: {
				goto case 13; // StatementTerminator
			}
			case 193: {
				if (t == null) break;
				Expect(87, t); // "Const"
				currentState = 195;
				break;
			}
			case 194: { // start of MemberVariableOrConstantDeclaration
				if (t == null) break;
				if (t.kind == 87) {
					goto case 193;
				} else {
					goto case 195;
				}
			}
			case 195: {
				stateStack.Push(198);
				goto case 442; // Identifier
			}
			case 196: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 197;
				break;
			}
			case 197: {
				stateStack.Push(201);
				goto case 287; // TypeName
			}
			case 198: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 196;
				} else {
					goto case 201;
				}
			}
			case 199: {
				if (t == null) break;
				Expect(21, t); // "="
				currentState = 200;
				break;
			}
			case 200: {
				stateStack.Push(202);
				goto case 227; // Expression
			}
			case 201: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 199;
				} else {
					goto case 202;
				}
			}
			case 202: {
				goto case 13; // StatementTerminator
			}
			case 203: { // start of ParameterList
				stateStack.Push(206);
				goto case 208; // Parameter
			}
			case 204: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 205;
				break;
			}
			case 205: {
				stateStack.Push(206);
				goto case 208; // Parameter
			}
			case 206: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 204;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 207: {
				stateStack.Push(208);
				goto case 24; // AttributeBlock
			}
			case 208: { // start of Parameter
				if (t == null) break;
				if (t.kind == 39) {
					goto case 207;
				} else {
					goto case 210;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 544; // ParameterModifier
			}
			case 210: {
				if (t == null) break;
				if (set[13, t.kind]) {
					goto case 209;
				} else {
					goto case 211;
				}
			}
			case 211: {
				stateStack.Push(214);
				goto case 442; // Identifier
			}
			case 212: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 213;
				break;
			}
			case 213: {
				stateStack.Push(217);
				goto case 287; // TypeName
			}
			case 214: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 212;
				} else {
					goto case 217;
				}
			}
			case 215: {
				if (t == null) break;
				Expect(21, t); // "="
				currentState = 216;
				break;
			}
			case 216: {
				goto case 227; // Expression
			}
			case 217: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 215;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 218: { // start of Block
				PushContext(Context.Body, t);
				goto case 219;
			}
			case 219: {
				stateStack.Push(221);
				goto case 13; // StatementTerminator
			}
			case 220: {
				stateStack.Push(221);
				goto case 13; // StatementTerminator
			}
			case 221: {
				if (t == null) break;
				if (t.kind == 1 || t.kind == 22) {
					goto case 220;
				} else {
					goto case 225;
				}
			}
			case 222: {
				stateStack.Push(224);
				goto case 338; // Statement
			}
			case 223: {
				stateStack.Push(224);
				goto case 13; // StatementTerminator
			}
			case 224: {
				if (t == null) break;
				if (t.kind == 1 || t.kind == 22) {
					goto case 223;
				} else {
					goto case 225;
				}
			}
			case 225: {
				if (t == null) break;
				if (set[14, t.kind]) {
					goto case 222;
				} else {
					goto case 226;
				}
			}
			case 226: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 227: { // start of Expression
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 232;
			}
			case 228: {
				goto case 312; // Literal
			}
			case 229: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 230;
				break;
			}
			case 230: {
				stateStack.Push(231);
				goto case 227; // Expression
			}
			case 231: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (t == null) break;
				if (set[15, t.kind]) {
					goto case 228;
				} else {
					goto case 233;
				}
			}
			case 233: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 229;
				} else {
					goto case 243;
				}
			}
			case 234: {
				stateStack.Push(242);
				goto case 442; // Identifier
			}
			case 235: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 236;
				break;
			}
			case 236: {
				if (t == null) break;
				Expect(168, t); // "Of"
				currentState = 237;
				break;
			}
			case 237: {
				stateStack.Push(240);
				goto case 287; // TypeName
			}
			case 238: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 239;
				break;
			}
			case 239: {
				stateStack.Push(240);
				goto case 287; // TypeName
			}
			case 240: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 238;
				} else {
					goto case 241;
				}
			}
			case 241: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 242: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 235;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 243: {
				if (t == null) break;
				if (set[16, t.kind]) {
					goto case 234;
				} else {
					goto case 246;
				}
			}
			case 244: {
				if (t == null) break;
				Expect(56, t); // "AddressOf"
				currentState = 245;
				break;
			}
			case 245: {
				goto case 227; // Expression
			}
			case 246: {
				if (t == null) break;
				if (t.kind == 56) {
					goto case 244;
				} else {
					goto case 252;
				}
			}
			case 247: {
				if (t == null) break;
				Expect(10, t); // XmlOpenTag
				currentState = 248;
				break;
			}
			case 248: {
				PushContext(Context.Xml, t);
				goto case 249;
			}
			case 249: {
				if (t == null) break;
				currentState = 250;
				break;
			}
			case 250: {
				if (t == null) break;
				Expect(11, t); // XmlCloseTag
				currentState = 251;
				break;
			}
			case 251: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 252: {
				if (t == null) break;
				if (t.kind == 10) {
					goto case 247;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 253: {
				if (t == null) break;
				Expect(70, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 254: {
				if (t == null) break;
				Expect(194, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 255: { // start of PrimitiveTypeName
				if (t == null) break;
				if (t.kind == 70) {
					goto case 253;
				} else {
					goto case 256;
				}
			}
			case 256: {
				if (t == null) break;
				if (t.kind == 194) {
					goto case 254;
				} else {
					goto case 258;
				}
			}
			case 257: {
				if (t == null) break;
				Expect(223, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 258: {
				if (t == null) break;
				if (t.kind == 223) {
					goto case 257;
				} else {
					goto case 260;
				}
			}
			case 259: {
				if (t == null) break;
				Expect(199, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				if (t == null) break;
				if (t.kind == 199) {
					goto case 259;
				} else {
					goto case 262;
				}
			}
			case 261: {
				if (t == null) break;
				Expect(219, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 262: {
				if (t == null) break;
				if (t.kind == 219) {
					goto case 261;
				} else {
					goto case 264;
				}
			}
			case 263: {
				if (t == null) break;
				Expect(140, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 264: {
				if (t == null) break;
				if (t.kind == 140) {
					goto case 263;
				} else {
					goto case 266;
				}
			}
			case 265: {
				if (t == null) break;
				Expect(220, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 266: {
				if (t == null) break;
				if (t.kind == 220) {
					goto case 265;
				} else {
					goto case 268;
				}
			}
			case 267: {
				if (t == null) break;
				Expect(150, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 268: {
				if (t == null) break;
				if (t.kind == 150) {
					goto case 267;
				} else {
					goto case 270;
				}
			}
			case 269: {
				if (t == null) break;
				Expect(200, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 270: {
				if (t == null) break;
				if (t.kind == 200) {
					goto case 269;
				} else {
					goto case 272;
				}
			}
			case 271: {
				if (t == null) break;
				Expect(108, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 272: {
				if (t == null) break;
				if (t.kind == 108) {
					goto case 271;
				} else {
					goto case 274;
				}
			}
			case 273: {
				if (t == null) break;
				Expect(99, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 274: {
				if (t == null) break;
				if (t.kind == 99) {
					goto case 273;
				} else {
					goto case 276;
				}
			}
			case 275: {
				if (t == null) break;
				Expect(67, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				if (t == null) break;
				if (t.kind == 67) {
					goto case 275;
				} else {
					goto case 278;
				}
			}
			case 277: {
				if (t == null) break;
				Expect(98, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 278: {
				if (t == null) break;
				if (t.kind == 98) {
					goto case 277;
				} else {
					goto case 280;
				}
			}
			case 279: {
				if (t == null) break;
				Expect(81, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 280: {
				if (t == null) break;
				if (t.kind == 81) {
					goto case 279;
				} else {
					goto case 282;
				}
			}
			case 281: {
				if (t == null) break;
				Expect(206, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 282: {
				if (t == null) break;
				if (t.kind == 206) {
					goto case 281;
				} else {
					goto case 284;
				}
			}
			case 283: {
				if (t == null) break;
				Expect(167, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 284: {
				if (t == null) break;
				if (t.kind == 167) {
					goto case 283;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 285: {
				if (t == null) break;
				Expect(129, t); // "Global"
				currentState = 292;
				break;
			}
			case 286: {
				stateStack.Push(292);
				goto case 442; // Identifier
			}
			case 287: { // start of TypeName
				if (t == null) break;
				if (t.kind == 129) {
					goto case 285;
				} else {
					goto case 288;
				}
			}
			case 288: {
				if (t == null) break;
				if (set[16, t.kind]) {
					goto case 286;
				} else {
					goto case 290;
				}
			}
			case 289: {
				stateStack.Push(292);
				goto case 255; // PrimitiveTypeName
			}
			case 290: {
				if (t == null) break;
				if (set[17, t.kind]) {
					goto case 289;
				} else {
					Error(t);
					goto case 292;
				}
			}
			case 291: {
				stateStack.Push(292);
				goto case 298; // TypeSuffix
			}
			case 292: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 291;
				} else {
					goto case 297;
				}
			}
			case 293: {
				if (t == null) break;
				Expect(27, t); // "."
				currentState = 294;
				break;
			}
			case 294: {
				stateStack.Push(296);
				goto case 309; // IdentifierOrKeyword
			}
			case 295: {
				stateStack.Push(296);
				goto case 298; // TypeSuffix
			}
			case 296: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 295;
				} else {
					goto case 297;
				}
			}
			case 297: {
				if (t == null) break;
				if (t.kind == 27) {
					goto case 293;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 298: { // start of TypeSuffix
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 306;
				break;
			}
			case 299: {
				if (t == null) break;
				Expect(168, t); // "Of"
				currentState = 300;
				break;
			}
			case 300: {
				stateStack.Push(303);
				goto case 287; // TypeName
			}
			case 301: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 302;
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 287; // TypeName
			}
			case 303: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 301;
				} else {
					goto case 308;
				}
			}
			case 304: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 305;
				break;
			}
			case 305: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 304;
				} else {
					goto case 308;
				}
			}
			case 306: {
				if (t == null) break;
				if (t.kind == 168) {
					goto case 299;
				} else {
					goto case 307;
				}
			}
			case 307: {
				if (t == null) break;
				if (t.kind == 23 || t.kind == 37) {
					goto case 305;
				} else {
					Error(t);
					goto case 308;
				}
			}
			case 308: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 309: { // start of IdentifierOrKeyword
				if (t == null) break;
				currentState = stateStack.Pop();
				break;
			}
			case 310: {
				if (t == null) break;
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 311: {
				if (t == null) break;
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 312: { // start of Literal
				if (t == null) break;
				if (t.kind == 3) {
					goto case 310;
				} else {
					goto case 313;
				}
			}
			case 313: {
				if (t == null) break;
				if (t.kind == 4) {
					goto case 311;
				} else {
					goto case 315;
				}
			}
			case 314: {
				if (t == null) break;
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 315: {
				if (t == null) break;
				if (t.kind == 5) {
					goto case 314;
				} else {
					goto case 317;
				}
			}
			case 316: {
				if (t == null) break;
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 317: {
				if (t == null) break;
				if (t.kind == 6) {
					goto case 316;
				} else {
					goto case 319;
				}
			}
			case 318: {
				if (t == null) break;
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 319: {
				if (t == null) break;
				if (t.kind == 7) {
					goto case 318;
				} else {
					goto case 321;
				}
			}
			case 320: {
				if (t == null) break;
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 321: {
				if (t == null) break;
				if (t.kind == 8) {
					goto case 320;
				} else {
					goto case 323;
				}
			}
			case 322: {
				if (t == null) break;
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 323: {
				if (t == null) break;
				if (t.kind == 9) {
					goto case 322;
				} else {
					goto case 325;
				}
			}
			case 324: {
				if (t == null) break;
				Expect(215, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 325: {
				if (t == null) break;
				if (t.kind == 215) {
					goto case 324;
				} else {
					goto case 327;
				}
			}
			case 326: {
				if (t == null) break;
				Expect(121, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 327: {
				if (t == null) break;
				if (t.kind == 121) {
					goto case 326;
				} else {
					goto case 329;
				}
			}
			case 328: {
				if (t == null) break;
				Expect(164, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 329: {
				if (t == null) break;
				if (t.kind == 164) {
					goto case 328;
				} else {
					goto case 331;
				}
			}
			case 330: {
				if (t == null) break;
				Expect(152, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 331: {
				if (t == null) break;
				if (t.kind == 152) {
					goto case 330;
				} else {
					goto case 333;
				}
			}
			case 332: {
				if (t == null) break;
				Expect(157, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 333: {
				if (t == null) break;
				if (t.kind == 157) {
					goto case 332;
				} else {
					goto case 335;
				}
			}
			case 334: {
				if (t == null) break;
				Expect(158, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				if (t == null) break;
				if (t.kind == 158) {
					goto case 334;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 336: {
				goto case 348; // VariableDeclarationStatement
			}
			case 337: {
				goto case 383; // WithOrLockStatement
			}
			case 338: { // start of Statement
				if (t == null) break;
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 336;
				} else {
					goto case 339;
				}
			}
			case 339: {
				if (t == null) break;
				if (t.kind == 209 || t.kind == 231) {
					goto case 337;
				} else {
					goto case 341;
				}
			}
			case 340: {
				goto case 395; // AddOrRemoveHandlerStatement
			}
			case 341: {
				if (t == null) break;
				if (t.kind == 55 || t.kind == 191) {
					goto case 340;
				} else {
					goto case 343;
				}
			}
			case 342: {
				goto case 400; // RaiseEventStatement
			}
			case 343: {
				if (t == null) break;
				if (t.kind == 187) {
					goto case 342;
				} else {
					goto case 345;
				}
			}
			case 344: {
				goto case 408; // InvocationStatement
			}
			case 345: {
				if (t == null) break;
				if (set[18, t.kind]) {
					goto case 344;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 346: {
				if (t == null) break;
				Expect(104, t); // "Dim"
				currentState = 352;
				break;
			}
			case 347: {
				if (t == null) break;
				Expect(202, t); // "Static"
				currentState = 352;
				break;
			}
			case 348: { // start of VariableDeclarationStatement
				if (t == null) break;
				if (t.kind == 104) {
					goto case 346;
				} else {
					goto case 349;
				}
			}
			case 349: {
				if (t == null) break;
				if (t.kind == 202) {
					goto case 347;
				} else {
					goto case 351;
				}
			}
			case 350: {
				if (t == null) break;
				Expect(87, t); // "Const"
				currentState = 352;
				break;
			}
			case 351: {
				if (t == null) break;
				if (t.kind == 87) {
					goto case 350;
				} else {
					Error(t);
					goto case 352;
				}
			}
			case 352: {
				stateStack.Push(354);
				goto case 442; // Identifier
			}
			case 353: {
				if (t == null) break;
				Expect(32, t); // "?"
				currentState = 359;
				break;
			}
			case 354: {
				if (t == null) break;
				if (t.kind == 32) {
					goto case 353;
				} else {
					goto case 359;
				}
			}
			case 355: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 357;
				break;
			}
			case 356: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 357;
				break;
			}
			case 357: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 356;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 369;
				break;
			}
			case 359: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 355;
				} else {
					goto case 369;
				}
			}
			case 360: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 361;
				break;
			}
			case 361: {
				stateStack.Push(363);
				goto case 442; // Identifier
			}
			case 362: {
				if (t == null) break;
				Expect(32, t); // "?"
				currentState = 368;
				break;
			}
			case 363: {
				if (t == null) break;
				if (t.kind == 32) {
					goto case 362;
				} else {
					goto case 368;
				}
			}
			case 364: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 366;
				break;
			}
			case 365: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 366;
				break;
			}
			case 366: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 365;
				} else {
					goto case 367;
				}
			}
			case 367: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 369;
				break;
			}
			case 368: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 364;
				} else {
					goto case 369;
				}
			}
			case 369: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 360;
				} else {
					goto case 377;
				}
			}
			case 370: {
				if (t == null) break;
				Expect(62, t); // "As"
				currentState = 372;
				break;
			}
			case 371: {
				if (t == null) break;
				Expect(161, t); // "New"
				currentState = 373;
				break;
			}
			case 372: {
				if (t == null) break;
				if (t.kind == 161) {
					goto case 371;
				} else {
					goto case 373;
				}
			}
			case 373: {
				stateStack.Push(376);
				goto case 287; // TypeName
			}
			case 374: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 375;
				break;
			}
			case 375: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 380;
				break;
			}
			case 376: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 374;
				} else {
					goto case 380;
				}
			}
			case 377: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 370;
				} else {
					goto case 380;
				}
			}
			case 378: {
				if (t == null) break;
				Expect(21, t); // "="
				currentState = 379;
				break;
			}
			case 379: {
				goto case 227; // Expression
			}
			case 380: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 378;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 381: {
				if (t == null) break;
				Expect(231, t); // "With"
				currentState = 385;
				break;
			}
			case 382: {
				if (t == null) break;
				Expect(209, t); // "SyncLock"
				currentState = 385;
				break;
			}
			case 383: { // start of WithOrLockStatement
				if (t == null) break;
				if (t.kind == 231) {
					goto case 381;
				} else {
					goto case 384;
				}
			}
			case 384: {
				if (t == null) break;
				if (t.kind == 209) {
					goto case 382;
				} else {
					Error(t);
					goto case 385;
				}
			}
			case 385: {
				stateStack.Push(386);
				goto case 227; // Expression
			}
			case 386: {
				stateStack.Push(387);
				goto case 13; // StatementTerminator
			}
			case 387: {
				stateStack.Push(388);
				goto case 218; // Block
			}
			case 388: {
				if (t == null) break;
				Expect(112, t); // "End"
				currentState = 391;
				break;
			}
			case 389: {
				if (t == null) break;
				Expect(231, t); // "With"
				currentState = stateStack.Pop();
				break;
			}
			case 390: {
				if (t == null) break;
				Expect(209, t); // "SyncLock"
				currentState = stateStack.Pop();
				break;
			}
			case 391: {
				if (t == null) break;
				if (t.kind == 231) {
					goto case 389;
				} else {
					goto case 392;
				}
			}
			case 392: {
				if (t == null) break;
				if (t.kind == 209) {
					goto case 390;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 393: {
				if (t == null) break;
				Expect(55, t); // "AddHandler"
				currentState = 397;
				break;
			}
			case 394: {
				if (t == null) break;
				Expect(191, t); // "RemoveHandler"
				currentState = 397;
				break;
			}
			case 395: { // start of AddOrRemoveHandlerStatement
				if (t == null) break;
				if (t.kind == 55) {
					goto case 393;
				} else {
					goto case 396;
				}
			}
			case 396: {
				if (t == null) break;
				if (t.kind == 191) {
					goto case 394;
				} else {
					Error(t);
					goto case 397;
				}
			}
			case 397: {
				stateStack.Push(398);
				goto case 227; // Expression
			}
			case 398: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 399;
				break;
			}
			case 399: {
				goto case 227; // Expression
			}
			case 400: { // start of RaiseEventStatement
				if (t == null) break;
				Expect(187, t); // "RaiseEvent"
				currentState = 401;
				break;
			}
			case 401: {
				stateStack.Push(406);
				goto case 309; // IdentifierOrKeyword
			}
			case 402: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 404;
				break;
			}
			case 403: {
				stateStack.Push(405);
				goto case 435; // ArgumentList
			}
			case 404: {
				if (t == null) break;
				if (set[19, t.kind]) {
					goto case 403;
				} else {
					goto case 405;
				}
			}
			case 405: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 406: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 402;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 407: {
				if (t == null) break;
				Expect(72, t); // "Call"
				currentState = 409;
				break;
			}
			case 408: { // start of InvocationStatement
				if (t == null) break;
				if (t.kind == 72) {
					goto case 407;
				} else {
					goto case 409;
				}
			}
			case 409: {
				stateStack.Push(414);
				goto case 227; // Expression
			}
			case 410: {
				if (t == null) break;
				Expect(36, t); // "("
				currentState = 412;
				break;
			}
			case 411: {
				stateStack.Push(413);
				goto case 435; // ArgumentList
			}
			case 412: {
				if (t == null) break;
				if (set[19, t.kind]) {
					goto case 411;
				} else {
					goto case 413;
				}
			}
			case 413: {
				if (t == null) break;
				Expect(37, t); // ")"
				currentState = 434;
				break;
			}
			case 414: {
				if (t == null) break;
				if (t.kind == 36) {
					goto case 410;
				} else {
					goto case 434;
				}
			}
			case 415: {
				if (t == null) break;
				Expect(21, t); // "="
				currentState = 433;
				break;
			}
			case 416: {
				if (t == null) break;
				Expect(46, t); // "^="
				currentState = 433;
				break;
			}
			case 417: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 415;
				} else {
					goto case 418;
				}
			}
			case 418: {
				if (t == null) break;
				if (t.kind == 46) {
					goto case 416;
				} else {
					goto case 420;
				}
			}
			case 419: {
				if (t == null) break;
				Expect(48, t); // "*="
				currentState = 433;
				break;
			}
			case 420: {
				if (t == null) break;
				if (t.kind == 48) {
					goto case 419;
				} else {
					goto case 422;
				}
			}
			case 421: {
				if (t == null) break;
				Expect(49, t); // "/="
				currentState = 433;
				break;
			}
			case 422: {
				if (t == null) break;
				if (t.kind == 49) {
					goto case 421;
				} else {
					goto case 424;
				}
			}
			case 423: {
				if (t == null) break;
				Expect(50, t); // "\\="
				currentState = 433;
				break;
			}
			case 424: {
				if (t == null) break;
				if (t.kind == 50) {
					goto case 423;
				} else {
					goto case 426;
				}
			}
			case 425: {
				if (t == null) break;
				Expect(45, t); // "+="
				currentState = 433;
				break;
			}
			case 426: {
				if (t == null) break;
				if (t.kind == 45) {
					goto case 425;
				} else {
					goto case 428;
				}
			}
			case 427: {
				if (t == null) break;
				Expect(47, t); // "-="
				currentState = 433;
				break;
			}
			case 428: {
				if (t == null) break;
				if (t.kind == 47) {
					goto case 427;
				} else {
					goto case 430;
				}
			}
			case 429: {
				if (t == null) break;
				Expect(53, t); // "&="
				currentState = 433;
				break;
			}
			case 430: {
				if (t == null) break;
				if (t.kind == 53) {
					goto case 429;
				} else {
					goto case 432;
				}
			}
			case 431: {
				if (t == null) break;
				Expect(41, t); // ">="
				currentState = 433;
				break;
			}
			case 432: {
				if (t == null) break;
				if (t.kind == 41) {
					goto case 431;
				} else {
					Error(t);
					goto case 433;
				}
			}
			case 433: {
				goto case 227; // Expression
			}
			case 434: {
				if (t == null) break;
				if (set[20, t.kind]) {
					goto case 417;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 435: { // start of ArgumentList
				stateStack.Push(441);
				goto case 227; // Expression
			}
			case 436: {
				if (t == null) break;
				Expect(23, t); // ","
				currentState = 437;
				break;
			}
			case 437: {
				stateStack.Push(440);
				goto case 227; // Expression
			}
			case 438: {
				if (t == null) break;
				Expect(54, t); // ":="
				currentState = 439;
				break;
			}
			case 439: {
				stateStack.Push(441);
				goto case 227; // Expression
			}
			case 440: {
				if (t == null) break;
				if (t.kind == 54) {
					goto case 438;
				} else {
					goto case 441;
				}
			}
			case 441: {
				if (t == null) break;
				if (t.kind == 23) {
					goto case 436;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 442: { // start of Identifier
				PushContext(Context.IdentifierExpected, t);
				goto case 445;
			}
			case 443: {
				stateStack.Push(447);
				goto case 450; // IdentifierForFieldDeclaration
			}
			case 444: {
				if (t == null) break;
				Expect(97, t); // "Custom"
				currentState = 447;
				break;
			}
			case 445: {
				if (t == null) break;
				if (set[21, t.kind]) {
					goto case 443;
				} else {
					goto case 446;
				}
			}
			case 446: {
				if (t == null) break;
				if (t.kind == 97) {
					goto case 444;
				} else {
					Error(t);
					goto case 447;
				}
			}
			case 447: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 448: {
				if (t == null) break;
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 449: {
				if (t == null) break;
				Expect(57, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 450: { // start of IdentifierForFieldDeclaration
				if (t == null) break;
				if (t.kind == 2) {
					goto case 448;
				} else {
					goto case 451;
				}
			}
			case 451: {
				if (t == null) break;
				if (t.kind == 57) {
					goto case 449;
				} else {
					goto case 453;
				}
			}
			case 452: {
				if (t == null) break;
				Expect(61, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 453: {
				if (t == null) break;
				if (t.kind == 61) {
					goto case 452;
				} else {
					goto case 455;
				}
			}
			case 454: {
				if (t == null) break;
				Expect(63, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 455: {
				if (t == null) break;
				if (t.kind == 63) {
					goto case 454;
				} else {
					goto case 457;
				}
			}
			case 456: {
				if (t == null) break;
				Expect(64, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 457: {
				if (t == null) break;
				if (t.kind == 64) {
					goto case 456;
				} else {
					goto case 459;
				}
			}
			case 458: {
				if (t == null) break;
				Expect(65, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 459: {
				if (t == null) break;
				if (t.kind == 65) {
					goto case 458;
				} else {
					goto case 461;
				}
			}
			case 460: {
				if (t == null) break;
				Expect(66, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 461: {
				if (t == null) break;
				if (t.kind == 66) {
					goto case 460;
				} else {
					goto case 463;
				}
			}
			case 462: {
				if (t == null) break;
				Expect(69, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 463: {
				if (t == null) break;
				if (t.kind == 69) {
					goto case 462;
				} else {
					goto case 465;
				}
			}
			case 464: {
				if (t == null) break;
				Expect(86, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 465: {
				if (t == null) break;
				if (t.kind == 86) {
					goto case 464;
				} else {
					goto case 467;
				}
			}
			case 466: {
				if (t == null) break;
				Expect(103, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 467: {
				if (t == null) break;
				if (t.kind == 103) {
					goto case 466;
				} else {
					goto case 469;
				}
			}
			case 468: {
				if (t == null) break;
				Expect(106, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 469: {
				if (t == null) break;
				if (t.kind == 106) {
					goto case 468;
				} else {
					goto case 471;
				}
			}
			case 470: {
				if (t == null) break;
				Expect(115, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 471: {
				if (t == null) break;
				if (t.kind == 115) {
					goto case 470;
				} else {
					goto case 473;
				}
			}
			case 472: {
				if (t == null) break;
				Expect(120, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 473: {
				if (t == null) break;
				if (t.kind == 120) {
					goto case 472;
				} else {
					goto case 475;
				}
			}
			case 474: {
				if (t == null) break;
				Expect(125, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 475: {
				if (t == null) break;
				if (t.kind == 125) {
					goto case 474;
				} else {
					goto case 477;
				}
			}
			case 476: {
				if (t == null) break;
				Expect(132, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 477: {
				if (t == null) break;
				if (t.kind == 132) {
					goto case 476;
				} else {
					goto case 479;
				}
			}
			case 478: {
				if (t == null) break;
				Expect(138, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 479: {
				if (t == null) break;
				if (t.kind == 138) {
					goto case 478;
				} else {
					goto case 481;
				}
			}
			case 480: {
				if (t == null) break;
				Expect(142, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 481: {
				if (t == null) break;
				if (t.kind == 142) {
					goto case 480;
				} else {
					goto case 483;
				}
			}
			case 482: {
				if (t == null) break;
				Expect(145, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 483: {
				if (t == null) break;
				if (t.kind == 145) {
					goto case 482;
				} else {
					goto case 485;
				}
			}
			case 484: {
				if (t == null) break;
				Expect(146, t); // "Key"
				currentState = stateStack.Pop();
				break;
			}
			case 485: {
				if (t == null) break;
				if (t.kind == 146) {
					goto case 484;
				} else {
					goto case 487;
				}
			}
			case 486: {
				if (t == null) break;
				Expect(169, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 487: {
				if (t == null) break;
				if (t.kind == 169) {
					goto case 486;
				} else {
					goto case 489;
				}
			}
			case 488: {
				if (t == null) break;
				Expect(175, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 489: {
				if (t == null) break;
				if (t.kind == 175) {
					goto case 488;
				} else {
					goto case 491;
				}
			}
			case 490: {
				if (t == null) break;
				Expect(182, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 491: {
				if (t == null) break;
				if (t.kind == 182) {
					goto case 490;
				} else {
					goto case 493;
				}
			}
			case 492: {
				if (t == null) break;
				Expect(201, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 493: {
				if (t == null) break;
				if (t.kind == 201) {
					goto case 492;
				} else {
					goto case 495;
				}
			}
			case 494: {
				if (t == null) break;
				Expect(210, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 495: {
				if (t == null) break;
				if (t.kind == 210) {
					goto case 494;
				} else {
					goto case 497;
				}
			}
			case 496: {
				if (t == null) break;
				Expect(211, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 497: {
				if (t == null) break;
				if (t.kind == 211) {
					goto case 496;
				} else {
					goto case 499;
				}
			}
			case 498: {
				if (t == null) break;
				Expect(221, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 499: {
				if (t == null) break;
				if (t.kind == 221) {
					goto case 498;
				} else {
					goto case 501;
				}
			}
			case 500: {
				if (t == null) break;
				Expect(222, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 501: {
				if (t == null) break;
				if (t.kind == 222) {
					goto case 500;
				} else {
					goto case 503;
				}
			}
			case 502: {
				if (t == null) break;
				Expect(228, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 503: {
				if (t == null) break;
				if (t.kind == 228) {
					goto case 502;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 504: {
				if (t == null) break;
				Expect(186, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 505: {
				if (t == null) break;
				Expect(124, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 506: { // start of AccessModifier
				if (t == null) break;
				if (t.kind == 186) {
					goto case 504;
				} else {
					goto case 507;
				}
			}
			case 507: {
				if (t == null) break;
				if (t.kind == 124) {
					goto case 505;
				} else {
					goto case 509;
				}
			}
			case 508: {
				if (t == null) break;
				Expect(185, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 509: {
				if (t == null) break;
				if (t.kind == 185) {
					goto case 508;
				} else {
					goto case 511;
				}
			}
			case 510: {
				if (t == null) break;
				Expect(183, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 511: {
				if (t == null) break;
				if (t.kind == 183) {
					goto case 510;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 512: {
				goto case 506; // AccessModifier
			}
			case 513: {
				if (t == null) break;
				Expect(197, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 514: { // start of TypeModifier
				if (t == null) break;
				if (set[22, t.kind]) {
					goto case 512;
				} else {
					goto case 515;
				}
			}
			case 515: {
				if (t == null) break;
				if (t.kind == 197) {
					goto case 513;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 516: {
				goto case 506; // AccessModifier
			}
			case 517: {
				if (t == null) break;
				Expect(197, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 518: { // start of MemberModifier
				if (t == null) break;
				if (set[22, t.kind]) {
					goto case 516;
				} else {
					goto case 519;
				}
			}
			case 519: {
				if (t == null) break;
				if (t.kind == 197) {
					goto case 517;
				} else {
					goto case 521;
				}
			}
			case 520: {
				if (t == null) break;
				Expect(198, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 521: {
				if (t == null) break;
				if (t.kind == 198) {
					goto case 520;
				} else {
					goto case 523;
				}
			}
			case 522: {
				if (t == null) break;
				Expect(178, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 523: {
				if (t == null) break;
				if (t.kind == 178) {
					goto case 522;
				} else {
					goto case 525;
				}
			}
			case 524: {
				if (t == null) break;
				Expect(166, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 525: {
				if (t == null) break;
				if (t.kind == 166) {
					goto case 524;
				} else {
					goto case 527;
				}
			}
			case 526: {
				if (t == null) break;
				Expect(179, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 527: {
				if (t == null) break;
				if (t.kind == 179) {
					goto case 526;
				} else {
					goto case 529;
				}
			}
			case 528: {
				if (t == null) break;
				Expect(177, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 529: {
				if (t == null) break;
				if (t.kind == 177) {
					goto case 528;
				} else {
					goto case 531;
				}
			}
			case 530: {
				if (t == null) break;
				Expect(181, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 531: {
				if (t == null) break;
				if (t.kind == 181) {
					goto case 530;
				} else {
					goto case 533;
				}
			}
			case 532: {
				if (t == null) break;
				Expect(232, t); // "WithEvents"
				currentState = stateStack.Pop();
				break;
			}
			case 533: {
				if (t == null) break;
				if (t.kind == 232) {
					goto case 532;
				} else {
					goto case 535;
				}
			}
			case 534: {
				if (t == null) break;
				Expect(156, t); // "MustOverride"
				currentState = stateStack.Pop();
				break;
			}
			case 535: {
				if (t == null) break;
				if (t.kind == 156) {
					goto case 534;
				} else {
					goto case 537;
				}
			}
			case 536: {
				if (t == null) break;
				Expect(230, t); // "Widening"
				currentState = stateStack.Pop();
				break;
			}
			case 537: {
				if (t == null) break;
				if (t.kind == 230) {
					goto case 536;
				} else {
					goto case 539;
				}
			}
			case 538: {
				if (t == null) break;
				Expect(160, t); // "Narrowing"
				currentState = stateStack.Pop();
				break;
			}
			case 539: {
				if (t == null) break;
				if (t.kind == 160) {
					goto case 538;
				} else {
					goto case 541;
				}
			}
			case 540: {
				if (t == null) break;
				Expect(104, t); // "Dim"
				currentState = stateStack.Pop();
				break;
			}
			case 541: {
				if (t == null) break;
				if (t.kind == 104) {
					goto case 540;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 542: {
				if (t == null) break;
				Expect(71, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 543: {
				if (t == null) break;
				Expect(68, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 544: { // start of ParameterModifier
				if (t == null) break;
				if (t.kind == 71) {
					goto case 542;
				} else {
					goto case 545;
				}
			}
			case 545: {
				if (t == null) break;
				if (t.kind == 68) {
					goto case 543;
				} else {
					goto case 547;
				}
			}
			case 546: {
				if (t == null) break;
				Expect(173, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 547: {
				if (t == null) break;
				if (t.kind == 173) {
					goto case 546;
				} else {
					goto case 549;
				}
			}
			case 548: {
				if (t == null) break;
				Expect(180, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 549: {
				if (t == null) break;
				if (t.kind == 180) {
					goto case 548;
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
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,T,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};

} // end Parser


}