// ════════════════════════════════════════════════════════════════════
// FpsChart.xaml.cs - Custom FPS chart control
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RustControlPanel.Services;

namespace RustControlPanel.Views.Controls
{
    /// <summary>
    /// Custom FPS chart control with real-time updates.
    /// Lightweight Canvas-based implementation.
    /// </summary>
    public partial class FpsChart : UserControl
    {
        #region Constants

        private const int MAX_DATA_POINTS = 60; // 60 seconds of data
        private const double CHART_PADDING = 10;

        #endregion

        #region Fields

        private readonly List<float> _fpsData = new();
        private readonly Polyline _chartLine;
        private readonly TextBlock _currentFpsText;
        private readonly TextBlock _avgFpsText;
        private readonly TextBlock _minFpsText;
        private readonly TextBlock _maxFpsText;

        #endregion

        #region Constructor

        public FpsChart()
        {
            InitializeComponent();

            // Create chart line
            _chartLine = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(59, 130, 246)), // Blue
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            ChartCanvas.Children.Add(_chartLine);

            // Create FPS labels
            _currentFpsText = CreateLabel("-- FPS", 0, 0);
            _avgFpsText = CreateLabel("Avg: --", 0, 20);
            _minFpsText = CreateLabel("Min: --", 0, 40);
            _maxFpsText = CreateLabel("Max: --", 0, 60);

            ChartCanvas.Children.Add(_currentFpsText);
            ChartCanvas.Children.Add(_avgFpsText);
            ChartCanvas.Children.Add(_minFpsText);
            ChartCanvas.Children.Add(_maxFpsText);

            // Subscribe to server stats
            ServerStatsService.Instance.ServerInfoUpdated += OnServerInfoUpdated;
            
            // Subscribe to size changed to redraw
            ChartCanvas.SizeChanged += OnCanvasSizeChanged;
        }

        #endregion

        #region Private Methods

        private TextBlock CreateLabel(string text, double left, double top)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
        }

        private void OnServerInfoUpdated(object? sender, Models.ServerInfo info)
        {
            Dispatcher.Invoke(() =>
            {
                // Add new data
                _fpsData.Add(info.Fps);

                // Keep only last 60 points
                while (_fpsData.Count > MAX_DATA_POINTS)
                {
                    _fpsData.RemoveAt(0);
                }

                // Update chart
                UpdateChart();
            });
        }

        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (_fpsData.Count == 0 || ChartCanvas.ActualWidth == 0 || ChartCanvas.ActualHeight == 0)
                return;

            var width = ChartCanvas.ActualWidth;
            var height = ChartCanvas.ActualHeight;

            // Calculate stats
            var currentFps = _fpsData.Last();
            var avgFps = _fpsData.Average();
            var minFps = _fpsData.Min();
            var maxFps = _fpsData.Max();

            // Update labels
            _currentFpsText.Text = $"{currentFps:F0} FPS";
            _avgFpsText.Text = $"Avg: {avgFps:F0}";
            _minFpsText.Text = $"Min: {minFps:F0}";
            _maxFpsText.Text = $"Max: {maxFps:F0}";

            // Position labels in top-left
            Canvas.SetLeft(_currentFpsText, CHART_PADDING);
            Canvas.SetTop(_currentFpsText, CHART_PADDING);
            Canvas.SetLeft(_avgFpsText, CHART_PADDING);
            Canvas.SetTop(_avgFpsText, CHART_PADDING + 20);
            Canvas.SetLeft(_minFpsText, CHART_PADDING);
            Canvas.SetTop(_minFpsText, CHART_PADDING + 40);
            Canvas.SetLeft(_maxFpsText, CHART_PADDING);
            Canvas.SetTop(_maxFpsText, CHART_PADDING + 60);

            // Calculate Y range
            var yMin = Math.Max(0, minFps - 10);
            var yMax = maxFps + 10;
            var yRange = yMax - yMin;

            if (yRange == 0)
                yRange = 1;

            // Create points for polyline
            _chartLine.Points.Clear();

            var xStep = width / (MAX_DATA_POINTS - 1);

            for (int i = 0; i < _fpsData.Count; i++)
            {
                var x = i * xStep;
                var normalizedY = (_fpsData[i] - yMin) / yRange;
                var y = height - (normalizedY * (height - CHART_PADDING * 2)) - CHART_PADDING;

                _chartLine.Points.Add(new Point(x, y));
            }
        }

        #endregion
    }
}
