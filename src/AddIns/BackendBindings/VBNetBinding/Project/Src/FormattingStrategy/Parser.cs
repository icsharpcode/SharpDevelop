using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;



namespace ICSharpCode.VBNetBinding.FormattingStrategy {



partial class VBIndentationStrategy {
	const int startOfExpression = 35;
	const int endOfStatementTerminatorAndBlock = 82;

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	List<Token> errors = new List<Token>();
	
	VBIndentationStrategy()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.Kind != expectedKind) {
			Error(la);
			Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		switchlbl: switch (currentState) {
			case 0: {
				if (la == null) { currentState = 0; break; }
				if (set[0].Get(la.Kind)) {
					currentState = 0;
					break;
				} else {
					goto case 1;
				}
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (set[1].Get(la.Kind)) {
					stateStack.Push(2);
					goto case 3;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (set[2].Get(la.Kind)) {
					currentState = 2;
					break;
				} else {
					goto case 1;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.Kind == 160) {
					goto case 358;
				} else {
					if (set[3].Get(la.Kind)) {
						if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
							goto case 351;
						} else {
							if (la.Kind == 103) {
								currentState = 264;
								break;
							} else {
								if (la.Kind == 115) {
									goto case 262;
								} else {
									if (la.Kind == 142) {
										currentState = 5;
										break;
									} else {
										goto case 4;
									}
								}
							}
						}
					} else {
						goto case 4;
					}
				}
			}
			case 4: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				currentState = 6;
				break;
			}
			case 6: {
				if (la == null) { currentState = 6; break; }
				if (la.Kind == 37) {
					currentState = 355;
					break;
				} else {
					goto case 7;
				}
			}
			case 7: {
				stateStack.Push(8);
				goto case 13;
			}
			case 8: {
				Indent(la);
				goto case 9;
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (la.Kind == 140) {
					goto case 352;
				} else {
					goto case 10;
				}
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (set[4].Get(la.Kind)) {
					if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
						stateStack.Push(15);
						goto case 269;
					} else {
						if (la.Kind == 103) {
							stateStack.Push(15);
							goto case 263;
						} else {
							if (la.Kind == 115) {
								stateStack.Push(15);
								goto case 253;
							} else {
								if (la.Kind == 142) {
									stateStack.Push(15);
									goto case 252;
								} else {
									if (set[5].Get(la.Kind)) {
										stateStack.Push(15);
										goto case 16;
									} else {
										Error(la);
										goto case 15;
									}
								}
							}
						}
					}
				} else {
					Unindent(la);
					goto case 11;
				}
			}
			case 11: {
				if (la == null) { currentState = 11; break; }
				Expect(113, la); // "End"
				currentState = 12;
				break;
			}
			case 12: {
				if (la == null) { currentState = 12; break; }
				Expect(142, la); // "Interface"
				currentState = 13;
				break;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (set[6].Get(la.Kind)) {
					currentState = 15;
					break;
				} else {
					goto case 10;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.Kind == 119) {
					goto case 250;
				} else {
					if (la.Kind == 186) {
						goto case 248;
					} else {
						if (la.Kind == 127 || la.Kind == 210) {
							goto case 17;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				currentState = 18;
				break;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[7].Get(la.Kind)) {
					goto case 17;
				} else {
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.Kind == 37) {
					currentState = 235;
					break;
				} else {
					goto case 20;
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.Kind == 63) {
					currentState = 21;
					break;
				} else {
					goto case 13;
				}
			}
			case 21: {
				stateStack.Push(13);
				goto case 22;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (set[8].Get(la.Kind)) {
					goto case 234;
				} else {
					Error(la);
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (la.Kind == 37) {
					stateStack.Push(23);
					goto case 27;
				} else {
					goto case 24;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.Kind == 26) {
					currentState = 25;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 25: {
				stateStack.Push(26);
				goto case 178;
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				if (la.Kind == 37) {
					stateStack.Push(26);
					goto case 27;
				} else {
					goto case 24;
				}
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(37, la); // "("
				currentState = 28;
				break;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (la.Kind == 169) {
					goto case 231;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 30;
					} else {
						Error(la);
						goto case 29;
					}
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(29);
					goto case 31;
				} else {
					goto case 29;
				}
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				if (set[9].Get(la.Kind)) {
					goto case 228;
				} else {
					if (la.Kind == 22) {
						goto case 32;
					} else {
						goto case 4;
					}
				}
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(34);
					goto case 35;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.Kind == 22) {
					goto case 32;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				goto case 36;
			}
			case 36: {
				stateStack.Push(37);
				goto case 38;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[10].Get(la.Kind)) {
					currentState = 36;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (set[11].Get(la.Kind)) {
					currentState = 38;
					break;
				} else {
					if (set[9].Get(la.Kind)) {
						stateStack.Push(56);
						goto case 65;
					} else {
						if (la.Kind == 220) {
							currentState = 54;
							break;
						} else {
							if (la.Kind == 162) {
								currentState = 43;
								break;
							} else {
								if (la.Kind == 35) {
									goto case 39;
								} else {
									goto case 4;
								}
							}
						}
					}
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				currentState = 40;
				break;
			}
			case 40: {
				stateStack.Push(41);
				goto case 35;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.Kind == 22) {
					goto case 39;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[8].Get(la.Kind)) {
					stateStack.Push(52);
					goto case 22;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.Kind == 233) {
					currentState = 45;
					break;
				} else {
					goto case 4;
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(35, la); // "{"
				currentState = 46;
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (la.Kind == 147) {
					currentState = 47;
					break;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				Expect(26, la); // "."
				currentState = 48;
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				currentState = 49;
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(20, la); // "="
				currentState = 50;
				break;
			}
			case 50: {
				stateStack.Push(51);
				goto case 35;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (la.Kind == 22) {
					currentState = 46;
					break;
				} else {
					goto case 42;
				}
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.Kind == 126 || la.Kind == 233) {
					if (la.Kind == 126) {
						currentState = 53;
						break;
					} else {
						goto case 44;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (la.Kind == 35) {
					goto case 39;
				} else {
					if (set[12].Get(la.Kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 4;
					}
				}
			}
			case 54: {
				stateStack.Push(55);
				goto case 38;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				Expect(144, la); // "Is"
				currentState = 22;
				break;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (set[13].Get(la.Kind)) {
					stateStack.Push(56);
					goto case 57;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (la.Kind == 37) {
					currentState = 61;
					break;
				} else {
					if (set[14].Get(la.Kind)) {
						currentState = 58;
						break;
					} else {
						goto case 4;
					}
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.Kind == 10) {
					currentState = 59;
					break;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = 60;
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.Kind == 11) {
					goto case 14;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.Kind == 169) {
					goto case 62;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 30;
					} else {
						goto case 4;
					}
				}
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = 63;
				break;
			}
			case 63: {
				stateStack.Push(64);
				goto case 22;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (la.Kind == 22) {
					goto case 62;
				} else {
					goto case 29;
				}
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (set[15].Get(la.Kind)) {
					goto case 14;
				} else {
					if (set[16].Get(la.Kind)) {
						if (la.Kind == 17 || la.Kind == 18 || la.Kind == 19) {
							goto case 223;
						} else {
							if (la.Kind == 10) {
								stateStack.Push(213);
								goto case 215;
							} else {
								goto case 4;
							}
						}
					} else {
						if (la.Kind == 127 || la.Kind == 210) {
							if (la.Kind == 210) {
								currentState = 207;
								break;
							} else {
								if (la.Kind == 127) {
									currentState = 72;
									break;
								} else {
									goto case 4;
								}
							}
						} else {
							if (la.Kind == 135) {
								currentState = 66;
								break;
							} else {
								goto case 4;
							}
						}
					}
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(37, la); // "("
				currentState = 67;
				break;
			}
			case 67: {
				stateStack.Push(68);
				goto case 35;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				Expect(22, la); // ","
				currentState = 69;
				break;
			}
			case 69: {
				stateStack.Push(70);
				goto case 35;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.Kind == 22) {
					currentState = 71;
					break;
				} else {
					goto case 29;
				}
			}
			case 71: {
				stateStack.Push(29);
				goto case 35;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				Expect(37, la); // "("
				currentState = 73;
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(74);
					goto case 203;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(38, la); // ")"
				currentState = 75;
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					if (la.Kind == 1 || la.Kind == 21 || la.Kind == 63) {
						if (la.Kind == 63) {
							currentState = 202;
							break;
						} else {
							goto case 76;
						}
					} else {
						goto case 4;
					}
				}
			}
			case 76: {
				stateStack.Push(77);
				goto case 79;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(113, la); // "End"
				currentState = 78;
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 79: {
				Indent(la);
				goto case 80;
			}
			case 80: {
				stateStack.Push(81);
				goto case 13;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[9].Get(la.Kind)) {
					if (set[9].Get(la.Kind)) {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(80);
							goto case 86;
						} else {
							goto case 80;
						}
					} else {
						if (la.Kind == 113) {
							currentState = 84;
							break;
						} else {
							goto case 83;
						}
					}
				} else {
					goto case 82;
				}
			}
			case 82: {
				Unindent(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 83: {
				Error(la);
				goto case 81;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 85;
				} else {
					if (set[17].Get(la.Kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 83;
					}
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				currentState = 81;
				break;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				if (la.Kind == 88 || la.Kind == 105 || la.Kind == 204) {
					currentState = 185;
					break;
				} else {
					if (la.Kind == 211 || la.Kind == 233) {
						currentState = 181;
						break;
					} else {
						if (la.Kind == 56 || la.Kind == 193) {
							currentState = 179;
							break;
						} else {
							if (la.Kind == 189) {
								currentState = 176;
								break;
							} else {
								if (la.Kind == 135) {
									currentState = 159;
									break;
								} else {
									if (la.Kind == 197) {
										currentState = 147;
										break;
									} else {
										if (la.Kind == 231) {
											currentState = 143;
											break;
										} else {
											if (la.Kind == 108) {
												currentState = 137;
												break;
											} else {
												if (la.Kind == 124) {
													currentState = 116;
													break;
												} else {
													if (la.Kind == 118 || la.Kind == 171 || la.Kind == 194) {
														if (la.Kind == 118 || la.Kind == 171) {
															if (la.Kind == 171) {
																currentState = 112;
																break;
															} else {
																goto case 112;
															}
														} else {
															if (la.Kind == 194) {
																currentState = 111;
																break;
															} else {
																goto case 4;
															}
														}
													} else {
														if (la.Kind == 215) {
															goto case 96;
														} else {
															if (la.Kind == 218) {
																currentState = 101;
																break;
															} else {
																if (set[18].Get(la.Kind)) {
																	if (la.Kind == 132) {
																		currentState = 100;
																		break;
																	} else {
																		if (la.Kind == 120) {
																			currentState = 99;
																			break;
																		} else {
																			if (la.Kind == 89) {
																				currentState = 98;
																				break;
																			} else {
																				if (la.Kind == 206) {
																					goto case 14;
																				} else {
																					if (la.Kind == 195) {
																						goto case 96;
																					} else {
																						goto case 4;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.Kind == 191) {
																		currentState = 95;
																		break;
																	} else {
																		if (la.Kind == 117) {
																			goto case 92;
																		} else {
																			if (la.Kind == 226) {
																				currentState = 88;
																				break;
																			} else {
																				if (set[9].Get(la.Kind)) {
																					if (la.Kind == 73) {
																						goto case 87;
																					} else {
																						goto case 35;
																					}
																				} else {
																					goto case 4;
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
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = 35;
				break;
			}
			case 88: {
				stateStack.Push(89);
				goto case 35;
			}
			case 89: {
				stateStack.Push(90);
				goto case 79;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				Expect(113, la); // "End"
				currentState = 91;
				break;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = 93;
				break;
			}
			case 93: {
				stateStack.Push(94);
				goto case 35;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.Kind == 22) {
					goto case 92;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				if (la.Kind == 184) {
					goto case 87;
				} else {
					goto case 35;
				}
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				currentState = 97;
				break;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				if (la.Kind == 108 || la.Kind == 124 || la.Kind == 231) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				if (set[19].Get(la.Kind)) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.Kind == 2 || la.Kind == 5) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 101: {
				stateStack.Push(102);
				goto case 79;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				if (la.Kind == 75) {
					currentState = 106;
					break;
				} else {
					if (la.Kind == 123) {
						currentState = 105;
						break;
					} else {
						goto case 103;
					}
				}
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				Expect(113, la); // "End"
				currentState = 104;
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				stateStack.Push(103);
				goto case 79;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (set[20].Get(la.Kind)) {
					currentState = 109;
					break;
				} else {
					goto case 107;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.Kind == 229) {
					currentState = 108;
					break;
				} else {
					goto case 101;
				}
			}
			case 108: {
				stateStack.Push(101);
				goto case 35;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.Kind == 63) {
					currentState = 110;
					break;
				} else {
					goto case 107;
				}
			}
			case 110: {
				stateStack.Push(107);
				goto case 22;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				if (la.Kind == 2 || la.Kind == 5 || la.Kind == 163) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				Expect(118, la); // "Error"
				currentState = 113;
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (set[9].Get(la.Kind)) {
					goto case 35;
				} else {
					if (la.Kind == 132) {
						currentState = 115;
						break;
					} else {
						if (la.Kind == 194) {
							currentState = 114;
							break;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.Kind == 2 || la.Kind == 5) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(128);
					goto case 125;
				} else {
					if (la.Kind == 110) {
						currentState = 117;
						break;
					} else {
						goto case 4;
					}
				}
			}
			case 117: {
				stateStack.Push(118);
				goto case 125;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				Expect(138, la); // "In"
				currentState = 119;
				break;
			}
			case 119: {
				stateStack.Push(120);
				goto case 35;
			}
			case 120: {
				stateStack.Push(121);
				goto case 79;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				Expect(163, la); // "Next"
				currentState = 122;
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				if (set[9].Get(la.Kind)) {
					goto case 123;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: {
				stateStack.Push(124);
				goto case 35;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (la.Kind == 22) {
					currentState = 123;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 125: {
				stateStack.Push(126);
				goto case 65;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.Kind == 33) {
					currentState = 127;
					break;
				} else {
					goto case 127;
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (set[13].Get(la.Kind)) {
					stateStack.Push(127);
					goto case 57;
				} else {
					if (la.Kind == 63) {
						currentState = 22;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(20, la); // "="
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 35;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.Kind == 205) {
					currentState = 136;
					break;
				} else {
					goto case 131;
				}
			}
			case 131: {
				stateStack.Push(132);
				goto case 79;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				Expect(163, la); // "Next"
				currentState = 133;
				break;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (set[9].Get(la.Kind)) {
					goto case 134;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 134: {
				stateStack.Push(135);
				goto case 35;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.Kind == 22) {
					currentState = 134;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 136: {
				stateStack.Push(131);
				goto case 35;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.Kind == 224 || la.Kind == 231) {
					currentState = 140;
					break;
				} else {
					if (la.Kind == 1 || la.Kind == 21) {
						stateStack.Push(138);
						goto case 79;
					} else {
						goto case 4;
					}
				}
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(152, la); // "Loop"
				currentState = 139;
				break;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (la.Kind == 224 || la.Kind == 231) {
					goto case 87;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				stateStack.Push(141);
				goto case 35;
			}
			case 141: {
				stateStack.Push(142);
				goto case 79;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 35;
			}
			case 144: {
				stateStack.Push(145);
				goto case 79;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(113, la); // "End"
				currentState = 146;
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (la.Kind == 74) {
					currentState = 148;
					break;
				} else {
					goto case 148;
				}
			}
			case 148: {
				stateStack.Push(149);
				goto case 35;
			}
			case 149: {
				stateStack.Push(150);
				goto case 13;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.Kind == 74) {
					currentState = 152;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 151;
					break;
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.Kind == 111) {
					currentState = 153;
					break;
				} else {
					if (set[9].Get(la.Kind)) {
						goto case 154;
					} else {
						Error(la);
						goto case 153;
					}
				}
			}
			case 153: {
				stateStack.Push(150);
				goto case 79;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[21].Get(la.Kind)) {
					if (la.Kind == 144) {
						currentState = 156;
						break;
					} else {
						goto case 156;
					}
				} else {
					if (set[9].Get(la.Kind)) {
						stateStack.Push(155);
						goto case 35;
					} else {
						Error(la);
						goto case 155;
					}
				}
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (la.Kind == 22) {
					currentState = 154;
					break;
				} else {
					goto case 153;
				}
			}
			case 156: {
				stateStack.Push(157);
				goto case 158;
			}
			case 157: {
				stateStack.Push(155);
				goto case 38;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (set[22].Get(la.Kind)) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 159: {
				stateStack.Push(160);
				goto case 35;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.Kind == 214) {
					currentState = 169;
					break;
				} else {
					goto case 161;
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.Kind == 1 || la.Kind == 21) {
					goto case 162;
				} else {
					goto case 4;
				}
			}
			case 162: {
				stateStack.Push(163);
				goto case 79;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.Kind == 111 || la.Kind == 112) {
					if (la.Kind == 111) {
						currentState = 168;
						break;
					} else {
						if (la.Kind == 112) {
							goto case 165;
						} else {
							Error(la);
							goto case 162;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 164;
					break;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 35;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (la.Kind == 214) {
					currentState = 162;
					break;
				} else {
					goto case 162;
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.Kind == 135) {
					goto case 165;
				} else {
					goto case 162;
				}
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (set[9].Get(la.Kind)) {
					goto case 170;
				} else {
					goto case 161;
				}
			}
			case 170: {
				stateStack.Push(171);
				goto case 86;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.Kind == 21) {
					currentState = 175;
					break;
				} else {
					if (la.Kind == 111) {
						goto case 172;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				currentState = 173;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(174);
					goto case 86;
				} else {
					goto case 174;
				}
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.Kind == 21) {
					goto case 172;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[9].Get(la.Kind)) {
					goto case 170;
				} else {
					goto case 171;
				}
			}
			case 176: {
				stateStack.Push(177);
				goto case 178;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (la.Kind == 37) {
					currentState = 30;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[23].Get(la.Kind)) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 35;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(22, la); // ","
				currentState = 35;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 35;
			}
			case 182: {
				stateStack.Push(183);
				goto case 79;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(113, la); // "End"
				currentState = 184;
				break;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (la.Kind == 211 || la.Kind == 233) {
					goto case 14;
				} else {
					goto case 4;
				}
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				currentState = 186;
				break;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.Kind == 33) {
					currentState = 187;
					break;
				} else {
					goto case 187;
				}
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.Kind == 37) {
					goto case 200;
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.Kind == 22) {
					currentState = 194;
					break;
				} else {
					goto case 189;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.Kind == 63) {
					currentState = 191;
					break;
				} else {
					goto case 190;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.Kind == 20) {
					goto case 87;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.Kind == 162) {
					stateStack.Push(190);
					goto case 193;
				} else {
					if (set[8].Get(la.Kind)) {
						goto case 192;
					} else {
						Error(la);
						goto case 190;
					}
				}
			}
			case 192: {
				stateStack.Push(190);
				goto case 22;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(162, la); // "New"
				currentState = 43;
				break;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				currentState = 195;
				break;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.Kind == 33) {
					currentState = 196;
					break;
				} else {
					goto case 196;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.Kind == 37) {
					goto case 197;
				} else {
					goto case 188;
				}
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				currentState = 198;
				break;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (la.Kind == 22) {
					goto case 197;
				} else {
					goto case 199;
				}
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(38, la); // ")"
				currentState = 188;
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				currentState = 201;
				break;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.Kind == 22) {
					goto case 200;
				} else {
					goto case 199;
				}
			}
			case 202: {
				stateStack.Push(76);
				goto case 22;
			}
			case 203: {
				stateStack.Push(204);
				goto case 205;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (la.Kind == 22) {
					currentState = 203;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				currentState = 206;
				break;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (set[24].Get(la.Kind)) {
					goto case 205;
				} else {
					if (la.Kind == 63) {
						currentState = 192;
						break;
					} else {
						goto case 190;
					}
				}
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(37, la); // "("
				currentState = 208;
				break;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(209);
					goto case 203;
				} else {
					goto case 209;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				Expect(38, la); // ")"
				currentState = 210;
				break;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (set[9].Get(la.Kind)) {
					goto case 86;
				} else {
					if (la.Kind == 1 || la.Kind == 21) {
						stateStack.Push(211);
						goto case 79;
					} else {
						goto case 4;
					}
				}
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(113, la); // "End"
				currentState = 212;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.Kind == 17) {
					currentState = 214;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.Kind == 16) {
					currentState = 213;
					break;
				} else {
					goto case 213;
				}
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 216;
				break;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[25].Get(la.Kind)) {
					if (set[26].Get(la.Kind)) {
						currentState = 216;
						break;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(216);
							goto case 220;
						} else {
							Error(la);
							goto case 216;
						}
					}
				} else {
					if (la.Kind == 14) {
						goto case 14;
					} else {
						if (la.Kind == 11) {
							goto case 217;
						} else {
							goto case 4;
						}
					}
				}
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				currentState = 218;
				break;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[27].Get(la.Kind)) {
					if (set[28].Get(la.Kind)) {
						goto case 217;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(218);
							goto case 220;
						} else {
							if (la.Kind == 10) {
								stateStack.Push(218);
								goto case 215;
							} else {
								Error(la);
								goto case 218;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 219;
					break;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (set[29].Get(la.Kind)) {
					if (set[30].Get(la.Kind)) {
						currentState = 219;
						break;
					} else {
						if (la.Kind == 12) {
							stateStack.Push(219);
							goto case 220;
						} else {
							Error(la);
							goto case 219;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 221;
				break;
			}
			case 221: {
				stateStack.Push(222);
				goto case 35;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				currentState = 224;
				break;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.Kind == 16) {
					currentState = 225;
					break;
				} else {
					goto case 225;
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.Kind == 17 || la.Kind == 19) {
					goto case 223;
				} else {
					if (la.Kind == 10) {
						stateStack.Push(226);
						goto case 215;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.Kind == 17) {
					currentState = 227;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.Kind == 16) {
					currentState = 226;
					break;
				} else {
					goto case 226;
				}
			}
			case 228: {
				stateStack.Push(229);
				goto case 35;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.Kind == 22) {
					currentState = 230;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (set[9].Get(la.Kind)) {
					goto case 228;
				} else {
					goto case 229;
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				currentState = 232;
				break;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (set[8].Get(la.Kind)) {
					stateStack.Push(233);
					goto case 22;
				} else {
					goto case 233;
				}
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (la.Kind == 22) {
					goto case 231;
				} else {
					goto case 29;
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				currentState = 23;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 169) {
						currentState = 237;
						break;
					} else {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(236);
							goto case 203;
						} else {
							Error(la);
							goto case 236;
						}
					}
				} else {
					goto case 236;
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(38, la); // ")"
				currentState = 19;
				break;
			}
			case 237: {
				stateStack.Push(236);
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (la.Kind == 138 || la.Kind == 178) {
					currentState = 239;
					break;
				} else {
					goto case 239;
				}
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				currentState = 240;
				break;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.Kind == 63) {
					currentState = 242;
					break;
				} else {
					goto case 241;
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.Kind == 22) {
					currentState = 238;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 242: {
				stateStack.Push(241);
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (set[31].Get(la.Kind)) {
					goto case 247;
				} else {
					if (la.Kind == 35) {
						goto case 244;
					} else {
						goto case 4;
					}
				}
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				currentState = 245;
				break;
			}
			case 245: {
				stateStack.Push(246);
				goto case 247;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.Kind == 22) {
					goto case 244;
				} else {
					goto case 42;
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (set[8].Get(la.Kind)) {
					goto case 234;
				} else {
					if (la.Kind == 84 || la.Kind == 162 || la.Kind == 209) {
						goto case 14;
					} else {
						goto case 4;
					}
				}
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				currentState = 249;
				break;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (set[17].Get(la.Kind)) {
					goto case 248;
				} else {
					goto case 13;
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				currentState = 251;
				break;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (set[17].Get(la.Kind)) {
					goto case 250;
				} else {
					goto case 13;
				}
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				Expect(142, la); // "Interface"
				currentState = 5;
				break;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				Expect(115, la); // "Enum"
				currentState = 254;
				break;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (set[17].Get(la.Kind)) {
					goto case 262;
				} else {
					stateStack.Push(255);
					goto case 13;
				}
			}
			case 255: {
				Indent(la);
				goto case 256;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[32].Get(la.Kind)) {
					currentState = 259;
					break;
				} else {
					Unindent(la);
					goto case 257;
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(113, la); // "End"
				currentState = 258;
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(115, la); // "Enum"
				currentState = 13;
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (la.Kind == 20) {
					currentState = 261;
					break;
				} else {
					goto case 260;
				}
			}
			case 260: {
				stateStack.Push(256);
				goto case 13;
			}
			case 261: {
				stateStack.Push(260);
				goto case 35;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				currentState = 254;
				break;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(103, la); // "Delegate"
				currentState = 264;
				break;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					currentState = 265;
					break;
				} else {
					Error(la);
					goto case 265;
				}
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				currentState = 266;
				break;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.Kind == 37) {
					currentState = 267;
					break;
				} else {
					goto case 20;
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(268);
					goto case 203;
				} else {
					goto case 268;
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(38, la); // ")"
				currentState = 20;
				break;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
					goto case 351;
				} else {
					Error(la);
					goto case 270;
				}
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				currentState = 271;
				break;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (la.Kind == 37) {
					currentState = 348;
					break;
				} else {
					goto case 272;
				}
			}
			case 272: {
				stateStack.Push(273);
				goto case 13;
			}
			case 273: {
				Indent(la);
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
						stateStack.Push(274);
						goto case 269;
					} else {
						if (la.Kind == 103) {
							stateStack.Push(274);
							goto case 263;
						} else {
							if (la.Kind == 115) {
								stateStack.Push(274);
								goto case 253;
							} else {
								if (la.Kind == 142) {
									stateStack.Push(274);
									goto case 252;
								} else {
									if (set[9].Get(la.Kind)) {
										stateStack.Push(274);
										goto case 279;
									} else {
										Error(la);
										goto case 274;
									}
								}
							}
						}
					}
				} else {
					Unindent(la);
					goto case 275;
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(113, la); // "End"
				currentState = 276;
				break;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (la.Kind == 84 || la.Kind == 155 || la.Kind == 209) {
					goto case 278;
				} else {
					goto case 277;
				}
			}
			case 277: {
				Error(la);
				goto case 13;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				currentState = 13;
				break;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[9].Get(la.Kind)) {
					goto case 344;
				} else {
					if (la.Kind == 127 || la.Kind == 210) {
						currentState = 335;
						break;
					} else {
						if (la.Kind == 101) {
							currentState = 328;
							break;
						} else {
							if (la.Kind == 119) {
								currentState = 319;
								break;
							} else {
								if (la.Kind == 98) {
									currentState = 308;
									break;
								} else {
									if (la.Kind == 186) {
										goto case 284;
									} else {
										if (la.Kind == 172) {
											goto case 280;
										} else {
											goto case 4;
										}
									}
								}
							}
						}
					}
				}
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				currentState = 281;
				break;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (set[17].Get(la.Kind)) {
					goto case 280;
				} else {
					stateStack.Push(282);
					goto case 79;
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(113, la); // "End"
				currentState = 283;
				break;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				Expect(172, la); // "Operator"
				currentState = 13;
				break;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				currentState = 285;
				break;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (set[33].Get(la.Kind)) {
					goto case 284;
				} else {
					if (la.Kind == 20) {
						currentState = 307;
						break;
					} else {
						goto case 286;
					}
				}
			}
			case 286: {
				stateStack.Push(287);
				goto case 13;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (set[34].Get(la.Kind)) {
					currentState = 287;
					break;
				} else {
					if (la.Kind == 128 || la.Kind == 198) {
						Indent(la);
						goto case 288;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 289;
					break;
				} else {
					Error(la);
					goto case 289;
				}
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (la.Kind == 37) {
					currentState = 305;
					break;
				} else {
					goto case 290;
				}
			}
			case 290: {
				stateStack.Push(291);
				goto case 79;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				Expect(113, la); // "End"
				currentState = 292;
				break;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 293;
					break;
				} else {
					Error(la);
					goto case 293;
				}
			}
			case 293: {
				stateStack.Push(294);
				goto case 13;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 298;
					break;
				} else {
					goto case 295;
				}
			}
			case 295: {
				Unindent(la);
				goto case 296;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(113, la); // "End"
				currentState = 297;
				break;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(186, la); // "Property"
				currentState = 13;
				break;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.Kind == 37) {
					currentState = 303;
					break;
				} else {
					goto case 299;
				}
			}
			case 299: {
				stateStack.Push(300);
				goto case 79;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(113, la); // "End"
				currentState = 301;
				break;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (la.Kind == 128 || la.Kind == 198) {
					currentState = 302;
					break;
				} else {
					Error(la);
					goto case 302;
				}
			}
			case 302: {
				stateStack.Push(295);
				goto case 13;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(304);
					goto case 203;
				} else {
					goto case 304;
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(38, la); // ")"
				currentState = 299;
				break;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(306);
					goto case 203;
				} else {
					goto case 306;
				}
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(38, la); // ")"
				currentState = 290;
				break;
			}
			case 307: {
				stateStack.Push(286);
				goto case 35;
			}
			case 308: {
				stateStack.Push(309);
				goto case 318;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.Kind == 56 || la.Kind == 189 || la.Kind == 193) {
					currentState = 311;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 310;
					break;
				}
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(119, la); // "Event"
				currentState = 13;
				break;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(37, la); // "("
				currentState = 312;
				break;
			}
			case 312: {
				stateStack.Push(313);
				goto case 203;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(38, la); // ")"
				currentState = 314;
				break;
			}
			case 314: {
				stateStack.Push(315);
				goto case 79;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(113, la); // "End"
				currentState = 316;
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (la.Kind == 56 || la.Kind == 189 || la.Kind == 193) {
					currentState = 317;
					break;
				} else {
					Error(la);
					goto case 317;
				}
			}
			case 317: {
				stateStack.Push(309);
				goto case 13;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				Expect(119, la); // "Event"
				currentState = 319;
				break;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				currentState = 320;
				break;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.Kind == 63) {
					currentState = 327;
					break;
				} else {
					if (set[35].Get(la.Kind)) {
						if (la.Kind == 37) {
							currentState = 325;
							break;
						} else {
							goto case 321;
						}
					} else {
						Error(la);
						goto case 321;
					}
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.Kind == 136) {
					goto case 322;
				} else {
					goto case 13;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				currentState = 323;
				break;
			}
			case 323: {
				stateStack.Push(324);
				goto case 22;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.Kind == 22) {
					goto case 322;
				} else {
					goto case 13;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (set[9].Get(la.Kind)) {
					stateStack.Push(326);
					goto case 203;
				} else {
					goto case 326;
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(38, la); // ")"
				currentState = 321;
				break;
			}
			case 327: {
				stateStack.Push(321);
				goto case 22;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.Kind == 62 || la.Kind == 66 || la.Kind == 223) {
					currentState = 329;
					break;
				} else {
					goto case 329;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					currentState = 330;
					break;
				} else {
					Error(la);
					goto case 330;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				currentState = 331;
				break;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(149, la); // "Lib"
				currentState = 332;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				Expect(3, la); // LiteralString
				currentState = 333;
				break;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.Kind == 59) {
					currentState = 334;
					break;
				} else {
					goto case 266;
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(3, la); // LiteralString
				currentState = 266;
				break;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				currentState = 336;
				break;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.Kind == 37) {
					currentState = 341;
					break;
				} else {
					if (la.Kind == 63) {
						currentState = 340;
						break;
					} else {
						goto case 337;
					}
				}
			}
			case 337: {
				stateStack.Push(338);
				goto case 79;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				Expect(113, la); // "End"
				currentState = 339;
				break;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.Kind == 127 || la.Kind == 210) {
					goto case 278;
				} else {
					goto case 277;
				}
			}
			case 340: {
				stateStack.Push(337);
				goto case 22;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (set[9].Get(la.Kind)) {
					if (la.Kind == 169) {
						currentState = 343;
						break;
					} else {
						if (set[9].Get(la.Kind)) {
							stateStack.Push(342);
							goto case 203;
						} else {
							Error(la);
							goto case 342;
						}
					}
				} else {
					goto case 342;
				}
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				Expect(38, la); // ")"
				currentState = 336;
				break;
			}
			case 343: {
				stateStack.Push(342);
				goto case 238;
			}
			case 344: {
				stateStack.Push(345);
				goto case 346;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.Kind == 22) {
					currentState = 344;
					break;
				} else {
					goto case 13;
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.Kind == 88) {
					currentState = 347;
					break;
				} else {
					goto case 347;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				currentState = 189;
				break;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				Expect(169, la); // "Of"
				currentState = 349;
				break;
			}
			case 349: {
				stateStack.Push(350);
				goto case 238;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				Expect(38, la); // ")"
				currentState = 272;
				break;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				currentState = 270;
				break;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				currentState = 353;
				break;
			}
			case 353: {
				stateStack.Push(354);
				goto case 22;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.Kind == 22) {
					goto case 352;
				} else {
					stateStack.Push(10);
					goto case 13;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(169, la); // "Of"
				currentState = 356;
				break;
			}
			case 356: {
				stateStack.Push(357);
				goto case 238;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(38, la); // ")"
				currentState = 7;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				currentState = 359;
				break;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (set[17].Get(la.Kind)) {
					goto case 358;
				} else {
					stateStack.Push(360);
					goto case 13;
				}
			}
			case 360: {
				Indent(la);
				goto case 361;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (set[1].Get(la.Kind)) {
					stateStack.Push(364);
					goto case 3;
				} else {
					Unindent(la);
					goto case 362;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(113, la); // "End"
				currentState = 363;
				break;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(160, la); // "Namespace"
				currentState = 13;
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (set[36].Get(la.Kind)) {
					currentState = 364;
					break;
				} else {
					goto case 361;
				}
			}
		}

		if (la != null)
			t = la;
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {-2, -1, -1048577, -524417, -134234113, -2, -131073, -1}),
		new BitArray(new int[] {0, 0, 1048576, 524416, 134234112, 1, 131072, 0}),
		new BitArray(new int[] {-2, -1, -1048577, -524417, -134234113, -2, -131073, -1}),
		new BitArray(new int[] {0, 0, 1048576, 524416, 134234112, 0, 131072, 0}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1048577, 2138439551, -134234113, -67108865, -393217, -1}),
		new BitArray(new int[] {-2097156, 2147483615, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 2, 262288, 8216, 8396804, 256, 1610679824, 2}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-918530, -1, -1, 2147483647, -129, -1, -262145, -1}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -33}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 2, 1310864, 8216, 8396804, 260, 1610810896, 2}),
		new BitArray(new int[] {-2, -1, -1, -131073, -1, -1, -1, -1}),
		new BitArray(new int[] {-3145732, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2, -1, -1, -1, -2, -1, -65, -1}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {-2, -1, -1048577, -655489, -134234113, -2, -131073, -1})

	};

} // end Parser


}