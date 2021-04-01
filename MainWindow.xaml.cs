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
       
        //int[] prvniCast = null;
        //int[] druhaCast = null;
        //int[] fullMove = null;
        public MainWindow()
        {
            InitializeComponent();
            GameController.InitGame();
            ShowBoard();
        }
        /// <summary>
        /// Vytvoření historie tahů
        /// </summary>
        public void HistorieTahu()
        {
            List<int[]> historie = GameController.HistorieTahu();
            int[] posledniVhistorii = historie[historie.Count-1];        
            string tah = GameController.HistorieNaString(posledniVhistorii);
            TextBox text = new TextBox();
            text.Text = tah;
            HistorieList.Items.Add(text);
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
                GameController.LoadGame(dialog);
                MessageBox.Show(dialog.FileName);
                ShowBoard();
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
                MessageBox.Show(newGame.GetPlayer1().ToString());
                MessageBox.Show(newGame.GetPlayer2().ToString());
                GameController.player1 = newGame.GetPlayer1();
                GameController.player2 = newGame.GetPlayer2();
                //GameController.InitGame();
                ShowBoard();
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
            DrawBoard(coords,fieldWidth,fieldHeight);
            DrawFigures(coords, fieldWidth, fieldHeight);
        }

        /// <summary>
        /// Vykreslí figurky na board
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="fWidth"></param>
        /// <param name="fHeight"></param>
        public void DrawFigures(List<int> coords, int fWidth, int fHeight)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int[] phyCoords = TransfFieldPhyCoords(coords, i, j);
                    DrawFigure(GameController.GetValueOnBoard(i,j), phyCoords[0], phyCoords[1], fWidth, fHeight);
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
            type = GameController.GetValueOnBoard(posX / 100, posY / 100); //uložení jaký typ se nachází na zvoleném poli
            if (GameController.GetValueOnBoard(posX / 100, posY / 100) != 0) //když není 0
            {
                SolidColorBrush border = new SolidColorBrush(Colors.Black);
                SolidColorBrush c = new SolidColorBrush(Colors.GhostWhite);
                if (type < 0) //vykreslení černého
                {
                    c = new SolidColorBrush(Colors.Gray);
                    DrawEllipse(posX, posY, fWidth, fHeight, c, border);
                    if (type == -2) //jestli je dáma
                    {
                        DrawQueen(posX, posY, fWidth, fHeight, "cerna");
                    }
                }
                if (type > 0) //vykreslní bílého
                {
                    DrawEllipse(posX, posY, fWidth, fHeight, c, border);
                    if (type == 2) //jetli je dáma
                    {
                        DrawQueen(posX, posY, fWidth, fHeight, "bila");
                    }
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
            if (typ == "bila")
            {
                TextBlock crown = new TextBlock //vykreslení korunky pro BílouDámu
                {
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 45,
                    Text = "\u2655"
                };
                BoardCanvas.Children.Add(crown);
                Canvas.SetLeft(crown, posX + 28); //Nastavení levé souřadnice Canvasu na posX
                Canvas.SetBottom(crown, posY + 25); //Nastavení odspodu souřadnice Canvasu na posX
            }
            if (typ == "cerna")
            {
                TextBlock crown = new TextBlock //vykreslení korunky pro ČernouDámu
                {
                    FontFamily = new FontFamily("Arial"),
                    FontSize = 45,
                    Text = "\u265b"
                };
                BoardCanvas.Children.Add(crown); //přidání do Canvasu
                Canvas.SetLeft(crown, posX + 28); //Nastavení levé souřadnice Canvasu na posX
                Canvas.SetBottom(crown, posY + 25); //Nastavení odspodu souřadnice Canvasu na posX
            }
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
            r.ToolTip = GameController.GetValueOnBoard(posX/100, posY/100);
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
            int clickX = (int)e.GetPosition(BoardCanvas).X;
            int clickY = (int)e.GetPosition(BoardCanvas).Y;

            int[] boardCoords = TransfClickPhyCoords(clickX, clickY); //přepočítání fyzického kliku na souřadnice na desce, 1.kliknutí

            if (!IsSelected) //Pokud nemám vybranou figurku
            {
                prvniCast = PrvniCast(boardCoords[0], boardCoords[1]);
                //MessageBox.Show($"Reálné souřadnice: {clickX},{clickY}. Přepočítané logické {boardCoords[0]},{boardCoords[1]}.");
                IsSelected = true;
            }
            else
            {
                druhaCast = DruhaCast(boardCoords[0], boardCoords[1]);
                pohyb = Spoj(prvniCast, druhaCast);
                plnyPohyb = GameController.FullMove(pohyb);
                GameController.MakeMove(plnyPohyb, true, false);
                ShowBoard();
                IsSelected = false;
                GameController.NextPlayer();
                HistorieTahu();
                //MessageBox.Show($"Tah z políček: {fullMove[0]},{fullMove[1]}. Na políčka: {fullMove[2]},{fullMove[3]}.");
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
    }
}
