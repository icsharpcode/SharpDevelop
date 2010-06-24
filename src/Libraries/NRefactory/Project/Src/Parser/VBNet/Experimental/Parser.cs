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

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind)
			Error(la);
	}
	
	void Error(Token la) 
	{
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		const int endOfStatementTerminatorAndBlock = 33;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 172) {
					stateStack.Push(1);
					goto case 388;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 136) {
					stateStack.Push(2);
					goto case 385;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 39) {
					stateStack.Push(3);
					goto case 183;
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
				if (la.kind == 159) {
					goto case 381;
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
				if (la.kind == 39) {
					stateStack.Push(7);
					goto case 183;
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
					if (la.kind == 83 || la.kind == 154) {
						goto case 380;
					} else {
						Error(la);
						goto case 9;
					}
				}
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (set[3, la.kind]) {
					goto case 380;
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
				if (la == null) { currentState = 11; break; }
				if (set[4, la.kind]) {
					stateStack.Push(11);
					PushContext(Context.Member, t);
					goto case 17;
				} else {
					Expect(112, la); // "End"
					currentState = 12;
					break;
				}
			}
			case 12: {
				if (la == null) { currentState = 12; break; }
				if (la.kind == 83 || la.kind == 154) {
					currentState = 13;
					break;
				} else {
					Error(la);
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
				if (la == null) { currentState = 15; break; }
				if (la.kind == 1 || la.kind == 22) {
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
				if (la == null) { currentState = 17; break; }
				if (la.kind == 39) {
					stateStack.Push(17);
					goto case 183;
				} else {
					goto case 18;
				}
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[5, la.kind]) {
					currentState = 18;
					break;
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(19);
						goto case 373;
					} else {
						if (la.kind == 126 || la.kind == 208) {
							stateStack.Push(19);
							goto case 361;
						} else {
							if (la.kind == 100) {
								stateStack.Push(19);
								goto case 350;
							} else {
								if (la.kind == 118) {
									stateStack.Push(19);
									goto case 340;
								} else {
									if (la.kind == 97) {
										stateStack.Push(19);
										goto case 328;
									} else {
										if (la.kind == 171) {
											stateStack.Push(19);
											goto case 20;
										} else {
											Error(la);
											goto case 19;
										}
									}
								}
							}
						}
					}
				}
			}
			case 19: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(171, la); // "Operator"
				currentState = 21;
				break;
			}
			case 21: {
				if (la == null) { currentState = 21; break; }
				currentState = 22;
				break;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				Expect(36, la); // "("
				currentState = 23;
				break;
			}
			case 23: {
				stateStack.Push(24);
				goto case 175;
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				Expect(37, la); // ")"
				currentState = 25;
				break;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 62) {
					currentState = 327;
					break;
				} else {
					goto case 26;
				}
			}
			case 26: {
				stateStack.Push(27);
				goto case 29;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(112, la); // "End"
				currentState = 28;
				break;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				Expect(171, la); // "Operator"
				currentState = 15;
				break;
			}
			case 29: {
				PushContext(Context.Body, t);
				goto case 30;
			}
			case 30: {
				stateStack.Push(31);
				goto case 15;
			}
			case 31: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[7, la.kind]) {
					if (set[8, la.kind]) {
						if (set[9, la.kind]) {
							stateStack.Push(30);
							goto case 37;
						} else {
							goto case 30;
						}
					} else {
						if (la.kind == 112) {
							currentState = 35;
							break;
						} else {
							goto case 34;
						}
					}
				} else {
					goto case 33;
				}
			}
			case 33: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 34: {
				Error(la);
				goto case 31;
			}
			case 35: {
				if (la == null) { currentState = 35; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 36;
				} else {
					if (set[3, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 34;
					}
				}
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				currentState = 31;
				break;
			}
			case 37: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 38;
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 87 || la.kind == 104 || la.kind == 202) {
					currentState = 314;
					break;
				} else {
					if (la.kind == 209 || la.kind == 231) {
						currentState = 310;
						break;
					} else {
						if (la.kind == 55 || la.kind == 191) {
							currentState = 308;
							break;
						} else {
							if (la.kind == 187) {
								currentState = 306;
								break;
							} else {
								if (la.kind == 134) {
									currentState = 285;
									break;
								} else {
									if (la.kind == 195) {
										currentState = 270;
										break;
									} else {
										if (la.kind == 229) {
											currentState = 266;
											break;
										} else {
											if (la.kind == 107) {
												currentState = 260;
												break;
											} else {
												if (la.kind == 123) {
													currentState = 236;
													break;
												} else {
													if (la.kind == 117 || la.kind == 170 || la.kind == 192) {
														if (la.kind == 117 || la.kind == 170) {
															if (la.kind == 170) {
																currentState = 231;
																break;
															} else {
																goto case 231;
															}
														} else {
															if (la.kind == 192) {
																currentState = 230;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 213) {
															goto case 214;
														} else {
															if (la.kind == 216) {
																currentState = 220;
																break;
															} else {
																if (set[10, la.kind]) {
																	if (la.kind == 131) {
																		currentState = 219;
																		break;
																	} else {
																		if (la.kind == 119) {
																			currentState = 218;
																			break;
																		} else {
																			if (la.kind == 88) {
																				currentState = 217;
																				break;
																			} else {
																				if (la.kind == 204) {
																					goto case 16;
																				} else {
																					if (la.kind == 193) {
																						goto case 214;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 189) {
																		currentState = 212;
																		break;
																	} else {
																		if (la.kind == 116) {
																			goto case 209;
																		} else {
																			if (la.kind == 224) {
																				currentState = 205;
																				break;
																			} else {
																				if (set[11, la.kind]) {
																					if (la.kind == 72) {
																						goto case 41;
																					} else {
																						goto case 39;
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
			case 39: {
				stateStack.Push(40);
				goto case 42;
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (set[12, la.kind]) {
					goto case 41;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				currentState = 39;
				break;
			}
			case 42: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[13, la.kind]) {
					currentState = 42;
					break;
				} else {
					if (set[14, la.kind]) {
						stateStack.Push(87);
						goto case 95;
					} else {
						if (la.kind == 218) {
							currentState = 85;
							break;
						} else {
							if (la.kind == 161) {
								currentState = 44;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (set[15, la.kind]) {
					stateStack.Push(54);
					goto case 61;
				} else {
					goto case 45;
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				if (la.kind == 231) {
					currentState = 46;
					break;
				} else {
					goto case 6;
				}
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(34, la); // "{"
				currentState = 47;
				break;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (la.kind == 146) {
					currentState = 48;
					break;
				} else {
					goto case 48;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				Expect(27, la); // "."
				currentState = 49;
				break;
			}
			case 49: {
				stateStack.Push(50);
				goto case 16;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(21, la); // "="
				currentState = 51;
				break;
			}
			case 51: {
				stateStack.Push(52);
				goto case 39;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.kind == 23) {
					currentState = 47;
					break;
				} else {
					goto case 53;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				Expect(35, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (la.kind == 125 || la.kind == 231) {
					if (la.kind == 125) {
						currentState = 55;
						break;
					} else {
						goto case 45;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 34) {
					goto case 56;
				} else {
					if (set[16, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				currentState = 57;
				break;
			}
			case 57: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 58;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (set[17, la.kind]) {
					stateStack.Push(59);
					goto case 39;
				} else {
					if (la.kind == 34) {
						stateStack.Push(59);
						goto case 60;
					} else {
						Error(la);
						goto case 59;
					}
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				if (la.kind == 23) {
					goto case 56;
				} else {
					goto case 53;
				}
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				Expect(34, la); // "{"
				currentState = 57;
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (set[15, la.kind]) {
					currentState = 62;
					break;
				} else {
					Error(la);
					goto case 62;
				}
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				if (la.kind == 36) {
					stateStack.Push(62);
					goto case 66;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 27) {
					currentState = 64;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 16;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 36) {
					stateStack.Push(65);
					goto case 66;
				} else {
					goto case 63;
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(36, la); // "("
				currentState = 67;
				break;
			}
			case 67: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 68;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 168) {
					goto case 82;
				} else {
					if (set[18, la.kind]) {
						goto case 70;
					} else {
						Error(la);
						goto case 69;
					}
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(37, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 70: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 71;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				if (set[19, la.kind]) {
					goto case 72;
				} else {
					goto case 69;
				}
			}
			case 72: {
				stateStack.Push(69);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 73;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (set[17, la.kind]) {
					goto case 78;
				} else {
					if (la.kind == 23) {
						goto case 74;
					} else {
						goto case 6;
					}
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				currentState = 75;
				break;
			}
			case 75: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 76;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				if (set[17, la.kind]) {
					stateStack.Push(77);
					goto case 39;
				} else {
					goto case 77;
				}
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (la.kind == 23) {
					goto case 74;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 78: {
				stateStack.Push(79);
				goto case 39;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				if (la.kind == 23) {
					currentState = 80;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 80: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 81;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[17, la.kind]) {
					goto case 78;
				} else {
					goto case 79;
				}
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				currentState = 83;
				break;
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				if (set[15, la.kind]) {
					stateStack.Push(84);
					goto case 61;
				} else {
					goto case 84;
				}
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 23) {
					goto case 82;
				} else {
					goto case 69;
				}
			}
			case 85: {
				stateStack.Push(86);
				goto case 42;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				Expect(143, la); // "Is"
				currentState = 61;
				break;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				if (la.kind == 27 || la.kind == 28 || la.kind == 36) {
					stateStack.Push(87);
					goto case 88;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				if (la.kind == 36) {
					currentState = 90;
					break;
				} else {
					if (la.kind == 27 || la.kind == 28) {
						goto case 89;
					} else {
						goto case 6;
					}
				}
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				currentState = 16;
				break;
			}
			case 90: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 91;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				if (la.kind == 168) {
					goto case 92;
				} else {
					if (set[19, la.kind]) {
						goto case 72;
					} else {
						goto case 6;
					}
				}
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = 93;
				break;
			}
			case 93: {
				stateStack.Push(94);
				goto case 61;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.kind == 23) {
					goto case 92;
				} else {
					goto case 69;
				}
			}
			case 95: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 96;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				if (set[20, la.kind]) {
					goto case 16;
				} else {
					if (la.kind == 36) {
						goto case 102;
					} else {
						if (set[21, la.kind]) {
							goto case 16;
						} else {
							if (la.kind == 27 || la.kind == 28) {
								goto case 89;
							} else {
								if (la.kind == 128) {
									currentState = 204;
									break;
								} else {
									if (la.kind == 235) {
										currentState = 202;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 195;
										} else {
											if (la.kind == 126 || la.kind == 208) {
												if (la.kind == 208) {
													currentState = 188;
													break;
												} else {
													if (la.kind == 126) {
														currentState = 166;
														break;
													} else {
														goto case 6;
													}
												}
											} else {
												if (la.kind == 57 || la.kind == 125) {
													if (la.kind == 125) {
														stateStack.Push(109);
														goto case 165;
													} else {
														if (la.kind == 57) {
															stateStack.Push(109);
															goto case 164;
														} else {
															Error(la);
															goto case 109;
														}
													}
												} else {
													if (set[22, la.kind]) {
														if (set[23, la.kind]) {
															currentState = 108;
															break;
														} else {
															if (la.kind == 93 || la.kind == 105 || la.kind == 217) {
																currentState = 104;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 134) {
															currentState = 97;
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
			case 97: {
				if (la == null) { currentState = 97; break; }
				Expect(36, la); // "("
				currentState = 98;
				break;
			}
			case 98: {
				stateStack.Push(99);
				goto case 39;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				Expect(23, la); // ","
				currentState = 100;
				break;
			}
			case 100: {
				stateStack.Push(101);
				goto case 39;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 23) {
					goto case 102;
				} else {
					goto case 69;
				}
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(69);
				goto case 39;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				Expect(36, la); // "("
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(106);
				goto case 39;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				Expect(23, la); // ","
				currentState = 107;
				break;
			}
			case 107: {
				stateStack.Push(69);
				goto case 61;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				Expect(36, la); // "("
				currentState = 103;
				break;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (set[24, la.kind]) {
					stateStack.Push(109);
					goto case 110;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				if (la.kind == 125) {
					goto case 161;
				} else {
					if (la.kind == 57) {
						currentState = 157;
						break;
					} else {
						if (la.kind == 195) {
							goto case 154;
						} else {
							if (la.kind == 106) {
								goto case 16;
							} else {
								if (la.kind == 228) {
									goto case 41;
								} else {
									if (la.kind == 175) {
										currentState = 150;
										break;
									} else {
										if (la.kind == 201 || la.kind == 210) {
											currentState = 148;
											break;
										} else {
											if (la.kind == 147) {
												goto case 145;
											} else {
												if (la.kind == 132) {
													currentState = 122;
													break;
												} else {
													if (la.kind == 145) {
														currentState = 111;
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
			case 111: {
				stateStack.Push(112);
				goto case 117;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				Expect(170, la); // "On"
				currentState = 113;
				break;
			}
			case 113: {
				stateStack.Push(114);
				goto case 39;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				Expect(115, la); // "Equals"
				currentState = 115;
				break;
			}
			case 115: {
				stateStack.Push(116);
				goto case 39;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				if (la.kind == 23) {
					currentState = 113;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 117: {
				stateStack.Push(118);
				goto case 121;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (la.kind == 62) {
					currentState = 120;
					break;
				} else {
					goto case 119;
				}
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				Expect(137, la); // "In"
				currentState = 39;
				break;
			}
			case 120: {
				stateStack.Push(119);
				goto case 61;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (set[25, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 122: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 123;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				if (la.kind == 145) {
					goto case 137;
				} else {
					if (set[26, la.kind]) {
						if (la.kind == 69) {
							goto case 134;
						} else {
							if (set[26, la.kind]) {
								goto case 135;
							} else {
								Error(la);
								goto case 124;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				Expect(69, la); // "By"
				currentState = 125;
				break;
			}
			case 125: {
				stateStack.Push(126);
				goto case 129;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 23) {
					goto case 134;
				} else {
					Expect(142, la); // "Into"
					currentState = 127;
					break;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 129;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 23) {
					currentState = 127;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 129: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 130;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (set[25, la.kind]) {
					currentState = 131;
					break;
				} else {
					goto case 39;
				}
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (la.kind == 62) {
					currentState = 132;
					break;
				} else {
					if (la.kind == 21) {
						goto case 41;
					} else {
						if (set[27, la.kind]) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 39;
						}
					}
				}
			}
			case 132: {
				stateStack.Push(133);
				goto case 61;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				Expect(21, la); // "="
				currentState = 39;
				break;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				currentState = 125;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 129;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 23) {
					currentState = 135;
					break;
				} else {
					goto case 124;
				}
			}
			case 137: {
				stateStack.Push(138);
				goto case 144;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 132 || la.kind == 145) {
					if (la.kind == 132) {
						currentState = 142;
						break;
					} else {
						if (la.kind == 145) {
							goto case 137;
						} else {
							Error(la);
							goto case 138;
						}
					}
				} else {
					goto case 139;
				}
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				Expect(142, la); // "Into"
				currentState = 140;
				break;
			}
			case 140: {
				stateStack.Push(141);
				goto case 129;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				if (la.kind == 23) {
					currentState = 140;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 142: {
				stateStack.Push(143);
				goto case 144;
			}
			case 143: {
				stateStack.Push(138);
				goto case 139;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(145, la); // "Join"
				currentState = 111;
				break;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				currentState = 146;
				break;
			}
			case 146: {
				stateStack.Push(147);
				goto case 129;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (la.kind == 23) {
					goto case 145;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 148: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 149;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				if (la.kind == 229) {
					goto case 41;
				} else {
					goto case 39;
				}
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(69, la); // "By"
				currentState = 151;
				break;
			}
			case 151: {
				stateStack.Push(152);
				goto case 39;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 63 || la.kind == 103) {
					currentState = 153;
					break;
				} else {
					Error(la);
					goto case 153;
				}
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (la.kind == 23) {
					currentState = 151;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				currentState = 155;
				break;
			}
			case 155: {
				stateStack.Push(156);
				goto case 129;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 23) {
					goto case 154;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 157: {
				stateStack.Push(158);
				goto case 117;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (set[24, la.kind]) {
					stateStack.Push(158);
					goto case 110;
				} else {
					Expect(142, la); // "Into"
					currentState = 159;
					break;
				}
			}
			case 159: {
				stateStack.Push(160);
				goto case 129;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 23) {
					currentState = 159;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 117;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 23) {
					goto case 161;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(57, la); // "Aggregate"
				currentState = 157;
				break;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				Expect(125, la); // "From"
				currentState = 162;
				break;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(36, la); // "("
				currentState = 167;
				break;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[28, la.kind]) {
					stateStack.Push(168);
					goto case 175;
				} else {
					goto case 168;
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(37, la); // ")"
				currentState = 169;
				break;
			}
			case 169: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 170;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (set[17, la.kind]) {
					goto case 39;
				} else {
					if (la.kind == 1 || la.kind == 22 || la.kind == 62) {
						if (la.kind == 62) {
							currentState = 174;
							break;
						} else {
							goto case 171;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 171: {
				stateStack.Push(172);
				goto case 29;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(112, la); // "End"
				currentState = 173;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(126, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 174: {
				stateStack.Push(171);
				goto case 61;
			}
			case 175: {
				stateStack.Push(176);
				goto case 177;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 23) {
					currentState = 175;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (la.kind == 39) {
					stateStack.Push(177);
					goto case 183;
				} else {
					goto case 178;
				}
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[29, la.kind]) {
					currentState = 178;
					break;
				} else {
					stateStack.Push(179);
					goto case 121;
				}
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (la.kind == 62) {
					goto case 181;
				} else {
					goto case 180;
				}
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 21) {
					goto case 41;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				currentState = 182;
				break;
			}
			case 182: {
				stateStack.Push(180);
				goto case 61;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(39, la); // "<"
				currentState = 184;
				break;
			}
			case 184: {
				PushContext(Context.Attribute, t);
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (set[30, la.kind]) {
					currentState = 185;
					break;
				} else {
					Expect(38, la); // ">"
					currentState = 186;
					break;
				}
			}
			case 186: {
				PopContext();
				goto case 187;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(36, la); // "("
				currentState = 189;
				break;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (set[28, la.kind]) {
					stateStack.Push(190);
					goto case 175;
				} else {
					goto case 190;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(37, la); // ")"
				currentState = 191;
				break;
			}
			case 191: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 192;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (set[9, la.kind]) {
					goto case 37;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(193);
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(112, la); // "End"
				currentState = 194;
				break;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(208, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.kind == 17) {
					currentState = 195;
					break;
				} else {
					stateStack.Push(196);
					goto case 197;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 17) {
					currentState = 196;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 198;
				break;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[31, la.kind]) {
					currentState = 198;
					break;
				} else {
					if (la.kind == 14) {
						goto case 16;
					} else {
						if (la.kind == 11) {
							goto case 199;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				currentState = 200;
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (set[32, la.kind]) {
					if (set[33, la.kind]) {
						goto case 199;
					} else {
						if (la.kind == 10) {
							stateStack.Push(200);
							goto case 197;
						} else {
							Error(la);
							goto case 200;
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 201;
					break;
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (set[34, la.kind]) {
					currentState = 201;
					break;
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				Expect(36, la); // "("
				currentState = 203;
				break;
			}
			case 203: {
				readXmlIdentifier = true;
				stateStack.Push(69);
				goto case 121;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				Expect(36, la); // "("
				currentState = 107;
				break;
			}
			case 205: {
				stateStack.Push(206);
				goto case 39;
			}
			case 206: {
				stateStack.Push(207);
				goto case 29;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(112, la); // "End"
				currentState = 208;
				break;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(224, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				currentState = 210;
				break;
			}
			case 210: {
				stateStack.Push(211);
				goto case 39;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 23) {
					goto case 209;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 212: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 213;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 182) {
					goto case 41;
				} else {
					goto case 39;
				}
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				currentState = 215;
				break;
			}
			case 215: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 216;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[17, la.kind]) {
					goto case 39;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (la.kind == 107 || la.kind == 123 || la.kind == 229) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[35, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (set[36, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 220: {
				stateStack.Push(221);
				goto case 29;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 74) {
					currentState = 225;
					break;
				} else {
					if (la.kind == 122) {
						currentState = 224;
						break;
					} else {
						goto case 222;
					}
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(112, la); // "End"
				currentState = 223;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(216, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				stateStack.Push(222);
				goto case 29;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (set[25, la.kind]) {
					currentState = 228;
					break;
				} else {
					goto case 226;
				}
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 227) {
					currentState = 227;
					break;
				} else {
					goto case 220;
				}
			}
			case 227: {
				stateStack.Push(220);
				goto case 39;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (la.kind == 62) {
					currentState = 229;
					break;
				} else {
					goto case 226;
				}
			}
			case 229: {
				stateStack.Push(226);
				goto case 61;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (set[37, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(117, la); // "Error"
				currentState = 232;
				break;
			}
			case 232: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 233;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (set[17, la.kind]) {
					goto case 39;
				} else {
					if (la.kind == 131) {
						currentState = 235;
						break;
					} else {
						if (la.kind == 192) {
							currentState = 234;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(162, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (set[36, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 236: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (set[14, la.kind]) {
					stateStack.Push(250);
					goto case 247;
				} else {
					if (la.kind == 109) {
						currentState = 238;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 238: {
				stateStack.Push(239);
				goto case 247;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				Expect(137, la); // "In"
				currentState = 240;
				break;
			}
			case 240: {
				stateStack.Push(241);
				goto case 39;
			}
			case 241: {
				stateStack.Push(242);
				goto case 29;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				Expect(162, la); // "Next"
				currentState = 243;
				break;
			}
			case 243: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 244;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (set[17, la.kind]) {
					goto case 245;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 245: {
				stateStack.Push(246);
				goto case 39;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 23) {
					currentState = 245;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 247: {
				stateStack.Push(248);
				goto case 95;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 32) {
					currentState = 249;
					break;
				} else {
					goto case 249;
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 27 || la.kind == 28 || la.kind == 36) {
					stateStack.Push(249);
					goto case 88;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(21, la); // "="
				currentState = 251;
				break;
			}
			case 251: {
				stateStack.Push(252);
				goto case 39;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 203) {
					currentState = 259;
					break;
				} else {
					goto case 253;
				}
			}
			case 253: {
				stateStack.Push(254);
				goto case 29;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				Expect(162, la); // "Next"
				currentState = 255;
				break;
			}
			case 255: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 256;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[17, la.kind]) {
					goto case 257;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 257: {
				stateStack.Push(258);
				goto case 39;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 23) {
					currentState = 257;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 259: {
				stateStack.Push(253);
				goto case 39;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (la.kind == 222 || la.kind == 229) {
					currentState = 263;
					break;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(261);
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				Expect(151, la); // "Loop"
				currentState = 262;
				break;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 222 || la.kind == 229) {
					goto case 41;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 263: {
				stateStack.Push(264);
				goto case 39;
			}
			case 264: {
				stateStack.Push(265);
				goto case 29;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				Expect(151, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 266: {
				stateStack.Push(267);
				goto case 39;
			}
			case 267: {
				stateStack.Push(268);
				goto case 29;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(112, la); // "End"
				currentState = 269;
				break;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				Expect(229, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 270: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 271;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (la.kind == 73) {
					currentState = 272;
					break;
				} else {
					goto case 272;
				}
			}
			case 272: {
				stateStack.Push(273);
				goto case 39;
			}
			case 273: {
				stateStack.Push(274);
				goto case 15;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 73) {
					currentState = 276;
					break;
				} else {
					Expect(112, la); // "End"
					currentState = 275;
					break;
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(195, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 110) {
					currentState = 278;
					break;
				} else {
					if (set[38, la.kind]) {
						goto case 279;
					} else {
						Error(la);
						goto case 278;
					}
				}
			}
			case 278: {
				stateStack.Push(274);
				goto case 29;
			}
			case 279: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 280;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (set[39, la.kind]) {
					if (la.kind == 143) {
						currentState = 282;
						break;
					} else {
						goto case 282;
					}
				} else {
					if (set[17, la.kind]) {
						stateStack.Push(281);
						goto case 39;
					} else {
						Error(la);
						goto case 281;
					}
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 23) {
					currentState = 279;
					break;
				} else {
					goto case 278;
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 284;
			}
			case 283: {
				stateStack.Push(281);
				goto case 42;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (set[40, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 285: {
				stateStack.Push(286);
				goto case 39;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 212) {
					currentState = 295;
					break;
				} else {
					goto case 287;
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 288;
				} else {
					goto case 6;
				}
			}
			case 288: {
				stateStack.Push(289);
				goto case 29;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (la.kind == 110 || la.kind == 111) {
					if (la.kind == 110) {
						currentState = 294;
						break;
					} else {
						if (la.kind == 111) {
							goto case 291;
						} else {
							Error(la);
							goto case 288;
						}
					}
				} else {
					Expect(112, la); // "End"
					currentState = 290;
					break;
				}
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(134, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				currentState = 292;
				break;
			}
			case 292: {
				stateStack.Push(293);
				goto case 39;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 212) {
					currentState = 288;
					break;
				} else {
					goto case 288;
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 134) {
					goto case 291;
				} else {
					goto case 288;
				}
			}
			case 295: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 296;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (set[9, la.kind]) {
					goto case 297;
				} else {
					goto case 287;
				}
			}
			case 297: {
				stateStack.Push(298);
				goto case 37;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 22) {
					currentState = 304;
					break;
				} else {
					if (la.kind == 110) {
						goto case 300;
					} else {
						goto case 299;
					}
				}
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				currentState = 301;
				break;
			}
			case 301: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 302;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[9, la.kind]) {
					stateStack.Push(303);
					goto case 37;
				} else {
					goto case 303;
				}
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 22) {
					goto case 300;
				} else {
					goto case 299;
				}
			}
			case 304: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 305;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (set[9, la.kind]) {
					goto case 297;
				} else {
					goto case 298;
				}
			}
			case 306: {
				stateStack.Push(307);
				goto case 16;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 36) {
					currentState = 70;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 308: {
				stateStack.Push(309);
				goto case 39;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(23, la); // ","
				currentState = 39;
				break;
			}
			case 310: {
				stateStack.Push(311);
				goto case 39;
			}
			case 311: {
				stateStack.Push(312);
				goto case 29;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				Expect(112, la); // "End"
				currentState = 313;
				break;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 209 || la.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 314: {
				stateStack.Push(315);
				goto case 121;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 32) {
					currentState = 316;
					break;
				} else {
					goto case 316;
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (la.kind == 36) {
					goto case 325;
				} else {
					goto case 317;
				}
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 23) {
					currentState = 319;
					break;
				} else {
					if (la.kind == 62) {
						currentState = 318;
						break;
					} else {
						goto case 180;
					}
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 161) {
					goto case 181;
				} else {
					goto case 182;
				}
			}
			case 319: {
				stateStack.Push(320);
				goto case 121;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 32) {
					currentState = 321;
					break;
				} else {
					goto case 321;
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 36) {
					goto case 322;
				} else {
					goto case 317;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				currentState = 323;
				break;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 23) {
					goto case 322;
				} else {
					goto case 324;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				Expect(37, la); // ")"
				currentState = 317;
				break;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				currentState = 326;
				break;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				if (la.kind == 23) {
					goto case 325;
				} else {
					goto case 324;
				}
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 39) {
					stateStack.Push(327);
					goto case 183;
				} else {
					stateStack.Push(26);
					goto case 61;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				Expect(97, la); // "Custom"
				currentState = 329;
				break;
			}
			case 329: {
				stateStack.Push(330);
				goto case 340;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (set[41, la.kind]) {
					goto case 332;
				} else {
					Expect(112, la); // "End"
					currentState = 331;
					break;
				}
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(118, la); // "Event"
				currentState = 15;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 39) {
					stateStack.Push(332);
					goto case 183;
				} else {
					if (la.kind == 55 || la.kind == 187 || la.kind == 191) {
						currentState = 333;
						break;
					} else {
						Error(la);
						goto case 333;
					}
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(36, la); // "("
				currentState = 334;
				break;
			}
			case 334: {
				stateStack.Push(335);
				goto case 175;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				Expect(37, la); // ")"
				currentState = 336;
				break;
			}
			case 336: {
				stateStack.Push(337);
				goto case 29;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(112, la); // "End"
				currentState = 338;
				break;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 55 || la.kind == 187 || la.kind == 191) {
					currentState = 339;
					break;
				} else {
					Error(la);
					goto case 339;
				}
			}
			case 339: {
				stateStack.Push(330);
				goto case 15;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(118, la); // "Event"
				currentState = 341;
				break;
			}
			case 341: {
				stateStack.Push(342);
				goto case 121;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 62) {
					currentState = 349;
					break;
				} else {
					if (set[42, la.kind]) {
						if (la.kind == 36) {
							currentState = 347;
							break;
						} else {
							goto case 343;
						}
					} else {
						Error(la);
						goto case 343;
					}
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 135) {
					goto case 344;
				} else {
					goto case 15;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				currentState = 345;
				break;
			}
			case 345: {
				stateStack.Push(346);
				goto case 61;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 23) {
					goto case 344;
				} else {
					goto case 15;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (set[28, la.kind]) {
					stateStack.Push(348);
					goto case 175;
				} else {
					goto case 348;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				Expect(37, la); // ")"
				currentState = 343;
				break;
			}
			case 349: {
				stateStack.Push(343);
				goto case 61;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				Expect(100, la); // "Declare"
				currentState = 351;
				break;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 61 || la.kind == 65 || la.kind == 221) {
					currentState = 352;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 353;
					break;
				} else {
					Error(la);
					goto case 353;
				}
			}
			case 353: {
				stateStack.Push(354);
				goto case 121;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(148, la); // "Lib"
				currentState = 355;
				break;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(3, la); // LiteralString
				currentState = 356;
				break;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 58) {
					currentState = 360;
					break;
				} else {
					goto case 357;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 36) {
					currentState = 358;
					break;
				} else {
					goto case 15;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (set[28, la.kind]) {
					stateStack.Push(359);
					goto case 175;
				} else {
					goto case 359;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				Expect(37, la); // ")"
				currentState = 15;
				break;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				Expect(3, la); // LiteralString
				currentState = 357;
				break;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 362;
					break;
				} else {
					Error(la);
					goto case 362;
				}
			}
			case 362: {
				PushContext(Context.IdentifierExpected, t);
				goto case 363;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				currentState = 364;
				break;
			}
			case 364: {
				PopContext();
				goto case 365;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 36) {
					currentState = 371;
					break;
				} else {
					goto case 366;
				}
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 62) {
					currentState = 370;
					break;
				} else {
					goto case 367;
				}
			}
			case 367: {
				stateStack.Push(368);
				goto case 29;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(112, la); // "End"
				currentState = 369;
				break;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 15;
					break;
				} else {
					Error(la);
					goto case 15;
				}
			}
			case 370: {
				stateStack.Push(367);
				goto case 61;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (set[28, la.kind]) {
					stateStack.Push(372);
					goto case 175;
				} else {
					goto case 372;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(37, la); // ")"
				currentState = 366;
				break;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 87) {
					currentState = 374;
					break;
				} else {
					goto case 374;
				}
			}
			case 374: {
				stateStack.Push(375);
				goto case 379;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 62) {
					currentState = 378;
					break;
				} else {
					goto case 376;
				}
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 21) {
					currentState = 377;
					break;
				} else {
					goto case 15;
				}
			}
			case 377: {
				stateStack.Push(15);
				goto case 39;
			}
			case 378: {
				stateStack.Push(376);
				goto case 61;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (set[43, la.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				currentState = 9;
				break;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				currentState = 382;
				break;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[3, la.kind]) {
					goto case 381;
				} else {
					stateStack.Push(383);
					goto case 15;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (set[44, la.kind]) {
					stateStack.Push(383);
					goto case 5;
				} else {
					Expect(112, la); // "End"
					currentState = 384;
					break;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(159, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				Expect(136, la); // "Imports"
				currentState = 386;
				break;
			}
			case 386: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (set[3, la.kind]) {
					currentState = 387;
					break;
				} else {
					goto case 15;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(172, la); // "Option"
				currentState = 389;
				break;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (set[3, la.kind]) {
					currentState = 389;
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
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,T, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}