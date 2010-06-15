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

int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	bool readXmlIdentifier = false;
	
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
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (t == null) { currentState = 1; break; }
				if (t.kind == 172) {
					stateStack.Push(1);
					goto case 209;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 206;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 138;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (t == null) { currentState = 4; break; }
				if (set[0, t.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (t == null) { currentState = 5; break; }
				if (t.kind == 159) {
					goto case 202;
				} else {
					if (set[1, t.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(t);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (t == null) { currentState = 7; break; }
				if (t.kind == 39) {
					stateStack.Push(7);
					goto case 138;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (t == null) { currentState = 8; break; }
				if (set[2, t.kind]) {
					currentState = 8;
					break;
				} else {
					if (t.kind == 83 || t.kind == 154) {
						goto case 201;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 201;
				} else {
					stateStack.Push(10);
					goto case 15;
				}
			}
			case 10: {
				PushContext(Context.Type, t);
				goto case 11;
			}
			case 11: {
				if (t == null) { currentState = 11; break; }
				if (set[4, t.kind]) {
					stateStack.Push(11);
					goto case 17;
				} else {
					Expect(112, t); // "End"
					currentState = 12;
					break;
				}
			}
			case 12: {
				if (t == null) { currentState = 12; break; }
				if (t.kind == 83 || t.kind == 154) {
					currentState = 13;
					break;
				} else {
					Error(t);
					goto case 13;
				}
			}
			case 13: {
				stateStack.Push(14);
				goto case 15;
			}
			case 14: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 15: {
				if (t == null) { currentState = 15; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 16: {
				if (t == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				PushContext(Context.Member, t);
				goto case 18;
			}
			case 18: {
				if (t == null) { currentState = 18; break; }
				if (t.kind == 39) {
					stateStack.Push(18);
					goto case 138;
				} else {
					goto case 19;
				}
			}
			case 19: {
				if (t == null) { currentState = 19; break; }
				if (set[5, t.kind]) {
					currentState = 19;
					break;
				} else {
					if (set[6, t.kind]) {
						stateStack.Push(20);
						goto case 194;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(20);
							goto case 182;
						} else {
							if (t.kind == 100) {
								stateStack.Push(20);
								goto case 171;
							} else {
								if (t.kind == 118) {
									stateStack.Push(20);
									goto case 161;
								} else {
									if (t.kind == 97) {
										stateStack.Push(20);
										goto case 148;
									} else {
										if (t.kind == 171) {
											stateStack.Push(20);
											goto case 21;
										} else {
											Error(t);
											goto case 20;
										}
									}
								}
							}
						}
					}
				}
			}
			case 20: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				Expect(171, t); // "Operator"
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				currentState = 23;
				break;
			}
			case 23: {
				if (t == null) { currentState = 23; break; }
				Expect(36, t); // "("
				currentState = 24;
				break;
			}
			case 24: {
				stateStack.Push(25);
				goto case 143;
			}
			case 25: {
				if (t == null) { currentState = 25; break; }
				Expect(37, t); // ")"
				currentState = 26;
				break;
			}
			case 26: {
				if (t == null) { currentState = 26; break; }
				if (t.kind == 62) {
					currentState = 137;
					break;
				} else {
					goto case 27;
				}
			}
			case 27: {
				if (t == null) { currentState = 27; break; }
				Expect(1, t); // EOL
				currentState = 28;
				break;
			}
			case 28: {
				stateStack.Push(29);
				goto case 32;
			}
			case 29: {
				if (t == null) { currentState = 29; break; }
				Expect(112, t); // "End"
				currentState = 30;
				break;
			}
			case 30: {
				if (t == null) { currentState = 30; break; }
				Expect(171, t); // "Operator"
				currentState = 31;
				break;
			}
			case 31: {
				goto case 15;
			}
			case 32: {
				PushContext(Context.Body, t);
				goto case 33;
			}
			case 33: {
				stateStack.Push(34);
				goto case 15;
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(33);
						goto case 35;
					} else {
						goto case 33;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 120;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						goto case 114;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							goto case 111;
						} else {
							if (t.kind == 187) {
								goto case 108;
							} else {
								if (set[9, t.kind]) {
									goto case 36;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 36: {
				if (t == null) { currentState = 36; break; }
				if (t.kind == 72) {
					goto case 107;
				} else {
					goto case 37;
				}
			}
			case 37: {
				goto case 38;
			}
			case 38: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 39;
			}
			case 39: {
				if (t == null) { currentState = 39; break; }
				if (set[10, t.kind]) {
					goto case 76;
				} else {
					if (t.kind == 161) {
						goto case 40;
					} else {
						goto case 6;
					}
				}
			}
			case 40: {
				if (t == null) { currentState = 40; break; }
				Expect(161, t); // "New"
				currentState = 41;
				break;
			}
			case 41: {
				if (t == null) { currentState = 41; break; }
				if (set[11, t.kind]) {
					stateStack.Push(52);
					goto case 57;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				if (t.kind == 231) {
					currentState = 43;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 43: {
				goto case 44;
			}
			case 44: {
				if (t == null) { currentState = 44; break; }
				Expect(34, t); // "{"
				currentState = 45;
				break;
			}
			case 45: {
				if (t == null) { currentState = 45; break; }
				if (t.kind == 146) {
					currentState = 46;
					break;
				} else {
					goto case 46;
				}
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				Expect(27, t); // "."
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(48);
				goto case 16;
			}
			case 48: {
				if (t == null) { currentState = 48; break; }
				Expect(21, t); // "="
				currentState = 49;
				break;
			}
			case 49: {
				stateStack.Push(50);
				goto case 38;
			}
			case 50: {
				if (t == null) { currentState = 50; break; }
				if (t.kind == 23) {
					currentState = 45;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				if (t.kind == 125) {
					currentState = 53;
					break;
				} else {
					goto case 42;
				}
			}
			case 53: {
				stateStack.Push(42);
				goto case 54;
			}
			case 54: {
				if (t == null) { currentState = 54; break; }
				Expect(34, t); // "{"
				currentState = 55;
				break;
			}
			case 55: {
				if (t == null) { currentState = 55; break; }
				if (set[12, t.kind]) {
					stateStack.Push(56);
					goto case 38;
				} else {
					if (t.kind == 34) {
						stateStack.Push(56);
						goto case 54;
					} else {
						Error(t);
						goto case 56;
					}
				}
			}
			case 56: {
				if (t == null) { currentState = 56; break; }
				if (t.kind == 23) {
					currentState = 55;
					break;
				} else {
					goto case 51;
				}
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (t.kind == 129) {
					goto case 72;
				} else {
					if (set[13, t.kind]) {
						stateStack.Push(58);
						goto case 73;
					} else {
						if (set[14, t.kind]) {
							goto case 72;
						} else {
							Error(t);
							goto case 58;
						}
					}
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				if (t.kind == 36) {
					stateStack.Push(58);
					goto case 62;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (t == null) { currentState = 59; break; }
				if (t.kind == 27) {
					currentState = 60;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 60: {
				stateStack.Push(61);
				goto case 16;
			}
			case 61: {
				if (t == null) { currentState = 61; break; }
				if (t.kind == 36) {
					stateStack.Push(61);
					goto case 62;
				} else {
					goto case 59;
				}
			}
			case 62: {
				if (t == null) { currentState = 62; break; }
				Expect(36, t); // "("
				currentState = 63;
				break;
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				if (t.kind == 168) {
					goto case 69;
				} else {
					if (set[15, t.kind]) {
						goto case 65;
					} else {
						Error(t);
						goto case 64;
					}
				}
			}
			case 64: {
				if (t == null) { currentState = 64; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (t == null) { currentState = 65; break; }
				if (set[12, t.kind]) {
					goto case 66;
				} else {
					goto case 64;
				}
			}
			case 66: {
				stateStack.Push(64);
				goto case 67;
			}
			case 67: {
				stateStack.Push(68);
				goto case 38;
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 23) {
					currentState = 67;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				currentState = 70;
				break;
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				if (set[11, t.kind]) {
					stateStack.Push(71);
					goto case 57;
				} else {
					goto case 71;
				}
			}
			case 71: {
				if (t == null) { currentState = 71; break; }
				if (t.kind == 23) {
					goto case 69;
				} else {
					goto case 64;
				}
			}
			case 72: {
				if (t == null) { currentState = 72; break; }
				currentState = 58;
				break;
			}
			case 73: {
				PushContext(Context.IdentifierExpected, t);
				goto case 74;
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				if (set[13, t.kind]) {
					currentState = 75;
					break;
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
				stateStack.Push(77);
				goto case 78;
			}
			case 77: {
				if (t == null) { currentState = 77; break; }
				if (set[16, t.kind]) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 78: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 79;
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				if (set[17, t.kind]) {
					currentState = 79;
					break;
				} else {
					if (set[18, t.kind]) {
						stateStack.Push(83);
						goto case 92;
					} else {
						if (t.kind == 218) {
							currentState = 80;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 80: {
				stateStack.Push(81);
				goto case 78;
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				Expect(143, t); // "Is"
				currentState = 82;
				break;
			}
			case 82: {
				goto case 57;
			}
			case 83: {
				if (t == null) { currentState = 83; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(83);
					goto case 84;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				if (t.kind == 36) {
					currentState = 87;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 85;
					} else {
						goto case 6;
					}
				}
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				currentState = 86;
				break;
			}
			case 86: {
				goto case 16;
			}
			case 87: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 88;
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				if (t.kind == 168) {
					goto case 89;
				} else {
					if (set[12, t.kind]) {
						goto case 66;
					} else {
						goto case 6;
					}
				}
			}
			case 89: {
				if (t == null) { currentState = 89; break; }
				currentState = 90;
				break;
			}
			case 90: {
				stateStack.Push(91);
				goto case 57;
			}
			case 91: {
				if (t == null) { currentState = 91; break; }
				if (t.kind == 23) {
					goto case 89;
				} else {
					goto case 64;
				}
			}
			case 92: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 93;
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				if (set[19, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						currentState = 106;
						break;
					} else {
						if (set[13, t.kind]) {
							goto case 73;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 85;
							} else {
								if (t.kind == 128) {
									currentState = 104;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 102;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											goto case 94;
										} else {
											goto case 6;
										}
									}
								}
							}
						}
					}
				}
			}
			case 94: {
				PushContext(Context.Xml, t);
				goto case 95;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 17) {
					currentState = 95;
					break;
				} else {
					stateStack.Push(96);
					goto case 97;
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				if (t.kind == 17) {
					currentState = 96;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 97: {
				if (t == null) { currentState = 97; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 98;
				break;
			}
			case 98: {
				if (t == null) { currentState = 98; break; }
				if (set[20, t.kind]) {
					currentState = 98;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 99;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 99: {
				if (t == null) { currentState = 99; break; }
				currentState = 100;
				break;
			}
			case 100: {
				if (t == null) { currentState = 100; break; }
				if (set[21, t.kind]) {
					if (set[22, t.kind]) {
						goto case 99;
					} else {
						if (t.kind == 10) {
							stateStack.Push(100);
							goto case 97;
						} else {
							Error(t);
							goto case 100;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 101;
					break;
				}
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				if (set[23, t.kind]) {
					currentState = 101;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 102: {
				if (t == null) { currentState = 102; break; }
				Expect(36, t); // "("
				currentState = 103;
				break;
			}
			case 103: {
				readXmlIdentifier = true;
				stateStack.Push(64);
				goto case 73;
			}
			case 104: {
				if (t == null) { currentState = 104; break; }
				Expect(36, t); // "("
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(64);
				goto case 57;
			}
			case 106: {
				stateStack.Push(64);
				goto case 38;
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				currentState = 37;
				break;
			}
			case 108: {
				if (t == null) { currentState = 108; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 109;
				break;
			}
			case 109: {
				stateStack.Push(110);
				goto case 16;
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				if (t.kind == 36) {
					currentState = 65;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				if (t.kind == 55 || t.kind == 191) {
					currentState = 112;
					break;
				} else {
					Error(t);
					goto case 112;
				}
			}
			case 112: {
				stateStack.Push(113);
				goto case 38;
			}
			case 113: {
				if (t == null) { currentState = 113; break; }
				Expect(23, t); // ","
				currentState = 37;
				break;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				if (t.kind == 209 || t.kind == 231) {
					currentState = 115;
					break;
				} else {
					Error(t);
					goto case 115;
				}
			}
			case 115: {
				stateStack.Push(116);
				goto case 38;
			}
			case 116: {
				stateStack.Push(117);
				goto case 15;
			}
			case 117: {
				stateStack.Push(118);
				goto case 32;
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				Expect(112, t); // "End"
				currentState = 119;
				break;
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 121;
					break;
				} else {
					Error(t);
					goto case 121;
				}
			}
			case 121: {
				stateStack.Push(122);
				goto case 73;
			}
			case 122: {
				if (t == null) { currentState = 122; break; }
				if (t.kind == 32) {
					currentState = 123;
					break;
				} else {
					goto case 123;
				}
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				if (t.kind == 36) {
					goto case 135;
				} else {
					goto case 124;
				}
			}
			case 124: {
				if (t == null) { currentState = 124; break; }
				if (t.kind == 23) {
					currentState = 129;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 126;
						break;
					} else {
						goto case 125;
					}
				}
			}
			case 125: {
				if (t == null) { currentState = 125; break; }
				if (t.kind == 21) {
					goto case 107;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (t.kind == 161) {
					goto case 128;
				} else {
					goto case 127;
				}
			}
			case 127: {
				stateStack.Push(125);
				goto case 57;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				currentState = 127;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 73;
			}
			case 130: {
				if (t == null) { currentState = 130; break; }
				if (t.kind == 32) {
					currentState = 131;
					break;
				} else {
					goto case 131;
				}
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				if (t.kind == 36) {
					goto case 132;
				} else {
					goto case 124;
				}
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				currentState = 133;
				break;
			}
			case 133: {
				if (t == null) { currentState = 133; break; }
				if (t.kind == 23) {
					goto case 132;
				} else {
					goto case 134;
				}
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				Expect(37, t); // ")"
				currentState = 124;
				break;
			}
			case 135: {
				if (t == null) { currentState = 135; break; }
				currentState = 136;
				break;
			}
			case 136: {
				if (t == null) { currentState = 136; break; }
				if (t.kind == 23) {
					goto case 135;
				} else {
					goto case 134;
				}
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				if (t.kind == 39) {
					stateStack.Push(137);
					goto case 138;
				} else {
					stateStack.Push(27);
					goto case 57;
				}
			}
			case 138: {
				if (t == null) { currentState = 138; break; }
				Expect(39, t); // "<"
				currentState = 139;
				break;
			}
			case 139: {
				PushContext(Context.Attribute, t);
				goto case 140;
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				if (set[24, t.kind]) {
					currentState = 140;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 141;
					break;
				}
			}
			case 141: {
				PopContext();
				goto case 142;
			}
			case 142: {
				if (t == null) { currentState = 142; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 143: {
				stateStack.Push(144);
				goto case 145;
			}
			case 144: {
				if (t == null) { currentState = 144; break; }
				if (t.kind == 23) {
					currentState = 143;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 145: {
				if (t == null) { currentState = 145; break; }
				if (t.kind == 39) {
					stateStack.Push(145);
					goto case 138;
				} else {
					goto case 146;
				}
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (set[25, t.kind]) {
					currentState = 146;
					break;
				} else {
					stateStack.Push(147);
					goto case 73;
				}
			}
			case 147: {
				if (t == null) { currentState = 147; break; }
				if (t.kind == 62) {
					goto case 128;
				} else {
					goto case 125;
				}
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				Expect(97, t); // "Custom"
				currentState = 149;
				break;
			}
			case 149: {
				stateStack.Push(150);
				goto case 161;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				if (set[26, t.kind]) {
					goto case 152;
				} else {
					Expect(112, t); // "End"
					currentState = 151;
					break;
				}
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				Expect(118, t); // "Event"
				currentState = 31;
				break;
			}
			case 152: {
				if (t == null) { currentState = 152; break; }
				if (t.kind == 39) {
					stateStack.Push(152);
					goto case 138;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 153;
						break;
					} else {
						Error(t);
						goto case 153;
					}
				}
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				Expect(36, t); // "("
				currentState = 154;
				break;
			}
			case 154: {
				stateStack.Push(155);
				goto case 143;
			}
			case 155: {
				if (t == null) { currentState = 155; break; }
				Expect(37, t); // ")"
				currentState = 156;
				break;
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				Expect(1, t); // EOL
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(158);
				goto case 32;
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				Expect(112, t); // "End"
				currentState = 159;
				break;
			}
			case 159: {
				if (t == null) { currentState = 159; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 160;
					break;
				} else {
					Error(t);
					goto case 160;
				}
			}
			case 160: {
				stateStack.Push(150);
				goto case 15;
			}
			case 161: {
				if (t == null) { currentState = 161; break; }
				Expect(118, t); // "Event"
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 73;
			}
			case 163: {
				if (t == null) { currentState = 163; break; }
				if (t.kind == 62) {
					currentState = 170;
					break;
				} else {
					if (set[27, t.kind]) {
						if (t.kind == 36) {
							currentState = 168;
							break;
						} else {
							goto case 164;
						}
					} else {
						Error(t);
						goto case 164;
					}
				}
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				if (t.kind == 135) {
					goto case 165;
				} else {
					goto case 31;
				}
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 57;
			}
			case 167: {
				if (t == null) { currentState = 167; break; }
				if (t.kind == 23) {
					goto case 165;
				} else {
					goto case 31;
				}
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				if (set[28, t.kind]) {
					stateStack.Push(169);
					goto case 143;
				} else {
					goto case 169;
				}
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				Expect(37, t); // ")"
				currentState = 164;
				break;
			}
			case 170: {
				stateStack.Push(164);
				goto case 57;
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				Expect(100, t); // "Declare"
				currentState = 172;
				break;
			}
			case 172: {
				if (t == null) { currentState = 172; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 173;
					break;
				} else {
					goto case 173;
				}
			}
			case 173: {
				if (t == null) { currentState = 173; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 174;
					break;
				} else {
					Error(t);
					goto case 174;
				}
			}
			case 174: {
				stateStack.Push(175);
				goto case 73;
			}
			case 175: {
				if (t == null) { currentState = 175; break; }
				Expect(148, t); // "Lib"
				currentState = 176;
				break;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				Expect(3, t); // LiteralString
				currentState = 177;
				break;
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				if (t.kind == 58) {
					currentState = 181;
					break;
				} else {
					goto case 178;
				}
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				if (t.kind == 36) {
					currentState = 179;
					break;
				} else {
					goto case 31;
				}
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				if (set[28, t.kind]) {
					stateStack.Push(180);
					goto case 143;
				} else {
					goto case 180;
				}
			}
			case 180: {
				if (t == null) { currentState = 180; break; }
				Expect(37, t); // ")"
				currentState = 31;
				break;
			}
			case 181: {
				if (t == null) { currentState = 181; break; }
				Expect(3, t); // LiteralString
				currentState = 178;
				break;
			}
			case 182: {
				if (t == null) { currentState = 182; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 183;
					break;
				} else {
					Error(t);
					goto case 183;
				}
			}
			case 183: {
				PushContext(Context.IdentifierExpected, t);
				goto case 184;
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				currentState = 185;
				break;
			}
			case 185: {
				PopContext();
				goto case 186;
			}
			case 186: {
				if (t == null) { currentState = 186; break; }
				if (t.kind == 36) {
					currentState = 192;
					break;
				} else {
					goto case 187;
				}
			}
			case 187: {
				if (t == null) { currentState = 187; break; }
				if (t.kind == 62) {
					currentState = 191;
					break;
				} else {
					goto case 188;
				}
			}
			case 188: {
				stateStack.Push(189);
				goto case 32;
			}
			case 189: {
				if (t == null) { currentState = 189; break; }
				Expect(112, t); // "End"
				currentState = 190;
				break;
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 31;
					break;
				} else {
					Error(t);
					goto case 31;
				}
			}
			case 191: {
				stateStack.Push(188);
				goto case 57;
			}
			case 192: {
				if (t == null) { currentState = 192; break; }
				if (set[28, t.kind]) {
					stateStack.Push(193);
					goto case 143;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				Expect(37, t); // ")"
				currentState = 187;
				break;
			}
			case 194: {
				if (t == null) { currentState = 194; break; }
				if (t.kind == 87) {
					currentState = 195;
					break;
				} else {
					goto case 195;
				}
			}
			case 195: {
				stateStack.Push(196);
				goto case 200;
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (t.kind == 62) {
					currentState = 199;
					break;
				} else {
					goto case 197;
				}
			}
			case 197: {
				if (t == null) { currentState = 197; break; }
				if (t.kind == 21) {
					currentState = 198;
					break;
				} else {
					goto case 31;
				}
			}
			case 198: {
				stateStack.Push(31);
				goto case 38;
			}
			case 199: {
				stateStack.Push(197);
				goto case 57;
			}
			case 200: {
				if (t == null) { currentState = 200; break; }
				if (set[29, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 201: {
				if (t == null) { currentState = 201; break; }
				currentState = 9;
				break;
			}
			case 202: {
				if (t == null) { currentState = 202; break; }
				Expect(159, t); // "Namespace"
				currentState = 203;
				break;
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (set[3, t.kind]) {
					currentState = 203;
					break;
				} else {
					stateStack.Push(204);
					goto case 15;
				}
			}
			case 204: {
				if (t == null) { currentState = 204; break; }
				if (set[30, t.kind]) {
					stateStack.Push(204);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 205;
					break;
				}
			}
			case 205: {
				if (t == null) { currentState = 205; break; }
				Expect(159, t); // "Namespace"
				currentState = 31;
				break;
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				Expect(136, t); // "Imports"
				currentState = 207;
				break;
			}
			case 207: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 208;
			}
			case 208: {
				if (t == null) { currentState = 208; break; }
				if (set[3, t.kind]) {
					currentState = 208;
					break;
				} else {
					goto case 31;
				}
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				Expect(172, t); // "Option"
				currentState = 210;
				break;
			}
			case 210: {
				if (t == null) { currentState = 210; break; }
				if (set[3, t.kind]) {
					currentState = 210;
					break;
				} else {
					goto case 31;
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
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}