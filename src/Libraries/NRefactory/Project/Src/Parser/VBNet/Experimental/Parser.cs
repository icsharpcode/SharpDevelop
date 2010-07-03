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
	const int startOfExpression = 35;
	const int endOfStatementTerminatorAndBlock = 158;

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
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 440;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 437;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 300;
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
					goto case 433;
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
					goto case 300;
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
						currentState = 338;
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
				PushContext(Context.IdentifierExpected, t);
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
					currentState = 336;
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
					goto case 335;
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
				goto case 50;
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
					goto case 332;
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
					goto case 29;
				} else {
					goto case 26;
				}
			}
			case 29: {
				stateStack.Push(26);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 30;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (set[6, la.kind]) {
					goto case 328;
				} else {
					if (la.kind == 22) {
						goto case 31;
					} else {
						goto case 6;
					}
				}
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				currentState = 32;
				break;
			}
			case 32: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 33;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (set[6, la.kind]) {
					stateStack.Push(34);
					goto case 35;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 22) {
					goto case 31;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				PushContext(Context.Expression, t);
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 36;
			}
			case 36: {
				stateStack.Push(37);
				goto case 38;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[7, la.kind]) {
					currentState = 36;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
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
						stateStack.Push(60);
						goto case 71;
					} else {
						if (la.kind == 220) {
							currentState = 58;
							break;
						} else {
							if (la.kind == 162) {
								currentState = 40;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (set[3, la.kind]) {
					stateStack.Push(51);
					goto case 18;
				} else {
					goto case 41;
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 233) {
					currentState = 42;
					break;
				} else {
					goto case 6;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(35, la); // "{"
				currentState = 43;
				break;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 147) {
					currentState = 44;
					break;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(26, la); // "."
				currentState = 45;
				break;
			}
			case 45: {
				stateStack.Push(46);
				goto case 50;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(20, la); // "="
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(48);
				goto case 35;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 22) {
					currentState = 43;
					break;
				} else {
					goto case 49;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (set[10, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 52;
						break;
					} else {
						goto case 41;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.kind == 35) {
					goto case 53;
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
			case 53: {
				if (la == null) { currentState = 53; break; }
				currentState = 54;
				break;
			}
			case 54: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 55;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (set[6, la.kind]) {
					stateStack.Push(56);
					goto case 35;
				} else {
					if (la.kind == 35) {
						stateStack.Push(56);
						goto case 57;
					} else {
						Error(la);
						goto case 56;
					}
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (la.kind == 22) {
					goto case 53;
				} else {
					goto case 49;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				Expect(35, la); // "{"
				currentState = 54;
				break;
			}
			case 58: {
				stateStack.Push(59);
				goto case 38;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				Expect(144, la); // "Is"
				currentState = 18;
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (set[12, la.kind]) {
					stateStack.Push(60);
					goto case 61;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.kind == 37) {
					currentState = 66;
					break;
				} else {
					if (set[13, la.kind]) {
						currentState = 62;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 62: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 63;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 10) {
					currentState = 64;
					break;
				} else {
					goto case 64;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 50;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 11) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 66: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 67;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (la.kind == 169) {
					goto case 68;
				} else {
					if (set[5, la.kind]) {
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = 69;
				break;
			}
			case 69: {
				stateStack.Push(70);
				goto case 18;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 22) {
					goto case 68;
				} else {
					goto case 26;
				}
			}
			case 71: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 72;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				if (set[14, la.kind]) {
					goto case 16;
				} else {
					if (la.kind == 37) {
						goto case 78;
					} else {
						if (set[15, la.kind]) {
							goto case 16;
						} else {
							if (set[13, la.kind]) {
								currentState = 327;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 326;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 324;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 312;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												if (la.kind == 210) {
													currentState = 305;
													break;
												} else {
													if (la.kind == 127) {
														currentState = 146;
														break;
													} else {
														goto case 6;
													}
												}
											} else {
												if (la.kind == 58 || la.kind == 126) {
													PushContext(Context.Query, t);
													goto case 85;
												} else {
													if (set[16, la.kind]) {
														if (set[17, la.kind]) {
															currentState = 84;
															break;
														} else {
															if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
																currentState = 80;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 135) {
															currentState = 73;
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
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				Expect(37, la); // "("
				currentState = 74;
				break;
			}
			case 74: {
				stateStack.Push(75);
				goto case 35;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				Expect(22, la); // ","
				currentState = 76;
				break;
			}
			case 76: {
				stateStack.Push(77);
				goto case 35;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (la.kind == 22) {
					goto case 78;
				} else {
					goto case 26;
				}
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				currentState = 79;
				break;
			}
			case 79: {
				stateStack.Push(26);
				goto case 35;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				Expect(37, la); // "("
				currentState = 81;
				break;
			}
			case 81: {
				stateStack.Push(82);
				goto case 35;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(22, la); // ","
				currentState = 83;
				break;
			}
			case 83: {
				stateStack.Push(26);
				goto case 18;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				Expect(37, la); // "("
				currentState = 79;
				break;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				if (la.kind == 126) {
					stateStack.Push(86);
					goto case 145;
				} else {
					if (la.kind == 58) {
						stateStack.Push(86);
						goto case 144;
					} else {
						Error(la);
						goto case 86;
					}
				}
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				if (set[18, la.kind]) {
					stateStack.Push(86);
					goto case 87;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				if (la.kind == 126) {
					goto case 141;
				} else {
					if (la.kind == 58) {
						currentState = 137;
						break;
					} else {
						if (la.kind == 197) {
							goto case 134;
						} else {
							if (la.kind == 107) {
								goto case 16;
							} else {
								if (la.kind == 230) {
									goto case 111;
								} else {
									if (la.kind == 176) {
										currentState = 130;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 128;
											break;
										} else {
											if (la.kind == 148) {
												goto case 125;
											} else {
												if (la.kind == 133) {
													currentState = 100;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 88;
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
			case 88: {
				stateStack.Push(89);
				goto case 94;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				Expect(171, la); // "On"
				currentState = 90;
				break;
			}
			case 90: {
				stateStack.Push(91);
				goto case 35;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				Expect(116, la); // "Equals"
				currentState = 92;
				break;
			}
			case 92: {
				stateStack.Push(93);
				goto case 35;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				if (la.kind == 22) {
					currentState = 90;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 94: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(95);
				goto case 99;
			}
			case 95: {
				PopContext();
				goto case 96;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				if (la.kind == 63) {
					currentState = 98;
					break;
				} else {
					goto case 97;
				}
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				Expect(138, la); // "In"
				currentState = 35;
				break;
			}
			case 98: {
				stateStack.Push(97);
				goto case 18;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				if (set[19, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 100: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 101;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 146) {
					goto case 117;
				} else {
					if (set[20, la.kind]) {
						if (la.kind == 70) {
							goto case 114;
						} else {
							if (set[20, la.kind]) {
								goto case 115;
							} else {
								Error(la);
								goto case 102;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				Expect(70, la); // "By"
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(104);
				goto case 107;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				if (la.kind == 22) {
					goto case 114;
				} else {
					Expect(143, la); // "Into"
					currentState = 105;
					break;
				}
			}
			case 105: {
				stateStack.Push(106);
				goto case 107;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (la.kind == 22) {
					currentState = 105;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 107: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 108;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(109);
					goto case 99;
				} else {
					goto case 35;
				}
			}
			case 109: {
				PopContext();
				goto case 110;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				if (la.kind == 63) {
					currentState = 112;
					break;
				} else {
					if (la.kind == 20) {
						goto case 111;
					} else {
						if (set[21, la.kind]) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 35;
						}
					}
				}
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = 35;
				break;
			}
			case 112: {
				stateStack.Push(113);
				goto case 18;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				Expect(20, la); // "="
				currentState = 35;
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				currentState = 103;
				break;
			}
			case 115: {
				stateStack.Push(116);
				goto case 107;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				if (la.kind == 22) {
					currentState = 115;
					break;
				} else {
					goto case 102;
				}
			}
			case 117: {
				stateStack.Push(118);
				goto case 124;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 122;
						break;
					} else {
						if (la.kind == 146) {
							goto case 117;
						} else {
							Error(la);
							goto case 118;
						}
					}
				} else {
					goto case 119;
				}
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				Expect(143, la); // "Into"
				currentState = 120;
				break;
			}
			case 120: {
				stateStack.Push(121);
				goto case 107;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 22) {
					currentState = 120;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 122: {
				stateStack.Push(123);
				goto case 124;
			}
			case 123: {
				stateStack.Push(118);
				goto case 119;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				Expect(146, la); // "Join"
				currentState = 88;
				break;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				currentState = 126;
				break;
			}
			case 126: {
				stateStack.Push(127);
				goto case 107;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (la.kind == 22) {
					goto case 125;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 128: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 129;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				if (la.kind == 231) {
					goto case 111;
				} else {
					goto case 35;
				}
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(70, la); // "By"
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(132);
				goto case 35;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 64 || la.kind == 104) {
					currentState = 133;
					break;
				} else {
					Error(la);
					goto case 133;
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 22) {
					currentState = 131;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 107;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 22) {
					goto case 134;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 137: {
				stateStack.Push(138);
				goto case 94;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (set[18, la.kind]) {
					stateStack.Push(138);
					goto case 87;
				} else {
					Expect(143, la); // "Into"
					currentState = 139;
					break;
				}
			}
			case 139: {
				stateStack.Push(140);
				goto case 107;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 22) {
					currentState = 139;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(143);
				goto case 94;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (la.kind == 22) {
					goto case 141;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(58, la); // "Aggregate"
				currentState = 137;
				break;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(126, la); // "From"
				currentState = 142;
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(37, la); // "("
				currentState = 147;
				break;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (set[22, la.kind]) {
					stateStack.Push(148);
					goto case 294;
				} else {
					goto case 148;
				}
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				Expect(38, la); // ")"
				currentState = 149;
				break;
			}
			case 149: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 150;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 293;
							break;
						} else {
							goto case 151;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 151: {
				stateStack.Push(152);
				goto case 154;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				Expect(113, la); // "End"
				currentState = 153;
				break;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 154: {
				PushContext(Context.Body, t);
				goto case 155;
			}
			case 155: {
				stateStack.Push(156);
				goto case 15;
			}
			case 156: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 157;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				if (set[23, la.kind]) {
					if (set[24, la.kind]) {
						if (set[25, la.kind]) {
							stateStack.Push(155);
							goto case 162;
						} else {
							goto case 155;
						}
					} else {
						if (la.kind == 113) {
							currentState = 160;
							break;
						} else {
							goto case 159;
						}
					}
				} else {
					goto case 158;
				}
			}
			case 158: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 159: {
				Error(la);
				goto case 156;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 161;
				} else {
					if (set[26, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 159;
					}
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				currentState = 156;
				break;
			}
			case 162: {
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 275;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 271;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 269;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 267;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 246;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 231;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 227;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 221;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 196;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 191;
																break;
															} else {
																goto case 191;
															}
														} else {
															if (la.kind == 194) {
																currentState = 190;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															goto case 173;
														} else {
															if (la.kind == 218) {
																currentState = 179;
																break;
															} else {
																if (set[27, la.kind]) {
																	if (la.kind == 132) {
																		currentState = 178;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 177;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 176;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 16;
																				} else {
																					if (la.kind == 195) {
																						goto case 173;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 171;
																		break;
																	} else {
																		if (la.kind == 117) {
																			goto case 168;
																		} else {
																			if (la.kind == 226) {
																				currentState = 164;
																				break;
																			} else {
																				if (set[28, la.kind]) {
																					if (la.kind == 73) {
																						goto case 111;
																					} else {
																						goto case 35;
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
			case 164: {
				stateStack.Push(165);
				goto case 35;
			}
			case 165: {
				stateStack.Push(166);
				goto case 154;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(113, la); // "End"
				currentState = 167;
				break;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 35;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 22) {
					goto case 168;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 171: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 172;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 184) {
					goto case 111;
				} else {
					goto case 35;
				}
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				currentState = 174;
				break;
			}
			case 174: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 175;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 108 || la.kind == 124 || la.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (set[29, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 154;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 75) {
					currentState = 184;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 183;
						break;
					} else {
						goto case 181;
					}
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(113, la); // "End"
				currentState = 182;
				break;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 183: {
				stateStack.Push(181);
				goto case 154;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (set[19, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(187);
					goto case 99;
				} else {
					goto case 185;
				}
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 229) {
					currentState = 186;
					break;
				} else {
					goto case 179;
				}
			}
			case 186: {
				stateStack.Push(179);
				goto case 35;
			}
			case 187: {
				PopContext();
				goto case 188;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 63) {
					currentState = 189;
					break;
				} else {
					goto case 185;
				}
			}
			case 189: {
				stateStack.Push(185);
				goto case 18;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (set[31, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				Expect(118, la); // "Error"
				currentState = 192;
				break;
			}
			case 192: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 193;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (set[6, la.kind]) {
					goto case 35;
				} else {
					if (la.kind == 132) {
						currentState = 195;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 194;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (set[30, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 196: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 197;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (set[9, la.kind]) {
					stateStack.Push(211);
					goto case 207;
				} else {
					if (la.kind == 110) {
						currentState = 198;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 198: {
				stateStack.Push(199);
				goto case 207;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(138, la); // "In"
				currentState = 200;
				break;
			}
			case 200: {
				stateStack.Push(201);
				goto case 35;
			}
			case 201: {
				stateStack.Push(202);
				goto case 154;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				Expect(163, la); // "Next"
				currentState = 203;
				break;
			}
			case 203: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 204;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (set[6, la.kind]) {
					goto case 205;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 205: {
				stateStack.Push(206);
				goto case 35;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (la.kind == 22) {
					currentState = 205;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 207: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(208);
				goto case 71;
			}
			case 208: {
				PopContext();
				goto case 209;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 33) {
					currentState = 210;
					break;
				} else {
					goto case 210;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (set[12, la.kind]) {
					stateStack.Push(210);
					goto case 61;
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
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(20, la); // "="
				currentState = 212;
				break;
			}
			case 212: {
				stateStack.Push(213);
				goto case 35;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 205) {
					currentState = 220;
					break;
				} else {
					goto case 214;
				}
			}
			case 214: {
				stateStack.Push(215);
				goto case 154;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				Expect(163, la); // "Next"
				currentState = 216;
				break;
			}
			case 216: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 217;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (set[6, la.kind]) {
					goto case 218;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 218: {
				stateStack.Push(219);
				goto case 35;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 22) {
					currentState = 218;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 220: {
				stateStack.Push(214);
				goto case 35;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 224;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(222);
						goto case 154;
					} else {
						goto case 6;
					}
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(152, la); // "Loop"
				currentState = 223;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 224 || la.kind == 231) {
					goto case 111;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 224: {
				stateStack.Push(225);
				goto case 35;
			}
			case 225: {
				stateStack.Push(226);
				goto case 154;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 227: {
				stateStack.Push(228);
				goto case 35;
			}
			case 228: {
				stateStack.Push(229);
				goto case 154;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				Expect(113, la); // "End"
				currentState = 230;
				break;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 231: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 232;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.kind == 74) {
					currentState = 233;
					break;
				} else {
					goto case 233;
				}
			}
			case 233: {
				stateStack.Push(234);
				goto case 35;
			}
			case 234: {
				stateStack.Push(235);
				goto case 15;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 74) {
					currentState = 237;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 236;
					break;
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 237: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (la.kind == 111) {
					currentState = 239;
					break;
				} else {
					if (set[32, la.kind]) {
						goto case 240;
					} else {
						Error(la);
						goto case 239;
					}
				}
			}
			case 239: {
				stateStack.Push(235);
				goto case 154;
			}
			case 240: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[33, la.kind]) {
					if (la.kind == 144) {
						currentState = 243;
						break;
					} else {
						goto case 243;
					}
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(242);
						goto case 35;
					} else {
						Error(la);
						goto case 242;
					}
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (la.kind == 22) {
					currentState = 240;
					break;
				} else {
					goto case 239;
				}
			}
			case 243: {
				stateStack.Push(244);
				goto case 245;
			}
			case 244: {
				stateStack.Push(242);
				goto case 38;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (set[34, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 246: {
				stateStack.Push(247);
				goto case 35;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 214) {
					currentState = 256;
					break;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 249;
				} else {
					goto case 6;
				}
			}
			case 249: {
				stateStack.Push(250);
				goto case 154;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 255;
						break;
					} else {
						if (la.kind == 112) {
							goto case 252;
						} else {
							Error(la);
							goto case 249;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 251;
					break;
				}
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				currentState = 253;
				break;
			}
			case 253: {
				stateStack.Push(254);
				goto case 35;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 214) {
					currentState = 249;
					break;
				} else {
					goto case 249;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 135) {
					goto case 252;
				} else {
					goto case 249;
				}
			}
			case 256: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (set[25, la.kind]) {
					goto case 258;
				} else {
					goto case 248;
				}
			}
			case 258: {
				stateStack.Push(259);
				goto case 162;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (la.kind == 21) {
					currentState = 265;
					break;
				} else {
					if (la.kind == 111) {
						goto case 261;
					} else {
						goto case 260;
					}
				}
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				currentState = 262;
				break;
			}
			case 262: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 263;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (set[25, la.kind]) {
					stateStack.Push(264);
					goto case 162;
				} else {
					goto case 264;
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (la.kind == 21) {
					goto case 261;
				} else {
					goto case 260;
				}
			}
			case 265: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[25, la.kind]) {
					goto case 258;
				} else {
					goto case 259;
				}
			}
			case 267: {
				stateStack.Push(268);
				goto case 50;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 37) {
					currentState = 27;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				stateStack.Push(270);
				goto case 35;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				Expect(22, la); // ","
				currentState = 35;
				break;
			}
			case 271: {
				stateStack.Push(272);
				goto case 35;
			}
			case 272: {
				stateStack.Push(273);
				goto case 154;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				Expect(113, la); // "End"
				currentState = 274;
				break;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 211 || la.kind == 233) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 275: {
				PushContext(Context.IdentifierExpected, t);
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				stateStack.Push(276);
				goto case 99;
			}
			case 276: {
				PopContext();
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 33) {
					currentState = 278;
					break;
				} else {
					goto case 278;
				}
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 37) {
					goto case 291;
				} else {
					goto case 279;
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (la.kind == 22) {
					currentState = 284;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 281;
						break;
					} else {
						goto case 280;
					}
				}
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 20) {
					goto case 111;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 162) {
					goto case 283;
				} else {
					goto case 282;
				}
			}
			case 282: {
				stateStack.Push(280);
				goto case 18;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				currentState = 282;
				break;
			}
			case 284: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(285);
				goto case 99;
			}
			case 285: {
				PopContext();
				goto case 286;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 33) {
					currentState = 287;
					break;
				} else {
					goto case 287;
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 37) {
					goto case 288;
				} else {
					goto case 279;
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				currentState = 289;
				break;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (la.kind == 22) {
					goto case 288;
				} else {
					goto case 290;
				}
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(38, la); // ")"
				currentState = 279;
				break;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				currentState = 292;
				break;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 22) {
					goto case 291;
				} else {
					goto case 290;
				}
			}
			case 293: {
				stateStack.Push(151);
				goto case 18;
			}
			case 294: {
				stateStack.Push(295);
				goto case 296;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 22) {
					currentState = 294;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 40) {
					stateStack.Push(296);
					goto case 300;
				} else {
					goto case 297;
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (set[35, la.kind]) {
					currentState = 297;
					break;
				} else {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(298);
					goto case 99;
				}
			}
			case 298: {
				PopContext();
				goto case 299;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 63) {
					goto case 283;
				} else {
					goto case 280;
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(40, la); // "<"
				currentState = 301;
				break;
			}
			case 301: {
				PushContext(Context.Attribute, t);
				goto case 302;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[36, la.kind]) {
					currentState = 302;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 303;
					break;
				}
			}
			case 303: {
				PopContext();
				goto case 304;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(37, la); // "("
				currentState = 306;
				break;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (set[22, la.kind]) {
					stateStack.Push(307);
					goto case 294;
				} else {
					goto case 307;
				}
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				Expect(38, la); // ")"
				currentState = 308;
				break;
			}
			case 308: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 309;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (set[25, la.kind]) {
					goto case 162;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(310);
						goto case 154;
					} else {
						goto case 6;
					}
				}
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(113, la); // "End"
				currentState = 311;
				break;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 323;
					break;
				} else {
					stateStack.Push(313);
					goto case 315;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 17) {
					currentState = 314;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 16) {
					currentState = 313;
					break;
				} else {
					goto case 313;
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 316;
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[37, la.kind]) {
					if (set[38, la.kind]) {
						currentState = 316;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(316);
							goto case 320;
						} else {
							Error(la);
							goto case 316;
						}
					}
				} else {
					if (la.kind == 14) {
						goto case 16;
					} else {
						if (la.kind == 11) {
							goto case 317;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				currentState = 318;
				break;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[39, la.kind]) {
					if (set[40, la.kind]) {
						goto case 317;
					} else {
						if (la.kind == 12) {
							stateStack.Push(318);
							goto case 320;
						} else {
							if (la.kind == 10) {
								stateStack.Push(318);
								goto case 315;
							} else {
								Error(la);
								goto case 318;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 319;
					break;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (set[41, la.kind]) {
					if (set[42, la.kind]) {
						currentState = 319;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(319);
							goto case 320;
						} else {
							Error(la);
							goto case 319;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 321;
				break;
			}
			case 321: {
				stateStack.Push(322);
				goto case 35;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 16) {
					currentState = 312;
					break;
				} else {
					goto case 312;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				Expect(37, la); // "("
				currentState = 325;
				break;
			}
			case 325: {
				readXmlIdentifier = true;
				stateStack.Push(26);
				goto case 99;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(37, la); // "("
				currentState = 83;
				break;
			}
			case 327: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 63;
			}
			case 328: {
				stateStack.Push(329);
				goto case 35;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 22) {
					currentState = 330;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 330: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 331;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (set[6, la.kind]) {
					goto case 328;
				} else {
					goto case 329;
				}
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				currentState = 333;
				break;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (set[3, la.kind]) {
					stateStack.Push(334);
					goto case 18;
				} else {
					goto case 334;
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (la.kind == 22) {
					goto case 332;
				} else {
					goto case 26;
				}
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				currentState = 19;
				break;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (set[22, la.kind]) {
					stateStack.Push(337);
					goto case 294;
				} else {
					goto case 337;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 338: {
				PushContext(Context.IdentifierExpected, t);
				goto case 339;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				currentState = 340;
				break;
			}
			case 340: {
				PopContext();
				goto case 341;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 37) {
					currentState = 421;
					break;
				} else {
					goto case 342;
				}
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (set[43, la.kind]) {
					currentState = 342;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						currentState = 343;
						break;
					} else {
						goto case 343;
					}
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 140) {
					goto case 419;
				} else {
					goto case 344;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 136) {
					goto case 417;
				} else {
					goto case 345;
				}
			}
			case 345: {
				PushContext(Context.Type, t);
				goto case 346;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (set[44, la.kind]) {
					stateStack.Push(346);
					PushContext(Context.Member, t);
					goto case 350;
				} else {
					Expect(113, la); // "End"
					currentState = 347;
					break;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					currentState = 348;
					break;
				} else {
					Error(la);
					goto case 348;
				}
			}
			case 348: {
				stateStack.Push(349);
				goto case 15;
			}
			case 349: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 40) {
					stateStack.Push(350);
					goto case 300;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (set[45, la.kind]) {
					currentState = 351;
					break;
				} else {
					if (set[46, la.kind]) {
						stateStack.Push(352);
						goto case 409;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(352);
							goto case 397;
						} else {
							if (la.kind == 101) {
								stateStack.Push(352);
								goto case 388;
							} else {
								if (la.kind == 119) {
									stateStack.Push(352);
									goto case 377;
								} else {
									if (la.kind == 98) {
										stateStack.Push(352);
										goto case 365;
									} else {
										if (la.kind == 172) {
											stateStack.Push(352);
											goto case 353;
										} else {
											Error(la);
											goto case 352;
										}
									}
								}
							}
						}
					}
				}
			}
			case 352: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(172, la); // "Operator"
				currentState = 354;
				break;
			}
			case 354: {
				PushContext(Context.IdentifierExpected, t);
				goto case 355;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				currentState = 356;
				break;
			}
			case 356: {
				PopContext();
				goto case 357;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(37, la); // "("
				currentState = 358;
				break;
			}
			case 358: {
				stateStack.Push(359);
				goto case 294;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				Expect(38, la); // ")"
				currentState = 360;
				break;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 63) {
					currentState = 364;
					break;
				} else {
					goto case 361;
				}
			}
			case 361: {
				stateStack.Push(362);
				goto case 154;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(113, la); // "End"
				currentState = 363;
				break;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 40) {
					stateStack.Push(364);
					goto case 300;
				} else {
					stateStack.Push(361);
					goto case 18;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				Expect(98, la); // "Custom"
				currentState = 366;
				break;
			}
			case 366: {
				stateStack.Push(367);
				goto case 377;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (set[47, la.kind]) {
					goto case 369;
				} else {
					Expect(113, la); // "End"
					currentState = 368;
					break;
				}
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 40) {
					stateStack.Push(369);
					goto case 300;
				} else {
					if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
						currentState = 370;
						break;
					} else {
						Error(la);
						goto case 370;
					}
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				Expect(37, la); // "("
				currentState = 371;
				break;
			}
			case 371: {
				stateStack.Push(372);
				goto case 294;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(38, la); // ")"
				currentState = 373;
				break;
			}
			case 373: {
				stateStack.Push(374);
				goto case 154;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				Expect(113, la); // "End"
				currentState = 375;
				break;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 56 || la.kind == 189 || la.kind == 193) {
					currentState = 376;
					break;
				} else {
					Error(la);
					goto case 376;
				}
			}
			case 376: {
				stateStack.Push(367);
				goto case 15;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				Expect(119, la); // "Event"
				currentState = 378;
				break;
			}
			case 378: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(379);
				goto case 99;
			}
			case 379: {
				PopContext();
				goto case 380;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 63) {
					currentState = 387;
					break;
				} else {
					if (set[48, la.kind]) {
						if (la.kind == 37) {
							currentState = 385;
							break;
						} else {
							goto case 381;
						}
					} else {
						Error(la);
						goto case 381;
					}
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (la.kind == 136) {
					goto case 382;
				} else {
					goto case 15;
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				currentState = 383;
				break;
			}
			case 383: {
				stateStack.Push(384);
				goto case 18;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 22) {
					goto case 382;
				} else {
					goto case 15;
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (set[22, la.kind]) {
					stateStack.Push(386);
					goto case 294;
				} else {
					goto case 386;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(38, la); // ")"
				currentState = 381;
				break;
			}
			case 387: {
				stateStack.Push(381);
				goto case 18;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(101, la); // "Declare"
				currentState = 389;
				break;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 390;
					break;
				} else {
					goto case 390;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 391;
					break;
				} else {
					Error(la);
					goto case 391;
				}
			}
			case 391: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(392);
				goto case 99;
			}
			case 392: {
				PopContext();
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				Expect(149, la); // "Lib"
				currentState = 394;
				break;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				Expect(3, la); // LiteralString
				currentState = 395;
				break;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 59) {
					currentState = 396;
					break;
				} else {
					goto case 13;
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 398;
					break;
				} else {
					Error(la);
					goto case 398;
				}
			}
			case 398: {
				PushContext(Context.IdentifierExpected, t);
				goto case 399;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				currentState = 400;
				break;
			}
			case 400: {
				PopContext();
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 37) {
					currentState = 407;
					break;
				} else {
					goto case 402;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 63) {
					currentState = 406;
					break;
				} else {
					goto case 403;
				}
			}
			case 403: {
				stateStack.Push(404);
				goto case 154;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				Expect(113, la); // "End"
				currentState = 405;
				break;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 127 || la.kind == 210) {
					currentState = 15;
					break;
				} else {
					Error(la);
					goto case 15;
				}
			}
			case 406: {
				stateStack.Push(403);
				goto case 18;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (set[22, la.kind]) {
					stateStack.Push(408);
					goto case 294;
				} else {
					goto case 408;
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(38, la); // ")"
				currentState = 402;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 88) {
					currentState = 410;
					break;
				} else {
					goto case 410;
				}
			}
			case 410: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(411);
				goto case 416;
			}
			case 411: {
				PopContext();
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 63) {
					currentState = 415;
					break;
				} else {
					goto case 413;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (la.kind == 20) {
					currentState = 414;
					break;
				} else {
					goto case 15;
				}
			}
			case 414: {
				stateStack.Push(15);
				goto case 35;
			}
			case 415: {
				stateStack.Push(413);
				goto case 18;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (set[49, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				currentState = 418;
				break;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (set[26, la.kind]) {
					goto case 417;
				} else {
					stateStack.Push(345);
					goto case 15;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				currentState = 420;
				break;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (set[26, la.kind]) {
					goto case 419;
				} else {
					stateStack.Push(344);
					goto case 15;
				}
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(169, la); // "Of"
				currentState = 422;
				break;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 423;
					break;
				} else {
					goto case 423;
				}
			}
			case 423: {
				stateStack.Push(424);
				goto case 432;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (la.kind == 63) {
					currentState = 426;
					break;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 22) {
					currentState = 422;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 342;
					break;
				}
			}
			case 426: {
				stateStack.Push(425);
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (set[50, la.kind]) {
					goto case 431;
				} else {
					if (la.kind == 35) {
						goto case 428;
					} else {
						goto case 6;
					}
				}
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				currentState = 429;
				break;
			}
			case 429: {
				stateStack.Push(430);
				goto case 431;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 22) {
					goto case 428;
				} else {
					goto case 49;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (set[3, la.kind]) {
					goto case 335;
				} else {
					if (la.kind == 84 || la.kind == 162 || la.kind == 209) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (set[51, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				currentState = 434;
				break;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (set[26, la.kind]) {
					goto case 433;
				} else {
					stateStack.Push(435);
					goto case 15;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[52, la.kind]) {
					stateStack.Push(435);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 436;
					break;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				Expect(137, la); // "Imports"
				currentState = 438;
				break;
			}
			case 438: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 439;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (set[26, la.kind]) {
					currentState = 439;
					break;
				} else {
					goto case 15;
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				Expect(173, la); // "Option"
				currentState = 441;
				break;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (set[26, la.kind]) {
					currentState = 441;
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
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
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
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,T,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,T,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, T,T,T,x, T,T,T,x, T,x,T,T, x,T,x,x, T,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,T,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,T,T, T,T,T,T, x,x,x,x, x,T,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, T,T,T,T, x,x,T,x, x,x,x,x, T,T,T,T, T,T,x,T, x,T,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,T,T, x,T,x,x, x,T,x,T, x,x,x,T, x,x,x,T, T,x,T,T, x,x,x,x, x,T,x,x, x,x,T,T, x,x,T,x, T,T,x,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
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