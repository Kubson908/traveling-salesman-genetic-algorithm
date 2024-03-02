using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TravelingSalesman.ViewModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using TravelingSalesman.Services;

namespace TravelingSalesman;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;
    private double? distance;
    public MainWindow()
    {
        InitializeComponent();
        viewModel = new MainWindowViewModel();
        DataContext = viewModel;
        distance = null;
        viewModel.DrawPath += ViewModel_DrawPath;
    }

    private void ViewModel_DrawPath(object? sender, System.EventArgs e)
    {
        DrawPath(((NewBestIndividualEventArgs)e).Path);
    }

    private void DrawButton_Click(object sender, RoutedEventArgs e)
    {
        canvas.Children.Clear();
        List<Point> path = viewModel.GeneratePointArray();
        distance = null;
        viewModel.SetDistance(path);
        DrawPath(path);
    }

    private async void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.IsBusy) return;
        List<Point> newPath = await viewModel.ShortestPath();
        if (distance == null || viewModel.Distance < distance)
        {
            distance = viewModel.Distance;
            DrawPath(newPath);
        }
    }

    private void DrawPath(List<Point> path)
    {
        canvas.Children.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            Ellipse circle = new()
            {
                Height = 10,
                Width = 10,
                Fill = Brushes.Red

            };
            canvas.Children.Add(circle);
            Canvas.SetTop(circle, path[i].Y - 5);
            Canvas.SetLeft(circle, path[i].X - 5);

            if (i == 0) continue;

            Line line = new()
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 2,
                X1 = path[i - 1].X,
                Y1 = path[i - 1].Y,
                X2 = path[i].X,
                Y2 = path[i].Y
            };
            canvas.Children.Add(line);

            if (i == path.Count - 1)
            {
                Line lastLine = new()
                {
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 2,
                    X1 = path[0].X,
                    Y1 = path[0].Y,
                    X2 = path[i].X,
                    Y2 = path[i].Y
                };
                canvas.Children.Add(lastLine);
                /*AddCoords(ref canvas, path[0]);*/
            }

            /*AddCoords(ref canvas, path[i]);*/
        }

        for (int i = 0; i < path.Count; i++)
        {
            AddCoords(ref canvas, path[i]);
        }
    }

    private void AddCoords(ref Canvas canvas, Point point)
    {
        TextBlock coords = new()
        {
            Text = $"({point.X}, {point.Y})",
            Foreground = Brushes.DarkRed,
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Background = Brushes.DarkGray,
        };
        canvas.Children.Add(coords);
        Canvas.SetTop(coords, point.Y + 10);
        Canvas.SetLeft(coords, point.X - 5);

    }
}
