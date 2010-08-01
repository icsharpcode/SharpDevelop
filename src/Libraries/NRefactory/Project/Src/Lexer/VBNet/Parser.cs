using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 237;
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
			case 71:
			case 238:
			case 473:
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
			case 166:
			case 172:
			case 178:
			case 215:
			case 219:
			case 258:
			case 358:
			case 367:
			case 420:
			case 460:
			case 470:
			case 481:
			case 511:
			case 547:
			case 604:
			case 621:
			case 689:
				return set[6];
			case 12:
			case 13:
			case 512:
			case 513:
			case 558:
			case 568:
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
			case 230:
			case 233:
			case 234:
			case 244:
			case 259:
			case 263:
			case 285:
			case 300:
			case 311:
			case 314:
			case 320:
			case 325:
			case 334:
			case 335:
			case 355:
			case 375:
			case 466:
			case 478:
			case 484:
			case 488:
			case 496:
			case 504:
			case 514:
			case 523:
			case 540:
			case 545:
			case 553:
			case 559:
			case 562:
			case 569:
			case 572:
			case 599:
			case 602:
			case 629:
			case 639:
			case 643:
			case 668:
			case 688:
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
			case 231:
			case 245:
			case 261:
			case 315:
			case 356:
			case 400:
			case 521:
			case 541:
			case 560:
			case 564:
			case 570:
			case 600:
			case 640:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 489:
			case 524:
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
			case 672:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 127:
			case 188:
			case 189:
			case 239:
			case 390:
			case 391:
			case 408:
			case 409:
			case 410:
			case 411:
			case 499:
			case 500:
			case 533:
			case 534:
			case 635:
			case 636:
			case 681:
			case 682:
				return set[14];
			case 33:
			case 34:
			case 461:
			case 462:
			case 471:
			case 472:
			case 501:
			case 502:
			case 626:
			case 637:
			case 638:
				return set[15];
			case 35:
			case 37:
			case 131:
			case 142:
			case 145:
			case 161:
			case 176:
			case 192:
			case 270:
			case 295:
			case 374:
			case 387:
			case 423:
			case 477:
			case 495:
			case 503:
			case 581:
			case 584:
			case 608:
			case 611:
			case 616:
			case 628:
			case 642:
			case 661:
			case 664:
			case 667:
			case 673:
			case 676:
			case 694:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 77:
			case 81:
			case 137:
			case 350:
			case 427:
				return set[19];
			case 42:
			case 151:
			case 158:
			case 162:
			case 224:
			case 394:
			case 419:
			case 422:
			case 535:
			case 536:
			case 596:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 139:
			case 140:
				return set[20];
			case 45:
			case 141:
			case 227:
			case 372:
			case 397:
			case 421:
			case 424:
			case 438:
			case 469:
			case 476:
			case 507:
			case 538:
			case 575:
			case 578:
			case 590:
			case 598:
			case 615:
			case 632:
			case 646:
			case 671:
			case 680:
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
			case 432:
			case 433:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 153:
			case 160:
			case 353:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 54:
			case 143:
			case 144:
			case 146:
			case 155:
			case 371:
			case 373:
			case 377:
			case 385:
			case 431:
			case 435:
			case 445:
			case 452:
			case 459:
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
			case 64:
			case 79:
			case 129:
			case 152:
			case 154:
			case 156:
			case 159:
			case 168:
			case 170:
			case 210:
			case 243:
			case 247:
			case 249:
			case 250:
			case 267:
			case 284:
			case 289:
			case 298:
			case 304:
			case 306:
			case 310:
			case 313:
			case 319:
			case 330:
			case 332:
			case 338:
			case 352:
			case 354:
			case 386:
			case 413:
			case 429:
			case 430:
			case 494:
			case 580:
				return set[23];
			case 58:
			case 62:
			case 132:
				return set[24];
			case 63:
			case 73:
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 65:
			case 80:
			case 455:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 67:
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 68:
			case 69:
				return set[25];
			case 70:
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 72:
				return set[26];
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 78:
			case 191:
			case 193:
			case 194:
			case 297:
			case 690:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 83:
			case 316:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 87:
			case 262:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 91:
			case 401:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 95:
			case 322:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 96:
			case 546:
			case 565:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 100:
			case 279:
			case 286:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 103:
			case 197:
			case 202:
			case 204:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 104:
			case 199:
			case 203:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 107:
			case 232:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 108:
			case 222:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 111:
			case 169:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 115:
			case 591:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 118:
			case 181:
			case 209:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 119:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 122:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 124:
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 126:
				return set[27];
			case 128:
				return set[28];
			case 130:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 133:
				return set[29];
			case 134:
				return set[30];
			case 135:
			case 136:
			case 425:
			case 426:
				return set[31];
			case 138:
				return set[32];
			case 147:
			case 148:
			case 282:
			case 291:
				return set[33];
			case 149:
			case 403:
				return set[34];
			case 150:
			case 337:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 157:
				return set[35];
			case 163:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 164:
			case 165:
				return set[36];
			case 167:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 171:
			case 185:
			case 201:
			case 206:
			case 212:
			case 214:
			case 218:
			case 220:
				return set[37];
			case 173:
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 175:
			case 177:
			case 283:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 179:
			case 180:
			case 182:
			case 184:
			case 186:
			case 187:
			case 195:
			case 200:
			case 205:
			case 213:
			case 217:
				return set[38];
			case 183:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 190:
				return set[39];
			case 196:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 207:
			case 208:
				return set[40];
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 216:
				return set[41];
			case 223:
			case 498:
			case 620:
			case 634:
			case 641:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 225:
			case 226:
			case 395:
			case 396:
			case 467:
			case 468:
			case 474:
			case 475:
			case 573:
			case 574:
			case 576:
			case 577:
			case 588:
			case 589:
			case 613:
			case 614:
			case 630:
			case 631:
				return set[42];
			case 228:
			case 229:
				return set[43];
			case 235:
			case 236:
				return set[44];
			case 237:
				return set[45];
			case 240:
				return set[46];
			case 241:
			case 242:
			case 343:
				return set[47];
			case 246:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 248:
			case 290:
			case 305:
				return set[48];
			case 251:
			case 252:
			case 272:
			case 273:
			case 287:
			case 288:
			case 302:
			case 303:
				return set[49];
			case 253:
			case 344:
			case 347:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 254:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 255:
				return set[50];
			case 256:
			case 275:
				return set[51];
			case 257:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 264:
			case 265:
				return set[52];
			case 266:
			case 271:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 268:
			case 269:
				return set[53];
			case 274:
				return set[54];
			case 276:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 277:
			case 278:
				return set[55];
			case 280:
			case 281:
				return set[56];
			case 292:
			case 293:
				return set[57];
			case 294:
				return set[58];
			case 296:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 299:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 307:
				return set[59];
			case 308:
			case 312:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 309:
				return set[60];
			case 317:
			case 318:
				return set[61];
			case 321:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 323:
			case 324:
				return set[62];
			case 326:
			case 327:
				return set[63];
			case 328:
			case 609:
			case 610:
			case 612:
			case 649:
			case 662:
			case 663:
			case 665:
			case 674:
			case 675:
			case 677:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 329:
			case 331:
				return set[64];
			case 333:
			case 339:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 336:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 341:
			case 342:
			case 398:
			case 399:
				return set[65];
			case 345:
			case 346:
			case 348:
			case 349:
				return set[66];
			case 351:
				return set[67];
			case 357:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 359:
			case 360:
			case 368:
			case 369:
				return set[68];
			case 361:
			case 370:
				return set[69];
			case 362:
				return set[70];
			case 363:
			case 366:
				return set[71];
			case 364:
			case 365:
			case 655:
			case 656:
				return set[72];
			case 376:
			case 378:
			case 379:
			case 537:
			case 597:
				return set[73];
			case 380:
			case 381:
				return set[74];
			case 382:
			case 383:
				return set[75];
			case 384:
			case 388:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 389:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 392:
			case 393:
				return set[76];
			case 402:
				return set[77];
			case 404:
			case 417:
				return set[78];
			case 405:
			case 418:
				return set[79];
			case 406:
			case 407:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 412:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 414:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 415:
				return set[80];
			case 416:
				return set[81];
			case 428:
				return set[82];
			case 434:
				return set[83];
			case 436:
			case 437:
			case 505:
			case 506:
			case 644:
			case 645:
				return set[84];
			case 439:
			case 440:
			case 441:
			case 446:
			case 447:
			case 508:
			case 647:
			case 670:
			case 679:
				return set[85];
			case 442:
			case 448:
			case 457:
				return set[86];
			case 443:
			case 444:
			case 449:
			case 450:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 451:
			case 453:
			case 458:
				return set[87];
			case 454:
			case 456:
				return set[88];
			case 463:
			case 482:
			case 483:
			case 539:
			case 627:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 464:
			case 465:
			case 543:
			case 544:
				return set[89];
			case 479:
			case 480:
			case 487:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 485:
			case 486:
				return set[90];
			case 490:
			case 491:
				return set[91];
			case 492:
			case 493:
			case 552:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 497:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 509:
			case 510:
			case 522:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 515:
			case 516:
				return set[92];
			case 517:
			case 518:
				return set[93];
			case 519:
			case 520:
			case 531:
				return set[94];
			case 525:
			case 526:
				return set[95];
			case 527:
			case 528:
			case 659:
				return set[96];
			case 529:
				return set[97];
			case 530:
				return set[98];
			case 532:
			case 542:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 548:
			case 549:
				return set[99];
			case 550:
				return set[100];
			case 551:
			case 587:
				return set[101];
			case 554:
			case 555:
			case 556:
			case 579:
				return set[102];
			case 557:
			case 561:
			case 571:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 563:
				return set[103];
			case 566:
				return set[104];
			case 567:
				return set[105];
			case 582:
			case 583:
			case 585:
			case 654:
			case 657:
				return set[106];
			case 586:
				return set[107];
			case 592:
			case 594:
			case 603:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 593:
				return set[108];
			case 595:
				return set[109];
			case 601:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 605:
			case 606:
				return set[110];
			case 607:
			case 617:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 618:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 619:
				return set[111];
			case 622:
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 624:
			case 633:
			case 691:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 625:
				return set[112];
			case 648:
			case 650:
				return set[113];
			case 651:
			case 658:
				return set[114];
			case 652:
			case 653:
				return set[115];
			case 660:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 666:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 669:
			case 678:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 683:
				return set[116];
			case 684:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 685:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 686:
			case 687:
				return set[117];
			case 692:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 693:
				return set[118];
			case 695:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 696:
				return set[119];
			case 697:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 698:
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
					goto case 695;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 685;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 389;
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
					currentState = 681;
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
					goto case 389;
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
						goto case 509;
					} else {
						if (la.kind == 103) {
							currentState = 498;
							break;
						} else {
							if (la.kind == 115) {
								goto case 479;
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
				goto case 178;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 678;
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
					currentState = 673;
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
					goto case 389;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[122].Get(la.kind)) {
					currentState = 672;
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
					goto case 509;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 497;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 479;
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
					currentState = 470;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 460;
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
					currentState = 436;
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
						if (set[123].Get(la.kind)) {
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
				goto case 81;
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
					currentState = 434;
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
					goto case 430;
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
				if (set[124].Get(la.kind)) {
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
				if (set[125].Get(la.kind)) {
					currentState = 60;
					break;
				} else {
					if (set[33].Get(la.kind)) {
						stateStack.Push(133);
						goto case 147;
					} else {
						if (la.kind == 220) {
							currentState = 129;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(62);
								goto case 67;
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
				stateStack.Push(65);
				goto case 56;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 22) {
					currentState = 64;
					break;
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
				if (la == null) { currentState = 67; break; }
				Expect(162, la); // "New"
				currentState = 68;
				break;
			}
			case 68: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 69;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(126);
					goto case 37;
				} else {
					goto case 70;
				}
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 233) {
					currentState = 73;
					break;
				} else {
					goto case 71;
				}
			}
			case 71: {
				Error(la);
				goto case 72;
			}
			case 72: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 73: {
				stateStack.Push(72);
				goto case 74;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(35, la); // "{"
				currentState = 75;
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (la.kind == 147) {
					currentState = 76;
					break;
				} else {
					goto case 76;
				}
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				Expect(26, la); // "."
				currentState = 77;
				break;
			}
			case 77: {
				stateStack.Push(78);
				goto case 81;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				Expect(20, la); // "="
				currentState = 79;
				break;
			}
			case 79: {
				stateStack.Push(80);
				goto case 56;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				if (la.kind == 22) {
					currentState = 75;
					break;
				} else {
					goto case 66;
				}
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (la.kind == 2) {
					goto case 125;
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
								goto case 124;
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
												goto case 123;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 122;
													} else {
														if (la.kind == 65) {
															goto case 121;
														} else {
															if (la.kind == 66) {
																goto case 120;
															} else {
																if (la.kind == 67) {
																	goto case 119;
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
																				goto case 118;
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
																																		goto case 117;
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
																																					goto case 116;
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
																																																goto case 115;
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
																																																						goto case 114;
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
																																																									goto case 113;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 112;
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
																																																																		goto case 111;
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
																																																																							goto case 110;
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
																																																																										goto case 109;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 108;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 107;
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
																																																																																			goto case 106;
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
																																																																																									goto case 105;
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
																																																																																													goto case 104;
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
																																																																																																goto case 103;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 102;
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
																																																																																																																goto case 101;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 100;
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
																																																																																																																								goto case 99;
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
																																																																																																																														goto case 98;
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
																																																																																																																																						goto case 97;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 96;
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
																																																																																																																																																			goto case 95;
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
																																																																																																																																																									goto case 94;
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
																																																																																																																																																												goto case 93;
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
																																																																																																																																																															goto case 92;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 91;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 90;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 89;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 88;
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
																																																																																																																																																																								goto case 87;
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
																																																																																																																																																																													goto case 86;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 85;
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
																																																																																																																																																																																				goto case 84;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 83;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 82;
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
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 127;
						break;
					} else {
						goto case 70;
					}
				} else {
					goto case 72;
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (la.kind == 35) {
					stateStack.Push(72);
					goto case 63;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 71;
					}
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				currentState = 72;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 59;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(144, la); // "Is"
				currentState = 131;
				break;
			}
			case 131: {
				PushContext(Context.Type, la, t);
				stateStack.Push(132);
				goto case 37;
			}
			case 132: {
				PopContext();
				goto case 62;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(133);
					goto case 134;
				} else {
					goto case 62;
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 37) {
					currentState = 139;
					break;
				} else {
					if (set[126].Get(la.kind)) {
						currentState = 135;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 135: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 136;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 10) {
					currentState = 137;
					break;
				} else {
					goto case 137;
				}
			}
			case 137: {
				stateStack.Push(138);
				goto case 81;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 139: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 140;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 169) {
					currentState = 142;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						if (set[22].Get(la.kind)) {
							stateStack.Push(141);
							goto case 48;
						} else {
							goto case 141;
						}
					} else {
						Error(la);
						goto case 141;
					}
				}
			}
			case 141: {
				PopContext();
				goto case 45;
			}
			case 142: {
				PushContext(Context.Type, la, t);
				stateStack.Push(143);
				goto case 37;
			}
			case 143: {
				PopContext();
				goto case 144;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 22) {
					currentState = 145;
					break;
				} else {
					goto case 141;
				}
			}
			case 145: {
				PushContext(Context.Type, la, t);
				stateStack.Push(146);
				goto case 37;
			}
			case 146: {
				PopContext();
				goto case 144;
			}
			case 147: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 148;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (set[127].Get(la.kind)) {
					currentState = 149;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 429;
						break;
					} else {
						if (set[128].Get(la.kind)) {
							currentState = 149;
							break;
						} else {
							if (set[123].Get(la.kind)) {
								currentState = 149;
								break;
							} else {
								if (set[126].Get(la.kind)) {
									currentState = 425;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 422;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 419;
											break;
										} else {
											if (set[77].Get(la.kind)) {
												stateStack.Push(149);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 402;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(149);
													goto case 223;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(149);
														PushContext(Context.Query, la, t);
														goto case 163;
													} else {
														if (set[35].Get(la.kind)) {
															stateStack.Push(149);
															goto case 157;
														} else {
															if (la.kind == 135) {
																stateStack.Push(149);
																goto case 150;
															} else {
																Error(la);
																goto case 149;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 149: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(135, la); // "If"
				currentState = 151;
				break;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(37, la); // "("
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 56;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				Expect(22, la); // ","
				currentState = 154;
				break;
			}
			case 154: {
				stateStack.Push(155);
				goto case 56;
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (la.kind == 22) {
					currentState = 156;
					break;
				} else {
					goto case 45;
				}
			}
			case 156: {
				stateStack.Push(45);
				goto case 56;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				if (set[129].Get(la.kind)) {
					currentState = 162;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 158;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(37, la); // "("
				currentState = 159;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 56;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				Expect(22, la); // ","
				currentState = 161;
				break;
			}
			case 161: {
				stateStack.Push(45);
				goto case 37;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				Expect(37, la); // "("
				currentState = 156;
				break;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 126) {
					stateStack.Push(164);
					goto case 222;
				} else {
					if (la.kind == 58) {
						stateStack.Push(164);
						goto case 221;
					} else {
						Error(la);
						goto case 164;
					}
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(164);
					goto case 165;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 126) {
					currentState = 219;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 215;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 213;
							break;
						} else {
							if (la.kind == 107) {
								goto case 113;
							} else {
								if (la.kind == 230) {
									currentState = 56;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 209;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 207;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 205;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 179;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 166;
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
			case 166: {
				stateStack.Push(167);
				goto case 172;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				Expect(171, la); // "On"
				currentState = 168;
				break;
			}
			case 168: {
				stateStack.Push(169);
				goto case 56;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				Expect(116, la); // "Equals"
				currentState = 170;
				break;
			}
			case 170: {
				stateStack.Push(171);
				goto case 56;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.kind == 22) {
					currentState = 168;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 172: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(173);
				goto case 178;
			}
			case 173: {
				PopContext();
				goto case 174;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 63) {
					currentState = 176;
					break;
				} else {
					goto case 175;
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 176: {
				PushContext(Context.Type, la, t);
				stateStack.Push(177);
				goto case 37;
			}
			case 177: {
				PopContext();
				goto case 175;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[114].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 115;
					} else {
						goto case 6;
					}
				}
			}
			case 179: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 180;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 146) {
					goto case 197;
				} else {
					if (set[38].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 182;
							break;
						} else {
							if (set[38].Get(la.kind)) {
								goto case 195;
							} else {
								Error(la);
								goto case 181;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(70, la); // "By"
				currentState = 182;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 186;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (la.kind == 22) {
					currentState = 182;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 184;
					break;
				}
			}
			case 184: {
				stateStack.Push(185);
				goto case 186;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 184;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 186: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 187;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(188);
					goto case 178;
				} else {
					goto case 56;
				}
			}
			case 188: {
				PopContext();
				goto case 189;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.kind == 63) {
					currentState = 192;
					break;
				} else {
					if (la.kind == 20) {
						goto case 191;
					} else {
						if (set[39].Get(la.kind)) {
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
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				currentState = 56;
				break;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				currentState = 56;
				break;
			}
			case 192: {
				PushContext(Context.Type, la, t);
				stateStack.Push(193);
				goto case 37;
			}
			case 193: {
				PopContext();
				goto case 194;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 195: {
				stateStack.Push(196);
				goto case 186;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 22) {
					currentState = 195;
					break;
				} else {
					goto case 181;
				}
			}
			case 197: {
				stateStack.Push(198);
				goto case 204;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 202;
						break;
					} else {
						if (la.kind == 146) {
							goto case 197;
						} else {
							Error(la);
							goto case 198;
						}
					}
				} else {
					goto case 199;
				}
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(143, la); // "Into"
				currentState = 200;
				break;
			}
			case 200: {
				stateStack.Push(201);
				goto case 186;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.kind == 22) {
					currentState = 200;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 202: {
				stateStack.Push(203);
				goto case 204;
			}
			case 203: {
				stateStack.Push(198);
				goto case 199;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				Expect(146, la); // "Join"
				currentState = 166;
				break;
			}
			case 205: {
				stateStack.Push(206);
				goto case 186;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 208;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				Expect(70, la); // "By"
				currentState = 210;
				break;
			}
			case 210: {
				stateStack.Push(211);
				goto case 56;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 64) {
					currentState = 212;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 212;
						break;
					} else {
						Error(la);
						goto case 212;
					}
				}
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 22) {
					currentState = 210;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 213: {
				stateStack.Push(214);
				goto case 186;
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
				goto case 172;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(216);
					goto case 165;
				} else {
					Expect(143, la); // "Into"
					currentState = 217;
					break;
				}
			}
			case 217: {
				stateStack.Push(218);
				goto case 186;
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
				stateStack.Push(220);
				goto case 172;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 22) {
					currentState = 219;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(58, la); // "Aggregate"
				currentState = 215;
				break;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(126, la); // "From"
				currentState = 219;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 210) {
					currentState = 394;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 224;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				Expect(37, la); // "("
				currentState = 225;
				break;
			}
			case 225: {
				SetIdentifierExpected(la);
				goto case 226;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(227);
					goto case 376;
				} else {
					goto case 227;
				}
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(38, la); // ")"
				currentState = 228;
				break;
			}
			case 228: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 229;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 374;
							break;
						} else {
							goto case 230;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 233;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(113, la); // "End"
				currentState = 232;
				break;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 233: {
				PushContext(Context.Body, la, t);
				goto case 234;
			}
			case 234: {
				stateStack.Push(235);
				goto case 23;
			}
			case 235: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 236;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (set[130].Get(la.kind)) {
					if (set[65].Get(la.kind)) {
						if (set[47].Get(la.kind)) {
							stateStack.Push(234);
							goto case 241;
						} else {
							goto case 234;
						}
					} else {
						if (la.kind == 113) {
							currentState = 239;
							break;
						} else {
							goto case 238;
						}
					}
				} else {
					goto case 237;
				}
			}
			case 237: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 238: {
				Error(la);
				goto case 235;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 234;
				} else {
					if (set[46].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 238;
					}
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				currentState = 235;
				break;
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 358;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 354;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 352;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 350;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 332;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 317;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 313;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 307;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 280;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 276;
																break;
															} else {
																goto case 276;
															}
														} else {
															if (la.kind == 194) {
																currentState = 274;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 272;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 259;
																break;
															} else {
																if (set[131].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 256;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 255;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 254;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 93;
																				} else {
																					if (la.kind == 195) {
																						currentState = 251;
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
																		currentState = 249;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 247;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 243;
																				break;
																			} else {
																				if (set[132].Get(la.kind)) {
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
			case 243: {
				stateStack.Push(244);
				goto case 56;
			}
			case 244: {
				stateStack.Push(245);
				goto case 233;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				Expect(113, la); // "End"
				currentState = 246;
				break;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				stateStack.Push(248);
				goto case 56;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (la.kind == 22) {
					currentState = 247;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 249: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 250;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 251: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 252;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(253);
					goto case 56;
				} else {
					goto case 253;
				}
			}
			case 253: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 108) {
					goto case 112;
				} else {
					if (la.kind == 124) {
						goto case 109;
					} else {
						if (la.kind == 231) {
							goto case 83;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 108) {
					goto case 112;
				} else {
					if (la.kind == 124) {
						goto case 109;
					} else {
						if (la.kind == 231) {
							goto case 83;
						} else {
							if (la.kind == 197) {
								goto case 95;
							} else {
								if (la.kind == 210) {
									goto case 91;
								} else {
									if (la.kind == 127) {
										goto case 107;
									} else {
										if (la.kind == 186) {
											goto case 96;
										} else {
											if (la.kind == 218) {
												goto case 87;
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
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[6].Get(la.kind)) {
					goto case 258;
				} else {
					if (la.kind == 5) {
						goto case 257;
					} else {
						goto case 6;
					}
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				stateStack.Push(260);
				goto case 233;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				if (la.kind == 75) {
					currentState = 264;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 263;
						break;
					} else {
						goto case 261;
					}
				}
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				Expect(113, la); // "End"
				currentState = 262;
				break;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 263: {
				stateStack.Push(261);
				goto case 233;
			}
			case 264: {
				SetIdentifierExpected(la);
				goto case 265;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(268);
					goto case 178;
				} else {
					goto case 266;
				}
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 229) {
					currentState = 267;
					break;
				} else {
					goto case 259;
				}
			}
			case 267: {
				stateStack.Push(259);
				goto case 56;
			}
			case 268: {
				PopContext();
				goto case 269;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (la.kind == 63) {
					currentState = 270;
					break;
				} else {
					goto case 266;
				}
			}
			case 270: {
				PushContext(Context.Type, la, t);
				stateStack.Push(271);
				goto case 37;
			}
			case 271: {
				PopContext();
				goto case 266;
			}
			case 272: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 273;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 163) {
					goto case 100;
				} else {
					goto case 275;
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (la.kind == 5) {
					goto case 257;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 258;
					} else {
						goto case 6;
					}
				}
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				Expect(118, la); // "Error"
				currentState = 277;
				break;
			}
			case 277: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 278;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 275;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 279;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 280: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 281;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (set[33].Get(la.kind)) {
					stateStack.Push(297);
					goto case 291;
				} else {
					if (la.kind == 110) {
						currentState = 282;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 291;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				Expect(138, la); // "In"
				currentState = 284;
				break;
			}
			case 284: {
				stateStack.Push(285);
				goto case 56;
			}
			case 285: {
				stateStack.Push(286);
				goto case 233;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(163, la); // "Next"
				currentState = 287;
				break;
			}
			case 287: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 288;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (set[23].Get(la.kind)) {
					goto case 289;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 56;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (la.kind == 22) {
					currentState = 289;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 291: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(292);
				goto case 147;
			}
			case 292: {
				PopContext();
				goto case 293;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 33) {
					currentState = 294;
					break;
				} else {
					goto case 294;
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(294);
					goto case 134;
				} else {
					if (la.kind == 63) {
						currentState = 295;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 295: {
				PushContext(Context.Type, la, t);
				stateStack.Push(296);
				goto case 37;
			}
			case 296: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(20, la); // "="
				currentState = 298;
				break;
			}
			case 298: {
				stateStack.Push(299);
				goto case 56;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 205) {
					currentState = 306;
					break;
				} else {
					goto case 300;
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 233;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(163, la); // "Next"
				currentState = 302;
				break;
			}
			case 302: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 303;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (set[23].Get(la.kind)) {
					goto case 304;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 304: {
				stateStack.Push(305);
				goto case 56;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 22) {
					currentState = 304;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 306: {
				stateStack.Push(300);
				goto case 56;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 310;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(308);
						goto case 233;
					} else {
						goto case 6;
					}
				}
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				Expect(152, la); // "Loop"
				currentState = 309;
				break;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 310: {
				stateStack.Push(311);
				goto case 56;
			}
			case 311: {
				stateStack.Push(312);
				goto case 233;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 313: {
				stateStack.Push(314);
				goto case 56;
			}
			case 314: {
				stateStack.Push(315);
				goto case 233;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(113, la); // "End"
				currentState = 316;
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 317: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 318;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 74) {
					currentState = 319;
					break;
				} else {
					goto case 319;
				}
			}
			case 319: {
				stateStack.Push(320);
				goto case 56;
			}
			case 320: {
				stateStack.Push(321);
				goto case 23;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 74) {
					currentState = 323;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 322;
					break;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 323: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 324;
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 111) {
					currentState = 325;
					break;
				} else {
					if (set[63].Get(la.kind)) {
						goto case 326;
					} else {
						Error(la);
						goto case 325;
					}
				}
			}
			case 325: {
				stateStack.Push(321);
				goto case 233;
			}
			case 326: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 327;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 329;
						break;
					} else {
						goto case 329;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(328);
						goto case 56;
					} else {
						Error(la);
						goto case 328;
					}
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 22) {
					currentState = 326;
					break;
				} else {
					goto case 325;
				}
			}
			case 329: {
				stateStack.Push(330);
				goto case 331;
			}
			case 330: {
				stateStack.Push(328);
				goto case 59;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
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
			case 332: {
				stateStack.Push(333);
				goto case 56;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 214) {
					currentState = 341;
					break;
				} else {
					goto case 334;
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 335;
				} else {
					goto case 6;
				}
			}
			case 335: {
				stateStack.Push(336);
				goto case 233;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 340;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 338;
							break;
						} else {
							Error(la);
							goto case 335;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 337;
					break;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 338: {
				stateStack.Push(339);
				goto case 56;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 214) {
					currentState = 335;
					break;
				} else {
					goto case 335;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 135) {
					currentState = 338;
					break;
				} else {
					goto case 335;
				}
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (set[47].Get(la.kind)) {
					goto case 343;
				} else {
					goto case 334;
				}
			}
			case 343: {
				stateStack.Push(344);
				goto case 241;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 21) {
					currentState = 348;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 345;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 345: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 346;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (set[47].Get(la.kind)) {
					stateStack.Push(347);
					goto case 241;
				} else {
					goto case 347;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 21) {
					currentState = 345;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 348: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 349;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[47].Get(la.kind)) {
					goto case 343;
				} else {
					goto case 344;
				}
			}
			case 350: {
				stateStack.Push(351);
				goto case 81;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 352: {
				stateStack.Push(353);
				goto case 56;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 354: {
				stateStack.Push(355);
				goto case 56;
			}
			case 355: {
				stateStack.Push(356);
				goto case 233;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				Expect(113, la); // "End"
				currentState = 357;
				break;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 233) {
					goto case 82;
				} else {
					if (la.kind == 211) {
						goto case 90;
					} else {
						goto case 6;
					}
				}
			}
			case 358: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(359);
				goto case 178;
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
					currentState = 373;
					break;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 22) {
					currentState = 367;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 364;
						break;
					} else {
						goto case 363;
					}
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 20) {
					goto case 191;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 364: {
				PushContext(Context.Type, la, t);
				goto case 365;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 162) {
					stateStack.Push(366);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(366);
						goto case 37;
					} else {
						Error(la);
						goto case 366;
					}
				}
			}
			case 366: {
				PopContext();
				goto case 363;
			}
			case 367: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(368);
				goto case 178;
			}
			case 368: {
				PopContext();
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 33) {
					currentState = 370;
					break;
				} else {
					goto case 370;
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 37) {
					currentState = 371;
					break;
				} else {
					goto case 362;
				}
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 22) {
					currentState = 371;
					break;
				} else {
					goto case 372;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(38, la); // ")"
				currentState = 362;
				break;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 22) {
					currentState = 373;
					break;
				} else {
					goto case 372;
				}
			}
			case 374: {
				PushContext(Context.Type, la, t);
				stateStack.Push(375);
				goto case 37;
			}
			case 375: {
				PopContext();
				goto case 230;
			}
			case 376: {
				stateStack.Push(377);
				PushContext(Context.Parameter, la, t);
				goto case 378;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 22) {
					currentState = 376;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 378: {
				SetIdentifierExpected(la);
				goto case 379;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 40) {
					stateStack.Push(378);
					goto case 389;
				} else {
					goto case 380;
				}
			}
			case 380: {
				SetIdentifierExpected(la);
				goto case 381;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (set[134].Get(la.kind)) {
					currentState = 380;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(382);
					goto case 178;
				}
			}
			case 382: {
				PopContext();
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 63) {
					currentState = 387;
					break;
				} else {
					goto case 384;
				}
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 20) {
					currentState = 386;
					break;
				} else {
					goto case 385;
				}
			}
			case 385: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 386: {
				stateStack.Push(385);
				goto case 56;
			}
			case 387: {
				PushContext(Context.Type, la, t);
				stateStack.Push(388);
				goto case 37;
			}
			case 388: {
				PopContext();
				goto case 384;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(40, la); // "<"
				currentState = 390;
				break;
			}
			case 390: {
				PushContext(Context.Attribute, la, t);
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (set[135].Get(la.kind)) {
					currentState = 391;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 392;
					break;
				}
			}
			case 392: {
				PopContext();
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				Expect(37, la); // "("
				currentState = 395;
				break;
			}
			case 395: {
				SetIdentifierExpected(la);
				goto case 396;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(397);
					goto case 376;
				} else {
					goto case 397;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				Expect(38, la); // ")"
				currentState = 398;
				break;
			}
			case 398: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 399;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (set[47].Get(la.kind)) {
					goto case 241;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(400);
						goto case 233;
					} else {
						goto case 6;
					}
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				Expect(113, la); // "End"
				currentState = 401;
				break;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 415;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(404);
						goto case 406;
					} else {
						Error(la);
						goto case 403;
					}
				}
			}
			case 403: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 17) {
					currentState = 405;
					break;
				} else {
					goto case 403;
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 16) {
					currentState = 404;
					break;
				} else {
					goto case 404;
				}
			}
			case 406: {
				PushContext(Context.Xml, la, t);
				goto case 407;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 408;
				break;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 408;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(408);
							goto case 412;
						} else {
							Error(la);
							goto case 408;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 409;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 410;
							break;
						} else {
							Error(la);
							goto case 409;
						}
					}
				}
			}
			case 409: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 410;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(410);
							goto case 412;
						} else {
							if (la.kind == 10) {
								stateStack.Push(410);
								goto case 406;
							} else {
								Error(la);
								goto case 410;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 411;
					break;
				}
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 411;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(411);
							goto case 412;
						} else {
							Error(la);
							goto case 411;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 409;
					break;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 413;
				break;
			}
			case 413: {
				stateStack.Push(414);
				goto case 56;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 16) {
					currentState = 416;
					break;
				} else {
					goto case 416;
				}
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 415;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(417);
						goto case 406;
					} else {
						goto case 403;
					}
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (la.kind == 17) {
					currentState = 418;
					break;
				} else {
					goto case 403;
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 16) {
					currentState = 417;
					break;
				} else {
					goto case 417;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(37, la); // "("
				currentState = 420;
				break;
			}
			case 420: {
				readXmlIdentifier = true;
				stateStack.Push(421);
				goto case 178;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				Expect(38, la); // ")"
				currentState = 149;
				break;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(37, la); // "("
				currentState = 423;
				break;
			}
			case 423: {
				PushContext(Context.Type, la, t);
				stateStack.Push(424);
				goto case 37;
			}
			case 424: {
				PopContext();
				goto case 421;
			}
			case 425: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 426;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 10) {
					currentState = 427;
					break;
				} else {
					goto case 427;
				}
			}
			case 427: {
				stateStack.Push(428);
				goto case 81;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 11) {
					currentState = 149;
					break;
				} else {
					goto case 149;
				}
			}
			case 429: {
				stateStack.Push(421);
				goto case 56;
			}
			case 430: {
				stateStack.Push(431);
				goto case 56;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (la.kind == 22) {
					currentState = 432;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 432: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 433;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (set[23].Get(la.kind)) {
					goto case 430;
				} else {
					goto case 431;
				}
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(435);
					goto case 37;
				} else {
					goto case 435;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 22) {
					currentState = 434;
					break;
				} else {
					goto case 45;
				}
			}
			case 436: {
				SetIdentifierExpected(la);
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 439;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(438);
							goto case 376;
						} else {
							Error(la);
							goto case 438;
						}
					}
				} else {
					goto case 438;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 439: {
				stateStack.Push(438);
				goto case 440;
			}
			case 440: {
				SetIdentifierExpected(la);
				goto case 441;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 442;
					break;
				} else {
					goto case 442;
				}
			}
			case 442: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(443);
				goto case 457;
			}
			case 443: {
				PopContext();
				goto case 444;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 63) {
					currentState = 458;
					break;
				} else {
					goto case 445;
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 22) {
					currentState = 446;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 446: {
				SetIdentifierExpected(la);
				goto case 447;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 448;
					break;
				} else {
					goto case 448;
				}
			}
			case 448: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(449);
				goto case 457;
			}
			case 449: {
				PopContext();
				goto case 450;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 63) {
					currentState = 451;
					break;
				} else {
					goto case 445;
				}
			}
			case 451: {
				PushContext(Context.Type, la, t);
				stateStack.Push(452);
				goto case 453;
			}
			case 452: {
				PopContext();
				goto case 445;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[88].Get(la.kind)) {
					goto case 456;
				} else {
					if (la.kind == 35) {
						currentState = 454;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 454: {
				stateStack.Push(455);
				goto case 456;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (la.kind == 22) {
					currentState = 454;
					break;
				} else {
					goto case 66;
				}
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 101;
					} else {
						if (la.kind == 84) {
							goto case 117;
						} else {
							if (la.kind == 209) {
								goto case 92;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 2) {
					goto case 125;
				} else {
					if (la.kind == 62) {
						goto case 123;
					} else {
						if (la.kind == 64) {
							goto case 122;
						} else {
							if (la.kind == 65) {
								goto case 121;
							} else {
								if (la.kind == 66) {
									goto case 120;
								} else {
									if (la.kind == 67) {
										goto case 119;
									} else {
										if (la.kind == 70) {
											goto case 118;
										} else {
											if (la.kind == 87) {
												goto case 116;
											} else {
												if (la.kind == 104) {
													goto case 114;
												} else {
													if (la.kind == 107) {
														goto case 113;
													} else {
														if (la.kind == 116) {
															goto case 111;
														} else {
															if (la.kind == 121) {
																goto case 110;
															} else {
																if (la.kind == 133) {
																	goto case 106;
																} else {
																	if (la.kind == 139) {
																		goto case 105;
																	} else {
																		if (la.kind == 143) {
																			goto case 104;
																		} else {
																			if (la.kind == 146) {
																				goto case 103;
																			} else {
																				if (la.kind == 147) {
																					goto case 102;
																				} else {
																					if (la.kind == 170) {
																						goto case 99;
																					} else {
																						if (la.kind == 176) {
																							goto case 98;
																						} else {
																							if (la.kind == 184) {
																								goto case 97;
																							} else {
																								if (la.kind == 203) {
																									goto case 94;
																								} else {
																									if (la.kind == 212) {
																										goto case 89;
																									} else {
																										if (la.kind == 213) {
																											goto case 88;
																										} else {
																											if (la.kind == 223) {
																												goto case 86;
																											} else {
																												if (la.kind == 224) {
																													goto case 85;
																												} else {
																													if (la.kind == 230) {
																														goto case 84;
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
			case 458: {
				PushContext(Context.Type, la, t);
				stateStack.Push(459);
				goto case 453;
			}
			case 459: {
				PopContext();
				goto case 445;
			}
			case 460: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(461);
				goto case 178;
			}
			case 461: {
				PopContext();
				goto case 462;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (la.kind == 37) {
					currentState = 467;
					break;
				} else {
					goto case 463;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 63) {
					currentState = 464;
					break;
				} else {
					goto case 23;
				}
			}
			case 464: {
				PushContext(Context.Type, la, t);
				goto case 465;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (la.kind == 40) {
					stateStack.Push(465);
					goto case 389;
				} else {
					stateStack.Push(466);
					goto case 37;
				}
			}
			case 466: {
				PopContext();
				goto case 23;
			}
			case 467: {
				SetIdentifierExpected(la);
				goto case 468;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(469);
					goto case 376;
				} else {
					goto case 469;
				}
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				Expect(38, la); // ")"
				currentState = 463;
				break;
			}
			case 470: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(471);
				goto case 178;
			}
			case 471: {
				PopContext();
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 477;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 474;
							break;
						} else {
							goto case 473;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 473: {
				Error(la);
				goto case 23;
			}
			case 474: {
				SetIdentifierExpected(la);
				goto case 475;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(476);
					goto case 376;
				} else {
					goto case 476;
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 477: {
				PushContext(Context.Type, la, t);
				stateStack.Push(478);
				goto case 37;
			}
			case 478: {
				PopContext();
				goto case 23;
			}
			case 479: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 480;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				Expect(115, la); // "Enum"
				currentState = 481;
				break;
			}
			case 481: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(482);
				goto case 178;
			}
			case 482: {
				PopContext();
				goto case 483;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				if (la.kind == 63) {
					currentState = 495;
					break;
				} else {
					goto case 484;
				}
			}
			case 484: {
				stateStack.Push(485);
				goto case 23;
			}
			case 485: {
				SetIdentifierExpected(la);
				goto case 486;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (set[91].Get(la.kind)) {
					goto case 490;
				} else {
					Expect(113, la); // "End"
					currentState = 487;
					break;
				}
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				Expect(115, la); // "Enum"
				currentState = 488;
				break;
			}
			case 488: {
				stateStack.Push(489);
				goto case 23;
			}
			case 489: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 490: {
				SetIdentifierExpected(la);
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 40) {
					stateStack.Push(490);
					goto case 389;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(492);
					goto case 178;
				}
			}
			case 492: {
				PopContext();
				goto case 493;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 20) {
					currentState = 494;
					break;
				} else {
					goto case 484;
				}
			}
			case 494: {
				stateStack.Push(484);
				goto case 56;
			}
			case 495: {
				PushContext(Context.Type, la, t);
				stateStack.Push(496);
				goto case 37;
			}
			case 496: {
				PopContext();
				goto case 484;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				Expect(103, la); // "Delegate"
				currentState = 498;
				break;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (la.kind == 210) {
					currentState = 499;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 499;
						break;
					} else {
						Error(la);
						goto case 499;
					}
				}
			}
			case 499: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 500;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				currentState = 501;
				break;
			}
			case 501: {
				PopContext();
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 37) {
					currentState = 505;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 503;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 503: {
				PushContext(Context.Type, la, t);
				stateStack.Push(504);
				goto case 37;
			}
			case 504: {
				PopContext();
				goto case 23;
			}
			case 505: {
				SetIdentifierExpected(la);
				goto case 506;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 508;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(507);
							goto case 376;
						} else {
							Error(la);
							goto case 507;
						}
					}
				} else {
					goto case 507;
				}
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				Expect(38, la); // ")"
				currentState = 502;
				break;
			}
			case 508: {
				stateStack.Push(507);
				goto case 440;
			}
			case 509: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 510;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (la.kind == 155) {
					currentState = 511;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 511;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 511;
							break;
						} else {
							Error(la);
							goto case 511;
						}
					}
				}
			}
			case 511: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(512);
				goto case 178;
			}
			case 512: {
				PopContext();
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (la.kind == 37) {
					currentState = 669;
					break;
				} else {
					goto case 514;
				}
			}
			case 514: {
				stateStack.Push(515);
				goto case 23;
			}
			case 515: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 516;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 666;
				} else {
					goto case 517;
				}
			}
			case 517: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 518;
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 660;
				} else {
					goto case 519;
				}
			}
			case 519: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 520;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (set[95].Get(la.kind)) {
					goto case 525;
				} else {
					isMissingModifier = false;
					goto case 521;
				}
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				Expect(113, la); // "End"
				currentState = 522;
				break;
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (la.kind == 155) {
					currentState = 523;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 523;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 523;
							break;
						} else {
							Error(la);
							goto case 523;
						}
					}
				}
			}
			case 523: {
				stateStack.Push(524);
				goto case 23;
			}
			case 524: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 525: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 40) {
					stateStack.Push(525);
					goto case 389;
				} else {
					isMissingModifier = true;
					goto case 527;
				}
			}
			case 527: {
				SetIdentifierExpected(la);
				goto case 528;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (set[122].Get(la.kind)) {
					currentState = 659;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 529;
				}
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(519);
					goto case 509;
				} else {
					if (la.kind == 103) {
						stateStack.Push(519);
						goto case 497;
					} else {
						if (la.kind == 115) {
							stateStack.Push(519);
							goto case 479;
						} else {
							if (la.kind == 142) {
								stateStack.Push(519);
								goto case 9;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(519);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 530;
								} else {
									Error(la);
									goto case 519;
								}
							}
						}
					}
				}
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (set[113].Get(la.kind)) {
					stateStack.Push(531);
					goto case 648;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(531);
						goto case 634;
					} else {
						if (la.kind == 101) {
							stateStack.Push(531);
							goto case 618;
						} else {
							if (la.kind == 119) {
								stateStack.Push(531);
								goto case 603;
							} else {
								if (la.kind == 98) {
									stateStack.Push(531);
									goto case 591;
								} else {
									if (la.kind == 186) {
										stateStack.Push(531);
										goto case 546;
									} else {
										if (la.kind == 172) {
											stateStack.Push(531);
											goto case 532;
										} else {
											Error(la);
											goto case 531;
										}
									}
								}
							}
						}
					}
				}
			}
			case 531: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				Expect(172, la); // "Operator"
				currentState = 533;
				break;
			}
			case 533: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 534;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				currentState = 535;
				break;
			}
			case 535: {
				PopContext();
				goto case 536;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				Expect(37, la); // "("
				currentState = 537;
				break;
			}
			case 537: {
				stateStack.Push(538);
				goto case 376;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				Expect(38, la); // ")"
				currentState = 539;
				break;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				if (la.kind == 63) {
					currentState = 543;
					break;
				} else {
					goto case 540;
				}
			}
			case 540: {
				stateStack.Push(541);
				goto case 233;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				Expect(113, la); // "End"
				currentState = 542;
				break;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 543: {
				PushContext(Context.Type, la, t);
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (la.kind == 40) {
					stateStack.Push(544);
					goto case 389;
				} else {
					stateStack.Push(545);
					goto case 37;
				}
			}
			case 545: {
				PopContext();
				goto case 540;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				Expect(186, la); // "Property"
				currentState = 547;
				break;
			}
			case 547: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(548);
				goto case 178;
			}
			case 548: {
				PopContext();
				goto case 549;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (la.kind == 37) {
					currentState = 588;
					break;
				} else {
					goto case 550;
				}
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 63) {
					currentState = 586;
					break;
				} else {
					goto case 551;
				}
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 136) {
					currentState = 581;
					break;
				} else {
					goto case 552;
				}
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (la.kind == 20) {
					currentState = 580;
					break;
				} else {
					goto case 553;
				}
			}
			case 553: {
				stateStack.Push(554);
				goto case 23;
			}
			case 554: {
				PopContext();
				goto case 555;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				if (la.kind == 40) {
					stateStack.Push(555);
					goto case 389;
				} else {
					goto case 556;
				}
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (set[143].Get(la.kind)) {
					currentState = 579;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 557;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 128) {
					currentState = 558;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 558;
						break;
					} else {
						Error(la);
						goto case 558;
					}
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 37) {
					currentState = 576;
					break;
				} else {
					goto case 559;
				}
			}
			case 559: {
				stateStack.Push(560);
				goto case 233;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(113, la); // "End"
				currentState = 561;
				break;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				if (la.kind == 128) {
					currentState = 562;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 562;
						break;
					} else {
						Error(la);
						goto case 562;
					}
				}
			}
			case 562: {
				stateStack.Push(563);
				goto case 23;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (set[104].Get(la.kind)) {
					goto case 566;
				} else {
					goto case 564;
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(113, la); // "End"
				currentState = 565;
				break;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				if (la.kind == 40) {
					stateStack.Push(566);
					goto case 389;
				} else {
					goto case 567;
				}
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				if (set[143].Get(la.kind)) {
					currentState = 567;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 568;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 568;
							break;
						} else {
							Error(la);
							goto case 568;
						}
					}
				}
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 37) {
					currentState = 573;
					break;
				} else {
					goto case 569;
				}
			}
			case 569: {
				stateStack.Push(570);
				goto case 233;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(113, la); // "End"
				currentState = 571;
				break;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 128) {
					currentState = 572;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 572;
						break;
					} else {
						Error(la);
						goto case 572;
					}
				}
			}
			case 572: {
				stateStack.Push(564);
				goto case 23;
			}
			case 573: {
				SetIdentifierExpected(la);
				goto case 574;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(575);
					goto case 376;
				} else {
					goto case 575;
				}
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				Expect(38, la); // ")"
				currentState = 569;
				break;
			}
			case 576: {
				SetIdentifierExpected(la);
				goto case 577;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(578);
					goto case 376;
				} else {
					goto case 578;
				}
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				Expect(38, la); // ")"
				currentState = 559;
				break;
			}
			case 579: {
				SetIdentifierExpected(la);
				goto case 556;
			}
			case 580: {
				stateStack.Push(553);
				goto case 56;
			}
			case 581: {
				PushContext(Context.Type, la, t);
				stateStack.Push(582);
				goto case 37;
			}
			case 582: {
				PopContext();
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 22) {
					currentState = 584;
					break;
				} else {
					goto case 552;
				}
			}
			case 584: {
				PushContext(Context.Type, la, t);
				stateStack.Push(585);
				goto case 37;
			}
			case 585: {
				PopContext();
				goto case 583;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (la.kind == 40) {
					stateStack.Push(586);
					goto case 389;
				} else {
					if (la.kind == 162) {
						stateStack.Push(551);
						goto case 67;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(587);
							goto case 37;
						} else {
							Error(la);
							goto case 551;
						}
					}
				}
			}
			case 587: {
				PopContext();
				goto case 551;
			}
			case 588: {
				SetIdentifierExpected(la);
				goto case 589;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(590);
					goto case 376;
				} else {
					goto case 590;
				}
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				Expect(38, la); // ")"
				currentState = 550;
				break;
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				Expect(98, la); // "Custom"
				currentState = 592;
				break;
			}
			case 592: {
				stateStack.Push(593);
				goto case 603;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				if (set[109].Get(la.kind)) {
					goto case 595;
				} else {
					Expect(113, la); // "End"
					currentState = 594;
					break;
				}
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (la.kind == 40) {
					stateStack.Push(595);
					goto case 389;
				} else {
					if (la.kind == 56) {
						currentState = 596;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 596;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 596;
								break;
							} else {
								Error(la);
								goto case 596;
							}
						}
					}
				}
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				Expect(37, la); // "("
				currentState = 597;
				break;
			}
			case 597: {
				stateStack.Push(598);
				goto case 376;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				Expect(38, la); // ")"
				currentState = 599;
				break;
			}
			case 599: {
				stateStack.Push(600);
				goto case 233;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				Expect(113, la); // "End"
				currentState = 601;
				break;
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				if (la.kind == 56) {
					currentState = 602;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 602;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 602;
							break;
						} else {
							Error(la);
							goto case 602;
						}
					}
				}
			}
			case 602: {
				stateStack.Push(593);
				goto case 23;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(119, la); // "Event"
				currentState = 604;
				break;
			}
			case 604: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(605);
				goto case 178;
			}
			case 605: {
				PopContext();
				goto case 606;
			}
			case 606: {
				if (la == null) { currentState = 606; break; }
				if (la.kind == 63) {
					currentState = 616;
					break;
				} else {
					if (set[144].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 613;
							break;
						} else {
							goto case 607;
						}
					} else {
						Error(la);
						goto case 607;
					}
				}
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 136) {
					currentState = 608;
					break;
				} else {
					goto case 23;
				}
			}
			case 608: {
				PushContext(Context.Type, la, t);
				stateStack.Push(609);
				goto case 37;
			}
			case 609: {
				PopContext();
				goto case 610;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				if (la.kind == 22) {
					currentState = 611;
					break;
				} else {
					goto case 23;
				}
			}
			case 611: {
				PushContext(Context.Type, la, t);
				stateStack.Push(612);
				goto case 37;
			}
			case 612: {
				PopContext();
				goto case 610;
			}
			case 613: {
				SetIdentifierExpected(la);
				goto case 614;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(615);
					goto case 376;
				} else {
					goto case 615;
				}
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				Expect(38, la); // ")"
				currentState = 607;
				break;
			}
			case 616: {
				PushContext(Context.Type, la, t);
				stateStack.Push(617);
				goto case 37;
			}
			case 617: {
				PopContext();
				goto case 607;
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(101, la); // "Declare"
				currentState = 619;
				break;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 620;
					break;
				} else {
					goto case 620;
				}
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (la.kind == 210) {
					currentState = 621;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 621;
						break;
					} else {
						Error(la);
						goto case 621;
					}
				}
			}
			case 621: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(622);
				goto case 178;
			}
			case 622: {
				PopContext();
				goto case 623;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				Expect(149, la); // "Lib"
				currentState = 624;
				break;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(3, la); // LiteralString
				currentState = 625;
				break;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 59) {
					currentState = 633;
					break;
				} else {
					goto case 626;
				}
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 37) {
					currentState = 630;
					break;
				} else {
					goto case 627;
				}
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (la.kind == 63) {
					currentState = 628;
					break;
				} else {
					goto case 23;
				}
			}
			case 628: {
				PushContext(Context.Type, la, t);
				stateStack.Push(629);
				goto case 37;
			}
			case 629: {
				PopContext();
				goto case 23;
			}
			case 630: {
				SetIdentifierExpected(la);
				goto case 631;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(632);
					goto case 376;
				} else {
					goto case 632;
				}
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				Expect(38, la); // ")"
				currentState = 627;
				break;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				Expect(3, la); // LiteralString
				currentState = 626;
				break;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (la.kind == 210) {
					currentState = 635;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 635;
						break;
					} else {
						Error(la);
						goto case 635;
					}
				}
			}
			case 635: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 636;
			}
			case 636: {
				if (la == null) { currentState = 636; break; }
				currentState = 637;
				break;
			}
			case 637: {
				PopContext();
				goto case 638;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 37) {
					currentState = 644;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 642;
						break;
					} else {
						goto case 639;
					}
				}
			}
			case 639: {
				stateStack.Push(640);
				goto case 233;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				Expect(113, la); // "End"
				currentState = 641;
				break;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 473;
					}
				}
			}
			case 642: {
				PushContext(Context.Type, la, t);
				stateStack.Push(643);
				goto case 37;
			}
			case 643: {
				PopContext();
				goto case 639;
			}
			case 644: {
				SetIdentifierExpected(la);
				goto case 645;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 647;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(646);
							goto case 376;
						} else {
							Error(la);
							goto case 646;
						}
					}
				} else {
					goto case 646;
				}
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				Expect(38, la); // ")"
				currentState = 638;
				break;
			}
			case 647: {
				stateStack.Push(646);
				goto case 440;
			}
			case 648: {
				stateStack.Push(649);
				SetIdentifierExpected(la);
				goto case 650;
			}
			case 649: {
				if (la == null) { currentState = 649; break; }
				if (la.kind == 22) {
					currentState = 648;
					break;
				} else {
					goto case 23;
				}
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (la.kind == 88) {
					currentState = 651;
					break;
				} else {
					goto case 651;
				}
			}
			case 651: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(652);
				goto case 658;
			}
			case 652: {
				PopContext();
				goto case 653;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (la.kind == 63) {
					currentState = 655;
					break;
				} else {
					goto case 654;
				}
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				if (la.kind == 20) {
					goto case 191;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 655: {
				PushContext(Context.Type, la, t);
				goto case 656;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (la.kind == 162) {
					stateStack.Push(657);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(657);
						goto case 37;
					} else {
						Error(la);
						goto case 657;
					}
				}
			}
			case 657: {
				PopContext();
				goto case 654;
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				if (set[128].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 124;
					} else {
						if (la.kind == 126) {
							goto case 108;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 659: {
				isMissingModifier = false;
				goto case 527;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(136, la); // "Implements"
				currentState = 661;
				break;
			}
			case 661: {
				PushContext(Context.Type, la, t);
				stateStack.Push(662);
				goto case 37;
			}
			case 662: {
				PopContext();
				goto case 663;
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				if (la.kind == 22) {
					currentState = 664;
					break;
				} else {
					stateStack.Push(519);
					goto case 23;
				}
			}
			case 664: {
				PushContext(Context.Type, la, t);
				stateStack.Push(665);
				goto case 37;
			}
			case 665: {
				PopContext();
				goto case 663;
			}
			case 666: {
				if (la == null) { currentState = 666; break; }
				Expect(140, la); // "Inherits"
				currentState = 667;
				break;
			}
			case 667: {
				PushContext(Context.Type, la, t);
				stateStack.Push(668);
				goto case 37;
			}
			case 668: {
				PopContext();
				stateStack.Push(517);
				goto case 23;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				Expect(169, la); // "Of"
				currentState = 670;
				break;
			}
			case 670: {
				stateStack.Push(671);
				goto case 440;
			}
			case 671: {
				if (la == null) { currentState = 671; break; }
				Expect(38, la); // ")"
				currentState = 514;
				break;
			}
			case 672: {
				isMissingModifier = false;
				goto case 28;
			}
			case 673: {
				PushContext(Context.Type, la, t);
				stateStack.Push(674);
				goto case 37;
			}
			case 674: {
				PopContext();
				goto case 675;
			}
			case 675: {
				if (la == null) { currentState = 675; break; }
				if (la.kind == 22) {
					currentState = 676;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 676: {
				PushContext(Context.Type, la, t);
				stateStack.Push(677);
				goto case 37;
			}
			case 677: {
				PopContext();
				goto case 675;
			}
			case 678: {
				if (la == null) { currentState = 678; break; }
				Expect(169, la); // "Of"
				currentState = 679;
				break;
			}
			case 679: {
				stateStack.Push(680);
				goto case 440;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 681: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 682;
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				if (set[46].Get(la.kind)) {
					currentState = 682;
					break;
				} else {
					PopContext();
					stateStack.Push(683);
					goto case 23;
				}
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(683);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 684;
					break;
				}
			}
			case 684: {
				if (la == null) { currentState = 684; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				Expect(137, la); // "Imports"
				currentState = 686;
				break;
			}
			case 686: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 687;
			}
			case 687: {
				if (la == null) { currentState = 687; break; }
				if (set[145].Get(la.kind)) {
					currentState = 693;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 689;
						break;
					} else {
						Error(la);
						goto case 688;
					}
				}
			}
			case 688: {
				PopContext();
				goto case 23;
			}
			case 689: {
				stateStack.Push(690);
				goto case 178;
			}
			case 690: {
				if (la == null) { currentState = 690; break; }
				Expect(20, la); // "="
				currentState = 691;
				break;
			}
			case 691: {
				if (la == null) { currentState = 691; break; }
				Expect(3, la); // LiteralString
				currentState = 692;
				break;
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 688;
				break;
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				if (la.kind == 37) {
					stateStack.Push(693);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 694;
						break;
					} else {
						goto case 688;
					}
				}
			}
			case 694: {
				stateStack.Push(688);
				goto case 37;
			}
			case 695: {
				if (la == null) { currentState = 695; break; }
				Expect(173, la); // "Option"
				currentState = 696;
				break;
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 698;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 697;
						break;
					} else {
						goto case 473;
					}
				}
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 473;
					}
				}
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
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