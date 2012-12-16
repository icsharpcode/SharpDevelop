using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 55;
	const int startOfBlock = 265;
	const int endOfStatementTerminatorAndBlock = 269;
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
			case 88:
			case 270:
			case 371:
			case 543:
				{
					BitArray a = new BitArray(243);
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
					BitArray a = new BitArray(243);
					a.Set(144, true);
					return a;
				}
			case 11:
			case 197:
			case 203:
			case 209:
			case 248:
			case 252:
			case 303:
			case 410:
			case 416:
			case 487:
			case 533:
			case 540:
			case 548:
			case 578:
			case 614:
			case 661:
			case 670:
			case 745:
				return set[6];
			case 12:
			case 13:
			case 579:
			case 580:
			case 625:
			case 635:
				{
					BitArray a = new BitArray(243);
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
			case 262:
			case 265:
			case 266:
			case 304:
			case 308:
			case 330:
			case 345:
			case 356:
			case 359:
			case 365:
			case 370:
			case 383:
			case 384:
			case 407:
			case 434:
			case 539:
			case 545:
			case 551:
			case 555:
			case 563:
			case 571:
			case 581:
			case 590:
			case 607:
			case 612:
			case 620:
			case 626:
			case 629:
			case 636:
			case 639:
			case 656:
			case 659:
			case 678:
			case 686:
			case 724:
			case 744:
				{
					BitArray a = new BitArray(243);
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
			case 263:
			case 277:
			case 306:
			case 360:
			case 408:
			case 467:
			case 588:
			case 608:
			case 627:
			case 631:
			case 637:
			case 657:
			case 687:
				{
					BitArray a = new BitArray(243);
					a.Set(115, true);
					return a;
				}
			case 22:
			case 556:
			case 591:
				return set[9];
			case 25:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					return a;
				}
			case 26:
			case 27:
				return set[10];
			case 28:
			case 728:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 156:
			case 221:
			case 222:
			case 271:
			case 282:
			case 283:
			case 475:
			case 476:
			case 477:
			case 478:
			case 566:
			case 567:
			case 600:
			case 601:
			case 681:
			case 682:
			case 737:
			case 738:
				return set[14];
			case 33:
			case 34:
			case 534:
			case 535:
			case 541:
			case 542:
			case 568:
			case 569:
			case 675:
				return set[15];
			case 35:
			case 37:
			case 161:
			case 172:
			case 175:
			case 191:
			case 207:
			case 225:
			case 315:
			case 340:
			case 433:
			case 451:
			case 462:
			case 490:
			case 544:
			case 562:
			case 570:
			case 643:
			case 665:
			case 677:
			case 694:
			case 717:
			case 720:
			case 723:
			case 729:
			case 732:
			case 750:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 97:
			case 101:
			case 167:
			case 399:
			case 494:
				return set[19];
			case 42:
				{
					BitArray a = new BitArray(243);
					a.Set(33, true);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 169:
			case 170:
				return set[20];
			case 45:
			case 46:
			case 171:
			case 192:
			case 403:
			case 438:
			case 488:
			case 491:
			case 511:
			case 574:
			case 605:
			case 655:
			case 698:
			case 727:
			case 736:
				{
					BitArray a = new BitArray(243);
					a.Set(38, true);
					return a;
				}
			case 47:
			case 48:
				return set[21];
			case 49:
			case 183:
			case 190:
			case 405:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					return a;
				}
			case 50:
			case 51:
			case 52:
			case 54:
			case 401:
			case 402:
			case 423:
			case 424:
			case 430:
			case 431:
			case 502:
			case 503:
			case 711:
			case 712:
				return set[22];
			case 53:
			case 173:
			case 174:
			case 176:
			case 185:
			case 425:
			case 432:
			case 440:
			case 449:
			case 453:
			case 498:
			case 501:
			case 505:
			case 507:
			case 508:
			case 518:
			case 525:
			case 532:
			case 713:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 55:
			case 56:
			case 70:
			case 75:
			case 76:
			case 77:
			case 83:
			case 99:
			case 159:
			case 182:
			case 184:
			case 186:
			case 189:
			case 199:
			case 201:
			case 219:
			case 243:
			case 280:
			case 290:
			case 292:
			case 293:
			case 294:
			case 312:
			case 329:
			case 334:
			case 343:
			case 349:
			case 351:
			case 355:
			case 358:
			case 364:
			case 375:
			case 378:
			case 380:
			case 381:
			case 387:
			case 404:
			case 406:
			case 426:
			case 450:
			case 480:
			case 496:
			case 497:
			case 499:
			case 500:
			case 561:
			case 641:
				return set[23];
			case 57:
			case 78:
			case 162:
				return set[24];
			case 58:
				return set[25];
			case 59:
				{
					BitArray a = new BitArray(243);
					a.Set(219, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(243);
					a.Set(147, true);
					return a;
				}
			case 61:
			case 160:
				{
					BitArray a = new BitArray(243);
					a.Set(146, true);
					return a;
				}
			case 62:
				{
					BitArray a = new BitArray(243);
					a.Set(239, true);
					return a;
				}
			case 63:
				{
					BitArray a = new BitArray(243);
					a.Set(180, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(243);
					a.Set(178, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(243);
					a.Set(61, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(243);
					a.Set(60, true);
					return a;
				}
			case 67:
				{
					BitArray a = new BitArray(243);
					a.Set(153, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(243);
					a.Set(42, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(243);
					a.Set(43, true);
					return a;
				}
			case 71:
			case 454:
				{
					BitArray a = new BitArray(243);
					a.Set(40, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(243);
					a.Set(41, true);
					return a;
				}
			case 73:
			case 98:
			case 226:
			case 227:
			case 288:
			case 289:
			case 342:
			case 746:
				{
					BitArray a = new BitArray(243);
					a.Set(20, true);
					return a;
				}
			case 74:
				{
					BitArray a = new BitArray(243);
					a.Set(157, true);
					return a;
				}
			case 79:
			case 91:
			case 93:
			case 152:
				{
					BitArray a = new BitArray(243);
					a.Set(35, true);
					return a;
				}
			case 80:
			case 81:
				return set[26];
			case 82:
				{
					BitArray a = new BitArray(243);
					a.Set(36, true);
					return a;
				}
			case 84:
			case 100:
			case 528:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 85:
			case 121:
				{
					BitArray a = new BitArray(243);
					a.Set(165, true);
					return a;
				}
			case 86:
			case 87:
				return set[27];
			case 89:
			case 92:
			case 153:
			case 154:
			case 157:
				return set[28];
			case 90:
			case 102:
			case 151:
				{
					BitArray a = new BitArray(243);
					a.Set(236, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(243);
					a.Set(26, true);
					a.Set(36, true);
					a.Set(150, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(243);
					a.Set(26, true);
					a.Set(150, true);
					return a;
				}
			case 96:
			case 693:
				{
					BitArray a = new BitArray(243);
					a.Set(26, true);
					return a;
				}
			case 103:
			case 361:
				{
					BitArray a = new BitArray(243);
					a.Set(234, true);
					return a;
				}
			case 104:
				{
					BitArray a = new BitArray(243);
					a.Set(233, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(243);
					a.Set(227, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(243);
					a.Set(226, true);
					return a;
				}
			case 107:
			case 307:
				{
					BitArray a = new BitArray(243);
					a.Set(221, true);
					return a;
				}
			case 108:
				{
					BitArray a = new BitArray(243);
					a.Set(216, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(243);
					a.Set(215, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(243);
					a.Set(214, true);
					return a;
				}
			case 111:
			case 463:
			case 468:
				{
					BitArray a = new BitArray(243);
					a.Set(213, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(243);
					a.Set(212, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(243);
					a.Set(209, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(243);
					a.Set(206, true);
					return a;
				}
			case 115:
			case 367:
				{
					BitArray a = new BitArray(243);
					a.Set(200, true);
					return a;
				}
			case 116:
			case 613:
			case 632:
				{
					BitArray a = new BitArray(243);
					a.Set(189, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(243);
					a.Set(187, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(243);
					a.Set(179, true);
					return a;
				}
			case 119:
				{
					BitArray a = new BitArray(243);
					a.Set(173, true);
					return a;
				}
			case 120:
			case 324:
			case 331:
			case 346:
				{
					BitArray a = new BitArray(243);
					a.Set(166, true);
					return a;
				}
			case 122:
				{
					BitArray a = new BitArray(243);
					a.Set(150, true);
					return a;
				}
			case 123:
			case 230:
			case 235:
			case 237:
				{
					BitArray a = new BitArray(243);
					a.Set(149, true);
					return a;
				}
			case 124:
			case 232:
			case 236:
				{
					BitArray a = new BitArray(243);
					a.Set(145, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(243);
					a.Set(141, true);
					return a;
				}
			case 126:
				{
					BitArray a = new BitArray(243);
					a.Set(135, true);
					return a;
				}
			case 127:
			case 258:
			case 264:
				{
					BitArray a = new BitArray(243);
					a.Set(129, true);
					return a;
				}
			case 128:
			case 155:
			case 255:
				{
					BitArray a = new BitArray(243);
					a.Set(128, true);
					return a;
				}
			case 129:
				{
					BitArray a = new BitArray(243);
					a.Set(126, true);
					return a;
				}
			case 130:
				{
					BitArray a = new BitArray(243);
					a.Set(123, true);
					return a;
				}
			case 131:
			case 200:
				{
					BitArray a = new BitArray(243);
					a.Set(118, true);
					return a;
				}
			case 132:
				{
					BitArray a = new BitArray(243);
					a.Set(110, true);
					return a;
				}
			case 133:
				{
					BitArray a = new BitArray(243);
					a.Set(109, true);
					return a;
				}
			case 134:
				{
					BitArray a = new BitArray(243);
					a.Set(106, true);
					return a;
				}
			case 135:
			case 648:
				{
					BitArray a = new BitArray(243);
					a.Set(100, true);
					return a;
				}
			case 136:
				{
					BitArray a = new BitArray(243);
					a.Set(89, true);
					return a;
				}
			case 137:
				{
					BitArray a = new BitArray(243);
					a.Set(86, true);
					return a;
				}
			case 138:
			case 212:
			case 242:
				{
					BitArray a = new BitArray(243);
					a.Set(72, true);
					return a;
				}
			case 139:
				{
					BitArray a = new BitArray(243);
					a.Set(69, true);
					return a;
				}
			case 140:
				{
					BitArray a = new BitArray(243);
					a.Set(67, true);
					return a;
				}
			case 141:
				{
					BitArray a = new BitArray(243);
					a.Set(65, true);
					return a;
				}
			case 142:
				{
					BitArray a = new BitArray(243);
					a.Set(64, true);
					return a;
				}
			case 143:
				{
					BitArray a = new BitArray(243);
					a.Set(62, true);
					return a;
				}
			case 144:
			case 254:
				{
					BitArray a = new BitArray(243);
					a.Set(58, true);
					return a;
				}
			case 145:
				{
					BitArray a = new BitArray(243);
					a.Set(240, true);
					return a;
				}
			case 146:
				{
					BitArray a = new BitArray(243);
					a.Set(148, true);
					return a;
				}
			case 147:
				{
					BitArray a = new BitArray(243);
					a.Set(68, true);
					return a;
				}
			case 148:
				{
					BitArray a = new BitArray(243);
					a.Set(66, true);
					return a;
				}
			case 149:
				{
					BitArray a = new BitArray(243);
					a.Set(2, true);
					return a;
				}
			case 150:
				return set[29];
			case 158:
				return set[30];
			case 163:
				return set[31];
			case 164:
				return set[32];
			case 165:
			case 166:
			case 492:
			case 493:
				return set[33];
			case 168:
				return set[34];
			case 177:
			case 178:
			case 327:
			case 336:
				return set[35];
			case 179:
			case 257:
			case 470:
				return set[36];
			case 180:
			case 386:
				{
					BitArray a = new BitArray(243);
					a.Set(137, true);
					return a;
				}
			case 181:
			case 188:
			case 193:
			case 259:
			case 435:
			case 464:
			case 486:
			case 489:
			case 602:
			case 603:
			case 653:
				{
					BitArray a = new BitArray(243);
					a.Set(37, true);
					return a;
				}
			case 187:
				return set[37];
			case 194:
				{
					BitArray a = new BitArray(243);
					a.Set(58, true);
					a.Set(128, true);
					return a;
				}
			case 195:
			case 196:
				return set[38];
			case 198:
				{
					BitArray a = new BitArray(243);
					a.Set(174, true);
					return a;
				}
			case 202:
			case 216:
			case 234:
			case 239:
			case 245:
			case 247:
			case 251:
			case 253:
				return set[39];
			case 204:
			case 205:
				{
					BitArray a = new BitArray(243);
					a.Set(63, true);
					a.Set(140, true);
					return a;
				}
			case 206:
			case 208:
			case 328:
				{
					BitArray a = new BitArray(243);
					a.Set(140, true);
					return a;
				}
			case 210:
			case 211:
			case 213:
			case 215:
			case 217:
			case 218:
			case 228:
			case 233:
			case 238:
			case 246:
			case 250:
			case 275:
			case 279:
				return set[40];
			case 214:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(145, true);
					return a;
				}
			case 220:
				return set[41];
			case 223:
			case 284:
				return set[42];
			case 224:
			case 285:
				return set[43];
			case 229:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(72, true);
					return a;
				}
			case 231:
				{
					BitArray a = new BitArray(243);
					a.Set(135, true);
					a.Set(145, true);
					a.Set(149, true);
					return a;
				}
			case 240:
			case 241:
				return set[44];
			case 244:
				{
					BitArray a = new BitArray(243);
					a.Set(64, true);
					a.Set(106, true);
					return a;
				}
			case 249:
				return set[45];
			case 256:
				return set[46];
			case 260:
			case 261:
				return set[47];
			case 267:
			case 268:
				return set[48];
			case 269:
				return set[49];
			case 272:
				return set[50];
			case 273:
			case 274:
			case 392:
				return set[51];
			case 276:
			case 281:
			case 372:
			case 691:
			case 701:
			case 718:
			case 719:
			case 721:
			case 730:
			case 731:
			case 733:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 278:
				{
					BitArray a = new BitArray(243);
					a.Set(229, true);
					return a;
				}
			case 286:
			case 287:
				return set[52];
			case 291:
			case 335:
			case 350:
			case 415:
				return set[53];
			case 295:
			case 298:
			case 393:
			case 396:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(113, true);
					return a;
				}
			case 296:
			case 297:
			case 317:
			case 318:
			case 332:
			case 333:
			case 347:
			case 348:
				return set[54];
			case 299:
				{
					BitArray a = new BitArray(243);
					a.Set(110, true);
					a.Set(126, true);
					a.Set(234, true);
					return a;
				}
			case 300:
				return set[55];
			case 301:
			case 320:
				return set[56];
			case 302:
				{
					BitArray a = new BitArray(243);
					a.Set(5, true);
					return a;
				}
			case 305:
				{
					BitArray a = new BitArray(243);
					a.Set(77, true);
					a.Set(115, true);
					a.Set(125, true);
					return a;
				}
			case 309:
			case 310:
				return set[57];
			case 311:
			case 316:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(232, true);
					return a;
				}
			case 313:
			case 314:
				return set[58];
			case 319:
				return set[59];
			case 321:
				{
					BitArray a = new BitArray(243);
					a.Set(120, true);
					return a;
				}
			case 322:
			case 323:
				return set[60];
			case 325:
			case 326:
				return set[61];
			case 337:
			case 338:
				return set[62];
			case 339:
				return set[63];
			case 341:
				{
					BitArray a = new BitArray(243);
					a.Set(20, true);
					a.Set(140, true);
					return a;
				}
			case 344:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(208, true);
					return a;
				}
			case 352:
				return set[64];
			case 353:
			case 357:
				{
					BitArray a = new BitArray(243);
					a.Set(155, true);
					return a;
				}
			case 354:
				return set[65];
			case 362:
			case 363:
				return set[66];
			case 366:
				{
					BitArray a = new BitArray(243);
					a.Set(76, true);
					a.Set(115, true);
					return a;
				}
			case 368:
			case 369:
				return set[67];
			case 373:
			case 374:
				return set[68];
			case 376:
				return set[69];
			case 377:
			case 379:
				return set[70];
			case 382:
			case 388:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(217, true);
					return a;
				}
			case 385:
				{
					BitArray a = new BitArray(243);
					a.Set(113, true);
					a.Set(114, true);
					a.Set(115, true);
					return a;
				}
			case 389:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(137, true);
					return a;
				}
			case 390:
			case 391:
			case 465:
			case 466:
				return set[71];
			case 394:
			case 395:
			case 397:
			case 398:
				return set[72];
			case 400:
				return set[73];
			case 409:
				{
					BitArray a = new BitArray(243);
					a.Set(214, true);
					a.Set(236, true);
					return a;
				}
			case 411:
			case 412:
			case 417:
			case 418:
				return set[74];
			case 413:
			case 419:
				return set[75];
			case 414:
			case 422:
			case 429:
				return set[76];
			case 420:
			case 421:
			case 427:
			case 428:
			case 708:
			case 709:
				return set[77];
			case 436:
			case 437:
				return set[78];
			case 439:
			case 441:
			case 442:
			case 604:
			case 654:
				return set[79];
			case 443:
			case 444:
				return set[80];
			case 445:
			case 446:
				return set[81];
			case 447:
				return set[82];
			case 448:
			case 452:
				{
					BitArray a = new BitArray(243);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 455:
			case 456:
			case 460:
				return set[83];
			case 457:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(39, true);
					return a;
				}
			case 458:
			case 459:
				return set[84];
			case 461:
				{
					BitArray a = new BitArray(243);
					a.Set(21, true);
					return a;
				}
			case 469:
				return set[85];
			case 471:
			case 484:
				return set[86];
			case 472:
			case 485:
				return set[87];
			case 473:
			case 474:
				{
					BitArray a = new BitArray(243);
					a.Set(10, true);
					return a;
				}
			case 479:
				{
					BitArray a = new BitArray(243);
					a.Set(12, true);
					return a;
				}
			case 481:
				{
					BitArray a = new BitArray(243);
					a.Set(13, true);
					return a;
				}
			case 482:
				return set[88];
			case 483:
				return set[89];
			case 495:
				return set[90];
			case 504:
			case 506:
				return set[91];
			case 509:
			case 510:
			case 572:
			case 573:
			case 696:
			case 697:
				return set[92];
			case 512:
			case 513:
			case 514:
			case 519:
			case 520:
			case 575:
			case 699:
			case 726:
			case 735:
				return set[93];
			case 515:
			case 521:
			case 530:
				return set[94];
			case 516:
			case 517:
			case 522:
			case 523:
				{
					BitArray a = new BitArray(243);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 524:
			case 526:
			case 531:
				return set[95];
			case 527:
			case 529:
				return set[96];
			case 536:
			case 549:
			case 550:
			case 606:
			case 676:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 537:
			case 538:
			case 610:
			case 611:
				return set[97];
			case 546:
			case 547:
			case 554:
				{
					BitArray a = new BitArray(243);
					a.Set(117, true);
					return a;
				}
			case 552:
			case 553:
				return set[98];
			case 557:
			case 558:
				return set[99];
			case 559:
			case 560:
			case 619:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 564:
				{
					BitArray a = new BitArray(243);
					a.Set(105, true);
					return a;
				}
			case 565:
			case 669:
			case 680:
			case 688:
				{
					BitArray a = new BitArray(243);
					a.Set(129, true);
					a.Set(213, true);
					return a;
				}
			case 576:
			case 577:
			case 589:
				{
					BitArray a = new BitArray(243);
					a.Set(86, true);
					a.Set(158, true);
					a.Set(212, true);
					return a;
				}
			case 582:
			case 583:
				return set[100];
			case 584:
			case 585:
				return set[101];
			case 586:
			case 587:
			case 598:
				return set[102];
			case 592:
			case 593:
				return set[103];
			case 594:
			case 595:
			case 715:
				return set[104];
			case 596:
				return set[105];
			case 597:
				return set[106];
			case 599:
			case 609:
				{
					BitArray a = new BitArray(243);
					a.Set(175, true);
					return a;
				}
			case 615:
			case 616:
				return set[107];
			case 617:
				return set[108];
			case 618:
			case 647:
				return set[109];
			case 621:
			case 622:
			case 623:
			case 640:
				return set[110];
			case 624:
			case 628:
			case 638:
				{
					BitArray a = new BitArray(243);
					a.Set(130, true);
					a.Set(201, true);
					return a;
				}
			case 630:
				return set[111];
			case 633:
				return set[112];
			case 634:
				return set[113];
			case 642:
			case 716:
				{
					BitArray a = new BitArray(243);
					a.Set(138, true);
					return a;
				}
			case 644:
			case 707:
			case 710:
				return set[114];
			case 645:
			case 646:
				return set[115];
			case 649:
			case 651:
			case 660:
				{
					BitArray a = new BitArray(243);
					a.Set(121, true);
					return a;
				}
			case 650:
				return set[116];
			case 652:
				return set[117];
			case 658:
				{
					BitArray a = new BitArray(243);
					a.Set(56, true);
					a.Set(192, true);
					a.Set(196, true);
					return a;
				}
			case 662:
			case 663:
				return set[118];
			case 664:
			case 666:
				{
					BitArray a = new BitArray(243);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(138, true);
					return a;
				}
			case 667:
				{
					BitArray a = new BitArray(243);
					a.Set(103, true);
					return a;
				}
			case 668:
				return set[119];
			case 671:
			case 672:
				{
					BitArray a = new BitArray(243);
					a.Set(152, true);
					return a;
				}
			case 673:
			case 679:
			case 747:
				{
					BitArray a = new BitArray(243);
					a.Set(3, true);
					return a;
				}
			case 674:
				return set[120];
			case 683:
			case 684:
				return set[121];
			case 685:
			case 695:
				return set[122];
			case 689:
				{
					BitArray a = new BitArray(243);
					a.Set(136, true);
					return a;
				}
			case 690:
			case 692:
				return set[123];
			case 700:
			case 702:
				return set[124];
			case 703:
			case 714:
				return set[125];
			case 704:
			case 705:
				return set[126];
			case 706:
				return set[127];
			case 722:
				{
					BitArray a = new BitArray(243);
					a.Set(142, true);
					return a;
				}
			case 725:
			case 734:
				{
					BitArray a = new BitArray(243);
					a.Set(172, true);
					return a;
				}
			case 739:
				return set[128];
			case 740:
				{
					BitArray a = new BitArray(243);
					a.Set(163, true);
					return a;
				}
			case 741:
				{
					BitArray a = new BitArray(243);
					a.Set(139, true);
					return a;
				}
			case 742:
			case 743:
				return set[129];
			case 748:
				{
					BitArray a = new BitArray(243);
					a.Set(11, true);
					return a;
				}
			case 749:
				return set[130];
			case 751:
				{
					BitArray a = new BitArray(243);
					a.Set(176, true);
					return a;
				}
			case 752:
				return set[131];
			case 753:
				{
					BitArray a = new BitArray(243);
					a.Set(69, true);
					a.Set(216, true);
					return a;
				}
			case 754:
				return set[132];
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
	bool xmlAllowed = true;
	bool identifierExpected = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	bool isMissingModifier = false;
	bool isAlreadyInExpr = false;
	bool wasNormalAttribute = false;
	int lambdaNestingDepth = 0;
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
				if (la.kind == 176) {
					stateStack.Push(1);
					goto case 751;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 139) {
					stateStack.Push(2);
					goto case 741;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 454;
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
				if (la.kind == 163) {
					currentState = 737;
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
					goto case 454;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[133].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 86 || la.kind == 158 || la.kind == 212) {
						goto case 576;
					} else {
						if (la.kind == 105) {
							currentState = 565;
							break;
						} else {
							if (la.kind == 117) {
								goto case 546;
							} else {
								if (la.kind == 144) {
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
				Expect(144, la); // "Interface"
				currentState = 11;
				break;
			}
			case 11: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(12);
				goto case 209;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 734;
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
				if (la.kind == 142) {
					currentState = 729;
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
				Expect(115, la); // "End"
				currentState = 20;
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(144, la); // "Interface"
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
					goto case 454;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[134].Get(la.kind)) {
					currentState = 728;
					break;
				} else {
					isMissingModifier = false;
					goto case 29;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 86 || la.kind == 158 || la.kind == 212) {
					stateStack.Push(17);
					goto case 576;
				} else {
					if (la.kind == 105) {
						stateStack.Push(17);
						goto case 564;
					} else {
						if (la.kind == 117) {
							stateStack.Push(17);
							goto case 546;
						} else {
							if (la.kind == 144) {
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
				if (la.kind == 121) {
					currentState = 540;
					break;
				} else {
					if (la.kind == 189) {
						currentState = 533;
						break;
					} else {
						if (la.kind == 129 || la.kind == 213) {
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
					currentState = 509;
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
				if (la.kind == 132) {
					currentState = 38;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 38;
						break;
					} else {
						if (set[135].Get(la.kind)) {
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
				if (la.kind == 33 || la.kind == 37) {
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
				goto case 101;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 33 || la.kind == 37) {
					stateStack.Push(41);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (la.kind == 33) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 37) {
						currentState = 43;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 43: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 172) {
					currentState = 504;
					break;
				} else {
					if (set[22].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(45);
							goto case 47;
						} else {
							goto case 45;
						}
					} else {
						Error(la);
						goto case 45;
					}
				}
			}
			case 45: {
				PopContext();
				goto case 46;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 47: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 48;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[23].Get(la.kind)) {
					activeArgument = 0;
					goto case 500;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 49;
					} else {
						goto case 6;
					}
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(22, la); // ","
				currentState = 50;
				break;
			}
			case 50: {
				activeArgument++;
				goto case 51;
			}
			case 51: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 52;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(53);
					goto case 55;
				} else {
					goto case 53;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (la.kind == 22) {
					currentState = 54;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 54: {
				activeArgument++;
				goto case 51;
			}
			case 55: {
				PushContext(Context.Expression, la, t);
				goto case 56;
			}
			case 56: {
				stateStack.Push(57);
				goto case 75;
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (set[25].Get(la.kind)) {
					stateStack.Push(56);
					goto case 58;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 31) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 30) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 34) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 25) {
								currentState = stateStack.Pop();
								break;
							} else {
								if (la.kind == 24) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 32) {
										currentState = stateStack.Pop();
										break;
									} else {
										if (la.kind == 157) {
											goto case 74;
										} else {
											if (la.kind == 20) {
												goto case 73;
											} else {
												if (la.kind == 41) {
													goto case 72;
												} else {
													if (la.kind == 40) {
														goto case 71;
													} else {
														if (la.kind == 39) {
															currentState = 70;
															break;
														} else {
															if (la.kind == 43) {
																goto case 69;
															} else {
																if (la.kind == 42) {
																	goto case 68;
																} else {
																	if (la.kind == 153) {
																		goto case 67;
																	} else {
																		if (la.kind == 23) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 60) {
																				goto case 66;
																			} else {
																				if (la.kind == 61) {
																					goto case 65;
																				} else {
																					if (la.kind == 178) {
																						goto case 64;
																					} else {
																						if (la.kind == 180) {
																							goto case 63;
																						} else {
																							if (la.kind == 239) {
																								goto case 62;
																							} else {
																								if (la.kind == 44) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 45) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 146) {
																											goto case 61;
																										} else {
																											if (la.kind == 147) {
																												goto case 60;
																											} else {
																												if (la.kind == 47) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 49) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 50) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 51) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 46) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 48) {
																																		currentState = stateStack.Pop();
																																		break;
																																	} else {
																																		if (la.kind == 54) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 52) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 53) {
																																					currentState = stateStack.Pop();
																																					break;
																																				} else {
																																					if (la.kind == 219) {
																																						goto case 59;
																																					} else {
																																						if (la.kind == 55) {
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
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 70: {
				wasNormalAttribute = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 75: {
				PushContext(Context.Expression, la, t);
				goto case 76;
			}
			case 76: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 77;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (set[136].Get(la.kind)) {
					currentState = 76;
					break;
				} else {
					if (set[35].Get(la.kind)) {
						stateStack.Push(163);
						goto case 177;
					} else {
						if (la.kind == 223) {
							currentState = 159;
							break;
						} else {
							if (la.kind == 165) {
								stateStack.Push(78);
								goto case 85;
							} else {
								if (la.kind == 35) {
									stateStack.Push(78);
									goto case 79;
								} else {
									Error(la);
									goto case 78;
								}
							}
						}
					}
				}
			}
			case 78: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				Expect(35, la); // "{"
				currentState = 80;
				break;
			}
			case 80: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 81;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[23].Get(la.kind)) {
					goto case 83;
				} else {
					goto case 82;
				}
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 83: {
				stateStack.Push(84);
				goto case 55;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 22) {
					currentState = 83;
					break;
				} else {
					goto case 82;
				}
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				Expect(165, la); // "New"
				currentState = 86;
				break;
			}
			case 86: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 87;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(150);
					goto case 37;
				} else {
					if (la.kind == 236) {
						PushContext(Context.ObjectInitializer, la, t);
						goto case 90;
					} else {
						goto case 88;
					}
				}
			}
			case 88: {
				Error(la);
				goto case 89;
			}
			case 89: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				Expect(236, la); // "With"
				currentState = 91;
				break;
			}
			case 91: {
				stateStack.Push(92);
				goto case 93;
			}
			case 92: {
				PopContext();
				goto case 89;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				Expect(35, la); // "{"
				currentState = 94;
				break;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.kind == 26 || la.kind == 150) {
					goto case 95;
				} else {
					goto case 82;
				}
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				if (la.kind == 150) {
					currentState = 96;
					break;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				Expect(26, la); // "."
				currentState = 97;
				break;
			}
			case 97: {
				stateStack.Push(98);
				goto case 101;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				Expect(20, la); // "="
				currentState = 99;
				break;
			}
			case 99: {
				stateStack.Push(100);
				goto case 55;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.kind == 22) {
					currentState = 95;
					break;
				} else {
					goto case 82;
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 2) {
					goto case 149;
				} else {
					if (la.kind == 66) {
						goto case 148;
					} else {
						if (la.kind == 68) {
							goto case 147;
						} else {
							if (la.kind == 148) {
								goto case 146;
							} else {
								if (la.kind == 240) {
									goto case 145;
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
												goto case 144;
											} else {
												if (la.kind == 59) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 60) {
														goto case 66;
													} else {
														if (la.kind == 61) {
															goto case 65;
														} else {
															if (la.kind == 62) {
																goto case 143;
															} else {
																if (la.kind == 63) {
																	currentState = stateStack.Pop();
																	break;
																} else {
																	if (la.kind == 64) {
																		goto case 142;
																	} else {
																		if (la.kind == 65) {
																			goto case 141;
																		} else {
																			if (la.kind == 67) {
																				goto case 140;
																			} else {
																				if (la.kind == 69) {
																					goto case 139;
																				} else {
																					if (la.kind == 70) {
																						currentState = stateStack.Pop();
																						break;
																					} else {
																						if (la.kind == 71) {
																							currentState = stateStack.Pop();
																							break;
																						} else {
																							if (la.kind == 72) {
																								goto case 138;
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
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 85) {
																																					currentState = stateStack.Pop();
																																					break;
																																				} else {
																																					if (la.kind == 86) {
																																						goto case 137;
																																					} else {
																																						if (la.kind == 87) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							if (la.kind == 88) {
																																								currentState = stateStack.Pop();
																																								break;
																																							} else {
																																								if (la.kind == 89) {
																																									goto case 136;
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
																																																		currentState = stateStack.Pop();
																																																		break;
																																																	} else {
																																																		if (la.kind == 99) {
																																																			currentState = stateStack.Pop();
																																																			break;
																																																		} else {
																																																			if (la.kind == 100) {
																																																				goto case 135;
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
																																																								currentState = stateStack.Pop();
																																																								break;
																																																							} else {
																																																								if (la.kind == 105) {
																																																									currentState = stateStack.Pop();
																																																									break;
																																																								} else {
																																																									if (la.kind == 106) {
																																																										goto case 134;
																																																									} else {
																																																										if (la.kind == 107) {
																																																											currentState = stateStack.Pop();
																																																											break;
																																																										} else {
																																																											if (la.kind == 108) {
																																																												currentState = stateStack.Pop();
																																																												break;
																																																											} else {
																																																												if (la.kind == 109) {
																																																													goto case 133;
																																																												} else {
																																																													if (la.kind == 110) {
																																																														goto case 132;
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
																																																																				currentState = stateStack.Pop();
																																																																				break;
																																																																			} else {
																																																																				if (la.kind == 117) {
																																																																					currentState = stateStack.Pop();
																																																																					break;
																																																																				} else {
																																																																					if (la.kind == 118) {
																																																																						goto case 131;
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
																																																																									currentState = stateStack.Pop();
																																																																									break;
																																																																								} else {
																																																																									if (la.kind == 122) {
																																																																										currentState = stateStack.Pop();
																																																																										break;
																																																																									} else {
																																																																										if (la.kind == 123) {
																																																																											goto case 130;
																																																																										} else {
																																																																											if (la.kind == 124) {
																																																																												currentState = stateStack.Pop();
																																																																												break;
																																																																											} else {
																																																																												if (la.kind == 125) {
																																																																													currentState = stateStack.Pop();
																																																																													break;
																																																																												} else {
																																																																													if (la.kind == 126) {
																																																																														goto case 129;
																																																																													} else {
																																																																														if (la.kind == 127) {
																																																																															currentState = stateStack.Pop();
																																																																															break;
																																																																														} else {
																																																																															if (la.kind == 128) {
																																																																																goto case 128;
																																																																															} else {
																																																																																if (la.kind == 129) {
																																																																																	goto case 127;
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
																																																																																					currentState = stateStack.Pop();
																																																																																					break;
																																																																																				} else {
																																																																																					if (la.kind == 134) {
																																																																																						currentState = stateStack.Pop();
																																																																																						break;
																																																																																					} else {
																																																																																						if (la.kind == 135) {
																																																																																							goto case 126;
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
																																																																																											currentState = stateStack.Pop();
																																																																																											break;
																																																																																										} else {
																																																																																											if (la.kind == 140) {
																																																																																												currentState = stateStack.Pop();
																																																																																												break;
																																																																																											} else {
																																																																																												if (la.kind == 141) {
																																																																																													goto case 125;
																																																																																												} else {
																																																																																													if (la.kind == 142) {
																																																																																														currentState = stateStack.Pop();
																																																																																														break;
																																																																																													} else {
																																																																																														if (la.kind == 143) {
																																																																																															currentState = stateStack.Pop();
																																																																																															break;
																																																																																														} else {
																																																																																															if (la.kind == 144) {
																																																																																																currentState = stateStack.Pop();
																																																																																																break;
																																																																																															} else {
																																																																																																if (la.kind == 145) {
																																																																																																	goto case 124;
																																																																																																} else {
																																																																																																	if (la.kind == 146) {
																																																																																																		goto case 61;
																																																																																																	} else {
																																																																																																		if (la.kind == 147) {
																																																																																																			goto case 60;
																																																																																																		} else {
																																																																																																			if (la.kind == 149) {
																																																																																																				goto case 123;
																																																																																																			} else {
																																																																																																				if (la.kind == 150) {
																																																																																																					goto case 122;
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
																																																																																																								goto case 67;
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
																																																																																																												goto case 74;
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
																																																																																																																	currentState = stateStack.Pop();
																																																																																																																	break;
																																																																																																																} else {
																																																																																																																	if (la.kind == 163) {
																																																																																																																		currentState = stateStack.Pop();
																																																																																																																		break;
																																																																																																																	} else {
																																																																																																																		if (la.kind == 164) {
																																																																																																																			currentState = stateStack.Pop();
																																																																																																																			break;
																																																																																																																		} else {
																																																																																																																			if (la.kind == 165) {
																																																																																																																				goto case 121;
																																																																																																																			} else {
																																																																																																																				if (la.kind == 166) {
																																																																																																																					goto case 120;
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
																																																																																																																									currentState = stateStack.Pop();
																																																																																																																									break;
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
																																																																																																																												goto case 119;
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
																																																																																																																															currentState = stateStack.Pop();
																																																																																																																															break;
																																																																																																																														} else {
																																																																																																																															if (la.kind == 177) {
																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																break;
																																																																																																																															} else {
																																																																																																																																if (la.kind == 178) {
																																																																																																																																	goto case 64;
																																																																																																																																} else {
																																																																																																																																	if (la.kind == 179) {
																																																																																																																																		goto case 118;
																																																																																																																																	} else {
																																																																																																																																		if (la.kind == 180) {
																																																																																																																																			goto case 63;
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
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 185) {
																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																								break;
																																																																																																																																							} else {
																																																																																																																																								if (la.kind == 186) {
																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																									break;
																																																																																																																																								} else {
																																																																																																																																									if (la.kind == 187) {
																																																																																																																																										goto case 117;
																																																																																																																																									} else {
																																																																																																																																										if (la.kind == 188) {
																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																											break;
																																																																																																																																										} else {
																																																																																																																																											if (la.kind == 189) {
																																																																																																																																												goto case 116;
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
																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																				break;
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
																																																																																																																																																							goto case 115;
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
																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																										break;
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
																																																																																																																																																													goto case 114;
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
																																																																																																																																																																goto case 113;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 210) {
																																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																																	break;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 211) {
																																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																																		break;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 212) {
																																																																																																																																																																			goto case 112;
																																																																																																																																																																		} else {
																																																																																																																																																																			if (la.kind == 213) {
																																																																																																																																																																				goto case 111;
																																																																																																																																																																			} else {
																																																																																																																																																																				if (la.kind == 214) {
																																																																																																																																																																					goto case 110;
																																																																																																																																																																				} else {
																																																																																																																																																																					if (la.kind == 215) {
																																																																																																																																																																						goto case 109;
																																																																																																																																																																					} else {
																																																																																																																																																																						if (la.kind == 216) {
																																																																																																																																																																							goto case 108;
																																																																																																																																																																						} else {
																																																																																																																																																																							if (la.kind == 217) {
																																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																																								break;
																																																																																																																																																																							} else {
																																																																																																																																																																								if (la.kind == 218) {
																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																									break;
																																																																																																																																																																								} else {
																																																																																																																																																																									if (la.kind == 219) {
																																																																																																																																																																										goto case 59;
																																																																																																																																																																									} else {
																																																																																																																																																																										if (la.kind == 220) {
																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																											break;
																																																																																																																																																																										} else {
																																																																																																																																																																											if (la.kind == 221) {
																																																																																																																																																																												goto case 107;
																																																																																																																																																																											} else {
																																																																																																																																																																												if (la.kind == 222) {
																																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																																													break;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 223) {
																																																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																																																														break;
																																																																																																																																																																													} else {
																																																																																																																																																																														if (la.kind == 224) {
																																																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																																																															break;
																																																																																																																																																																														} else {
																																																																																																																																																																															if (la.kind == 225) {
																																																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																																																break;
																																																																																																																																																																															} else {
																																																																																																																																																																																if (la.kind == 226) {
																																																																																																																																																																																	goto case 106;
																																																																																																																																																																																} else {
																																																																																																																																																																																	if (la.kind == 227) {
																																																																																																																																																																																		goto case 105;
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
																																																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																																																					break;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 231) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 232) {
																																																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																																																							break;
																																																																																																																																																																																						} else {
																																																																																																																																																																																							if (la.kind == 233) {
																																																																																																																																																																																								goto case 104;
																																																																																																																																																																																							} else {
																																																																																																																																																																																								if (la.kind == 234) {
																																																																																																																																																																																									goto case 103;
																																																																																																																																																																																								} else {
																																																																																																																																																																																									if (la.kind == 235) {
																																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																																										break;
																																																																																																																																																																																									} else {
																																																																																																																																																																																										if (la.kind == 236) {
																																																																																																																																																																																											goto case 102;
																																																																																																																																																																																										} else {
																																																																																																																																																																																											if (la.kind == 237) {
																																																																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																																																																												break;
																																																																																																																																																																																											} else {
																																																																																																																																																																																												if (la.kind == 238) {
																																																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																																																													break;
																																																																																																																																																																																												} else {
																																																																																																																																																																																													if (la.kind == 239) {
																																																																																																																																																																																														goto case 62;
																																																																																																																																																																																													} else {
																																																																																																																																																																																														if (la.kind == 241) {
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
						}
					}
				}
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
				currentState = stateStack.Pop();
				break;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 35 || la.kind == 128 || la.kind == 236) {
					if (la.kind == 128) {
						PushContext(Context.CollectionInitializer, la, t);
						goto case 155;
					} else {
						if (la.kind == 35) {
							PushContext(Context.CollectionInitializer, la, t);
							stateStack.Push(154);
							goto case 79;
						} else {
							if (la.kind == 236) {
								PushContext(Context.ObjectInitializer, la, t);
								goto case 151;
							} else {
								goto case 88;
							}
						}
					}
				} else {
					goto case 89;
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(236, la); // "With"
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 93;
			}
			case 153: {
				PopContext();
				goto case 89;
			}
			case 154: {
				PopContext();
				goto case 89;
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				Expect(128, la); // "From"
				currentState = 156;
				break;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 35) {
					stateStack.Push(157);
					goto case 79;
				} else {
					if (set[30].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						Error(la);
						goto case 157;
					}
				}
			}
			case 157: {
				PopContext();
				goto case 89;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				currentState = 157;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 75;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				Expect(146, la); // "Is"
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
				goto case 78;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(163);
					goto case 164;
				} else {
					goto case 78;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 37) {
					currentState = 169;
					break;
				} else {
					if (set[137].Get(la.kind)) {
						currentState = 165;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 165: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 166;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 10) {
					currentState = 167;
					break;
				} else {
					goto case 167;
				}
			}
			case 167: {
				stateStack.Push(168);
				goto case 101;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 169: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 170;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 172) {
					currentState = 172;
					break;
				} else {
					if (set[22].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(171);
							goto case 47;
						} else {
							goto case 171;
						}
					} else {
						Error(la);
						goto case 171;
					}
				}
			}
			case 171: {
				PopContext();
				goto case 46;
			}
			case 172: {
				PushContext(Context.Type, la, t);
				stateStack.Push(173);
				goto case 37;
			}
			case 173: {
				PopContext();
				goto case 174;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 22) {
					currentState = 175;
					break;
				} else {
					goto case 171;
				}
			}
			case 175: {
				PushContext(Context.Type, la, t);
				stateStack.Push(176);
				goto case 37;
			}
			case 176: {
				PopContext();
				goto case 174;
			}
			case 177: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[138].Get(la.kind)) {
					currentState = 179;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 496;
						break;
					} else {
						if (set[139].Get(la.kind)) {
							currentState = 179;
							break;
						} else {
							if (set[135].Get(la.kind)) {
								currentState = 179;
								break;
							} else {
								if (set[137].Get(la.kind)) {
									currentState = 492;
									break;
								} else {
									if (la.kind == 131) {
										currentState = 489;
										break;
									} else {
										if (la.kind == 241) {
											currentState = 486;
											break;
										} else {
											if (set[85].Get(la.kind)) {
												stateStack.Push(179);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 469;
											} else {
												if (set[46].Get(la.kind)) {
													stateStack.Push(179);
													lambdaNestingDepth++;
													goto case 256;
												} else {
													if (la.kind == 58 || la.kind == 128) {
														stateStack.Push(179);
														PushContext(Context.Query, la, t);
														goto case 194;
													} else {
														if (set[37].Get(la.kind)) {
															stateStack.Push(179);
															goto case 187;
														} else {
															if (la.kind == 137) {
																stateStack.Push(179);
																goto case 180;
															} else {
																Error(la);
																goto case 179;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 179: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(137, la); // "If"
				currentState = 181;
				break;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(37, la); // "("
				currentState = 182;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 55;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(22, la); // ","
				currentState = 184;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 55;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 186;
					break;
				} else {
					goto case 46;
				}
			}
			case 186: {
				stateStack.Push(46);
				goto case 55;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (set[140].Get(la.kind)) {
					currentState = 193;
					break;
				} else {
					if (la.kind == 96 || la.kind == 108 || la.kind == 222) {
						currentState = 188;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(37, la); // "("
				currentState = 189;
				break;
			}
			case 189: {
				stateStack.Push(190);
				goto case 55;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(22, la); // ","
				currentState = 191;
				break;
			}
			case 191: {
				PushContext(Context.Type, la, t);
				stateStack.Push(192);
				goto case 37;
			}
			case 192: {
				PopContext();
				goto case 46;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(37, la); // "("
				currentState = 186;
				break;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 128) {
					stateStack.Push(195);
					goto case 255;
				} else {
					if (la.kind == 58) {
						stateStack.Push(195);
						goto case 254;
					} else {
						Error(la);
						goto case 195;
					}
				}
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (set[38].Get(la.kind)) {
					stateStack.Push(195);
					goto case 196;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 128) {
					currentState = 252;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 248;
						break;
					} else {
						if (la.kind == 200) {
							currentState = 246;
							break;
						} else {
							if (la.kind == 109) {
								goto case 133;
							} else {
								if (la.kind == 233) {
									currentState = 55;
									break;
								} else {
									if (la.kind == 179) {
										currentState = 242;
										break;
									} else {
										if (la.kind == 206 || la.kind == 215) {
											currentState = 240;
											break;
										} else {
											if (la.kind == 151) {
												currentState = 238;
												break;
											} else {
												if (la.kind == 135) {
													currentState = 210;
													break;
												} else {
													if (la.kind == 149) {
														currentState = 197;
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
			case 197: {
				stateStack.Push(198);
				goto case 203;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(174, la); // "On"
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(200);
				goto case 55;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				Expect(118, la); // "Equals"
				currentState = 201;
				break;
			}
			case 201: {
				stateStack.Push(202);
				goto case 55;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 22) {
					currentState = 199;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(204);
				goto case 209;
			}
			case 204: {
				PopContext();
				goto case 205;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (la.kind == 63) {
					currentState = 207;
					break;
				} else {
					goto case 206;
				}
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(140, la); // "In"
				currentState = 55;
				break;
			}
			case 207: {
				PushContext(Context.Type, la, t);
				stateStack.Push(208);
				goto case 37;
			}
			case 208: {
				PopContext();
				goto case 206;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (set[125].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 100) {
						goto case 135;
					} else {
						if (la.kind == 66) {
							goto case 148;
						} else {
							if (la.kind == 148) {
								goto case 146;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 210: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 211;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				if (la.kind == 149) {
					goto case 230;
				} else {
					if (set[40].Get(la.kind)) {
						if (la.kind == 72) {
							currentState = 213;
							break;
						} else {
							if (set[40].Get(la.kind)) {
								goto case 228;
							} else {
								Error(la);
								goto case 212;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(72, la); // "By"
				currentState = 213;
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 217;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 22) {
					currentState = 213;
					break;
				} else {
					Expect(145, la); // "Into"
					currentState = 215;
					break;
				}
			}
			case 215: {
				stateStack.Push(216);
				goto case 217;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (la.kind == 22) {
					currentState = 215;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 217: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(221);
					goto case 209;
				} else {
					goto case 219;
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 55;
			}
			case 220: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 221: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 222;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (set[42].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 223;
				} else {
					goto case 219;
				}
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 63) {
					currentState = 225;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 219;
						break;
					} else {
						if (set[43].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 219;
						}
					}
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				currentState = 219;
				break;
			}
			case 225: {
				PushContext(Context.Type, la, t);
				stateStack.Push(226);
				goto case 37;
			}
			case 226: {
				PopContext();
				goto case 227;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(20, la); // "="
				currentState = 219;
				break;
			}
			case 228: {
				stateStack.Push(229);
				goto case 217;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.kind == 22) {
					currentState = 228;
					break;
				} else {
					goto case 212;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 237;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 135 || la.kind == 149) {
					if (la.kind == 135) {
						currentState = 235;
						break;
					} else {
						if (la.kind == 149) {
							goto case 230;
						} else {
							Error(la);
							goto case 231;
						}
					}
				} else {
					goto case 232;
				}
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				Expect(145, la); // "Into"
				currentState = 233;
				break;
			}
			case 233: {
				stateStack.Push(234);
				goto case 217;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 22) {
					currentState = 233;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 235: {
				stateStack.Push(236);
				goto case 237;
			}
			case 236: {
				stateStack.Push(231);
				goto case 232;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(149, la); // "Join"
				currentState = 197;
				break;
			}
			case 238: {
				stateStack.Push(239);
				goto case 217;
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
				if (la.kind == 234) {
					currentState = 55;
					break;
				} else {
					goto case 55;
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				Expect(72, la); // "By"
				currentState = 243;
				break;
			}
			case 243: {
				stateStack.Push(244);
				goto case 55;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 64) {
					currentState = 245;
					break;
				} else {
					if (la.kind == 106) {
						currentState = 245;
						break;
					} else {
						Error(la);
						goto case 245;
					}
				}
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (la.kind == 22) {
					currentState = 243;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 246: {
				stateStack.Push(247);
				goto case 217;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 22) {
					currentState = 246;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 248: {
				stateStack.Push(249);
				goto case 203;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (set[38].Get(la.kind)) {
					stateStack.Push(249);
					goto case 196;
				} else {
					Expect(145, la); // "Into"
					currentState = 250;
					break;
				}
			}
			case 250: {
				stateStack.Push(251);
				goto case 217;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 22) {
					currentState = 250;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 252: {
				stateStack.Push(253);
				goto case 203;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (la.kind == 22) {
					currentState = 252;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				Expect(58, la); // "Aggregate"
				currentState = 248;
				break;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				Expect(128, la); // "From"
				currentState = 252;
				break;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (la.kind == 66 || la.kind == 148) {
					currentState = 256;
					break;
				} else {
					if (la.kind == 213) {
						stateStack.Push(257);
						goto case 463;
					} else {
						if (la.kind == 129) {
							stateStack.Push(257);
							goto case 258;
						} else {
							Error(la);
							goto case 257;
						}
					}
				}
			}
			case 257: {
				lambdaNestingDepth--;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(129, la); // "Function"
				currentState = 259;
				break;
			}
			case 259: {
				stateStack.Push(260);
				goto case 435;
			}
			case 260: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 433;
							break;
						} else {
							goto case 262;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 262: {
				stateStack.Push(263);
				goto case 265;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(115, la); // "End"
				currentState = 264;
				break;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				Expect(129, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 265: {
				PushContext(Context.Body, la, t);
				goto case 266;
			}
			case 266: {
				stateStack.Push(267);
				goto case 23;
			}
			case 267: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[141].Get(la.kind)) {
					if (set[71].Get(la.kind)) {
						if (set[51].Get(la.kind)) {
							stateStack.Push(266);
							goto case 273;
						} else {
							goto case 266;
						}
					} else {
						if (la.kind == 115) {
							currentState = 271;
							break;
						} else {
							goto case 270;
						}
					}
				} else {
					goto case 269;
				}
			}
			case 269: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 270: {
				Error(la);
				goto case 267;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 266;
				} else {
					if (set[50].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 270;
					}
				}
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				currentState = 267;
				break;
			}
			case 273: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (la.kind == 90 || la.kind == 107 || la.kind == 207) {
					currentState = 410;
					break;
				} else {
					if (la.kind == 214 || la.kind == 236) {
						currentState = 406;
						break;
					} else {
						if (la.kind == 56 || la.kind == 196) {
							currentState = 404;
							break;
						} else {
							if (la.kind == 192) {
								currentState = 399;
								break;
							} else {
								if (la.kind == 137) {
									currentState = 381;
									break;
								} else {
									if (la.kind == 200) {
										currentState = 362;
										break;
									} else {
										if (la.kind == 234) {
											currentState = 358;
											break;
										} else {
											if (la.kind == 110) {
												currentState = 352;
												break;
											} else {
												if (la.kind == 126) {
													currentState = 325;
													break;
												} else {
													if (la.kind == 120 || la.kind == 174 || la.kind == 197) {
														if (la.kind == 120 || la.kind == 174) {
															if (la.kind == 174) {
																currentState = 321;
																break;
															} else {
																goto case 321;
															}
														} else {
															if (la.kind == 197) {
																currentState = 319;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 218) {
															currentState = 317;
															break;
														} else {
															if (la.kind == 221) {
																currentState = 304;
																break;
															} else {
																if (set[142].Get(la.kind)) {
																	if (la.kind == 134) {
																		currentState = 301;
																		break;
																	} else {
																		if (la.kind == 122) {
																			currentState = 300;
																			break;
																		} else {
																			if (la.kind == 91) {
																				currentState = 299;
																				break;
																			} else {
																				if (la.kind == 209) {
																					goto case 113;
																				} else {
																					if (la.kind == 198) {
																						currentState = 296;
																						break;
																					} else {
																						if (la.kind == 240) {
																							currentState = 294;
																							break;
																						} else {
																							goto case 6;
																						}
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 194) {
																		currentState = 292;
																		break;
																	} else {
																		if (la.kind == 119) {
																			currentState = 290;
																			break;
																		} else {
																			if (la.kind == 229) {
																				currentState = 275;
																				break;
																			} else {
																				if (set[143].Get(la.kind)) {
																					if (la.kind == 75) {
																						currentState = 55;
																						break;
																					} else {
																						goto case 55;
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
			case 275: {
				stateStack.Push(276);
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 279;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (la.kind == 22) {
					currentState = 275;
					break;
				} else {
					stateStack.Push(277);
					goto case 265;
				}
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				Expect(115, la); // "End"
				currentState = 278;
				break;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				Expect(229, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(282);
					goto case 209;
				} else {
					goto case 280;
				}
			}
			case 280: {
				stateStack.Push(281);
				goto case 55;
			}
			case 281: {
				if (!isAlreadyInExpr) PopContext(); isAlreadyInExpr = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 282: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[42].Get(la.kind)) {
					PopContext(); isAlreadyInExpr = true;
					goto case 284;
				} else {
					goto case 280;
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 63) {
					currentState = 286;
					break;
				} else {
					if (la.kind == 20) {
						currentState = 280;
						break;
					} else {
						if (set[43].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 280;
						}
					}
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				currentState = 280;
				break;
			}
			case 286: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(288);
					goto case 37;
				} else {
					goto case 280;
				}
			}
			case 288: {
				PopContext();
				goto case 289;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				Expect(20, la); // "="
				currentState = 280;
				break;
			}
			case 290: {
				stateStack.Push(291);
				goto case 55;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 293;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 187) {
					currentState = 55;
					break;
				} else {
					goto case 55;
				}
			}
			case 294: {
				PushContext(Context.Expression, la, t);
				stateStack.Push(295);
				goto case 55;
			}
			case 295: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 296: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 297;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(298);
					goto case 55;
				} else {
					goto case 298;
				}
			}
			case 298: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 110) {
					goto case 132;
				} else {
					if (la.kind == 126) {
						goto case 129;
					} else {
						if (la.kind == 234) {
							goto case 103;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 110) {
					goto case 132;
				} else {
					if (la.kind == 126) {
						goto case 129;
					} else {
						if (la.kind == 234) {
							goto case 103;
						} else {
							if (la.kind == 200) {
								goto case 115;
							} else {
								if (la.kind == 213) {
									goto case 111;
								} else {
									if (la.kind == 129) {
										goto case 127;
									} else {
										if (la.kind == 189) {
											goto case 116;
										} else {
											if (la.kind == 221) {
												goto case 107;
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
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (set[6].Get(la.kind)) {
					goto case 303;
				} else {
					if (la.kind == 5) {
						goto case 302;
					} else {
						goto case 6;
					}
				}
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				stateStack.Push(305);
				goto case 265;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 77) {
					currentState = 309;
					break;
				} else {
					if (la.kind == 125) {
						currentState = 308;
						break;
					} else {
						goto case 306;
					}
				}
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(115, la); // "End"
				currentState = 307;
				break;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				Expect(221, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 308: {
				stateStack.Push(306);
				goto case 265;
			}
			case 309: {
				SetIdentifierExpected(la);
				goto case 310;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(313);
					goto case 209;
				} else {
					goto case 311;
				}
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 232) {
					currentState = 312;
					break;
				} else {
					goto case 304;
				}
			}
			case 312: {
				stateStack.Push(304);
				goto case 55;
			}
			case 313: {
				PopContext();
				goto case 314;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 63) {
					currentState = 315;
					break;
				} else {
					goto case 311;
				}
			}
			case 315: {
				PushContext(Context.Type, la, t);
				stateStack.Push(316);
				goto case 37;
			}
			case 316: {
				PopContext();
				goto case 311;
			}
			case 317: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 318;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 166) {
					goto case 120;
				} else {
					goto case 320;
				}
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 5) {
					goto case 302;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 303;
					} else {
						goto case 6;
					}
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				Expect(120, la); // "Error"
				currentState = 322;
				break;
			}
			case 322: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 323;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (set[23].Get(la.kind)) {
					goto case 55;
				} else {
					if (la.kind == 134) {
						currentState = 320;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 324;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				Expect(166, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 325: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 326;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(342);
					goto case 336;
				} else {
					if (la.kind == 112) {
						currentState = 327;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 327: {
				stateStack.Push(328);
				goto case 336;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				Expect(140, la); // "In"
				currentState = 329;
				break;
			}
			case 329: {
				stateStack.Push(330);
				goto case 55;
			}
			case 330: {
				stateStack.Push(331);
				goto case 265;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(166, la); // "Next"
				currentState = 332;
				break;
			}
			case 332: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 333;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (set[23].Get(la.kind)) {
					goto case 334;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 334: {
				stateStack.Push(335);
				goto case 55;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 22) {
					currentState = 334;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 336: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(337);
				goto case 177;
			}
			case 337: {
				PopContext();
				goto case 338;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 33) {
					currentState = 339;
					break;
				} else {
					goto case 339;
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(339);
					goto case 164;
				} else {
					if (la.kind == 63) {
						currentState = 340;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 340: {
				PushContext(Context.Type, la, t);
				stateStack.Push(341);
				goto case 37;
			}
			case 341: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				Expect(20, la); // "="
				currentState = 343;
				break;
			}
			case 343: {
				stateStack.Push(344);
				goto case 55;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 208) {
					currentState = 351;
					break;
				} else {
					goto case 345;
				}
			}
			case 345: {
				stateStack.Push(346);
				goto case 265;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				Expect(166, la); // "Next"
				currentState = 347;
				break;
			}
			case 347: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 348;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (set[23].Get(la.kind)) {
					goto case 349;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 349: {
				stateStack.Push(350);
				goto case 55;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 22) {
					currentState = 349;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 351: {
				stateStack.Push(345);
				goto case 55;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 227 || la.kind == 234) {
					currentState = 355;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(353);
						goto case 265;
					} else {
						goto case 6;
					}
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(155, la); // "Loop"
				currentState = 354;
				break;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 227 || la.kind == 234) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 355: {
				stateStack.Push(356);
				goto case 55;
			}
			case 356: {
				stateStack.Push(357);
				goto case 265;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(155, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 358: {
				stateStack.Push(359);
				goto case 55;
			}
			case 359: {
				stateStack.Push(360);
				goto case 265;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				Expect(115, la); // "End"
				currentState = 361;
				break;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(234, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 362: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 363;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 76) {
					currentState = 364;
					break;
				} else {
					goto case 364;
				}
			}
			case 364: {
				stateStack.Push(365);
				goto case 55;
			}
			case 365: {
				stateStack.Push(366);
				goto case 23;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 76) {
					currentState = 368;
					break;
				} else {
					Expect(115, la); // "End"
					currentState = 367;
					break;
				}
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				Expect(200, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 368: {
				xmlAllowed = false;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 369;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 113) {
					currentState = 370;
					break;
				} else {
					if (set[68].Get(la.kind)) {
						if (set[69].Get(la.kind)) {
							xmlAllowed = true;
							goto case 376;
						} else {
							if (set[23].Get(la.kind)) {
								xmlAllowed = true;
								goto case 375;
							} else {
								goto case 371;
							}
						}
					} else {
						Error(la);
						goto case 370;
					}
				}
			}
			case 370: {
				stateStack.Push(366);
				goto case 265;
			}
			case 371: {
				Error(la);
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 22) {
					currentState = 373;
					break;
				} else {
					goto case 370;
				}
			}
			case 373: {
				xmlAllowed = false;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 374;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (set[69].Get(la.kind)) {
					xmlAllowed = true;
					goto case 376;
				} else {
					if (set[23].Get(la.kind)) {
						xmlAllowed = true;
						goto case 375;
					} else {
						goto case 371;
					}
				}
			}
			case 375: {
				stateStack.Push(372);
				goto case 55;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 146) {
					currentState = 377;
					break;
				} else {
					goto case 377;
				}
			}
			case 377: {
				stateStack.Push(378);
				goto case 379;
			}
			case 378: {
				stateStack.Push(372);
				goto case 75;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 20) {
					goto case 73;
				} else {
					if (la.kind == 41) {
						goto case 72;
					} else {
						if (la.kind == 40) {
							goto case 71;
						} else {
							if (la.kind == 39) {
								currentState = 380;
								break;
							} else {
								if (la.kind == 42) {
									goto case 68;
								} else {
									if (la.kind == 43) {
										goto case 69;
									} else {
										goto case 6;
									}
								}
							}
						}
					}
				}
			}
			case 380: {
				wasNormalAttribute = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 381: {
				stateStack.Push(382);
				goto case 55;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 217) {
					currentState = 390;
					break;
				} else {
					goto case 383;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 384;
				} else {
					goto case 6;
				}
			}
			case 384: {
				stateStack.Push(385);
				goto case 265;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 113 || la.kind == 114) {
					if (la.kind == 113) {
						currentState = 389;
						break;
					} else {
						if (la.kind == 114) {
							currentState = 387;
							break;
						} else {
							Error(la);
							goto case 384;
						}
					}
				} else {
					Expect(115, la); // "End"
					currentState = 386;
					break;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(137, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 387: {
				stateStack.Push(388);
				goto case 55;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 217) {
					currentState = 384;
					break;
				} else {
					goto case 384;
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 137) {
					currentState = 387;
					break;
				} else {
					goto case 384;
				}
			}
			case 390: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (set[51].Get(la.kind)) {
					goto case 392;
				} else {
					goto case 383;
				}
			}
			case 392: {
				stateStack.Push(393);
				goto case 273;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 21) {
					currentState = 397;
					break;
				} else {
					if (la.kind == 113) {
						currentState = 394;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 394: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 395;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (set[51].Get(la.kind)) {
					stateStack.Push(396);
					goto case 273;
				} else {
					goto case 396;
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 21) {
					currentState = 394;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 397: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 398;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (set[51].Get(la.kind)) {
					goto case 392;
				} else {
					goto case 393;
				}
			}
			case 399: {
				stateStack.Push(400);
				goto case 101;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 37) {
					currentState = 401;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 401: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 402;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(403);
					goto case 47;
				} else {
					goto case 403;
				}
			}
			case 403: {
				PopContext();
				goto case 46;
			}
			case 404: {
				stateStack.Push(405);
				goto case 55;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(22, la); // ","
				currentState = 55;
				break;
			}
			case 406: {
				stateStack.Push(407);
				goto case 55;
			}
			case 407: {
				stateStack.Push(408);
				goto case 265;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(115, la); // "End"
				currentState = 409;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 236) {
					goto case 102;
				} else {
					if (la.kind == 214) {
						goto case 110;
					} else {
						goto case 6;
					}
				}
			}
			case 410: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(411);
				goto case 209;
			}
			case 411: {
				PopContext();
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 33) {
					currentState = 413;
					break;
				} else {
					goto case 413;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (la.kind == 37) {
					currentState = 430;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 427;
						break;
					} else {
						goto case 414;
					}
				}
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 20) {
					currentState = 426;
					break;
				} else {
					goto case 415;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 22) {
					currentState = 416;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 416: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(417);
				goto case 209;
			}
			case 417: {
				PopContext();
				goto case 418;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 33) {
					currentState = 419;
					break;
				} else {
					goto case 419;
				}
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (la.kind == 37) {
					currentState = 423;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 420;
						break;
					} else {
						goto case 414;
					}
				}
			}
			case 420: {
				PushContext(Context.Type, la, t);
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 165) {
					stateStack.Push(422);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(422);
						goto case 37;
					} else {
						Error(la);
						goto case 422;
					}
				}
			}
			case 422: {
				PopContext();
				goto case 414;
			}
			case 423: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 424;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(425);
					goto case 55;
				} else {
					goto case 425;
				}
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 22) {
					currentState = 423;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 419;
					break;
				}
			}
			case 426: {
				stateStack.Push(415);
				goto case 55;
			}
			case 427: {
				PushContext(Context.Type, la, t);
				goto case 428;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 165) {
					stateStack.Push(429);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(429);
						goto case 37;
					} else {
						Error(la);
						goto case 429;
					}
				}
			}
			case 429: {
				PopContext();
				goto case 414;
			}
			case 430: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 431;
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(432);
					goto case 55;
				} else {
					goto case 432;
				}
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 22) {
					currentState = 430;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 413;
					break;
				}
			}
			case 433: {
				PushContext(Context.Type, la, t);
				stateStack.Push(434);
				goto case 37;
			}
			case 434: {
				PopContext();
				goto case 262;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				Expect(37, la); // "("
				currentState = 436;
				break;
			}
			case 436: {
				PushContext(Context.Default, la, t);
				SetIdentifierExpected(la);
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (set[79].Get(la.kind)) {
					stateStack.Push(438);
					goto case 439;
				} else {
					goto case 438;
				}
			}
			case 438: {
				PopContext();
				goto case 46;
			}
			case 439: {
				stateStack.Push(440);
				PushContext(Context.Parameter, la, t);
				goto case 441;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 22) {
					currentState = 439;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 441: {
				SetIdentifierExpected(la);
				goto case 442;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 40) {
					stateStack.Push(441);
					goto case 454;
				} else {
					goto case 443;
				}
			}
			case 443: {
				SetIdentifierExpected(la);
				goto case 444;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[144].Get(la.kind)) {
					currentState = 443;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(445);
					goto case 209;
				}
			}
			case 445: {
				PopContext();
				goto case 446;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 33) {
					currentState = 447;
					break;
				} else {
					goto case 447;
				}
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (la.kind == 37) {
					currentState = 453;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 451;
						break;
					} else {
						goto case 448;
					}
				}
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 20) {
					currentState = 450;
					break;
				} else {
					goto case 449;
				}
			}
			case 449: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 450: {
				stateStack.Push(449);
				goto case 55;
			}
			case 451: {
				PushContext(Context.Type, la, t);
				stateStack.Push(452);
				goto case 37;
			}
			case 452: {
				PopContext();
				goto case 448;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 22) {
					currentState = 453;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 447;
					break;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(40, la); // "<"
				currentState = 455;
				break;
			}
			case 455: {
				wasNormalAttribute = true; PushContext(Context.Attribute, la, t);
				goto case 456;
			}
			case 456: {
				stateStack.Push(457);
				goto case 460;
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 22) {
					currentState = 456;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 458;
					break;
				}
			}
			case 458: {
				PopContext();
				goto case 459;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 65 || la.kind == 158) {
					currentState = 461;
					break;
				} else {
					goto case 37;
				}
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				Expect(21, la); // ":"
				currentState = 462;
				break;
			}
			case 462: {
				wasNormalAttribute = false;
				goto case 37;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(213, la); // "Sub"
				currentState = 464;
				break;
			}
			case 464: {
				stateStack.Push(465);
				goto case 435;
			}
			case 465: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 466;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (set[51].Get(la.kind)) {
					goto case 273;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(467);
						goto case 265;
					} else {
						goto case 6;
					}
				}
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				Expect(115, la); // "End"
				currentState = 468;
				break;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				Expect(213, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 482;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(471);
						goto case 473;
					} else {
						Error(la);
						goto case 470;
					}
				}
			}
			case 470: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (la.kind == 17) {
					currentState = 472;
					break;
				} else {
					goto case 470;
				}
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 16) {
					currentState = 471;
					break;
				} else {
					goto case 471;
				}
			}
			case 473: {
				PushContext(Context.Xml, la, t);
				goto case 474;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 475;
				break;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[145].Get(la.kind)) {
					if (set[146].Get(la.kind)) {
						currentState = 475;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(475);
							goto case 479;
						} else {
							Error(la);
							goto case 475;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 476;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 477;
							break;
						} else {
							Error(la);
							goto case 476;
						}
					}
				}
			}
			case 476: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				if (set[147].Get(la.kind)) {
					if (set[148].Get(la.kind)) {
						currentState = 477;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(477);
							goto case 479;
						} else {
							if (la.kind == 10) {
								stateStack.Push(477);
								goto case 473;
							} else {
								Error(la);
								goto case 477;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 478;
					break;
				}
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (set[149].Get(la.kind)) {
					if (set[150].Get(la.kind)) {
						currentState = 478;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(478);
							goto case 479;
						} else {
							Error(la);
							goto case 478;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 476;
					break;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 480;
				break;
			}
			case 480: {
				stateStack.Push(481);
				goto case 55;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (la.kind == 16) {
					currentState = 483;
					break;
				} else {
					goto case 483;
				}
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 482;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(484);
						goto case 473;
					} else {
						goto case 470;
					}
				}
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 17) {
					currentState = 485;
					break;
				} else {
					goto case 470;
				}
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (la.kind == 16) {
					currentState = 484;
					break;
				} else {
					goto case 484;
				}
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				Expect(37, la); // "("
				currentState = 487;
				break;
			}
			case 487: {
				readXmlIdentifier = true;
				stateStack.Push(488);
				goto case 209;
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				Expect(38, la); // ")"
				currentState = 179;
				break;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				Expect(37, la); // "("
				currentState = 490;
				break;
			}
			case 490: {
				PushContext(Context.Type, la, t);
				stateStack.Push(491);
				goto case 37;
			}
			case 491: {
				PopContext();
				goto case 488;
			}
			case 492: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 493;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 10) {
					currentState = 494;
					break;
				} else {
					goto case 494;
				}
			}
			case 494: {
				stateStack.Push(495);
				goto case 101;
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				if (la.kind == 11) {
					currentState = 179;
					break;
				} else {
					goto case 179;
				}
			}
			case 496: {
				activeArgument = 0;
				goto case 497;
			}
			case 497: {
				stateStack.Push(498);
				goto case 55;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (la.kind == 22) {
					currentState = 499;
					break;
				} else {
					goto case 488;
				}
			}
			case 499: {
				activeArgument++;
				goto case 497;
			}
			case 500: {
				stateStack.Push(501);
				goto case 55;
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				if (la.kind == 22) {
					currentState = 502;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 502: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 503;
			}
			case 503: {
				if (la == null) { currentState = 503; break; }
				if (set[23].Get(la.kind)) {
					goto case 500;
				} else {
					goto case 501;
				}
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(508);
					goto case 37;
				} else {
					goto case 505;
				}
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 22) {
					currentState = 506;
					break;
				} else {
					goto case 45;
				}
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(507);
					goto case 37;
				} else {
					goto case 505;
				}
			}
			case 507: {
				PopContext();
				goto case 505;
			}
			case 508: {
				PopContext();
				goto case 505;
			}
			case 509: {
				SetIdentifierExpected(la);
				goto case 510;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (set[151].Get(la.kind)) {
					if (la.kind == 172) {
						currentState = 512;
						break;
					} else {
						if (set[79].Get(la.kind)) {
							stateStack.Push(511);
							goto case 439;
						} else {
							Error(la);
							goto case 511;
						}
					}
				} else {
					goto case 511;
				}
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 512: {
				stateStack.Push(511);
				goto case 513;
			}
			case 513: {
				SetIdentifierExpected(la);
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 140 || la.kind == 181) {
					currentState = 515;
					break;
				} else {
					goto case 515;
				}
			}
			case 515: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(516);
				goto case 530;
			}
			case 516: {
				PopContext();
				goto case 517;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				if (la.kind == 63) {
					currentState = 531;
					break;
				} else {
					goto case 518;
				}
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				if (la.kind == 22) {
					currentState = 519;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 519: {
				SetIdentifierExpected(la);
				goto case 520;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 140 || la.kind == 181) {
					currentState = 521;
					break;
				} else {
					goto case 521;
				}
			}
			case 521: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(522);
				goto case 530;
			}
			case 522: {
				PopContext();
				goto case 523;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				if (la.kind == 63) {
					currentState = 524;
					break;
				} else {
					goto case 518;
				}
			}
			case 524: {
				PushContext(Context.Type, la, t);
				stateStack.Push(525);
				goto case 526;
			}
			case 525: {
				PopContext();
				goto case 518;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (set[96].Get(la.kind)) {
					goto case 529;
				} else {
					if (la.kind == 35) {
						currentState = 527;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 527: {
				stateStack.Push(528);
				goto case 529;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (la.kind == 22) {
					currentState = 527;
					break;
				} else {
					goto case 82;
				}
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 165) {
						goto case 121;
					} else {
						if (la.kind == 86) {
							goto case 137;
						} else {
							if (la.kind == 212) {
								goto case 112;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (la.kind == 2) {
					goto case 149;
				} else {
					if (la.kind == 62) {
						goto case 143;
					} else {
						if (la.kind == 64) {
							goto case 142;
						} else {
							if (la.kind == 65) {
								goto case 141;
							} else {
								if (la.kind == 67) {
									goto case 140;
								} else {
									if (la.kind == 69) {
										goto case 139;
									} else {
										if (la.kind == 72) {
											goto case 138;
										} else {
											if (la.kind == 89) {
												goto case 136;
											} else {
												if (la.kind == 106) {
													goto case 134;
												} else {
													if (la.kind == 109) {
														goto case 133;
													} else {
														if (la.kind == 118) {
															goto case 131;
														} else {
															if (la.kind == 123) {
																goto case 130;
															} else {
																if (la.kind == 135) {
																	goto case 126;
																} else {
																	if (la.kind == 141) {
																		goto case 125;
																	} else {
																		if (la.kind == 145) {
																			goto case 124;
																		} else {
																			if (la.kind == 149) {
																				goto case 123;
																			} else {
																				if (la.kind == 150) {
																					goto case 122;
																				} else {
																					if (la.kind == 173) {
																						goto case 119;
																					} else {
																						if (la.kind == 179) {
																							goto case 118;
																						} else {
																							if (la.kind == 187) {
																								goto case 117;
																							} else {
																								if (la.kind == 206) {
																									goto case 114;
																								} else {
																									if (la.kind == 215) {
																										goto case 109;
																									} else {
																										if (la.kind == 216) {
																											goto case 108;
																										} else {
																											if (la.kind == 226) {
																												goto case 106;
																											} else {
																												if (la.kind == 227) {
																													goto case 105;
																												} else {
																													if (la.kind == 233) {
																														goto case 104;
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
			case 531: {
				PushContext(Context.Type, la, t);
				stateStack.Push(532);
				goto case 526;
			}
			case 532: {
				PopContext();
				goto case 518;
			}
			case 533: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(534);
				goto case 209;
			}
			case 534: {
				PopContext();
				goto case 535;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				if (la.kind == 37) {
					stateStack.Push(536);
					goto case 435;
				} else {
					goto case 536;
				}
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 63) {
					currentState = 537;
					break;
				} else {
					goto case 23;
				}
			}
			case 537: {
				PushContext(Context.Type, la, t);
				goto case 538;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 40) {
					stateStack.Push(538);
					goto case 454;
				} else {
					stateStack.Push(539);
					goto case 37;
				}
			}
			case 539: {
				PopContext();
				goto case 23;
			}
			case 540: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(541);
				goto case 209;
			}
			case 541: {
				PopContext();
				goto case 542;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 544;
						break;
					} else {
						if (la.kind == 37) {
							stateStack.Push(23);
							goto case 435;
						} else {
							goto case 543;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 543: {
				Error(la);
				goto case 23;
			}
			case 544: {
				PushContext(Context.Type, la, t);
				stateStack.Push(545);
				goto case 37;
			}
			case 545: {
				PopContext();
				goto case 23;
			}
			case 546: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 547;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				Expect(117, la); // "Enum"
				currentState = 548;
				break;
			}
			case 548: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(549);
				goto case 209;
			}
			case 549: {
				PopContext();
				goto case 550;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 63) {
					currentState = 562;
					break;
				} else {
					goto case 551;
				}
			}
			case 551: {
				stateStack.Push(552);
				goto case 23;
			}
			case 552: {
				SetIdentifierExpected(la);
				goto case 553;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				if (set[99].Get(la.kind)) {
					goto case 557;
				} else {
					Expect(115, la); // "End"
					currentState = 554;
					break;
				}
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				Expect(117, la); // "Enum"
				currentState = 555;
				break;
			}
			case 555: {
				stateStack.Push(556);
				goto case 23;
			}
			case 556: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 557: {
				SetIdentifierExpected(la);
				goto case 558;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 40) {
					stateStack.Push(557);
					goto case 454;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(559);
					goto case 209;
				}
			}
			case 559: {
				PopContext();
				goto case 560;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (la.kind == 20) {
					currentState = 561;
					break;
				} else {
					goto case 551;
				}
			}
			case 561: {
				stateStack.Push(551);
				goto case 55;
			}
			case 562: {
				PushContext(Context.Type, la, t);
				stateStack.Push(563);
				goto case 37;
			}
			case 563: {
				PopContext();
				goto case 551;
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(105, la); // "Delegate"
				currentState = 565;
				break;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				if (la.kind == 213) {
					currentState = 566;
					break;
				} else {
					if (la.kind == 129) {
						currentState = 566;
						break;
					} else {
						Error(la);
						goto case 566;
					}
				}
			}
			case 566: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 567;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				currentState = 568;
				break;
			}
			case 568: {
				PopContext();
				goto case 569;
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				if (la.kind == 37) {
					currentState = 572;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 570;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 570: {
				PushContext(Context.Type, la, t);
				stateStack.Push(571);
				goto case 37;
			}
			case 571: {
				PopContext();
				goto case 23;
			}
			case 572: {
				SetIdentifierExpected(la);
				goto case 573;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (set[151].Get(la.kind)) {
					if (la.kind == 172) {
						currentState = 575;
						break;
					} else {
						if (set[79].Get(la.kind)) {
							stateStack.Push(574);
							goto case 439;
						} else {
							Error(la);
							goto case 574;
						}
					}
				} else {
					goto case 574;
				}
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				Expect(38, la); // ")"
				currentState = 569;
				break;
			}
			case 575: {
				stateStack.Push(574);
				goto case 513;
			}
			case 576: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 577;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				if (la.kind == 158) {
					currentState = 578;
					break;
				} else {
					if (la.kind == 86) {
						currentState = 578;
						break;
					} else {
						if (la.kind == 212) {
							currentState = 578;
							break;
						} else {
							Error(la);
							goto case 578;
						}
					}
				}
			}
			case 578: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(579);
				goto case 209;
			}
			case 579: {
				PopContext();
				goto case 580;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 37) {
					currentState = 725;
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
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 142) {
					isMissingModifier = false;
					goto case 722;
				} else {
					goto case 584;
				}
			}
			case 584: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 585;
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (la.kind == 138) {
					isMissingModifier = false;
					goto case 716;
				} else {
					goto case 586;
				}
			}
			case 586: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 587;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				if (set[103].Get(la.kind)) {
					goto case 592;
				} else {
					isMissingModifier = false;
					goto case 588;
				}
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				Expect(115, la); // "End"
				currentState = 589;
				break;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 158) {
					currentState = 590;
					break;
				} else {
					if (la.kind == 86) {
						currentState = 590;
						break;
					} else {
						if (la.kind == 212) {
							currentState = 590;
							break;
						} else {
							Error(la);
							goto case 590;
						}
					}
				}
			}
			case 590: {
				stateStack.Push(591);
				goto case 23;
			}
			case 591: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 592: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 593;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				if (la.kind == 40) {
					stateStack.Push(592);
					goto case 454;
				} else {
					isMissingModifier = true;
					goto case 594;
				}
			}
			case 594: {
				SetIdentifierExpected(la);
				goto case 595;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (set[134].Get(la.kind)) {
					currentState = 715;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 596;
				}
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (la.kind == 86 || la.kind == 158 || la.kind == 212) {
					stateStack.Push(586);
					goto case 576;
				} else {
					if (la.kind == 105) {
						stateStack.Push(586);
						goto case 564;
					} else {
						if (la.kind == 117) {
							stateStack.Push(586);
							goto case 546;
						} else {
							if (la.kind == 144) {
								stateStack.Push(586);
								goto case 9;
							} else {
								if (set[106].Get(la.kind)) {
									stateStack.Push(586);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 597;
								} else {
									Error(la);
									goto case 586;
								}
							}
						}
					}
				}
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				if (set[124].Get(la.kind)) {
					stateStack.Push(598);
					goto case 700;
				} else {
					if (la.kind == 129 || la.kind == 213) {
						stateStack.Push(598);
						goto case 680;
					} else {
						if (la.kind == 103) {
							stateStack.Push(598);
							goto case 667;
						} else {
							if (la.kind == 121) {
								stateStack.Push(598);
								goto case 660;
							} else {
								if (la.kind == 100) {
									stateStack.Push(598);
									goto case 648;
								} else {
									if (la.kind == 189) {
										stateStack.Push(598);
										goto case 613;
									} else {
										if (la.kind == 175) {
											stateStack.Push(598);
											goto case 599;
										} else {
											Error(la);
											goto case 598;
										}
									}
								}
							}
						}
					}
				}
			}
			case 598: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 599: {
				if (la == null) { currentState = 599; break; }
				Expect(175, la); // "Operator"
				currentState = 600;
				break;
			}
			case 600: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 601;
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				currentState = 602;
				break;
			}
			case 602: {
				PopContext();
				goto case 603;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(37, la); // "("
				currentState = 604;
				break;
			}
			case 604: {
				stateStack.Push(605);
				goto case 439;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				Expect(38, la); // ")"
				currentState = 606;
				break;
			}
			case 606: {
				if (la == null) { currentState = 606; break; }
				if (la.kind == 63) {
					currentState = 610;
					break;
				} else {
					goto case 607;
				}
			}
			case 607: {
				stateStack.Push(608);
				goto case 265;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				Expect(115, la); // "End"
				currentState = 609;
				break;
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				Expect(175, la); // "Operator"
				currentState = 23;
				break;
			}
			case 610: {
				PushContext(Context.Type, la, t);
				goto case 611;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				if (la.kind == 40) {
					stateStack.Push(611);
					goto case 454;
				} else {
					stateStack.Push(612);
					goto case 37;
				}
			}
			case 612: {
				PopContext();
				goto case 607;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				Expect(189, la); // "Property"
				currentState = 614;
				break;
			}
			case 614: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(615);
				goto case 209;
			}
			case 615: {
				PopContext();
				goto case 616;
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				if (la.kind == 37) {
					stateStack.Push(617);
					goto case 435;
				} else {
					goto case 617;
				}
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 63) {
					currentState = 645;
					break;
				} else {
					goto case 618;
				}
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				if (la.kind == 138) {
					stateStack.Push(619);
					goto case 642;
				} else {
					goto case 619;
				}
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				if (la.kind == 20) {
					currentState = 641;
					break;
				} else {
					goto case 620;
				}
			}
			case 620: {
				stateStack.Push(621);
				goto case 23;
			}
			case 621: {
				PopContext();
				goto case 622;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				if (la.kind == 40) {
					stateStack.Push(622);
					goto case 454;
				} else {
					goto case 623;
				}
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				if (set[152].Get(la.kind)) {
					currentState = 640;
					break;
				} else {
					if (la.kind == 130 || la.kind == 201) {
						PushContext(Context.Member, la, t);
						goto case 624;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				if (la.kind == 130) {
					currentState = 625;
					break;
				} else {
					if (la.kind == 201) {
						currentState = 625;
						break;
					} else {
						Error(la);
						goto case 625;
					}
				}
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 37) {
					stateStack.Push(626);
					goto case 435;
				} else {
					goto case 626;
				}
			}
			case 626: {
				stateStack.Push(627);
				goto case 265;
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				Expect(115, la); // "End"
				currentState = 628;
				break;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				if (la.kind == 130) {
					currentState = 629;
					break;
				} else {
					if (la.kind == 201) {
						currentState = 629;
						break;
					} else {
						Error(la);
						goto case 629;
					}
				}
			}
			case 629: {
				stateStack.Push(630);
				goto case 23;
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				if (set[112].Get(la.kind)) {
					goto case 633;
				} else {
					goto case 631;
				}
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				Expect(115, la); // "End"
				currentState = 632;
				break;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				Expect(189, la); // "Property"
				currentState = 23;
				break;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				if (la.kind == 40) {
					stateStack.Push(633);
					goto case 454;
				} else {
					goto case 634;
				}
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (set[152].Get(la.kind)) {
					currentState = 634;
					break;
				} else {
					if (la.kind == 130) {
						currentState = 635;
						break;
					} else {
						if (la.kind == 201) {
							currentState = 635;
							break;
						} else {
							Error(la);
							goto case 635;
						}
					}
				}
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 37) {
					stateStack.Push(636);
					goto case 435;
				} else {
					goto case 636;
				}
			}
			case 636: {
				stateStack.Push(637);
				goto case 265;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(115, la); // "End"
				currentState = 638;
				break;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 130) {
					currentState = 639;
					break;
				} else {
					if (la.kind == 201) {
						currentState = 639;
						break;
					} else {
						Error(la);
						goto case 639;
					}
				}
			}
			case 639: {
				stateStack.Push(631);
				goto case 23;
			}
			case 640: {
				SetIdentifierExpected(la);
				goto case 623;
			}
			case 641: {
				stateStack.Push(620);
				goto case 55;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				Expect(138, la); // "Implements"
				currentState = 643;
				break;
			}
			case 643: {
				stateStack.Push(644);
				goto case 37;
			}
			case 644: {
				if (la == null) { currentState = 644; break; }
				if (la.kind == 22) {
					currentState = 643;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 645: {
				PushContext(Context.Type, la, t);
				goto case 646;
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				if (la.kind == 40) {
					stateStack.Push(646);
					goto case 454;
				} else {
					if (la.kind == 165) {
						stateStack.Push(647);
						goto case 85;
					} else {
						if (set[16].Get(la.kind)) {
							stateStack.Push(647);
							goto case 37;
						} else {
							Error(la);
							goto case 647;
						}
					}
				}
			}
			case 647: {
				PopContext();
				goto case 618;
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				Expect(100, la); // "Custom"
				currentState = 649;
				break;
			}
			case 649: {
				stateStack.Push(650);
				goto case 660;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (set[117].Get(la.kind)) {
					goto case 652;
				} else {
					Expect(115, la); // "End"
					currentState = 651;
					break;
				}
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				Expect(121, la); // "Event"
				currentState = 23;
				break;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				if (la.kind == 40) {
					stateStack.Push(652);
					goto case 454;
				} else {
					if (la.kind == 56) {
						currentState = 653;
						break;
					} else {
						if (la.kind == 196) {
							currentState = 653;
							break;
						} else {
							if (la.kind == 192) {
								currentState = 653;
								break;
							} else {
								Error(la);
								goto case 653;
							}
						}
					}
				}
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				Expect(37, la); // "("
				currentState = 654;
				break;
			}
			case 654: {
				stateStack.Push(655);
				goto case 439;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				Expect(38, la); // ")"
				currentState = 656;
				break;
			}
			case 656: {
				stateStack.Push(657);
				goto case 265;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				Expect(115, la); // "End"
				currentState = 658;
				break;
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				if (la.kind == 56) {
					currentState = 659;
					break;
				} else {
					if (la.kind == 196) {
						currentState = 659;
						break;
					} else {
						if (la.kind == 192) {
							currentState = 659;
							break;
						} else {
							Error(la);
							goto case 659;
						}
					}
				}
			}
			case 659: {
				stateStack.Push(650);
				goto case 23;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(121, la); // "Event"
				currentState = 661;
				break;
			}
			case 661: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(662);
				goto case 209;
			}
			case 662: {
				PopContext();
				goto case 663;
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				if (la.kind == 63) {
					currentState = 665;
					break;
				} else {
					if (set[153].Get(la.kind)) {
						if (la.kind == 37) {
							stateStack.Push(664);
							goto case 435;
						} else {
							goto case 664;
						}
					} else {
						Error(la);
						goto case 664;
					}
				}
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				if (la.kind == 138) {
					stateStack.Push(23);
					goto case 642;
				} else {
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
				Expect(103, la); // "Declare"
				currentState = 668;
				break;
			}
			case 668: {
				if (la == null) { currentState = 668; break; }
				if (la.kind == 62 || la.kind == 67 || la.kind == 226) {
					currentState = 669;
					break;
				} else {
					goto case 669;
				}
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				if (la.kind == 213) {
					currentState = 670;
					break;
				} else {
					if (la.kind == 129) {
						currentState = 670;
						break;
					} else {
						Error(la);
						goto case 670;
					}
				}
			}
			case 670: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(671);
				goto case 209;
			}
			case 671: {
				PopContext();
				goto case 672;
			}
			case 672: {
				if (la == null) { currentState = 672; break; }
				Expect(152, la); // "Lib"
				currentState = 673;
				break;
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				Expect(3, la); // LiteralString
				currentState = 674;
				break;
			}
			case 674: {
				if (la == null) { currentState = 674; break; }
				if (la.kind == 59) {
					currentState = 679;
					break;
				} else {
					goto case 675;
				}
			}
			case 675: {
				if (la == null) { currentState = 675; break; }
				if (la.kind == 37) {
					stateStack.Push(676);
					goto case 435;
				} else {
					goto case 676;
				}
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				if (la.kind == 63) {
					currentState = 677;
					break;
				} else {
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
				goto case 23;
			}
			case 679: {
				if (la == null) { currentState = 679; break; }
				Expect(3, la); // LiteralString
				currentState = 675;
				break;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				if (la.kind == 213) {
					currentState = 681;
					break;
				} else {
					if (la.kind == 129) {
						currentState = 681;
						break;
					} else {
						Error(la);
						goto case 681;
					}
				}
			}
			case 681: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 682;
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				currentState = 683;
				break;
			}
			case 683: {
				PopContext();
				goto case 684;
			}
			case 684: {
				if (la == null) { currentState = 684; break; }
				if (la.kind == 37) {
					currentState = 696;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 694;
						break;
					} else {
						goto case 685;
					}
				}
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				if (la.kind == 136 || la.kind == 138) {
					if (la.kind == 138) {
						stateStack.Push(686);
						goto case 642;
					} else {
						if (la.kind == 136) {
							stateStack.Push(686);
							goto case 689;
						} else {
							Error(la);
							goto case 686;
						}
					}
				} else {
					goto case 686;
				}
			}
			case 686: {
				stateStack.Push(687);
				goto case 265;
			}
			case 687: {
				if (la == null) { currentState = 687; break; }
				Expect(115, la); // "End"
				currentState = 688;
				break;
			}
			case 688: {
				if (la == null) { currentState = 688; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 129) {
						currentState = 23;
						break;
					} else {
						goto case 543;
					}
				}
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				Expect(136, la); // "Handles"
				currentState = 690;
				break;
			}
			case 690: {
				stateStack.Push(691);
				goto case 692;
			}
			case 691: {
				if (la == null) { currentState = 691; break; }
				if (la.kind == 22) {
					currentState = 690;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 692: {
				if (la == null) { currentState = 692; break; }
				if (la.kind == 156) {
					currentState = 693;
					break;
				} else {
					if (la.kind == 162) {
						currentState = 693;
						break;
					} else {
						if (la.kind == 161) {
							currentState = 693;
							break;
						} else {
							if (set[6].Get(la.kind)) {
								currentState = 693;
								break;
							} else {
								Error(la);
								goto case 693;
							}
						}
					}
				}
			}
			case 693: {
				if (la == null) { currentState = 693; break; }
				Expect(26, la); // "."
				currentState = 209;
				break;
			}
			case 694: {
				PushContext(Context.Type, la, t);
				stateStack.Push(695);
				goto case 37;
			}
			case 695: {
				PopContext();
				goto case 685;
			}
			case 696: {
				SetIdentifierExpected(la);
				goto case 697;
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (set[151].Get(la.kind)) {
					if (la.kind == 172) {
						currentState = 699;
						break;
					} else {
						if (set[79].Get(la.kind)) {
							stateStack.Push(698);
							goto case 439;
						} else {
							Error(la);
							goto case 698;
						}
					}
				} else {
					goto case 698;
				}
			}
			case 698: {
				if (la == null) { currentState = 698; break; }
				Expect(38, la); // ")"
				currentState = 684;
				break;
			}
			case 699: {
				stateStack.Push(698);
				goto case 513;
			}
			case 700: {
				stateStack.Push(701);
				SetIdentifierExpected(la);
				goto case 702;
			}
			case 701: {
				if (la == null) { currentState = 701; break; }
				if (la.kind == 22) {
					currentState = 700;
					break;
				} else {
					goto case 23;
				}
			}
			case 702: {
				if (la == null) { currentState = 702; break; }
				if (la.kind == 90) {
					currentState = 703;
					break;
				} else {
					goto case 703;
				}
			}
			case 703: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(704);
				goto case 714;
			}
			case 704: {
				PopContext();
				goto case 705;
			}
			case 705: {
				if (la == null) { currentState = 705; break; }
				if (la.kind == 33) {
					currentState = 706;
					break;
				} else {
					goto case 706;
				}
			}
			case 706: {
				if (la == null) { currentState = 706; break; }
				if (la.kind == 37) {
					currentState = 711;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 708;
						break;
					} else {
						goto case 707;
					}
				}
			}
			case 707: {
				if (la == null) { currentState = 707; break; }
				if (la.kind == 20) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 708: {
				PushContext(Context.Type, la, t);
				goto case 709;
			}
			case 709: {
				if (la == null) { currentState = 709; break; }
				if (la.kind == 165) {
					stateStack.Push(710);
					goto case 85;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(710);
						goto case 37;
					} else {
						Error(la);
						goto case 710;
					}
				}
			}
			case 710: {
				PopContext();
				goto case 707;
			}
			case 711: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 712;
			}
			case 712: {
				if (la == null) { currentState = 712; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(713);
					goto case 55;
				} else {
					goto case 713;
				}
			}
			case 713: {
				if (la == null) { currentState = 713; break; }
				if (la.kind == 22) {
					currentState = 711;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 706;
					break;
				}
			}
			case 714: {
				if (la == null) { currentState = 714; break; }
				if (set[139].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 144;
					} else {
						if (la.kind == 128) {
							goto case 128;
						} else {
							if (la.kind == 68) {
								goto case 147;
							} else {
								if (la.kind == 240) {
									goto case 145;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 715: {
				isMissingModifier = false;
				goto case 594;
			}
			case 716: {
				if (la == null) { currentState = 716; break; }
				Expect(138, la); // "Implements"
				currentState = 717;
				break;
			}
			case 717: {
				PushContext(Context.Type, la, t);
				stateStack.Push(718);
				goto case 37;
			}
			case 718: {
				PopContext();
				goto case 719;
			}
			case 719: {
				if (la == null) { currentState = 719; break; }
				if (la.kind == 22) {
					currentState = 720;
					break;
				} else {
					stateStack.Push(586);
					goto case 23;
				}
			}
			case 720: {
				PushContext(Context.Type, la, t);
				stateStack.Push(721);
				goto case 37;
			}
			case 721: {
				PopContext();
				goto case 719;
			}
			case 722: {
				if (la == null) { currentState = 722; break; }
				Expect(142, la); // "Inherits"
				currentState = 723;
				break;
			}
			case 723: {
				PushContext(Context.Type, la, t);
				stateStack.Push(724);
				goto case 37;
			}
			case 724: {
				PopContext();
				stateStack.Push(584);
				goto case 23;
			}
			case 725: {
				if (la == null) { currentState = 725; break; }
				Expect(172, la); // "Of"
				currentState = 726;
				break;
			}
			case 726: {
				stateStack.Push(727);
				goto case 513;
			}
			case 727: {
				if (la == null) { currentState = 727; break; }
				Expect(38, la); // ")"
				currentState = 581;
				break;
			}
			case 728: {
				isMissingModifier = false;
				goto case 28;
			}
			case 729: {
				PushContext(Context.Type, la, t);
				stateStack.Push(730);
				goto case 37;
			}
			case 730: {
				PopContext();
				goto case 731;
			}
			case 731: {
				if (la == null) { currentState = 731; break; }
				if (la.kind == 22) {
					currentState = 732;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 732: {
				PushContext(Context.Type, la, t);
				stateStack.Push(733);
				goto case 37;
			}
			case 733: {
				PopContext();
				goto case 731;
			}
			case 734: {
				if (la == null) { currentState = 734; break; }
				Expect(172, la); // "Of"
				currentState = 735;
				break;
			}
			case 735: {
				stateStack.Push(736);
				goto case 513;
			}
			case 736: {
				if (la == null) { currentState = 736; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 737: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 738;
			}
			case 738: {
				if (la == null) { currentState = 738; break; }
				if (set[50].Get(la.kind)) {
					currentState = 738;
					break;
				} else {
					PopContext();
					stateStack.Push(739);
					goto case 23;
				}
			}
			case 739: {
				if (la == null) { currentState = 739; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(739);
					goto case 5;
				} else {
					Expect(115, la); // "End"
					currentState = 740;
					break;
				}
			}
			case 740: {
				if (la == null) { currentState = 740; break; }
				Expect(163, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 741: {
				if (la == null) { currentState = 741; break; }
				Expect(139, la); // "Imports"
				currentState = 742;
				break;
			}
			case 742: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 743;
			}
			case 743: {
				if (la == null) { currentState = 743; break; }
				if (set[154].Get(la.kind)) {
					currentState = 749;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 745;
						break;
					} else {
						Error(la);
						goto case 744;
					}
				}
			}
			case 744: {
				PopContext();
				goto case 23;
			}
			case 745: {
				stateStack.Push(746);
				goto case 209;
			}
			case 746: {
				if (la == null) { currentState = 746; break; }
				Expect(20, la); // "="
				currentState = 747;
				break;
			}
			case 747: {
				if (la == null) { currentState = 747; break; }
				Expect(3, la); // LiteralString
				currentState = 748;
				break;
			}
			case 748: {
				if (la == null) { currentState = 748; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 744;
				break;
			}
			case 749: {
				if (la == null) { currentState = 749; break; }
				if (la.kind == 33 || la.kind == 37) {
					stateStack.Push(749);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 750;
						break;
					} else {
						goto case 744;
					}
				}
			}
			case 750: {
				stateStack.Push(744);
				goto case 37;
			}
			case 751: {
				if (la == null) { currentState = 751; break; }
				Expect(176, la); // "Option"
				currentState = 752;
				break;
			}
			case 752: {
				if (la == null) { currentState = 752; break; }
				if (la.kind == 123 || la.kind == 141 || la.kind == 210) {
					currentState = 754;
					break;
				} else {
					if (la.kind == 89) {
						currentState = 753;
						break;
					} else {
						goto case 543;
					}
				}
			}
			case 753: {
				if (la == null) { currentState = 753; break; }
				if (la.kind == 216) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 69) {
						currentState = 23;
						break;
					} else {
						goto case 543;
					}
				}
			}
			case 754: {
				if (la == null) { currentState = 754; break; }
				if (la.kind == 173 || la.kind == 174) {
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
		new BitArray(new int[] {1, 256, 4194304, -2145385984, -1073674240, -738131448, 1049600, 0}),
		new BitArray(new int[] {1, 256, 4194304, -2145385984, -1073674240, -738196984, 1049600, 0}),
		new BitArray(new int[] {1, 256, 4194304, -2145385984, -1073676288, -738196984, 1049600, 0}),
		new BitArray(new int[] {0, 256, 4194304, -2145385984, -1073676288, -738196984, 1049600, 0}),
		new BitArray(new int[] {0, 256, 4194304, -2145385984, -1073676288, -738196992, 1049600, 0}),
		new BitArray(new int[] {0, 0, 4194304, -2145385984, -1073676288, -738196992, 1049600, 0}),
		new BitArray(new int[] {4, 1140850688, 33554751, 138421264, 7479425, 136847360, 25182208, 66060}),
		new BitArray(new int[] {0, 256, 4194308, -2111304960, 1074872322, -171964911, 3148802, 26624}),
		new BitArray(new int[] {0, 256, 4194308, -2111304960, 1074855938, -171964911, 3148802, 26624}),
		new BitArray(new int[] {5, 1140850944, 104857919, -1972883568, -1066196861, -35084775, 28331010, 92684}),
		new BitArray(new int[] {0, 256, 4194308, -2111829248, 1074855938, -171964911, 3148802, 26624}),
		new BitArray(new int[] {0, 0, 4194308, -2111829248, 1074855938, -171964911, 3148802, 26624}),
		new BitArray(new int[] {0, 0, 4194304, 35652096, 1073807362, 536870912, 3145728, 0}),
		new BitArray(new int[] {0, 0, 0, 33554432, 2, 536870912, 2097152, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 34603903, 138454128, 74621073, 136849408, 25718912, 66079}),
		new BitArray(new int[] {-940564478, 889192447, 257, 4334592, 581834113, 1851392, 176242944, 37632}),
		new BitArray(new int[] {-940564478, 889192413, 257, 4334592, 581834113, 1851392, 176242944, 37632}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 262143}),
		new BitArray(new int[] {-61995012, 1174405224, -205536385, 406893679, 343057035, 136853926, -777490304, 131615}),
		new BitArray(new int[] {-61995012, 1174405160, -205536385, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-61995012, 1174405224, -205536385, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-66189316, 1174405160, -205536385, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-1007673342, 889192405, 257, 4334592, 581828737, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 571211776, 1310720, 134217728, 32768}),
		new BitArray(new int[] {-66189316, 1174405176, -205536385, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {4, 1140850690, 34603903, 138454128, 74621073, 136849408, 25718912, 70175}),
		new BitArray(new int[] {-1007673342, 889192405, 257, 4334592, 581829761, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-1007673342, 889192413, 257, 4334592, 581829761, 1851392, 176242944, 37376}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 257, 4334592, 581828737, 1851392, 176242944, 33280}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 262143}),
		new BitArray(new int[] {-1038334, -1258291211, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {1007552508, 1140850720, -205536401, 406893679, 343057035, 136849670, 1369993344, 131615}),
		new BitArray(new int[] {-1040382, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {0, 0, -240140288, 4111, 0, 0, 1073741824, 0}),
		new BitArray(new int[] {0, 67108864, 0, 8192, 10485889, 524288, 8405248, 512}),
		new BitArray(new int[] {4194304, 67108864, 0, 8192, 10616961, 524288, 8405248, 512}),
		new BitArray(new int[] {-66189316, 1174405160, -205536385, 406893695, 343057035, 136849830, -777490304, 197151}),
		new BitArray(new int[] {4194304, 67108864, 256, 8192, 10616961, 524288, 8405248, 512}),
		new BitArray(new int[] {66189314, -1174405161, 205536384, -406893680, -343057036, -136849831, 777490303, -131616}),
		new BitArray(new int[] {65140738, 973078487, 205536384, -406893680, -343057036, -136849831, 777490303, -131616}),
		new BitArray(new int[] {-66189316, 1174405160, -205536385, 406893679, 343057035, 136849830, -777490304, 132639}),
		new BitArray(new int[] {0, 67108864, 0, 8192, 10616961, 524288, 8405248, 512}),
		new BitArray(new int[] {0, 0, 4, 0, 1048578, 0, 2097152, 0}),
		new BitArray(new int[] {-64092162, -973078488, -205536385, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-64092162, 1191182376, -4195457, 2110717039, 477274827, 136866278, -169152011, 202303}),
		new BitArray(new int[] {0, 0, 12288, 537788416, 134217728, 64, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -4207745, 1572928623, 343057099, 136866214, -169152011, 202303}),
		new BitArray(new int[] {-66189316, 1174405162, -205536385, 406893695, 343057051, 136849830, -777490304, 197151}),
		new BitArray(new int[] {6291458, 0, 0, 131072, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1174405160, -205536385, 407024751, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {0, 0, 0, 1073758208, 2, 536870912, 538968320, 1024}),
		new BitArray(new int[] {36, 1140850688, 33554751, 138421264, 7479425, 136847360, 25182208, 66060}),
		new BitArray(new int[] {2097158, 1140850688, 33554751, 138421264, 7479425, 136847360, 25182208, 66316}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 256}),
		new BitArray(new int[] {36, 1140850688, 33554751, 138421264, 7479425, 136847424, 25182208, 66060}),
		new BitArray(new int[] {-66189316, 1174405160, -205536385, 406893679, 343057099, 136849830, -777490272, 131615}),
		new BitArray(new int[] {1007552508, 1140850720, -205536401, 406959215, 343057035, 136849670, 1369993344, 131615}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 4096, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 4096, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 1032}),
		new BitArray(new int[] {2097154, 0, 0, 131072, 0, 0, 0, 1032}),
		new BitArray(new int[] {-66189316, 1174405160, -205532289, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-65140740, 1174409128, -205536385, 407024751, 343319179, 136849830, -777490304, 131615}),
		new BitArray(new int[] {-65140740, 1174409128, -205536385, 406893679, 343319179, 136849830, -777490304, 131615}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 262144, 0, 0, 0}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -4207745, 1572928623, 343057099, 136866214, -169152011, 202303}),
		new BitArray(new int[] {-64092162, 1191182376, -4207745, 1573059695, 343057099, 136866214, -169152011, 202303}),
		new BitArray(new int[] {2097154, 32, 0, 131072, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 131072, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 131072, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, 0, 0, 131072, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 34603903, 138454128, 74621073, 136849440, 25718912, 66079}),
		new BitArray(new int[] {4, 1140851008, 33555903, 138421264, 7479425, 170532864, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850944, 33555903, 138421264, 7479425, 170532864, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850688, 33555903, 138421264, 7479425, 170532864, 25182208, 66060}),
		new BitArray(new int[] {5242880, -2147483550, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {5242880, -2147483552, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 34603903, 138454128, 1148362897, 136849408, 25718912, 66079}),
		new BitArray(new int[] {7, 1157628162, 105908223, -1972850704, -999055209, -1397191, 28868243, 92703}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-909310, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-843774, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-318462, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-383998, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {-1038334, -1258291209, 257, 4334592, 581832833, 1851392, 176242944, 33280}),
		new BitArray(new int[] {4194308, 1140850754, 34603903, 138454128, 74621073, 136849408, 25718912, 66079}),
		new BitArray(new int[] {4, 1140851008, 33555903, 138421264, 7479425, 170536960, 25182208, 66060}),
		new BitArray(new int[] {4, 1073741824, 33554731, 138421248, 6434944, 136847360, 25182208, 524}),
		new BitArray(new int[] {4, 1073741824, 33554731, 138421248, 6430848, 134750208, 25182208, 524}),
		new BitArray(new int[] {4, 1140850698, 38798207, 138454128, 74621073, 136849440, 26767488, 66079}),
		new BitArray(new int[] {4, 1140850690, 38798207, 138454128, 74621073, 136849440, 26767488, 66079}),
		new BitArray(new int[] {4, 1140850946, 34603903, 138454128, 74621073, 136849408, 25718912, 66079}),
		new BitArray(new int[] {4, 1140850944, 33554751, 138945552, 7479425, 136847360, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850944, 33554751, 138421264, 7479425, 136847360, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850944, 104857919, -1972883568, 1081304195, -35084783, 28331010, 92684}),
		new BitArray(new int[] {4, 1140850944, 104857919, -1972883568, 1081287811, -35084783, 28331010, 92684}),
		new BitArray(new int[] {4, 1140850944, 104857919, -1972883568, 1081286787, -35084783, 28331010, 92684}),
		new BitArray(new int[] {4, 1140850944, 104857919, -1973407856, 1081286787, -35084783, 28331010, 92684}),
		new BitArray(new int[] {4, 1140850688, 104857919, -1973407856, 1081286787, -35084783, 28331010, 92684}),
		new BitArray(new int[] {4, 1140850688, 104857915, 174073488, 1080238211, 673751040, 28327936, 66060}),
		new BitArray(new int[] {4, 1140850688, 100663611, 171975824, 6430851, 673751040, 27279360, 66060}),
		new BitArray(new int[] {3145730, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 104857919, -1972883568, 1081286791, -35084783, 28331522, 92684}),
		new BitArray(new int[] {0, 256, 0, -2146959360, 4, -805306368, 512, 0}),
		new BitArray(new int[] {0, 256, 0, -2147483648, 4, -805306368, 512, 0}),
		new BitArray(new int[] {0, 0, 0, -2147483648, 4, -805306368, 512, 0}),
		new BitArray(new int[] {7340034, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850946, 34603903, 138454128, 74621073, 136849440, 25718912, 66079}),
		new BitArray(new int[] {0, 16777472, 0, 524288, 0, 0, 17, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 0, 17, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 8, 0, 2, 0, 2097152, 4}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 1280, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 1280, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 33554751, 138421264, 275914881, 136847366, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850688, 100663611, 138421248, 6430849, 136847360, 25182208, 66060}),
		new BitArray(new int[] {4, 1140850688, 33554747, 138421248, 6430849, 136847360, 25182208, 66060}),
		new BitArray(new int[] {7340034, -2147483614, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 4194304, -2144861696, -1073676288, -738196984, 1049600, 0}),
		new BitArray(new int[] {1028, 1140850688, 34603903, 138454128, 74621073, 136849408, 25718912, 66079}),
		new BitArray(new int[] {70254594, 34, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 33554432, 134217728, 8192, 0, 262144, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 24576, 0, 0}),
		new BitArray(new int[] {0, 0, 0, -2147483648, -2147483648, -738196992, 1024, 0}),
		new BitArray(new int[] {0, 0, 4, -2147481344, 1048576, -708835823, 3074, 26624}),
		new BitArray(new int[] {0, 0, 1049152, 32864, 67141632, 2048, 536704, 19}),
		new BitArray(new int[] {-1073741824, 33554432, 16, 0, 0, 128, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 268435456, 268435456, 262, 268435456, 0}),
		new BitArray(new int[] {4, 1073741824, 33554731, 138421248, 6430848, 136847360, 25182208, 524}),
		new BitArray(new int[] {0, 0, -240140288, 14, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -4207745, 1573452911, 343057099, 136866214, -169152011, 202303}),
		new BitArray(new int[] {0, 0, 134217728, 67108864, 64, 0, 131136, 65536}),
		new BitArray(new int[] {-66189316, 1174405160, -205534337, 406893679, 343057035, 136849830, -777490304, 131615}),
		new BitArray(new int[] {0, 0, 1152, 0, 0, 33685504, 0, 0}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850944, 33555903, 138421264, 7479425, 170536960, 25182208, 66060}),
		new BitArray(new int[] {0, 0, 0, -2147483648, 0, -805306368, 0, 0}),
		new BitArray(new int[] {2097154, 32, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 34603903, 138454128, 74621073, 136849408, 25718912, 66079})

	};

} // end Parser


}