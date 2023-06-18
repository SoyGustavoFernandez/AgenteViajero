using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AgenteViajero
{
    public partial class Form1 : Form
    {
        private List<Point> cities;
        private List<Point> path;
        private List<double> distances;
        private int cityCounter;
        private Font cityNumberFont;
        private Pen bestPathPen;
        private Font bestPathFont;
        private Brush bestPathBrush;
        private Brush bestPathDistanceBrush;
        private Rectangle bestPathDistanceRect;

        public Form1()
        {
            InitializeComponent();
            cities = new List<Point>();
            path = new List<Point>();
            distances = new List<double>();
            cityCounter = 0;
            cityNumberFont = new Font(Font.FontFamily, 8); // Tamaño de fuente para el número de la ciudad
            bestPathPen = new Pen(Color.Green, 2); // Línea verde más gruesa para el mejor recorrido
            bestPathFont = new Font(Font.FontFamily, 8, FontStyle.Bold); // Fuente en negrita para la distancia del mejor recorrido
            bestPathBrush = Brushes.Green; // Pincel verde para el texto del mejor recorrido
            bestPathDistanceBrush = Brushes.Yellow; // Pincel amarillo para el fondo del texto de la distancia del mejor recorrido
            bestPathDistanceRect = new Rectangle(0, 0, 40, 20); // Rectángulo para el fondo del texto de la distancia del mejor recorrido
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Dibujar las ciudades y los números de visita
            for (int i = 0; i < cities.Count; i++)
            {
                Point city = cities[i];
                string cityName = ((char)('A' + i)).ToString(); // Letra de la ciudad
                e.Graphics.FillEllipse(Brushes.Red, city.X - 5, city.Y - 5, 10, 10);
                e.Graphics.DrawString(cityName, Font, Brushes.Black, city.X - 5, city.Y - 20);

                if (path.Contains(city))
                {
                    int visitOrder = path.IndexOf(city);
                    e.Graphics.DrawString(visitOrder.ToString(), cityNumberFont, Brushes.Black, city.X + 10, city.Y);
                }
            }

            // Dibujar las líneas de todas las conexiones y mostrar las distancias
            for (int i = 0; i < cities.Count; i++)
            {
                Point currentCity = cities[i];
                for (int j = 0; j < cities.Count; j++)
                {
                    if (i != j)
                    {
                        Point nextCity = cities[j];
                        double distance = CalculateDistance(currentCity, nextCity);
                        string distanceText = distance.ToString("F0"); // Formatear la distancia como número entero
                        Point textPosition = new Point((currentCity.X + nextCity.X) / 2, (currentCity.Y + nextCity.Y) / 2);
                        e.Graphics.DrawLine(Pens.LightGray, currentCity, nextCity);
                        e.Graphics.DrawString(distanceText, Font, Brushes.Gray, textPosition);
                    }
                }
            }

            // Dibujar el mejor recorrido y mostrar la distancia
            for (int i = 0; i < path.Count - 1; i++)
            {
                Point currentCity = path[i];
                Point nextCity = path[i + 1];
                double distance = distances[i];
                string distanceText = distance.ToString("F0"); // Formatear la distancia como número entero
                Point textPosition = new Point((currentCity.X + nextCity.X) / 2, (currentCity.Y + nextCity.Y) / 2);
                e.Graphics.DrawLine(bestPathPen, currentCity, nextCity);
                e.Graphics.DrawString(distanceText, bestPathFont, bestPathBrush, textPosition);

                // Dibujar el cuadrado con el fondo de la distancia del mejor recorrido
                RectangleF rect = new RectangleF(textPosition.X - bestPathDistanceRect.Width / 2, textPosition.Y - bestPathDistanceRect.Height / 2, bestPathDistanceRect.Width, bestPathDistanceRect.Height);
                e.Graphics.FillRectangle(bestPathDistanceBrush, rect);

                // Dibujar la distancia del mejor recorrido en el cuadrado
                e.Graphics.DrawString(distanceText, bestPathFont, Brushes.Black, rect, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            // Añadir una nueva ciudad con las coordenadas del clic
            if (e.Button == MouseButtons.Left)
            {
                cities.Add(e.Location);
                Refresh();
            }
            // Calcular el camino óptimo
            else if (e.Button == MouseButtons.Right)
            {
                CalculateOptimalPath();
                Refresh();
            }
        }

        private void CalculateOptimalPath()
        {
            // Crear una lista auxiliar de ciudades no visitadas
            List<Point> unvisitedCities = new List<Point>(cities);

            // Encontrar la ciudad más cercana a la ciudad de partida
            Point currentCity = cities[0];
            unvisitedCities.Remove(currentCity);
            path.Clear();
            distances.Clear();
            path.Add(currentCity);

            while (unvisitedCities.Count > 0)
            {
                double minDistance = double.MaxValue;
                Point nearestCity = Point.Empty;

                foreach (Point city in unvisitedCities)
                {
                    double distance = CalculateDistance(currentCity, city);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCity = city;
                    }
                }

                currentCity = nearestCity;
                unvisitedCities.Remove(currentCity);
                path.Add(currentCity);
                distances.Add(minDistance);
            }

            // Regresar a la ciudad de partida
            path.Add(cities[0]);
            distances.Add(CalculateDistance(currentCity, cities[0]));
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
