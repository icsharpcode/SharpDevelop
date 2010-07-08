using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 34;
	const int endOfStatementTerminatorAndBlock = 163;

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfExpression = false;
	bool readXmlIdentifier = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	List<Token> errors = new List<Token>();
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind) {
			Error(la);
			output.AppendLine("expected: " + expectedKind);
			//Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		//Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfExpression = false;
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		wasQualifierTokenAtStart = false;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 452;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 449;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 305;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[0, la.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				if (la.kind == 160) {
					currentState = 445;
					break;
				} else {
					if (set[1, la.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (la == null) { currentState = 7; break; }
				if (la.kind == 40) {
					stateStack.Push(7);
					goto case 305;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[2, la.kind]) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						currentState = 350;
						break;
					} else {
						if (la.kind == 103) {
							currentState = 9;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 10;
					break;
				} else {
					Error(la);
					goto case 10;
				}
			}
			case 10: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 11;
			}
			case 11: {
				if (la == null) { currentState = 11; break; }
				currentState = 12;
				break;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 348;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					currentState = 17;
					break;
				} else {
					goto case 15;
				}
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				stateStack.Push(15);
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[3, la.kind]) {
					goto case 347;
				} else {
					Error(la);
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 37) {
					stateStack.Push(19);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.kind == 26) {
					currentState = 21;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 21: {
				stateStack.Push(22);
				goto case 56;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 37) {
					stateStack.Push(22);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				Expect(37, la); // "("
				currentState = 24;
				break;
			}
			case 24: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 169) {
					goto case 344;
				} else {
					if (set[4, la.kind]) {
						goto case 27;
					} else {
						Error(la);
						goto case 26;
					}
				}
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 27: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[5, la.kind]) {
					stateStack.Push(26);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 29;
				} else {
					goto case 26;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (set[6, la.kind]) {
					goto case 340;
				} else {
					if (la.kind == 22) {
						goto case 30;
					} else {
						goto case 6;
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				currentState = 31;
				break;
			}
			case 31: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[6, la.kind]) {
					stateStack.Push(33);
					goto case 34;
				} else {
					goto case 33;
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 22) {
					goto case 30;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 34: {
				PushContext(Context.Expression, la, t);
				goto case 35;
			}
			case 35: {
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (set[7, la.kind]) {
					currentState = 35;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 37: {
				PushContext(Context.Expression, la, t);
				goto case 38;
			}
			case 38: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[8, la.kind]) {
					currentState = 38;
					break;
				} else {
					if (set[9, la.kind]) {
						stateStack.Push(62);
						goto case 73;
					} else {
						if (la.kind == 220) {
							currentState = 59;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(40);
								goto case 46;
							} else {
								if (la.kind == 35) {
									stateStack.Push(40);
									goto case 41;
								} else {
									Error(la);
									goto case 40;
								}
							}
						}
					}
				}
			}
			case 40: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				Expect(35, la); // "{"
				currentState = 42;
				break;
			}
			case 42: {
				stateStack.Push(43);
				goto case 34;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 22) {
					goto case 45;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				currentState = 42;
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(162, la); // "New"
				currentState = 47;
				break;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[3, la.kind]) {
					stateStack.Push(57);
					goto case 18;
				} else {
					goto case 48;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 233) {
					currentState = 49;
					break;
				} else {
					goto case 6;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(35, la); // "{"
				currentState = 50;
				break;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 147) {
					currentState = 51;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				Expect(26, la); // "."
				currentState = 52;
				break;
			}
			case 52: {
				stateStack.Push(53);
				goto case 56;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				Expect(20, la); // "="
				currentState = 54;
				break;
			}
			case 54: {
				stateStack.Push(55);
				goto case 34;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 22) {
					currentState = 50;
					break;
				} else {
					goto case 44;
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (set[10, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 58;
						break;
					} else {
						goto case 48;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 35) {
					goto case 45;
				} else {
					if (set[11, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 59: {
				stateStack.Push(60);
				goto case 37;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				Expect(144, la); // "Is"
				currentState = 61;
				break;
			}
			case 61: {
				stateStack.Push(40);
				goto case 18;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				if (set[12, la.kind]) {
					stateStack.Push(62);
					goto case 63;
				} else {
					goto case 40;
				}
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 37) {
					currentState = 68;
					break;
				} else {
					if (set[13, la.kind]) {
						currentState = 64;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 64: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 65;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 10) {
					currentState = 66;
					break;
				} else {
					goto case 66;
				}
			}
			case 66: {
				stateStack.Push(67);
				goto case 56;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (la.kind == 11) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 68: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 69;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (la.kind == 169) {
					goto case 70;
				} else {
					if (set[4, la.kind]) {
						goto case 27;
					} else {
						goto case 6;
					}
				}
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				currentState = 71;
				break;
			}
			case 71: {
				stateStack.Push(72);
				goto case 18;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				if (la.kind == 22) {
					goto case 70;
				} else {
					goto case 26;
				}
			}
			case 73: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 74;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				if (set[14, la.kind]) {
					goto case 338;
				} else {
					if (la.kind == 37) {
						currentState = 339;
						break;
					} else {
						if (set[15, la.kind]) {
							goto case 338;
						} else {
							if (set[13, la.kind]) {
								currentState = 334;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 332;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 329;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(75);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 317;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(75);
												goto case 150;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(75);
													PushContext(Context.Query, la, t);
													goto case 89;
												} else {
													if (set[16, la.kind]) {
														stateStack.Push(75);
														goto case 83;
													} else {
														if (la.kind == 135) {
															stateStack.Push(75);
															goto case 76;
														} else {
															Error(la);
															goto case 75;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 75: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				Expect(135, la); // "If"
				currentState = 77;
				break;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(37, la); // "("
				currentState = 78;
				break;
			}
			case 78: {
				stateStack.Push(79);
				goto case 34;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				Expect(22, la); // ","
				currentState = 80;
				break;
			}
			case 80: {
				stateStack.Push(81);
				goto case 34;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (la.kind == 22) {
					currentState = 82;
					break;
				} else {
					goto case 26;
				}
			}
			case 82: {
				stateStack.Push(26);
				goto case 34;
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				if (set[17, la.kind]) {
					currentState = 88;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 84;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				Expect(37, la); // "("
				currentState = 85;
				break;
			}
			case 85: {
				stateStack.Push(86);
				goto case 34;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				Expect(22, la); // ","
				currentState = 87;
				break;
			}
			case 87: {
				stateStack.Push(26);
				goto case 18;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				Expect(37, la); // "("
				currentState = 82;
				break;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				if (la.kind == 126) {
					stateStack.Push(90);
					goto case 149;
				} else {
					if (la.kind == 58) {
						stateStack.Push(90);
						goto case 148;
					} else {
						Error(la);
						goto case 90;
					}
				}
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				if (set[18, la.kind]) {
					stateStack.Push(90);
					goto case 91;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				if (la.kind == 126) {
					goto case 145;
				} else {
					if (la.kind == 58) {
						currentState = 141;
						break;
					} else {
						if (la.kind == 197) {
							goto case 138;
						} else {
							if (la.kind == 107) {
								goto case 16;
							} else {
								if (la.kind == 230) {
									goto case 115;
								} else {
									if (la.kind == 176) {
										currentState = 134;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 132;
											break;
										} else {
											if (la.kind == 148) {
												goto case 129;
											} else {
												if (la.kind == 133) {
													currentState = 104;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 92;
														break;
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
					}
				}
			}
			case 92: {
				stateStack.Push(93);
				goto case 98;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				Expect(171, la); // "On"
				currentState = 94;
				break;
			}
			case 94: {
				stateStack.Push(95);
				goto case 34;
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				Expect(116, la); // "Equals"
				currentState = 96;
				break;
			}
			case 96: {
				stateStack.Push(97);
				goto case 34;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				if (la.kind == 22) {
					currentState = 94;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 98: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(99);
				goto case 103;
			}
			case 99: {
				PopContext();
				goto case 100;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.kind == 63) {
					currentState = 102;
					break;
				} else {
					goto case 101;
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				Expect(138, la); // "In"
				currentState = 34;
				break;
			}
			case 102: {
				stateStack.Push(101);
				goto case 18;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				if (set[19, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 104: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 105;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				if (la.kind == 146) {
					goto case 121;
				} else {
					if (set[20, la.kind]) {
						if (la.kind == 70) {
							goto case 118;
						} else {
							if (set[20, la.kind]) {
								goto case 119;
							} else {
								Error(la);
								goto case 106;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				Expect(70, la); // "By"
				currentState = 107;
				break;
			}
			case 107: {
				stateStack.Push(108);
				goto case 111;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				if (la.kind == 22) {
					goto case 118;
				} else {
					Expect(143, la); // "Into"
					currentState = 109;
					break;
				}
			}
			case 109: {
				stateStack.Push(110);
				goto case 111;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				if (la.kind == 22) {
					currentState = 109;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 111: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 112;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(113);
					goto case 103;
				} else {
					goto case 34;
				}
			}
			case 113: {
				PopContext();
				goto case 114;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				if (la.kind == 63) {
					currentState = 116;
					break;
				} else {
					if (la.kind == 20) {
						goto case 115;
					} else {
						if (set[21, la.kind]) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 34;
						}
					}
				}
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				currentState = 34;
				break;
			}
			case 116: {
				stateStack.Push(117);
				goto case 18;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				Expect(20, la); // "="
				currentState = 34;
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				currentState = 107;
				break;
			}
			case 119: {
				stateStack.Push(120);
				goto case 111;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 22) {
					currentState = 119;
					break;
				} else {
					goto case 106;
				}
			}
			case 121: {
				stateStack.Push(122);
				goto case 128;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 126;
						break;
					} else {
						if (la.kind == 146) {
							goto case 121;
						} else {
							Error(la);
							goto case 122;
						}
					}
				} else {
					goto case 123;
				}
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(143, la); // "Into"
				currentState = 124;
				break;
			}
			case 124: {
				stateStack.Push(125);
				goto case 111;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (la.kind == 22) {
					currentState = 124;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 126: {
				stateStack.Push(127);
				goto case 128;
			}
			case 127: {
				stateStack.Push(122);
				goto case 123;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(146, la); // "Join"
				currentState = 92;
				break;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				currentState = 130;
				break;
			}
			case 130: {
				stateStack.Push(131);
				goto case 111;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (la.kind == 22) {
					goto case 129;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 132: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 133;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 231) {
					goto case 115;
				} else {
					goto case 34;
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				Expect(70, la); // "By"
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 34;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 64 || la.kind == 104) {
					currentState = 137;
					break;
				} else {
					Error(la);
					goto case 137;
				}
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.kind == 22) {
					currentState = 135;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				currentState = 139;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 111;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 22) {
					goto case 138;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 141: {
				stateStack.Push(142);
				goto case 98;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (set[18, la.kind]) {
					stateStack.Push(142);
					goto case 91;
				} else {
					Expect(143, la); // "Into"
					currentState = 143;
					break;
				}
			}
			case 143: {
				stateStack.Push(144);
				goto case 111;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 22) {
					currentState = 143;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				currentState = 146;
				break;
			}
			case 146: {
				stateStack.Push(147);
				goto case 98;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (la.kind == 22) {
					goto case 145;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				Expect(58, la); // "Aggregate"
				currentState = 141;
				break;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(126, la); // "From"
				currentState = 146;
				break;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 210) {
					currentState = 310;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 151;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(37, la); // "("
				currentState = 152;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (set[22, la.kind]) {
					stateStack.Push(153);
					goto case 299;
				} else {
					goto case 153;
				}
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				Expect(38, la); // ")"
				currentState = 154;
				break;
			}
			case 154: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 155;
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 298;
							break;
						} else {
							goto case 156;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 156: {
				stateStack.Push(157);
				goto case 159;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(113, la); // "End"
				currentState = 158;
				break;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 159: {
				PushContext(Context.Body, la, t);
				goto case 160;
			}
			case 160: {
				stateStack.Push(161);
				goto case 15;
			}
			case 161: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 162;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				if (set[23, la.kind]) {
					if (set[24, la.kind]) {
						if (set[25, la.kind]) {
							stateStack.Push(160);
							goto case 167;
						} else {
							goto case 160;
						}
					} else {
						if (la.kind == 113) {
							currentState = 165;
							break;
						} else {
							goto case 164;
						}
					}
				} else {
					goto case 163;
				}
			}
			case 163: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 164: {
				Error(la);
				goto case 161;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 166;
				} else {
					if (set[26, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 164;
					}
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				currentState = 161;
				break;
			}
			case 167: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 168;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 280;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 276;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 274;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 272;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 251;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 236;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 232;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 226;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 201;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 196;
																break;
															} else {
																goto case 196;
															}
														} else {
															if (la.kind == 194) {
																currentState = 195;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															goto case 178;
														} else {
															if (la.kind == 218) {
																currentState = 184;
																break;
															} else {
																if (set[27, la.kind]) {
																	if (la.kind == 132) {
																		currentState = 183;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 182;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 181;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 16;
																				} else {
																					if (la.kind == 195) {
																						goto case 178;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 176;
																		break;
																	} else {
																		if (la.kind == 117) {
																			goto case 173;
																		} else {
																			if (la.kind == 226) {
																				currentState = 169;
																				break;
																			} else {
																				if (set[28, la.kind]) {
																					if (la.kind == 73) {
																						goto case 115;
																					} else {
																						goto case 34;
																					}
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
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 169: {
				stateStack.Push(170);
				goto case 34;
			}
			case 170: {
				stateStack.Push(171);
				goto case 159;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(113, la); // "End"
				currentState = 172;
				break;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				currentState = 174;
				break;
			}
			case 174: {
				stateStack.Push(175);
				goto case 34;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 22) {
					goto case 173;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 176: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 177;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (la.kind == 184) {
					goto case 115;
				} else {
					goto case 34;
				}
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				currentState = 179;
				break;
			}
			case 179: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 180;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (la.kind == 108 || la.kind == 124 || la.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (set[29, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 184: {
				stateStack.Push(185);
				goto case 159;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 75) {
					currentState = 189;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 188;
						break;
					} else {
						goto case 186;
					}
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(113, la); // "End"
				currentState = 187;
				break;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 188: {
				stateStack.Push(186);
				goto case 159;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(192);
					goto case 103;
				} else {
					goto case 190;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 229) {
					currentState = 191;
					break;
				} else {
					goto case 184;
				}
			}
			case 191: {
				stateStack.Push(184);
				goto case 34;
			}
			case 192: {
				PopContext();
				goto case 193;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (la.kind == 63) {
					currentState = 194;
					break;
				} else {
					goto case 190;
				}
			}
			case 194: {
				stateStack.Push(190);
				goto case 18;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (set[31, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				Expect(118, la); // "Error"
				currentState = 197;
				break;
			}
			case 197: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 198;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					if (la.kind == 132) {
						currentState = 200;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 199;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 201: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 202;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[9, la.kind]) {
					stateStack.Push(216);
					goto case 212;
				} else {
					if (la.kind == 110) {
						currentState = 203;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 203: {
				stateStack.Push(204);
				goto case 212;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				Expect(138, la); // "In"
				currentState = 205;
				break;
			}
			case 205: {
				stateStack.Push(206);
				goto case 34;
			}
			case 206: {
				stateStack.Push(207);
				goto case 159;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(163, la); // "Next"
				currentState = 208;
				break;
			}
			case 208: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 209;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (set[6, la.kind]) {
					goto case 210;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				stateStack.Push(211);
				goto case 34;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 22) {
					currentState = 210;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 212: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(213);
				goto case 73;
			}
			case 213: {
				PopContext();
				goto case 214;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 33) {
					currentState = 215;
					break;
				} else {
					goto case 215;
				}
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (set[12, la.kind]) {
					stateStack.Push(215);
					goto case 63;
				} else {
					if (la.kind == 63) {
						currentState = 18;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(20, la); // "="
				currentState = 217;
				break;
			}
			case 217: {
				stateStack.Push(218);
				goto case 34;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (la.kind == 205) {
					currentState = 225;
					break;
				} else {
					goto case 219;
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 159;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(163, la); // "Next"
				currentState = 221;
				break;
			}
			case 221: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 222;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (set[6, la.kind]) {
					goto case 223;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 223: {
				stateStack.Push(224);
				goto case 34;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 22) {
					currentState = 223;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 225: {
				stateStack.Push(219);
				goto case 34;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 229;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(227);
						goto case 159;
					} else {
						goto case 6;
					}
				}
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(152, la); // "Loop"
				currentState = 228;
				break;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (la.kind == 224 || la.kind == 231) {
					goto case 115;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 229: {
				stateStack.Push(230);
				goto case 34;
			}
			case 230: {
				stateStack.Push(231);
				goto case 159;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				stateStack.Push(233);
				goto case 34;
			}
			case 233: {
				stateStack.Push(234);
				goto case 159;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(113, la); // "End"
				currentState = 235;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 74) {
					currentState = 238;
					break;
				} else {
					goto case 238;
				}
			}
			case 238: {
				stateStack.Push(239);
				goto case 34;
			}
			case 239: {
				stateStack.Push(240);
				goto case 15;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 74) {
					currentState = 242;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 241;
					break;
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 242: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 111) {
					currentState = 244;
					break;
				} else {
					if (set[32, la.kind]) {
						goto case 245;
					} else {
						Error(la);
						goto case 244;
					}
				}
			}
			case 244: {
				stateStack.Push(240);
				goto case 159;
			}
			case 245: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 246;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (set[33, la.kind]) {
					if (la.kind == 144) {
						currentState = 248;
						break;
					} else {
						goto case 248;
					}
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(247);
						goto case 34;
					} else {
						Error(la);
						goto case 247;
					}
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 22) {
					currentState = 245;
					break;
				} else {
					goto case 244;
				}
			}
			case 248: {
				stateStack.Push(249);
				goto case 250;
			}
			case 249: {
				stateStack.Push(247);
				goto case 37;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (set[34, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 251: {
				stateStack.Push(252);
				goto case 34;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 214) {
					currentState = 261;
					break;
				} else {
					goto case 253;
				}
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 254;
				} else {
					goto case 6;
				}
			}
			case 254: {
				stateStack.Push(255);
				goto case 159;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 260;
						break;
					} else {
						if (la.kind == 112) {
							goto case 257;
						} else {
							Error(la);
							goto case 254;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 256;
					break;
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				currentState = 258;
				break;
			}
			case 258: {
				stateStack.Push(259);
				goto case 34;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (la.kind == 214) {
					currentState = 254;
					break;
				} else {
					goto case 254;
				}
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (la.kind == 135) {
					goto case 257;
				} else {
					goto case 254;
				}
			}
			case 261: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[25, la.kind]) {
					goto case 263;
				} else {
					goto case 253;
				}
			}
			case 263: {
				stateStack.Push(264);
				goto case 167;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (la.kind == 21) {
					currentState = 270;
					break;
				} else {
					if (la.kind == 111) {
						goto case 266;
					} else {
						goto case 265;
					}
				}
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				currentState = 267;
				break;
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[25, la.kind]) {
					stateStack.Push(269);
					goto case 167;
				} else {
					goto case 269;
				}
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (la.kind == 21) {
					goto case 266;
				} else {
					goto case 265;
				}
			}
			case 270: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 271;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (set[25, la.kind]) {
					goto case 263;
				} else {
					goto case 264;
				}
			}
			case 272: {
				stateStack.Push(273);
				goto case 56;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 37) {
					currentState = 27;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 274: {
				stateStack.Push(275);
				goto case 34;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(22, la); // ","
				currentState = 34;
				break;
			}
			case 276: {
				stateStack.Push(277);
				goto case 34;
			}
			case 277: {
				stateStack.Push(278);
				goto case 159;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				Expect(113, la); // "End"
				currentState = 279;
				break;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (la.kind == 211 || la.kind == 233) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 280: {
				PushContext(Context.IdentifierExpected, la, t);	
				stateStack.Push(281);
				goto case 103;
			}
			case 281: {
				PopContext();
				goto case 282;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (la.kind == 33) {
					currentState = 283;
					break;
				} else {
					goto case 283;
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 37) {
					goto case 296;
				} else {
					goto case 284;
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 22) {
					currentState = 289;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 286;
						break;
					} else {
						goto case 285;
					}
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (la.kind == 20) {
					goto case 115;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 162) {
					goto case 288;
				} else {
					goto case 287;
				}
			}
			case 287: {
				stateStack.Push(285);
				goto case 18;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				currentState = 287;
				break;
			}
			case 289: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(290);
				goto case 103;
			}
			case 290: {
				PopContext();
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 33) {
					currentState = 292;
					break;
				} else {
					goto case 292;
				}
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 37) {
					goto case 293;
				} else {
					goto case 284;
				}
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				currentState = 294;
				break;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 22) {
					goto case 293;
				} else {
					goto case 295;
				}
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				Expect(38, la); // ")"
				currentState = 284;
				break;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				currentState = 297;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 22) {
					goto case 296;
				} else {
					goto case 295;
				}
			}
			case 298: {
				stateStack.Push(156);
				goto case 18;
			}
			case 299: {
				stateStack.Push(300);
				goto case 301;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 22) {
					currentState = 299;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (la.kind == 40) {
					stateStack.Push(301);
					goto case 305;
				} else {
					goto case 302;
				}
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[35, la.kind]) {
					currentState = 302;
					break;
				} else {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(303);
					goto case 103;
				}
			}
			case 303: {
				PopContext();
				goto case 304;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (la.kind == 63) {
					goto case 288;
				} else {
					goto case 285;
				}
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(40, la); // "<"
				currentState = 306;
				break;
			}
			case 306: {
				PushContext(Context.Attribute, la, t);
				goto case 307;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (set[36, la.kind]) {
					currentState = 307;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 308;
					break;
				}
			}
			case 308: {
				PopContext();
				goto case 309;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(37, la); // "("
				currentState = 311;
				break;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (set[22, la.kind]) {
					stateStack.Push(312);
					goto case 299;
				} else {
					goto case 312;
				}
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				Expect(38, la); // ")"
				currentState = 313;
				break;
			}
			case 313: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 314;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (set[25, la.kind]) {
					goto case 167;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(315);
						goto case 159;
					} else {
						goto case 6;
					}
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(113, la); // "End"
				currentState = 316;
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 328;
					break;
				} else {
					stateStack.Push(318);
					goto case 320;
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 17) {
					currentState = 319;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 16) {
					currentState = 318;
					break;
				} else {
					goto case 318;
				}
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 321;
				break;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[37, la.kind]) {
					if (set[38, la.kind]) {
						currentState = 321;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(321);
							goto case 325;
						} else {
							Error(la);
							goto case 321;
						}
					}
				} else {
					if (la.kind == 14) {
						goto case 16;
					} else {
						if (la.kind == 11) {
							goto case 322;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				currentState = 323;
				break;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (set[39, la.kind]) {
					if (set[40, la.kind]) {
						goto case 322;
					} else {
						if (la.kind == 12) {
							stateStack.Push(323);
							goto case 325;
						} else {
							if (la.kind == 10) {
								stateStack.Push(323);
								goto case 320;
							} else {
								Error(la);
								goto case 323;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 324;
					break;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (set[41, la.kind]) {
					if (set[42, la.kind]) {
						currentState = 324;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(324);
							goto case 325;
						} else {
							Error(la);
							goto case 324;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 326;
				break;
			}
			case 326: {
				stateStack.Push(327);
				goto case 34;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 16) {
					currentState = 317;
					break;
				} else {
					goto case 317;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				Expect(37, la); // "("
				currentState = 330;
				break;
			}
			case 330: {
				readXmlIdentifier = true;
				stateStack.Push(331);
				goto case 103;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(38, la); // ")"
				currentState = 75;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				Expect(37, la); // "("
				currentState = 333;
				break;
			}
			case 333: {
				stateStack.Push(331);
				goto case 18;
			}
			case 334: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 335;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 10) {
					currentState = 336;
					break;
				} else {
					goto case 336;
				}
			}
			case 336: {
				stateStack.Push(337);
				goto case 56;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 11) {
					goto case 338;
				} else {
					goto case 75;
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				currentState = 75;
				break;
			}
			case 339: {
				stateStack.Push(331);
				goto case 34;
			}
			case 340: {
				stateStack.Push(341);
				goto case 34;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 22) {
					currentState = 342;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 342: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 343;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (set[6, la.kind]) {
					goto case 340;
				} else {
					goto case 341;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				currentState = 345;
				break;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (set[3, la.kind]) {
					stateStack.Push(346);
					goto case 18;
				} else {
					goto case 346;
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 22) {
					goto case 344;
				} else {
					goto case 26;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				currentState = 19;
				break;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (set[22, la.kind]) {
					stateStack.Push(349);
					goto case 299;
				} else {
					goto case 349;
				}
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 350: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 351;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				currentState = 352;
				break;
			}
			case 352: {
				PopContext();
				goto case 353;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 37) {
					currentState = 433;
					break;
				} else {
					goto case 354;
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (set[43, la.kind]) {
					currentState = 354;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						currentState = 355;
						break;
					} else {
						goto case 355;
					}
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 140) {
					goto case 431;
				} else {
					goto case 356;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 136) {
					goto case 429;
				} else {
					goto case 357;
				}
			}
			case 357: {
				PushContext(Context.Type, la, t);
				goto case 358;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (set[44, la.kind]) {
					stateStack.Push(358);
					PushContext(Context.Member, la, t);
					goto case 362;
				} else {
					Expect(113, la); // "End"
					currentState = 359;
					break;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					currentState = 360;
					break;
				} else {
					Error(la);
					goto case 360;
				}
			}
			case 360: {
				stateStack.Push(361);
				goto case 15;
			}
			case 361: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 40) {
					stateStack.Push(362);
					goto case 305;
				} else {
					goto case 363;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (set[45, la.kind]) {
					currentState = 363;
					break;
				} else {
					if (set[46, la.kind]) {
						stateStack.Push(364);
						goto case 421;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(364);
							goto case 409;
						} else {
							if (la.kind == 101) {
								stateStack.Push(364);
								goto case 400;
							} else {
								if (la.kind == 119) {
									stateStack.Push(364);
									goto case 389;
								} else {
									if (la.kind == 98) {
										stateStack.Push(364);
										goto case 377;
									} else {
										if (la.kind == 172) {
											stateStack.Push(364);
											goto case 365;
										} else {
											Error(la);
											goto case 364;
										}
									}
								}
							}
						}
					}
				}
			}
			case 364: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				Expect(172, la); // "Operator"
				currentState = 366;
				break;
			}
			case 366: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				currentState = 368;
				break;
			}
			case 368: {
				PopContext();
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				Expect(37, la); // "("
				currentState = 370;
				break;
			}
			case 370: {
				stateStack.Push(371);
				goto case 299;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				Expect(38, la); // ")"
				currentState = 372;
				break;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 63) {
					currentState = 376;
					break;
				} else {
					goto case 373;
				}
			}
			case 373: {
				stateStack.Push(374);
				goto case 159;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				Expect(113, la); // "End"
				currentState = 375;
				break;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 40) {
					stateStack.Push(376);
					goto case 305;
				} else {
					stateStack.Push(373);
					goto case 18;
				}
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				Expect(98, la); // "Custom"
				currentState = 378;
				break;
			}
			case 378: {
				stateStack.Push(379);
				goto case 389;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (set[47, la.kind]) {
					goto case 381;
				} else {
					Expect(113, la); // "End"
					currentState = 380;
					break;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (la.kind == 40) {
					stateStack.Push(381);
					goto case 305;
				} else {
					if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
						currentState = 382;
						break;
					} else {
						Error(la);
						goto case 382;
					}
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				Expect(37, la); // "("
				currentState = 383;
				break;
			}
			case 383: {
				stateStack.Push(384);
				goto case 299;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(38, la); // ")"
				currentState = 385;
				break;
			}
			case 385: {
				stateStack.Push(386);
				goto case 159;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(113, la); // "End"
				currentState = 387;
				break;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
					currentState = 388;
					break;
				} else {
					Error(la);
					goto case 388;
				}
			}
			case 388: {
				stateStack.Push(379);
				goto case 15;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(119, la); // "Event"
				currentState = 390;
				break;
			}
			case 390: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(391);
				goto case 103;
			}
			case 391: {
				PopContext();
				goto case 392;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (la.kind == 63) {
					currentState = 399;
					break;
				} else {
					if (set[48, la.kind]) {
						if (la.kind == 37) {
							currentState = 397;
							break;
						} else {
							goto case 393;
						}
					} else {
						Error(la);
						goto case 393;
					}
				}
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 136) {
					goto case 394;
				} else {
					goto case 15;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				currentState = 395;
				break;
			}
			case 395: {
				stateStack.Push(396);
				goto case 18;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 22) {
					goto case 394;
				} else {
					goto case 15;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[22, la.kind]) {
					stateStack.Push(398);
					goto case 299;
				} else {
					goto case 398;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(38, la); // ")"
				currentState = 393;
				break;
			}
			case 399: {
				stateStack.Push(393);
				goto case 18;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				Expect(101, la); // "Declare"
				currentState = 401;
				break;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 402;
					break;
				} else {
					goto case 402;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 403;
					break;
				} else {
					Error(la);
					goto case 403;
				}
			}
			case 403: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(404);
				goto case 103;
			}
			case 404: {
				PopContext();
				goto case 405;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(149, la); // "Lib"
				currentState = 406;
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				Expect(3, la); // LiteralString
				currentState = 407;
				break;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 59) {
					currentState = 408;
					break;
				} else {
					goto case 13;
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 410;
					break;
				} else {
					Error(la);
					goto case 410;
				}
			}
			case 410: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 411;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				currentState = 412;
				break;
			}
			case 412: {
				PopContext();
				goto case 413;
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (la.kind == 37) {
					currentState = 419;
					break;
				} else {
					goto case 414;
				}
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 63) {
					currentState = 418;
					break;
				} else {
					goto case 415;
				}
			}
			case 415: {
				stateStack.Push(416);
				goto case 159;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				Expect(113, la); // "End"
				currentState = 417;
				break;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 15;
					break;
				} else {
					Error(la);
					goto case 15;
				}
			}
			case 418: {
				stateStack.Push(415);
				goto case 18;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (set[22, la.kind]) {
					stateStack.Push(420);
					goto case 299;
				} else {
					goto case 420;
				}
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				Expect(38, la); // ")"
				currentState = 414;
				break;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 88) {
					currentState = 422;
					break;
				} else {
					goto case 422;
				}
			}
			case 422: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(423);
				goto case 428;
			}
			case 423: {
				PopContext();
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (la.kind == 63) {
					currentState = 427;
					break;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 20) {
					currentState = 426;
					break;
				} else {
					goto case 15;
				}
			}
			case 426: {
				stateStack.Push(15);
				goto case 34;
			}
			case 427: {
				stateStack.Push(425);
				goto case 18;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (set[49, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				currentState = 430;
				break;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (set[26, la.kind]) {
					goto case 429;
				} else {
					stateStack.Push(357);
					goto case 15;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				currentState = 432;
				break;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (set[26, la.kind]) {
					goto case 431;
				} else {
					stateStack.Push(356);
					goto case 15;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				Expect(169, la); // "Of"
				currentState = 434;
				break;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 435;
					break;
				} else {
					goto case 435;
				}
			}
			case 435: {
				stateStack.Push(436);
				goto case 444;
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (la.kind == 63) {
					currentState = 438;
					break;
				} else {
					goto case 437;
				}
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 22) {
					currentState = 434;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 354;
					break;
				}
			}
			case 438: {
				stateStack.Push(437);
				goto case 439;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (set[50, la.kind]) {
					goto case 443;
				} else {
					if (la.kind == 35) {
						goto case 440;
					} else {
						goto case 6;
					}
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				currentState = 441;
				break;
			}
			case 441: {
				stateStack.Push(442);
				goto case 443;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 22) {
					goto case 440;
				} else {
					goto case 44;
				}
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (set[3, la.kind]) {
					goto case 347;
				} else {
					if (la.kind == 84 || la.kind == 162 || la.kind == 209) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[51, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 445: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 446;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (set[26, la.kind]) {
					currentState = 446;
					break;
				} else {
					PopContext();
					stateStack.Push(447);
					goto case 15;
				}
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (set[52, la.kind]) {
					stateStack.Push(447);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 448;
					break;
				}
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				Expect(137, la); // "Imports"
				currentState = 450;
				break;
			}
			case 450: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 451;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (set[26, la.kind]) {
					currentState = 451;
					break;
				} else {
					goto case 15;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				Expect(173, la); // "Option"
				currentState = 453;
				break;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[26, la.kind]) {
					currentState = 453;
					break;
				} else {
					goto case 15;
				}
			}
		}

		ApplyToken(la);
		if (la != null) t = la;
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,T,x,x, x,x,T,T, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,x, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,x, x,x,T,T, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,T,T, T,T,T,T, x,x,x,T, x,T,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, T,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,x,T, x,x,x,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,T, T,x,T,T, x,x,T,x, T,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,x,x, T,T,T,T, T,x,T,T, x,T,T,x, x,T,T,x, T,T,x,x, T,T,T,T, T,T,T,T, T,x,T,T, T,x,T,T, T,T,T,x, T,T,x,T, x,T,T,T, x,T,x,x, x,x,T,x, x,x,T,x, x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,x,T, x,x,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,x,T, x,T,x,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, x,T,T,T, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,x,T,T, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,T,T, T,x,x,x, T,x,x,T, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,T,x,T, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,T,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser


}