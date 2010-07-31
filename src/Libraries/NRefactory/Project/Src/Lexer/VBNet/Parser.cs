using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 51;
	const int endOfStatementTerminatorAndBlock = 228;
	static BitArray GetExpectedSet(int state)
	{
		switch (state) {
			case 0:
			case 1:
				return set[0];
			case 2:
				return set[1];
			case 3:
			case 4:
				return set[2];
			case 5:
				return set[3];
			case 6:
			case 66:
			case 229:
			case 460:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 157:
			case 163:
			case 169:
			case 206:
			case 210:
			case 249:
			case 349:
			case 358:
			case 411:
			case 450:
			case 458:
			case 466:
			case 489:
			case 524:
			case 578:
			case 592:
			case 661:
				return set[6];
			case 10:
			case 490:
			case 491:
			case 535:
			case 545:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 11:
			case 18:
			case 19:
			case 31:
			case 221:
			case 224:
			case 225:
			case 235:
			case 250:
			case 254:
			case 276:
			case 291:
			case 302:
			case 305:
			case 311:
			case 316:
			case 325:
			case 326:
			case 346:
			case 366:
			case 468:
			case 483:
			case 492:
			case 501:
			case 518:
			case 522:
			case 530:
			case 536:
			case 539:
			case 546:
			case 549:
			case 573:
			case 576:
			case 600:
			case 610:
			case 614:
			case 639:
			case 660:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 12:
			case 13:
				return set[7];
			case 14:
			case 15:
				return set[8];
			case 16:
			case 222:
			case 236:
			case 252:
			case 306:
			case 347:
			case 391:
			case 499:
			case 519:
			case 537:
			case 541:
			case 547:
			case 574:
			case 611:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 464:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 643:
				return set[10];
			case 24:
				return set[11];
			case 25:
				return set[12];
			case 26:
			case 27:
			case 122:
			case 179:
			case 180:
			case 230:
			case 381:
			case 382:
			case 399:
			case 400:
			case 401:
			case 402:
			case 477:
			case 478:
			case 511:
			case 512:
			case 606:
			case 607:
			case 653:
			case 654:
				return set[13];
			case 28:
			case 29:
			case 451:
			case 459:
			case 479:
			case 480:
			case 597:
			case 608:
			case 609:
				return set[14];
			case 30:
			case 32:
			case 126:
			case 136:
			case 152:
			case 167:
			case 183:
			case 261:
			case 286:
			case 365:
			case 378:
			case 414:
			case 454:
			case 474:
			case 482:
			case 558:
			case 582:
			case 587:
			case 599:
			case 613:
			case 632:
			case 635:
			case 638:
			case 645:
			case 648:
			case 666:
				return set[15];
			case 33:
			case 36:
				return set[16];
			case 34:
				return set[17];
			case 35:
			case 72:
			case 76:
			case 131:
			case 341:
			case 417:
				return set[18];
			case 37:
			case 142:
			case 149:
			case 153:
			case 215:
			case 385:
			case 410:
			case 413:
			case 513:
			case 514:
			case 570:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 38:
			case 39:
			case 133:
			case 134:
				return set[19];
			case 40:
			case 135:
			case 218:
			case 363:
			case 388:
			case 412:
			case 428:
			case 457:
			case 463:
			case 486:
			case 516:
			case 552:
			case 555:
			case 564:
			case 572:
			case 586:
			case 603:
			case 617:
			case 642:
			case 652:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 41:
			case 42:
			case 46:
			case 47:
			case 48:
			case 50:
			case 422:
			case 423:
				return set[20];
			case 43:
			case 44:
				return set[21];
			case 45:
			case 144:
			case 151:
			case 344:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 49:
			case 137:
			case 146:
			case 362:
			case 364:
			case 368:
			case 376:
			case 421:
			case 425:
			case 435:
			case 442:
			case 449:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 51:
			case 52:
			case 54:
			case 55:
			case 56:
			case 59:
			case 74:
			case 124:
			case 143:
			case 145:
			case 147:
			case 150:
			case 159:
			case 161:
			case 201:
			case 234:
			case 238:
			case 240:
			case 241:
			case 258:
			case 275:
			case 280:
			case 289:
			case 295:
			case 297:
			case 301:
			case 304:
			case 310:
			case 321:
			case 323:
			case 329:
			case 343:
			case 345:
			case 377:
			case 404:
			case 419:
			case 420:
			case 473:
			case 557:
				return set[22];
			case 53:
			case 57:
				return set[23];
			case 58:
			case 68:
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 60:
			case 75:
			case 445:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 61:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 62:
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 63:
			case 64:
				return set[24];
			case 65:
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 67:
				return set[25];
			case 70:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 71:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 73:
			case 182:
			case 184:
			case 185:
			case 288:
			case 662:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 78:
			case 307:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 82:
			case 253:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 86:
			case 392:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 90:
			case 313:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 91:
			case 523:
			case 542:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 95:
			case 270:
			case 277:
			case 292:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 98:
			case 188:
			case 193:
			case 195:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 99:
			case 190:
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 102:
			case 223:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 103:
			case 213:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 104:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 106:
			case 160:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 107:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 108:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 110:
			case 565:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 111:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 113:
			case 172:
			case 200:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 119:
			case 212:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 121:
				return set[26];
			case 123:
				return set[27];
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 127:
				return set[28];
			case 128:
				return set[29];
			case 129:
			case 130:
			case 415:
			case 416:
				return set[30];
			case 132:
				return set[31];
			case 138:
			case 139:
			case 273:
			case 282:
				return set[32];
			case 140:
			case 394:
				return set[33];
			case 141:
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 148:
				return set[34];
			case 154:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 155:
			case 156:
				return set[35];
			case 158:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 162:
			case 176:
			case 192:
			case 197:
			case 203:
			case 205:
			case 209:
			case 211:
				return set[36];
			case 164:
			case 165:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 166:
			case 168:
			case 274:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 170:
			case 171:
			case 173:
			case 175:
			case 177:
			case 178:
			case 186:
			case 191:
			case 196:
			case 204:
			case 208:
				return set[37];
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 181:
				return set[38];
			case 187:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 189:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 198:
			case 199:
				return set[39];
			case 202:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 207:
				return set[40];
			case 214:
			case 476:
			case 591:
			case 605:
			case 612:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 216:
			case 217:
			case 386:
			case 387:
			case 455:
			case 456:
			case 461:
			case 462:
			case 484:
			case 485:
			case 550:
			case 551:
			case 553:
			case 554:
			case 562:
			case 563:
			case 584:
			case 585:
			case 601:
			case 602:
				return set[41];
			case 219:
			case 220:
				return set[42];
			case 226:
			case 227:
				return set[43];
			case 228:
				return set[44];
			case 231:
				return set[45];
			case 232:
			case 233:
			case 334:
				return set[46];
			case 237:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 239:
			case 281:
			case 296:
				return set[47];
			case 242:
			case 243:
			case 263:
			case 264:
			case 278:
			case 279:
			case 293:
			case 294:
				return set[48];
			case 244:
			case 335:
			case 338:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 245:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 246:
				return set[49];
			case 247:
			case 266:
				return set[50];
			case 248:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 251:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 255:
			case 256:
				return set[51];
			case 257:
			case 262:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 259:
			case 260:
				return set[52];
			case 265:
				return set[53];
			case 267:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 268:
			case 269:
				return set[54];
			case 271:
			case 272:
				return set[55];
			case 283:
			case 284:
				return set[56];
			case 285:
				return set[57];
			case 287:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 298:
				return set[58];
			case 299:
			case 303:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 300:
				return set[59];
			case 308:
			case 309:
				return set[60];
			case 312:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 314:
			case 315:
				return set[61];
			case 317:
			case 318:
				return set[62];
			case 319:
			case 583:
			case 620:
			case 633:
			case 634:
			case 636:
			case 646:
			case 647:
			case 649:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 320:
			case 322:
				return set[63];
			case 324:
			case 330:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 327:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 331:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 332:
			case 333:
			case 389:
			case 390:
				return set[64];
			case 336:
			case 337:
			case 339:
			case 340:
				return set[65];
			case 342:
				return set[66];
			case 348:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 350:
			case 351:
			case 359:
			case 360:
				return set[67];
			case 352:
			case 361:
				return set[68];
			case 353:
				return set[69];
			case 354:
			case 357:
				return set[70];
			case 355:
			case 356:
			case 626:
			case 627:
				return set[71];
			case 367:
			case 369:
			case 370:
			case 515:
			case 571:
				return set[72];
			case 371:
			case 372:
				return set[73];
			case 373:
			case 374:
				return set[74];
			case 375:
			case 379:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 380:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 383:
			case 384:
				return set[75];
			case 393:
				return set[76];
			case 395:
			case 408:
				return set[77];
			case 396:
			case 409:
				return set[78];
			case 397:
			case 398:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 403:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 405:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 406:
				return set[79];
			case 407:
				return set[80];
			case 418:
				return set[81];
			case 424:
				return set[82];
			case 426:
			case 427:
			case 615:
			case 616:
				return set[83];
			case 429:
			case 430:
			case 431:
			case 436:
			case 437:
			case 618:
			case 641:
			case 651:
				return set[84];
			case 432:
			case 438:
			case 447:
				return set[85];
			case 433:
			case 434:
			case 439:
			case 440:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 441:
			case 443:
			case 448:
				return set[86];
			case 444:
			case 446:
				return set[87];
			case 452:
			case 467:
			case 481:
			case 517:
			case 598:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 453:
			case 521:
				return set[88];
			case 465:
			case 470:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 469:
				return set[89];
			case 471:
				return set[90];
			case 472:
			case 529:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 475:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 487:
			case 488:
			case 500:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 493:
			case 494:
				return set[91];
			case 495:
			case 496:
				return set[92];
			case 497:
			case 498:
			case 509:
				return set[93];
			case 502:
				return set[94];
			case 503:
			case 504:
				return set[95];
			case 505:
			case 506:
			case 630:
				return set[96];
			case 507:
				return set[97];
			case 508:
				return set[98];
			case 510:
			case 520:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 525:
			case 526:
				return set[99];
			case 527:
				return set[100];
			case 528:
			case 561:
				return set[101];
			case 531:
			case 532:
			case 533:
			case 556:
				return set[102];
			case 534:
			case 538:
			case 548:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 540:
				return set[103];
			case 543:
				return set[104];
			case 544:
				return set[105];
			case 559:
			case 625:
			case 628:
				return set[106];
			case 560:
				return set[107];
			case 566:
			case 568:
			case 577:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 567:
				return set[108];
			case 569:
				return set[109];
			case 575:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 579:
			case 580:
				return set[110];
			case 581:
			case 588:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 589:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 590:
				return set[111];
			case 593:
			case 594:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 595:
			case 604:
			case 663:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 596:
				return set[112];
			case 619:
			case 621:
				return set[113];
			case 622:
			case 629:
				return set[114];
			case 623:
			case 624:
				return set[115];
			case 631:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 637:
			case 644:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 640:
			case 650:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 655:
				return set[116];
			case 656:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 657:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 658:
			case 659:
				return set[117];
			case 664:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 665:
				return set[118];
			case 667:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 668:
				return set[119];
			case 669:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 670:
				return set[120];
			default: throw new InvalidOperationException();
		}
	}

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfExpression = false;
	bool readXmlIdentifier = false;
	bool identifierExpected = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	bool isMissingModifier = false;
	int activeArgument = 0;
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
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 667;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 657;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 380;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[3].Get(la.kind)) {
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
					currentState = 653;
					break;
				} else {
					if (set[4].Get(la.kind)) {
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
					goto case 380;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[121].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 487;
					} else {
						if (la.kind == 103) {
							currentState = 476;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 466;
								break;
							} else {
								if (la.kind == 142) {
									currentState = 9;
									break;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				stateStack.Push(10);
				goto case 169;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 37) {
					currentState = 650;
					break;
				} else {
					goto case 11;
				}
			}
			case 11: {
				stateStack.Push(12);
				goto case 18;
			}
			case 12: {
				isMissingModifier = true;
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 140) {
					PushContext(Context.Type, la, t);
					goto case 644;
				} else {
					goto case 14;
				}
			}
			case 14: {
				isMissingModifier = true;
				goto case 15;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (set[9].Get(la.kind)) {
					goto case 21;
				} else {
					isMissingModifier = false;
					goto case 16;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				Expect(113, la); // "End"
				currentState = 17;
				break;
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				Expect(142, la); // "Interface"
				currentState = 18;
				break;
			}
			case 18: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 19;
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 21: {
				isMissingModifier = true;
				goto case 22;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 40) {
					stateStack.Push(21);
					goto case 380;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[122].Get(la.kind)) {
					currentState = 643;
					break;
				} else {
					isMissingModifier = false;
					goto case 24;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(14);
					goto case 487;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 475;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 465;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 464;
							} else {
								if (set[12].Get(la.kind)) {
									stateStack.Push(14);
									goto case 25;
								} else {
									Error(la);
									goto case 14;
								}
							}
						}
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 119) {
					currentState = 458;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 450;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 26;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 26: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				currentState = 28;
				break;
			}
			case 28: {
				PopContext();
				goto case 29;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 37) {
					currentState = 426;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 30;
						break;
					} else {
						goto case 18;
					}
				}
			}
			case 30: {
				PushContext(Context.Type, la, t);
				stateStack.Push(31);
				goto case 32;
			}
			case 31: {
				PopContext();
				goto case 18;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (la.kind == 130) {
					currentState = 33;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 33;
						break;
					} else {
						if (set[123].Get(la.kind)) {
							currentState = 33;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 33;
								break;
							} else {
								Error(la);
								goto case 33;
							}
						}
					}
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 37) {
					stateStack.Push(33);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 26) {
					currentState = 35;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				stateStack.Push(36);
				goto case 76;
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (la.kind == 37) {
					stateStack.Push(36);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				Expect(37, la); // "("
				currentState = 38;
				break;
			}
			case 38: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 169) {
					currentState = 424;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 41;
					} else {
						Error(la);
						goto case 40;
					}
				}
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 41: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 42;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(40);
					goto case 43;
				} else {
					goto case 40;
				}
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (set[22].Get(la.kind)) {
					activeArgument = 0;
					goto case 420;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 45;
					} else {
						goto case 6;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(22, la); // ","
				currentState = 46;
				break;
			}
			case 46: {
				activeArgument++;
				goto case 47;
			}
			case 47: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 48;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(49);
					goto case 51;
				} else {
					goto case 49;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (la.kind == 22) {
					currentState = 50;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 50: {
				activeArgument++;
				goto case 47;
			}
			case 51: {
				PushContext(Context.Expression, la, t);
				goto case 52;
			}
			case 52: {
				stateStack.Push(53);
				goto case 54;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (set[124].Get(la.kind)) {
					currentState = 52;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 54: {
				PushContext(Context.Expression, la, t);
				goto case 55;
			}
			case 55: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 56;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (set[125].Get(la.kind)) {
					currentState = 55;
					break;
				} else {
					if (set[32].Get(la.kind)) {
						stateStack.Push(127);
						goto case 138;
					} else {
						if (la.kind == 220) {
							currentState = 124;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(57);
								goto case 62;
							} else {
								if (la.kind == 35) {
									stateStack.Push(57);
									goto case 58;
								} else {
									Error(la);
									goto case 57;
								}
							}
						}
					}
				}
			}
			case 57: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				Expect(35, la); // "{"
				currentState = 59;
				break;
			}
			case 59: {
				stateStack.Push(60);
				goto case 51;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.kind == 22) {
					currentState = 59;
					break;
				} else {
					goto case 61;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				Expect(162, la); // "New"
				currentState = 63;
				break;
			}
			case 63: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 64;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(121);
					goto case 32;
				} else {
					goto case 65;
				}
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 233) {
					currentState = 68;
					break;
				} else {
					goto case 66;
				}
			}
			case 66: {
				Error(la);
				goto case 67;
			}
			case 67: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 68: {
				stateStack.Push(67);
				goto case 69;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(35, la); // "{"
				currentState = 70;
				break;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 147) {
					currentState = 71;
					break;
				} else {
					goto case 71;
				}
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				Expect(26, la); // "."
				currentState = 72;
				break;
			}
			case 72: {
				stateStack.Push(73);
				goto case 76;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				Expect(20, la); // "="
				currentState = 74;
				break;
			}
			case 74: {
				stateStack.Push(75);
				goto case 51;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (la.kind == 22) {
					currentState = 70;
					break;
				} else {
					goto case 61;
				}
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				if (la.kind == 2) {
					goto case 120;
				} else {
					if (la.kind == 56) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 57) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 58) {
								goto case 119;
							} else {
								if (la.kind == 59) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 60) {
										currentState = stateStack.Pop();
										break;
									} else {
										if (la.kind == 61) {
											currentState = stateStack.Pop();
											break;
										} else {
											if (la.kind == 62) {
												goto case 118;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 117;
													} else {
														if (la.kind == 65) {
															goto case 116;
														} else {
															if (la.kind == 66) {
																goto case 115;
															} else {
																if (la.kind == 67) {
																	goto case 114;
																} else {
																	if (la.kind == 68) {
																		currentState = stateStack.Pop();
																		break;
																	} else {
																		if (la.kind == 69) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 70) {
																				goto case 113;
																			} else {
																				if (la.kind == 71) {
																					currentState = stateStack.Pop();
																					break;
																				} else {
																					if (la.kind == 72) {
																						currentState = stateStack.Pop();
																						break;
																					} else {
																						if (la.kind == 73) {
																							currentState = stateStack.Pop();
																							break;
																						} else {
																							if (la.kind == 74) {
																								currentState = stateStack.Pop();
																								break;
																							} else {
																								if (la.kind == 75) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 76) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 77) {
																											currentState = stateStack.Pop();
																											break;
																										} else {
																											if (la.kind == 78) {
																												currentState = stateStack.Pop();
																												break;
																											} else {
																												if (la.kind == 79) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 80) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 81) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 82) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 83) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 84) {
																																		goto case 112;
																																	} else {
																																		if (la.kind == 85) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 86) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 87) {
																																					goto case 111;
																																				} else {
																																					if (la.kind == 88) {
																																						currentState = stateStack.Pop();
																																						break;
																																					} else {
																																						if (la.kind == 89) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							if (la.kind == 90) {
																																								currentState = stateStack.Pop();
																																								break;
																																							} else {
																																								if (la.kind == 91) {
																																									currentState = stateStack.Pop();
																																									break;
																																								} else {
																																									if (la.kind == 92) {
																																										currentState = stateStack.Pop();
																																										break;
																																									} else {
																																										if (la.kind == 93) {
																																											currentState = stateStack.Pop();
																																											break;
																																										} else {
																																											if (la.kind == 94) {
																																												currentState = stateStack.Pop();
																																												break;
																																											} else {
																																												if (la.kind == 95) {
																																													currentState = stateStack.Pop();
																																													break;
																																												} else {
																																													if (la.kind == 96) {
																																														currentState = stateStack.Pop();
																																														break;
																																													} else {
																																														if (la.kind == 97) {
																																															currentState = stateStack.Pop();
																																															break;
																																														} else {
																																															if (la.kind == 98) {
																																																goto case 110;
																																															} else {
																																																if (la.kind == 99) {
																																																	currentState = stateStack.Pop();
																																																	break;
																																																} else {
																																																	if (la.kind == 100) {
																																																		currentState = stateStack.Pop();
																																																		break;
																																																	} else {
																																																		if (la.kind == 101) {
																																																			currentState = stateStack.Pop();
																																																			break;
																																																		} else {
																																																			if (la.kind == 102) {
																																																				currentState = stateStack.Pop();
																																																				break;
																																																			} else {
																																																				if (la.kind == 103) {
																																																					currentState = stateStack.Pop();
																																																					break;
																																																				} else {
																																																					if (la.kind == 104) {
																																																						goto case 109;
																																																					} else {
																																																						if (la.kind == 105) {
																																																							currentState = stateStack.Pop();
																																																							break;
																																																						} else {
																																																							if (la.kind == 106) {
																																																								currentState = stateStack.Pop();
																																																								break;
																																																							} else {
																																																								if (la.kind == 107) {
																																																									goto case 108;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 107;
																																																									} else {
																																																										if (la.kind == 109) {
																																																											currentState = stateStack.Pop();
																																																											break;
																																																										} else {
																																																											if (la.kind == 110) {
																																																												currentState = stateStack.Pop();
																																																												break;
																																																											} else {
																																																												if (la.kind == 111) {
																																																													currentState = stateStack.Pop();
																																																													break;
																																																												} else {
																																																													if (la.kind == 112) {
																																																														currentState = stateStack.Pop();
																																																														break;
																																																													} else {
																																																														if (la.kind == 113) {
																																																															currentState = stateStack.Pop();
																																																															break;
																																																														} else {
																																																															if (la.kind == 114) {
																																																																currentState = stateStack.Pop();
																																																																break;
																																																															} else {
																																																																if (la.kind == 115) {
																																																																	currentState = stateStack.Pop();
																																																																	break;
																																																																} else {
																																																																	if (la.kind == 116) {
																																																																		goto case 106;
																																																																	} else {
																																																																		if (la.kind == 117) {
																																																																			currentState = stateStack.Pop();
																																																																			break;
																																																																		} else {
																																																																			if (la.kind == 118) {
																																																																				currentState = stateStack.Pop();
																																																																				break;
																																																																			} else {
																																																																				if (la.kind == 119) {
																																																																					currentState = stateStack.Pop();
																																																																					break;
																																																																				} else {
																																																																					if (la.kind == 120) {
																																																																						currentState = stateStack.Pop();
																																																																						break;
																																																																					} else {
																																																																						if (la.kind == 121) {
																																																																							goto case 105;
																																																																						} else {
																																																																							if (la.kind == 122) {
																																																																								currentState = stateStack.Pop();
																																																																								break;
																																																																							} else {
																																																																								if (la.kind == 123) {
																																																																									currentState = stateStack.Pop();
																																																																									break;
																																																																								} else {
																																																																									if (la.kind == 124) {
																																																																										goto case 104;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 103;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 102;
																																																																												} else {
																																																																													if (la.kind == 128) {
																																																																														currentState = stateStack.Pop();
																																																																														break;
																																																																													} else {
																																																																														if (la.kind == 129) {
																																																																															currentState = stateStack.Pop();
																																																																															break;
																																																																														} else {
																																																																															if (la.kind == 130) {
																																																																																currentState = stateStack.Pop();
																																																																																break;
																																																																															} else {
																																																																																if (la.kind == 131) {
																																																																																	currentState = stateStack.Pop();
																																																																																	break;
																																																																																} else {
																																																																																	if (la.kind == 132) {
																																																																																		currentState = stateStack.Pop();
																																																																																		break;
																																																																																	} else {
																																																																																		if (la.kind == 133) {
																																																																																			goto case 101;
																																																																																		} else {
																																																																																			if (la.kind == 134) {
																																																																																				currentState = stateStack.Pop();
																																																																																				break;
																																																																																			} else {
																																																																																				if (la.kind == 135) {
																																																																																					currentState = stateStack.Pop();
																																																																																					break;
																																																																																				} else {
																																																																																					if (la.kind == 136) {
																																																																																						currentState = stateStack.Pop();
																																																																																						break;
																																																																																					} else {
																																																																																						if (la.kind == 137) {
																																																																																							currentState = stateStack.Pop();
																																																																																							break;
																																																																																						} else {
																																																																																							if (la.kind == 138) {
																																																																																								currentState = stateStack.Pop();
																																																																																								break;
																																																																																							} else {
																																																																																								if (la.kind == 139) {
																																																																																									goto case 100;
																																																																																								} else {
																																																																																									if (la.kind == 140) {
																																																																																										currentState = stateStack.Pop();
																																																																																										break;
																																																																																									} else {
																																																																																										if (la.kind == 141) {
																																																																																											currentState = stateStack.Pop();
																																																																																											break;
																																																																																										} else {
																																																																																											if (la.kind == 142) {
																																																																																												currentState = stateStack.Pop();
																																																																																												break;
																																																																																											} else {
																																																																																												if (la.kind == 143) {
																																																																																													goto case 99;
																																																																																												} else {
																																																																																													if (la.kind == 144) {
																																																																																														currentState = stateStack.Pop();
																																																																																														break;
																																																																																													} else {
																																																																																														if (la.kind == 145) {
																																																																																															currentState = stateStack.Pop();
																																																																																															break;
																																																																																														} else {
																																																																																															if (la.kind == 146) {
																																																																																																goto case 98;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 97;
																																																																																																} else {
																																																																																																	if (la.kind == 148) {
																																																																																																		currentState = stateStack.Pop();
																																																																																																		break;
																																																																																																	} else {
																																																																																																		if (la.kind == 149) {
																																																																																																			currentState = stateStack.Pop();
																																																																																																			break;
																																																																																																		} else {
																																																																																																			if (la.kind == 150) {
																																																																																																				currentState = stateStack.Pop();
																																																																																																				break;
																																																																																																			} else {
																																																																																																				if (la.kind == 151) {
																																																																																																					currentState = stateStack.Pop();
																																																																																																					break;
																																																																																																				} else {
																																																																																																					if (la.kind == 152) {
																																																																																																						currentState = stateStack.Pop();
																																																																																																						break;
																																																																																																					} else {
																																																																																																						if (la.kind == 153) {
																																																																																																							currentState = stateStack.Pop();
																																																																																																							break;
																																																																																																						} else {
																																																																																																							if (la.kind == 154) {
																																																																																																								currentState = stateStack.Pop();
																																																																																																								break;
																																																																																																							} else {
																																																																																																								if (la.kind == 155) {
																																																																																																									currentState = stateStack.Pop();
																																																																																																									break;
																																																																																																								} else {
																																																																																																									if (la.kind == 156) {
																																																																																																										currentState = stateStack.Pop();
																																																																																																										break;
																																																																																																									} else {
																																																																																																										if (la.kind == 157) {
																																																																																																											currentState = stateStack.Pop();
																																																																																																											break;
																																																																																																										} else {
																																																																																																											if (la.kind == 158) {
																																																																																																												currentState = stateStack.Pop();
																																																																																																												break;
																																																																																																											} else {
																																																																																																												if (la.kind == 159) {
																																																																																																													currentState = stateStack.Pop();
																																																																																																													break;
																																																																																																												} else {
																																																																																																													if (la.kind == 160) {
																																																																																																														currentState = stateStack.Pop();
																																																																																																														break;
																																																																																																													} else {
																																																																																																														if (la.kind == 161) {
																																																																																																															currentState = stateStack.Pop();
																																																																																																															break;
																																																																																																														} else {
																																																																																																															if (la.kind == 162) {
																																																																																																																goto case 96;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 95;
																																																																																																																} else {
																																																																																																																	if (la.kind == 164) {
																																																																																																																		currentState = stateStack.Pop();
																																																																																																																		break;
																																																																																																																	} else {
																																																																																																																		if (la.kind == 165) {
																																																																																																																			currentState = stateStack.Pop();
																																																																																																																			break;
																																																																																																																		} else {
																																																																																																																			if (la.kind == 166) {
																																																																																																																				currentState = stateStack.Pop();
																																																																																																																				break;
																																																																																																																			} else {
																																																																																																																				if (la.kind == 167) {
																																																																																																																					currentState = stateStack.Pop();
																																																																																																																					break;
																																																																																																																				} else {
																																																																																																																					if (la.kind == 168) {
																																																																																																																						currentState = stateStack.Pop();
																																																																																																																						break;
																																																																																																																					} else {
																																																																																																																						if (la.kind == 169) {
																																																																																																																							currentState = stateStack.Pop();
																																																																																																																							break;
																																																																																																																						} else {
																																																																																																																							if (la.kind == 170) {
																																																																																																																								goto case 94;
																																																																																																																							} else {
																																																																																																																								if (la.kind == 171) {
																																																																																																																									currentState = stateStack.Pop();
																																																																																																																									break;
																																																																																																																								} else {
																																																																																																																									if (la.kind == 172) {
																																																																																																																										currentState = stateStack.Pop();
																																																																																																																										break;
																																																																																																																									} else {
																																																																																																																										if (la.kind == 173) {
																																																																																																																											currentState = stateStack.Pop();
																																																																																																																											break;
																																																																																																																										} else {
																																																																																																																											if (la.kind == 174) {
																																																																																																																												currentState = stateStack.Pop();
																																																																																																																												break;
																																																																																																																											} else {
																																																																																																																												if (la.kind == 175) {
																																																																																																																													currentState = stateStack.Pop();
																																																																																																																													break;
																																																																																																																												} else {
																																																																																																																													if (la.kind == 176) {
																																																																																																																														goto case 93;
																																																																																																																													} else {
																																																																																																																														if (la.kind == 177) {
																																																																																																																															currentState = stateStack.Pop();
																																																																																																																															break;
																																																																																																																														} else {
																																																																																																																															if (la.kind == 178) {
																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																break;
																																																																																																																															} else {
																																																																																																																																if (la.kind == 179) {
																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																	break;
																																																																																																																																} else {
																																																																																																																																	if (la.kind == 180) {
																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																		break;
																																																																																																																																	} else {
																																																																																																																																		if (la.kind == 181) {
																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																			break;
																																																																																																																																		} else {
																																																																																																																																			if (la.kind == 182) {
																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																				break;
																																																																																																																																			} else {
																																																																																																																																				if (la.kind == 183) {
																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																					break;
																																																																																																																																				} else {
																																																																																																																																					if (la.kind == 184) {
																																																																																																																																						goto case 92;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 91;
																																																																																																																																							} else {
																																																																																																																																								if (la.kind == 187) {
																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																									break;
																																																																																																																																								} else {
																																																																																																																																									if (la.kind == 188) {
																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																										break;
																																																																																																																																									} else {
																																																																																																																																										if (la.kind == 189) {
																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																											break;
																																																																																																																																										} else {
																																																																																																																																											if (la.kind == 190) {
																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																												break;
																																																																																																																																											} else {
																																																																																																																																												if (la.kind == 191) {
																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																													break;
																																																																																																																																												} else {
																																																																																																																																													if (la.kind == 192) {
																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																														break;
																																																																																																																																													} else {
																																																																																																																																														if (la.kind == 193) {
																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																															break;
																																																																																																																																														} else {
																																																																																																																																															if (la.kind == 194) {
																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																break;
																																																																																																																																															} else {
																																																																																																																																																if (la.kind == 195) {
																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																	break;
																																																																																																																																																} else {
																																																																																																																																																	if (la.kind == 196) {
																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																		break;
																																																																																																																																																	} else {
																																																																																																																																																		if (la.kind == 197) {
																																																																																																																																																			goto case 90;
																																																																																																																																																		} else {
																																																																																																																																																			if (la.kind == 198) {
																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																				break;
																																																																																																																																																			} else {
																																																																																																																																																				if (la.kind == 199) {
																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																					break;
																																																																																																																																																				} else {
																																																																																																																																																					if (la.kind == 200) {
																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																						break;
																																																																																																																																																					} else {
																																																																																																																																																						if (la.kind == 201) {
																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																							break;
																																																																																																																																																						} else {
																																																																																																																																																							if (la.kind == 202) {
																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																								break;
																																																																																																																																																							} else {
																																																																																																																																																								if (la.kind == 203) {
																																																																																																																																																									goto case 89;
																																																																																																																																																								} else {
																																																																																																																																																									if (la.kind == 204) {
																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																										break;
																																																																																																																																																									} else {
																																																																																																																																																										if (la.kind == 205) {
																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																											break;
																																																																																																																																																										} else {
																																																																																																																																																											if (la.kind == 206) {
																																																																																																																																																												goto case 88;
																																																																																																																																																											} else {
																																																																																																																																																												if (la.kind == 207) {
																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																													break;
																																																																																																																																																												} else {
																																																																																																																																																													if (la.kind == 208) {
																																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																																														break;
																																																																																																																																																													} else {
																																																																																																																																																														if (la.kind == 209) {
																																																																																																																																																															goto case 87;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 86;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 85;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 84;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 83;
																																																																																																																																																																		} else {
																																																																																																																																																																			if (la.kind == 214) {
																																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																																				break;
																																																																																																																																																																			} else {
																																																																																																																																																																				if (la.kind == 215) {
																																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																																					break;
																																																																																																																																																																				} else {
																																																																																																																																																																					if (la.kind == 216) {
																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																						break;
																																																																																																																																																																					} else {
																																																																																																																																																																						if (la.kind == 217) {
																																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																																							break;
																																																																																																																																																																						} else {
																																																																																																																																																																							if (la.kind == 218) {
																																																																																																																																																																								goto case 82;
																																																																																																																																																																							} else {
																																																																																																																																																																								if (la.kind == 219) {
																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																									break;
																																																																																																																																																																								} else {
																																																																																																																																																																									if (la.kind == 220) {
																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																										break;
																																																																																																																																																																									} else {
																																																																																																																																																																										if (la.kind == 221) {
																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																											break;
																																																																																																																																																																										} else {
																																																																																																																																																																											if (la.kind == 222) {
																																																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																																																												break;
																																																																																																																																																																											} else {
																																																																																																																																																																												if (la.kind == 223) {
																																																																																																																																																																													goto case 81;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 80;
																																																																																																																																																																													} else {
																																																																																																																																																																														if (la.kind == 225) {
																																																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																																																															break;
																																																																																																																																																																														} else {
																																																																																																																																																																															if (la.kind == 226) {
																																																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																																																break;
																																																																																																																																																																															} else {
																																																																																																																																																																																if (la.kind == 227) {
																																																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																																																	break;
																																																																																																																																																																																} else {
																																																																																																																																																																																	if (la.kind == 228) {
																																																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																																																		break;
																																																																																																																																																																																	} else {
																																																																																																																																																																																		if (la.kind == 229) {
																																																																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																																																																			break;
																																																																																																																																																																																		} else {
																																																																																																																																																																																			if (la.kind == 230) {
																																																																																																																																																																																				goto case 79;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 78;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 77;
																																																																																																																																																																																						} else {
																																																																																																																																																																																							if (la.kind == 234) {
																																																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																																																								break;
																																																																																																																																																																																							} else {
																																																																																																																																																																																								if (la.kind == 235) {
																																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																																									break;
																																																																																																																																																																																								} else {
																																																																																																																																																																																									if (la.kind == 236) {
																																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																																										break;
																																																																																																																																																																																									} else {
																																																																																																																																																																																										if (la.kind == 237) {
																																																																																																																																																																																											currentState = stateStack.Pop();
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
			case 77: {
				if (la == null) { currentState = 77; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 122;
						break;
					} else {
						goto case 65;
					}
				} else {
					goto case 67;
				}
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				if (la.kind == 35) {
					stateStack.Push(67);
					goto case 58;
				} else {
					if (set[27].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 66;
					}
				}
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				currentState = 67;
				break;
			}
			case 124: {
				stateStack.Push(125);
				goto case 54;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				Expect(144, la); // "Is"
				currentState = 126;
				break;
			}
			case 126: {
				stateStack.Push(57);
				goto case 32;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (set[29].Get(la.kind)) {
					stateStack.Push(127);
					goto case 128;
				} else {
					goto case 57;
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 37) {
					currentState = 133;
					break;
				} else {
					if (set[126].Get(la.kind)) {
						currentState = 129;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 129: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 130;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 10) {
					currentState = 131;
					break;
				} else {
					goto case 131;
				}
			}
			case 131: {
				stateStack.Push(132);
				goto case 76;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 133: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 134;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 169) {
					currentState = 136;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(135);
							goto case 43;
						} else {
							goto case 135;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 135: {
				PopContext();
				goto case 40;
			}
			case 136: {
				stateStack.Push(137);
				goto case 32;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.kind == 22) {
					currentState = 136;
					break;
				} else {
					goto case 40;
				}
			}
			case 138: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 139;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (set[127].Get(la.kind)) {
					currentState = 140;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 419;
						break;
					} else {
						if (set[128].Get(la.kind)) {
							currentState = 140;
							break;
						} else {
							if (set[123].Get(la.kind)) {
								currentState = 140;
								break;
							} else {
								if (set[126].Get(la.kind)) {
									currentState = 415;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 413;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 410;
											break;
										} else {
											if (set[76].Get(la.kind)) {
												stateStack.Push(140);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 393;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(140);
													goto case 214;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(140);
														PushContext(Context.Query, la, t);
														goto case 154;
													} else {
														if (set[34].Get(la.kind)) {
															stateStack.Push(140);
															goto case 148;
														} else {
															if (la.kind == 135) {
																stateStack.Push(140);
																goto case 141;
															} else {
																Error(la);
																goto case 140;
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
			case 140: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				Expect(135, la); // "If"
				currentState = 142;
				break;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(37, la); // "("
				currentState = 143;
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 51;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(22, la); // ","
				currentState = 145;
				break;
			}
			case 145: {
				stateStack.Push(146);
				goto case 51;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 22) {
					currentState = 147;
					break;
				} else {
					goto case 40;
				}
			}
			case 147: {
				stateStack.Push(40);
				goto case 51;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (set[129].Get(la.kind)) {
					currentState = 153;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 149;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(37, la); // "("
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(151);
				goto case 51;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(22, la); // ","
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(40);
				goto case 32;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				Expect(37, la); // "("
				currentState = 147;
				break;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 126) {
					stateStack.Push(155);
					goto case 213;
				} else {
					if (la.kind == 58) {
						stateStack.Push(155);
						goto case 212;
					} else {
						Error(la);
						goto case 155;
					}
				}
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(155);
					goto case 156;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 126) {
					currentState = 210;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 206;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 204;
							break;
						} else {
							if (la.kind == 107) {
								goto case 108;
							} else {
								if (la.kind == 230) {
									currentState = 51;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 200;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 198;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 196;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 170;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 157;
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
			case 157: {
				stateStack.Push(158);
				goto case 163;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(171, la); // "On"
				currentState = 159;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 51;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				Expect(116, la); // "Equals"
				currentState = 161;
				break;
			}
			case 161: {
				stateStack.Push(162);
				goto case 51;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				if (la.kind == 22) {
					currentState = 159;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 163: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(164);
				goto case 169;
			}
			case 164: {
				PopContext();
				goto case 165;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 63) {
					currentState = 167;
					break;
				} else {
					goto case 166;
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(138, la); // "In"
				currentState = 51;
				break;
			}
			case 167: {
				PushContext(Context.Type, la, t);
				stateStack.Push(168);
				goto case 32;
			}
			case 168: {
				PopContext();
				goto case 166;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (set[114].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 110;
					} else {
						goto case 6;
					}
				}
			}
			case 170: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 171;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.kind == 146) {
					goto case 188;
				} else {
					if (set[37].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 173;
							break;
						} else {
							if (set[37].Get(la.kind)) {
								goto case 186;
							} else {
								Error(la);
								goto case 172;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(70, la); // "By"
				currentState = 173;
				break;
			}
			case 173: {
				stateStack.Push(174);
				goto case 177;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 22) {
					currentState = 173;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 175;
					break;
				}
			}
			case 175: {
				stateStack.Push(176);
				goto case 177;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 22) {
					currentState = 175;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 177: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(179);
					goto case 169;
				} else {
					goto case 51;
				}
			}
			case 179: {
				PopContext();
				goto case 180;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 63) {
					currentState = 183;
					break;
				} else {
					if (la.kind == 20) {
						goto case 182;
					} else {
						if (set[38].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 51;
						}
					}
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				currentState = 51;
				break;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				currentState = 51;
				break;
			}
			case 183: {
				PushContext(Context.Type, la, t);
				stateStack.Push(184);
				goto case 32;
			}
			case 184: {
				PopContext();
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				Expect(20, la); // "="
				currentState = 51;
				break;
			}
			case 186: {
				stateStack.Push(187);
				goto case 177;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 22) {
					currentState = 186;
					break;
				} else {
					goto case 172;
				}
			}
			case 188: {
				stateStack.Push(189);
				goto case 195;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 193;
						break;
					} else {
						if (la.kind == 146) {
							goto case 188;
						} else {
							Error(la);
							goto case 189;
						}
					}
				} else {
					goto case 190;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(143, la); // "Into"
				currentState = 191;
				break;
			}
			case 191: {
				stateStack.Push(192);
				goto case 177;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (la.kind == 22) {
					currentState = 191;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 193: {
				stateStack.Push(194);
				goto case 195;
			}
			case 194: {
				stateStack.Push(189);
				goto case 190;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				Expect(146, la); // "Join"
				currentState = 157;
				break;
			}
			case 196: {
				stateStack.Push(197);
				goto case 177;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 22) {
					currentState = 196;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 198: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 199;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				if (la.kind == 231) {
					currentState = 51;
					break;
				} else {
					goto case 51;
				}
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				Expect(70, la); // "By"
				currentState = 201;
				break;
			}
			case 201: {
				stateStack.Push(202);
				goto case 51;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 64) {
					currentState = 203;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 203;
						break;
					} else {
						Error(la);
						goto case 203;
					}
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 22) {
					currentState = 201;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 177;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (la.kind == 22) {
					currentState = 204;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 163;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(207);
					goto case 156;
				} else {
					Expect(143, la); // "Into"
					currentState = 208;
					break;
				}
			}
			case 208: {
				stateStack.Push(209);
				goto case 177;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 22) {
					currentState = 208;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				stateStack.Push(211);
				goto case 163;
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
				if (la == null) { currentState = 212; break; }
				Expect(58, la); // "Aggregate"
				currentState = 206;
				break;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(126, la); // "From"
				currentState = 210;
				break;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 210) {
					currentState = 385;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 215;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				Expect(37, la); // "("
				currentState = 216;
				break;
			}
			case 216: {
				SetIdentifierExpected(la);
				goto case 217;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(218);
					goto case 367;
				} else {
					goto case 218;
				}
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				Expect(38, la); // ")"
				currentState = 219;
				break;
			}
			case 219: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 220;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (set[22].Get(la.kind)) {
					goto case 51;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 365;
							break;
						} else {
							goto case 221;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 221: {
				stateStack.Push(222);
				goto case 224;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(113, la); // "End"
				currentState = 223;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				PushContext(Context.Body, la, t);
				goto case 225;
			}
			case 225: {
				stateStack.Push(226);
				goto case 18;
			}
			case 226: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 227;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (set[130].Get(la.kind)) {
					if (set[64].Get(la.kind)) {
						if (set[46].Get(la.kind)) {
							stateStack.Push(225);
							goto case 232;
						} else {
							goto case 225;
						}
					} else {
						if (la.kind == 113) {
							currentState = 230;
							break;
						} else {
							goto case 229;
						}
					}
				} else {
					goto case 228;
				}
			}
			case 228: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 229: {
				Error(la);
				goto case 226;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 225;
				} else {
					if (set[45].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 229;
					}
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				currentState = 226;
				break;
			}
			case 232: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 233;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 349;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 345;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 343;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 341;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 323;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 308;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 304;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 298;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 271;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 267;
																break;
															} else {
																goto case 267;
															}
														} else {
															if (la.kind == 194) {
																currentState = 265;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 263;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 250;
																break;
															} else {
																if (set[131].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 247;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 246;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 245;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 88;
																				} else {
																					if (la.kind == 195) {
																						currentState = 242;
																						break;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 240;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 238;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 234;
																				break;
																			} else {
																				if (set[132].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 51;
																						break;
																					} else {
																						goto case 51;
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
			case 234: {
				stateStack.Push(235);
				goto case 51;
			}
			case 235: {
				stateStack.Push(236);
				goto case 224;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(113, la); // "End"
				currentState = 237;
				break;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				stateStack.Push(239);
				goto case 51;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (la.kind == 22) {
					currentState = 238;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 240: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (la.kind == 184) {
					currentState = 51;
					break;
				} else {
					goto case 51;
				}
			}
			case 242: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(244);
					goto case 51;
				} else {
					goto case 244;
				}
			}
			case 244: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (la.kind == 108) {
					goto case 107;
				} else {
					if (la.kind == 124) {
						goto case 104;
					} else {
						if (la.kind == 231) {
							goto case 78;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 108) {
					goto case 107;
				} else {
					if (la.kind == 124) {
						goto case 104;
					} else {
						if (la.kind == 231) {
							goto case 78;
						} else {
							if (la.kind == 197) {
								goto case 90;
							} else {
								if (la.kind == 210) {
									goto case 86;
								} else {
									if (la.kind == 127) {
										goto case 102;
									} else {
										if (la.kind == 186) {
											goto case 91;
										} else {
											if (la.kind == 218) {
												goto case 82;
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
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (set[6].Get(la.kind)) {
					goto case 249;
				} else {
					if (la.kind == 5) {
						goto case 248;
					} else {
						goto case 6;
					}
				}
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 250: {
				stateStack.Push(251);
				goto case 224;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 75) {
					currentState = 255;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 254;
						break;
					} else {
						goto case 252;
					}
				}
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				Expect(113, la); // "End"
				currentState = 253;
				break;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 254: {
				stateStack.Push(252);
				goto case 224;
			}
			case 255: {
				SetIdentifierExpected(la);
				goto case 256;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(259);
					goto case 169;
				} else {
					goto case 257;
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (la.kind == 229) {
					currentState = 258;
					break;
				} else {
					goto case 250;
				}
			}
			case 258: {
				stateStack.Push(250);
				goto case 51;
			}
			case 259: {
				PopContext();
				goto case 260;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (la.kind == 63) {
					currentState = 261;
					break;
				} else {
					goto case 257;
				}
			}
			case 261: {
				PushContext(Context.Type, la, t);
				stateStack.Push(262);
				goto case 32;
			}
			case 262: {
				PopContext();
				goto case 257;
			}
			case 263: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 264;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (set[22].Get(la.kind)) {
					goto case 51;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.kind == 163) {
					goto case 95;
				} else {
					goto case 266;
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 5) {
					goto case 248;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 249;
					} else {
						goto case 6;
					}
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				Expect(118, la); // "Error"
				currentState = 268;
				break;
			}
			case 268: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 269;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (set[22].Get(la.kind)) {
					goto case 51;
				} else {
					if (la.kind == 132) {
						currentState = 266;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 270;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 271: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 272;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(288);
					goto case 282;
				} else {
					if (la.kind == 110) {
						currentState = 273;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 273: {
				stateStack.Push(274);
				goto case 282;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				Expect(138, la); // "In"
				currentState = 275;
				break;
			}
			case 275: {
				stateStack.Push(276);
				goto case 51;
			}
			case 276: {
				stateStack.Push(277);
				goto case 224;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				Expect(163, la); // "Next"
				currentState = 278;
				break;
			}
			case 278: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 279;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[22].Get(la.kind)) {
					goto case 280;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 280: {
				stateStack.Push(281);
				goto case 51;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 22) {
					currentState = 280;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 282: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(283);
				goto case 138;
			}
			case 283: {
				PopContext();
				goto case 284;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 33) {
					currentState = 285;
					break;
				} else {
					goto case 285;
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (set[29].Get(la.kind)) {
					stateStack.Push(285);
					goto case 128;
				} else {
					if (la.kind == 63) {
						currentState = 286;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 286: {
				PushContext(Context.Type, la, t);
				stateStack.Push(287);
				goto case 32;
			}
			case 287: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				Expect(20, la); // "="
				currentState = 289;
				break;
			}
			case 289: {
				stateStack.Push(290);
				goto case 51;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (la.kind == 205) {
					currentState = 297;
					break;
				} else {
					goto case 291;
				}
			}
			case 291: {
				stateStack.Push(292);
				goto case 224;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				Expect(163, la); // "Next"
				currentState = 293;
				break;
			}
			case 293: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 294;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (set[22].Get(la.kind)) {
					goto case 295;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 295: {
				stateStack.Push(296);
				goto case 51;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 22) {
					currentState = 295;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 297: {
				stateStack.Push(291);
				goto case 51;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 301;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(299);
						goto case 224;
					} else {
						goto case 6;
					}
				}
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				Expect(152, la); // "Loop"
				currentState = 300;
				break;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 51;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 301: {
				stateStack.Push(302);
				goto case 51;
			}
			case 302: {
				stateStack.Push(303);
				goto case 224;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				stateStack.Push(305);
				goto case 51;
			}
			case 305: {
				stateStack.Push(306);
				goto case 224;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(113, la); // "End"
				currentState = 307;
				break;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 308: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 309;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 74) {
					currentState = 310;
					break;
				} else {
					goto case 310;
				}
			}
			case 310: {
				stateStack.Push(311);
				goto case 51;
			}
			case 311: {
				stateStack.Push(312);
				goto case 18;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 74) {
					currentState = 314;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 313;
					break;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 314: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 315;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 111) {
					currentState = 316;
					break;
				} else {
					if (set[62].Get(la.kind)) {
						goto case 317;
					} else {
						Error(la);
						goto case 316;
					}
				}
			}
			case 316: {
				stateStack.Push(312);
				goto case 224;
			}
			case 317: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 318;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 320;
						break;
					} else {
						goto case 320;
					}
				} else {
					if (set[22].Get(la.kind)) {
						stateStack.Push(319);
						goto case 51;
					} else {
						Error(la);
						goto case 319;
					}
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 22) {
					currentState = 317;
					break;
				} else {
					goto case 316;
				}
			}
			case 320: {
				stateStack.Push(321);
				goto case 322;
			}
			case 321: {
				stateStack.Push(319);
				goto case 54;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 20) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 41) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 40) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 39) {
								currentState = stateStack.Pop();
								break;
							} else {
								if (la.kind == 42) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 43) {
										currentState = stateStack.Pop();
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
			case 323: {
				stateStack.Push(324);
				goto case 51;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 214) {
					currentState = 332;
					break;
				} else {
					goto case 325;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 326;
				} else {
					goto case 6;
				}
			}
			case 326: {
				stateStack.Push(327);
				goto case 224;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 331;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 329;
							break;
						} else {
							Error(la);
							goto case 326;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 328;
					break;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 329: {
				stateStack.Push(330);
				goto case 51;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (la.kind == 214) {
					currentState = 326;
					break;
				} else {
					goto case 326;
				}
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (la.kind == 135) {
					currentState = 329;
					break;
				} else {
					goto case 326;
				}
			}
			case 332: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 333;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (set[46].Get(la.kind)) {
					goto case 334;
				} else {
					goto case 325;
				}
			}
			case 334: {
				stateStack.Push(335);
				goto case 232;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 21) {
					currentState = 339;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 336;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 336: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 337;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (set[46].Get(la.kind)) {
					stateStack.Push(338);
					goto case 232;
				} else {
					goto case 338;
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 21) {
					currentState = 336;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 339: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 340;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (set[46].Get(la.kind)) {
					goto case 334;
				} else {
					goto case 335;
				}
			}
			case 341: {
				stateStack.Push(342);
				goto case 76;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 37) {
					currentState = 41;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 343: {
				stateStack.Push(344);
				goto case 51;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				Expect(22, la); // ","
				currentState = 51;
				break;
			}
			case 345: {
				stateStack.Push(346);
				goto case 51;
			}
			case 346: {
				stateStack.Push(347);
				goto case 224;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				Expect(113, la); // "End"
				currentState = 348;
				break;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 233) {
					goto case 77;
				} else {
					if (la.kind == 211) {
						goto case 85;
					} else {
						goto case 6;
					}
				}
			}
			case 349: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(350);
				goto case 169;
			}
			case 350: {
				PopContext();
				goto case 351;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 33) {
					currentState = 352;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 37) {
					currentState = 364;
					break;
				} else {
					goto case 353;
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 22) {
					currentState = 358;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 355;
						break;
					} else {
						goto case 354;
					}
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 20) {
					goto case 182;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 355: {
				PushContext(Context.Type, la, t);
				goto case 356;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 162) {
					stateStack.Push(357);
					goto case 62;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(357);
						goto case 32;
					} else {
						Error(la);
						goto case 357;
					}
				}
			}
			case 357: {
				PopContext();
				goto case 354;
			}
			case 358: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(359);
				goto case 169;
			}
			case 359: {
				PopContext();
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 33) {
					currentState = 361;
					break;
				} else {
					goto case 361;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 37) {
					currentState = 362;
					break;
				} else {
					goto case 353;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 22) {
					currentState = 362;
					break;
				} else {
					goto case 363;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(38, la); // ")"
				currentState = 353;
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 22) {
					currentState = 364;
					break;
				} else {
					goto case 363;
				}
			}
			case 365: {
				PushContext(Context.Type, la, t);
				stateStack.Push(366);
				goto case 32;
			}
			case 366: {
				PopContext();
				goto case 221;
			}
			case 367: {
				stateStack.Push(368);
				PushContext(Context.Parameter, la, t);
				goto case 369;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (la.kind == 22) {
					currentState = 367;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 369: {
				SetIdentifierExpected(la);
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 40) {
					stateStack.Push(369);
					goto case 380;
				} else {
					goto case 371;
				}
			}
			case 371: {
				SetIdentifierExpected(la);
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (set[134].Get(la.kind)) {
					currentState = 371;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(373);
					goto case 169;
				}
			}
			case 373: {
				PopContext();
				goto case 374;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 63) {
					currentState = 378;
					break;
				} else {
					goto case 375;
				}
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 20) {
					currentState = 377;
					break;
				} else {
					goto case 376;
				}
			}
			case 376: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 377: {
				stateStack.Push(376);
				goto case 51;
			}
			case 378: {
				PushContext(Context.Type, la, t);
				stateStack.Push(379);
				goto case 32;
			}
			case 379: {
				PopContext();
				goto case 375;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				Expect(40, la); // "<"
				currentState = 381;
				break;
			}
			case 381: {
				PushContext(Context.Attribute, la, t);
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[135].Get(la.kind)) {
					currentState = 382;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 383;
					break;
				}
			}
			case 383: {
				PopContext();
				goto case 384;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				Expect(37, la); // "("
				currentState = 386;
				break;
			}
			case 386: {
				SetIdentifierExpected(la);
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(388);
					goto case 367;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(38, la); // ")"
				currentState = 389;
				break;
			}
			case 389: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (set[46].Get(la.kind)) {
					goto case 232;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(391);
						goto case 224;
					} else {
						goto case 6;
					}
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(113, la); // "End"
				currentState = 392;
				break;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 406;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(395);
						goto case 397;
					} else {
						Error(la);
						goto case 394;
					}
				}
			}
			case 394: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 17) {
					currentState = 396;
					break;
				} else {
					goto case 394;
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 16) {
					currentState = 395;
					break;
				} else {
					goto case 395;
				}
			}
			case 397: {
				PushContext(Context.Xml, la, t);
				goto case 398;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 399;
				break;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 399;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(399);
							goto case 403;
						} else {
							Error(la);
							goto case 399;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 400;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 401;
							break;
						} else {
							Error(la);
							goto case 400;
						}
					}
				}
			}
			case 400: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 401;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(401);
							goto case 403;
						} else {
							if (la.kind == 10) {
								stateStack.Push(401);
								goto case 397;
							} else {
								Error(la);
								goto case 401;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 402;
					break;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 402;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(402);
							goto case 403;
						} else {
							Error(la);
							goto case 402;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 400;
					break;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 404;
				break;
			}
			case 404: {
				stateStack.Push(405);
				goto case 51;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 16) {
					currentState = 407;
					break;
				} else {
					goto case 407;
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 406;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(408);
						goto case 397;
					} else {
						goto case 394;
					}
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (la.kind == 17) {
					currentState = 409;
					break;
				} else {
					goto case 394;
				}
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 16) {
					currentState = 408;
					break;
				} else {
					goto case 408;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(37, la); // "("
				currentState = 411;
				break;
			}
			case 411: {
				readXmlIdentifier = true;
				stateStack.Push(412);
				goto case 169;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				Expect(38, la); // ")"
				currentState = 140;
				break;
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(37, la); // "("
				currentState = 414;
				break;
			}
			case 414: {
				stateStack.Push(412);
				goto case 32;
			}
			case 415: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 416;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 10) {
					currentState = 417;
					break;
				} else {
					goto case 417;
				}
			}
			case 417: {
				stateStack.Push(418);
				goto case 76;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 11) {
					currentState = 140;
					break;
				} else {
					goto case 140;
				}
			}
			case 419: {
				stateStack.Push(412);
				goto case 51;
			}
			case 420: {
				stateStack.Push(421);
				goto case 51;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 22) {
					currentState = 422;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 422: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 423;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (set[22].Get(la.kind)) {
					goto case 420;
				} else {
					goto case 421;
				}
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(425);
					goto case 32;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 22) {
					currentState = 424;
					break;
				} else {
					goto case 40;
				}
			}
			case 426: {
				SetIdentifierExpected(la);
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 429;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(428);
							goto case 367;
						} else {
							Error(la);
							goto case 428;
						}
					}
				} else {
					goto case 428;
				}
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				Expect(38, la); // ")"
				currentState = 29;
				break;
			}
			case 429: {
				stateStack.Push(428);
				goto case 430;
			}
			case 430: {
				SetIdentifierExpected(la);
				goto case 431;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 432;
					break;
				} else {
					goto case 432;
				}
			}
			case 432: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(433);
				goto case 447;
			}
			case 433: {
				PopContext();
				goto case 434;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 63) {
					currentState = 448;
					break;
				} else {
					goto case 435;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 22) {
					currentState = 436;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 436: {
				SetIdentifierExpected(la);
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 438;
					break;
				} else {
					goto case 438;
				}
			}
			case 438: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(439);
				goto case 447;
			}
			case 439: {
				PopContext();
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 63) {
					currentState = 441;
					break;
				} else {
					goto case 435;
				}
			}
			case 441: {
				PushContext(Context.Type, la, t);
				stateStack.Push(442);
				goto case 443;
			}
			case 442: {
				PopContext();
				goto case 435;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (set[87].Get(la.kind)) {
					goto case 446;
				} else {
					if (la.kind == 35) {
						currentState = 444;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 444: {
				stateStack.Push(445);
				goto case 446;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 22) {
					currentState = 444;
					break;
				} else {
					goto case 61;
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (set[15].Get(la.kind)) {
					currentState = 33;
					break;
				} else {
					if (la.kind == 162) {
						goto case 96;
					} else {
						if (la.kind == 84) {
							goto case 112;
						} else {
							if (la.kind == 209) {
								goto case 87;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (la.kind == 2) {
					goto case 120;
				} else {
					if (la.kind == 62) {
						goto case 118;
					} else {
						if (la.kind == 64) {
							goto case 117;
						} else {
							if (la.kind == 65) {
								goto case 116;
							} else {
								if (la.kind == 66) {
									goto case 115;
								} else {
									if (la.kind == 67) {
										goto case 114;
									} else {
										if (la.kind == 70) {
											goto case 113;
										} else {
											if (la.kind == 87) {
												goto case 111;
											} else {
												if (la.kind == 104) {
													goto case 109;
												} else {
													if (la.kind == 107) {
														goto case 108;
													} else {
														if (la.kind == 116) {
															goto case 106;
														} else {
															if (la.kind == 121) {
																goto case 105;
															} else {
																if (la.kind == 133) {
																	goto case 101;
																} else {
																	if (la.kind == 139) {
																		goto case 100;
																	} else {
																		if (la.kind == 143) {
																			goto case 99;
																		} else {
																			if (la.kind == 146) {
																				goto case 98;
																			} else {
																				if (la.kind == 147) {
																					goto case 97;
																				} else {
																					if (la.kind == 170) {
																						goto case 94;
																					} else {
																						if (la.kind == 176) {
																							goto case 93;
																						} else {
																							if (la.kind == 184) {
																								goto case 92;
																							} else {
																								if (la.kind == 203) {
																									goto case 89;
																								} else {
																									if (la.kind == 212) {
																										goto case 84;
																									} else {
																										if (la.kind == 213) {
																											goto case 83;
																										} else {
																											if (la.kind == 223) {
																												goto case 81;
																											} else {
																												if (la.kind == 224) {
																													goto case 80;
																												} else {
																													if (la.kind == 230) {
																														goto case 79;
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
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 448: {
				PushContext(Context.Type, la, t);
				stateStack.Push(449);
				goto case 443;
			}
			case 449: {
				PopContext();
				goto case 435;
			}
			case 450: {
				stateStack.Push(451);
				goto case 169;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 37) {
					currentState = 455;
					break;
				} else {
					goto case 452;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 63) {
					currentState = 453;
					break;
				} else {
					goto case 18;
				}
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 40) {
					stateStack.Push(453);
					goto case 380;
				} else {
					goto case 454;
				}
			}
			case 454: {
				stateStack.Push(18);
				goto case 32;
			}
			case 455: {
				SetIdentifierExpected(la);
				goto case 456;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(457);
					goto case 367;
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				Expect(38, la); // ")"
				currentState = 452;
				break;
			}
			case 458: {
				stateStack.Push(459);
				goto case 169;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 454;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 461;
							break;
						} else {
							goto case 460;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 460: {
				Error(la);
				goto case 18;
			}
			case 461: {
				SetIdentifierExpected(la);
				goto case 462;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(463);
					goto case 367;
				} else {
					goto case 463;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				Expect(115, la); // "Enum"
				currentState = 466;
				break;
			}
			case 466: {
				stateStack.Push(467);
				goto case 169;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 63) {
					currentState = 474;
					break;
				} else {
					goto case 468;
				}
			}
			case 468: {
				stateStack.Push(469);
				goto case 18;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (set[90].Get(la.kind)) {
					goto case 471;
				} else {
					Expect(113, la); // "End"
					currentState = 470;
					break;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (la.kind == 40) {
					stateStack.Push(471);
					goto case 380;
				} else {
					stateStack.Push(472);
					goto case 169;
				}
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 20) {
					currentState = 473;
					break;
				} else {
					goto case 468;
				}
			}
			case 473: {
				stateStack.Push(468);
				goto case 51;
			}
			case 474: {
				stateStack.Push(468);
				goto case 32;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				Expect(103, la); // "Delegate"
				currentState = 476;
				break;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 210) {
					currentState = 477;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 477;
						break;
					} else {
						Error(la);
						goto case 477;
					}
				}
			}
			case 477: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				currentState = 479;
				break;
			}
			case 479: {
				PopContext();
				goto case 480;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				if (la.kind == 37) {
					currentState = 484;
					break;
				} else {
					goto case 481;
				}
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 63) {
					currentState = 482;
					break;
				} else {
					goto case 18;
				}
			}
			case 482: {
				PushContext(Context.Type, la, t);
				stateStack.Push(483);
				goto case 32;
			}
			case 483: {
				PopContext();
				goto case 18;
			}
			case 484: {
				SetIdentifierExpected(la);
				goto case 485;
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(486);
					goto case 367;
				} else {
					goto case 486;
				}
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				Expect(38, la); // ")"
				currentState = 481;
				break;
			}
			case 487: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 488;
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				if (la.kind == 155) {
					currentState = 489;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 489;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 489;
							break;
						} else {
							Error(la);
							goto case 489;
						}
					}
				}
			}
			case 489: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(490);
				goto case 169;
			}
			case 490: {
				PopContext();
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 37) {
					currentState = 640;
					break;
				} else {
					goto case 492;
				}
			}
			case 492: {
				stateStack.Push(493);
				goto case 18;
			}
			case 493: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 637;
				} else {
					goto case 495;
				}
			}
			case 495: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 631;
				} else {
					goto case 497;
				}
			}
			case 497: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 498;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (set[95].Get(la.kind)) {
					goto case 503;
				} else {
					isMissingModifier = false;
					goto case 499;
				}
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				Expect(113, la); // "End"
				currentState = 500;
				break;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 155) {
					currentState = 501;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 501;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 501;
							break;
						} else {
							Error(la);
							goto case 501;
						}
					}
				}
			}
			case 501: {
				stateStack.Push(502);
				goto case 18;
			}
			case 502: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 503: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (la.kind == 40) {
					stateStack.Push(503);
					goto case 380;
				} else {
					isMissingModifier = true;
					goto case 505;
				}
			}
			case 505: {
				SetIdentifierExpected(la);
				goto case 506;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[122].Get(la.kind)) {
					currentState = 630;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 507;
				}
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(497);
					goto case 487;
				} else {
					if (la.kind == 103) {
						stateStack.Push(497);
						goto case 475;
					} else {
						if (la.kind == 115) {
							stateStack.Push(497);
							goto case 465;
						} else {
							if (la.kind == 142) {
								stateStack.Push(497);
								goto case 464;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(497);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 508;
								} else {
									Error(la);
									goto case 497;
								}
							}
						}
					}
				}
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (set[113].Get(la.kind)) {
					stateStack.Push(509);
					goto case 619;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(509);
						goto case 605;
					} else {
						if (la.kind == 101) {
							stateStack.Push(509);
							goto case 589;
						} else {
							if (la.kind == 119) {
								stateStack.Push(509);
								goto case 577;
							} else {
								if (la.kind == 98) {
									stateStack.Push(509);
									goto case 565;
								} else {
									if (la.kind == 186) {
										stateStack.Push(509);
										goto case 523;
									} else {
										if (la.kind == 172) {
											stateStack.Push(509);
											goto case 510;
										} else {
											Error(la);
											goto case 509;
										}
									}
								}
							}
						}
					}
				}
			}
			case 509: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				Expect(172, la); // "Operator"
				currentState = 511;
				break;
			}
			case 511: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 512;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				currentState = 513;
				break;
			}
			case 513: {
				PopContext();
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				Expect(37, la); // "("
				currentState = 515;
				break;
			}
			case 515: {
				stateStack.Push(516);
				goto case 367;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(38, la); // ")"
				currentState = 517;
				break;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 63) {
					currentState = 521;
					break;
				} else {
					goto case 518;
				}
			}
			case 518: {
				stateStack.Push(519);
				goto case 224;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				Expect(113, la); // "End"
				currentState = 520;
				break;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 40) {
					stateStack.Push(521);
					goto case 380;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(522);
					goto case 32;
				}
			}
			case 522: {
				PopContext();
				goto case 518;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				Expect(186, la); // "Property"
				currentState = 524;
				break;
			}
			case 524: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(525);
				goto case 169;
			}
			case 525: {
				PopContext();
				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 37) {
					currentState = 562;
					break;
				} else {
					goto case 527;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 63) {
					currentState = 560;
					break;
				} else {
					goto case 528;
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (la.kind == 136) {
					currentState = 558;
					break;
				} else {
					goto case 529;
				}
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 20) {
					currentState = 557;
					break;
				} else {
					goto case 530;
				}
			}
			case 530: {
				stateStack.Push(531);
				goto case 18;
			}
			case 531: {
				PopContext();
				goto case 532;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 40) {
					stateStack.Push(532);
					goto case 380;
				} else {
					goto case 533;
				}
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (set[143].Get(la.kind)) {
					currentState = 556;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 534;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				if (la.kind == 128) {
					currentState = 535;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 535;
						break;
					} else {
						Error(la);
						goto case 535;
					}
				}
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (la.kind == 37) {
					currentState = 553;
					break;
				} else {
					goto case 536;
				}
			}
			case 536: {
				stateStack.Push(537);
				goto case 224;
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				Expect(113, la); // "End"
				currentState = 538;
				break;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 128) {
					currentState = 539;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 539;
						break;
					} else {
						Error(la);
						goto case 539;
					}
				}
			}
			case 539: {
				stateStack.Push(540);
				goto case 18;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				if (set[104].Get(la.kind)) {
					goto case 543;
				} else {
					goto case 541;
				}
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				Expect(113, la); // "End"
				currentState = 542;
				break;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				Expect(186, la); // "Property"
				currentState = 18;
				break;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				if (la.kind == 40) {
					stateStack.Push(543);
					goto case 380;
				} else {
					goto case 544;
				}
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (set[143].Get(la.kind)) {
					currentState = 544;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 545;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 545;
							break;
						} else {
							Error(la);
							goto case 545;
						}
					}
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				if (la.kind == 37) {
					currentState = 550;
					break;
				} else {
					goto case 546;
				}
			}
			case 546: {
				stateStack.Push(547);
				goto case 224;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				Expect(113, la); // "End"
				currentState = 548;
				break;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (la.kind == 128) {
					currentState = 549;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 549;
						break;
					} else {
						Error(la);
						goto case 549;
					}
				}
			}
			case 549: {
				stateStack.Push(541);
				goto case 18;
			}
			case 550: {
				SetIdentifierExpected(la);
				goto case 551;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(552);
					goto case 367;
				} else {
					goto case 552;
				}
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				Expect(38, la); // ")"
				currentState = 546;
				break;
			}
			case 553: {
				SetIdentifierExpected(la);
				goto case 554;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(555);
					goto case 367;
				} else {
					goto case 555;
				}
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				Expect(38, la); // ")"
				currentState = 536;
				break;
			}
			case 556: {
				SetIdentifierExpected(la);
				goto case 533;
			}
			case 557: {
				stateStack.Push(530);
				goto case 51;
			}
			case 558: {
				stateStack.Push(559);
				goto case 32;
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				if (la.kind == 22) {
					currentState = 558;
					break;
				} else {
					goto case 529;
				}
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (la.kind == 40) {
					stateStack.Push(560);
					goto case 380;
				} else {
					if (la.kind == 162) {
						stateStack.Push(528);
						goto case 62;
					} else {
						if (set[15].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(561);
							goto case 32;
						} else {
							Error(la);
							goto case 528;
						}
					}
				}
			}
			case 561: {
				PopContext();
				goto case 528;
			}
			case 562: {
				SetIdentifierExpected(la);
				goto case 563;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(564);
					goto case 367;
				} else {
					goto case 564;
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(38, la); // ")"
				currentState = 527;
				break;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(98, la); // "Custom"
				currentState = 566;
				break;
			}
			case 566: {
				stateStack.Push(567);
				goto case 577;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				if (set[109].Get(la.kind)) {
					goto case 569;
				} else {
					Expect(113, la); // "End"
					currentState = 568;
					break;
				}
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				if (la.kind == 40) {
					stateStack.Push(569);
					goto case 380;
				} else {
					if (la.kind == 56) {
						currentState = 570;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 570;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 570;
								break;
							} else {
								Error(la);
								goto case 570;
							}
						}
					}
				}
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(37, la); // "("
				currentState = 571;
				break;
			}
			case 571: {
				stateStack.Push(572);
				goto case 367;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(38, la); // ")"
				currentState = 573;
				break;
			}
			case 573: {
				stateStack.Push(574);
				goto case 224;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				Expect(113, la); // "End"
				currentState = 575;
				break;
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (la.kind == 56) {
					currentState = 576;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 576;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 576;
							break;
						} else {
							Error(la);
							goto case 576;
						}
					}
				}
			}
			case 576: {
				stateStack.Push(567);
				goto case 18;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				Expect(119, la); // "Event"
				currentState = 578;
				break;
			}
			case 578: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(579);
				goto case 169;
			}
			case 579: {
				PopContext();
				goto case 580;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 63) {
					currentState = 587;
					break;
				} else {
					if (set[144].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 584;
							break;
						} else {
							goto case 581;
						}
					} else {
						Error(la);
						goto case 581;
					}
				}
			}
			case 581: {
				if (la == null) { currentState = 581; break; }
				if (la.kind == 136) {
					currentState = 582;
					break;
				} else {
					goto case 18;
				}
			}
			case 582: {
				stateStack.Push(583);
				goto case 32;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 22) {
					currentState = 582;
					break;
				} else {
					goto case 18;
				}
			}
			case 584: {
				SetIdentifierExpected(la);
				goto case 585;
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(586);
					goto case 367;
				} else {
					goto case 586;
				}
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				Expect(38, la); // ")"
				currentState = 581;
				break;
			}
			case 587: {
				PushContext(Context.Type, la, t);
				stateStack.Push(588);
				goto case 32;
			}
			case 588: {
				PopContext();
				goto case 581;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				Expect(101, la); // "Declare"
				currentState = 590;
				break;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 591;
					break;
				} else {
					goto case 591;
				}
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				if (la.kind == 210) {
					currentState = 592;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 592;
						break;
					} else {
						Error(la);
						goto case 592;
					}
				}
			}
			case 592: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(593);
				goto case 169;
			}
			case 593: {
				PopContext();
				goto case 594;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				Expect(149, la); // "Lib"
				currentState = 595;
				break;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				Expect(3, la); // LiteralString
				currentState = 596;
				break;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (la.kind == 59) {
					currentState = 604;
					break;
				} else {
					goto case 597;
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				if (la.kind == 37) {
					currentState = 601;
					break;
				} else {
					goto case 598;
				}
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				if (la.kind == 63) {
					currentState = 599;
					break;
				} else {
					goto case 18;
				}
			}
			case 599: {
				PushContext(Context.Type, la, t);
				stateStack.Push(600);
				goto case 32;
			}
			case 600: {
				PopContext();
				goto case 18;
			}
			case 601: {
				SetIdentifierExpected(la);
				goto case 602;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(603);
					goto case 367;
				} else {
					goto case 603;
				}
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(38, la); // ")"
				currentState = 598;
				break;
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				Expect(3, la); // LiteralString
				currentState = 597;
				break;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (la.kind == 210) {
					currentState = 606;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 606;
						break;
					} else {
						Error(la);
						goto case 606;
					}
				}
			}
			case 606: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				currentState = 608;
				break;
			}
			case 608: {
				PopContext();
				goto case 609;
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				if (la.kind == 37) {
					currentState = 615;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 613;
						break;
					} else {
						goto case 610;
					}
				}
			}
			case 610: {
				stateStack.Push(611);
				goto case 224;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				Expect(113, la); // "End"
				currentState = 612;
				break;
			}
			case 612: {
				if (la == null) { currentState = 612; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 613: {
				PushContext(Context.Type, la, t);
				stateStack.Push(614);
				goto case 32;
			}
			case 614: {
				PopContext();
				goto case 610;
			}
			case 615: {
				SetIdentifierExpected(la);
				goto case 616;
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 618;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(617);
							goto case 367;
						} else {
							Error(la);
							goto case 617;
						}
					}
				} else {
					goto case 617;
				}
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				Expect(38, la); // ")"
				currentState = 609;
				break;
			}
			case 618: {
				stateStack.Push(617);
				goto case 430;
			}
			case 619: {
				stateStack.Push(620);
				SetIdentifierExpected(la);
				goto case 621;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (la.kind == 22) {
					currentState = 619;
					break;
				} else {
					goto case 18;
				}
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (la.kind == 88) {
					currentState = 622;
					break;
				} else {
					goto case 622;
				}
			}
			case 622: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(623);
				goto case 629;
			}
			case 623: {
				PopContext();
				goto case 624;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				if (la.kind == 63) {
					currentState = 626;
					break;
				} else {
					goto case 625;
				}
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 20) {
					goto case 182;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 626: {
				PushContext(Context.Type, la, t);
				goto case 627;
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (la.kind == 162) {
					stateStack.Push(628);
					goto case 62;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(628);
						goto case 32;
					} else {
						Error(la);
						goto case 628;
					}
				}
			}
			case 628: {
				PopContext();
				goto case 625;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (set[128].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 119;
					} else {
						if (la.kind == 126) {
							goto case 103;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 630: {
				isMissingModifier = false;
				goto case 505;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				Expect(136, la); // "Implements"
				currentState = 632;
				break;
			}
			case 632: {
				PushContext(Context.Type, la, t);
				stateStack.Push(633);
				goto case 32;
			}
			case 633: {
				PopContext();
				goto case 634;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (la.kind == 22) {
					currentState = 635;
					break;
				} else {
					stateStack.Push(497);
					goto case 18;
				}
			}
			case 635: {
				PushContext(Context.Type, la, t);
				stateStack.Push(636);
				goto case 32;
			}
			case 636: {
				PopContext();
				goto case 634;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(140, la); // "Inherits"
				currentState = 638;
				break;
			}
			case 638: {
				PushContext(Context.Type, la, t);
				stateStack.Push(639);
				goto case 32;
			}
			case 639: {
				PopContext();
				stateStack.Push(495);
				goto case 18;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				Expect(169, la); // "Of"
				currentState = 641;
				break;
			}
			case 641: {
				stateStack.Push(642);
				goto case 430;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				Expect(38, la); // ")"
				currentState = 492;
				break;
			}
			case 643: {
				isMissingModifier = false;
				goto case 23;
			}
			case 644: {
				if (la == null) { currentState = 644; break; }
				Expect(140, la); // "Inherits"
				currentState = 645;
				break;
			}
			case 645: {
				stateStack.Push(646);
				goto case 32;
			}
			case 646: {
				PopContext();
				goto case 647;
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				if (la.kind == 22) {
					currentState = 648;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 648: {
				PushContext(Context.Type, la, t);
				stateStack.Push(649);
				goto case 32;
			}
			case 649: {
				PopContext();
				goto case 647;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				Expect(169, la); // "Of"
				currentState = 651;
				break;
			}
			case 651: {
				stateStack.Push(652);
				goto case 430;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 653: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 654;
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				if (set[45].Get(la.kind)) {
					currentState = 654;
					break;
				} else {
					PopContext();
					stateStack.Push(655);
					goto case 18;
				}
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(655);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 656;
					break;
				}
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				Expect(137, la); // "Imports"
				currentState = 658;
				break;
			}
			case 658: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 659;
			}
			case 659: {
				if (la == null) { currentState = 659; break; }
				if (set[145].Get(la.kind)) {
					currentState = 665;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 661;
						break;
					} else {
						Error(la);
						goto case 660;
					}
				}
			}
			case 660: {
				PopContext();
				goto case 18;
			}
			case 661: {
				stateStack.Push(662);
				goto case 169;
			}
			case 662: {
				if (la == null) { currentState = 662; break; }
				Expect(20, la); // "="
				currentState = 663;
				break;
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				Expect(3, la); // LiteralString
				currentState = 664;
				break;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 660;
				break;
			}
			case 665: {
				if (la == null) { currentState = 665; break; }
				if (la.kind == 37) {
					stateStack.Push(665);
					goto case 37;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 666;
						break;
					} else {
						goto case 660;
					}
				}
			}
			case 666: {
				stateStack.Push(660);
				goto case 32;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				Expect(173, la); // "Option"
				currentState = 668;
				break;
			}
			case 668: {
				if (la == null) { currentState = 668; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 670;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 669;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 670: {
				if (la == null) { currentState = 670; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 18;
					break;
				} else {
					goto case 18;
				}
			}
		}

		if (la != null) {
			t = la;
			nextTokenIsPotentialStartOfExpression = false;
			readXmlIdentifier = false;
			nextTokenIsStartOfImportsOrAccessExpression = false;
			wasQualifierTokenAtStart = false;
			identifierExpected = false;
		}
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444604417, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444596225, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671109120, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, 889192437, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, 889192405, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972018405, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018401, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1174405160, -51384097, -971985637, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969166, 17106228, -97186284, 8259}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972002021, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66189316, 1174405160, -51383073, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -971985637, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -972018405, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680476901, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483648, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483584, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7, 1157628162, 26477055, -493212676, 680323109, 2147308935, -533262382, 3395}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-318462, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-383998, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850754, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850698, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850690, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671930656, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926560, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 84218880, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 84218880, -2144073728, 65}),
		new BitArray(new int[] {3145730, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926305, 1606227074, -2143942208, 3393}),
		new BitArray(new int[] {0, 256, 0, 537001984, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 256, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {7340034, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {7340034, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 444596224, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871488, 536870912, 1522008194, 384, 3328}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680378597, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66189316, 1174405160, -51383585, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 288, 0, 0, 4210688, 0, 0}),
		new BitArray(new int[] {-2, -129, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 0, 0}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67})

	};

} // end Parser


}