using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace SmartCitySimulator
{
    public partial class MainWindow : Window
    {
        private bool _isRaining = false;
        private bool _isSnowing = false;
        private double _car1Position = 50;
        private double _car2Position = 50;
        private int _car1StopIndex = 0;
        private int _car2StopIndex = 0;
        private readonly double[] _stops = { 50, 188, 477, 638 };
        private DispatcherTimer _transportTimer;

        public MainWindow()
        {
            InitializeComponent();

            _transportTimer = new DispatcherTimer();
            _transportTimer.Interval = TimeSpan.FromSeconds(1);
            _transportTimer.Tick += TransportTimer_Tick;
        }

        private void ToggleLightsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in CityMap.Children)
            {
                if (element is Ellipse light)
                {
                    light.Fill = light.Fill == Brushes.Yellow ? Brushes.Gray : Brushes.Yellow;
                }
            }
        }

        private void StartTransportButton_Click(object sender, RoutedEventArgs e)
        {
            _transportTimer.Start();
        }

        private void ChangeWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeWeather();
        }

        private void TransportTimer_Tick(object sender, EventArgs e)
        {
            MoveCar(ref _car1Position, ref _car1StopIndex, Car1);
            MoveCar(ref _car2Position, ref _car2StopIndex, Car2);
        }

        private void MoveCar(ref double carPosition, ref int stopIndex, Rectangle car)
        {
            if (stopIndex < _stops.Length)
            {
                if (Math.Abs(carPosition - _stops[stopIndex]) < 5)
                {
                    stopIndex++;
                }
                else
                {
                    if (carPosition < _stops[stopIndex])
                    {
                        carPosition += 20;
                    }
                    else
                    {
                        carPosition -= 20;
                    }
                }
            }
            Canvas.SetLeft(car, carPosition);
        }

        private void ChangeWeather()
        {
            if (_isRaining)
            {
                StopRainEffect();
                _isRaining = false;
            }
            else if (_isSnowing)
            {
                StopSnowEffect();
                _isSnowing = false;
            }
            else
            {
                Random rand = new Random();
                int weatherCondition = rand.Next(2);
                if (weatherCondition == 0)
                {
                    StartRainEffect();
                    _isRaining = true;
                }
                else
                {
                    StartSnowEffect();
                    _isSnowing = true;
                }
            }
        }

        private void StartRainEffect()
        {
            for (int i = 0; i < 50; i++)
            {
                Line rainDrop = new Line
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    X1 = new Random().Next(0, (int)CityMap.Width),
                    Y1 = 0,
                    X2 = new Random().Next(0, (int)CityMap.Width),
                    Y2 = 20
                };

                CityMap.Children.Add(rainDrop);
                AnimateRainDrop(rainDrop);
            }
        }

        private void AnimateRainDrop(Line rainDrop)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = CityMap.Height,
                Duration = TimeSpan.FromSeconds(1),
                RepeatBehavior = RepeatBehavior.Forever
            };

            rainDrop.BeginAnimation(Canvas.TopProperty, animation);
        }

        private void StopRainEffect()
        {
            for (int i = CityMap.Children.Count - 1; i >= 0; i--)
            {
                if (CityMap.Children[i] is Line)
                {
                    CityMap.Children.RemoveAt(i);
                }
            }
        }

        private void StartSnowEffect()
        {
            for (int i = 0; i < 50; i++)
            {
                Ellipse snowFlake = new Ellipse
                {
                    Fill = Brushes.White,
                    Width = 5,
                    Height = 5
                };

                double left = new Random().Next(0, (int)(CityMap.Width - snowFlake.Width));
                double top = new Random().Next(0, (int)(CityMap.Height - snowFlake.Height));

                CityMap.Children.Add(snowFlake);
                Canvas.SetLeft(snowFlake, left);
                Canvas.SetTop(snowFlake, top);

                AnimateSnowFlake(snowFlake);
            }
        }

        private void AnimateSnowFlake(Ellipse snowFlake)
        {
            DoubleAnimation fallAnimation = new DoubleAnimation
            {
                To = CityMap.Height,
                Duration = TimeSpan.FromSeconds(3),
                RepeatBehavior = RepeatBehavior.Forever
            };

            snowFlake.BeginAnimation(Canvas.TopProperty, fallAnimation);
        }

        private void StopSnowEffect()
        {
            for (int i = CityMap.Children.Count - 1; i >= 0; i--)
            {
                if (CityMap.Children[i] is Ellipse snowFlake)
                {
                    CityMap.Children.RemoveAt(i);
                }
            }
        }
    }
}
