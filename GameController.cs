using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace Dama_WPF
{
    class GameController
    {
        private Board board = new Board();
        private Rules rules;
        private UI ui = new UI();
        private Brain brain;
        private Data data = new Data();
        //MainWindow MainWindow = new MainWindow();


        //proměnné hráčů, pro uživatele 0, 1-4 obtížnost PC
        public int player1 = 0;
        public int player2 = 0;
        public int ptrTah;

        public GameController()
        {
            rules = new Rules(board);
            ptrTah = 0;
            //ui = new UI();
            //data = new Data();
        }

        public string HistorieNaString(int[] prvek)
        {
            return board.PohybNaString(prvek);
        }
        //public string HistorieNaString(List<int[]> hist)
        //{
        //    foreach (int[] item in hist)
        //    {
        //        return board.PohybNaString(item);
        //    }
        //    return "nic";
        //}

        public List<int[]> HistorieTahu()
        {
            return board.HistoryMove;
        }
        public void InitGame()
        {
            rules.InitBoard(); //naplní se deska
            rules.InitPlayer(); //začne bílý
            rules.MovesGenerate(); //vygenerují se možné tahy pro hráče
            //PcPlayer();
            board.tahuBezSkoku = 0;
        }
        public void PcPlayer()
        {
            if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0) //pokud hráč na tahu je 1 a player1 > 0 tak true, provede tah a continue na dalšího hráče
            {

                //int[] move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2);
                brain = new Brain(board, rules);
                int[] move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2);
                //Thread pc = new Thread(() => move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2));
                //pc.IsBackground = true;
                //pc.Start();
                board.Move(move, true, false);

                //pokud tah není skok tak se navýší počítadlo TahuBezSkoku
                if (move.Length == 8)
                {
                    board.tahuBezSkoku++;
                }
                else
                {
                    board.tahuBezSkoku = 0;
                }

                //kolo = board.HistoryMove.Count / 2; //přičtení do počítadla kol

                rules.ChangePlayer();
                rules.MovesGenerate();
                //ptrTah = board.HistoryMove.Count;
                //Thread.Sleep(1500);
                //continue;
                //PcPlayer();
            }
        }

        public void NextPlayer()
        {
            rules.ChangePlayer();
            rules.MovesGenerate();
        }

        public bool IsValidCoords(int x, int y)
        {
            return board.IsValidCoordinates(x, y);
        }

        public void MovesGenerate()
        {
            rules.MovesGenerate();
        }

        public int[] FullMove(int[] move)
        {
            return rules.FullMove(move);
        }

        public void MakeMove(int[] fullMove, bool ulozit, bool zpet)
        {
            board.Move(fullMove, ulozit, zpet);
            ptrTah = board.HistoryMove.Count();
        }

        public bool IsPcPlayer()
        {
            if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hlavní herní smyčka
        /// </summary>
        public void Game()
        {
            rules.InitBoard(); //inicializace desky
            ui.SelectPlayer(out player1, out player2); //výběr hráče na tahu
            rules.InitPlayer(); //inicializace hráče na tahu
            rules.MovesGenerate(); //vygenerování všech tahů pro aktuálního hráče tj. 1-bílý
            board.tahuBezSkoku = 0;
            int kolo = 0; //počítadlo kol
            int ptrTah = board.HistoryMove.Count;//ukazatel na poslední tah v historii tahů
            int[] posledniTah = null; //uložen poslední tah

            while (!rules.IsGameFinished()) //cyklus dokud platí že oba hráči mají figurky, jinak konec
            {
                //Console.Clear();
                //ui.PocetKol(kolo);
                //ui.PocetTahuBezSkoku(board.tahuBezSkoku);
                //ui.PrintBoard(board);

                //Tahy počítače
                if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0) //pokud hráč na tahu je 1 a player1 > 0 tak true, provede tah a continue na dalšího hráče
                {
                    //ui.PcInfo();
                    int[] move = null;
                    Brain brain = new Brain(board, rules);
                    Thread pc = new Thread(() => move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2));
                    pc.IsBackground = true;
                    pc.Start();

                    ConsoleKey pressKey = ConsoleKey.A;

                    while (pc.IsAlive && pressKey != ConsoleKey.Escape && pressKey != ConsoleKey.Z)
                    {
                        if (Console.KeyAvailable)
                        {
                            pressKey = Console.ReadKey().Key;
                        }
                    }
                    if (pressKey == ConsoleKey.Escape)
                    {
                        pc.Abort();
                        Start(); //zobrazení menu
                        //Game(player1,player2); //start hry
                        continue;
                    }
                    if (pressKey == ConsoleKey.Z)
                    {
                        pc.Abort();
                        ui.SelectPlayer(out player1, out player2);
                        continue;
                    }
                    else
                    {
                        board.Move(move, true, false);
                    }

                    //pokud tah není skok tak se navýší počítadlo TahuBezSkoku
                    if (move.Length == 8)
                    {
                        board.tahuBezSkoku++;
                    }
                    else
                    {
                        board.tahuBezSkoku = 0;
                    }

                    kolo = board.HistoryMove.Count / 2; //přičtení do počítadla kol

                    rules.ChangePlayer();
                    rules.MovesGenerate();
                    //Thread.Sleep(1500);
                    continue;
                }

                //Tahy Hráče
                int[] vstup = null;
                int[] plnyVstup = null;
                bool platnyVstup = false;

                while (!platnyVstup) //Dokud je vstup !playtnyVstup tak pokračuje
                {
                    vstup = ui.InputUser(rules.PlayerOnMove(),board); //pokud -1 tak se podmínka neprovede protože -1 >= 0, pokud 0 tak se provede 0=0 a zkontroluje se platnost tahu

                    //Výpis historie tahu
                    if (vstup[0] == -4)
                    {
                        ui.PrintHelpMove(board.HistoryMove,board);
                    }

                    //Možnost tahu zpět/undo
                    if (vstup[0] == -3)
                    {
                        if (ptrTah > 0)
                        {
                            ptrTah--;
                            posledniTah = board.HistoryMove[ptrTah];
                            //moveServices.TahZpet(board,rules, ui,ptrTah, posledniTah);
                        }
                    }
                    //Možnost tahu vpřed/redo
                    if (vstup[0] == -6)
                    {
                        if (ptrTah < board.HistoryMove.Count && board.HistoryMove.Count > 0)
                        {
                            posledniTah = board.HistoryMove[ptrTah];
                            //moveServices.TahVpred(board, rules, ui, ptrTah, posledniTah, kolo);
                        }
                    }

                    //Pokud hráč do konzole zadá HELP
                    if (vstup[0] == -2)
                    {
                        if (vstup.Length > 1) //Pokud ještě zadá pro jakou figurku chce help
                        {
                            ui.PrintHelpMove(rules.GetMovesList(vstup[1], vstup[2]),board); //pro zadanou figurku
                        }
                        else //Vypíše všechny možné tahy hráče na tahu
                        {
                            ui.PrintHelpMove(rules.GetMovesList(),board); //všechny možné tahy hráče
                            //ui.PrintHelpMove(board.HistoryMove); //všechny možné tahy hráče
                        }
                    }

                    //SPRÁVNĚ
                    if (vstup[0] >= 0) //pokud je zadán správný pohyb tj A2-B3
                    {
                        plnyVstup = rules.FullMove(vstup); //převedení na kompletní pohyb který se skládá ze 4 souřadnic X,Y, stav před, stav po

                        platnyVstup = plnyVstup[0] != -1; //ověření zda je táhnuto dle pravidel, typ bool ve while cyklu

                        //ClearHistoryFromToEnd(ptrTah);

                        if (!platnyVstup) //pokud není vypíše uživately chybu
                        {
                            ui.Mistake(); //chyba
                        }
                    }

                    //Uložení hry
                    if (vstup[0] == -8)
                    {
                        data.SaveGame(player1, player2, ptrTah, board.HistoryMove);
                    }

                    //Načítání hry
                    if (vstup[0] == -9)
                    {

                        //if (data.LoadGame(out loadBoard, out loadRules, out loadPlayer1, out loadPlayer2, out int loadUkazatel, out int loadTahuBezSkoku))
                        //{
                        //    board = loadBoard;
                        //    rules = loadRules;
                        //    player1 = loadPlayer1;
                        //    player2 = loadPlayer2;
                        //    ptrTah = board.HistoryMove.Count;
                        //    board.tahuBezSkoku = loadTahuBezSkoku;
                            
                        //    while (ptrTah > loadUkazatel) //pokud aktuální ukazatel je větší než načtený
                        //    {
                        //        ptrTah--; //aktualní se zmenší
                        //        board.Move(board.HistoryMove[ptrTah], false, true);
                        //        rules.ChangePlayer();
                        //    }

                        //    Console.Clear();
                        //    kolo = board.HistoryMove.Count / 2;
                        //    ui.PocetKol(kolo);
                        //    ui.PocetTahuBezSkoku(board.tahuBezSkoku);
                        //    ui.PrintBoard(board);
                        //    rules.MovesGenerate();
                        //}
                        //else
                        //{
                        //    ui.Mistake();
                        //}
                    }
                    //Zpět do menu
                    if (vstup[0] == -5)
                    {
                        Console.Clear();
                        Start();
                        //Game();
                    }
                }
                board.Move(plnyVstup, true, false); //pokud je zadáno správně, metoda nastaví pohyb na desce
                ptrTah = board.HistoryMove.Count;

                //počítání kol
                kolo = board.HistoryMove.Count / 2;

                if (plnyVstup.Length == 8)
                {
                    board.tahuBezSkoku++;
                }
                else
                {
                    board.tahuBezSkoku = 0;
                }

                if (rules.ListMove.Count == 0) //pokud je ListMove prázdnej tak se změní hráč na tahu a vygenerují se pro něj nové možné tahy
                {
                    rules.ChangePlayer();
                    rules.MovesGenerate();
                }
                else //pokud v listu stále je možnost, tak pokračuje hráč, vícenásobné skoky
                {
                    continue;
                }
            }
            ui.PrintBoard(board);
            ui.Finished(board);
        }
        /// <summary>
        /// Metoda pro nastavení hodnoty políčka
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="hodnota"></param>
        public void SetValueOnBoard(int posX, int posY, int value)
        {
            board.SetValue(posX, posY, value);
        }
        /// <summary>
        /// Metoda pro čtení hodnoty na souřadnicích
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public int GetValueOnBoard(int posX, int posY)
        {
            return board.GetValue(posX, posY);
        }
        /// <summary>
        /// Metoda pro hlavní menu
        /// </summary>
        public void Start()
        {
            ui.HlavniMenu();
        }
        /// <summary>
        /// Metoda smaže historii od pointeru
        /// </summary>
        /// <param name="ptrTah"></param>
        public void ClearHistoryFromToEnd()
        {
            board.HistoryMove.RemoveRange(ptrTah, board.HistoryMove.Count - ptrTah); //odstraní tahy=index ptrTah, od indexu Count-ptrTah
        }









        /// <summary>
        /// List možných tahů pro figurku
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public List<int[]> GetPossibleMoves(int X, int Y)
        {
            return rules.GetMovesList(X, Y);
        }
        /// <summary>
        /// Tah zpět
        /// </summary>
        public void UndoMove()
        {
            int[] posledniTah = null;
            if (ptrTah > 0)
            {
                ptrTah--;
                posledniTah = board.HistoryMove[ptrTah];
                board.Move(posledniTah, false, true);
                rules.ChangePlayer();
                rules.MovesGenerate();
            }
        }
        /// <summary>
        /// Tah vpřed
        /// </summary>
        public void RedoMove()
        {
            int[] posledniTah = null;
            if (ptrTah < board.HistoryMove.Count && board.HistoryMove.Count > 0)
            {
                posledniTah = board.HistoryMove[ptrTah];
                board.Move(posledniTah, false, false);
                ptrTah++;
                rules.ChangePlayer();
                rules.MovesGenerate();
            }
        }
        /// <summary>
        /// Vrátí hráče na tahu
        /// </summary>
        /// <returns></returns>
        public int GetPlayerOnMove()
        {
            return rules.PlayerOnMove();
        }


        /// <summary>
        /// Načtení hry
        /// </summary>
        /// <param name="openFile"></param>
        /// <returns></returns>
        public bool LoadGame(OpenFileDialog openFile)
        {
            Board loadBoard;
            Rules loadRules;
            int ptrTah, loadPlayer1, loadPlayer2;

            if (data.LoadGame(openFile,out loadBoard, out loadRules, out loadPlayer1, out loadPlayer2, out int loadUkazatel, out int loadTahuBezSkoku))
            {
                board = loadBoard;
                rules = loadRules;
                player1 = loadPlayer1;
                player2 = loadPlayer2;
                ptrTah = board.HistoryMove.Count;
                board.tahuBezSkoku = loadTahuBezSkoku;

                while (ptrTah > loadUkazatel) //pokud aktuální ukazatel je větší než načtený
                {
                    ptrTah--; //aktualní se zmenší
                    board.Move(board.HistoryMove[ptrTah], false, true);
                    rules.ChangePlayer();
                }
                return true;
            }
            return false;
        }
        public bool SaveGame(int player1, int player2,List<int[]> historie)
        {
            if(data.SaveGame(player1, player2, ptrTah, board.HistoryMove))
            {
                return true;
            }
            return false;
        }
        public bool IsGameFinished()
        {
            int bilyPesak, cernyPesak, bilaDama, cernaDama;
            board.CountStones(out bilyPesak, out bilaDama, out cernyPesak, out cernaDama);
            int cerna = cernyPesak + cernaDama;
            int bila = bilyPesak + bilaDama;

            if (cerna == 0)
            {
                return true;
            }
            if (bila == 0)
            {
                return true;
            }
            return false;
        }

    }
}
