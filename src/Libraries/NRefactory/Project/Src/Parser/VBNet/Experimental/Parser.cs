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
		switchlbl: switch (currentState) {
			case 0: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 1: { // start of ExpressionFinder
				PushContext(Context.Global);
				goto case 3;
			}
			case 2: {
				stateStack.Push(3);
				goto case 15; // OptionStatement
			}
			case 3: {
				if (t == null) break;
				if (t.kind == 170) {
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
				if (t.kind == 134) {
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
				if (t.kind == 37) {
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
				Expect(20, t); // ":"
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
				if (t.kind == 20) {
					goto case 12;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 15: { // start of OptionStatement
				if (t == null) break;
				Expect(170, t); // "Option"
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
				Expect(134, t); // "Imports"
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
				Expect(37, t); // "<"
				currentState = 25;
				break;
			}
			case 25: {
				PushContext(Context.Attribute);
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
				Expect(36, t); // ">"
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
				if (t.kind == 157) {
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
				Expect(157, t); // "Namespace"
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
				Expect(110, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				if (t == null) break;
				Expect(157, t); // "Namespace"
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
				if (t.kind == 37) {
					goto case 45;
				} else {
					goto case 48;
				}
			}
			case 47: {
				stateStack.Push(48);
				goto case 443; // TypeModifier
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
				Expect(152, t); // "Module"
				currentState = 54;
				break;
			}
			case 50: {
				if (t == null) break;
				Expect(81, t); // "Class"
				currentState = 54;
				break;
			}
			case 51: {
				if (t == null) break;
				if (t.kind == 152) {
					goto case 49;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (t == null) break;
				if (t.kind == 81) {
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
				PushContext(Context.Type);
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
				Expect(110, t); // "End"
				currentState = 62;
				break;
			}
			case 60: {
				if (t == null) break;
				Expect(152, t); // "Module"
				currentState = 64;
				break;
			}
			case 61: {
				if (t == null) break;
				Expect(81, t); // "Class"
				currentState = 64;
				break;
			}
			case 62: {
				if (t == null) break;
				if (t.kind == 152) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t == null) break;
				if (t.kind == 81) {
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
				PushContext(Context.Member);
				goto case 68;
			}
			case 67: {
				stateStack.Push(68);
				goto case 24; // AttributeBlock
			}
			case 68: {
				if (t == null) break;
				if (t.kind == 37) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 447; // MemberModifier
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
				stateStack.Push(75);
				goto case 99; // MemberVariableOrConstantDeclaration
			}
			case 72: {
				stateStack.Push(75);
				goto case 78; // SubOrFunctionDeclaration
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
				if (t.kind == 124 || t.kind == 206) {
					goto case 72;
				} else {
					Error(t);
					goto case 75;
				}
			}
			case 75: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 76: {
				if (t == null) break;
				Expect(206, t); // "Sub"
				currentState = 80;
				break;
			}
			case 77: {
				if (t == null) break;
				Expect(124, t); // "Function"
				currentState = 80;
				break;
			}
			case 78: { // start of SubOrFunctionDeclaration
				if (t == null) break;
				if (t.kind == 206) {
					goto case 76;
				} else {
					goto case 79;
				}
			}
			case 79: {
				if (t == null) break;
				if (t.kind == 124) {
					goto case 77;
				} else {
					Error(t);
					goto case 80;
				}
			}
			case 80: {
				PushContext(Context.IdentifierExpected);
				goto case 81;
			}
			case 81: {
				if (t == null) break;
				currentState = 82;
				break;
			}
			case 82: {
				PopContext();
				goto case 87;
			}
			case 83: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 85;
				break;
			}
			case 84: {
				stateStack.Push(86);
				goto case 108; // ParameterList
			}
			case 85: {
				if (t == null) break;
				if (set[9, t.kind]) {
					goto case 84;
				} else {
					goto case 86;
				}
			}
			case 86: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = 90;
				break;
			}
			case 87: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 83;
				} else {
					goto case 90;
				}
			}
			case 88: {
				if (t == null) break;
				Expect(60, t); // "As"
				currentState = 89;
				break;
			}
			case 89: {
				stateStack.Push(91);
				goto case 192; // TypeName
			}
			case 90: {
				if (t == null) break;
				if (t.kind == 60) {
					goto case 88;
				} else {
					goto case 91;
				}
			}
			case 91: {
				stateStack.Push(92);
				goto case 123; // Block
			}
			case 92: {
				if (t == null) break;
				Expect(110, t); // "End"
				currentState = 95;
				break;
			}
			case 93: {
				if (t == null) break;
				Expect(206, t); // "Sub"
				currentState = 97;
				break;
			}
			case 94: {
				if (t == null) break;
				Expect(124, t); // "Function"
				currentState = 97;
				break;
			}
			case 95: {
				if (t == null) break;
				if (t.kind == 206) {
					goto case 93;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (t == null) break;
				if (t.kind == 124) {
					goto case 94;
				} else {
					Error(t);
					goto case 97;
				}
			}
			case 97: {
				goto case 13; // StatementTerminator
			}
			case 98: {
				if (t == null) break;
				Expect(85, t); // "Const"
				currentState = 100;
				break;
			}
			case 99: { // start of MemberVariableOrConstantDeclaration
				if (t == null) break;
				if (t.kind == 85) {
					goto case 98;
				} else {
					goto case 100;
				}
			}
			case 100: {
				stateStack.Push(103);
				goto case 371; // Identifier
			}
			case 101: {
				if (t == null) break;
				Expect(60, t); // "As"
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(106);
				goto case 192; // TypeName
			}
			case 103: {
				if (t == null) break;
				if (t.kind == 60) {
					goto case 101;
				} else {
					goto case 106;
				}
			}
			case 104: {
				if (t == null) break;
				Expect(19, t); // "="
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(107);
				goto case 132; // Expression
			}
			case 106: {
				if (t == null) break;
				if (t.kind == 19) {
					goto case 104;
				} else {
					goto case 107;
				}
			}
			case 107: {
				goto case 13; // StatementTerminator
			}
			case 108: { // start of ParameterList
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 109: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 111: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 109;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 112: {
				stateStack.Push(113);
				goto case 24; // AttributeBlock
			}
			case 113: { // start of Parameter
				if (t == null) break;
				if (t.kind == 37) {
					goto case 112;
				} else {
					goto case 115;
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 467; // ParameterModifier
			}
			case 115: {
				if (t == null) break;
				if (set[10, t.kind]) {
					goto case 114;
				} else {
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(119);
				goto case 371; // Identifier
			}
			case 117: {
				if (t == null) break;
				Expect(60, t); // "As"
				currentState = 118;
				break;
			}
			case 118: {
				stateStack.Push(122);
				goto case 192; // TypeName
			}
			case 119: {
				if (t == null) break;
				if (t.kind == 60) {
					goto case 117;
				} else {
					goto case 122;
				}
			}
			case 120: {
				if (t == null) break;
				Expect(19, t); // "="
				currentState = 121;
				break;
			}
			case 121: {
				goto case 132; // Expression
			}
			case 122: {
				if (t == null) break;
				if (t.kind == 19) {
					goto case 120;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: { // start of Block
				PushContext(Context.Body);
				goto case 124;
			}
			case 124: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 125: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 126: {
				if (t == null) break;
				if (t.kind == 1 || t.kind == 20) {
					goto case 125;
				} else {
					goto case 128;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 281; // Statement
			}
			case 128: {
				if (t == null) break;
				if (set[11, t.kind]) {
					goto case 127;
				} else {
					goto case 130;
				}
			}
			case 129: {
				stateStack.Push(130);
				goto case 13; // StatementTerminator
			}
			case 130: {
				if (t == null) break;
				if (t.kind == 1 || t.kind == 20) {
					goto case 129;
				} else {
					goto case 131;
				}
			}
			case 131: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 132: { // start of Expression
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 137;
			}
			case 133: {
				goto case 217; // Literal
			}
			case 134: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 132; // Expression
			}
			case 136: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 137: {
				if (t == null) break;
				if (set[12, t.kind]) {
					goto case 133;
				} else {
					goto case 138;
				}
			}
			case 138: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 134;
				} else {
					goto case 148;
				}
			}
			case 139: {
				stateStack.Push(147);
				goto case 371; // Identifier
			}
			case 140: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 141;
				break;
			}
			case 141: {
				if (t == null) break;
				Expect(166, t); // "Of"
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(145);
				goto case 192; // TypeName
			}
			case 143: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 144;
				break;
			}
			case 144: {
				stateStack.Push(145);
				goto case 192; // TypeName
			}
			case 145: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 143;
				} else {
					goto case 146;
				}
			}
			case 146: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 140;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 148: {
				if (t == null) break;
				if (set[13, t.kind]) {
					goto case 139;
				} else {
					goto case 151;
				}
			}
			case 149: {
				if (t == null) break;
				Expect(54, t); // "AddressOf"
				currentState = 150;
				break;
			}
			case 150: {
				goto case 132; // Expression
			}
			case 151: {
				if (t == null) break;
				if (t.kind == 54) {
					goto case 149;
				} else {
					goto case 157;
				}
			}
			case 152: {
				if (t == null) break;
				Expect(10, t); // XmlOpenTag
				currentState = 153;
				break;
			}
			case 153: {
				PushContext(Context.Xml);
				goto case 154;
			}
			case 154: {
				if (t == null) break;
				currentState = 155;
				break;
			}
			case 155: {
				if (t == null) break;
				Expect(36, t); // ">"
				currentState = 156;
				break;
			}
			case 156: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 157: {
				if (t == null) break;
				if (t.kind == 10) {
					goto case 152;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 158: {
				if (t == null) break;
				Expect(68, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 159: {
				if (t == null) break;
				Expect(192, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 160: { // start of PrimitiveTypeName
				if (t == null) break;
				if (t.kind == 68) {
					goto case 158;
				} else {
					goto case 161;
				}
			}
			case 161: {
				if (t == null) break;
				if (t.kind == 192) {
					goto case 159;
				} else {
					goto case 163;
				}
			}
			case 162: {
				if (t == null) break;
				Expect(221, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 163: {
				if (t == null) break;
				if (t.kind == 221) {
					goto case 162;
				} else {
					goto case 165;
				}
			}
			case 164: {
				if (t == null) break;
				Expect(197, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 165: {
				if (t == null) break;
				if (t.kind == 197) {
					goto case 164;
				} else {
					goto case 167;
				}
			}
			case 166: {
				if (t == null) break;
				Expect(217, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 167: {
				if (t == null) break;
				if (t.kind == 217) {
					goto case 166;
				} else {
					goto case 169;
				}
			}
			case 168: {
				if (t == null) break;
				Expect(138, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 169: {
				if (t == null) break;
				if (t.kind == 138) {
					goto case 168;
				} else {
					goto case 171;
				}
			}
			case 170: {
				if (t == null) break;
				Expect(218, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 171: {
				if (t == null) break;
				if (t.kind == 218) {
					goto case 170;
				} else {
					goto case 173;
				}
			}
			case 172: {
				if (t == null) break;
				Expect(148, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 173: {
				if (t == null) break;
				if (t.kind == 148) {
					goto case 172;
				} else {
					goto case 175;
				}
			}
			case 174: {
				if (t == null) break;
				Expect(198, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 175: {
				if (t == null) break;
				if (t.kind == 198) {
					goto case 174;
				} else {
					goto case 177;
				}
			}
			case 176: {
				if (t == null) break;
				Expect(106, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 177: {
				if (t == null) break;
				if (t.kind == 106) {
					goto case 176;
				} else {
					goto case 179;
				}
			}
			case 178: {
				if (t == null) break;
				Expect(97, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 179: {
				if (t == null) break;
				if (t.kind == 97) {
					goto case 178;
				} else {
					goto case 181;
				}
			}
			case 180: {
				if (t == null) break;
				Expect(65, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 181: {
				if (t == null) break;
				if (t.kind == 65) {
					goto case 180;
				} else {
					goto case 183;
				}
			}
			case 182: {
				if (t == null) break;
				Expect(96, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 183: {
				if (t == null) break;
				if (t.kind == 96) {
					goto case 182;
				} else {
					goto case 185;
				}
			}
			case 184: {
				if (t == null) break;
				Expect(79, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 185: {
				if (t == null) break;
				if (t.kind == 79) {
					goto case 184;
				} else {
					goto case 187;
				}
			}
			case 186: {
				if (t == null) break;
				Expect(204, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 187: {
				if (t == null) break;
				if (t.kind == 204) {
					goto case 186;
				} else {
					goto case 189;
				}
			}
			case 188: {
				if (t == null) break;
				Expect(165, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				if (t == null) break;
				if (t.kind == 165) {
					goto case 188;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				if (t == null) break;
				Expect(127, t); // "Global"
				currentState = 197;
				break;
			}
			case 191: {
				stateStack.Push(197);
				goto case 371; // Identifier
			}
			case 192: { // start of TypeName
				if (t == null) break;
				if (t.kind == 127) {
					goto case 190;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (t == null) break;
				if (set[13, t.kind]) {
					goto case 191;
				} else {
					goto case 195;
				}
			}
			case 194: {
				stateStack.Push(197);
				goto case 160; // PrimitiveTypeName
			}
			case 195: {
				if (t == null) break;
				if (set[14, t.kind]) {
					goto case 194;
				} else {
					Error(t);
					goto case 197;
				}
			}
			case 196: {
				stateStack.Push(197);
				goto case 203; // TypeSuffix
			}
			case 197: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 196;
				} else {
					goto case 202;
				}
			}
			case 198: {
				if (t == null) break;
				Expect(25, t); // "."
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(201);
				goto case 214; // IdentifierOrKeyword
			}
			case 200: {
				stateStack.Push(201);
				goto case 203; // TypeSuffix
			}
			case 201: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 200;
				} else {
					goto case 202;
				}
			}
			case 202: {
				if (t == null) break;
				if (t.kind == 25) {
					goto case 198;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: { // start of TypeSuffix
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 211;
				break;
			}
			case 204: {
				if (t == null) break;
				Expect(166, t); // "Of"
				currentState = 205;
				break;
			}
			case 205: {
				stateStack.Push(208);
				goto case 192; // TypeName
			}
			case 206: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 207;
				break;
			}
			case 207: {
				stateStack.Push(208);
				goto case 192; // TypeName
			}
			case 208: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 206;
				} else {
					goto case 213;
				}
			}
			case 209: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 210;
				break;
			}
			case 210: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 209;
				} else {
					goto case 213;
				}
			}
			case 211: {
				if (t == null) break;
				if (t.kind == 166) {
					goto case 204;
				} else {
					goto case 212;
				}
			}
			case 212: {
				if (t == null) break;
				if (t.kind == 21 || t.kind == 35) {
					goto case 210;
				} else {
					Error(t);
					goto case 213;
				}
			}
			case 213: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 214: { // start of IdentifierOrKeyword
				if (t == null) break;
				currentState = stateStack.Pop();
				break;
			}
			case 215: {
				if (t == null) break;
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 216: {
				if (t == null) break;
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 217: { // start of Literal
				if (t == null) break;
				if (t.kind == 3) {
					goto case 215;
				} else {
					goto case 218;
				}
			}
			case 218: {
				if (t == null) break;
				if (t.kind == 4) {
					goto case 216;
				} else {
					goto case 220;
				}
			}
			case 219: {
				if (t == null) break;
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 220: {
				if (t == null) break;
				if (t.kind == 5) {
					goto case 219;
				} else {
					goto case 222;
				}
			}
			case 221: {
				if (t == null) break;
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				if (t == null) break;
				if (t.kind == 6) {
					goto case 221;
				} else {
					goto case 224;
				}
			}
			case 223: {
				if (t == null) break;
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				if (t == null) break;
				if (t.kind == 7) {
					goto case 223;
				} else {
					goto case 226;
				}
			}
			case 225: {
				if (t == null) break;
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 226: {
				if (t == null) break;
				if (t.kind == 8) {
					goto case 225;
				} else {
					goto case 228;
				}
			}
			case 227: {
				if (t == null) break;
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 228: {
				if (t == null) break;
				if (t.kind == 9) {
					goto case 227;
				} else {
					goto case 230;
				}
			}
			case 229: {
				if (t == null) break;
				Expect(213, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 230: {
				if (t == null) break;
				if (t.kind == 213) {
					goto case 229;
				} else {
					goto case 232;
				}
			}
			case 231: {
				if (t == null) break;
				Expect(119, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (t == null) break;
				if (t.kind == 119) {
					goto case 231;
				} else {
					goto case 234;
				}
			}
			case 233: {
				if (t == null) break;
				Expect(162, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				if (t == null) break;
				if (t.kind == 162) {
					goto case 233;
				} else {
					goto case 236;
				}
			}
			case 235: {
				if (t == null) break;
				Expect(150, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				if (t == null) break;
				if (t.kind == 150) {
					goto case 235;
				} else {
					goto case 238;
				}
			}
			case 237: {
				if (t == null) break;
				Expect(155, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				if (t == null) break;
				if (t.kind == 155) {
					goto case 237;
				} else {
					goto case 240;
				}
			}
			case 239: {
				if (t == null) break;
				Expect(156, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				if (t == null) break;
				if (t.kind == 156) {
					goto case 239;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 241: {
				stateStack.Push(245);
				goto case 371; // Identifier
			}
			case 242: {
				if (t == null) break;
				Expect(5, t); // LiteralInteger
				currentState = 245;
				break;
			}
			case 243: {
				if (t == null) break;
				if (set[13, t.kind]) {
					goto case 241;
				} else {
					goto case 244;
				}
			}
			case 244: {
				if (t == null) break;
				if (t.kind == 5) {
					goto case 242;
				} else {
					Error(t);
					goto case 245;
				}
			}
			case 245: {
				if (t == null) break;
				Expect(20, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				if (t == null) break;
				Expect(102, t); // "Dim"
				currentState = 252;
				break;
			}
			case 247: {
				if (t == null) break;
				Expect(200, t); // "Static"
				currentState = 252;
				break;
			}
			case 248: {
				if (t == null) break;
				if (t.kind == 102) {
					goto case 246;
				} else {
					goto case 249;
				}
			}
			case 249: {
				if (t == null) break;
				if (t.kind == 200) {
					goto case 247;
				} else {
					goto case 251;
				}
			}
			case 250: {
				if (t == null) break;
				Expect(85, t); // "Const"
				currentState = 252;
				break;
			}
			case 251: {
				if (t == null) break;
				if (t.kind == 85) {
					goto case 250;
				} else {
					Error(t);
					goto case 252;
				}
			}
			case 252: {
				stateStack.Push(254);
				goto case 371; // Identifier
			}
			case 253: {
				if (t == null) break;
				Expect(30, t); // "?"
				currentState = 259;
				break;
			}
			case 254: {
				if (t == null) break;
				if (t.kind == 30) {
					goto case 253;
				} else {
					goto case 259;
				}
			}
			case 255: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 257;
				break;
			}
			case 256: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 257;
				break;
			}
			case 257: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 256;
				} else {
					goto case 258;
				}
			}
			case 258: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = 269;
				break;
			}
			case 259: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 255;
				} else {
					goto case 269;
				}
			}
			case 260: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 261;
				break;
			}
			case 261: {
				stateStack.Push(263);
				goto case 371; // Identifier
			}
			case 262: {
				if (t == null) break;
				Expect(30, t); // "?"
				currentState = 268;
				break;
			}
			case 263: {
				if (t == null) break;
				if (t.kind == 30) {
					goto case 262;
				} else {
					goto case 268;
				}
			}
			case 264: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 266;
				break;
			}
			case 265: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 266;
				break;
			}
			case 266: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 265;
				} else {
					goto case 267;
				}
			}
			case 267: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = 269;
				break;
			}
			case 268: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 264;
				} else {
					goto case 269;
				}
			}
			case 269: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 260;
				} else {
					goto case 277;
				}
			}
			case 270: {
				if (t == null) break;
				Expect(60, t); // "As"
				currentState = 272;
				break;
			}
			case 271: {
				if (t == null) break;
				Expect(159, t); // "New"
				currentState = 273;
				break;
			}
			case 272: {
				if (t == null) break;
				if (t.kind == 159) {
					goto case 271;
				} else {
					goto case 273;
				}
			}
			case 273: {
				stateStack.Push(276);
				goto case 192; // TypeName
			}
			case 274: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 275;
				break;
			}
			case 275: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = 280;
				break;
			}
			case 276: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 274;
				} else {
					goto case 280;
				}
			}
			case 277: {
				if (t == null) break;
				if (t.kind == 60) {
					goto case 270;
				} else {
					goto case 280;
				}
			}
			case 278: {
				if (t == null) break;
				Expect(19, t); // "="
				currentState = 279;
				break;
			}
			case 279: {
				goto case 132; // Expression
			}
			case 280: {
				if (t == null) break;
				if (t.kind == 19) {
					goto case 278;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 281: { // start of Statement
				if (t == null) break;
				if (set[15, t.kind]) {
					goto case 243;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (t == null) break;
				if (t.kind == 85 || t.kind == 102 || t.kind == 200) {
					goto case 248;
				} else {
					goto case 295;
				}
			}
			case 283: {
				if (t == null) break;
				Expect(229, t); // "With"
				currentState = 287;
				break;
			}
			case 284: {
				if (t == null) break;
				Expect(207, t); // "SyncLock"
				currentState = 287;
				break;
			}
			case 285: {
				if (t == null) break;
				if (t.kind == 229) {
					goto case 283;
				} else {
					goto case 286;
				}
			}
			case 286: {
				if (t == null) break;
				if (t.kind == 207) {
					goto case 284;
				} else {
					Error(t);
					goto case 287;
				}
			}
			case 287: {
				stateStack.Push(288);
				goto case 132; // Expression
			}
			case 288: {
				stateStack.Push(289);
				goto case 13; // StatementTerminator
			}
			case 289: {
				stateStack.Push(290);
				goto case 123; // Block
			}
			case 290: {
				if (t == null) break;
				Expect(110, t); // "End"
				currentState = 293;
				break;
			}
			case 291: {
				if (t == null) break;
				Expect(229, t); // "With"
				currentState = stateStack.Pop();
				break;
			}
			case 292: {
				if (t == null) break;
				Expect(207, t); // "SyncLock"
				currentState = stateStack.Pop();
				break;
			}
			case 293: {
				if (t == null) break;
				if (t.kind == 229) {
					goto case 291;
				} else {
					goto case 294;
				}
			}
			case 294: {
				if (t == null) break;
				if (t.kind == 207) {
					goto case 292;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 295: {
				if (t == null) break;
				if (t.kind == 207 || t.kind == 229) {
					goto case 285;
				} else {
					goto case 303;
				}
			}
			case 296: {
				if (t == null) break;
				Expect(53, t); // "AddHandler"
				currentState = 300;
				break;
			}
			case 297: {
				if (t == null) break;
				Expect(189, t); // "RemoveHandler"
				currentState = 300;
				break;
			}
			case 298: {
				if (t == null) break;
				if (t.kind == 53) {
					goto case 296;
				} else {
					goto case 299;
				}
			}
			case 299: {
				if (t == null) break;
				if (t.kind == 189) {
					goto case 297;
				} else {
					Error(t);
					goto case 300;
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 132; // Expression
			}
			case 301: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 302;
				break;
			}
			case 302: {
				goto case 132; // Expression
			}
			case 303: {
				if (t == null) break;
				if (t.kind == 53 || t.kind == 189) {
					goto case 298;
				} else {
					goto case 311;
				}
			}
			case 304: {
				if (t == null) break;
				Expect(185, t); // "RaiseEvent"
				currentState = 305;
				break;
			}
			case 305: {
				stateStack.Push(310);
				goto case 214; // IdentifierOrKeyword
			}
			case 306: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 308;
				break;
			}
			case 307: {
				stateStack.Push(309);
				goto case 357; // ArgumentList
			}
			case 308: {
				if (t == null) break;
				if (set[16, t.kind]) {
					goto case 307;
				} else {
					goto case 309;
				}
			}
			case 309: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 310: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 306;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 311: {
				if (t == null) break;
				if (t.kind == 185) {
					goto case 304;
				} else {
					goto case 336;
				}
			}
			case 312: {
				stateStack.Push(315);
				goto case 132; // Expression
			}
			case 313: {
				if (t == null) break;
				Expect(19, t); // "="
				currentState = 334;
				break;
			}
			case 314: {
				if (t == null) break;
				Expect(44, t); // "^="
				currentState = 334;
				break;
			}
			case 315: {
				if (t == null) break;
				if (t.kind == 19) {
					goto case 313;
				} else {
					goto case 316;
				}
			}
			case 316: {
				if (t == null) break;
				if (t.kind == 44) {
					goto case 314;
				} else {
					goto case 318;
				}
			}
			case 317: {
				if (t == null) break;
				Expect(46, t); // "*="
				currentState = 334;
				break;
			}
			case 318: {
				if (t == null) break;
				if (t.kind == 46) {
					goto case 317;
				} else {
					goto case 320;
				}
			}
			case 319: {
				if (t == null) break;
				Expect(47, t); // "/="
				currentState = 334;
				break;
			}
			case 320: {
				if (t == null) break;
				if (t.kind == 47) {
					goto case 319;
				} else {
					goto case 322;
				}
			}
			case 321: {
				if (t == null) break;
				Expect(48, t); // "\\="
				currentState = 334;
				break;
			}
			case 322: {
				if (t == null) break;
				if (t.kind == 48) {
					goto case 321;
				} else {
					goto case 324;
				}
			}
			case 323: {
				if (t == null) break;
				Expect(43, t); // "+="
				currentState = 334;
				break;
			}
			case 324: {
				if (t == null) break;
				if (t.kind == 43) {
					goto case 323;
				} else {
					goto case 326;
				}
			}
			case 325: {
				if (t == null) break;
				Expect(45, t); // "-="
				currentState = 334;
				break;
			}
			case 326: {
				if (t == null) break;
				if (t.kind == 45) {
					goto case 325;
				} else {
					goto case 328;
				}
			}
			case 327: {
				if (t == null) break;
				Expect(51, t); // "&="
				currentState = 334;
				break;
			}
			case 328: {
				if (t == null) break;
				if (t.kind == 51) {
					goto case 327;
				} else {
					goto case 330;
				}
			}
			case 329: {
				if (t == null) break;
				Expect(49, t); // "<<="
				currentState = 334;
				break;
			}
			case 330: {
				if (t == null) break;
				if (t.kind == 49) {
					goto case 329;
				} else {
					goto case 332;
				}
			}
			case 331: {
				if (t == null) break;
				Expect(50, t); // ">>="
				currentState = 334;
				break;
			}
			case 332: {
				if (t == null) break;
				if (t.kind == 50) {
					goto case 331;
				} else {
					Error(t);
					goto case 334;
				}
			}
			case 333: {
				if (t == null) break;
				Expect(1, t); // EOL
				currentState = 335;
				break;
			}
			case 334: {
				if (t == null) break;
				if (t.kind == 1) {
					goto case 333;
				} else {
					goto case 335;
				}
			}
			case 335: {
				goto case 132; // Expression
			}
			case 336: {
				if (t == null) break;
				if (set[16, t.kind]) {
					goto case 312;
				} else {
					goto case 345;
				}
			}
			case 337: {
				if (t == null) break;
				Expect(70, t); // "Call"
				currentState = 339;
				break;
			}
			case 338: {
				if (t == null) break;
				if (t.kind == 70) {
					goto case 337;
				} else {
					goto case 339;
				}
			}
			case 339: {
				stateStack.Push(344);
				goto case 132; // Expression
			}
			case 340: {
				if (t == null) break;
				Expect(34, t); // "("
				currentState = 342;
				break;
			}
			case 341: {
				stateStack.Push(343);
				goto case 357; // ArgumentList
			}
			case 342: {
				if (t == null) break;
				if (set[16, t.kind]) {
					goto case 341;
				} else {
					goto case 343;
				}
			}
			case 343: {
				if (t == null) break;
				Expect(35, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 344: {
				if (t == null) break;
				if (t.kind == 34) {
					goto case 340;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 345: {
				if (t == null) break;
				if (set[17, t.kind]) {
					goto case 338;
				} else {
					goto case 356;
				}
			}
			case 346: {
				if (t == null) break;
				Expect(132, t); // "If"
				currentState = 347;
				break;
			}
			case 347: {
				stateStack.Push(349);
				goto case 132; // Expression
			}
			case 348: {
				if (t == null) break;
				Expect(210, t); // "Then"
				currentState = 350;
				break;
			}
			case 349: {
				if (t == null) break;
				if (t.kind == 210) {
					goto case 348;
				} else {
					goto case 350;
				}
			}
			case 350: {
				stateStack.Push(354);
				goto case 13; // StatementTerminator
			}
			case 351: {
				goto case 123; // Block
			}
			case 352: {
				stateStack.Push(353);
				goto case 281; // Statement
			}
			case 353: {
				if (t == null) break;
				if (set[11, t.kind]) {
					goto case 352;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 354: {
				if (t == null) break;
				if (t.kind == 1 || t.kind == 20) {
					goto case 351;
				} else {
					goto case 355;
				}
			}
			case 355: {
				if (t == null) break;
				if (set[11, t.kind]) {
					goto case 353;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 356: {
				if (t == null) break;
				if (t.kind == 132) {
					goto case 346;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 357: { // start of ArgumentList
				stateStack.Push(360);
				goto case 132; // Expression
			}
			case 358: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 359;
				break;
			}
			case 359: {
				stateStack.Push(360);
				goto case 132; // Expression
			}
			case 360: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 358;
				} else {
					goto case 370;
				}
			}
			case 361: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 362;
				break;
			}
			case 362: {
				stateStack.Push(363);
				goto case 214; // IdentifierOrKeyword
			}
			case 363: {
				if (t == null) break;
				Expect(52, t); // ":="
				currentState = 364;
				break;
			}
			case 364: {
				stateStack.Push(369);
				goto case 132; // Expression
			}
			case 365: {
				if (t == null) break;
				Expect(21, t); // ","
				currentState = 366;
				break;
			}
			case 366: {
				stateStack.Push(367);
				goto case 214; // IdentifierOrKeyword
			}
			case 367: {
				if (t == null) break;
				Expect(52, t); // ":="
				currentState = 368;
				break;
			}
			case 368: {
				stateStack.Push(369);
				goto case 132; // Expression
			}
			case 369: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 365;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 370: {
				if (t == null) break;
				if (t.kind == 21) {
					goto case 361;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 371: { // start of Identifier
				PushContext(Context.IdentifierExpected);
				goto case 374;
			}
			case 372: {
				stateStack.Push(376);
				goto case 379; // IdentifierForFieldDeclaration
			}
			case 373: {
				if (t == null) break;
				Expect(95, t); // "Custom"
				currentState = 376;
				break;
			}
			case 374: {
				if (t == null) break;
				if (set[18, t.kind]) {
					goto case 372;
				} else {
					goto case 375;
				}
			}
			case 375: {
				if (t == null) break;
				if (t.kind == 95) {
					goto case 373;
				} else {
					Error(t);
					goto case 376;
				}
			}
			case 376: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 377: {
				if (t == null) break;
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 378: {
				if (t == null) break;
				Expect(55, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 379: { // start of IdentifierForFieldDeclaration
				if (t == null) break;
				if (t.kind == 2) {
					goto case 377;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (t == null) break;
				if (t.kind == 55) {
					goto case 378;
				} else {
					goto case 382;
				}
			}
			case 381: {
				if (t == null) break;
				Expect(59, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 382: {
				if (t == null) break;
				if (t.kind == 59) {
					goto case 381;
				} else {
					goto case 384;
				}
			}
			case 383: {
				if (t == null) break;
				Expect(61, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 384: {
				if (t == null) break;
				if (t.kind == 61) {
					goto case 383;
				} else {
					goto case 386;
				}
			}
			case 385: {
				if (t == null) break;
				Expect(62, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 386: {
				if (t == null) break;
				if (t.kind == 62) {
					goto case 385;
				} else {
					goto case 388;
				}
			}
			case 387: {
				if (t == null) break;
				Expect(63, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 388: {
				if (t == null) break;
				if (t.kind == 63) {
					goto case 387;
				} else {
					goto case 390;
				}
			}
			case 389: {
				if (t == null) break;
				Expect(64, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 390: {
				if (t == null) break;
				if (t.kind == 64) {
					goto case 389;
				} else {
					goto case 392;
				}
			}
			case 391: {
				if (t == null) break;
				Expect(67, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 392: {
				if (t == null) break;
				if (t.kind == 67) {
					goto case 391;
				} else {
					goto case 394;
				}
			}
			case 393: {
				if (t == null) break;
				Expect(84, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 394: {
				if (t == null) break;
				if (t.kind == 84) {
					goto case 393;
				} else {
					goto case 396;
				}
			}
			case 395: {
				if (t == null) break;
				Expect(101, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 396: {
				if (t == null) break;
				if (t.kind == 101) {
					goto case 395;
				} else {
					goto case 398;
				}
			}
			case 397: {
				if (t == null) break;
				Expect(104, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 398: {
				if (t == null) break;
				if (t.kind == 104) {
					goto case 397;
				} else {
					goto case 400;
				}
			}
			case 399: {
				if (t == null) break;
				Expect(113, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 400: {
				if (t == null) break;
				if (t.kind == 113) {
					goto case 399;
				} else {
					goto case 402;
				}
			}
			case 401: {
				if (t == null) break;
				Expect(118, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 402: {
				if (t == null) break;
				if (t.kind == 118) {
					goto case 401;
				} else {
					goto case 404;
				}
			}
			case 403: {
				if (t == null) break;
				Expect(123, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 404: {
				if (t == null) break;
				if (t.kind == 123) {
					goto case 403;
				} else {
					goto case 406;
				}
			}
			case 405: {
				if (t == null) break;
				Expect(130, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 406: {
				if (t == null) break;
				if (t.kind == 130) {
					goto case 405;
				} else {
					goto case 408;
				}
			}
			case 407: {
				if (t == null) break;
				Expect(136, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 408: {
				if (t == null) break;
				if (t.kind == 136) {
					goto case 407;
				} else {
					goto case 410;
				}
			}
			case 409: {
				if (t == null) break;
				Expect(140, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 410: {
				if (t == null) break;
				if (t.kind == 140) {
					goto case 409;
				} else {
					goto case 412;
				}
			}
			case 411: {
				if (t == null) break;
				Expect(143, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 412: {
				if (t == null) break;
				if (t.kind == 143) {
					goto case 411;
				} else {
					goto case 414;
				}
			}
			case 413: {
				if (t == null) break;
				Expect(144, t); // "Key"
				currentState = stateStack.Pop();
				break;
			}
			case 414: {
				if (t == null) break;
				if (t.kind == 144) {
					goto case 413;
				} else {
					goto case 416;
				}
			}
			case 415: {
				if (t == null) break;
				Expect(167, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 416: {
				if (t == null) break;
				if (t.kind == 167) {
					goto case 415;
				} else {
					goto case 418;
				}
			}
			case 417: {
				if (t == null) break;
				Expect(173, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 418: {
				if (t == null) break;
				if (t.kind == 173) {
					goto case 417;
				} else {
					goto case 420;
				}
			}
			case 419: {
				if (t == null) break;
				Expect(180, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 420: {
				if (t == null) break;
				if (t.kind == 180) {
					goto case 419;
				} else {
					goto case 422;
				}
			}
			case 421: {
				if (t == null) break;
				Expect(199, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 422: {
				if (t == null) break;
				if (t.kind == 199) {
					goto case 421;
				} else {
					goto case 424;
				}
			}
			case 423: {
				if (t == null) break;
				Expect(208, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 424: {
				if (t == null) break;
				if (t.kind == 208) {
					goto case 423;
				} else {
					goto case 426;
				}
			}
			case 425: {
				if (t == null) break;
				Expect(209, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 426: {
				if (t == null) break;
				if (t.kind == 209) {
					goto case 425;
				} else {
					goto case 428;
				}
			}
			case 427: {
				if (t == null) break;
				Expect(219, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 428: {
				if (t == null) break;
				if (t.kind == 219) {
					goto case 427;
				} else {
					goto case 430;
				}
			}
			case 429: {
				if (t == null) break;
				Expect(220, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 430: {
				if (t == null) break;
				if (t.kind == 220) {
					goto case 429;
				} else {
					goto case 432;
				}
			}
			case 431: {
				if (t == null) break;
				Expect(226, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 432: {
				if (t == null) break;
				if (t.kind == 226) {
					goto case 431;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 433: {
				if (t == null) break;
				Expect(184, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 434: {
				if (t == null) break;
				Expect(122, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 435: { // start of AccessModifier
				if (t == null) break;
				if (t.kind == 184) {
					goto case 433;
				} else {
					goto case 436;
				}
			}
			case 436: {
				if (t == null) break;
				if (t.kind == 122) {
					goto case 434;
				} else {
					goto case 438;
				}
			}
			case 437: {
				if (t == null) break;
				Expect(183, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 438: {
				if (t == null) break;
				if (t.kind == 183) {
					goto case 437;
				} else {
					goto case 440;
				}
			}
			case 439: {
				if (t == null) break;
				Expect(181, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 440: {
				if (t == null) break;
				if (t.kind == 181) {
					goto case 439;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 441: {
				goto case 435; // AccessModifier
			}
			case 442: {
				if (t == null) break;
				Expect(195, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 443: { // start of TypeModifier
				if (t == null) break;
				if (set[19, t.kind]) {
					goto case 441;
				} else {
					goto case 444;
				}
			}
			case 444: {
				if (t == null) break;
				if (t.kind == 195) {
					goto case 442;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 445: {
				goto case 435; // AccessModifier
			}
			case 446: {
				if (t == null) break;
				Expect(195, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 447: { // start of MemberModifier
				if (t == null) break;
				if (set[19, t.kind]) {
					goto case 445;
				} else {
					goto case 448;
				}
			}
			case 448: {
				if (t == null) break;
				if (t.kind == 195) {
					goto case 446;
				} else {
					goto case 450;
				}
			}
			case 449: {
				if (t == null) break;
				Expect(196, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 450: {
				if (t == null) break;
				if (t.kind == 196) {
					goto case 449;
				} else {
					goto case 452;
				}
			}
			case 451: {
				if (t == null) break;
				Expect(176, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 452: {
				if (t == null) break;
				if (t.kind == 176) {
					goto case 451;
				} else {
					goto case 454;
				}
			}
			case 453: {
				if (t == null) break;
				Expect(164, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 454: {
				if (t == null) break;
				if (t.kind == 164) {
					goto case 453;
				} else {
					goto case 456;
				}
			}
			case 455: {
				if (t == null) break;
				Expect(177, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 456: {
				if (t == null) break;
				if (t.kind == 177) {
					goto case 455;
				} else {
					goto case 458;
				}
			}
			case 457: {
				if (t == null) break;
				Expect(175, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 458: {
				if (t == null) break;
				if (t.kind == 175) {
					goto case 457;
				} else {
					goto case 460;
				}
			}
			case 459: {
				if (t == null) break;
				Expect(179, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 460: {
				if (t == null) break;
				if (t.kind == 179) {
					goto case 459;
				} else {
					goto case 462;
				}
			}
			case 461: {
				if (t == null) break;
				Expect(230, t); // "WithEvents"
				currentState = stateStack.Pop();
				break;
			}
			case 462: {
				if (t == null) break;
				if (t.kind == 230) {
					goto case 461;
				} else {
					goto case 464;
				}
			}
			case 463: {
				if (t == null) break;
				Expect(102, t); // "Dim"
				currentState = stateStack.Pop();
				break;
			}
			case 464: {
				if (t == null) break;
				if (t.kind == 102) {
					goto case 463;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 465: {
				if (t == null) break;
				Expect(69, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 466: {
				if (t == null) break;
				Expect(66, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 467: { // start of ParameterModifier
				if (t == null) break;
				if (t.kind == 69) {
					goto case 465;
				} else {
					goto case 468;
				}
			}
			case 468: {
				if (t == null) break;
				if (t.kind == 66) {
					goto case 466;
				} else {
					goto case 470;
				}
			}
			case 469: {
				if (t == null) break;
				Expect(171, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 470: {
				if (t == null) break;
				if (t.kind == 171) {
					goto case 469;
				} else {
					goto case 472;
				}
			}
			case 471: {
				if (t == null) break;
				Expect(178, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 472: {
				if (t == null) break;
				if (t.kind == 178) {
					goto case 471;
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
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,T,x,T, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,T,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,T,x,x, x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};

} // end Parser


}