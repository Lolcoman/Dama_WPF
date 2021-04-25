using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace Dama_WPF
{
    class GameController
    {
        private Board board = new Board();
        private Rules rules;
        private Brain brain;
        private Data data = new Data();

        //proměnné hráčů, pro uživatele 0, 1-4 obtížnost PC
        public int player1 = 0;
        public int player2 = 0;
        public int ptrTah;

        public GameController()
        {
            rules = new Rules(board);
            ptrTah = 0;
        }
        /// <summary>
        /// Metoda int[] na string
        /// </summary>
        /// <param name="prvek"></param>
        /// <returns></returns>
        public string HistorieNaString(int[] prvek)
        {
            return board.PohybNaString(prvek);
        }
        /// <summary>
        /// Metoda vrátí list historie tahů
        /// </summary>
        /// <returns></returns>
        public List<int[]> HistorieTahu()
        {
            return board.HistoryMove;
        }
        /// <summary>
        /// Metoda pro inicializaci hry
        /// </summary>
        public void InitGame()
        {
            rules.InitBoard(); //naplní se deska
            rules.InitPlayer(); //začne bílý
            rules.MovesGenerate(); //vygenerují se možné tahy pro hráče
            //PcPlayer();
            board.tahuBezSkoku = 0;
        }
        /// <summary>
        /// Metoda pro tah PC hráče
        /// </summary>
        public void PcPlayer(BackgroundWorker bw)
        {
            if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0) //pokud hráč na tahu je 1 a player1 > 0 tak true, provede tah a continue na dalšího hráče
            {
                brain = new Brain(board, rules, bw);
                int[] move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2);
                //board.Move(move, true, false);
                if (!bw.CancellationPending) //pokud se čeká na zrušení tah se neprovede
                {
                    MakeMove(move, true, false);
                    rules.ChangePlayer();
                    rules.MovesGenerate();
                }

            }
        }

        /// <summary>
        /// Metoda vrátí počet tahu bez skoku
        /// </summary>
        /// <returns></returns>
        public int WithoutJump()
        {
            return board.tahuBezSkoku;
        }
        /// <summary>
        /// Metoda pro změnu hráče na tahu a vygenerování tahů
        /// </summary>
        public void NextPlayer()
        {
            rules.ChangePlayer();
            rules.MovesGenerate();
        }
        /// <summary>
        /// Metoda pro kontrolu souřadnic
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValidCoords(int x, int y)
        {
            return board.IsValidCoordinates(x, y);
        }
        /// <summary>
        /// Metoda pro generování všech tahů
        /// </summary>
        public void MovesGenerate()
        {
            rules.MovesGenerate();
        }
        /// <summary>
        /// Metoda pro převod na plný tah
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public int[] FullMove(int[] move)
        {
            return rules.FullMove(move);
        }
        /// <summary>
        /// Metoda pro provedení tahu
        /// </summary>
        /// <param name="fullMove"></param>
        /// <param name="ulozit"></param>
        /// <param name="zpet"></param>
        public void MakeMove(int[] fullMove, bool ulozit, bool zpet)
        {
            if (fullMove.Length == 8)
            {
                board.tahuBezSkoku++;
            }
            else
            {
                board.tahuBezSkoku = 0;
            }
            board.Move(fullMove, ulozit, zpet);
            ptrTah = board.HistoryMove.Count();
        }
        /// <summary>
        /// Metoda pro ověření zda je hráč PC
        /// </summary>
        /// <returns></returns>
        public bool IsPcPlayer()
        {
            if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0)
            {
                return true;
            }
            return false;
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
        /// Vrátí všechny tahy hráče na tahu
        /// </summary>
        /// <returns></returns>
        public List<int[]> AllPossibleMoves()
        {
            return rules.GetMovesList();
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
                board.VypocitejTahyBezSkoku(ptrTah);
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
                board.VypocitejTahyBezSkoku(ptrTah);
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
        /// <summary>
        /// Metoda pro uložení hry
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="historie"></param>
        /// <returns></returns>
        public bool SaveGame(string fileName, int player1, int player2, List<int[]> historie)
        {
            if(data.SaveGame(fileName,player1, player2, ptrTah, board.HistoryMove))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Metoda pro ověření zda hra skončila
        /// </summary>
        /// <returns></returns>
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
            if (board.tahuBezSkoku == 30)
            {
                return true;
            }
            return false;
        }

        public List<int[]>ListTahu()
        {
            return rules.ListMove;
        }

        public int[] BestMove(BackgroundWorker bw)
        {
            brain = new Brain(board, rules, bw);
            //bw.RunWorkerAsync();
            return brain.GetBestMove(4);
        }
    }
}
