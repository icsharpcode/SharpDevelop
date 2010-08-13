using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 250;
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
			case 251:
			case 501:
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
			case 228:
			case 232:
			case 280:
			case 380:
			case 389:
			case 445:
			case 488:
			case 498:
			case 509:
			case 539:
			case 575:
			case 632:
			case 649:
			case 725:
				return set[6];
			case 12:
			case 13:
			case 540:
			case 541:
			case 586:
			case 596:
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
			case 243:
			case 246:
			case 247:
			case 281:
			case 285:
			case 307:
			case 322:
			case 333:
			case 336:
			case 342:
			case 347:
			case 356:
			case 357:
			case 377:
			case 400:
			case 494:
			case 506:
			case 512:
			case 516:
			case 524:
			case 532:
			case 542:
			case 551:
			case 568:
			case 573:
			case 581:
			case 587:
			case 590:
			case 597:
			case 600:
			case 627:
			case 630:
			case 657:
			case 668:
			case 672:
			case 704:
			case 724:
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
			case 244:
			case 258:
			case 283:
			case 337:
			case 378:
			case 425:
			case 549:
			case 569:
			case 588:
			case 592:
			case 598:
			case 628:
			case 669:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 517:
			case 552:
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
			case 708:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 136:
			case 201:
			case 202:
			case 252:
			case 261:
			case 262:
			case 415:
			case 416:
			case 433:
			case 434:
			case 435:
			case 436:
			case 527:
			case 528:
			case 561:
			case 562:
			case 663:
			case 664:
			case 717:
			case 718:
				return set[14];
			case 33:
			case 34:
			case 489:
			case 490:
			case 499:
			case 500:
			case 529:
			case 530:
			case 654:
				return set[15];
			case 35:
			case 37:
			case 141:
			case 152:
			case 155:
			case 171:
			case 187:
			case 205:
			case 292:
			case 317:
			case 399:
			case 412:
			case 448:
			case 505:
			case 523:
			case 531:
			case 609:
			case 612:
			case 636:
			case 639:
			case 644:
			case 656:
			case 671:
			case 674:
			case 697:
			case 700:
			case 703:
			case 709:
			case 712:
			case 730:
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
			case 372:
			case 452:
				return set[19];
			case 42:
			case 161:
			case 168:
			case 173:
			case 237:
			case 419:
			case 444:
			case 447:
			case 563:
			case 564:
			case 624:
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
			case 240:
			case 422:
			case 446:
			case 449:
			case 466:
			case 497:
			case 504:
			case 535:
			case 566:
			case 603:
			case 606:
			case 618:
			case 626:
			case 643:
			case 660:
			case 678:
			case 707:
			case 716:
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
			case 393:
			case 394:
			case 396:
			case 397:
			case 460:
			case 461:
			case 691:
			case 692:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 163:
			case 170:
			case 375:
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
			case 395:
			case 398:
			case 402:
			case 410:
			case 456:
			case 459:
			case 463:
			case 473:
			case 480:
			case 487:
			case 693:
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
			case 199:
			case 223:
			case 269:
			case 271:
			case 272:
			case 289:
			case 306:
			case 311:
			case 320:
			case 326:
			case 328:
			case 332:
			case 335:
			case 341:
			case 352:
			case 354:
			case 360:
			case 374:
			case 376:
			case 411:
			case 438:
			case 454:
			case 455:
			case 457:
			case 458:
			case 522:
			case 608:
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
			case 483:
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
			case 675:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 82:
			case 206:
			case 207:
			case 264:
			case 267:
			case 268:
			case 319:
			case 726:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 87:
			case 338:
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
			case 284:
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
			case 426:
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
			case 344:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 100:
			case 574:
			case 593:
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
			case 301:
			case 308:
			case 323:
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
			case 210:
			case 215:
			case 217:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 108:
			case 212:
			case 216:
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
			case 245:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 112:
			case 135:
			case 235:
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
			case 619:
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
			case 222:
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
			case 234:
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
			case 450:
			case 451:
				return set[32];
			case 148:
				return set[33];
			case 157:
			case 158:
			case 304:
			case 313:
				return set[34];
			case 159:
			case 428:
				return set[35];
			case 160:
			case 359:
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
			case 214:
			case 219:
			case 225:
			case 227:
			case 231:
			case 233:
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
			case 305:
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
			case 208:
			case 213:
			case 218:
			case 226:
			case 230:
			case 256:
			case 260:
				return set[39];
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 200:
				return set[40];
			case 203:
				return set[41];
			case 204:
			case 263:
				return set[42];
			case 209:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 220:
			case 221:
				return set[43];
			case 224:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 229:
				return set[44];
			case 236:
			case 526:
			case 648:
			case 662:
			case 670:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 238:
			case 239:
			case 420:
			case 421:
			case 495:
			case 496:
			case 502:
			case 503:
			case 601:
			case 602:
			case 604:
			case 605:
			case 616:
			case 617:
			case 641:
			case 642:
			case 658:
			case 659:
				return set[45];
			case 241:
			case 242:
				return set[46];
			case 248:
			case 249:
				return set[47];
			case 250:
				return set[48];
			case 253:
				return set[49];
			case 254:
			case 255:
			case 365:
				return set[50];
			case 257:
			case 350:
			case 637:
			case 638:
			case 640:
			case 681:
			case 698:
			case 699:
			case 701:
			case 710:
			case 711:
			case 713:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 259:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 265:
			case 266:
				return set[51];
			case 270:
			case 312:
			case 327:
				return set[52];
			case 273:
			case 274:
			case 294:
			case 295:
			case 309:
			case 310:
			case 324:
			case 325:
				return set[53];
			case 275:
			case 366:
			case 369:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 276:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 277:
				return set[54];
			case 278:
			case 297:
				return set[55];
			case 279:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 282:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 286:
			case 287:
				return set[56];
			case 288:
			case 293:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 290:
			case 291:
				return set[57];
			case 296:
				return set[58];
			case 298:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 299:
			case 300:
				return set[59];
			case 302:
			case 303:
				return set[60];
			case 314:
			case 315:
				return set[61];
			case 316:
				return set[62];
			case 318:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 321:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 329:
				return set[63];
			case 330:
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 331:
				return set[64];
			case 339:
			case 340:
				return set[65];
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 345:
			case 346:
				return set[66];
			case 348:
			case 349:
				return set[67];
			case 351:
			case 353:
				return set[68];
			case 355:
			case 361:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 358:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 362:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 363:
			case 364:
			case 423:
			case 424:
				return set[69];
			case 367:
			case 368:
			case 370:
			case 371:
				return set[70];
			case 373:
				return set[71];
			case 379:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 381:
			case 382:
			case 390:
			case 391:
				return set[72];
			case 383:
			case 392:
				return set[73];
			case 384:
				return set[74];
			case 385:
			case 388:
				return set[75];
			case 386:
			case 387:
			case 688:
			case 689:
				return set[76];
			case 401:
			case 403:
			case 404:
			case 565:
			case 625:
				return set[77];
			case 405:
			case 406:
				return set[78];
			case 407:
			case 408:
				return set[79];
			case 409:
			case 413:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 414:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 417:
			case 418:
				return set[80];
			case 427:
				return set[81];
			case 429:
			case 442:
				return set[82];
			case 430:
			case 443:
				return set[83];
			case 431:
			case 432:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 437:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 439:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 440:
				return set[84];
			case 441:
				return set[85];
			case 453:
				return set[86];
			case 462:
				return set[87];
			case 464:
			case 465:
			case 533:
			case 534:
			case 676:
			case 677:
				return set[88];
			case 467:
			case 468:
			case 469:
			case 474:
			case 475:
			case 536:
			case 679:
			case 706:
			case 715:
				return set[89];
			case 470:
			case 476:
			case 485:
				return set[90];
			case 471:
			case 472:
			case 477:
			case 478:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 479:
			case 481:
			case 486:
				return set[91];
			case 482:
			case 484:
				return set[92];
			case 491:
			case 510:
			case 511:
			case 567:
			case 655:
			case 667:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 492:
			case 493:
			case 571:
			case 572:
				return set[93];
			case 507:
			case 508:
			case 515:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 513:
			case 514:
				return set[94];
			case 518:
			case 519:
				return set[95];
			case 520:
			case 521:
			case 580:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 525:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 537:
			case 538:
			case 550:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 543:
			case 544:
				return set[96];
			case 545:
			case 546:
				return set[97];
			case 547:
			case 548:
			case 559:
				return set[98];
			case 553:
			case 554:
				return set[99];
			case 555:
			case 556:
			case 695:
				return set[100];
			case 557:
				return set[101];
			case 558:
				return set[102];
			case 560:
			case 570:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 576:
			case 577:
				return set[103];
			case 578:
				return set[104];
			case 579:
			case 615:
				return set[105];
			case 582:
			case 583:
			case 584:
			case 607:
				return set[106];
			case 585:
			case 589:
			case 599:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 591:
				return set[107];
			case 594:
				return set[108];
			case 595:
				return set[109];
			case 610:
			case 611:
			case 613:
			case 687:
			case 690:
				return set[110];
			case 614:
				return set[111];
			case 620:
			case 622:
			case 631:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 621:
				return set[112];
			case 623:
				return set[113];
			case 629:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 633:
			case 634:
				return set[114];
			case 635:
			case 645:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 646:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 647:
				return set[115];
			case 650:
			case 651:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 652:
			case 661:
			case 727:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 653:
				return set[116];
			case 665:
			case 666:
				return set[117];
			case 673:
				return set[118];
			case 680:
			case 682:
				return set[119];
			case 683:
			case 694:
				return set[120];
			case 684:
			case 685:
				return set[121];
			case 686:
				return set[122];
			case 696:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 702:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 705:
			case 714:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 719:
				return set[123];
			case 720:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 721:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 722:
			case 723:
				return set[124];
			case 728:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 729:
				return set[125];
			case 731:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 732:
				return set[126];
			case 733:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 734:
				return set[127];
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
	bool isAlreadyInExpr = false;
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
					goto case 731;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 721;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 414;
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
					currentState = 717;
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
					goto case 414;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[128].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 537;
					} else {
						if (la.kind == 103) {
							currentState = 526;
							break;
						} else {
							if (la.kind == 115) {
								goto case 507;
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
					currentState = 714;
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
					currentState = 709;
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
					goto case 414;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[129].Get(la.kind)) {
					currentState = 708;
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
					goto case 537;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 525;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 507;
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
					currentState = 498;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 488;
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
					currentState = 464;
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
						if (set[130].Get(la.kind)) {
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
					currentState = 462;
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
					goto case 458;
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
				if (set[131].Get(la.kind)) {
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
				if (set[132].Get(la.kind)) {
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
					if (set[133].Get(la.kind)) {
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
				if (set[134].Get(la.kind)) {
					currentState = 159;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 454;
						break;
					} else {
						if (set[135].Get(la.kind)) {
							currentState = 159;
							break;
						} else {
							if (set[130].Get(la.kind)) {
								currentState = 159;
								break;
							} else {
								if (set[133].Get(la.kind)) {
									currentState = 450;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 447;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 444;
											break;
										} else {
											if (set[81].Get(la.kind)) {
												stateStack.Push(159);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 427;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(159);
													goto case 236;
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
				if (set[136].Get(la.kind)) {
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
					goto case 235;
				} else {
					if (la.kind == 58) {
						stateStack.Push(175);
						goto case 234;
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
					currentState = 232;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 228;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 226;
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
										currentState = 222;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 220;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 218;
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
				if (set[120].Get(la.kind)) {
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
					goto case 210;
				} else {
					if (set[39].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 193;
							break;
						} else {
							if (set[39].Get(la.kind)) {
								goto case 208;
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
					stateStack.Push(201);
					goto case 189;
				} else {
					goto case 199;
				}
			}
			case 199: {
				stateStack.Push(200);
				goto case 56;
			}
			case 200: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 201: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 202;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[41].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 203;
				} else {
					goto case 199;
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 63) {
					currentState = 205;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 199;
						break;
					} else {
						if (set[42].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 199;
						}
					}
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				currentState = 199;
				break;
			}
			case 205: {
				PushContext(Context.Type, la, t);
				stateStack.Push(206);
				goto case 37;
			}
			case 206: {
				PopContext();
				goto case 207;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(20, la); // "="
				currentState = 199;
				break;
			}
			case 208: {
				stateStack.Push(209);
				goto case 197;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 22) {
					currentState = 208;
					break;
				} else {
					goto case 192;
				}
			}
			case 210: {
				stateStack.Push(211);
				goto case 217;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 215;
						break;
					} else {
						if (la.kind == 146) {
							goto case 210;
						} else {
							Error(la);
							goto case 211;
						}
					}
				} else {
					goto case 212;
				}
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(143, la); // "Into"
				currentState = 213;
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 197;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 22) {
					currentState = 213;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 215: {
				stateStack.Push(216);
				goto case 217;
			}
			case 216: {
				stateStack.Push(211);
				goto case 212;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				Expect(146, la); // "Join"
				currentState = 177;
				break;
			}
			case 218: {
				stateStack.Push(219);
				goto case 197;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 221;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(70, la); // "By"
				currentState = 223;
				break;
			}
			case 223: {
				stateStack.Push(224);
				goto case 56;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 64) {
					currentState = 225;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 225;
						break;
					} else {
						Error(la);
						goto case 225;
					}
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 223;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 197;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.kind == 22) {
					currentState = 226;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 228: {
				stateStack.Push(229);
				goto case 183;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[37].Get(la.kind)) {
					stateStack.Push(229);
					goto case 176;
				} else {
					Expect(143, la); // "Into"
					currentState = 230;
					break;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 197;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 22) {
					currentState = 230;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 232: {
				stateStack.Push(233);
				goto case 183;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (la.kind == 22) {
					currentState = 232;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(58, la); // "Aggregate"
				currentState = 228;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(126, la); // "From"
				currentState = 232;
				break;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 210) {
					currentState = 419;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 237;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(37, la); // "("
				currentState = 238;
				break;
			}
			case 238: {
				SetIdentifierExpected(la);
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(240);
					goto case 401;
				} else {
					goto case 240;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				Expect(38, la); // ")"
				currentState = 241;
				break;
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 399;
							break;
						} else {
							goto case 243;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 243: {
				stateStack.Push(244);
				goto case 246;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(113, la); // "End"
				currentState = 245;
				break;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				PushContext(Context.Body, la, t);
				goto case 247;
			}
			case 247: {
				stateStack.Push(248);
				goto case 23;
			}
			case 248: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 249;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (set[137].Get(la.kind)) {
					if (set[69].Get(la.kind)) {
						if (set[50].Get(la.kind)) {
							stateStack.Push(247);
							goto case 254;
						} else {
							goto case 247;
						}
					} else {
						if (la.kind == 113) {
							currentState = 252;
							break;
						} else {
							goto case 251;
						}
					}
				} else {
					goto case 250;
				}
			}
			case 250: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 251: {
				Error(la);
				goto case 248;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 247;
				} else {
					if (set[49].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 251;
					}
				}
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				currentState = 248;
				break;
			}
			case 254: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 255;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 380;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 376;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 374;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 372;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 354;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 339;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 335;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 329;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 302;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 298;
																break;
															} else {
																goto case 298;
															}
														} else {
															if (la.kind == 194) {
																currentState = 296;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 294;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 281;
																break;
															} else {
																if (set[138].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 278;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 277;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 276;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 97;
																				} else {
																					if (la.kind == 195) {
																						currentState = 273;
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
																		currentState = 271;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 269;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 256;
																				break;
																			} else {
																				if (set[139].Get(la.kind)) {
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
			case 256: {
				stateStack.Push(257);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 260;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (la.kind == 22) {
					currentState = 256;
					break;
				} else {
					stateStack.Push(258);
					goto case 246;
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(113, la); // "End"
				currentState = 259;
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(261);
					goto case 189;
				} else {
					goto case 56;
				}
			}
			case 261: {
				PopContext();
				nextTokenIsPotentialStartOfExpression = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[41].Get(la.kind)) {
					if (la.kind == 63) {
						currentState = 265;
						break;
					} else {
						if (la.kind == 20) {
							goto case 264;
						} else {
							if (set[42].Get(la.kind)) {
								currentState = endOfStatementTerminatorAndBlock; /* leave this block */
									InformToken(t); /* process Identifier again*/
									/* for processing current token (la): go to the position after processing End */
									goto switchlbl;

							} else {
								Error(la);
								goto case 56;
							}
						}
					}
				} else {
					goto case 56;
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				currentState = 56;
				break;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				currentState = 56;
				break;
			}
			case 265: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(267);
					goto case 37;
				} else {
					goto case 56;
				}
			}
			case 267: {
				PopContext();
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 269: {
				stateStack.Push(270);
				goto case 56;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 22) {
					currentState = 269;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 271: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 272;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 273: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(275);
					goto case 56;
				} else {
					goto case 275;
				}
			}
			case 275: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
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
			case 277: {
				if (la == null) { currentState = 277; break; }
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
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (set[6].Get(la.kind)) {
					goto case 280;
				} else {
					if (la.kind == 5) {
						goto case 279;
					} else {
						goto case 6;
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				stateStack.Push(282);
				goto case 246;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (la.kind == 75) {
					currentState = 286;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 285;
						break;
					} else {
						goto case 283;
					}
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				Expect(113, la); // "End"
				currentState = 284;
				break;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 285: {
				stateStack.Push(283);
				goto case 246;
			}
			case 286: {
				SetIdentifierExpected(la);
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(290);
					goto case 189;
				} else {
					goto case 288;
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 229) {
					currentState = 289;
					break;
				} else {
					goto case 281;
				}
			}
			case 289: {
				stateStack.Push(281);
				goto case 56;
			}
			case 290: {
				PopContext();
				goto case 291;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 63) {
					currentState = 292;
					break;
				} else {
					goto case 288;
				}
			}
			case 292: {
				PushContext(Context.Type, la, t);
				stateStack.Push(293);
				goto case 37;
			}
			case 293: {
				PopContext();
				goto case 288;
			}
			case 294: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 295;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 163) {
					goto case 104;
				} else {
					goto case 297;
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 5) {
					goto case 279;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 280;
					} else {
						goto case 6;
					}
				}
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				Expect(118, la); // "Error"
				currentState = 299;
				break;
			}
			case 299: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 300;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 297;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 301;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 303;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(319);
					goto case 313;
				} else {
					if (la.kind == 110) {
						currentState = 304;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 304: {
				stateStack.Push(305);
				goto case 313;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(138, la); // "In"
				currentState = 306;
				break;
			}
			case 306: {
				stateStack.Push(307);
				goto case 56;
			}
			case 307: {
				stateStack.Push(308);
				goto case 246;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				Expect(163, la); // "Next"
				currentState = 309;
				break;
			}
			case 309: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 310;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (set[23].Get(la.kind)) {
					goto case 311;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 311: {
				stateStack.Push(312);
				goto case 56;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 22) {
					currentState = 311;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(314);
				goto case 157;
			}
			case 314: {
				PopContext();
				goto case 315;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 33) {
					currentState = 316;
					break;
				} else {
					goto case 316;
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(316);
					goto case 144;
				} else {
					if (la.kind == 63) {
						currentState = 317;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 317: {
				PushContext(Context.Type, la, t);
				stateStack.Push(318);
				goto case 37;
			}
			case 318: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(20, la); // "="
				currentState = 320;
				break;
			}
			case 320: {
				stateStack.Push(321);
				goto case 56;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 205) {
					currentState = 328;
					break;
				} else {
					goto case 322;
				}
			}
			case 322: {
				stateStack.Push(323);
				goto case 246;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				Expect(163, la); // "Next"
				currentState = 324;
				break;
			}
			case 324: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 325;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (set[23].Get(la.kind)) {
					goto case 326;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 326: {
				stateStack.Push(327);
				goto case 56;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 22) {
					currentState = 326;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				stateStack.Push(322);
				goto case 56;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 332;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(330);
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(152, la); // "Loop"
				currentState = 331;
				break;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 332: {
				stateStack.Push(333);
				goto case 56;
			}
			case 333: {
				stateStack.Push(334);
				goto case 246;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				stateStack.Push(336);
				goto case 56;
			}
			case 336: {
				stateStack.Push(337);
				goto case 246;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(113, la); // "End"
				currentState = 338;
				break;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 339: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 340;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 74) {
					currentState = 341;
					break;
				} else {
					goto case 341;
				}
			}
			case 341: {
				stateStack.Push(342);
				goto case 56;
			}
			case 342: {
				stateStack.Push(343);
				goto case 23;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 74) {
					currentState = 345;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 344;
					break;
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 345: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 346;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 111) {
					currentState = 347;
					break;
				} else {
					if (set[67].Get(la.kind)) {
						goto case 348;
					} else {
						Error(la);
						goto case 347;
					}
				}
			}
			case 347: {
				stateStack.Push(343);
				goto case 246;
			}
			case 348: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 349;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[140].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 351;
						break;
					} else {
						goto case 351;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(350);
						goto case 56;
					} else {
						Error(la);
						goto case 350;
					}
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 22) {
					currentState = 348;
					break;
				} else {
					goto case 347;
				}
			}
			case 351: {
				stateStack.Push(352);
				goto case 353;
			}
			case 352: {
				stateStack.Push(350);
				goto case 59;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
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
			case 354: {
				stateStack.Push(355);
				goto case 56;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 214) {
					currentState = 363;
					break;
				} else {
					goto case 356;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 357;
				} else {
					goto case 6;
				}
			}
			case 357: {
				stateStack.Push(358);
				goto case 246;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 362;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 360;
							break;
						} else {
							Error(la);
							goto case 357;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 359;
					break;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 360: {
				stateStack.Push(361);
				goto case 56;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 214) {
					currentState = 357;
					break;
				} else {
					goto case 357;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 135) {
					currentState = 360;
					break;
				} else {
					goto case 357;
				}
			}
			case 363: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 364;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (set[50].Get(la.kind)) {
					goto case 365;
				} else {
					goto case 356;
				}
			}
			case 365: {
				stateStack.Push(366);
				goto case 254;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 21) {
					currentState = 370;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 367;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 367: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 368;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (set[50].Get(la.kind)) {
					stateStack.Push(369);
					goto case 254;
				} else {
					goto case 369;
				}
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 21) {
					currentState = 367;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 370: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 371;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (set[50].Get(la.kind)) {
					goto case 365;
				} else {
					goto case 366;
				}
			}
			case 372: {
				stateStack.Push(373);
				goto case 85;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 374: {
				stateStack.Push(375);
				goto case 56;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 376: {
				stateStack.Push(377);
				goto case 56;
			}
			case 377: {
				stateStack.Push(378);
				goto case 246;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				Expect(113, la); // "End"
				currentState = 379;
				break;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
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
			case 380: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(381);
				goto case 189;
			}
			case 381: {
				PopContext();
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 33) {
					currentState = 383;
					break;
				} else {
					goto case 383;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 37) {
					currentState = 396;
					break;
				} else {
					goto case 384;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 22) {
					currentState = 389;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 386;
						break;
					} else {
						goto case 385;
					}
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 20) {
					goto case 264;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 386: {
				PushContext(Context.Type, la, t);
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 162) {
					stateStack.Push(388);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(388);
						goto case 37;
					} else {
						Error(la);
						goto case 388;
					}
				}
			}
			case 388: {
				PopContext();
				goto case 385;
			}
			case 389: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(390);
				goto case 189;
			}
			case 390: {
				PopContext();
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 33) {
					currentState = 392;
					break;
				} else {
					goto case 392;
				}
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (la.kind == 37) {
					currentState = 393;
					break;
				} else {
					goto case 384;
				}
			}
			case 393: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(395);
					goto case 56;
				} else {
					goto case 395;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 22) {
					currentState = 393;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 392;
					break;
				}
			}
			case 396: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 397;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(398);
					goto case 56;
				} else {
					goto case 398;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 22) {
					currentState = 396;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 383;
					break;
				}
			}
			case 399: {
				PushContext(Context.Type, la, t);
				stateStack.Push(400);
				goto case 37;
			}
			case 400: {
				PopContext();
				goto case 243;
			}
			case 401: {
				stateStack.Push(402);
				PushContext(Context.Parameter, la, t);
				goto case 403;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 22) {
					currentState = 401;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 403: {
				SetIdentifierExpected(la);
				goto case 404;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 40) {
					stateStack.Push(403);
					goto case 414;
				} else {
					goto case 405;
				}
			}
			case 405: {
				SetIdentifierExpected(la);
				goto case 406;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (set[141].Get(la.kind)) {
					currentState = 405;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(407);
					goto case 189;
				}
			}
			case 407: {
				PopContext();
				goto case 408;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (la.kind == 63) {
					currentState = 412;
					break;
				} else {
					goto case 409;
				}
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 20) {
					currentState = 411;
					break;
				} else {
					goto case 410;
				}
			}
			case 410: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 411: {
				stateStack.Push(410);
				goto case 56;
			}
			case 412: {
				PushContext(Context.Type, la, t);
				stateStack.Push(413);
				goto case 37;
			}
			case 413: {
				PopContext();
				goto case 409;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				Expect(40, la); // "<"
				currentState = 415;
				break;
			}
			case 415: {
				PushContext(Context.Attribute, la, t);
				goto case 416;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (set[142].Get(la.kind)) {
					currentState = 416;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 417;
					break;
				}
			}
			case 417: {
				PopContext();
				goto case 418;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(37, la); // "("
				currentState = 420;
				break;
			}
			case 420: {
				SetIdentifierExpected(la);
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(422);
					goto case 401;
				} else {
					goto case 422;
				}
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(38, la); // ")"
				currentState = 423;
				break;
			}
			case 423: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (set[50].Get(la.kind)) {
					goto case 254;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(425);
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				Expect(113, la); // "End"
				currentState = 426;
				break;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 440;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(429);
						goto case 431;
					} else {
						Error(la);
						goto case 428;
					}
				}
			}
			case 428: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (la.kind == 17) {
					currentState = 430;
					break;
				} else {
					goto case 428;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 16) {
					currentState = 429;
					break;
				} else {
					goto case 429;
				}
			}
			case 431: {
				PushContext(Context.Xml, la, t);
				goto case 432;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 433;
				break;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (set[143].Get(la.kind)) {
					if (set[144].Get(la.kind)) {
						currentState = 433;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(433);
							goto case 437;
						} else {
							Error(la);
							goto case 433;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 434;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 435;
							break;
						} else {
							Error(la);
							goto case 434;
						}
					}
				}
			}
			case 434: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[145].Get(la.kind)) {
					if (set[146].Get(la.kind)) {
						currentState = 435;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(435);
							goto case 437;
						} else {
							if (la.kind == 10) {
								stateStack.Push(435);
								goto case 431;
							} else {
								Error(la);
								goto case 435;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 436;
					break;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (set[147].Get(la.kind)) {
					if (set[148].Get(la.kind)) {
						currentState = 436;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(436);
							goto case 437;
						} else {
							Error(la);
							goto case 436;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 434;
					break;
				}
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 438;
				break;
			}
			case 438: {
				stateStack.Push(439);
				goto case 56;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 16) {
					currentState = 441;
					break;
				} else {
					goto case 441;
				}
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 440;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(442);
						goto case 431;
					} else {
						goto case 428;
					}
				}
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 17) {
					currentState = 443;
					break;
				} else {
					goto case 428;
				}
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 16) {
					currentState = 442;
					break;
				} else {
					goto case 442;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				Expect(37, la); // "("
				currentState = 445;
				break;
			}
			case 445: {
				readXmlIdentifier = true;
				stateStack.Push(446);
				goto case 189;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				Expect(38, la); // ")"
				currentState = 159;
				break;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				Expect(37, la); // "("
				currentState = 448;
				break;
			}
			case 448: {
				PushContext(Context.Type, la, t);
				stateStack.Push(449);
				goto case 37;
			}
			case 449: {
				PopContext();
				goto case 446;
			}
			case 450: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 451;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 10) {
					currentState = 452;
					break;
				} else {
					goto case 452;
				}
			}
			case 452: {
				stateStack.Push(453);
				goto case 85;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 11) {
					currentState = 159;
					break;
				} else {
					goto case 159;
				}
			}
			case 454: {
				activeArgument = 0;
				goto case 455;
			}
			case 455: {
				stateStack.Push(456);
				goto case 56;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 22) {
					currentState = 457;
					break;
				} else {
					goto case 446;
				}
			}
			case 457: {
				activeArgument++;
				goto case 455;
			}
			case 458: {
				stateStack.Push(459);
				goto case 56;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 22) {
					currentState = 460;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 460: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 461;
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (set[23].Get(la.kind)) {
					goto case 458;
				} else {
					goto case 459;
				}
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(463);
					goto case 37;
				} else {
					goto case 463;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 22) {
					currentState = 462;
					break;
				} else {
					goto case 45;
				}
			}
			case 464: {
				SetIdentifierExpected(la);
				goto case 465;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 467;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(466);
							goto case 401;
						} else {
							Error(la);
							goto case 466;
						}
					}
				} else {
					goto case 466;
				}
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 467: {
				stateStack.Push(466);
				goto case 468;
			}
			case 468: {
				SetIdentifierExpected(la);
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 470;
					break;
				} else {
					goto case 470;
				}
			}
			case 470: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(471);
				goto case 485;
			}
			case 471: {
				PopContext();
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 63) {
					currentState = 486;
					break;
				} else {
					goto case 473;
				}
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 22) {
					currentState = 474;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 474: {
				SetIdentifierExpected(la);
				goto case 475;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 476;
					break;
				} else {
					goto case 476;
				}
			}
			case 476: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(477);
				goto case 485;
			}
			case 477: {
				PopContext();
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (la.kind == 63) {
					currentState = 479;
					break;
				} else {
					goto case 473;
				}
			}
			case 479: {
				PushContext(Context.Type, la, t);
				stateStack.Push(480);
				goto case 481;
			}
			case 480: {
				PopContext();
				goto case 473;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (set[92].Get(la.kind)) {
					goto case 484;
				} else {
					if (la.kind == 35) {
						currentState = 482;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 482: {
				stateStack.Push(483);
				goto case 484;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				if (la.kind == 22) {
					currentState = 482;
					break;
				} else {
					goto case 66;
				}
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
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
			case 485: {
				if (la == null) { currentState = 485; break; }
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
			case 486: {
				PushContext(Context.Type, la, t);
				stateStack.Push(487);
				goto case 481;
			}
			case 487: {
				PopContext();
				goto case 473;
			}
			case 488: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(489);
				goto case 189;
			}
			case 489: {
				PopContext();
				goto case 490;
			}
			case 490: {
				if (la == null) { currentState = 490; break; }
				if (la.kind == 37) {
					currentState = 495;
					break;
				} else {
					goto case 491;
				}
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 63) {
					currentState = 492;
					break;
				} else {
					goto case 23;
				}
			}
			case 492: {
				PushContext(Context.Type, la, t);
				goto case 493;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 40) {
					stateStack.Push(493);
					goto case 414;
				} else {
					stateStack.Push(494);
					goto case 37;
				}
			}
			case 494: {
				PopContext();
				goto case 23;
			}
			case 495: {
				SetIdentifierExpected(la);
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(497);
					goto case 401;
				} else {
					goto case 497;
				}
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				Expect(38, la); // ")"
				currentState = 491;
				break;
			}
			case 498: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(499);
				goto case 189;
			}
			case 499: {
				PopContext();
				goto case 500;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 505;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 502;
							break;
						} else {
							goto case 501;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 501: {
				Error(la);
				goto case 23;
			}
			case 502: {
				SetIdentifierExpected(la);
				goto case 503;
			}
			case 503: {
				if (la == null) { currentState = 503; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(504);
					goto case 401;
				} else {
					goto case 504;
				}
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 505: {
				PushContext(Context.Type, la, t);
				stateStack.Push(506);
				goto case 37;
			}
			case 506: {
				PopContext();
				goto case 23;
			}
			case 507: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 508;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				Expect(115, la); // "Enum"
				currentState = 509;
				break;
			}
			case 509: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(510);
				goto case 189;
			}
			case 510: {
				PopContext();
				goto case 511;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				if (la.kind == 63) {
					currentState = 523;
					break;
				} else {
					goto case 512;
				}
			}
			case 512: {
				stateStack.Push(513);
				goto case 23;
			}
			case 513: {
				SetIdentifierExpected(la);
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (set[95].Get(la.kind)) {
					goto case 518;
				} else {
					Expect(113, la); // "End"
					currentState = 515;
					break;
				}
			}
			case 515: {
				if (la == null) { currentState = 515; break; }
				Expect(115, la); // "Enum"
				currentState = 516;
				break;
			}
			case 516: {
				stateStack.Push(517);
				goto case 23;
			}
			case 517: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 518: {
				SetIdentifierExpected(la);
				goto case 519;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (la.kind == 40) {
					stateStack.Push(518);
					goto case 414;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(520);
					goto case 189;
				}
			}
			case 520: {
				PopContext();
				goto case 521;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (la.kind == 20) {
					currentState = 522;
					break;
				} else {
					goto case 512;
				}
			}
			case 522: {
				stateStack.Push(512);
				goto case 56;
			}
			case 523: {
				PushContext(Context.Type, la, t);
				stateStack.Push(524);
				goto case 37;
			}
			case 524: {
				PopContext();
				goto case 512;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				Expect(103, la); // "Delegate"
				currentState = 526;
				break;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 210) {
					currentState = 527;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 527;
						break;
					} else {
						Error(la);
						goto case 527;
					}
				}
			}
			case 527: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 528;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				currentState = 529;
				break;
			}
			case 529: {
				PopContext();
				goto case 530;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (la.kind == 37) {
					currentState = 533;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 531;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 531: {
				PushContext(Context.Type, la, t);
				stateStack.Push(532);
				goto case 37;
			}
			case 532: {
				PopContext();
				goto case 23;
			}
			case 533: {
				SetIdentifierExpected(la);
				goto case 534;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 536;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(535);
							goto case 401;
						} else {
							Error(la);
							goto case 535;
						}
					}
				} else {
					goto case 535;
				}
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				Expect(38, la); // ")"
				currentState = 530;
				break;
			}
			case 536: {
				stateStack.Push(535);
				goto case 468;
			}
			case 537: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 538;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 155) {
					currentState = 539;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 539;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 539;
							break;
						} else {
							Error(la);
							goto case 539;
						}
					}
				}
			}
			case 539: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(540);
				goto case 189;
			}
			case 540: {
				PopContext();
				goto case 541;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 37) {
					currentState = 705;
					break;
				} else {
					goto case 542;
				}
			}
			case 542: {
				stateStack.Push(543);
				goto case 23;
			}
			case 543: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 702;
				} else {
					goto case 545;
				}
			}
			case 545: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 546;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 696;
				} else {
					goto case 547;
				}
			}
			case 547: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 548;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (set[99].Get(la.kind)) {
					goto case 553;
				} else {
					isMissingModifier = false;
					goto case 549;
				}
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				Expect(113, la); // "End"
				currentState = 550;
				break;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 155) {
					currentState = 551;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 551;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 551;
							break;
						} else {
							Error(la);
							goto case 551;
						}
					}
				}
			}
			case 551: {
				stateStack.Push(552);
				goto case 23;
			}
			case 552: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 553: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 554;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				if (la.kind == 40) {
					stateStack.Push(553);
					goto case 414;
				} else {
					isMissingModifier = true;
					goto case 555;
				}
			}
			case 555: {
				SetIdentifierExpected(la);
				goto case 556;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (set[129].Get(la.kind)) {
					currentState = 695;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(547);
					goto case 537;
				} else {
					if (la.kind == 103) {
						stateStack.Push(547);
						goto case 525;
					} else {
						if (la.kind == 115) {
							stateStack.Push(547);
							goto case 507;
						} else {
							if (la.kind == 142) {
								stateStack.Push(547);
								goto case 9;
							} else {
								if (set[102].Get(la.kind)) {
									stateStack.Push(547);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 558;
								} else {
									Error(la);
									goto case 547;
								}
							}
						}
					}
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (set[119].Get(la.kind)) {
					stateStack.Push(559);
					goto case 680;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(559);
						goto case 662;
					} else {
						if (la.kind == 101) {
							stateStack.Push(559);
							goto case 646;
						} else {
							if (la.kind == 119) {
								stateStack.Push(559);
								goto case 631;
							} else {
								if (la.kind == 98) {
									stateStack.Push(559);
									goto case 619;
								} else {
									if (la.kind == 186) {
										stateStack.Push(559);
										goto case 574;
									} else {
										if (la.kind == 172) {
											stateStack.Push(559);
											goto case 560;
										} else {
											Error(la);
											goto case 559;
										}
									}
								}
							}
						}
					}
				}
			}
			case 559: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(172, la); // "Operator"
				currentState = 561;
				break;
			}
			case 561: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 562;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				currentState = 563;
				break;
			}
			case 563: {
				PopContext();
				goto case 564;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(37, la); // "("
				currentState = 565;
				break;
			}
			case 565: {
				stateStack.Push(566);
				goto case 401;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				Expect(38, la); // ")"
				currentState = 567;
				break;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				if (la.kind == 63) {
					currentState = 571;
					break;
				} else {
					goto case 568;
				}
			}
			case 568: {
				stateStack.Push(569);
				goto case 246;
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				Expect(113, la); // "End"
				currentState = 570;
				break;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 571: {
				PushContext(Context.Type, la, t);
				goto case 572;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				if (la.kind == 40) {
					stateStack.Push(572);
					goto case 414;
				} else {
					stateStack.Push(573);
					goto case 37;
				}
			}
			case 573: {
				PopContext();
				goto case 568;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				Expect(186, la); // "Property"
				currentState = 575;
				break;
			}
			case 575: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(576);
				goto case 189;
			}
			case 576: {
				PopContext();
				goto case 577;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (la.kind == 37) {
					currentState = 616;
					break;
				} else {
					goto case 578;
				}
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (la.kind == 63) {
					currentState = 614;
					break;
				} else {
					goto case 579;
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				if (la.kind == 136) {
					currentState = 609;
					break;
				} else {
					goto case 580;
				}
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 20) {
					currentState = 608;
					break;
				} else {
					goto case 581;
				}
			}
			case 581: {
				stateStack.Push(582);
				goto case 23;
			}
			case 582: {
				PopContext();
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 40) {
					stateStack.Push(583);
					goto case 414;
				} else {
					goto case 584;
				}
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (set[150].Get(la.kind)) {
					currentState = 607;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 585;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (la.kind == 128) {
					currentState = 586;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 586;
						break;
					} else {
						Error(la);
						goto case 586;
					}
				}
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (la.kind == 37) {
					currentState = 604;
					break;
				} else {
					goto case 587;
				}
			}
			case 587: {
				stateStack.Push(588);
				goto case 246;
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				Expect(113, la); // "End"
				currentState = 589;
				break;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 128) {
					currentState = 590;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 590;
						break;
					} else {
						Error(la);
						goto case 590;
					}
				}
			}
			case 590: {
				stateStack.Push(591);
				goto case 23;
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				if (set[108].Get(la.kind)) {
					goto case 594;
				} else {
					goto case 592;
				}
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(113, la); // "End"
				currentState = 593;
				break;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				if (la.kind == 40) {
					stateStack.Push(594);
					goto case 414;
				} else {
					goto case 595;
				}
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (set[150].Get(la.kind)) {
					currentState = 595;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 596;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 596;
							break;
						} else {
							Error(la);
							goto case 596;
						}
					}
				}
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (la.kind == 37) {
					currentState = 601;
					break;
				} else {
					goto case 597;
				}
			}
			case 597: {
				stateStack.Push(598);
				goto case 246;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				Expect(113, la); // "End"
				currentState = 599;
				break;
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				if (la.kind == 128) {
					currentState = 600;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 600;
						break;
					} else {
						Error(la);
						goto case 600;
					}
				}
			}
			case 600: {
				stateStack.Push(592);
				goto case 23;
			}
			case 601: {
				SetIdentifierExpected(la);
				goto case 602;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(603);
					goto case 401;
				} else {
					goto case 603;
				}
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(38, la); // ")"
				currentState = 597;
				break;
			}
			case 604: {
				SetIdentifierExpected(la);
				goto case 605;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(606);
					goto case 401;
				} else {
					goto case 606;
				}
			}
			case 606: {
				if (la == null) { currentState = 606; break; }
				Expect(38, la); // ")"
				currentState = 587;
				break;
			}
			case 607: {
				SetIdentifierExpected(la);
				goto case 584;
			}
			case 608: {
				stateStack.Push(581);
				goto case 56;
			}
			case 609: {
				PushContext(Context.Type, la, t);
				stateStack.Push(610);
				goto case 37;
			}
			case 610: {
				PopContext();
				goto case 611;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				if (la.kind == 22) {
					currentState = 612;
					break;
				} else {
					goto case 580;
				}
			}
			case 612: {
				PushContext(Context.Type, la, t);
				stateStack.Push(613);
				goto case 37;
			}
			case 613: {
				PopContext();
				goto case 611;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (la.kind == 40) {
					stateStack.Push(614);
					goto case 414;
				} else {
					if (la.kind == 162) {
						stateStack.Push(579);
						goto case 69;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(615);
							goto case 37;
						} else {
							Error(la);
							goto case 579;
						}
					}
				}
			}
			case 615: {
				PopContext();
				goto case 579;
			}
			case 616: {
				SetIdentifierExpected(la);
				goto case 617;
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(618);
					goto case 401;
				} else {
					goto case 618;
				}
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(38, la); // ")"
				currentState = 578;
				break;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				Expect(98, la); // "Custom"
				currentState = 620;
				break;
			}
			case 620: {
				stateStack.Push(621);
				goto case 631;
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (set[113].Get(la.kind)) {
					goto case 623;
				} else {
					Expect(113, la); // "End"
					currentState = 622;
					break;
				}
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				if (la.kind == 40) {
					stateStack.Push(623);
					goto case 414;
				} else {
					if (la.kind == 56) {
						currentState = 624;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 624;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 624;
								break;
							} else {
								Error(la);
								goto case 624;
							}
						}
					}
				}
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(37, la); // "("
				currentState = 625;
				break;
			}
			case 625: {
				stateStack.Push(626);
				goto case 401;
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				Expect(38, la); // ")"
				currentState = 627;
				break;
			}
			case 627: {
				stateStack.Push(628);
				goto case 246;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				Expect(113, la); // "End"
				currentState = 629;
				break;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 56) {
					currentState = 630;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 630;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 630;
							break;
						} else {
							Error(la);
							goto case 630;
						}
					}
				}
			}
			case 630: {
				stateStack.Push(621);
				goto case 23;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				Expect(119, la); // "Event"
				currentState = 632;
				break;
			}
			case 632: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(633);
				goto case 189;
			}
			case 633: {
				PopContext();
				goto case 634;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (la.kind == 63) {
					currentState = 644;
					break;
				} else {
					if (set[151].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 641;
							break;
						} else {
							goto case 635;
						}
					} else {
						Error(la);
						goto case 635;
					}
				}
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 136) {
					currentState = 636;
					break;
				} else {
					goto case 23;
				}
			}
			case 636: {
				PushContext(Context.Type, la, t);
				stateStack.Push(637);
				goto case 37;
			}
			case 637: {
				PopContext();
				goto case 638;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 22) {
					currentState = 639;
					break;
				} else {
					goto case 23;
				}
			}
			case 639: {
				PushContext(Context.Type, la, t);
				stateStack.Push(640);
				goto case 37;
			}
			case 640: {
				PopContext();
				goto case 638;
			}
			case 641: {
				SetIdentifierExpected(la);
				goto case 642;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(643);
					goto case 401;
				} else {
					goto case 643;
				}
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				Expect(38, la); // ")"
				currentState = 635;
				break;
			}
			case 644: {
				PushContext(Context.Type, la, t);
				stateStack.Push(645);
				goto case 37;
			}
			case 645: {
				PopContext();
				goto case 635;
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				Expect(101, la); // "Declare"
				currentState = 647;
				break;
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 648;
					break;
				} else {
					goto case 648;
				}
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				if (la.kind == 210) {
					currentState = 649;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 649;
						break;
					} else {
						Error(la);
						goto case 649;
					}
				}
			}
			case 649: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(650);
				goto case 189;
			}
			case 650: {
				PopContext();
				goto case 651;
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				Expect(149, la); // "Lib"
				currentState = 652;
				break;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				Expect(3, la); // LiteralString
				currentState = 653;
				break;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (la.kind == 59) {
					currentState = 661;
					break;
				} else {
					goto case 654;
				}
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				if (la.kind == 37) {
					currentState = 658;
					break;
				} else {
					goto case 655;
				}
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				if (la.kind == 63) {
					currentState = 656;
					break;
				} else {
					goto case 23;
				}
			}
			case 656: {
				PushContext(Context.Type, la, t);
				stateStack.Push(657);
				goto case 37;
			}
			case 657: {
				PopContext();
				goto case 23;
			}
			case 658: {
				SetIdentifierExpected(la);
				goto case 659;
			}
			case 659: {
				if (la == null) { currentState = 659; break; }
				if (set[77].Get(la.kind)) {
					stateStack.Push(660);
					goto case 401;
				} else {
					goto case 660;
				}
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(38, la); // ")"
				currentState = 655;
				break;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				Expect(3, la); // LiteralString
				currentState = 654;
				break;
			}
			case 662: {
				if (la == null) { currentState = 662; break; }
				if (la.kind == 210) {
					currentState = 663;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 663;
						break;
					} else {
						Error(la);
						goto case 663;
					}
				}
			}
			case 663: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 664;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				currentState = 665;
				break;
			}
			case 665: {
				PopContext();
				goto case 666;
			}
			case 666: {
				if (la == null) { currentState = 666; break; }
				if (la.kind == 37) {
					currentState = 676;
					break;
				} else {
					if (la.kind == 134 || la.kind == 136) {
						currentState = 673;
						break;
					} else {
						goto case 667;
					}
				}
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				if (la.kind == 63) {
					currentState = 671;
					break;
				} else {
					goto case 668;
				}
			}
			case 668: {
				stateStack.Push(669);
				goto case 246;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				Expect(113, la); // "End"
				currentState = 670;
				break;
			}
			case 670: {
				if (la == null) { currentState = 670; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 501;
					}
				}
			}
			case 671: {
				PushContext(Context.Type, la, t);
				stateStack.Push(672);
				goto case 37;
			}
			case 672: {
				PopContext();
				goto case 668;
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				if (la.kind == 153 || la.kind == 158 || la.kind == 159) {
					currentState = 675;
					break;
				} else {
					goto case 674;
				}
			}
			case 674: {
				stateStack.Push(667);
				goto case 37;
			}
			case 675: {
				if (la == null) { currentState = 675; break; }
				Expect(26, la); // "."
				currentState = 674;
				break;
			}
			case 676: {
				SetIdentifierExpected(la);
				goto case 677;
			}
			case 677: {
				if (la == null) { currentState = 677; break; }
				if (set[149].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 679;
						break;
					} else {
						if (set[77].Get(la.kind)) {
							stateStack.Push(678);
							goto case 401;
						} else {
							Error(la);
							goto case 678;
						}
					}
				} else {
					goto case 678;
				}
			}
			case 678: {
				if (la == null) { currentState = 678; break; }
				Expect(38, la); // ")"
				currentState = 666;
				break;
			}
			case 679: {
				stateStack.Push(678);
				goto case 468;
			}
			case 680: {
				stateStack.Push(681);
				SetIdentifierExpected(la);
				goto case 682;
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				if (la.kind == 22) {
					currentState = 680;
					break;
				} else {
					goto case 23;
				}
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				if (la.kind == 88) {
					currentState = 683;
					break;
				} else {
					goto case 683;
				}
			}
			case 683: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(684);
				goto case 694;
			}
			case 684: {
				PopContext();
				goto case 685;
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				if (la.kind == 33) {
					currentState = 686;
					break;
				} else {
					goto case 686;
				}
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				if (la.kind == 37) {
					currentState = 691;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 688;
						break;
					} else {
						goto case 687;
					}
				}
			}
			case 687: {
				if (la == null) { currentState = 687; break; }
				if (la.kind == 20) {
					goto case 264;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 688: {
				PushContext(Context.Type, la, t);
				goto case 689;
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				if (la.kind == 162) {
					stateStack.Push(690);
					goto case 69;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(690);
						goto case 37;
					} else {
						Error(la);
						goto case 690;
					}
				}
			}
			case 690: {
				PopContext();
				goto case 687;
			}
			case 691: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 692;
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(693);
					goto case 56;
				} else {
					goto case 693;
				}
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				if (la.kind == 22) {
					currentState = 691;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 686;
					break;
				}
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				if (set[135].Get(la.kind)) {
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
			case 695: {
				isMissingModifier = false;
				goto case 555;
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				Expect(136, la); // "Implements"
				currentState = 697;
				break;
			}
			case 697: {
				PushContext(Context.Type, la, t);
				stateStack.Push(698);
				goto case 37;
			}
			case 698: {
				PopContext();
				goto case 699;
			}
			case 699: {
				if (la == null) { currentState = 699; break; }
				if (la.kind == 22) {
					currentState = 700;
					break;
				} else {
					stateStack.Push(547);
					goto case 23;
				}
			}
			case 700: {
				PushContext(Context.Type, la, t);
				stateStack.Push(701);
				goto case 37;
			}
			case 701: {
				PopContext();
				goto case 699;
			}
			case 702: {
				if (la == null) { currentState = 702; break; }
				Expect(140, la); // "Inherits"
				currentState = 703;
				break;
			}
			case 703: {
				PushContext(Context.Type, la, t);
				stateStack.Push(704);
				goto case 37;
			}
			case 704: {
				PopContext();
				stateStack.Push(545);
				goto case 23;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				Expect(169, la); // "Of"
				currentState = 706;
				break;
			}
			case 706: {
				stateStack.Push(707);
				goto case 468;
			}
			case 707: {
				if (la == null) { currentState = 707; break; }
				Expect(38, la); // ")"
				currentState = 542;
				break;
			}
			case 708: {
				isMissingModifier = false;
				goto case 28;
			}
			case 709: {
				PushContext(Context.Type, la, t);
				stateStack.Push(710);
				goto case 37;
			}
			case 710: {
				PopContext();
				goto case 711;
			}
			case 711: {
				if (la == null) { currentState = 711; break; }
				if (la.kind == 22) {
					currentState = 712;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 712: {
				PushContext(Context.Type, la, t);
				stateStack.Push(713);
				goto case 37;
			}
			case 713: {
				PopContext();
				goto case 711;
			}
			case 714: {
				if (la == null) { currentState = 714; break; }
				Expect(169, la); // "Of"
				currentState = 715;
				break;
			}
			case 715: {
				stateStack.Push(716);
				goto case 468;
			}
			case 716: {
				if (la == null) { currentState = 716; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 717: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 718;
			}
			case 718: {
				if (la == null) { currentState = 718; break; }
				if (set[49].Get(la.kind)) {
					currentState = 718;
					break;
				} else {
					PopContext();
					stateStack.Push(719);
					goto case 23;
				}
			}
			case 719: {
				if (la == null) { currentState = 719; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(719);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 720;
					break;
				}
			}
			case 720: {
				if (la == null) { currentState = 720; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 721: {
				if (la == null) { currentState = 721; break; }
				Expect(137, la); // "Imports"
				currentState = 722;
				break;
			}
			case 722: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 723;
			}
			case 723: {
				if (la == null) { currentState = 723; break; }
				if (set[152].Get(la.kind)) {
					currentState = 729;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 725;
						break;
					} else {
						Error(la);
						goto case 724;
					}
				}
			}
			case 724: {
				PopContext();
				goto case 23;
			}
			case 725: {
				stateStack.Push(726);
				goto case 189;
			}
			case 726: {
				if (la == null) { currentState = 726; break; }
				Expect(20, la); // "="
				currentState = 727;
				break;
			}
			case 727: {
				if (la == null) { currentState = 727; break; }
				Expect(3, la); // LiteralString
				currentState = 728;
				break;
			}
			case 728: {
				if (la == null) { currentState = 728; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 724;
				break;
			}
			case 729: {
				if (la == null) { currentState = 729; break; }
				if (la.kind == 37) {
					stateStack.Push(729);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 730;
						break;
					} else {
						goto case 724;
					}
				}
			}
			case 730: {
				stateStack.Push(724);
				goto case 37;
			}
			case 731: {
				if (la == null) { currentState = 731; break; }
				Expect(173, la); // "Option"
				currentState = 732;
				break;
			}
			case 732: {
				if (la == null) { currentState = 732; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 734;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 733;
						break;
					} else {
						goto case 501;
					}
				}
			}
			case 733: {
				if (la == null) { currentState = 733; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 501;
					}
				}
			}
			case 734: {
				if (la == null) { currentState = 734; break; }
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
		new BitArray(new int[] {4194304, 67108864, 64, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {66189314, -1174405161, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {65140738, 973078487, 51384096, 972018404, 1030969181, -17106229, 97186287, -8260}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-66189316, 1174405162, -51384097, -972018401, -1030969178, 17106228, -97186288, 8259}),
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
		new BitArray(new int[] {7340034, -2147483614, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 0, 0, 0, 0, 0}),
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