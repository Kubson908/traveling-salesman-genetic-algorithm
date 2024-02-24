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
        
        for (int i = 0; i < points.Count; i++) 
        {
            Ellipse circle = new Ellipse();
            circle.Height = circle.Width = 10;
            circle.Fill = Brushes.Red;
            Line line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            canvas.Children.Add(circle);
            Canvas.SetTop(circle, points[i].Y-5);
            Canvas.SetLeft(circle, points[i].X-5);
            if (i == 0) continue;

            line.X1 = points[i - 1].X;
            line.Y1 = points[i - 1].Y;
            line.X2 = points[i].X;
            line.Y2 = points[i].Y;
            canvas.Children.Add(line);

            if (i == points.Count - 1)
            {
                Line lastLine = new Line();
                lastLine.Stroke = Brushes.Black;
                lastLine.StrokeThickness = 2;
                lastLine.X1 = points[0].X;
                lastLine.Y1 = points[0].Y;
                lastLine.X2 = points[i].X;
                lastLine.Y2 = points[i].Y;
                canvas.Children.Add(lastLine);
            }
            
        }
    }

    private void PopulateButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.Populate();
    }
}
