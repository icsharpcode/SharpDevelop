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
	const int endOfStatementTerminatorAndBlock = 164;

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfXmlMode = false;
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
			Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
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
					goto case 453;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 450;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 306;
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
					currentState = 446;
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
					goto case 306;
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
						currentState = 351;
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
					currentState = 349;
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
					goto case 348;
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
				goto case 57;
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
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 169) {
					goto case 345;
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
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[5, la.kind]) {
					stateStack.Push(26);
					nextTokenIsPotentialStartOfXmlMode = true;
					goto case 29;
				} else {
					goto case 26;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (set[6, la.kind]) {
					goto case 341;
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
				nextTokenIsPotentialStartOfXmlMode = true;
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
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[8, la.kind]) {
					currentState = 38;
					break;
				} else {
					if (set[9, la.kind]) {
						stateStack.Push(63);
						goto case 74;
					} else {
						if (la.kind == 220) {
							currentState = 60;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(40);
								goto case 47;
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
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[6, la.kind]) {
					stateStack.Push(44);
					goto case 34;
				} else {
					if (la.kind == 35) {
						stateStack.Push(44);
						goto case 41;
					} else {
						Error(la);
						goto case 44;
					}
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 22) {
					goto case 46;
				} else {
					goto case 45;
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				currentState = 42;
				break;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				Expect(162, la); // "New"
				currentState = 48;
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[3, la.kind]) {
					stateStack.Push(58);
					goto case 18;
				} else {
					goto case 49;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (la.kind == 233) {
					currentState = 50;
					break;
				} else {
					goto case 6;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(35, la); // "{"
				currentState = 51;
				break;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (la.kind == 147) {
					currentState = 52;
					break;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				Expect(26, la); // "."
				currentState = 53;
				break;
			}
			case 53: {
				stateStack.Push(54);
				goto case 57;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				Expect(20, la); // "="
				currentState = 55;
				break;
			}
			case 55: {
				stateStack.Push(56);
				goto case 34;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (la.kind == 22) {
					currentState = 51;
					break;
				} else {
					goto case 45;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (set[10, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 59;
						break;
					} else {
						goto case 49;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				if (la.kind == 35) {
					goto case 46;
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
			case 60: {
				stateStack.Push(61);
				goto case 37;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				Expect(144, la); // "Is"
				currentState = 62;
				break;
			}
			case 62: {
				stateStack.Push(40);
				goto case 18;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (set[12, la.kind]) {
					stateStack.Push(63);
					goto case 64;
				} else {
					goto case 40;
				}
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (la.kind == 37) {
					currentState = 69;
					break;
				} else {
					if (set[13, la.kind]) {
						currentState = 65;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 65: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 66;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				if (la.kind == 10) {
					currentState = 67;
					break;
				} else {
					goto case 67;
				}
			}
			case 67: {
				stateStack.Push(68);
				goto case 57;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 11) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 69: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 70;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 169) {
					goto case 71;
				} else {
					if (set[4, la.kind]) {
						goto case 27;
					} else {
						goto case 6;
					}
				}
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				currentState = 72;
				break;
			}
			case 72: {
				stateStack.Push(73);
				goto case 18;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (la.kind == 22) {
					goto case 71;
				} else {
					goto case 26;
				}
			}
			case 74: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 75;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (set[14, la.kind]) {
					goto case 339;
				} else {
					if (la.kind == 37) {
						currentState = 340;
						break;
					} else {
						if (set[15, la.kind]) {
							goto case 339;
						} else {
							if (set[13, la.kind]) {
								currentState = 335;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 333;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 330;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(76);
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, la, t);
											goto case 318;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(76);
												goto case 151;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(76);
													PushContext(Context.Query, la, t);
													goto case 90;
												} else {
													if (set[16, la.kind]) {
														stateStack.Push(76);
														goto case 84;
													} else {
														if (la.kind == 135) {
															stateStack.Push(76);
															goto case 77;
														} else {
															Error(la);
															goto case 76;
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
			case 76: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(135, la); // "If"
				currentState = 78;
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				Expect(37, la); // "("
				currentState = 79;
				break;
			}
			case 79: {
				stateStack.Push(80);
				goto case 34;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				Expect(22, la); // ","
				currentState = 81;
				break;
			}
			case 81: {
				stateStack.Push(82);
				goto case 34;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				if (la.kind == 22) {
					currentState = 83;
					break;
				} else {
					goto case 26;
				}
			}
			case 83: {
				stateStack.Push(26);
				goto case 34;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (set[17, la.kind]) {
					currentState = 89;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 85;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				Expect(37, la); // "("
				currentState = 86;
				break;
			}
			case 86: {
				stateStack.Push(87);
				goto case 34;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				Expect(22, la); // ","
				currentState = 88;
				break;
			}
			case 88: {
				stateStack.Push(26);
				goto case 18;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				Expect(37, la); // "("
				currentState = 83;
				break;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				if (la.kind == 126) {
					stateStack.Push(91);
					goto case 150;
				} else {
					if (la.kind == 58) {
						stateStack.Push(91);
						goto case 149;
					} else {
						Error(la);
						goto case 91;
					}
				}
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				if (set[18, la.kind]) {
					stateStack.Push(91);
					goto case 92;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				if (la.kind == 126) {
					goto case 146;
				} else {
					if (la.kind == 58) {
						currentState = 142;
						break;
					} else {
						if (la.kind == 197) {
							goto case 139;
						} else {
							if (la.kind == 107) {
								goto case 16;
							} else {
								if (la.kind == 230) {
									goto case 116;
								} else {
									if (la.kind == 176) {
										currentState = 135;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 133;
											break;
										} else {
											if (la.kind == 148) {
												goto case 130;
											} else {
												if (la.kind == 133) {
													currentState = 105;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 93;
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
			case 93: {
				stateStack.Push(94);
				goto case 99;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				Expect(171, la); // "On"
				currentState = 95;
				break;
			}
			case 95: {
				stateStack.Push(96);
				goto case 34;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				Expect(116, la); // "Equals"
				currentState = 97;
				break;
			}
			case 97: {
				stateStack.Push(98);
				goto case 34;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				if (la.kind == 22) {
					currentState = 95;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 99: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(100);
				goto case 104;
			}
			case 100: {
				PopContext();
				goto case 101;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 63) {
					currentState = 103;
					break;
				} else {
					goto case 102;
				}
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				Expect(138, la); // "In"
				currentState = 34;
				break;
			}
			case 103: {
				stateStack.Push(102);
				goto case 18;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				if (set[19, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 105: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 106;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (la.kind == 146) {
					goto case 122;
				} else {
					if (set[20, la.kind]) {
						if (la.kind == 70) {
							goto case 119;
						} else {
							if (set[20, la.kind]) {
								goto case 120;
							} else {
								Error(la);
								goto case 107;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				Expect(70, la); // "By"
				currentState = 108;
				break;
			}
			case 108: {
				stateStack.Push(109);
				goto case 112;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.kind == 22) {
					goto case 119;
				} else {
					Expect(143, la); // "Into"
					currentState = 110;
					break;
				}
			}
			case 110: {
				stateStack.Push(111);
				goto case 112;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				if (la.kind == 22) {
					currentState = 110;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 112: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 113;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(114);
					goto case 104;
				} else {
					goto case 34;
				}
			}
			case 114: {
				PopContext();
				goto case 115;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 63) {
					currentState = 117;
					break;
				} else {
					if (la.kind == 20) {
						goto case 116;
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
			case 116: {
				if (la == null) { currentState = 116; break; }
				currentState = 34;
				break;
			}
			case 117: {
				stateStack.Push(118);
				goto case 18;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				Expect(20, la); // "="
				currentState = 34;
				break;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				currentState = 108;
				break;
			}
			case 120: {
				stateStack.Push(121);
				goto case 112;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 22) {
					currentState = 120;
					break;
				} else {
					goto case 107;
				}
			}
			case 122: {
				stateStack.Push(123);
				goto case 129;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 127;
						break;
					} else {
						if (la.kind == 146) {
							goto case 122;
						} else {
							Error(la);
							goto case 123;
						}
					}
				} else {
					goto case 124;
				}
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				Expect(143, la); // "Into"
				currentState = 125;
				break;
			}
			case 125: {
				stateStack.Push(126);
				goto case 112;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 22) {
					currentState = 125;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 129;
			}
			case 128: {
				stateStack.Push(123);
				goto case 124;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				Expect(146, la); // "Join"
				currentState = 93;
				break;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(132);
				goto case 112;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 22) {
					goto case 130;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 133: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 134;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 231) {
					goto case 116;
				} else {
					goto case 34;
				}
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				Expect(70, la); // "By"
				currentState = 136;
				break;
			}
			case 136: {
				stateStack.Push(137);
				goto case 34;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.kind == 64 || la.kind == 104) {
					currentState = 138;
					break;
				} else {
					Error(la);
					goto case 138;
				}
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 22) {
					currentState = 136;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				currentState = 140;
				break;
			}
			case 140: {
				stateStack.Push(141);
				goto case 112;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				if (la.kind == 22) {
					goto case 139;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 142: {
				stateStack.Push(143);
				goto case 99;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (set[18, la.kind]) {
					stateStack.Push(143);
					goto case 92;
				} else {
					Expect(143, la); // "Into"
					currentState = 144;
					break;
				}
			}
			case 144: {
				stateStack.Push(145);
				goto case 112;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				if (la.kind == 22) {
					currentState = 144;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				currentState = 147;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 99;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 22) {
					goto case 146;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(58, la); // "Aggregate"
				currentState = 142;
				break;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(126, la); // "From"
				currentState = 147;
				break;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (la.kind == 210) {
					currentState = 311;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 152;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				Expect(37, la); // "("
				currentState = 153;
				break;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (set[22, la.kind]) {
					stateStack.Push(154);
					goto case 300;
				} else {
					goto case 154;
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				Expect(38, la); // ")"
				currentState = 155;
				break;
			}
			case 155: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 299;
							break;
						} else {
							goto case 157;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 157: {
				stateStack.Push(158);
				goto case 160;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(113, la); // "End"
				currentState = 159;
				break;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 160: {
				PushContext(Context.Body, la, t);
				goto case 161;
			}
			case 161: {
				stateStack.Push(162);
				goto case 15;
			}
			case 162: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (set[23, la.kind]) {
					if (set[24, la.kind]) {
						if (set[25, la.kind]) {
							stateStack.Push(161);
							goto case 168;
						} else {
							goto case 161;
						}
					} else {
						if (la.kind == 113) {
							currentState = 166;
							break;
						} else {
							goto case 165;
						}
					}
				} else {
					goto case 164;
				}
			}
			case 164: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 165: {
				Error(la);
				goto case 162;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 167;
				} else {
					if (set[26, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 165;
					}
				}
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				currentState = 162;
				break;
			}
			case 168: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 169;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 281;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 277;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 275;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 273;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 252;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 237;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 233;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 227;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 202;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 197;
																break;
															} else {
																goto case 197;
															}
														} else {
															if (la.kind == 194) {
																currentState = 196;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															goto case 179;
														} else {
															if (la.kind == 218) {
																currentState = 185;
																break;
															} else {
																if (set[27, la.kind]) {
																	if (la.kind == 132) {
																		currentState = 184;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 183;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 182;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 16;
																				} else {
																					if (la.kind == 195) {
																						goto case 179;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 177;
																		break;
																	} else {
																		if (la.kind == 117) {
																			goto case 174;
																		} else {
																			if (la.kind == 226) {
																				currentState = 170;
																				break;
																			} else {
																				if (set[28, la.kind]) {
																					if (la.kind == 73) {
																						goto case 116;
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
			case 170: {
				stateStack.Push(171);
				goto case 34;
			}
			case 171: {
				stateStack.Push(172);
				goto case 160;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(113, la); // "End"
				currentState = 173;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				currentState = 175;
				break;
			}
			case 175: {
				stateStack.Push(176);
				goto case 34;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 22) {
					goto case 174;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 177: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 184) {
					goto case 116;
				} else {
					goto case 34;
				}
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				currentState = 180;
				break;
			}
			case 180: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 181;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (la.kind == 108 || la.kind == 124 || la.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[29, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 185: {
				stateStack.Push(186);
				goto case 160;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.kind == 75) {
					currentState = 190;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 189;
						break;
					} else {
						goto case 187;
					}
				}
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				Expect(113, la); // "End"
				currentState = 188;
				break;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				stateStack.Push(187);
				goto case 160;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(193);
					goto case 104;
				} else {
					goto case 191;
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 229) {
					currentState = 192;
					break;
				} else {
					goto case 185;
				}
			}
			case 192: {
				stateStack.Push(185);
				goto case 34;
			}
			case 193: {
				PopContext();
				goto case 194;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 63) {
					currentState = 195;
					break;
				} else {
					goto case 191;
				}
			}
			case 195: {
				stateStack.Push(191);
				goto case 18;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (set[31, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				Expect(118, la); // "Error"
				currentState = 198;
				break;
			}
			case 198: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 199;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				if (set[6, la.kind]) {
					goto case 34;
				} else {
					if (la.kind == 132) {
						currentState = 201;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 200;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 202: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 203;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (set[9, la.kind]) {
					stateStack.Push(217);
					goto case 213;
				} else {
					if (la.kind == 110) {
						currentState = 204;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 213;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				Expect(138, la); // "In"
				currentState = 206;
				break;
			}
			case 206: {
				stateStack.Push(207);
				goto case 34;
			}
			case 207: {
				stateStack.Push(208);
				goto case 160;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(163, la); // "Next"
				currentState = 209;
				break;
			}
			case 209: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 210;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (set[6, la.kind]) {
					goto case 211;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 211: {
				stateStack.Push(212);
				goto case 34;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 22) {
					currentState = 211;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 213: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(214);
				goto case 74;
			}
			case 214: {
				PopContext();
				goto case 215;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 33) {
					currentState = 216;
					break;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[12, la.kind]) {
					stateStack.Push(216);
					goto case 64;
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
			case 217: {
				if (la == null) { currentState = 217; break; }
				Expect(20, la); // "="
				currentState = 218;
				break;
			}
			case 218: {
				stateStack.Push(219);
				goto case 34;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 205) {
					currentState = 226;
					break;
				} else {
					goto case 220;
				}
			}
			case 220: {
				stateStack.Push(221);
				goto case 160;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(163, la); // "Next"
				currentState = 222;
				break;
			}
			case 222: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 223;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (set[6, la.kind]) {
					goto case 224;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 224: {
				stateStack.Push(225);
				goto case 34;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 224;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				stateStack.Push(220);
				goto case 34;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 230;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(228);
						goto case 160;
					} else {
						goto case 6;
					}
				}
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(152, la); // "Loop"
				currentState = 229;
				break;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.kind == 224 || la.kind == 231) {
					goto case 116;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 34;
			}
			case 231: {
				stateStack.Push(232);
				goto case 160;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 233: {
				stateStack.Push(234);
				goto case 34;
			}
			case 234: {
				stateStack.Push(235);
				goto case 160;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(113, la); // "End"
				currentState = 236;
				break;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 237: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (la.kind == 74) {
					currentState = 239;
					break;
				} else {
					goto case 239;
				}
			}
			case 239: {
				stateStack.Push(240);
				goto case 34;
			}
			case 240: {
				stateStack.Push(241);
				goto case 15;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.kind == 74) {
					currentState = 243;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 242;
					break;
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 243: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 244;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 111) {
					currentState = 245;
					break;
				} else {
					if (set[32, la.kind]) {
						goto case 246;
					} else {
						Error(la);
						goto case 245;
					}
				}
			}
			case 245: {
				stateStack.Push(241);
				goto case 160;
			}
			case 246: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 247;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (set[33, la.kind]) {
					if (la.kind == 144) {
						currentState = 249;
						break;
					} else {
						goto case 249;
					}
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(248);
						goto case 34;
					} else {
						Error(la);
						goto case 248;
					}
				}
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 22) {
					currentState = 246;
					break;
				} else {
					goto case 245;
				}
			}
			case 249: {
				stateStack.Push(250);
				goto case 251;
			}
			case 250: {
				stateStack.Push(248);
				goto case 37;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (set[34, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 252: {
				stateStack.Push(253);
				goto case 34;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (la.kind == 214) {
					currentState = 262;
					break;
				} else {
					goto case 254;
				}
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 255;
				} else {
					goto case 6;
				}
			}
			case 255: {
				stateStack.Push(256);
				goto case 160;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 261;
						break;
					} else {
						if (la.kind == 112) {
							goto case 258;
						} else {
							Error(la);
							goto case 255;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 257;
					break;
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				currentState = 259;
				break;
			}
			case 259: {
				stateStack.Push(260);
				goto case 34;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (la.kind == 214) {
					currentState = 255;
					break;
				} else {
					goto case 255;
				}
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (la.kind == 135) {
					goto case 258;
				} else {
					goto case 255;
				}
			}
			case 262: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 263;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (set[25, la.kind]) {
					goto case 264;
				} else {
					goto case 254;
				}
			}
			case 264: {
				stateStack.Push(265);
				goto case 168;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.kind == 21) {
					currentState = 271;
					break;
				} else {
					if (la.kind == 111) {
						goto case 267;
					} else {
						goto case 266;
					}
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				currentState = 268;
				break;
			}
			case 268: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 269;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (set[25, la.kind]) {
					stateStack.Push(270);
					goto case 168;
				} else {
					goto case 270;
				}
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 21) {
					goto case 267;
				} else {
					goto case 266;
				}
			}
			case 271: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 272;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (set[25, la.kind]) {
					goto case 264;
				} else {
					goto case 265;
				}
			}
			case 273: {
				stateStack.Push(274);
				goto case 57;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 37) {
					currentState = 27;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 275: {
				stateStack.Push(276);
				goto case 34;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				Expect(22, la); // ","
				currentState = 34;
				break;
			}
			case 277: {
				stateStack.Push(278);
				goto case 34;
			}
			case 278: {
				stateStack.Push(279);
				goto case 160;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(113, la); // "End"
				currentState = 280;
				break;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 211 || la.kind == 233) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 281: {
				PushContext(Context.IdentifierExpected, la, t);	
				stateStack.Push(282);
				goto case 104;
			}
			case 282: {
				PopContext();
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 33) {
					currentState = 284;
					break;
				} else {
					goto case 284;
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 37) {
					goto case 297;
				} else {
					goto case 285;
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (la.kind == 22) {
					currentState = 290;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 287;
						break;
					} else {
						goto case 286;
					}
				}
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 20) {
					goto case 116;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 162) {
					goto case 289;
				} else {
					goto case 288;
				}
			}
			case 288: {
				stateStack.Push(286);
				goto case 18;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				currentState = 288;
				break;
			}
			case 290: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(291);
				goto case 104;
			}
			case 291: {
				PopContext();
				goto case 292;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 33) {
					currentState = 293;
					break;
				} else {
					goto case 293;
				}
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 37) {
					goto case 294;
				} else {
					goto case 285;
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				currentState = 295;
				break;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 22) {
					goto case 294;
				} else {
					goto case 296;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(38, la); // ")"
				currentState = 285;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				currentState = 298;
				break;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 22) {
					goto case 297;
				} else {
					goto case 296;
				}
			}
			case 299: {
				stateStack.Push(157);
				goto case 18;
			}
			case 300: {
				stateStack.Push(301);
				goto case 302;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (la.kind == 22) {
					currentState = 300;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (la.kind == 40) {
					stateStack.Push(302);
					goto case 306;
				} else {
					goto case 303;
				}
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (set[35, la.kind]) {
					currentState = 303;
					break;
				} else {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(304);
					goto case 104;
				}
			}
			case 304: {
				PopContext();
				goto case 305;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 63) {
					goto case 289;
				} else {
					goto case 286;
				}
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(40, la); // "<"
				currentState = 307;
				break;
			}
			case 307: {
				PushContext(Context.Attribute, la, t);
				goto case 308;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (set[36, la.kind]) {
					currentState = 308;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 309;
					break;
				}
			}
			case 309: {
				PopContext();
				goto case 310;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(37, la); // "("
				currentState = 312;
				break;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[22, la.kind]) {
					stateStack.Push(313);
					goto case 300;
				} else {
					goto case 313;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(38, la); // ")"
				currentState = 314;
				break;
			}
			case 314: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 315;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (set[25, la.kind]) {
					goto case 168;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(316);
						goto case 160;
					} else {
						goto case 6;
					}
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(113, la); // "End"
				currentState = 317;
				break;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 329;
					break;
				} else {
					stateStack.Push(319);
					goto case 321;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 17) {
					currentState = 320;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 16) {
					currentState = 319;
					break;
				} else {
					goto case 319;
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 322;
				break;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (set[37, la.kind]) {
					if (set[38, la.kind]) {
						currentState = 322;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(322);
							goto case 326;
						} else {
							Error(la);
							goto case 322;
						}
					}
				} else {
					if (la.kind == 14) {
						goto case 16;
					} else {
						if (la.kind == 11) {
							goto case 323;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				currentState = 324;
				break;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (set[39, la.kind]) {
					if (set[40, la.kind]) {
						goto case 323;
					} else {
						if (la.kind == 12) {
							stateStack.Push(324);
							goto case 326;
						} else {
							if (la.kind == 10) {
								stateStack.Push(324);
								goto case 321;
							} else {
								Error(la);
								goto case 324;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 325;
					break;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (set[41, la.kind]) {
					if (set[42, la.kind]) {
						currentState = 325;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(325);
							goto case 326;
						} else {
							Error(la);
							goto case 325;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 327;
				break;
			}
			case 327: {
				stateStack.Push(328);
				goto case 34;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 16) {
					currentState = 318;
					break;
				} else {
					goto case 318;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(37, la); // "("
				currentState = 331;
				break;
			}
			case 331: {
				readXmlIdentifier = true;
				stateStack.Push(332);
				goto case 104;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				Expect(38, la); // ")"
				currentState = 76;
				break;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(37, la); // "("
				currentState = 334;
				break;
			}
			case 334: {
				stateStack.Push(332);
				goto case 18;
			}
			case 335: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 10) {
					currentState = 337;
					break;
				} else {
					goto case 337;
				}
			}
			case 337: {
				stateStack.Push(338);
				goto case 57;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 11) {
					goto case 339;
				} else {
					goto case 76;
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				currentState = 76;
				break;
			}
			case 340: {
				stateStack.Push(332);
				goto case 34;
			}
			case 341: {
				stateStack.Push(342);
				goto case 34;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 22) {
					currentState = 343;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 343: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 344;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (set[6, la.kind]) {
					goto case 341;
				} else {
					goto case 342;
				}
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				currentState = 346;
				break;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (set[3, la.kind]) {
					stateStack.Push(347);
					goto case 18;
				} else {
					goto case 347;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 22) {
					goto case 345;
				} else {
					goto case 26;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				currentState = 19;
				break;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[22, la.kind]) {
					stateStack.Push(350);
					goto case 300;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 351: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 352;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				currentState = 353;
				break;
			}
			case 353: {
				PopContext();
				goto case 354;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 37) {
					currentState = 434;
					break;
				} else {
					goto case 355;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (set[43, la.kind]) {
					currentState = 355;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						currentState = 356;
						break;
					} else {
						goto case 356;
					}
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 140) {
					goto case 432;
				} else {
					goto case 357;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 136) {
					goto case 430;
				} else {
					goto case 358;
				}
			}
			case 358: {
				PushContext(Context.Type, la, t);
				goto case 359;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (set[44, la.kind]) {
					stateStack.Push(359);
					PushContext(Context.Member, la, t);
					goto case 363;
				} else {
					Expect(113, la); // "End"
					currentState = 360;
					break;
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					currentState = 361;
					break;
				} else {
					Error(la);
					goto case 361;
				}
			}
			case 361: {
				stateStack.Push(362);
				goto case 15;
			}
			case 362: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 40) {
					stateStack.Push(363);
					goto case 306;
				} else {
					goto case 364;
				}
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (set[45, la.kind]) {
					currentState = 364;
					break;
				} else {
					if (set[46, la.kind]) {
						stateStack.Push(365);
						goto case 422;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(365);
							goto case 410;
						} else {
							if (la.kind == 101) {
								stateStack.Push(365);
								goto case 401;
							} else {
								if (la.kind == 119) {
									stateStack.Push(365);
									goto case 390;
								} else {
									if (la.kind == 98) {
										stateStack.Push(365);
										goto case 378;
									} else {
										if (la.kind == 172) {
											stateStack.Push(365);
											goto case 366;
										} else {
											Error(la);
											goto case 365;
										}
									}
								}
							}
						}
					}
				}
			}
			case 365: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				Expect(172, la); // "Operator"
				currentState = 367;
				break;
			}
			case 367: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 368;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				currentState = 369;
				break;
			}
			case 369: {
				PopContext();
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				Expect(37, la); // "("
				currentState = 371;
				break;
			}
			case 371: {
				stateStack.Push(372);
				goto case 300;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(38, la); // ")"
				currentState = 373;
				break;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 63) {
					currentState = 377;
					break;
				} else {
					goto case 374;
				}
			}
			case 374: {
				stateStack.Push(375);
				goto case 160;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				Expect(113, la); // "End"
				currentState = 376;
				break;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 40) {
					stateStack.Push(377);
					goto case 306;
				} else {
					stateStack.Push(374);
					goto case 18;
				}
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				Expect(98, la); // "Custom"
				currentState = 379;
				break;
			}
			case 379: {
				stateStack.Push(380);
				goto case 390;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (set[47, la.kind]) {
					goto case 382;
				} else {
					Expect(113, la); // "End"
					currentState = 381;
					break;
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 40) {
					stateStack.Push(382);
					goto case 306;
				} else {
					if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
						currentState = 383;
						break;
					} else {
						Error(la);
						goto case 383;
					}
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(37, la); // "("
				currentState = 384;
				break;
			}
			case 384: {
				stateStack.Push(385);
				goto case 300;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				Expect(38, la); // ")"
				currentState = 386;
				break;
			}
			case 386: {
				stateStack.Push(387);
				goto case 160;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				Expect(113, la); // "End"
				currentState = 388;
				break;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
					currentState = 389;
					break;
				} else {
					Error(la);
					goto case 389;
				}
			}
			case 389: {
				stateStack.Push(380);
				goto case 15;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(119, la); // "Event"
				currentState = 391;
				break;
			}
			case 391: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(392);
				goto case 104;
			}
			case 392: {
				PopContext();
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 63) {
					currentState = 400;
					break;
				} else {
					if (set[48, la.kind]) {
						if (la.kind == 37) {
							currentState = 398;
							break;
						} else {
							goto case 394;
						}
					} else {
						Error(la);
						goto case 394;
					}
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 136) {
					goto case 395;
				} else {
					goto case 15;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				currentState = 396;
				break;
			}
			case 396: {
				stateStack.Push(397);
				goto case 18;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (la.kind == 22) {
					goto case 395;
				} else {
					goto case 15;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (set[22, la.kind]) {
					stateStack.Push(399);
					goto case 300;
				} else {
					goto case 399;
				}
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(38, la); // ")"
				currentState = 394;
				break;
			}
			case 400: {
				stateStack.Push(394);
				goto case 18;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(101, la); // "Declare"
				currentState = 402;
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 403;
					break;
				} else {
					goto case 403;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 404;
					break;
				} else {
					Error(la);
					goto case 404;
				}
			}
			case 404: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(405);
				goto case 104;
			}
			case 405: {
				PopContext();
				goto case 406;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				Expect(149, la); // "Lib"
				currentState = 407;
				break;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(3, la); // LiteralString
				currentState = 408;
				break;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (la.kind == 59) {
					currentState = 409;
					break;
				} else {
					goto case 13;
				}
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 411;
					break;
				} else {
					Error(la);
					goto case 411;
				}
			}
			case 411: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				currentState = 413;
				break;
			}
			case 413: {
				PopContext();
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 37) {
					currentState = 420;
					break;
				} else {
					goto case 415;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 63) {
					currentState = 419;
					break;
				} else {
					goto case 416;
				}
			}
			case 416: {
				stateStack.Push(417);
				goto case 160;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(113, la); // "End"
				currentState = 418;
				break;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 15;
					break;
				} else {
					Error(la);
					goto case 15;
				}
			}
			case 419: {
				stateStack.Push(416);
				goto case 18;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (set[22, la.kind]) {
					stateStack.Push(421);
					goto case 300;
				} else {
					goto case 421;
				}
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(38, la); // ")"
				currentState = 415;
				break;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 88) {
					currentState = 423;
					break;
				} else {
					goto case 423;
				}
			}
			case 423: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(424);
				goto case 429;
			}
			case 424: {
				PopContext();
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 63) {
					currentState = 428;
					break;
				} else {
					goto case 426;
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 20) {
					currentState = 427;
					break;
				} else {
					goto case 15;
				}
			}
			case 427: {
				stateStack.Push(15);
				goto case 34;
			}
			case 428: {
				stateStack.Push(426);
				goto case 18;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[49, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				currentState = 431;
				break;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (set[26, la.kind]) {
					goto case 430;
				} else {
					stateStack.Push(358);
					goto case 15;
				}
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				currentState = 433;
				break;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (set[26, la.kind]) {
					goto case 432;
				} else {
					stateStack.Push(357);
					goto case 15;
				}
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				Expect(169, la); // "Of"
				currentState = 435;
				break;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 436;
					break;
				} else {
					goto case 436;
				}
			}
			case 436: {
				stateStack.Push(437);
				goto case 445;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 63) {
					currentState = 439;
					break;
				} else {
					goto case 438;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 22) {
					currentState = 435;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 355;
					break;
				}
			}
			case 439: {
				stateStack.Push(438);
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (set[50, la.kind]) {
					goto case 444;
				} else {
					if (la.kind == 35) {
						goto case 441;
					} else {
						goto case 6;
					}
				}
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				currentState = 442;
				break;
			}
			case 442: {
				stateStack.Push(443);
				goto case 444;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 22) {
					goto case 441;
				} else {
					goto case 45;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[3, la.kind]) {
					goto case 348;
				} else {
					if (la.kind == 84 || la.kind == 162 || la.kind == 209) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (set[51, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 446: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 447;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (set[26, la.kind]) {
					currentState = 447;
					break;
				} else {
					PopContext();
					stateStack.Push(448);
					goto case 15;
				}
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (set[52, la.kind]) {
					stateStack.Push(448);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 449;
					break;
				}
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				Expect(137, la); // "Imports"
				currentState = 451;
				break;
			}
			case 451: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 452;
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (set[26, la.kind]) {
					currentState = 452;
					break;
				} else {
					goto case 15;
				}
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				Expect(173, la); // "Option"
				currentState = 454;
				break;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (set[26, la.kind]) {
					currentState = 454;
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
		Console.WriteLine("Advance");
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