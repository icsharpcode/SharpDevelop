using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 238;
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
			case 239:
			case 474:
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
			case 167:
			case 173:
			case 179:
			case 216:
			case 220:
			case 259:
			case 359:
			case 368:
			case 421:
			case 461:
			case 471:
			case 482:
			case 512:
			case 548:
			case 605:
			case 622:
			case 690:
				return set[6];
			case 12:
			case 13:
			case 513:
			case 514:
			case 559:
			case 569:
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
			case 231:
			case 234:
			case 235:
			case 245:
			case 260:
			case 264:
			case 286:
			case 301:
			case 312:
			case 315:
			case 321:
			case 326:
			case 335:
			case 336:
			case 356:
			case 376:
			case 467:
			case 479:
			case 485:
			case 489:
			case 497:
			case 505:
			case 515:
			case 524:
			case 541:
			case 546:
			case 554:
			case 560:
			case 563:
			case 570:
			case 573:
			case 600:
			case 603:
			case 630:
			case 640:
			case 644:
			case 669:
			case 689:
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
			case 232:
			case 246:
			case 262:
			case 316:
			case 357:
			case 401:
			case 522:
			case 542:
			case 561:
			case 565:
			case 571:
			case 601:
			case 641:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 490:
			case 525:
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
			case 673:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 127:
			case 189:
			case 190:
			case 240:
			case 391:
			case 392:
			case 409:
			case 410:
			case 411:
			case 412:
			case 500:
			case 501:
			case 534:
			case 535:
			case 636:
			case 637:
			case 682:
			case 683:
				return set[14];
			case 33:
			case 34:
			case 462:
			case 463:
			case 472:
			case 473:
			case 502:
			case 503:
			case 627:
			case 638:
			case 639:
				return set[15];
			case 35:
			case 37:
			case 131:
			case 142:
			case 145:
			case 161:
			case 177:
			case 193:
			case 271:
			case 296:
			case 375:
			case 388:
			case 424:
			case 478:
			case 496:
			case 504:
			case 582:
			case 585:
			case 609:
			case 612:
			case 617:
			case 629:
			case 643:
			case 662:
			case 665:
			case 668:
			case 674:
			case 677:
			case 695:
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
			case 351:
			case 428:
				return set[19];
			case 42:
			case 151:
			case 158:
			case 163:
			case 225:
			case 395:
			case 420:
			case 423:
			case 536:
			case 537:
			case 597:
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
			case 162:
			case 228:
			case 373:
			case 398:
			case 422:
			case 425:
			case 439:
			case 470:
			case 477:
			case 508:
			case 539:
			case 576:
			case 579:
			case 591:
			case 599:
			case 616:
			case 633:
			case 647:
			case 672:
			case 681:
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
			case 433:
			case 434:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 153:
			case 160:
			case 354:
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
			case 372:
			case 374:
			case 378:
			case 386:
			case 432:
			case 436:
			case 446:
			case 453:
			case 460:
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
			case 169:
			case 171:
			case 211:
			case 244:
			case 248:
			case 250:
			case 251:
			case 268:
			case 285:
			case 290:
			case 299:
			case 305:
			case 307:
			case 311:
			case 314:
			case 320:
			case 331:
			case 333:
			case 339:
			case 353:
			case 355:
			case 387:
			case 414:
			case 430:
			case 431:
			case 495:
			case 581:
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
			case 456:
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
			case 192:
			case 194:
			case 195:
			case 298:
			case 691:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 83:
			case 317:
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
			case 263:
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
			case 402:
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
			case 323:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 96:
			case 547:
			case 566:
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
			case 280:
			case 287:
			case 302:
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
			case 198:
			case 203:
			case 205:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 104:
			case 200:
			case 204:
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
			case 233:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 108:
			case 223:
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
			case 170:
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
			case 592:
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
			case 182:
			case 210:
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
			case 222:
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
			case 426:
			case 427:
				return set[31];
			case 138:
				return set[32];
			case 147:
			case 148:
			case 283:
			case 292:
				return set[33];
			case 149:
			case 404:
				return set[34];
			case 150:
			case 338:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 157:
				return set[35];
			case 164:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 165:
			case 166:
				return set[36];
			case 168:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 172:
			case 186:
			case 202:
			case 207:
			case 213:
			case 215:
			case 219:
			case 221:
				return set[37];
			case 174:
			case 175:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 176:
			case 178:
			case 284:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 180:
			case 181:
			case 183:
			case 185:
			case 187:
			case 188:
			case 196:
			case 201:
			case 206:
			case 214:
			case 218:
				return set[38];
			case 184:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 191:
				return set[39];
			case 197:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 208:
			case 209:
				return set[40];
			case 212:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 217:
				return set[41];
			case 224:
			case 499:
			case 621:
			case 635:
			case 642:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 226:
			case 227:
			case 396:
			case 397:
			case 468:
			case 469:
			case 475:
			case 476:
			case 574:
			case 575:
			case 577:
			case 578:
			case 589:
			case 590:
			case 614:
			case 615:
			case 631:
			case 632:
				return set[42];
			case 229:
			case 230:
				return set[43];
			case 236:
			case 237:
				return set[44];
			case 238:
				return set[45];
			case 241:
				return set[46];
			case 242:
			case 243:
			case 344:
				return set[47];
			case 247:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 249:
			case 291:
			case 306:
				return set[48];
			case 252:
			case 253:
			case 273:
			case 274:
			case 288:
			case 289:
			case 303:
			case 304:
				return set[49];
			case 254:
			case 345:
			case 348:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 255:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 256:
				return set[50];
			case 257:
			case 276:
				return set[51];
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 261:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 265:
			case 266:
				return set[52];
			case 267:
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 269:
			case 270:
				return set[53];
			case 275:
				return set[54];
			case 277:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 278:
			case 279:
				return set[55];
			case 281:
			case 282:
				return set[56];
			case 293:
			case 294:
				return set[57];
			case 295:
				return set[58];
			case 297:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 300:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 308:
				return set[59];
			case 309:
			case 313:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 310:
				return set[60];
			case 318:
			case 319:
				return set[61];
			case 322:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 324:
			case 325:
				return set[62];
			case 327:
			case 328:
				return set[63];
			case 329:
			case 610:
			case 611:
			case 613:
			case 650:
			case 663:
			case 664:
			case 666:
			case 675:
			case 676:
			case 678:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 330:
			case 332:
				return set[64];
			case 334:
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 337:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 341:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 342:
			case 343:
			case 399:
			case 400:
				return set[65];
			case 346:
			case 347:
			case 349:
			case 350:
				return set[66];
			case 352:
				return set[67];
			case 358:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 360:
			case 361:
			case 369:
			case 370:
				return set[68];
			case 362:
			case 371:
				return set[69];
			case 363:
				return set[70];
			case 364:
			case 367:
				return set[71];
			case 365:
			case 366:
			case 656:
			case 657:
				return set[72];
			case 377:
			case 379:
			case 380:
			case 538:
			case 598:
				return set[73];
			case 381:
			case 382:
				return set[74];
			case 383:
			case 384:
				return set[75];
			case 385:
			case 389:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 390:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 393:
			case 394:
				return set[76];
			case 403:
				return set[77];
			case 405:
			case 418:
				return set[78];
			case 406:
			case 419:
				return set[79];
			case 407:
			case 408:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 413:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 415:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 416:
				return set[80];
			case 417:
				return set[81];
			case 429:
				return set[82];
			case 435:
				return set[83];
			case 437:
			case 438:
			case 506:
			case 507:
			case 645:
			case 646:
				return set[84];
			case 440:
			case 441:
			case 442:
			case 447:
			case 448:
			case 509:
			case 648:
			case 671:
			case 680:
				return set[85];
			case 443:
			case 449:
			case 458:
				return set[86];
			case 444:
			case 445:
			case 450:
			case 451:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 452:
			case 454:
			case 459:
				return set[87];
			case 455:
			case 457:
				return set[88];
			case 464:
			case 483:
			case 484:
			case 540:
			case 628:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 465:
			case 466:
			case 544:
			case 545:
				return set[89];
			case 480:
			case 481:
			case 488:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 486:
			case 487:
				return set[90];
			case 491:
			case 492:
				return set[91];
			case 493:
			case 494:
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 498:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 510:
			case 511:
			case 523:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 516:
			case 517:
				return set[92];
			case 518:
			case 519:
				return set[93];
			case 520:
			case 521:
			case 532:
				return set[94];
			case 526:
			case 527:
				return set[95];
			case 528:
			case 529:
			case 660:
				return set[96];
			case 530:
				return set[97];
			case 531:
				return set[98];
			case 533:
			case 543:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 549:
			case 550:
				return set[99];
			case 551:
				return set[100];
			case 552:
			case 588:
				return set[101];
			case 555:
			case 556:
			case 557:
			case 580:
				return set[102];
			case 558:
			case 562:
			case 572:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 564:
				return set[103];
			case 567:
				return set[104];
			case 568:
				return set[105];
			case 583:
			case 584:
			case 586:
			case 655:
			case 658:
				return set[106];
			case 587:
				return set[107];
			case 593:
			case 595:
			case 604:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 594:
				return set[108];
			case 596:
				return set[109];
			case 602:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 606:
			case 607:
				return set[110];
			case 608:
			case 618:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 619:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 620:
				return set[111];
			case 623:
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 625:
			case 634:
			case 692:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 626:
				return set[112];
			case 649:
			case 651:
				return set[113];
			case 652:
			case 659:
				return set[114];
			case 653:
			case 654:
				return set[115];
			case 661:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 667:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 670:
			case 679:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 684:
				return set[116];
			case 685:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 686:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 687:
			case 688:
				return set[117];
			case 693:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 694:
				return set[118];
			case 696:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 697:
				return set[119];
			case 698:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 699:
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
					goto case 696;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 686;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 390;
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
					currentState = 682;
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
					goto case 390;
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
						goto case 510;
					} else {
						if (la.kind == 103) {
							currentState = 499;
							break;
						} else {
							if (la.kind == 115) {
								goto case 480;
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
				goto case 179;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 679;
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
					currentState = 674;
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
					goto case 390;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[122].Get(la.kind)) {
					currentState = 673;
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
					goto case 510;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 498;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 480;
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
					currentState = 471;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 461;
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
					currentState = 437;
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
					currentState = 435;
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
					goto case 431;
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
						currentState = 430;
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
									currentState = 426;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 423;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 420;
											break;
										} else {
											if (set[77].Get(la.kind)) {
												stateStack.Push(149);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 403;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(149);
													goto case 224;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(149);
														PushContext(Context.Query, la, t);
														goto case 164;
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
					currentState = 163;
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
				PushContext(Context.Type, la, t);
				stateStack.Push(162);
				goto case 37;
			}
			case 162: {
				PopContext();
				goto case 45;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(37, la); // "("
				currentState = 156;
				break;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 126) {
					stateStack.Push(165);
					goto case 223;
				} else {
					if (la.kind == 58) {
						stateStack.Push(165);
						goto case 222;
					} else {
						Error(la);
						goto case 165;
					}
				}
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(165);
					goto case 166;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 126) {
					currentState = 220;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 216;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 214;
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
										currentState = 210;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 208;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 206;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 180;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 167;
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
			case 167: {
				stateStack.Push(168);
				goto case 173;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(171, la); // "On"
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 56;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				Expect(116, la); // "Equals"
				currentState = 171;
				break;
			}
			case 171: {
				stateStack.Push(172);
				goto case 56;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 22) {
					currentState = 169;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 173: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(174);
				goto case 179;
			}
			case 174: {
				PopContext();
				goto case 175;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 63) {
					currentState = 177;
					break;
				} else {
					goto case 176;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 177: {
				PushContext(Context.Type, la, t);
				stateStack.Push(178);
				goto case 37;
			}
			case 178: {
				PopContext();
				goto case 176;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
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
			case 180: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 181;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (la.kind == 146) {
					goto case 198;
				} else {
					if (set[38].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 183;
							break;
						} else {
							if (set[38].Get(la.kind)) {
								goto case 196;
							} else {
								Error(la);
								goto case 182;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				Expect(70, la); // "By"
				currentState = 183;
				break;
			}
			case 183: {
				stateStack.Push(184);
				goto case 187;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (la.kind == 22) {
					currentState = 183;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 185;
					break;
				}
			}
			case 185: {
				stateStack.Push(186);
				goto case 187;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.kind == 22) {
					currentState = 185;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 187: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 188;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(189);
					goto case 179;
				} else {
					goto case 56;
				}
			}
			case 189: {
				PopContext();
				goto case 190;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 63) {
					currentState = 193;
					break;
				} else {
					if (la.kind == 20) {
						goto case 192;
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
			case 191: {
				if (la == null) { currentState = 191; break; }
				currentState = 56;
				break;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				currentState = 56;
				break;
			}
			case 193: {
				PushContext(Context.Type, la, t);
				stateStack.Push(194);
				goto case 37;
			}
			case 194: {
				PopContext();
				goto case 195;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 196: {
				stateStack.Push(197);
				goto case 187;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 22) {
					currentState = 196;
					break;
				} else {
					goto case 182;
				}
			}
			case 198: {
				stateStack.Push(199);
				goto case 205;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 203;
						break;
					} else {
						if (la.kind == 146) {
							goto case 198;
						} else {
							Error(la);
							goto case 199;
						}
					}
				} else {
					goto case 200;
				}
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				Expect(143, la); // "Into"
				currentState = 201;
				break;
			}
			case 201: {
				stateStack.Push(202);
				goto case 187;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 22) {
					currentState = 201;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: {
				stateStack.Push(204);
				goto case 205;
			}
			case 204: {
				stateStack.Push(199);
				goto case 200;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				Expect(146, la); // "Join"
				currentState = 167;
				break;
			}
			case 206: {
				stateStack.Push(207);
				goto case 187;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (la.kind == 22) {
					currentState = 206;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 208: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 209;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(70, la); // "By"
				currentState = 211;
				break;
			}
			case 211: {
				stateStack.Push(212);
				goto case 56;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 64) {
					currentState = 213;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 213;
						break;
					} else {
						Error(la);
						goto case 213;
					}
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 22) {
					currentState = 211;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 214: {
				stateStack.Push(215);
				goto case 187;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 22) {
					currentState = 214;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 216: {
				stateStack.Push(217);
				goto case 173;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(217);
					goto case 166;
				} else {
					Expect(143, la); // "Into"
					currentState = 218;
					break;
				}
			}
			case 218: {
				stateStack.Push(219);
				goto case 187;
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
				stateStack.Push(221);
				goto case 173;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 22) {
					currentState = 220;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(58, la); // "Aggregate"
				currentState = 216;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(126, la); // "From"
				currentState = 220;
				break;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (la.kind == 210) {
					currentState = 395;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 225;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				Expect(37, la); // "("
				currentState = 226;
				break;
			}
			case 226: {
				SetIdentifierExpected(la);
				goto case 227;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(228);
					goto case 377;
				} else {
					goto case 228;
				}
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(38, la); // ")"
				currentState = 229;
				break;
			}
			case 229: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 230;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 375;
							break;
						} else {
							goto case 231;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 231: {
				stateStack.Push(232);
				goto case 234;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				Expect(113, la); // "End"
				currentState = 233;
				break;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				PushContext(Context.Body, la, t);
				goto case 235;
			}
			case 235: {
				stateStack.Push(236);
				goto case 23;
			}
			case 236: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (set[130].Get(la.kind)) {
					if (set[65].Get(la.kind)) {
						if (set[47].Get(la.kind)) {
							stateStack.Push(235);
							goto case 242;
						} else {
							goto case 235;
						}
					} else {
						if (la.kind == 113) {
							currentState = 240;
							break;
						} else {
							goto case 239;
						}
					}
				} else {
					goto case 238;
				}
			}
			case 238: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 239: {
				Error(la);
				goto case 236;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 235;
				} else {
					if (set[46].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 239;
					}
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				currentState = 236;
				break;
			}
			case 242: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 359;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 355;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 353;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 351;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 333;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 318;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 314;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 308;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 281;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 277;
																break;
															} else {
																goto case 277;
															}
														} else {
															if (la.kind == 194) {
																currentState = 275;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 273;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 260;
																break;
															} else {
																if (set[131].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 257;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 256;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 255;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 93;
																				} else {
																					if (la.kind == 195) {
																						currentState = 252;
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
																		currentState = 250;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 248;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 244;
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
			case 244: {
				stateStack.Push(245);
				goto case 56;
			}
			case 245: {
				stateStack.Push(246);
				goto case 234;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				Expect(113, la); // "End"
				currentState = 247;
				break;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 248: {
				stateStack.Push(249);
				goto case 56;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 22) {
					currentState = 248;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 250: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 251;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 252: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 253;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(254);
					goto case 56;
				} else {
					goto case 254;
				}
			}
			case 254: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
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
							goto case 6;
						}
					}
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
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
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (set[6].Get(la.kind)) {
					goto case 259;
				} else {
					if (la.kind == 5) {
						goto case 258;
					} else {
						goto case 6;
					}
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				stateStack.Push(261);
				goto case 234;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (la.kind == 75) {
					currentState = 265;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 264;
						break;
					} else {
						goto case 262;
					}
				}
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				Expect(113, la); // "End"
				currentState = 263;
				break;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 264: {
				stateStack.Push(262);
				goto case 234;
			}
			case 265: {
				SetIdentifierExpected(la);
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(269);
					goto case 179;
				} else {
					goto case 267;
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 229) {
					currentState = 268;
					break;
				} else {
					goto case 260;
				}
			}
			case 268: {
				stateStack.Push(260);
				goto case 56;
			}
			case 269: {
				PopContext();
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 63) {
					currentState = 271;
					break;
				} else {
					goto case 267;
				}
			}
			case 271: {
				PushContext(Context.Type, la, t);
				stateStack.Push(272);
				goto case 37;
			}
			case 272: {
				PopContext();
				goto case 267;
			}
			case 273: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (la.kind == 163) {
					goto case 100;
				} else {
					goto case 276;
				}
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (la.kind == 5) {
					goto case 258;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 259;
					} else {
						goto case 6;
					}
				}
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				Expect(118, la); // "Error"
				currentState = 278;
				break;
			}
			case 278: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 279;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 276;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 280;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 282;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (set[33].Get(la.kind)) {
					stateStack.Push(298);
					goto case 292;
				} else {
					if (la.kind == 110) {
						currentState = 283;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 283: {
				stateStack.Push(284);
				goto case 292;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				Expect(138, la); // "In"
				currentState = 285;
				break;
			}
			case 285: {
				stateStack.Push(286);
				goto case 56;
			}
			case 286: {
				stateStack.Push(287);
				goto case 234;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				Expect(163, la); // "Next"
				currentState = 288;
				break;
			}
			case 288: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 289;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (set[23].Get(la.kind)) {
					goto case 290;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 290: {
				stateStack.Push(291);
				goto case 56;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 22) {
					currentState = 290;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 292: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(293);
				goto case 147;
			}
			case 293: {
				PopContext();
				goto case 294;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 33) {
					currentState = 295;
					break;
				} else {
					goto case 295;
				}
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(295);
					goto case 134;
				} else {
					if (la.kind == 63) {
						currentState = 296;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 296: {
				PushContext(Context.Type, la, t);
				stateStack.Push(297);
				goto case 37;
			}
			case 297: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				Expect(20, la); // "="
				currentState = 299;
				break;
			}
			case 299: {
				stateStack.Push(300);
				goto case 56;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 205) {
					currentState = 307;
					break;
				} else {
					goto case 301;
				}
			}
			case 301: {
				stateStack.Push(302);
				goto case 234;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				Expect(163, la); // "Next"
				currentState = 303;
				break;
			}
			case 303: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 304;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (set[23].Get(la.kind)) {
					goto case 305;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 305: {
				stateStack.Push(306);
				goto case 56;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (la.kind == 22) {
					currentState = 305;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 307: {
				stateStack.Push(301);
				goto case 56;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 311;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(309);
						goto case 234;
					} else {
						goto case 6;
					}
				}
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(152, la); // "Loop"
				currentState = 310;
				break;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
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
				stateStack.Push(313);
				goto case 234;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 314: {
				stateStack.Push(315);
				goto case 56;
			}
			case 315: {
				stateStack.Push(316);
				goto case 234;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(113, la); // "End"
				currentState = 317;
				break;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 318: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 319;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 74) {
					currentState = 320;
					break;
				} else {
					goto case 320;
				}
			}
			case 320: {
				stateStack.Push(321);
				goto case 56;
			}
			case 321: {
				stateStack.Push(322);
				goto case 23;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 74) {
					currentState = 324;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 323;
					break;
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 324: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 325;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 111) {
					currentState = 326;
					break;
				} else {
					if (set[63].Get(la.kind)) {
						goto case 327;
					} else {
						Error(la);
						goto case 326;
					}
				}
			}
			case 326: {
				stateStack.Push(322);
				goto case 234;
			}
			case 327: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 328;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 330;
						break;
					} else {
						goto case 330;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(329);
						goto case 56;
					} else {
						Error(la);
						goto case 329;
					}
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 22) {
					currentState = 327;
					break;
				} else {
					goto case 326;
				}
			}
			case 330: {
				stateStack.Push(331);
				goto case 332;
			}
			case 331: {
				stateStack.Push(329);
				goto case 59;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
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
			case 333: {
				stateStack.Push(334);
				goto case 56;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (la.kind == 214) {
					currentState = 342;
					break;
				} else {
					goto case 335;
				}
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 336;
				} else {
					goto case 6;
				}
			}
			case 336: {
				stateStack.Push(337);
				goto case 234;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 341;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 339;
							break;
						} else {
							Error(la);
							goto case 336;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 338;
					break;
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 339: {
				stateStack.Push(340);
				goto case 56;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 214) {
					currentState = 336;
					break;
				} else {
					goto case 336;
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 135) {
					currentState = 339;
					break;
				} else {
					goto case 336;
				}
			}
			case 342: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 343;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (set[47].Get(la.kind)) {
					goto case 344;
				} else {
					goto case 335;
				}
			}
			case 344: {
				stateStack.Push(345);
				goto case 242;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (la.kind == 21) {
					currentState = 349;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 346;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 346: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 347;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (set[47].Get(la.kind)) {
					stateStack.Push(348);
					goto case 242;
				} else {
					goto case 348;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 21) {
					currentState = 346;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 349: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 350;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (set[47].Get(la.kind)) {
					goto case 344;
				} else {
					goto case 345;
				}
			}
			case 351: {
				stateStack.Push(352);
				goto case 81;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 353: {
				stateStack.Push(354);
				goto case 56;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 355: {
				stateStack.Push(356);
				goto case 56;
			}
			case 356: {
				stateStack.Push(357);
				goto case 234;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(113, la); // "End"
				currentState = 358;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
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
			case 359: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(360);
				goto case 179;
			}
			case 360: {
				PopContext();
				goto case 361;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 33) {
					currentState = 362;
					break;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 37) {
					currentState = 374;
					break;
				} else {
					goto case 363;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 22) {
					currentState = 368;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 365;
						break;
					} else {
						goto case 364;
					}
				}
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 20) {
					goto case 192;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 365: {
				PushContext(Context.Type, la, t);
				goto case 366;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 162) {
					stateStack.Push(367);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(367);
						goto case 37;
					} else {
						Error(la);
						goto case 367;
					}
				}
			}
			case 367: {
				PopContext();
				goto case 364;
			}
			case 368: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(369);
				goto case 179;
			}
			case 369: {
				PopContext();
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 33) {
					currentState = 371;
					break;
				} else {
					goto case 371;
				}
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 37) {
					currentState = 372;
					break;
				} else {
					goto case 363;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 22) {
					currentState = 372;
					break;
				} else {
					goto case 373;
				}
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				Expect(38, la); // ")"
				currentState = 363;
				break;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 22) {
					currentState = 374;
					break;
				} else {
					goto case 373;
				}
			}
			case 375: {
				PushContext(Context.Type, la, t);
				stateStack.Push(376);
				goto case 37;
			}
			case 376: {
				PopContext();
				goto case 231;
			}
			case 377: {
				stateStack.Push(378);
				PushContext(Context.Parameter, la, t);
				goto case 379;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 22) {
					currentState = 377;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 379: {
				SetIdentifierExpected(la);
				goto case 380;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 40) {
					stateStack.Push(379);
					goto case 390;
				} else {
					goto case 381;
				}
			}
			case 381: {
				SetIdentifierExpected(la);
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[134].Get(la.kind)) {
					currentState = 381;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(383);
					goto case 179;
				}
			}
			case 383: {
				PopContext();
				goto case 384;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 63) {
					currentState = 388;
					break;
				} else {
					goto case 385;
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 20) {
					currentState = 387;
					break;
				} else {
					goto case 386;
				}
			}
			case 386: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 387: {
				stateStack.Push(386);
				goto case 56;
			}
			case 388: {
				PushContext(Context.Type, la, t);
				stateStack.Push(389);
				goto case 37;
			}
			case 389: {
				PopContext();
				goto case 385;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(40, la); // "<"
				currentState = 391;
				break;
			}
			case 391: {
				PushContext(Context.Attribute, la, t);
				goto case 392;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (set[135].Get(la.kind)) {
					currentState = 392;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 393;
					break;
				}
			}
			case 393: {
				PopContext();
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				Expect(37, la); // "("
				currentState = 396;
				break;
			}
			case 396: {
				SetIdentifierExpected(la);
				goto case 397;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(398);
					goto case 377;
				} else {
					goto case 398;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(38, la); // ")"
				currentState = 399;
				break;
			}
			case 399: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 400;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (set[47].Get(la.kind)) {
					goto case 242;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(401);
						goto case 234;
					} else {
						goto case 6;
					}
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(113, la); // "End"
				currentState = 402;
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 416;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(405);
						goto case 407;
					} else {
						Error(la);
						goto case 404;
					}
				}
			}
			case 404: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 17) {
					currentState = 406;
					break;
				} else {
					goto case 404;
				}
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 16) {
					currentState = 405;
					break;
				} else {
					goto case 405;
				}
			}
			case 407: {
				PushContext(Context.Xml, la, t);
				goto case 408;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 409;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 409;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(409);
							goto case 413;
						} else {
							Error(la);
							goto case 409;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 410;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 411;
							break;
						} else {
							Error(la);
							goto case 410;
						}
					}
				}
			}
			case 410: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 411;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(411);
							goto case 413;
						} else {
							if (la.kind == 10) {
								stateStack.Push(411);
								goto case 407;
							} else {
								Error(la);
								goto case 411;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 412;
					break;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 412;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(412);
							goto case 413;
						} else {
							Error(la);
							goto case 412;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 410;
					break;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 414;
				break;
			}
			case 414: {
				stateStack.Push(415);
				goto case 56;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 16) {
					currentState = 417;
					break;
				} else {
					goto case 417;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 416;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(418);
						goto case 407;
					} else {
						goto case 404;
					}
				}
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 17) {
					currentState = 419;
					break;
				} else {
					goto case 404;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (la.kind == 16) {
					currentState = 418;
					break;
				} else {
					goto case 418;
				}
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				Expect(37, la); // "("
				currentState = 421;
				break;
			}
			case 421: {
				readXmlIdentifier = true;
				stateStack.Push(422);
				goto case 179;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(38, la); // ")"
				currentState = 149;
				break;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				Expect(37, la); // "("
				currentState = 424;
				break;
			}
			case 424: {
				PushContext(Context.Type, la, t);
				stateStack.Push(425);
				goto case 37;
			}
			case 425: {
				PopContext();
				goto case 422;
			}
			case 426: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 427;
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (la.kind == 10) {
					currentState = 428;
					break;
				} else {
					goto case 428;
				}
			}
			case 428: {
				stateStack.Push(429);
				goto case 81;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (la.kind == 11) {
					currentState = 149;
					break;
				} else {
					goto case 149;
				}
			}
			case 430: {
				stateStack.Push(422);
				goto case 56;
			}
			case 431: {
				stateStack.Push(432);
				goto case 56;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 22) {
					currentState = 433;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 433: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 434;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (set[23].Get(la.kind)) {
					goto case 431;
				} else {
					goto case 432;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(436);
					goto case 37;
				} else {
					goto case 436;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (la.kind == 22) {
					currentState = 435;
					break;
				} else {
					goto case 45;
				}
			}
			case 437: {
				SetIdentifierExpected(la);
				goto case 438;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 440;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(439);
							goto case 377;
						} else {
							Error(la);
							goto case 439;
						}
					}
				} else {
					goto case 439;
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 440: {
				stateStack.Push(439);
				goto case 441;
			}
			case 441: {
				SetIdentifierExpected(la);
				goto case 442;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 443;
					break;
				} else {
					goto case 443;
				}
			}
			case 443: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(444);
				goto case 458;
			}
			case 444: {
				PopContext();
				goto case 445;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 63) {
					currentState = 459;
					break;
				} else {
					goto case 446;
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 22) {
					currentState = 447;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 447: {
				SetIdentifierExpected(la);
				goto case 448;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 449;
					break;
				} else {
					goto case 449;
				}
			}
			case 449: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(450);
				goto case 458;
			}
			case 450: {
				PopContext();
				goto case 451;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 63) {
					currentState = 452;
					break;
				} else {
					goto case 446;
				}
			}
			case 452: {
				PushContext(Context.Type, la, t);
				stateStack.Push(453);
				goto case 454;
			}
			case 453: {
				PopContext();
				goto case 446;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (set[88].Get(la.kind)) {
					goto case 457;
				} else {
					if (la.kind == 35) {
						currentState = 455;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 455: {
				stateStack.Push(456);
				goto case 457;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 22) {
					currentState = 455;
					break;
				} else {
					goto case 66;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
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
			case 458: {
				if (la == null) { currentState = 458; break; }
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
			case 459: {
				PushContext(Context.Type, la, t);
				stateStack.Push(460);
				goto case 454;
			}
			case 460: {
				PopContext();
				goto case 446;
			}
			case 461: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(462);
				goto case 179;
			}
			case 462: {
				PopContext();
				goto case 463;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 37) {
					currentState = 468;
					break;
				} else {
					goto case 464;
				}
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 63) {
					currentState = 465;
					break;
				} else {
					goto case 23;
				}
			}
			case 465: {
				PushContext(Context.Type, la, t);
				goto case 466;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 40) {
					stateStack.Push(466);
					goto case 390;
				} else {
					stateStack.Push(467);
					goto case 37;
				}
			}
			case 467: {
				PopContext();
				goto case 23;
			}
			case 468: {
				SetIdentifierExpected(la);
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(470);
					goto case 377;
				} else {
					goto case 470;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(38, la); // ")"
				currentState = 464;
				break;
			}
			case 471: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(472);
				goto case 179;
			}
			case 472: {
				PopContext();
				goto case 473;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 478;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 475;
							break;
						} else {
							goto case 474;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 474: {
				Error(la);
				goto case 23;
			}
			case 475: {
				SetIdentifierExpected(la);
				goto case 476;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(477);
					goto case 377;
				} else {
					goto case 477;
				}
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 478: {
				PushContext(Context.Type, la, t);
				stateStack.Push(479);
				goto case 37;
			}
			case 479: {
				PopContext();
				goto case 23;
			}
			case 480: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 481;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				Expect(115, la); // "Enum"
				currentState = 482;
				break;
			}
			case 482: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(483);
				goto case 179;
			}
			case 483: {
				PopContext();
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 63) {
					currentState = 496;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				stateStack.Push(486);
				goto case 23;
			}
			case 486: {
				SetIdentifierExpected(la);
				goto case 487;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				if (set[91].Get(la.kind)) {
					goto case 491;
				} else {
					Expect(113, la); // "End"
					currentState = 488;
					break;
				}
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				Expect(115, la); // "Enum"
				currentState = 489;
				break;
			}
			case 489: {
				stateStack.Push(490);
				goto case 23;
			}
			case 490: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 491: {
				SetIdentifierExpected(la);
				goto case 492;
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 40) {
					stateStack.Push(491);
					goto case 390;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(493);
					goto case 179;
				}
			}
			case 493: {
				PopContext();
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 20) {
					currentState = 495;
					break;
				} else {
					goto case 485;
				}
			}
			case 495: {
				stateStack.Push(485);
				goto case 56;
			}
			case 496: {
				PushContext(Context.Type, la, t);
				stateStack.Push(497);
				goto case 37;
			}
			case 497: {
				PopContext();
				goto case 485;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				Expect(103, la); // "Delegate"
				currentState = 499;
				break;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (la.kind == 210) {
					currentState = 500;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 500;
						break;
					} else {
						Error(la);
						goto case 500;
					}
				}
			}
			case 500: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 501;
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				currentState = 502;
				break;
			}
			case 502: {
				PopContext();
				goto case 503;
			}
			case 503: {
				if (la == null) { currentState = 503; break; }
				if (la.kind == 37) {
					currentState = 506;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 504;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 504: {
				PushContext(Context.Type, la, t);
				stateStack.Push(505);
				goto case 37;
			}
			case 505: {
				PopContext();
				goto case 23;
			}
			case 506: {
				SetIdentifierExpected(la);
				goto case 507;
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 509;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(508);
							goto case 377;
						} else {
							Error(la);
							goto case 508;
						}
					}
				} else {
					goto case 508;
				}
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				Expect(38, la); // ")"
				currentState = 503;
				break;
			}
			case 509: {
				stateStack.Push(508);
				goto case 441;
			}
			case 510: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 511;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				if (la.kind == 155) {
					currentState = 512;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 512;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 512;
							break;
						} else {
							Error(la);
							goto case 512;
						}
					}
				}
			}
			case 512: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(513);
				goto case 179;
			}
			case 513: {
				PopContext();
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 37) {
					currentState = 670;
					break;
				} else {
					goto case 515;
				}
			}
			case 515: {
				stateStack.Push(516);
				goto case 23;
			}
			case 516: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 517;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 667;
				} else {
					goto case 518;
				}
			}
			case 518: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 519;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 661;
				} else {
					goto case 520;
				}
			}
			case 520: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 521;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (set[95].Get(la.kind)) {
					goto case 526;
				} else {
					isMissingModifier = false;
					goto case 522;
				}
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				Expect(113, la); // "End"
				currentState = 523;
				break;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				if (la.kind == 155) {
					currentState = 524;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 524;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 524;
							break;
						} else {
							Error(la);
							goto case 524;
						}
					}
				}
			}
			case 524: {
				stateStack.Push(525);
				goto case 23;
			}
			case 525: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 526: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 527;
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 40) {
					stateStack.Push(526);
					goto case 390;
				} else {
					isMissingModifier = true;
					goto case 528;
				}
			}
			case 528: {
				SetIdentifierExpected(la);
				goto case 529;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (set[122].Get(la.kind)) {
					currentState = 660;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 530;
				}
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(520);
					goto case 510;
				} else {
					if (la.kind == 103) {
						stateStack.Push(520);
						goto case 498;
					} else {
						if (la.kind == 115) {
							stateStack.Push(520);
							goto case 480;
						} else {
							if (la.kind == 142) {
								stateStack.Push(520);
								goto case 9;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(520);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 531;
								} else {
									Error(la);
									goto case 520;
								}
							}
						}
					}
				}
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				if (set[113].Get(la.kind)) {
					stateStack.Push(532);
					goto case 649;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(532);
						goto case 635;
					} else {
						if (la.kind == 101) {
							stateStack.Push(532);
							goto case 619;
						} else {
							if (la.kind == 119) {
								stateStack.Push(532);
								goto case 604;
							} else {
								if (la.kind == 98) {
									stateStack.Push(532);
									goto case 592;
								} else {
									if (la.kind == 186) {
										stateStack.Push(532);
										goto case 547;
									} else {
										if (la.kind == 172) {
											stateStack.Push(532);
											goto case 533;
										} else {
											Error(la);
											goto case 532;
										}
									}
								}
							}
						}
					}
				}
			}
			case 532: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				Expect(172, la); // "Operator"
				currentState = 534;
				break;
			}
			case 534: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 535;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				currentState = 536;
				break;
			}
			case 536: {
				PopContext();
				goto case 537;
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				Expect(37, la); // "("
				currentState = 538;
				break;
			}
			case 538: {
				stateStack.Push(539);
				goto case 377;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				Expect(38, la); // ")"
				currentState = 540;
				break;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				if (la.kind == 63) {
					currentState = 544;
					break;
				} else {
					goto case 541;
				}
			}
			case 541: {
				stateStack.Push(542);
				goto case 234;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				Expect(113, la); // "End"
				currentState = 543;
				break;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 544: {
				PushContext(Context.Type, la, t);
				goto case 545;
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				if (la.kind == 40) {
					stateStack.Push(545);
					goto case 390;
				} else {
					stateStack.Push(546);
					goto case 37;
				}
			}
			case 546: {
				PopContext();
				goto case 541;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				Expect(186, la); // "Property"
				currentState = 548;
				break;
			}
			case 548: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(549);
				goto case 179;
			}
			case 549: {
				PopContext();
				goto case 550;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 37) {
					currentState = 589;
					break;
				} else {
					goto case 551;
				}
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 63) {
					currentState = 587;
					break;
				} else {
					goto case 552;
				}
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (la.kind == 136) {
					currentState = 582;
					break;
				} else {
					goto case 553;
				}
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				if (la.kind == 20) {
					currentState = 581;
					break;
				} else {
					goto case 554;
				}
			}
			case 554: {
				stateStack.Push(555);
				goto case 23;
			}
			case 555: {
				PopContext();
				goto case 556;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 40) {
					stateStack.Push(556);
					goto case 390;
				} else {
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (set[143].Get(la.kind)) {
					currentState = 580;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 558;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 128) {
					currentState = 559;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 559;
						break;
					} else {
						Error(la);
						goto case 559;
					}
				}
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				if (la.kind == 37) {
					currentState = 577;
					break;
				} else {
					goto case 560;
				}
			}
			case 560: {
				stateStack.Push(561);
				goto case 234;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				Expect(113, la); // "End"
				currentState = 562;
				break;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (la.kind == 128) {
					currentState = 563;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 563;
						break;
					} else {
						Error(la);
						goto case 563;
					}
				}
			}
			case 563: {
				stateStack.Push(564);
				goto case 23;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				if (set[104].Get(la.kind)) {
					goto case 567;
				} else {
					goto case 565;
				}
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(113, la); // "End"
				currentState = 566;
				break;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				if (la.kind == 40) {
					stateStack.Push(567);
					goto case 390;
				} else {
					goto case 568;
				}
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (set[143].Get(la.kind)) {
					currentState = 568;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 569;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 569;
							break;
						} else {
							Error(la);
							goto case 569;
						}
					}
				}
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				if (la.kind == 37) {
					currentState = 574;
					break;
				} else {
					goto case 570;
				}
			}
			case 570: {
				stateStack.Push(571);
				goto case 234;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				Expect(113, la); // "End"
				currentState = 572;
				break;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				if (la.kind == 128) {
					currentState = 573;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 573;
						break;
					} else {
						Error(la);
						goto case 573;
					}
				}
			}
			case 573: {
				stateStack.Push(565);
				goto case 23;
			}
			case 574: {
				SetIdentifierExpected(la);
				goto case 575;
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(576);
					goto case 377;
				} else {
					goto case 576;
				}
			}
			case 576: {
				if (la == null) { currentState = 576; break; }
				Expect(38, la); // ")"
				currentState = 570;
				break;
			}
			case 577: {
				SetIdentifierExpected(la);
				goto case 578;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(579);
					goto case 377;
				} else {
					goto case 579;
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(38, la); // ")"
				currentState = 560;
				break;
			}
			case 580: {
				SetIdentifierExpected(la);
				goto case 557;
			}
			case 581: {
				stateStack.Push(554);
				goto case 56;
			}
			case 582: {
				PushContext(Context.Type, la, t);
				stateStack.Push(583);
				goto case 37;
			}
			case 583: {
				PopContext();
				goto case 584;
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (la.kind == 22) {
					currentState = 585;
					break;
				} else {
					goto case 553;
				}
			}
			case 585: {
				PushContext(Context.Type, la, t);
				stateStack.Push(586);
				goto case 37;
			}
			case 586: {
				PopContext();
				goto case 584;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				if (la.kind == 40) {
					stateStack.Push(587);
					goto case 390;
				} else {
					if (la.kind == 162) {
						stateStack.Push(552);
						goto case 67;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(588);
							goto case 37;
						} else {
							Error(la);
							goto case 552;
						}
					}
				}
			}
			case 588: {
				PopContext();
				goto case 552;
			}
			case 589: {
				SetIdentifierExpected(la);
				goto case 590;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(591);
					goto case 377;
				} else {
					goto case 591;
				}
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				Expect(38, la); // ")"
				currentState = 551;
				break;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(98, la); // "Custom"
				currentState = 593;
				break;
			}
			case 593: {
				stateStack.Push(594);
				goto case 604;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				if (set[109].Get(la.kind)) {
					goto case 596;
				} else {
					Expect(113, la); // "End"
					currentState = 595;
					break;
				}
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (la.kind == 40) {
					stateStack.Push(596);
					goto case 390;
				} else {
					if (la.kind == 56) {
						currentState = 597;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 597;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 597;
								break;
							} else {
								Error(la);
								goto case 597;
							}
						}
					}
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(37, la); // "("
				currentState = 598;
				break;
			}
			case 598: {
				stateStack.Push(599);
				goto case 377;
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				Expect(38, la); // ")"
				currentState = 600;
				break;
			}
			case 600: {
				stateStack.Push(601);
				goto case 234;
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				Expect(113, la); // "End"
				currentState = 602;
				break;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				if (la.kind == 56) {
					currentState = 603;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 603;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 603;
							break;
						} else {
							Error(la);
							goto case 603;
						}
					}
				}
			}
			case 603: {
				stateStack.Push(594);
				goto case 23;
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				Expect(119, la); // "Event"
				currentState = 605;
				break;
			}
			case 605: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(606);
				goto case 179;
			}
			case 606: {
				PopContext();
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 63) {
					currentState = 617;
					break;
				} else {
					if (set[144].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 614;
							break;
						} else {
							goto case 608;
						}
					} else {
						Error(la);
						goto case 608;
					}
				}
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (la.kind == 136) {
					currentState = 609;
					break;
				} else {
					goto case 23;
				}
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
					goto case 23;
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
				SetIdentifierExpected(la);
				goto case 615;
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(616);
					goto case 377;
				} else {
					goto case 616;
				}
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				Expect(38, la); // ")"
				currentState = 608;
				break;
			}
			case 617: {
				PushContext(Context.Type, la, t);
				stateStack.Push(618);
				goto case 37;
			}
			case 618: {
				PopContext();
				goto case 608;
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				Expect(101, la); // "Declare"
				currentState = 620;
				break;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 621;
					break;
				} else {
					goto case 621;
				}
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (la.kind == 210) {
					currentState = 622;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 622;
						break;
					} else {
						Error(la);
						goto case 622;
					}
				}
			}
			case 622: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(623);
				goto case 179;
			}
			case 623: {
				PopContext();
				goto case 624;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(149, la); // "Lib"
				currentState = 625;
				break;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				Expect(3, la); // LiteralString
				currentState = 626;
				break;
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 59) {
					currentState = 634;
					break;
				} else {
					goto case 627;
				}
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (la.kind == 37) {
					currentState = 631;
					break;
				} else {
					goto case 628;
				}
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				if (la.kind == 63) {
					currentState = 629;
					break;
				} else {
					goto case 23;
				}
			}
			case 629: {
				PushContext(Context.Type, la, t);
				stateStack.Push(630);
				goto case 37;
			}
			case 630: {
				PopContext();
				goto case 23;
			}
			case 631: {
				SetIdentifierExpected(la);
				goto case 632;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(633);
					goto case 377;
				} else {
					goto case 633;
				}
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				Expect(38, la); // ")"
				currentState = 628;
				break;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				Expect(3, la); // LiteralString
				currentState = 627;
				break;
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 210) {
					currentState = 636;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 636;
						break;
					} else {
						Error(la);
						goto case 636;
					}
				}
			}
			case 636: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 637;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				currentState = 638;
				break;
			}
			case 638: {
				PopContext();
				goto case 639;
			}
			case 639: {
				if (la == null) { currentState = 639; break; }
				if (la.kind == 37) {
					currentState = 645;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 643;
						break;
					} else {
						goto case 640;
					}
				}
			}
			case 640: {
				stateStack.Push(641);
				goto case 234;
			}
			case 641: {
				if (la == null) { currentState = 641; break; }
				Expect(113, la); // "End"
				currentState = 642;
				break;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 474;
					}
				}
			}
			case 643: {
				PushContext(Context.Type, la, t);
				stateStack.Push(644);
				goto case 37;
			}
			case 644: {
				PopContext();
				goto case 640;
			}
			case 645: {
				SetIdentifierExpected(la);
				goto case 646;
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 648;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(647);
							goto case 377;
						} else {
							Error(la);
							goto case 647;
						}
					}
				} else {
					goto case 647;
				}
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				Expect(38, la); // ")"
				currentState = 639;
				break;
			}
			case 648: {
				stateStack.Push(647);
				goto case 441;
			}
			case 649: {
				stateStack.Push(650);
				SetIdentifierExpected(la);
				goto case 651;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (la.kind == 22) {
					currentState = 649;
					break;
				} else {
					goto case 23;
				}
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				if (la.kind == 88) {
					currentState = 652;
					break;
				} else {
					goto case 652;
				}
			}
			case 652: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(653);
				goto case 659;
			}
			case 653: {
				PopContext();
				goto case 654;
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				if (la.kind == 63) {
					currentState = 656;
					break;
				} else {
					goto case 655;
				}
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				if (la.kind == 20) {
					goto case 192;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 656: {
				PushContext(Context.Type, la, t);
				goto case 657;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				if (la.kind == 162) {
					stateStack.Push(658);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(658);
						goto case 37;
					} else {
						Error(la);
						goto case 658;
					}
				}
			}
			case 658: {
				PopContext();
				goto case 655;
			}
			case 659: {
				if (la == null) { currentState = 659; break; }
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
			case 660: {
				isMissingModifier = false;
				goto case 528;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				Expect(136, la); // "Implements"
				currentState = 662;
				break;
			}
			case 662: {
				PushContext(Context.Type, la, t);
				stateStack.Push(663);
				goto case 37;
			}
			case 663: {
				PopContext();
				goto case 664;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				if (la.kind == 22) {
					currentState = 665;
					break;
				} else {
					stateStack.Push(520);
					goto case 23;
				}
			}
			case 665: {
				PushContext(Context.Type, la, t);
				stateStack.Push(666);
				goto case 37;
			}
			case 666: {
				PopContext();
				goto case 664;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				Expect(140, la); // "Inherits"
				currentState = 668;
				break;
			}
			case 668: {
				PushContext(Context.Type, la, t);
				stateStack.Push(669);
				goto case 37;
			}
			case 669: {
				PopContext();
				stateStack.Push(518);
				goto case 23;
			}
			case 670: {
				if (la == null) { currentState = 670; break; }
				Expect(169, la); // "Of"
				currentState = 671;
				break;
			}
			case 671: {
				stateStack.Push(672);
				goto case 441;
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				Expect(38, la); // ")"
				currentState = 515;
				break;
			}
			case 673: {
				isMissingModifier = false;
				goto case 28;
			}
			case 674: {
				PushContext(Context.Type, la, t);
				stateStack.Push(675);
				goto case 37;
			}
			case 675: {
				PopContext();
				goto case 676;
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				if (la.kind == 22) {
					currentState = 677;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 677: {
				PushContext(Context.Type, la, t);
				stateStack.Push(678);
				goto case 37;
			}
			case 678: {
				PopContext();
				goto case 676;
			}
			case 679: {
				if (la == null) { currentState = 679; break; }
				Expect(169, la); // "Of"
				currentState = 680;
				break;
			}
			case 680: {
				stateStack.Push(681);
				goto case 441;
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 682: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 683;
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				if (set[46].Get(la.kind)) {
					currentState = 683;
					break;
				} else {
					PopContext();
					stateStack.Push(684);
					goto case 23;
				}
			}
			case 684: {
				if (la == null) { currentState = 684; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(684);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 685;
					break;
				}
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 686: {
				if (la == null) { currentState = 686; break; }
				Expect(137, la); // "Imports"
				currentState = 687;
				break;
			}
			case 687: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 688;
			}
			case 688: {
				if (la == null) { currentState = 688; break; }
				if (set[145].Get(la.kind)) {
					currentState = 694;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 690;
						break;
					} else {
						Error(la);
						goto case 689;
					}
				}
			}
			case 689: {
				PopContext();
				goto case 23;
			}
			case 690: {
				stateStack.Push(691);
				goto case 179;
			}
			case 691: {
				if (la == null) { currentState = 691; break; }
				Expect(20, la); // "="
				currentState = 692;
				break;
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				Expect(3, la); // LiteralString
				currentState = 693;
				break;
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 689;
				break;
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				if (la.kind == 37) {
					stateStack.Push(694);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 695;
						break;
					} else {
						goto case 689;
					}
				}
			}
			case 695: {
				stateStack.Push(689);
				goto case 37;
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				Expect(173, la); // "Option"
				currentState = 697;
				break;
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 699;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 698;
						break;
					} else {
						goto case 474;
					}
				}
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 474;
					}
				}
			}
			case 699: {
				if (la == null) { currentState = 699; break; }
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