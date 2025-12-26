// ════════════════════════════════════════════════════════════════════
// FpsChart.xaml.cs - Compact FPS chart with neon style
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RustControlPanel.Services;

namespace RustControlPanel.Views.Controls
{
    /// <summary>
    /// Compact FPS chart for TopBar with neon style and grid.
    /// </summary>
    public partial class FpsChart : UserControl
    {
        #region Constants

        private const int MAX_DATA_POINTS = 60; // 60 seconds
        private const double GRID_LINES = 5;

        #endregion

        #region Fields

        private readonly List<float> _fpsData = new();
        private readonly Polyline _chartLine;
        private readonly Polyline _chartGlow;
        private readonly Path _chartArea;
        private readonly Border _tooltip;
        private readonly TextBlock _tooltipText;

        #endregion

        #region Constructor

        public FpsChart()
        {
            InitializeComponent();

            // Create glow effect (wider line behind)
            _chartGlow = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 255, 255)), // Cyan glow
                StrokeThickness = 6,
                StrokeLineJoin = PenLineJoin.Round
            };

            // Create filled area under the line
            _chartArea = new Path
            {
                Fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb(60, 0, 255, 255), 0),
                        new GradientStop(Color.FromArgb(0, 0, 255, 255), 1)
                    }
                }
            };

            // Create main line
            _chartLine = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 255)), // Neon cyan
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            // Create tooltip
            _tooltipText = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(6, 3, 6, 3)
            };

            _tooltip = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(230, 26, 26, 26)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Child = _tooltipText,
                Visibility = Visibility.Collapsed
            };

            ChartCanvas.Children.Add(_chartArea);
            ChartCanvas.Children.Add(_chartGlow);
            ChartCanvas.Children.Add(_chartLine);
            ChartCanvas.Children.Add(_tooltip);

            // Mouse events for tooltip
            ChartCanvas.MouseMove += OnCanvasMouseMove;
            ChartCanvas.MouseLeave += OnCanvasMouseLeave;

            // Subscribe to updates
            ServerStatsService.Instance.ServerInfoUpdated += OnServerInfoUpdated;
            ChartCanvas.SizeChanged += OnCanvasSizeChanged;
        }

        #endregion

        #region Private Methods

        private void OnServerInfoUpdated(object? sender, Models.ServerInfo info)
        {
            Dispatcher.Invoke(() =>
            {
                _fpsData.Add(info.Fps);

                while (_fpsData.Count > MAX_DATA_POINTS)
                    _fpsData.RemoveAt(0);

                UpdateChart();
            });
        }

        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateChart();
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_fpsData.Count == 0)
                return;

            var pos = e.GetPosition(ChartCanvas);
            var width = ChartCanvas.ActualWidth;
            var xStep = width / (MAX_DATA_POINTS - 1);
            var index = (int)(pos.X / xStep);

            if (index >= 0 && index < _fpsData.Count)
            {
                var fps = _fpsData[index];
                var secondsAgo = _fpsData.Count - index - 1;
                
                _tooltipText.Text = $"{fps:F0} FPS ({secondsAgo}s ago)";
                _tooltip.Visibility = Visibility.Visible;

                Canvas.SetLeft(_tooltip, Math.Min(pos.X + 10, width - _tooltip.ActualWidth - 5));
                Canvas.SetTop(_tooltip, 5);
            }
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            _tooltip.Visibility = Visibility.Collapsed;
        }

        private void UpdateChart()
        {
            if (_fpsData.Count == 0 || ChartCanvas.ActualWidth == 0 || ChartCanvas.ActualHeight == 0)
                return;

            var width = ChartCanvas.ActualWidth;
            var height = ChartCanvas.ActualHeight;

            // Calculate range
            var minFps = _fpsData.Min();
            var maxFps = _fpsData.Max();
            var yMin = Math.Max(0, Math.Floor(minFps / 10) * 10 - 10);
            var yMax = Math.Ceiling(maxFps / 10) * 10 + 10;
            var yRange = yMax - yMin;

            if (yRange == 0) yRange = 1;

            // Clear existing grid lines
            ChartCanvas.Children.RemoveRange(0, ChartCanvas.Children.Count);
            
            // Draw grid
            DrawGrid(width, height, yMin, yMax);

            // Re-add chart elements
            ChartCanvas.Children.Add(_chartArea);
            ChartCanvas.Children.Add(_chartGlow);
            ChartCanvas.Children.Add(_chartLine);
            ChartCanvas.Children.Add(_tooltip);

            // Create points
            _chartLine.Points.Clear();
            _chartGlow.Points.Clear();
            var areaGeometry = new PathGeometry();
            var areaFigure = new PathFigure { StartPoint = new Point(0, height) };

            var xStep = width / (MAX_DATA_POINTS - 1);

            for (int i = 0; i < _fpsData.Count; i++)
            {
                var x = i * xStep;
                var normalizedY = (_fpsData[i] - yMin) / yRange;
                var y = height - (normalizedY * height);

                var point = new Point(x, y);
                _chartLine.Points.Add(point);
                _chartGlow.Points.Add(point);
                areaFigure.Segments.Add(new LineSegment(point, true));
            }

            // Close the area
            if (_fpsData.Count > 0)
            {
                areaFigure.Segments.Add(new LineSegment(new Point((_fpsData.Count - 1) * xStep, height), true));
            }

            areaGeometry.Figures.Add(areaFigure);
            _chartArea.Data = areaGeometry;
        }

        private void DrawGrid(double width, double height, double yMin, double yMax)
        {
            var gridBrush = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255));

            // Horizontal grid lines
            for (int i = 0; i <= GRID_LINES; i++)
            {
                var y = (height / GRID_LINES) * i;
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = gridBrush,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                };
                ChartCanvas.Children.Add(line);
            }

            // Vertical grid lines
            var verticalLines = 6;
            for (int i = 0; i <= verticalLines; i++)
            {
                var x = (width / verticalLines) * i;
                var line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = height,
                    Stroke = gridBrush,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                };
                ChartCanvas.Children.Add(line);
            }
        }

        #endregion
    }
}
