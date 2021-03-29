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
        public MainWindow()
        {
            InitializeComponent();
            GameController.InitGame();
            ShowBoard();
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
        private void LoadGameMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "Save game file|*.txt";
            if (dialog.ShowDialog() == true)
            {
                //zde se bude načítat hra z .txt
                //game.Load(dialog.FileName)
                MessageBox.Show(dialog.FileName);
            }
        }

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
                GameController.InitGame();
            }
        }
        /// <summary>
        /// Zobrazení desky
        /// </summary>
        public void ShowBoard()
        {
            BoardCanvas.Children.Clear(); //smazání všeho na Canvasu
            int fieldWidth = GetFieldWidth();
            int fieldHeight = GetFieldHeight();
            List<int> coords = GetFieldCoords(fieldWidth,fieldHeight); //Vrátí souřadnice rohů jednotlivých políček
            DrawBoard(coords,fieldWidth,fieldHeight);
            DrawFigures(coords, fieldWidth, fieldHeight);
        }





        public void DrawFigures(List<int> coords, int fWidth, int fHeight)
        {
            DrawFigure(1, 30, 30, fWidth, fHeight);
        }
        public void DrawFigure(int type, int posX, int posY, int fWidth, int fHeight)
        {
            SolidColorBrush border = new SolidColorBrush(Colors.Gray);
            SolidColorBrush c = new SolidColorBrush(Colors.Red);
            if (GameController.GetValueOnBoard(posX/100,posY/100) < 0)
            {
                c = new SolidColorBrush(Colors.LightBlue);
            }
            DrawEllipse(posX, posY, fWidth, fHeight, c,border);
            if (Math.Abs(GameController.GetValueOnBoard(posX / 100, posY / 100)) > 1) //pokud větší než 1 přijde dáma
            {
                DrawQueen(posX, posY, fWidth, fHeight, new SolidColorBrush(Colors.Green), border);
            }
        }

        public void DrawQueen(int posX, int posY, int fWidth, int fHeight, SolidColorBrush brush, SolidColorBrush stroke)
        {
            DrawEllipse(posX + fWidth / 4, posY + fHeight / 4, fWidth / 2, fHeight / 2, brush, stroke);
        }
        public void DrawEllipse(int posX, int posY, int fWidth, int fHeight, SolidColorBrush fill, SolidColorBrush stroke)
        {
            Ellipse el = new Ellipse();
            el.Fill = fill;
            el.Stroke = stroke;
            el.Width = fWidth;
            el.Height = fHeight;
            BoardCanvas.Children.Add(el);
            Canvas.SetLeft(el, posX);
            Canvas.SetBottom(el, posY);
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
            //DrawField(30, 30, fWidth, fHeight, new SolidColorBrush(Colors.Black)); //jedno pole
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
            r.Stroke = Brushes.Gray;
            r.StrokeThickness = 2;
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
    }
}
