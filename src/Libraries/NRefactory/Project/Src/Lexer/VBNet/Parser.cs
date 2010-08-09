using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 249;
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
			case 72:
			case 201:
			case 250:
			case 492:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 10:
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 11:
			case 177:
			case 183:
			case 189:
			case 227:
			case 231:
			case 276:
			case 376:
			case 385:
			case 438:
			case 479:
			case 489:
			case 500:
			case 530:
			case 566:
			case 623:
			case 640:
			case 712:
				return set[6];
			case 12:
			case 13:
			case 531:
			case 532:
			case 577:
			case 587:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 14:
			case 21:
			case 23:
			case 24:
			case 36:
			case 242:
			case 245:
			case 246:
			case 277:
			case 281:
			case 303:
			case 318:
			case 329:
			case 332:
			case 338:
			case 343:
			case 352:
			case 353:
			case 373:
			case 393:
			case 485:
			case 497:
			case 503:
			case 507:
			case 515:
			case 523:
			case 533:
			case 542:
			case 559:
			case 564:
			case 572:
			case 578:
			case 581:
			case 588:
			case 591:
			case 618:
			case 621:
			case 648:
			case 659:
			case 663:
			case 691:
			case 711:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 15:
			case 16:
				return set[7];
			case 17:
			case 18:
				return set[8];
			case 19:
			case 243:
			case 257:
			case 279:
			case 333:
			case 374:
			case 418:
			case 540:
			case 560:
			case 579:
			case 583:
			case 589:
			case 619:
			case 660:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 508:
			case 543:
				return set[9];
			case 25:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 26:
			case 27:
				return set[10];
			case 28:
			case 695:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 136:
			case 199:
			case 200:
			case 251:
			case 260:
			case 261:
			case 408:
			case 409:
			case 426:
			case 427:
			case 428:
			case 429:
			case 518:
			case 519:
			case 552:
			case 553:
			case 654:
			case 655:
			case 704:
			case 705:
				return set[14];
			case 33:
			case 34:
			case 480:
			case 481:
			case 490:
			case 491:
			case 520:
			case 521:
			case 645:
				return set[15];
			case 35:
			case 37:
			case 141:
			case 152:
			case 155:
			case 171:
			case 187:
			case 204:
			case 288:
			case 313:
			case 392:
			case 405:
			case 441:
			case 496:
			case 514:
			case 522:
			case 600:
			case 603:
			case 627:
			case 630:
			case 635:
			case 647:
			case 662:
			case 665:
			case 684:
			case 687:
			case 690:
			case 696:
			case 699:
			case 717:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 81:
			case 85:
			case 147:
			case 368:
			case 445:
				return set[19];
			case 42:
			case 161:
			case 168:
			case 173:
			case 236:
			case 412:
			case 437:
			case 440:
			case 554:
			case 555:
			case 615:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 149:
			case 150:
				return set[20];
			case 45:
			case 151:
			case 172:
			case 239:
			case 390:
			case 415:
			case 439:
			case 442:
			case 457:
			case 488:
			case 495:
			case 526:
			case 557:
			case 594:
			case 597:
			case 609:
			case 617:
			case 634:
			case 651:
			case 669:
			case 694:
			case 703:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 46:
			case 47:
			case 51:
			case 52:
			case 53:
			case 55:
			case 451:
			case 452:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 163:
			case 170:
			case 371:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 54:
			case 153:
			case 154:
			case 156:
			case 165:
			case 389:
			case 391:
			case 395:
			case 403:
			case 448:
			case 450:
			case 454:
			case 464:
			case 471:
			case 478:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 56:
			case 57:
			case 59:
			case 60:
			case 61:
			case 67:
			case 83:
			case 139:
			case 162:
			case 164:
			case 166:
			case 169:
			case 179:
			case 181:
			case 222:
			case 265:
			case 267:
			case 268:
			case 285:
			case 302:
			case 307:
			case 316:
			case 322:
			case 324:
			case 328:
			case 331:
			case 337:
			case 348:
			case 350:
			case 356:
			case 370:
			case 372:
			case 404:
			case 431:
			case 447:
			case 449:
			case 513:
			case 599:
				return set[23];
			case 58:
			case 62:
			case 142:
				return set[24];
			case 63:
			case 75:
			case 77:
			case 132:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 64:
			case 65:
				return set[25];
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 68:
			case 84:
			case 474:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 69:
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 70:
			case 71:
				return set[26];
			case 73:
			case 76:
			case 133:
			case 134:
			case 137:
				return set[27];
			case 74:
			case 86:
			case 131:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(36, true);
					a.Set(147, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 80:
			case 666:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 82:
			case 203:
			case 205:
			case 206:
			case 264:
			case 315:
			case 713:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 87:
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 91:
			case 280:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 95:
			case 419:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 99:
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 100:
			case 565:
			case 584:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 104:
			case 297:
			case 304:
			case 319:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 107:
			case 209:
			case 214:
			case 216:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 108:
			case 211:
			case 215:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 111:
			case 244:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 112:
			case 135:
			case 234:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 115:
			case 180:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 119:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 122:
			case 192:
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 124:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 126:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 127:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 128:
			case 233:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 129:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 130:
				return set[28];
			case 138:
				return set[29];
			case 140:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 143:
				return set[30];
			case 144:
				return set[31];
			case 145:
			case 146:
			case 443:
			case 444:
				return set[32];
			case 148:
				return set[33];
			case 157:
			case 158:
			case 300:
			case 309:
				return set[34];
			case 159:
			case 421:
				return set[35];
			case 160:
			case 355:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 167:
				return set[36];
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 175:
			case 176:
				return set[37];
			case 178:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 182:
			case 196:
			case 213:
			case 218:
			case 224:
			case 226:
			case 230:
			case 232:
				return set[38];
			case 184:
			case 185:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 186:
			case 188:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 190:
			case 191:
			case 193:
			case 195:
			case 197:
			case 198:
			case 207:
			case 212:
			case 217:
			case 225:
			case 229:
			case 255:
			case 259:
				return set[39];
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 202:
				return set[40];
			case 208:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 210:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 219:
			case 220:
				return set[41];
			case 223:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 228:
				return set[42];
			case 235:
			case 517:
			case 639:
			case 653:
			case 661:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 237:
			case 238:
			case 413:
			case 414:
			case 486:
			case 487:
			case 493:
			case 494:
			case 592:
			case 593:
			case 595:
			case 596:
			case 607:
			case 608:
			case 632:
			case 633:
			case 649:
			case 650:
				return set[43];
			case 240:
			case 241:
				return set[44];
			case 247:
			case 248:
				return set[45];
			case 249:
				return set[46];
			case 252:
				return set[47];
			case 253:
			case 254:
			case 361:
				return set[48];
			case 256:
			case 346:
			case 628:
			case 629:
			case 631:
			case 672:
			case 685:
			case 686:
			case 688:
			case 697:
			case 698:
			case 700:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 262:
			case 263:
			case 382:
			case 383:
			case 678:
			case 679:
				return set[49];
			case 266:
			case 308:
			case 323:
				return set[50];
			case 269:
			case 270:
			case 290:
			case 291:
			case 305:
			case 306:
			case 320:
			case 321:
				return set[51];
			case 271:
			case 362:
			case 365:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 273:
				return set[52];
			case 274:
			case 293:
				return set[53];
			case 275:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 278:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 282:
			case 283:
				return set[54];
			case 284:
			case 289:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 286:
			case 287:
				return set[55];
			case 292:
				return set[56];
			case 294:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 295:
			case 296:
				return set[57];
			case 298:
			case 299:
				return set[58];
			case 310:
			case 311:
				return set[59];
			case 312:
				return set[60];
			case 314:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 317:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 325:
				return set[61];
			case 326:
			case 330:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 327:
				return set[62];
			case 335:
			case 336:
				return set[63];
			case 339:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 341:
			case 342:
				return set[64];
			case 344:
			case 345:
				return set[65];
			case 347:
			case 349:
				return set[66];
			case 351:
			case 357:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 354:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 358:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 359:
			case 360:
			case 416:
			case 417:
				return set[67];
			case 363:
			case 364:
			case 366:
			case 367:
				return set[68];
			case 369:
				return set[69];
			case 375:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 377:
			case 378:
			case 386:
			case 387:
				return set[70];
			case 379:
			case 388:
				return set[71];
			case 380:
				return set[72];
			case 381:
			case 384:
				return set[73];
			case 394:
			case 396:
			case 397:
			case 556:
			case 616:
				return set[74];
			case 398:
			case 399:
				return set[75];
			case 400:
			case 401:
				return set[76];
			case 402:
			case 406:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 407:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 410:
			case 411:
				return set[77];
			case 420:
				return set[78];
			case 422:
			case 435:
				return set[79];
			case 423:
			case 436:
				return set[80];
			case 424:
			case 425:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 430:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 432:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 433:
				return set[81];
			case 434:
				return set[82];
			case 446:
				return set[83];
			case 453:
				return set[84];
			case 455:
			case 456:
			case 524:
			case 525:
			case 667:
			case 668:
				return set[85];
			case 458:
			case 459:
			case 460:
			case 465:
			case 466:
			case 527:
			case 670:
			case 693:
			case 702:
				return set[86];
			case 461:
			case 467:
			case 476:
				return set[87];
			case 462:
			case 463:
			case 468:
			case 469:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 470:
			case 472:
			case 477:
				return set[88];
			case 473:
			case 475:
				return set[89];
			case 482:
			case 501:
			case 502:
			case 558:
			case 646:
			case 658:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 483:
			case 484:
			case 562:
			case 563:
				return set[90];
			case 498:
			case 499:
			case 506:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 504:
			case 505:
				return set[91];
			case 509:
			case 510:
				return set[92];
			case 511:
			case 512:
			case 571:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 516:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 528:
			case 529:
			case 541:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 534:
			case 535:
				return set[93];
			case 536:
			case 537:
				return set[94];
			case 538:
			case 539:
			case 550:
				return set[95];
			case 544:
			case 545:
				return set[96];
			case 546:
			case 547:
			case 682:
				return set[97];
			case 548:
				return set[98];
			case 549:
				return set[99];
			case 551:
			case 561:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 567:
			case 568:
				return set[100];
			case 569:
				return set[101];
			case 570:
			case 606:
				return set[102];
			case 573:
			case 574:
			case 575:
			case 598:
				return set[103];
			case 576:
			case 580:
			case 590:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 582:
				return set[104];
			case 585:
				return set[105];
			case 586:
				return set[106];
			case 601:
			case 602:
			case 604:
			case 677:
			case 680:
				return set[107];
			case 605:
				return set[108];
			case 611:
			case 613:
			case 622:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 612:
				return set[109];
			case 614:
				return set[110];
			case 620:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 624:
			case 625:
				return set[111];
			case 626:
			case 636:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 637:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 638:
				return set[112];
			case 641:
			case 642:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 643:
			case 652:
			case 714:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 644:
				return set[113];
			case 656:
			case 657:
				return set[114];
			case 664:
				return set[115];
			case 671:
			case 673:
				return set[116];
			case 674:
			case 681:
				return set[117];
			case 675:
			case 676:
				return set[118];
			case 683:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 689:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 692:
			case 701:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 706:
				return set[119];
			case 707:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 708:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 709:
			case 710:
				return set[120];
			case 715:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 716:
				return set[121];
			case 718:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 719:
				return set[122];
			case 720:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 721:
				return set[123];
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
					goto case 718;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 708;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 407;
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
					currentState = 704;
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
					goto case 407;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[124].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 528;
					} else {
						if (la.kind == 103) {
							currentState = 517;
							break;
						} else {
							if (la.kind == 115) {
								goto case 498;
							} else {
								if (la.kind == 142) {
									goto case 9;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 10;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				Expect(142, la); // "Interface"
				currentState = 11;
				break;
			}
			case 11: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(12);
				goto case 189;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 701;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				stateStack.Push(15);
				goto case 23;
			}
			case 15: {
				isMissingModifier = true;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 140) {
					currentState = 696;
					break;
				} else {
					goto case 17;
				}
			}
			case 17: {
				isMissingModifier = true;
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[10].Get(la.kind)) {
					goto case 26;
				} else {
					isMissingModifier = false;
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				Expect(113, la); // "End"
				currentState = 20;
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(142, la); // "Interface"
				currentState = 21;
				break;
			}
			case 21: {
				stateStack.Push(22);
				goto case 23;
			}
			case 22: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 23: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 24;
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 26: {
				isMissingModifier = true;
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				if (la.kind == 40) {
					stateStack.Push(26);
					goto case 407;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[125].Get(la.kind)) {
					currentState = 695;
					break;
				} else {
					isMissingModifier = false;
					goto case 29;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(17);
					goto case 528;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 516;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 498;
						} else {
							if (la.kind == 142) {
								stateStack.Push(17);
								goto case 9;
							} else {
								if (set[13].Get(la.kind)) {
									stateStack.Push(17);
									goto case 30;
								} else {
									Error(la);
									goto case 17;
								}
							}
						}
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 119) {
					currentState = 489;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 479;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 31;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 31: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				PopContext();
				goto case 34;
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					currentState = 455;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 35;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 35: {
				PushContext(Context.Type, la, t);
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				PopContext();
				goto case 23;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 130) {
					currentState = 38;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 38;
						break;
					} else {
						if (set[126].Get(la.kind)) {
							currentState = 38;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 38;
								break;
							} else {
								Error(la);
								goto case 38;
							}
						}
					}
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 37) {
					stateStack.Push(38);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 26) {
					currentState = 40;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				stateStack.Push(41);
				goto case 85;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 37) {
					stateStack.Push(41);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(37, la); // "("
				currentState = 43;
				break;
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 169) {
					currentState = 453;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						goto case 46;
					} else {
						Error(la);
						goto case 45;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 47;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(45);
					goto case 48;
				} else {
					goto case 45;
				}
			}
			case 48: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 49;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (set[23].Get(la.kind)) {
					activeArgument = 0;
					goto case 449;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 50;
					} else {
						goto case 6;
					}
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(22, la); // ","
				currentState = 51;
				break;
			}
			case 51: {
				activeArgument++;
				goto case 52;
			}
			case 52: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 53;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(54);
					goto case 56;
				} else {
					goto case 54;
				}
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (la.kind == 22) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				activeArgument++;
				goto case 52;
			}
			case 56: {
				PushContext(Context.Expression, la, t);
				goto case 57;
			}
			case 57: {
				stateStack.Push(58);
				goto case 59;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (set[127].Get(la.kind)) {
					currentState = 57;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 59: {
				PushContext(Context.Expression, la, t);
				goto case 60;
			}
			case 60: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 61;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (set[128].Get(la.kind)) {
					currentState = 60;
					break;
				} else {
					if (set[34].Get(la.kind)) {
						stateStack.Push(143);
						goto case 157;
					} else {
						if (la.kind == 220) {
							currentState = 139;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(62);
								goto case 69;
							} else {
								if (la.kind == 35) {
									stateStack.Push(62);
									goto case 63;
								} else {
									Error(la);
									goto case 62;
								}
							}
						}
					}
				}
			}
			case 62: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				Expect(35, la); // "{"
				currentState = 64;
				break;
			}
			case 64: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 65;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (set[23].Get(la.kind)) {
					goto case 67;
				} else {
					goto case 66;
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				stateStack.Push(68);
				goto case 56;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 22) {
					currentState = 67;
					break;
				} else {
					goto case 66;
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(162, la); // "New"
				currentState = 70;
				break;
			}
			case 70: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 71;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(130);
					goto case 37;
				} else {
					if (la.kind == 233) {
						PushContext(Context.ObjectInitializer, la, t);
						goto case 74;
					} else {
						goto case 72;
					}
				}
			}
			case 72: {
				Error(la);
				goto case 73;
			}
			case 73: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(233, la); // "With"
				currentState = 75;
				break;
			}
			case 75: {
				stateStack.Push(76);
				goto case 77;
			}
			case 76: {
				PopContext();
				goto case 73;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				Expect(35, la); // "{"
				currentState = 78;
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				if (la.kind == 26 || la.kind == 147) {
					goto case 79;
				} else {
					goto case 66;
				}
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				if (la.kind == 147) {
					currentState = 80;
					break;
				} else {
					goto case 80;
				}
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				Expect(26, la); // "."
				currentState = 81;
				break;
			}
			case 81: {
				stateStack.Push(82);
				goto case 85;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(20, la); // "="
				currentState = 83;
				break;
			}
			case 83: {
				stateStack.Push(84);
				goto case 56;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 22) {
					currentState = 79;
					break;
				} else {
					goto case 66;
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				if (la.kind == 2) {
					goto case 129;
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
								goto case 128;
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
												goto case 127;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 126;
													} else {
														if (la.kind == 65) {
															goto case 125;
														} else {
															if (la.kind == 66) {
																goto case 124;
															} else {
																if (la.kind == 67) {
																	goto case 123;
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
																				goto case 122;
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
																																		goto case 121;
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
																																					goto case 120;
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
																																																goto case 119;
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
																																																						goto case 118;
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
																																																									goto case 117;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 116;
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
																																																																		goto case 115;
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
																																																																							goto case 114;
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
																																																																										goto case 113;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 112;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 111;
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
																																																																																			goto case 110;
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
																																																																																									goto case 109;
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
																																																																																													goto case 108;
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
																																																																																																goto case 107;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 106;
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
																																																																																																																goto case 105;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 104;
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
																																																																																																																								goto case 103;
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
																																																																																																																														goto case 102;
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
																																																																																																																																						goto case 101;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 100;
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
																																																																																																																																																			goto case 99;
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
																																																																																																																																																									goto case 98;
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
																																																																																																																																																												goto case 97;
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
																																																																																																																																																															goto case 96;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 95;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 94;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 93;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 92;
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
																																																																																																																																																																								goto case 91;
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
																																																																																																																																																																													goto case 90;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 89;
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
																																																																																																																																																																																				goto case 88;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 87;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 86;
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
				currentState = stateStack.Pop();
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 35 || la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						PushContext(Context.CollectionInitializer, la, t);
						goto case 135;
					} else {
						if (la.kind == 35) {
							PushContext(Context.CollectionInitializer, la, t);
							stateStack.Push(134);
							goto case 63;
						} else {
							if (la.kind == 233) {
								PushContext(Context.ObjectInitializer, la, t);
								goto case 131;
							} else {
								goto case 72;
							}
						}
					}
				} else {
					goto case 73;
				}
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				Expect(233, la); // "With"
				currentState = 132;
				break;
			}
			case 132: {
				stateStack.Push(133);
				goto case 77;
			}
			case 133: {
				PopContext();
				goto case 73;
			}
			case 134: {
				PopContext();
				goto case 73;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				Expect(126, la); // "From"
				currentState = 136;
				break;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 35) {
					stateStack.Push(137);
					goto case 63;
				} else {
					if (set[29].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						Error(la);
						goto case 137;
					}
				}
			}
			case 137: {
				PopContext();
				goto case 73;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				currentState = 137;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 59;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(144, la); // "Is"
				currentState = 141;
				break;
			}
			case 141: {
				PushContext(Context.Type, la, t);
				stateStack.Push(142);
				goto case 37;
			}
			case 142: {
				PopContext();
				goto case 62;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(143);
					goto case 144;
				} else {
					goto case 62;
				}
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 37) {
					currentState = 149;
					break;
				} else {
					if (set[129].Get(la.kind)) {
						currentState = 145;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 145: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 146;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 10) {
					currentState = 147;
					break;
				} else {
					goto case 147;
				}
			}
			case 147: {
				stateStack.Push(148);
				goto case 85;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 149: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 150;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 169) {
					currentState = 152;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						if (set[22].Get(la.kind)) {
							stateStack.Push(151);
							goto case 48;
						} else {
							goto case 151;
						}
					} else {
						Error(la);
						goto case 151;
					}
				}
			}
			case 151: {
				PopContext();
				goto case 45;
			}
			case 152: {
				PushContext(Context.Type, la, t);
				stateStack.Push(153);
				goto case 37;
			}
			case 153: {
				PopContext();
				goto case 154;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 22) {
					currentState = 155;
					break;
				} else {
					goto case 151;
				}
			}
			case 155: {
				PushContext(Context.Type, la, t);
				stateStack.Push(156);
				goto case 37;
			}
			case 156: {
				PopContext();
				goto case 154;
			}
			case 157: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 158;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (set[130].Get(la.kind)) {
					currentState = 159;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 447;
						break;
					} else {
						if (set[131].Get(la.kind)) {
							currentState = 159;
							break;
						} else {
							if (set[126].Get(la.kind)) {
								currentState = 159;
								break;
							} else {
								if (set[129].Get(la.kind)) {
									currentState = 443;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 440;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 437;
											break;
										} else {
											if (set[78].Get(la.kind)) {
												stateStack.Push(159);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 420;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(159);
													goto case 235;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(159);
														PushContext(Context.Query, la, t);
														goto case 174;
													} else {
														if (set[36].Get(la.kind)) {
															stateStack.Push(159);
															goto case 167;
														} else {
															if (la.kind == 135) {
																stateStack.Push(159);
																goto case 160;
															} else {
																Error(la);
																goto case 159;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 159: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				Expect(135, la); // "If"
				currentState = 161;
				break;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				Expect(37, la); // "("
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 56;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(22, la); // ","
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 56;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 22) {
					currentState = 166;
					break;
				} else {
					goto case 45;
				}
			}
			case 166: {
				stateStack.Push(45);
				goto case 56;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[132].Get(la.kind)) {
					currentState = 173;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 168;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(37, la); // "("
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 56;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				Expect(22, la); // ","
				currentState = 171;
				break;
			}
			case 171: {
				PushContext(Context.Type, la, t);
				stateStack.Push(172);
				goto case 37;
			}
			case 172: {
				PopContext();
				goto case 45;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(37, la); // "("
				currentState = 166;
				break;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 126) {
					stateStack.Push(175);
					goto case 234;
				} else {
					if (la.kind == 58) {
						stateStack.Push(175);
						goto case 233;
					} else {
						Error(la);
						goto case 175;
					}
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(175);
					goto case 176;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 126) {
					currentState = 231;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 227;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 225;
							break;
						} else {
							if (la.kind == 107) {
								goto case 117;
							} else {
								if (la.kind == 230) {
									currentState = 56;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 221;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 219;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 217;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 190;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 177;
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
			case 177: {
				stateStack.Push(178);
				goto case 183;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				Expect(171, la); // "On"
				currentState = 179;
				break;
			}
			case 179: {
				stateStack.Push(180);
				goto case 56;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(116, la); // "Equals"
				currentState = 181;
				break;
			}
			case 181: {
				stateStack.Push(182);
				goto case 56;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (la.kind == 22) {
					currentState = 179;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 183: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(184);
				goto case 189;
			}
			case 184: {
				PopContext();
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 63) {
					currentState = 187;
					break;
				} else {
					goto case 186;
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 187: {
				PushContext(Context.Type, la, t);
				stateStack.Push(188);
				goto case 37;
			}
			case 188: {
				PopContext();
				goto case 186;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (set[117].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 119;
					} else {
						goto case 6;
					}
				}
			}
			case 190: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 191;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 146) {
					goto case 209;
				} else {
					if (set[39].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 193;
							break;
						} else {
							if (set[39].Get(la.kind)) {
								goto case 207;
							} else {
								Error(la);
								goto case 192;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				Expect(70, la); // "By"
				currentState = 193;
				break;
			}
			case 193: {
				stateStack.Push(194);
				goto case 197;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 22) {
					currentState = 193;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 195;
					break;
				}
			}
			case 195: {
				stateStack.Push(196);
				goto case 197;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 22) {
					currentState = 195;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 197: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 198;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(199);
					goto case 189;
				} else {
					goto case 56;
				}
			}
			case 199: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 200;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 204;
						break;
					} else {
						if (la.kind == 20) {
							goto case 203;
						} else {
							if (set[40].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								goto case 201;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 201: {
				Error(la);
				goto case 56;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				currentState = 56;
				break;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				currentState = 56;
				break;
			}
			case 204: {
				PushContext(Context.Type, la, t);
				stateStack.Push(205);
				goto case 37;
			}
			case 205: {
				PopContext();
				goto case 206;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 207: {
				stateStack.Push(208);
				goto case 197;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 22) {
					currentState = 207;
					break;
				} else {
					goto case 192;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 216;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 214;
						break;
					} else {
						if (la.kind == 146) {
							goto case 209;
						} else {
							Error(la);
							goto case 210;
						}
					}
				} else {
					goto case 211;
				}
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(143, la); // "Into"
				currentState = 212;
				break;
			}
			case 212: {
				stateStack.Push(213);
				goto case 197;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 22) {
					currentState = 212;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 214: {
				stateStack.Push(215);
				goto case 216;
			}
			case 215: {
				stateStack.Push(210);
				goto case 211;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(146, la); // "Join"
				currentState = 177;
				break;
			}
			case 217: {
				stateStack.Push(218);
				goto case 197;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (la.kind == 22) {
					currentState = 217;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 219: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 220;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(70, la); // "By"
				currentState = 222;
				break;
			}
			case 222: {
				stateStack.Push(223);
				goto case 56;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 64) {
					currentState = 224;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 224;
						break;
					} else {
						Error(la);
						goto case 224;
					}
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 22) {
					currentState = 222;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 225: {
				stateStack.Push(226);
				goto case 197;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 22) {
					currentState = 225;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 227: {
				stateStack.Push(228);
				goto case 183;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(228);
					goto case 176;
				} else {
					Expect(143, la); // "Into"
					currentState = 229;
					break;
				}
			}
			case 229: {
				stateStack.Push(230);
				goto case 197;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 22) {
					currentState = 229;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 231: {
				stateStack.Push(232);
				goto case 183;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.kind == 22) {
					currentState = 231;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				Expect(58, la); // "Aggregate"
				currentState = 227;
				break;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(126, la); // "From"
				currentState = 231;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 210) {
					currentState = 412;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 236;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(37, la); // "("
				currentState = 237;
				break;
			}
			case 237: {
				SetIdentifierExpected(la);
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(239);
					goto case 394;
				} else {
					goto case 239;
				}
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				Expect(38, la); // ")"
				currentState = 240;
				break;
			}
			case 240: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 392;
							break;
						} else {
							goto case 242;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 242: {
				stateStack.Push(243);
				goto case 245;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				Expect(113, la); // "End"
				currentState = 244;
				break;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 245: {
				PushContext(Context.Body, la, t);
				goto case 246;
			}
			case 246: {
				stateStack.Push(247);
				goto case 23;
			}
			case 247: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 248;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[134].Get(la.kind)) {
					if (set[67].Get(la.kind)) {
						if (set[48].Get(la.kind)) {
							stateStack.Push(246);
							goto case 253;
						} else {
							goto case 246;
						}
					} else {
						if (la.kind == 113) {
							currentState = 251;
							break;
						} else {
							goto case 250;
						}
					}
				} else {
					goto case 249;
				}
			}
			case 249: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 250: {
				Error(la);
				goto case 247;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 246;
				} else {
					if (set[47].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 250;
					}
				}
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				currentState = 247;
				break;
			}
			case 253: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 376;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 372;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 370;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 368;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 350;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 335;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 331;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 325;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 298;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 294;
																break;
															} else {
																goto case 294;
															}
														} else {
															if (la.kind == 194) {
																currentState = 292;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 290;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 277;
																break;
															} else {
																if (set[135].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 274;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 273;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 272;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 97;
																				} else {
																					if (la.kind == 195) {
																						currentState = 269;
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
																		currentState = 267;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 265;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 255;
																				break;
																			} else {
																				if (set[136].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 56;
																						break;
																					} else {
																						goto case 56;
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
			case 255: {
				stateStack.Push(256);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 259;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (la.kind == 22) {
					currentState = 255;
					break;
				} else {
					stateStack.Push(257);
					goto case 245;
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
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(260);
					goto case 189;
				} else {
					goto case 56;
				}
			}
			case 260: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 262;
						break;
					} else {
						if (la.kind == 20) {
							goto case 203;
						} else {
							if (set[40].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								goto case 201;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 262: {
				PushContext(Context.Type, la, t);
				goto case 263;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 162) {
					stateStack.Push(264);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(264);
						goto case 37;
					} else {
						Error(la);
						goto case 264;
					}
				}
			}
			case 264: {
				PopContext();
				goto case 206;
			}
			case 265: {
				stateStack.Push(266);
				goto case 56;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 22) {
					currentState = 265;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 269: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(271);
					goto case 56;
				} else {
					goto case 271;
				}
			}
			case 271: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 108) {
					goto case 116;
				} else {
					if (la.kind == 124) {
						goto case 113;
					} else {
						if (la.kind == 231) {
							goto case 87;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 108) {
					goto case 116;
				} else {
					if (la.kind == 124) {
						goto case 113;
					} else {
						if (la.kind == 231) {
							goto case 87;
						} else {
							if (la.kind == 197) {
								goto case 99;
							} else {
								if (la.kind == 210) {
									goto case 95;
								} else {
									if (la.kind == 127) {
										goto case 111;
									} else {
										if (la.kind == 186) {
											goto case 100;
										} else {
											if (la.kind == 218) {
												goto case 91;
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
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[6].Get(la.kind)) {
					goto case 276;
				} else {
					if (la.kind == 5) {
						goto case 275;
					} else {
						goto case 6;
					}
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 277: {
				stateStack.Push(278);
				goto case 245;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 75) {
					currentState = 282;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 281;
						break;
					} else {
						goto case 279;
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(113, la); // "End"
				currentState = 280;
				break;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				stateStack.Push(279);
				goto case 245;
			}
			case 282: {
				SetIdentifierExpected(la);
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(286);
					goto case 189;
				} else {
					goto case 284;
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 229) {
					currentState = 285;
					break;
				} else {
					goto case 277;
				}
			}
			case 285: {
				stateStack.Push(277);
				goto case 56;
			}
			case 286: {
				PopContext();
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 63) {
					currentState = 288;
					break;
				} else {
					goto case 284;
				}
			}
			case 288: {
				PushContext(Context.Type, la, t);
				stateStack.Push(289);
				goto case 37;
			}
			case 289: {
				PopContext();
				goto case 284;
			}
			case 290: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 163) {
					goto case 104;
				} else {
					goto case 293;
				}
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 5) {
					goto case 275;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 276;
					} else {
						goto case 6;
					}
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				Expect(118, la); // "Error"
				currentState = 295;
				break;
			}
			case 295: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 296;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 293;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 297;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 299;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(315);
					goto case 309;
				} else {
					if (la.kind == 110) {
						currentState = 300;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 309;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(138, la); // "In"
				currentState = 302;
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 56;
			}
			case 303: {
				stateStack.Push(304);
				goto case 245;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(163, la); // "Next"
				currentState = 305;
				break;
			}
			case 305: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 306;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (set[23].Get(la.kind)) {
					goto case 307;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 56;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 22) {
					currentState = 307;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 309: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(310);
				goto case 157;
			}
			case 310: {
				PopContext();
				goto case 311;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 33) {
					currentState = 312;
					break;
				} else {
					goto case 312;
				}
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(312);
					goto case 144;
				} else {
					if (la.kind == 63) {
						currentState = 313;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 313: {
				PushContext(Context.Type, la, t);
				stateStack.Push(314);
				goto case 37;
			}
			case 314: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(20, la); // "="
				currentState = 316;
				break;
			}
			case 316: {
				stateStack.Push(317);
				goto case 56;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 205) {
					currentState = 324;
					break;
				} else {
					goto case 318;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 245;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(163, la); // "Next"
				currentState = 320;
				break;
			}
			case 320: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[23].Get(la.kind)) {
					goto case 322;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 322: {
				stateStack.Push(323);
				goto case 56;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 22) {
					currentState = 322;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 324: {
				stateStack.Push(318);
				goto case 56;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 328;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(326);
						goto case 245;
					} else {
						goto case 6;
					}
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(152, la); // "Loop"
				currentState = 327;
				break;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				stateStack.Push(329);
				goto case 56;
			}
			case 329: {
				stateStack.Push(330);
				goto case 245;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 331: {
				stateStack.Push(332);
				goto case 56;
			}
			case 332: {
				stateStack.Push(333);
				goto case 245;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(113, la); // "End"
				currentState = 334;
				break;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 74) {
					currentState = 337;
					break;
				} else {
					goto case 337;
				}
			}
			case 337: {
				stateStack.Push(338);
				goto case 56;
			}
			case 338: {
				stateStack.Push(339);
				goto case 23;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 74) {
					currentState = 341;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 340;
					break;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 111) {
					currentState = 343;
					break;
				} else {
					if (set[65].Get(la.kind)) {
						goto case 344;
					} else {
						Error(la);
						goto case 343;
					}
				}
			}
			case 343: {
				stateStack.Push(339);
				goto case 245;
			}
			case 344: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 345;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (set[137].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 347;
						break;
					} else {
						goto case 347;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(346);
						goto case 56;
					} else {
						Error(la);
						goto case 346;
					}
				}
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 22) {
					currentState = 344;
					break;
				} else {
					goto case 343;
				}
			}
			case 347: {
				stateStack.Push(348);
				goto case 349;
			}
			case 348: {
				stateStack.Push(346);
				goto case 59;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
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
			case 350: {
				stateStack.Push(351);
				goto case 56;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 214) {
					currentState = 359;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 353;
				} else {
					goto case 6;
				}
			}
			case 353: {
				stateStack.Push(354);
				goto case 245;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 358;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 356;
							break;
						} else {
							Error(la);
							goto case 353;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 355;
					break;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 356: {
				stateStack.Push(357);
				goto case 56;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 214) {
					currentState = 353;
					break;
				} else {
					goto case 353;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 135) {
					currentState = 356;
					break;
				} else {
					goto case 353;
				}
			}
			case 359: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[48].Get(la.kind)) {
					goto case 361;
				} else {
					goto case 352;
				}
			}
			case 361: {
				stateStack.Push(362);
				goto case 253;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 21) {
					currentState = 366;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 363;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 363: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 364;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (set[48].Get(la.kind)) {
					stateStack.Push(365);
					goto case 253;
				} else {
					goto case 365;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 21) {
					currentState = 363;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 366: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (set[48].Get(la.kind)) {
					goto case 361;
				} else {
					goto case 362;
				}
			}
			case 368: {
				stateStack.Push(369);
				goto case 85;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 370: {
				stateStack.Push(371);
				goto case 56;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 372: {
				stateStack.Push(373);
				goto case 56;
			}
			case 373: {
				stateStack.Push(374);
				goto case 245;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				Expect(113, la); // "End"
				currentState = 375;
				break;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 233) {
					goto case 86;
				} else {
					if (la.kind == 211) {
						goto case 94;
					} else {
						goto case 6;
					}
				}
			}
			case 376: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(377);
				goto case 189;
			}
			case 377: {
				PopContext();
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 33) {
					currentState = 379;
					break;
				} else {
					goto case 379;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 37) {
					currentState = 391;
					break;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 22) {
					currentState = 385;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 382;
						break;
					} else {
						goto case 381;
					}
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (la.kind == 20) {
					goto case 203;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 382: {
				PushContext(Context.Type, la, t);
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 162) {
					stateStack.Push(384);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(384);
						goto case 37;
					} else {
						Error(la);
						goto case 384;
					}
				}
			}
			case 384: {
				PopContext();
				goto case 381;
			}
			case 385: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(386);
				goto case 189;
			}
			case 386: {
				PopContext();
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 33) {
					currentState = 388;
					break;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 37) {
					currentState = 389;
					break;
				} else {
					goto case 380;
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 22) {
					currentState = 389;
					break;
				} else {
					goto case 390;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(38, la); // ")"
				currentState = 380;
				break;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 22) {
					currentState = 391;
					break;
				} else {
					goto case 390;
				}
			}
			case 392: {
				PushContext(Context.Type, la, t);
				stateStack.Push(393);
				goto case 37;
			}
			case 393: {
				PopContext();
				goto case 242;
			}
			case 394: {
				stateStack.Push(395);
				PushContext(Context.Parameter, la, t);
				goto case 396;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 22) {
					currentState = 394;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 396: {
				SetIdentifierExpected(la);
				goto case 397;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (la.kind == 40) {
					stateStack.Push(396);
					goto case 407;
				} else {
					goto case 398;
				}
			}
			case 398: {
				SetIdentifierExpected(la);
				goto case 399;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (set[138].Get(la.kind)) {
					currentState = 398;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(400);
					goto case 189;
				}
			}
			case 400: {
				PopContext();
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 63) {
					currentState = 405;
					break;
				} else {
					goto case 402;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 20) {
					currentState = 404;
					break;
				} else {
					goto case 403;
				}
			}
			case 403: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 404: {
				stateStack.Push(403);
				goto case 56;
			}
			case 405: {
				PushContext(Context.Type, la, t);
				stateStack.Push(406);
				goto case 37;
			}
			case 406: {
				PopContext();
				goto case 402;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(40, la); // "<"
				currentState = 408;
				break;
			}
			case 408: {
				PushContext(Context.Attribute, la, t);
				goto case 409;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (set[139].Get(la.kind)) {
					currentState = 409;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 410;
					break;
				}
			}
			case 410: {
				PopContext();
				goto case 411;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				Expect(37, la); // "("
				currentState = 413;
				break;
			}
			case 413: {
				SetIdentifierExpected(la);
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(415);
					goto case 394;
				} else {
					goto case 415;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(38, la); // ")"
				currentState = 416;
				break;
			}
			case 416: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 417;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (set[48].Get(la.kind)) {
					goto case 253;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(418);
						goto case 245;
					} else {
						goto case 6;
					}
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				Expect(113, la); // "End"
				currentState = 419;
				break;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 433;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(422);
						goto case 424;
					} else {
						Error(la);
						goto case 421;
					}
				}
			}
			case 421: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (la.kind == 17) {
					currentState = 423;
					break;
				} else {
					goto case 421;
				}
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 16) {
					currentState = 422;
					break;
				} else {
					goto case 422;
				}
			}
			case 424: {
				PushContext(Context.Xml, la, t);
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 426;
				break;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 426;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(426);
							goto case 430;
						} else {
							Error(la);
							goto case 426;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 427;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 428;
							break;
						} else {
							Error(la);
							goto case 427;
						}
					}
				}
			}
			case 427: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (set[142].Get(la.kind)) {
					if (set[143].Get(la.kind)) {
						currentState = 428;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(428);
							goto case 430;
						} else {
							if (la.kind == 10) {
								stateStack.Push(428);
								goto case 424;
							} else {
								Error(la);
								goto case 428;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 429;
					break;
				}
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[144].Get(la.kind)) {
					if (set[145].Get(la.kind)) {
						currentState = 429;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(429);
							goto case 430;
						} else {
							Error(la);
							goto case 429;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 427;
					break;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 431;
				break;
			}
			case 431: {
				stateStack.Push(432);
				goto case 56;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 16) {
					currentState = 434;
					break;
				} else {
					goto case 434;
				}
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 433;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(435);
						goto case 424;
					} else {
						goto case 421;
					}
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 17) {
					currentState = 436;
					break;
				} else {
					goto case 421;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (la.kind == 16) {
					currentState = 435;
					break;
				} else {
					goto case 435;
				}
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				Expect(37, la); // "("
				currentState = 438;
				break;
			}
			case 438: {
				readXmlIdentifier = true;
				stateStack.Push(439);
				goto case 189;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(38, la); // ")"
				currentState = 159;
				break;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				Expect(37, la); // "("
				currentState = 441;
				break;
			}
			case 441: {
				PushContext(Context.Type, la, t);
				stateStack.Push(442);
				goto case 37;
			}
			case 442: {
				PopContext();
				goto case 439;
			}
			case 443: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 444;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 10) {
					currentState = 445;
					break;
				} else {
					goto case 445;
				}
			}
			case 445: {
				stateStack.Push(446);
				goto case 85;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 11) {
					currentState = 159;
					break;
				} else {
					goto case 159;
				}
			}
			case 447: {
				stateStack.Push(448);
				goto case 56;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 22) {
					currentState = 447;
					break;
				} else {
					goto case 439;
				}
			}
			case 449: {
				stateStack.Push(450);
				goto case 56;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 22) {
					currentState = 451;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 451: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 452;
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (set[23].Get(la.kind)) {
					goto case 449;
				} else {
					goto case 450;
				}
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(454);
					goto case 37;
				} else {
					goto case 454;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (la.kind == 22) {
					currentState = 453;
					break;
				} else {
					goto case 45;
				}
			}
			case 455: {
				SetIdentifierExpected(la);
				goto case 456;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (set[146].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 458;
						break;
					} else {
						if (set[74].Get(la.kind)) {
							stateStack.Push(457);
							goto case 394;
						} else {
							Error(la);
							goto case 457;
						}
					}
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 458: {
				stateStack.Push(457);
				goto case 459;
			}
			case 459: {
				SetIdentifierExpected(la);
				goto case 460;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 461;
					break;
				} else {
					goto case 461;
				}
			}
			case 461: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(462);
				goto case 476;
			}
			case 462: {
				PopContext();
				goto case 463;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 63) {
					currentState = 477;
					break;
				} else {
					goto case 464;
				}
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 22) {
					currentState = 465;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 465: {
				SetIdentifierExpected(la);
				goto case 466;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 467;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(468);
				goto case 476;
			}
			case 468: {
				PopContext();
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 63) {
					currentState = 470;
					break;
				} else {
					goto case 464;
				}
			}
			case 470: {
				PushContext(Context.Type, la, t);
				stateStack.Push(471);
				goto case 472;
			}
			case 471: {
				PopContext();
				goto case 464;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (set[89].Get(la.kind)) {
					goto case 475;
				} else {
					if (la.kind == 35) {
						currentState = 473;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 473: {
				stateStack.Push(474);
				goto case 475;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 22) {
					currentState = 473;
					break;
				} else {
					goto case 66;
				}
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 105;
					} else {
						if (la.kind == 84) {
							goto case 121;
						} else {
							if (la.kind == 209) {
								goto case 96;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 2) {
					goto case 129;
				} else {
					if (la.kind == 62) {
						goto case 127;
					} else {
						if (la.kind == 64) {
							goto case 126;
						} else {
							if (la.kind == 65) {
								goto case 125;
							} else {
								if (la.kind == 66) {
									goto case 124;
								} else {
									if (la.kind == 67) {
										goto case 123;
									} else {
										if (la.kind == 70) {
											goto case 122;
										} else {
											if (la.kind == 87) {
												goto case 120;
											} else {
												if (la.kind == 104) {
													goto case 118;
												} else {
													if (la.kind == 107) {
														goto case 117;
													} else {
														if (la.kind == 116) {
															goto case 115;
														} else {
															if (la.kind == 121) {
																goto case 114;
															} else {
																if (la.kind == 133) {
																	goto case 110;
																} else {
																	if (la.kind == 139) {
																		goto case 109;
																	} else {
																		if (la.kind == 143) {
																			goto case 108;
																		} else {
																			if (la.kind == 146) {
																				goto case 107;
																			} else {
																				if (la.kind == 147) {
																					goto case 106;
																				} else {
																					if (la.kind == 170) {
																						goto case 103;
																					} else {
																						if (la.kind == 176) {
																							goto case 102;
																						} else {
																							if (la.kind == 184) {
																								goto case 101;
																							} else {
																								if (la.kind == 203) {
																									goto case 98;
																								} else {
																									if (la.kind == 212) {
																										goto case 93;
																									} else {
																										if (la.kind == 213) {
																											goto case 92;
																										} else {
																											if (la.kind == 223) {
																												goto case 90;
																											} else {
																												if (la.kind == 224) {
																													goto case 89;
																												} else {
																													if (la.kind == 230) {
																														goto case 88;
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
			case 477: {
				PushContext(Context.Type, la, t);
				stateStack.Push(478);
				goto case 472;
			}
			case 478: {
				PopContext();
				goto case 464;
			}
			case 479: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(480);
				goto case 189;
			}
			case 480: {
				PopContext();
				goto case 481;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 37) {
					currentState = 486;
					break;
				} else {
					goto case 482;
				}
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (la.kind == 63) {
					currentState = 483;
					break;
				} else {
					goto case 23;
				}
			}
			case 483: {
				PushContext(Context.Type, la, t);
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 40) {
					stateStack.Push(484);
					goto case 407;
				} else {
					stateStack.Push(485);
					goto case 37;
				}
			}
			case 485: {
				PopContext();
				goto case 23;
			}
			case 486: {
				SetIdentifierExpected(la);
				goto case 487;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(488);
					goto case 394;
				} else {
					goto case 488;
				}
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				Expect(38, la); // ")"
				currentState = 482;
				break;
			}
			case 489: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(490);
				goto case 189;
			}
			case 490: {
				PopContext();
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 496;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 493;
							break;
						} else {
							goto case 492;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 492: {
				Error(la);
				goto case 23;
			}
			case 493: {
				SetIdentifierExpected(la);
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(495);
					goto case 394;
				} else {
					goto case 495;
				}
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 496: {
				PushContext(Context.Type, la, t);
				stateStack.Push(497);
				goto case 37;
			}
			case 497: {
				PopContext();
				goto case 23;
			}
			case 498: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 499;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				Expect(115, la); // "Enum"
				currentState = 500;
				break;
			}
			case 500: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(501);
				goto case 189;
			}
			case 501: {
				PopContext();
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 63) {
					currentState = 514;
					break;
				} else {
					goto case 503;
				}
			}
			case 503: {
				stateStack.Push(504);
				goto case 23;
			}
			case 504: {
				SetIdentifierExpected(la);
				goto case 505;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (set[92].Get(la.kind)) {
					goto case 509;
				} else {
					Expect(113, la); // "End"
					currentState = 506;
					break;
				}
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				Expect(115, la); // "Enum"
				currentState = 507;
				break;
			}
			case 507: {
				stateStack.Push(508);
				goto case 23;
			}
			case 508: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 509: {
				SetIdentifierExpected(la);
				goto case 510;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (la.kind == 40) {
					stateStack.Push(509);
					goto case 407;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(511);
					goto case 189;
				}
			}
			case 511: {
				PopContext();
				goto case 512;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				if (la.kind == 20) {
					currentState = 513;
					break;
				} else {
					goto case 503;
				}
			}
			case 513: {
				stateStack.Push(503);
				goto case 56;
			}
			case 514: {
				PushContext(Context.Type, la, t);
				stateStack.Push(515);
				goto case 37;
			}
			case 515: {
				PopContext();
				goto case 503;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(103, la); // "Delegate"
				currentState = 517;
				break;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 210) {
					currentState = 518;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 518;
						break;
					} else {
						Error(la);
						goto case 518;
					}
				}
			}
			case 518: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 519;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				currentState = 520;
				break;
			}
			case 520: {
				PopContext();
				goto case 521;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 37) {
					currentState = 524;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 522;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 522: {
				PushContext(Context.Type, la, t);
				stateStack.Push(523);
				goto case 37;
			}
			case 523: {
				PopContext();
				goto case 23;
			}
			case 524: {
				SetIdentifierExpected(la);
				goto case 525;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				if (set[146].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 527;
						break;
					} else {
						if (set[74].Get(la.kind)) {
							stateStack.Push(526);
							goto case 394;
						} else {
							Error(la);
							goto case 526;
						}
					}
				} else {
					goto case 526;
				}
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				Expect(38, la); // ")"
				currentState = 521;
				break;
			}
			case 527: {
				stateStack.Push(526);
				goto case 459;
			}
			case 528: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 529;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 155) {
					currentState = 530;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 530;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 530;
							break;
						} else {
							Error(la);
							goto case 530;
						}
					}
				}
			}
			case 530: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(531);
				goto case 189;
			}
			case 531: {
				PopContext();
				goto case 532;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 37) {
					currentState = 692;
					break;
				} else {
					goto case 533;
				}
			}
			case 533: {
				stateStack.Push(534);
				goto case 23;
			}
			case 534: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 535;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 689;
				} else {
					goto case 536;
				}
			}
			case 536: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 537;
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 683;
				} else {
					goto case 538;
				}
			}
			case 538: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 539;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				if (set[96].Get(la.kind)) {
					goto case 544;
				} else {
					isMissingModifier = false;
					goto case 540;
				}
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				Expect(113, la); // "End"
				currentState = 541;
				break;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 155) {
					currentState = 542;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 542;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 542;
							break;
						} else {
							Error(la);
							goto case 542;
						}
					}
				}
			}
			case 542: {
				stateStack.Push(543);
				goto case 23;
			}
			case 543: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 544: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 545;
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				if (la.kind == 40) {
					stateStack.Push(544);
					goto case 407;
				} else {
					isMissingModifier = true;
					goto case 546;
				}
			}
			case 546: {
				SetIdentifierExpected(la);
				goto case 547;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (set[125].Get(la.kind)) {
					currentState = 682;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 548;
				}
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(538);
					goto case 528;
				} else {
					if (la.kind == 103) {
						stateStack.Push(538);
						goto case 516;
					} else {
						if (la.kind == 115) {
							stateStack.Push(538);
							goto case 498;
						} else {
							if (la.kind == 142) {
								stateStack.Push(538);
								goto case 9;
							} else {
								if (set[99].Get(la.kind)) {
									stateStack.Push(538);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 549;
								} else {
									Error(la);
									goto case 538;
								}
							}
						}
					}
				}
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (set[116].Get(la.kind)) {
					stateStack.Push(550);
					goto case 671;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(550);
						goto case 653;
					} else {
						if (la.kind == 101) {
							stateStack.Push(550);
							goto case 637;
						} else {
							if (la.kind == 119) {
								stateStack.Push(550);
								goto case 622;
							} else {
								if (la.kind == 98) {
									stateStack.Push(550);
									goto case 610;
								} else {
									if (la.kind == 186) {
										stateStack.Push(550);
										goto case 565;
									} else {
										if (la.kind == 172) {
											stateStack.Push(550);
											goto case 551;
										} else {
											Error(la);
											goto case 550;
										}
									}
								}
							}
						}
					}
				}
			}
			case 550: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				Expect(172, la); // "Operator"
				currentState = 552;
				break;
			}
			case 552: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 553;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				currentState = 554;
				break;
			}
			case 554: {
				PopContext();
				goto case 555;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				Expect(37, la); // "("
				currentState = 556;
				break;
			}
			case 556: {
				stateStack.Push(557);
				goto case 394;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				Expect(38, la); // ")"
				currentState = 558;
				break;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 63) {
					currentState = 562;
					break;
				} else {
					goto case 559;
				}
			}
			case 559: {
				stateStack.Push(560);
				goto case 245;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(113, la); // "End"
				currentState = 561;
				break;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 562: {
				PushContext(Context.Type, la, t);
				goto case 563;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (la.kind == 40) {
					stateStack.Push(563);
					goto case 407;
				} else {
					stateStack.Push(564);
					goto case 37;
				}
			}
			case 564: {
				PopContext();
				goto case 559;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(186, la); // "Property"
				currentState = 566;
				break;
			}
			case 566: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(567);
				goto case 189;
			}
			case 567: {
				PopContext();
				goto case 568;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 37) {
					currentState = 607;
					break;
				} else {
					goto case 569;
				}
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				if (la.kind == 63) {
					currentState = 605;
					break;
				} else {
					goto case 570;
				}
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				if (la.kind == 136) {
					currentState = 600;
					break;
				} else {
					goto case 571;
				}
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 20) {
					currentState = 599;
					break;
				} else {
					goto case 572;
				}
			}
			case 572: {
				stateStack.Push(573);
				goto case 23;
			}
			case 573: {
				PopContext();
				goto case 574;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (la.kind == 40) {
					stateStack.Push(574);
					goto case 407;
				} else {
					goto case 575;
				}
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (set[147].Get(la.kind)) {
					currentState = 598;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 576;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 576: {
				if (la == null) { currentState = 576; break; }
				if (la.kind == 128) {
					currentState = 577;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 577;
						break;
					} else {
						Error(la);
						goto case 577;
					}
				}
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (la.kind == 37) {
					currentState = 595;
					break;
				} else {
					goto case 578;
				}
			}
			case 578: {
				stateStack.Push(579);
				goto case 245;
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(113, la); // "End"
				currentState = 580;
				break;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 128) {
					currentState = 581;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 581;
						break;
					} else {
						Error(la);
						goto case 581;
					}
				}
			}
			case 581: {
				stateStack.Push(582);
				goto case 23;
			}
			case 582: {
				if (la == null) { currentState = 582; break; }
				if (set[105].Get(la.kind)) {
					goto case 585;
				} else {
					goto case 583;
				}
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				Expect(113, la); // "End"
				currentState = 584;
				break;
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (la.kind == 40) {
					stateStack.Push(585);
					goto case 407;
				} else {
					goto case 586;
				}
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (set[147].Get(la.kind)) {
					currentState = 586;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 587;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 587;
							break;
						} else {
							Error(la);
							goto case 587;
						}
					}
				}
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				if (la.kind == 37) {
					currentState = 592;
					break;
				} else {
					goto case 588;
				}
			}
			case 588: {
				stateStack.Push(589);
				goto case 245;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				Expect(113, la); // "End"
				currentState = 590;
				break;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 128) {
					currentState = 591;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 591;
						break;
					} else {
						Error(la);
						goto case 591;
					}
				}
			}
			case 591: {
				stateStack.Push(583);
				goto case 23;
			}
			case 592: {
				SetIdentifierExpected(la);
				goto case 593;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(594);
					goto case 394;
				} else {
					goto case 594;
				}
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				Expect(38, la); // ")"
				currentState = 588;
				break;
			}
			case 595: {
				SetIdentifierExpected(la);
				goto case 596;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(597);
					goto case 394;
				} else {
					goto case 597;
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(38, la); // ")"
				currentState = 578;
				break;
			}
			case 598: {
				SetIdentifierExpected(la);
				goto case 575;
			}
			case 599: {
				stateStack.Push(572);
				goto case 56;
			}
			case 600: {
				PushContext(Context.Type, la, t);
				stateStack.Push(601);
				goto case 37;
			}
			case 601: {
				PopContext();
				goto case 602;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 22) {
					currentState = 603;
					break;
				} else {
					goto case 571;
				}
			}
			case 603: {
				PushContext(Context.Type, la, t);
				stateStack.Push(604);
				goto case 37;
			}
			case 604: {
				PopContext();
				goto case 602;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (la.kind == 40) {
					stateStack.Push(605);
					goto case 407;
				} else {
					if (la.kind == 162) {
						stateStack.Push(570);
						goto case 69;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(606);
							goto case 37;
						} else {
							Error(la);
							goto case 570;
						}
					}
				}
			}
			case 606: {
				PopContext();
				goto case 570;
			}
			case 607: {
				SetIdentifierExpected(la);
				goto case 608;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(609);
					goto case 394;
				} else {
					goto case 609;
				}
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				Expect(38, la); // ")"
				currentState = 569;
				break;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				Expect(98, la); // "Custom"
				currentState = 611;
				break;
			}
			case 611: {
				stateStack.Push(612);
				goto case 622;
			}
			case 612: {
				if (la == null) { currentState = 612; break; }
				if (set[110].Get(la.kind)) {
					goto case 614;
				} else {
					Expect(113, la); // "End"
					currentState = 613;
					break;
				}
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (la.kind == 40) {
					stateStack.Push(614);
					goto case 407;
				} else {
					if (la.kind == 56) {
						currentState = 615;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 615;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 615;
								break;
							} else {
								Error(la);
								goto case 615;
							}
						}
					}
				}
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				Expect(37, la); // "("
				currentState = 616;
				break;
			}
			case 616: {
				stateStack.Push(617);
				goto case 394;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				Expect(38, la); // ")"
				currentState = 618;
				break;
			}
			case 618: {
				stateStack.Push(619);
				goto case 245;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				Expect(113, la); // "End"
				currentState = 620;
				break;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (la.kind == 56) {
					currentState = 621;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 621;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 621;
							break;
						} else {
							Error(la);
							goto case 621;
						}
					}
				}
			}
			case 621: {
				stateStack.Push(612);
				goto case 23;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(119, la); // "Event"
				currentState = 623;
				break;
			}
			case 623: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(624);
				goto case 189;
			}
			case 624: {
				PopContext();
				goto case 625;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 63) {
					currentState = 635;
					break;
				} else {
					if (set[148].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 632;
							break;
						} else {
							goto case 626;
						}
					} else {
						Error(la);
						goto case 626;
					}
				}
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 136) {
					currentState = 627;
					break;
				} else {
					goto case 23;
				}
			}
			case 627: {
				PushContext(Context.Type, la, t);
				stateStack.Push(628);
				goto case 37;
			}
			case 628: {
				PopContext();
				goto case 629;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 22) {
					currentState = 630;
					break;
				} else {
					goto case 23;
				}
			}
			case 630: {
				PushContext(Context.Type, la, t);
				stateStack.Push(631);
				goto case 37;
			}
			case 631: {
				PopContext();
				goto case 629;
			}
			case 632: {
				SetIdentifierExpected(la);
				goto case 633;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(634);
					goto case 394;
				} else {
					goto case 634;
				}
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				Expect(38, la); // ")"
				currentState = 626;
				break;
			}
			case 635: {
				PushContext(Context.Type, la, t);
				stateStack.Push(636);
				goto case 37;
			}
			case 636: {
				PopContext();
				goto case 626;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(101, la); // "Declare"
				currentState = 638;
				break;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 639;
					break;
				} else {
					goto case 639;
				}
			}
			case 639: {
				if (la == null) { currentState = 639; break; }
				if (la.kind == 210) {
					currentState = 640;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 640;
						break;
					} else {
						Error(la);
						goto case 640;
					}
				}
			}
			case 640: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(641);
				goto case 189;
			}
			case 641: {
				PopContext();
				goto case 642;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				Expect(149, la); // "Lib"
				currentState = 643;
				break;
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				Expect(3, la); // LiteralString
				currentState = 644;
				break;
			}
			case 644: {
				if (la == null) { currentState = 644; break; }
				if (la.kind == 59) {
					currentState = 652;
					break;
				} else {
					goto case 645;
				}
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (la.kind == 37) {
					currentState = 649;
					break;
				} else {
					goto case 646;
				}
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				if (la.kind == 63) {
					currentState = 647;
					break;
				} else {
					goto case 23;
				}
			}
			case 647: {
				PushContext(Context.Type, la, t);
				stateStack.Push(648);
				goto case 37;
			}
			case 648: {
				PopContext();
				goto case 23;
			}
			case 649: {
				SetIdentifierExpected(la);
				goto case 650;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (set[74].Get(la.kind)) {
					stateStack.Push(651);
					goto case 394;
				} else {
					goto case 651;
				}
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				Expect(38, la); // ")"
				currentState = 646;
				break;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				Expect(3, la); // LiteralString
				currentState = 645;
				break;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (la.kind == 210) {
					currentState = 654;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 654;
						break;
					} else {
						Error(la);
						goto case 654;
					}
				}
			}
			case 654: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 655;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				currentState = 656;
				break;
			}
			case 656: {
				PopContext();
				goto case 657;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				if (la.kind == 37) {
					currentState = 667;
					break;
				} else {
					if (la.kind == 134 || la.kind == 136) {
						currentState = 664;
						break;
					} else {
						goto case 658;
					}
				}
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				if (la.kind == 63) {
					currentState = 662;
					break;
				} else {
					goto case 659;
				}
			}
			case 659: {
				stateStack.Push(660);
				goto case 245;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(113, la); // "End"
				currentState = 661;
				break;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 492;
					}
				}
			}
			case 662: {
				PushContext(Context.Type, la, t);
				stateStack.Push(663);
				goto case 37;
			}
			case 663: {
				PopContext();
				goto case 659;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 666;
					break;
				} else {
					goto case 665;
				}
			}
			case 665: {
				stateStack.Push(658);
				goto case 37;
			}
			case 666: {
				if (la == null) { currentState = 666; break; }
				Expect(26, la); // "."
				currentState = 665;
				break;
			}
			case 667: {
				SetIdentifierExpected(la);
				goto case 668;
			}
			case 668: {
				if (la == null) { currentState = 668; break; }
				if (set[146].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 670;
						break;
					} else {
						if (set[74].Get(la.kind)) {
							stateStack.Push(669);
							goto case 394;
						} else {
							Error(la);
							goto case 669;
						}
					}
				} else {
					goto case 669;
				}
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				Expect(38, la); // ")"
				currentState = 657;
				break;
			}
			case 670: {
				stateStack.Push(669);
				goto case 459;
			}
			case 671: {
				stateStack.Push(672);
				SetIdentifierExpected(la);
				goto case 673;
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				if (la.kind == 22) {
					currentState = 671;
					break;
				} else {
					goto case 23;
				}
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				if (la.kind == 88) {
					currentState = 674;
					break;
				} else {
					goto case 674;
				}
			}
			case 674: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(675);
				goto case 681;
			}
			case 675: {
				PopContext();
				goto case 676;
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				if (la.kind == 63) {
					currentState = 678;
					break;
				} else {
					goto case 677;
				}
			}
			case 677: {
				if (la == null) { currentState = 677; break; }
				if (la.kind == 20) {
					goto case 203;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 678: {
				PushContext(Context.Type, la, t);
				goto case 679;
			}
			case 679: {
				if (la == null) { currentState = 679; break; }
				if (la.kind == 162) {
					stateStack.Push(680);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(680);
						goto case 37;
					} else {
						Error(la);
						goto case 680;
					}
				}
			}
			case 680: {
				PopContext();
				goto case 677;
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				if (set[131].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 128;
					} else {
						if (la.kind == 126) {
							goto case 112;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 682: {
				isMissingModifier = false;
				goto case 546;
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				Expect(136, la); // "Implements"
				currentState = 684;
				break;
			}
			case 684: {
				PushContext(Context.Type, la, t);
				stateStack.Push(685);
				goto case 37;
			}
			case 685: {
				PopContext();
				goto case 686;
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				if (la.kind == 22) {
					currentState = 687;
					break;
				} else {
					stateStack.Push(538);
					goto case 23;
				}
			}
			case 687: {
				PushContext(Context.Type, la, t);
				stateStack.Push(688);
				goto case 37;
			}
			case 688: {
				PopContext();
				goto case 686;
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				Expect(140, la); // "Inherits"
				currentState = 690;
				break;
			}
			case 690: {
				PushContext(Context.Type, la, t);
				stateStack.Push(691);
				goto case 37;
			}
			case 691: {
				PopContext();
				stateStack.Push(536);
				goto case 23;
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				Expect(169, la); // "Of"
				currentState = 693;
				break;
			}
			case 693: {
				stateStack.Push(694);
				goto case 459;
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				Expect(38, la); // ")"
				currentState = 533;
				break;
			}
			case 695: {
				isMissingModifier = false;
				goto case 28;
			}
			case 696: {
				PushContext(Context.Type, la, t);
				stateStack.Push(697);
				goto case 37;
			}
			case 697: {
				PopContext();
				goto case 698;
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
				if (la.kind == 22) {
					currentState = 699;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 699: {
				PushContext(Context.Type, la, t);
				stateStack.Push(700);
				goto case 37;
			}
			case 700: {
				PopContext();
				goto case 698;
			}
			case 701: {
				if (la == null) { currentState = 701; break; }
				Expect(169, la); // "Of"
				currentState = 702;
				break;
			}
			case 702: {
				stateStack.Push(703);
				goto case 459;
			}
			case 703: {
				if (la == null) { currentState = 703; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 704: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 705;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				if (set[47].Get(la.kind)) {
					currentState = 705;
					break;
				} else {
					PopContext();
					stateStack.Push(706);
					goto case 23;
				}
			}
			case 706: {
				if (la == null) { currentState = 706; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(706);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 707;
					break;
				}
			}
			case 707: {
				if (la == null) { currentState = 707; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 708: {
				if (la == null) { currentState = 708; break; }
				Expect(137, la); // "Imports"
				currentState = 709;
				break;
			}
			case 709: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 710;
			}
			case 710: {
				if (la == null) { currentState = 710; break; }
				if (set[149].Get(la.kind)) {
					currentState = 716;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 712;
						break;
					} else {
						Error(la);
						goto case 711;
					}
				}
			}
			case 711: {
				PopContext();
				goto case 23;
			}
			case 712: {
				stateStack.Push(713);
				goto case 189;
			}
			case 713: {
				if (la == null) { currentState = 713; break; }
				Expect(20, la); // "="
				currentState = 714;
				break;
			}
			case 714: {
				if (la == null) { currentState = 714; break; }
				Expect(3, la); // LiteralString
				currentState = 715;
				break;
			}
			case 715: {
				if (la == null) { currentState = 715; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 711;
				break;
			}
			case 716: {
				if (la == null) { currentState = 716; break; }
				if (la.kind == 37) {
					stateStack.Push(716);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 717;
						break;
					} else {
						goto case 711;
					}
				}
			}
			case 717: {
				stateStack.Push(711);
				goto case 37;
			}
			case 718: {
				if (la == null) { currentState = 718; break; }
				Expect(173, la); // "Option"
				currentState = 719;
				break;
			}
			case 719: {
				if (la == null) { currentState = 719; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 721;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 720;
						break;
					} else {
						goto case 492;
					}
				}
			}
			case 720: {
				if (la == null) { currentState = 720; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 492;
					}
				}
			}
			case 721: {
				if (la == null) { currentState = 721; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 23;
					break;
				} else {
					goto case 23;
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
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, -1258291203, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, -1258291235, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {-66189316, 1174405176, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192413, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
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
		new BitArray(new int[] {65140738, 973078487, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
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
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 320, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, -1030969308, 17106176, -533656048, 67}),
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
		new BitArray(new int[] {66189314, -1174405161, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
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