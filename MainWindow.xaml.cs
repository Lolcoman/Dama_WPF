using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dama_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        GameController GameController = new GameController();
        private bool IsSelected = false;
        private bool IsPossible = false;
        Ellipse SelectFigure = new Ellipse();
        private int round = 0;


        public MainWindow()
        {
            InitializeComponent();
            GameController.InitGame();
            ShowBoard();
            PlayerOnMove();
            Rounds();
        }
        public void PcPlaying()
        {
            ProgressBar.Value = 30;
            ProgressBar.Maximum = 101;
            GameController.PcPlayer();
        }
        /// <summary>
        /// Výpis labelu jaký hráč je na tahu
        /// </summary>
        public void PlayerOnMove()
        {
            OnMoveLabel.Content = GameController.GetPlayerOnMove() == 1 ? " Hraje BÍLÝ " : " Hraje ČERNÝ ";
            OnMoveLabel.Width = GameController.GetPlayerOnMove() == 1 ? 112 : 150;
            OnMoveLabel.Foreground = GameController.GetPlayerOnMove() == 1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Výpis labelu pro počet kol
        /// </summary>
        public void Rounds()
        {
            RoundsLabel.Content = (" Počet kol: " + (round = GameController.HistorieTahu().Count / 2));
        }
        /// <summary>
        /// Vytvoření historie tahů
        /// </summary>
        public void HistorieTahu()
        {
            HistorieList.Items.Clear();
            foreach (int[] move in GameController.HistorieTahu())
            {
                string tah = GameController.HistorieNaString(move);
                TextBlock text = new TextBlock();
                if (move.Length > 8)
                {
                    text.Background = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                }
                text.FontFamily = new FontFamily("Comic Sans MS");
                text.Text = tah;
                HistorieList.Items.Add(text);
                //HistorieList.Items.Insert(0,text); poslední tah bude nahoře
            }
        }
        /// <summary>
        /// Ukončení aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitGameMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Opravdu chcete ukončit hru?", "Ukončit hru", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        /// <summary>
        /// Načítání hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadGameMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "Save game file (.txt)|*.txt";

            if (dialog.ShowDialog() == true)
            {
                //zde se bude načítat hra z .txt
                //game.Load(dialog.FileName)
                if (GameController.LoadGame(dialog))
                {
                    //Hra se nečetla dobře
                    HistorieTahu();
                    ShowBoard();
                }
                else
                {
                    //Hra se nenačetla
                    MessageBox.Show("Chyba při načítání souboru!");
                }
            }
        }
        /// <summary>
        /// Nová hra
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameMenu_Click(object sender, RoutedEventArgs e)
        {
            NewGame newGame = new NewGame();
            newGame.Owner = this; //hlavní okno je vlastníkem tohoto okna
            newGame.ShowDialog();
            if (newGame.IsCreated)
            {
                GameController.player1 = newGame.GetPlayer1(); //získání a nastavení player1
                GameController.player2 = newGame.GetPlayer2(); //získání a nastavení player2
                GameController.InitGame(); //inicializace hry
                GameController.HistorieTahu().Clear(); //smazání historie
                if (IsPcPlay()) //ověření zda byl vybrán PC
                {
                    PcPlaying();
                }
                round = GameController.HistorieTahu().Count / 2; //nastavení počtu kol
                Rounds(); //label pro kola
                HistorieTahu(); //výpis historie
                PlayerOnMove(); //výpis hráče na tahu
                ShowBoard(); //vykreslení desky
            }
        }
        /// <summary>
        /// Zobrazení desky
        /// </summary>
        public void ShowBoard()
        {
            BoardCanvas.Children.Clear(); //smazání všeho na Canvasu
            int fieldWidth = GetFieldWidth(); //získání šířky pole
            int fieldHeight = GetFieldHeight(); //získání výšky pole
            List<int> coords = GetFieldCoords(fieldWidth,fieldHeight); //Vrátí souřadnice rohů jednotlivých políček
            DrawBoard(coords,fieldWidth,fieldHeight); //vykreslí desku
            DrawFigures(coords, fieldWidth, fieldHeight); //vykreslí figurky
        }

        /// <summary>
        /// Vykreslí figurky na board
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        public void DrawFigures(List<int> coords, int fWidth, int fHeight)
        {
            for (int posY = 0; posY < 8; posY++)
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    //int[] phyCoords = TransfFieldPhyCoords(coords, i, j); //TADY JE CHYBA VYKRESLOVÁNÍ!!!!!
                    DrawFigure(GameController.GetValueOnBoard(posX,posY), posX, posY, fWidth, fHeight);
                }
            }
        }
        /// <summary>
        /// Převod souřadnic logických na fyzické
        /// </summary>
        /// <param name="phyCoords"></param>
        /// <param name="radek"></param>
        /// <param name="sloupec"></param>
        /// <returns></returns>
        public int[] TransfFieldPhyCoords(List<int> phyCoords, int radek, int sloupec)
        {
            int[] result = new int[2];
            int x = (radek * 8 + sloupec) * 2; 
            int y = x + 1;
            result[0] = phyCoords[x];
            result[1] = phyCoords[y];
            return result;
        }
        /// <summary>
        /// Vykreslí jednu figurku
        /// </summary>
        /// <param name="type"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        public void DrawFigure(int type, int posX, int posY, int fWidth, int fHeight)
        {
            SolidColorBrush border = new SolidColorBrush(Colors.Black);
            SolidColorBrush c = new SolidColorBrush(Colors.GhostWhite);
            if (type < 0) //vykreslení černého
            {
                c = new SolidColorBrush(Colors.Gray);
                DrawEllipse(posX * fWidth, posY * fHeight, fWidth, fHeight, c, border); //vynásobení šířkou poziceX a výškou poziceY
                if (type == -2) //jestli je dáma
                {
                    DrawQueen(posX * fWidth, posY * fHeight, fWidth, fHeight, "cerna");
                }
            }
            if (type > 0) //vykreslní bílého
            {
                DrawEllipse(posX * fWidth, posY * fHeight, fWidth, fHeight, c, border);
                if (type == 2) //jetli je dáma
                {
                    DrawQueen(posX * fWidth, posY * fHeight, fWidth, fHeight, "bila");
                }
            }

        }
        /// <summary>
        /// Kreslení dámy
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        /// <param name="brush"></param>
        /// <param name="stroke"></param>
        public void DrawQueen(int posX, int posY, int fWidth, int fHeight,string typ)
        {
            TextBlock crown = new TextBlock(); //vykreslení korunky pro BílouDámu
            crown.FontFamily = new FontFamily("Arial");
            crown.FontSize = 45;
            if (typ == "bila")
            {
                crown.Text = "\u2655";
            }
            else
            {
                crown.Text = "\u265b";
            }
            BoardCanvas.Children.Add(crown);
            Canvas.SetLeft(crown, posX + 15); //Nastavení levé souřadnice Canvasu na posX
            Canvas.SetBottom(crown, posY + 13); //Nastavení odspodu souřadnice Canvasu na posX
        }
        /// <summary>
        /// Kreslení jedné ellipsy
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        /// <param name="fill"></param>
        /// <param name="stroke"></param>
        public void DrawEllipse(int posX, int posY, int fWidth, int fHeight, SolidColorBrush fill, SolidColorBrush stroke)
        {
            Ellipse el = new Ellipse();
            el.Fill = fill;
            el.Stroke = stroke;
            el.Width = fWidth - 20;
            el.Height = fHeight - 20;
            BoardCanvas.Children.Add(el);
            Canvas.SetLeft(el, posX + 10);
            Canvas.SetBottom(el, posY + 10);
        }
        /// <summary>
        /// Vykreslení herní desky
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        public void DrawBoard(List<int> coords, int fWidth, int fHeight)
        {
            SolidColorBrush black = new SolidColorBrush(Colors.Black);
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            int sloupec = 0; 
            for (int i = 0; i < coords.Count; i = i + 2) //Count = 128
            {
                SolidColorBrush brush;
                if ((i%4)/2 == sloupec % 2) //pokud i%4/2 je sloupec % 2 tak je černý tj. (0%4)/2 je 0 a 0 % 2 je 0, 0 == 0
                {
                    brush = black;
                }
                else
                {
                    brush = white;
                }
                DrawField(coords[i], coords[i + 1], fWidth, fHeight,brush);
                if (i % 16 == 14) // 14 % 16 == 14
                {
                    sloupec++;
                }
            }
        }
        /// <summary>
        /// Vykreslení herního políčka na desce
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        /// <param name="color"></param>
        public void DrawField(int posX, int posY, int fWidth, int fHeight, SolidColorBrush color)
        {
            Rectangle r = new Rectangle();
            r.Width = fWidth;
            r.Height = fHeight;
            r.Fill = color;
            //r.ToolTip = ("X:" + posX / 100, "Y:" + posY / 100);
            //r.ToolTip = GameController.GetValueOnBoard(posX/100, posY/100);
            r.Stroke = Brushes.Black;
            r.StrokeThickness = 1;
            BoardCanvas.Children.Add(r);
            Canvas.SetLeft(r, posX); //Nastavení levé souřadnice Canvasu na posX
            Canvas.SetBottom(r, posY); //Nastavení odspodu souřadnice Canvasu na posX
        }
        /// <summary>
        /// Vrátí šířku políčka na Canvasu
        /// </summary>
        /// <returns></returns>
        public int GetFieldWidth()
        {
            return (int)(BoardCanvas.Width / 8);
        }
        /// <summary>
        /// Vrátí výšku políčka na Canvasu
        /// </summary>
        /// <returns></returns>
        public int GetFieldHeight()
        {
            return (int)(BoardCanvas.Height / 8);
        }
        /// <summary>
        /// Získání souřadnic z šířky a výšky políčka
        /// </summary>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        /// <returns></returns>
        public List<int> GetFieldCoords(int fWidth, int fHeight)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    result.Add(j * fWidth);
                    result.Add(i * fHeight);
                }
            }
            return result;
        }

        int[] prvniCast = null;
        int[] druhaCast = null;
        int[] pohyb = null;
        int[] plnyPohyb = null;
        /// <summary>
        /// Klik na Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BoardCanvas.Children.Remove(SelectFigure);
            int clickX = (int)e.GetPosition(BoardCanvas).X;
            int clickY = (int)e.GetPosition(BoardCanvas).Y;

            int[] boardCoords = TransfClickPhyCoords(clickX, clickY); //přepočítání fyzického kliku na souřadnice na desce, 1.kliknutí
            try
            {
                if (!IsSelected) //Pokud nemám vybranou figurku
                {
                    if (GameController.GetValueOnBoard(boardCoords[0], boardCoords[1]) > 0 && GameController.GetPlayerOnMove() > 0 || GameController.GetValueOnBoard(boardCoords[0], boardCoords[1]) < 0 && GameController.GetPlayerOnMove() < 0)
                    {
                        List<int> coords = GetFieldCoords(GetFieldWidth(), GetFieldHeight());
                        if (IsPossible)
                        {
                            List<int[]> moves = GameController.GetPossibleMoves(boardCoords[0], boardCoords[1]); //pozice možných tahů figurky
                            SolidColorBrush poss = new SolidColorBrush(Color.FromArgb(80, 0, 255, 0)); //barva tahů
                            foreach (int[] item in moves) //vykreslení pro každý tah
                            {
                                int[] recCoor = TransfFieldPhyCoords(coords, item[4], item[5]); //převod na fyzické souřadnice
                                if (item.Length > 8) //pokud je to skok, barva červená
                                {
                                    int[] recCoor1 = TransfFieldPhyCoords(coords, item[8], item[9]);
                                    poss = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                                    DrawField(recCoor1[1], recCoor1[0], GetFieldWidth(), GetFieldHeight(), poss);
                                }
                                else
                                {
                                    DrawField(recCoor[1], recCoor[0], GetFieldWidth(), GetFieldHeight(), poss);
                                }
                            }
                        }
                        //VYKRESLENÍ ZELENÉHO OZNAČENÍ VYBRANÉ FIGURKY
                        int[] elCoor = TransfFieldPhyCoords(coords, boardCoords[0], boardCoords[1]); //převod kliknutí na fyzické
                        SelectFigure.Width = GetFieldWidth() - 20;
                        SelectFigure.Height = GetFieldHeight() - 20; 
                        SelectFigure.Fill = new SolidColorBrush(Color.FromArgb(155, 0, 255, 0));
                        BoardCanvas.Children.Add(SelectFigure);
                        Canvas.SetLeft(SelectFigure, elCoor[1] + 10); //Nastavení levé souřadnice Canvasu na posX
                        Canvas.SetBottom(SelectFigure, elCoor[0] + 10); //Nastavení odspodu souřadnice Canvasu na posY
                    }
                    prvniCast = PrvniCast(boardCoords[0], boardCoords[1]); //uložení prvních souřadnic
                    IsSelected = true; //nastavení že jsem první klik provedl
                }
                else
                {
                    druhaCast = DruhaCast(boardCoords[0], boardCoords[1]); //Uložení druhé části kliku
                    pohyb = Spoj(prvniCast, druhaCast); //spojení a vytvoření tahu
                    plnyPohyb = GameController.FullMove(pohyb); //převod na plný pohyb
                    GameController.MakeMove(plnyPohyb, true, false); //provedení TAHU
                    ShowBoard(); //překreslení desky
                    IsSelected = false; //nastavení že nemám vybráno nic
                    GameController.NextPlayer(); //přepnutí hráče na tahu
                    HistorieTahu(); //vykreslení historie tahu
                    PlayerOnMove(); //label hráč na tahu
                    Rounds(); //label počet kol

                    if (IsPcPlay()) //Počítač
                    {
                        GameController.PcPlayer(); //odehraje PC a změní hráče na tahu
                        ShowBoard();
                        HistorieTahu(); //vykreslení historie tahu
                        PlayerOnMove(); //label hráč na tahu
                        Rounds();
                    }
                    //GameController.PcPlayer();
                    //ShowBoard();
                    //GameController.NextPlayer(); //přepnutí hráče na tahu
                    //HistorieTahu(); //vykreslení historie tahu
                    //PlayerOnMove(); //label hráč na tahu
                    //Rounds();

                    //HistorieTahu();
                    //MessageBox.Show($"Tah z políček: {fullMove[0]},{fullMove[1]}. Na políčka: {fullMove[2]},{fullMove[3]}.");
                }
            }catch
            {
                ShowBoard(); //překreslení kvůli špatnému výběru
                MessageBox.Show("Opakuj celý tah!", "Špatný výběr");
                IsSelected = false; //nastavení výběru na false, pro celý výběř znovu
            }
            if (GameController.HistorieTahu().Count > 1)
            {
                GameController.ClearHistoryFromToEnd(); //při provedení undo,redo smazání historie
            }
            plnyPohyb = null;
        }
        /// <summary>
        /// Přepočet kliknutí souřadnic
        /// </summary>
        /// <param name="clickX"></param>
        /// <param name="clickY"></param>
        /// <returns></returns>
        public int[] TransfClickPhyCoords(int clickX, int clickY)
        {
            clickY = (int)BoardCanvas.Height - clickY; //kliknutí 
            List<int> coords = GetFieldCoords(GetFieldWidth(), GetFieldHeight());
            int coordI = 7; 
            int coordJ = 7;
            for (int i = 0; i < coords.Count; i = i + 16)
            {
                if (clickY < coords[i + 1])
                {
                    coordI = (i / 16) - 1;
                    break;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (clickX < coords[i * 2])
                {
                    coordJ = (i % 8) - 1;
                    break;
                }
            }
            if (GameController.IsValidCoords(coordI,coordJ))
            {
                return new int[] { coordJ, coordI };
            }
            return null;
        }
        /// <summary>
        /// Prvni cast kliku
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <returns></returns>
        public int[] PrvniCast(int X1, int Y1)
        {
            return new int[] { X1, Y1 };
        }
        /// <summary>
        /// Druha cast kliku
        /// </summary>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <returns></returns>
        public int[] DruhaCast(int X2, int Y2)
        {
            return new int[] { X2, Y2 };
        }
        /// <summary>
        /// Spojení kliku
        /// </summary>
        /// <param name="prvni"></param>
        /// <param name="druha"></param>
        /// <returns></returns>
        public int[] Spoj(int[] prvni, int[] druha)
        {
            int[] spojeny = prvni.Concat(druha).ToArray();
            return spojeny;
        }
        /// <summary>
        /// Tlačítko pro tah zpět
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoMenu_Click(object sender, RoutedEventArgs e)
        {
            GameController.UndoMove();
            PlayerOnMove();
            Rounds();
            ShowBoard();
            if (GameController.ptrTah == 0 && IsPcPlay())
            {
                GameController.PcPlayer();
            }
        }
        /// <summary>
        /// Tlačítko pro tah vpřed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoMenu_Click(object sender, RoutedEventArgs e)
        {
            GameController.RedoMove();
            PlayerOnMove();
            Rounds();
            ShowBoard();
        }

        /// <summary>
        /// Zapnutí nápovědy pro jeden kámen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpForOneStoneMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPossible)
            {
                IsPossible = true;
            }
            else
            {
                IsPossible = false;
            }
        }

        /// <summary>
        /// Uložení hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveGameMenu_Click(object sender, RoutedEventArgs e)
        {
            if(GameController.SaveGame(GameController.player1, GameController.player2,GameController.HistorieTahu()))
            {
                MessageBox.Show("Hra byla uložena!", "Uložení hry");
            }
        }
        
        public bool IsPcPlay()
        {
            if (GameController.IsPcPlayer())
            {
                //GameController.PcPlayer();
                return true;
            }
            return false;
        }
    }
}
