using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public class Game
    {
        //cuanto mide el cuadradito
        public int scale = 10;
        public int lengtMap = 35;
        private int[,] Squares;
        private List<Square> Snake;

        public enum Direction { Right, Down, Left, Up }
        public Direction ActualDirection = Direction.Right; //cuando comienza el juego es hacia la derecha
        private Square Food = null;
        private Random oRandom = new Random(); //es alternativo en donde aparece la comida
        private int Points = 0; //suma puntos en la medida que come

        PictureBox oPictureBox;
        Label labelPoint; 

        private int InitialPositionX
        {
            get
            {
                return lengtMap / 2;
            }
        }
        private int InitialPositionY
        {
            get
            {
                return lengtMap / 2;
            }
        }
        //evalua si ya perdiste
        public bool IsLot
        {
            get
            {
                foreach(var oSquare in Snake)
                {
                    if (Snake.Where(d => d.Y == oSquare.Y && d.X == oSquare.X && oSquare != d).Count() > 0)
                        return true;
                }
                return false;
            }
        }

        //se necesita el PictureBox para pintarlo
        public Game(PictureBox oPictureBox, Label labelPoint)
        {
            this.oPictureBox = oPictureBox;
            this.labelPoint = labelPoint;
            Reset();
        }

        //limpiar el mapa o reiniciarlo
        public void Reset()
        {
            Snake = new List<Square>();
            Square oInitialSquare = new Square(InitialPositionX,InitialPositionY);
            Snake.Add(oInitialSquare); //metodo de lista

            Squares = new int[lengtMap, lengtMap];
            for(int j=0;j<lengtMap;j++)
            {
                for (int i=0; i<lengtMap;i++)
                {
                    Squares[i, j] = 0;
                }
            }

            Points = 0;

        }

        //muestra los elementos pintandolos
        public void Show()
        {
            Bitmap bmp = new Bitmap(oPictureBox.Width, oPictureBox.Height);
            for(int j = 0; j < lengtMap; j++)
            {
                for (int i = 0; i < lengtMap; i++)
                {
                    if (Snake.Where(d => d.X == i && d.Y == j).Count() > 0)
                        PaintPixel(bmp, i, j, Color.Green);
                    else
                        PaintPixel(bmp, i, j, Color.Black);
                }
            }

            //mostramos la comida
            if(Food!=null)
            PaintPixel(bmp, Food.X, Food.Y, Color.Red);

            oPictureBox.Image = bmp;

            labelPoint.Text = Points.ToString();
        }
        
        public void Next() //manipulo situaciones
        {
            if (Food == null) //la comida existe si no se la comio la viborita
                GetFood();

            GetHistorySnake();
            switch (ActualDirection)
            { //comienza por la derecha y en vez de chocarse con los bordes
                case Direction.Right:
                    {
                        if (Snake[0].X == (lengtMap - 1))
                            Snake[0].X = 0; 
                        else
                            Snake[0].X++;
                        break;
                    }
                case Direction.Left:
                    {
                        if (Snake[0].X == 0)
                            Snake[0].X = lengtMap-1;
                        else
                            Snake[0].X--;
                        break;
                    }
                case Direction.Down:
                    {
                        if (Snake[0].Y == (lengtMap - 1))
                            Snake[0].Y = 0;
                        else
                            Snake[0].Y++;
                        break;
                    }
                case Direction.Up:
                    {
                        if (Snake[0].Y == 0)
                            Snake[0].Y = lengtMap-1;
                        else
                            Snake[0].Y--;
                        break;
                    }
            }

            GetNextMoveSnake();

            SnakeEating();
        }
        //va sumando posiciones
        private void GetNextMoveSnake()
        {
            if (Snake.Count > 1)
            for (int i = 1; i < Snake.Count;i++)
            {
                Snake[i].X = Snake[i - 1].X_old;
                Snake[i].Y = Snake[i - 1].Y_old;
            }
        }
        private void GetHistorySnake()
        {
            foreach (var oSquare in Snake) //siempre la viborita va a tener un elemento
            {
                oSquare.X_old = oSquare.X;
                oSquare.Y_old = oSquare.Y;
            }
        }
        private void SnakeEating()
        {
            if (Snake[0].X == Food.X && Snake[0].Y==Food.Y)
            {
                Food = null;
                Points++; //suma un punto

                //se asigna un nuevo elemento a la vibrorita cuando come
                Square LastSquare = Snake[Snake.Count - 1];
                Square oSquare = new Square(LastSquare.X_old,LastSquare.Y_old);
                Snake.Add(oSquare);
            }
        }
        //obtiene la comida
        private void GetFood()
        {
            int X = oRandom.Next(0, lengtMap - 1);
            int Y = oRandom.Next(0, lengtMap - 1);

            Food = new Square(X, Y);

        }
        //pinta 10 pixeles * 10 pixeles
        private void PaintPixel(Bitmap bmp,int x, int y, Color color)
        {
            for (int j = 0;j< scale; j++)
                for (int i = 0; i < scale; i++)
                    bmp.SetPixel(i + (x * scale), j + (y * scale),color);
        }
    }

    //representa los cuadros vivos de la viborita
    public class Square
    {
        public int X, Y, X_old, Y_old; //posiciones
        public Square(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.X_old = X; //guardamos la posicion original
            this.Y_old = Y;
        }
    }
}
