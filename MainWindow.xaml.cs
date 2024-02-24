using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TravelingSalesman.ViewModel;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace TravelingSalesman;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;
    public MainWindow()
    {
        InitializeComponent();
        viewModel = new MainWindowViewModel();
        DataContext = viewModel;
    }

    private void DrawButton_Click(object sender, RoutedEventArgs e)
    {
        canvas.Children.Clear();
        List<Point> points = viewModel.GeneratePointArray();
        DrawPath(points);
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        List<Point> path = viewModel.ShortestPath();
        DrawPath(path);
    }

    private void DrawPath(List<Point> path)
    {
        canvas.Children.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            Ellipse circle = new Ellipse();
            circle.Height = circle.Width = 10;
            circle.Fill = Brushes.Red;
            Line line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            canvas.Children.Add(circle);
            Canvas.SetTop(circle, path[i].Y - 5);
            Canvas.SetLeft(circle, path[i].X - 5);
            if (i == 0) continue;

            line.X1 = path[i - 1].X;
            line.Y1 = path[i - 1].Y;
            line.X2 = path[i].X;
            line.Y2 = path[i].Y;
            canvas.Children.Add(line);

            if (i == path.Count - 1)
            {
                Line lastLine = new Line();
                lastLine.Stroke = Brushes.Black;
                lastLine.StrokeThickness = 2;
                lastLine.X1 = path[0].X;
                lastLine.Y1 = path[0].Y;
                lastLine.X2 = path[i].X;
                lastLine.Y2 = path[i].Y;
                canvas.Children.Add(lastLine);
            }

        }
    }
}
