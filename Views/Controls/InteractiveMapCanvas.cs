// ════════════════════════════════════════════════════════════════════
// InteractiveMapCanvas.cs - Interactive map with zoom, pan, and entities
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RustControlPanel.Models;

namespace RustControlPanel.Views.Controls
{
    /// <summary>
    /// Interactive map canvas with zoom, pan, and entity rendering.
    /// Supports team highlighting with purple glow for teammates.
    /// </summary>
    public class InteractiveMapCanvas : Canvas
    {
        #region Fields

        private ToolTip? _tooltip;
        private DispatcherTimer? _refreshTimer;
        private Point _panOrigin;
        private Point _panOffset;
        private bool _isPanning;
        private MapEntity? _hoveredEntity;
        private double _renderWidth;
        private double _renderHeight;
        private double _offsetX;
        private double _offsetY;
        private double _bladeRotation = 0;

        // Preloaded icons
        private BitmapImage? _heliIcon;
        private BitmapImage? _chinookBodyIcon;
        private BitmapImage? _chinookBladesIcon;
        private BitmapImage? _cargoIcon;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MapImageProperty =
            DependencyProperty.Register(nameof(MapImage), typeof(BitmapImage), typeof(InteractiveMapCanvas),
                new PropertyMetadata(null, OnVisualPropertyChanged));

        public static readonly DependencyProperty EntitiesProperty =
            DependencyProperty.Register(nameof(Entities), typeof(ObservableCollection<MapEntity>), typeof(InteractiveMapCanvas),
                new PropertyMetadata(null, OnEntitiesChanged));

        public static readonly DependencyProperty MonumentsProperty =
            DependencyProperty.Register(nameof(Monuments), typeof(ObservableCollection<Monument>), typeof(InteractiveMapCanvas),
                new PropertyMetadata(null, OnVisualPropertyChanged));

        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(InteractiveMapCanvas),
                new PropertyMetadata(false, OnVisualPropertyChanged));

        public static readonly DependencyProperty ShowMonumentsProperty =
            DependencyProperty.Register(nameof(ShowMonuments), typeof(bool), typeof(InteractiveMapCanvas),
                new PropertyMetadata(true, OnVisualPropertyChanged));

        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(InteractiveMapCanvas),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVisualPropertyChanged));

        public static readonly DependencyProperty WorldSizeProperty =
            DependencyProperty.Register(nameof(WorldSize), typeof(uint), typeof(InteractiveMapCanvas),
                new PropertyMetadata((uint)4000));

        public static readonly DependencyProperty SelectedPlayerProperty =
            DependencyProperty.Register(nameof(SelectedPlayer), typeof(PlayerDetails), typeof(InteractiveMapCanvas),
                new PropertyMetadata(null, OnSelectedPlayerChanged));

        public BitmapImage? MapImage
        {
            get => (BitmapImage?)GetValue(MapImageProperty);
            set => SetValue(MapImageProperty, value);
        }

        public ObservableCollection<MapEntity>? Entities
        {
            get => (ObservableCollection<MapEntity>?)GetValue(EntitiesProperty);
            set => SetValue(EntitiesProperty, value);
        }

        public ObservableCollection<Monument>? Monuments
        {
            get => (ObservableCollection<Monument>?)GetValue(MonumentsProperty);
            set => SetValue(MonumentsProperty, value);
        }

        public bool ShowGrid
        {
            get => (bool)GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }

        public bool ShowMonuments
        {
            get => (bool)GetValue(ShowMonumentsProperty);
            set => SetValue(ShowMonumentsProperty, value);
        }

        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }

        public uint WorldSize
        {
            get => (uint)GetValue(WorldSizeProperty);
            set => SetValue(WorldSizeProperty, value);
        }

        public PlayerDetails? SelectedPlayer
        {
            get => (PlayerDetails?)GetValue(SelectedPlayerProperty);
            set => SetValue(SelectedPlayerProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<MapEntity>? EntityHovered;
        public event EventHandler<MapEntity>? EntityClicked;

        #endregion

        #region Constructor

        public InteractiveMapCanvas()
        {
            ClipToBounds = true;
            Background = new SolidColorBrush(Color.FromRgb(26, 26, 26));

            InitializeTooltip();
            LoadIcons();

            // Mouse events
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;
            MouseWheel += OnMouseWheel;
            MouseLeave += (s, e) => { if (_tooltip != null) _tooltip.IsOpen = false; _hoveredEntity = null; };

            // Refresh timer for animations (chinook blades)
            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _refreshTimer.Tick += (s, e) =>
            {
                _bladeRotation = (_bladeRotation + 15) % 360;
                InvalidateVisual();
            };

            Loaded += (s, e) => _refreshTimer?.Start();
            Unloaded += (s, e) => _refreshTimer?.Stop();
        }

        #endregion

        #region Initialization

        private void InitializeTooltip()
        {
            _tooltip = new ToolTip
            {
                Background = new SolidColorBrush(Color.FromRgb(37, 37, 38)),
                Foreground = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(63, 63, 70)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 11,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Relative
            };
        }

        private void LoadIcons()
        {
            try
            {
                _heliIcon = LoadIcon("pack://application:,,,/Resources/Icons/icon-patrol-helicopter.png");
                _chinookBodyIcon = LoadIcon("pack://application:,,,/Resources/Icons/chinook_map_body.png");
                _chinookBladesIcon = LoadIcon("pack://application:,,,/Resources/Icons/chinook_map_blades.png");
                _cargoIcon = LoadIcon("pack://application:,,,/Resources/Icons/cargo_ship_body.png");
            }
            catch { }
        }

        private BitmapImage? LoadIcon(string uri)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(uri);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch { return null; }
        }

        #endregion

        #region Dependency Property Callbacks

        private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((InteractiveMapCanvas)d).InvalidateVisual();

        private static void OnSelectedPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((InteractiveMapCanvas)d).InvalidateVisual();

        private static void OnEntitiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (InteractiveMapCanvas)d;
            if (e.OldValue is ObservableCollection<MapEntity> oldCol)
                oldCol.CollectionChanged -= canvas.OnEntitiesCollectionChanged;
            if (e.NewValue is ObservableCollection<MapEntity> newCol)
                newCol.CollectionChanged += canvas.OnEntitiesCollectionChanged;
            canvas.InvalidateVisual();
        }

        private void OnEntitiesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => Dispatcher.Invoke(() => InvalidateVisual());

        #endregion

        #region Rendering

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (MapImage == null) return;

            var canvasWidth = ActualWidth;
            var canvasHeight = ActualHeight;
            if (canvasWidth <= 0 || canvasHeight <= 0) return;

            // Calculate image aspect ratio
            var imageAspect = MapImage.PixelWidth / (double)MapImage.PixelHeight;
            var canvasAspect = canvasWidth / canvasHeight;

            // Fit image to canvas
            if (imageAspect > canvasAspect)
            {
                _renderWidth = canvasWidth;
                _renderHeight = canvasWidth / imageAspect;
            }
            else
            {
                _renderHeight = canvasHeight;
                _renderWidth = canvasHeight * imageAspect;
            }

            _offsetX = (canvasWidth - _renderWidth) / 2;
            _offsetY = (canvasHeight - _renderHeight) / 2;

            var centerX = _renderWidth / 2;
            var centerY = _renderHeight / 2;

            // Apply transforms
            dc.PushTransform(new TranslateTransform(_panOffset.X + _offsetX, _panOffset.Y + _offsetY));
            dc.PushTransform(new ScaleTransform(ZoomLevel, ZoomLevel, centerX, centerY));

            // Draw map image
            dc.DrawImage(MapImage, new Rect(0, 0, _renderWidth, _renderHeight));

            // Draw grid
            if (ShowGrid)
                DrawGrid(dc);

            // Draw monuments
            if (ShowMonuments && Monuments != null)
                DrawMonuments(dc);

            // Draw entities
            if (Entities != null)
            {
                foreach (var entity in Entities)
                    DrawEntity(dc, entity);
            }

            dc.Pop();
            dc.Pop();
        }

        private void DrawGrid(DrawingContext dc)
        {
            var gridPen = new Pen(new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)), 1 / ZoomLevel);
            gridPen.Freeze();

            int gridSize = 26;
            double cellWidth = _renderWidth / gridSize;
            double cellHeight = _renderHeight / gridSize;

            // Vertical lines
            for (int i = 0; i <= gridSize; i++)
            {
                double x = i * cellWidth;
                dc.DrawLine(gridPen, new Point(x, 0), new Point(x, _renderHeight));
            }

            // Horizontal lines
            for (int i = 0; i <= gridSize; i++)
            {
                double y = i * cellHeight;
                dc.DrawLine(gridPen, new Point(0, y), new Point(_renderWidth, y));
            }
        }

        private void DrawMonuments(DrawingContext dc)
        {
            if (Monuments == null) return;

            var monumentBrush = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
            monumentBrush.Freeze();
            var monumentPen = new Pen(new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)), 1 / ZoomLevel);
            monumentPen.Freeze();

            var typeface = new Typeface("Segoe UI");
            var fontSize = Math.Max(8, 10 / ZoomLevel);

            foreach (var monument in Monuments)
            {
                var x = monument.X * _renderWidth;
                var y = (1 - monument.Y) * _renderHeight; // Y is inverted

                // Draw monument marker (small circle)
                dc.DrawEllipse(monumentBrush, monumentPen, new Point(x, y), 4 / ZoomLevel, 4 / ZoomLevel);

                // Draw label
                if (!string.IsNullOrEmpty(monument.Label))
                {
                    var formattedText = new FormattedText(monument.Label,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight, typeface, fontSize, monumentBrush, 1.0);
                    dc.DrawText(formattedText, new Point(x - formattedText.Width / 2, y + 6 / ZoomLevel));
                }
            }
        }

        private void DrawEntity(DrawingContext dc, MapEntity entity)
        {
            var x = entity.X * _renderWidth;
            var y = (1 - entity.Y) * _renderHeight; // Y is inverted

            switch (entity.Type)
            {
                case MapEntityType.PatrolHelicopter:
                    DrawHelicopter(dc, x, y);
                    break;
                case MapEntityType.Chinook:
                    DrawChinook(dc, x, y);
                    break;
                case MapEntityType.CargoShip:
                    DrawCargoShip(dc, x, y);
                    break;
                case MapEntityType.ActivePlayer:
                case MapEntityType.SleepingPlayer:
                    DrawPlayerMarker(dc, x, y, entity);
                    break;
                default:
                    DrawDefaultMarker(dc, x, y, entity);
                    break;
            }
        }

        private void DrawHelicopter(DrawingContext dc, double x, double y)
        {
            var size = 32 / ZoomLevel;
            if (_heliIcon != null)
            {
                dc.PushTransform(new RotateTransform(_bladeRotation, x, y));
                dc.DrawImage(_heliIcon, new Rect(x - size / 2, y - size / 2, size, size));
                dc.Pop();
            }
            else
            {
                var brush = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                brush.Freeze();
                dc.DrawEllipse(brush, null, new Point(x, y), size / 2, size / 2);
            }
        }

        private void DrawChinook(DrawingContext dc, double x, double y)
        {
            var size = 40 / ZoomLevel;
            if (_chinookBodyIcon != null)
            {
                dc.DrawImage(_chinookBodyIcon, new Rect(x - size / 2, y - size / 2, size, size));
            }
            if (_chinookBladesIcon != null)
            {
                dc.PushTransform(new RotateTransform(_bladeRotation, x, y));
                dc.DrawImage(_chinookBladesIcon, new Rect(x - size / 2, y - size / 2, size, size));
                dc.Pop();
            }
        }

        private void DrawCargoShip(DrawingContext dc, double x, double y)
        {
            var size = 50 / ZoomLevel;
            if (_cargoIcon != null)
            {
                dc.DrawImage(_cargoIcon, new Rect(x - size / 2, y - size / 2, size, size));
            }
            else
            {
                var brush = new SolidColorBrush(Color.FromRgb(0, 191, 255));
                brush.Freeze();
                dc.DrawEllipse(brush, null, new Point(x, y), size / 2, size / 2);
            }
        }

        private void DrawPlayerMarker(DrawingContext dc, double x, double y, MapEntity entity)
        {
            var radius = 6 / ZoomLevel;
            
            // Determine if selected or teammate
            bool isSelected = SelectedPlayer != null && entity.SteamId == SelectedPlayer.SteamId;
            bool isTeammate = false;

            if (SelectedPlayer != null && SelectedPlayer.HasTeam && !isSelected)
            {
                // Check if this entity is a teammate of selected player
                isTeammate = SelectedPlayer.TeamMembers.Contains(entity.SteamId);
            }

            // Choose color
            Color color;
            if (isSelected)
            {
                color = Color.FromRgb(59, 130, 246); // Blue for selected
                radius = 8 / ZoomLevel; // Bigger
            }
            else if (entity.IsDead)
            {
                color = Color.FromRgb(239, 68, 68); // Red for dead
            }
            else if (entity.Type == MapEntityType.ActivePlayer || entity.IsOnline)
            {
                color = Color.FromRgb(16, 185, 129); // Green for online
            }
            else
            {
                color = Color.FromRgb(251, 146, 60); // Orange for sleeping
            }

            // TEAMMATE HIGHLIGHT: Purple outline with glow
            if (isTeammate && !isSelected)
            {
                // Glow effect (outer semi-transparent circle)
                var glowBrush = new SolidColorBrush(Color.FromArgb(80, 139, 92, 246));
                glowBrush.Freeze();
                var glowPen = new Pen(glowBrush, 4 / ZoomLevel);
                glowPen.Freeze();
                dc.DrawEllipse(null, glowPen, new Point(x, y), radius + 3 / ZoomLevel, radius + 3 / ZoomLevel);

                // Purple outline
                var outlineBrush = new SolidColorBrush(Color.FromRgb(139, 92, 246)); // Purple
                outlineBrush.Freeze();
                var outlinePen = new Pen(outlineBrush, 2 / ZoomLevel);
                outlinePen.Freeze();

                var fillBrush = new SolidColorBrush(color);
                fillBrush.Freeze();
                dc.DrawEllipse(fillBrush, outlinePen, new Point(x, y), radius, radius);
            }
            else
            {
                // Normal player (with black outline)
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                var outlinePen = new Pen(Brushes.Black, 1.5 / ZoomLevel);
                outlinePen.Freeze();

                dc.DrawEllipse(brush, outlinePen, new Point(x, y), radius, radius);
            }

            // Direction indicator for active players
            if ((entity.Type == MapEntityType.ActivePlayer || entity.IsOnline) && entity.Rotation != 0)
            {
                var dirLength = radius * 2;
                var angle = entity.Rotation * Math.PI / 180;
                var dirX = x + Math.Sin(angle) * dirLength;
                var dirY = y - Math.Cos(angle) * dirLength;

                var dirBrush = new SolidColorBrush(color);
                dirBrush.Freeze();
                var dirPen = new Pen(dirBrush, 2 / ZoomLevel);
                dirPen.Freeze();
                dc.DrawLine(dirPen, new Point(x, y), new Point(dirX, dirY));
            }
        }

        private void DrawDefaultMarker(DrawingContext dc, double x, double y, MapEntity entity)
        {
            var radius = 4 / ZoomLevel;
            var brush = GetEntityBrush(entity.Type);
            brush.Freeze();
            var outlinePen = new Pen(Brushes.Black, 0.5 / ZoomLevel);
            outlinePen.Freeze();
            dc.DrawEllipse(brush, outlinePen, new Point(x, y), radius, radius);
        }

        private SolidColorBrush GetEntityBrush(MapEntityType type) => type switch
        {
            MapEntityType.ActivePlayer => new SolidColorBrush(Color.FromRgb(16, 185, 129)),
            MapEntityType.SleepingPlayer => new SolidColorBrush(Color.FromRgb(251, 146, 60)),
            MapEntityType.PatrolHelicopter => new SolidColorBrush(Color.FromRgb(255, 165, 0)),
            MapEntityType.Chinook => new SolidColorBrush(Color.FromRgb(255, 140, 0)),
            MapEntityType.CargoShip => new SolidColorBrush(Color.FromRgb(0, 191, 255)),
            MapEntityType.Bradley => new SolidColorBrush(Color.FromRgb(255, 0, 255)),
            MapEntityType.Airdrop => new SolidColorBrush(Color.FromRgb(255, 255, 0)),
            MapEntityType.LockedCrate => new SolidColorBrush(Color.FromRgb(139, 69, 19)),
            MapEntityType.Minicopter => new SolidColorBrush(Color.FromRgb(144, 238, 144)),
            MapEntityType.ScrapHelicopter => new SolidColorBrush(Color.FromRgb(173, 216, 230)),
            MapEntityType.RHIB => new SolidColorBrush(Color.FromRgb(70, 130, 180)),
            MapEntityType.ModularCar => new SolidColorBrush(Color.FromRgb(128, 128, 128)),
            _ => new SolidColorBrush(Colors.White)
        };

        #endregion

        #region Mouse Events

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            var entity = GetEntityAtPosition(pos);

            if (entity != null)
            {
                EntityClicked?.Invoke(this, entity);
                e.Handled = true;
                return;
            }

            _isPanning = true;
            _panOrigin = pos;
            CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this);

            if (_isPanning)
            {
                var delta = pos - _panOrigin;
                _panOffset += delta;
                _panOrigin = pos;
                InvalidateVisual();
                if (_tooltip != null) _tooltip.IsOpen = false;
            }
            else
            {
                var entity = GetEntityAtPosition(pos);
                if (entity != null)
                {
                    if (_hoveredEntity != entity)
                    {
                        _hoveredEntity = entity;
                        if (_tooltip != null)
                        {
                            _tooltip.Content = BuildTooltipContent(entity);
                            _tooltip.PlacementTarget = this;
                        }
                    }
                    if (_tooltip != null)
                    {
                        _tooltip.HorizontalOffset = pos.X + 15;
                        _tooltip.VerticalOffset = pos.Y + 15;
                        _tooltip.IsOpen = true;
                    }
                    Cursor = Cursors.Hand;
                    EntityHovered?.Invoke(this, entity);
                }
                else
                {
                    if (_tooltip != null) _tooltip.IsOpen = false;
                    _hoveredEntity = null;
                    Cursor = Cursors.Arrow;
                }
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = e.Delta > 0 ? 1.1 : 0.9;
            ZoomLevel = Math.Clamp(ZoomLevel * delta, 0.5, 4.0);
            e.Handled = true;
        }

        private MapEntity? GetEntityAtPosition(Point pos)
        {
            if (Entities == null) return null;

            // Transform position to map coordinates
            var transformedX = (pos.X - _panOffset.X - _offsetX) / ZoomLevel;
            var transformedY = (pos.Y - _panOffset.Y - _offsetY) / ZoomLevel;

            var centerX = _renderWidth / 2;
            var centerY = _renderHeight / 2;

            transformedX = (transformedX - centerX) / ZoomLevel + centerX;
            transformedY = (transformedY - centerY) / ZoomLevel + centerY;

            // Find entity under cursor
            foreach (var entity in Entities.Reverse())
            {
                var x = entity.X * _renderWidth;
                var y = (1 - entity.Y) * _renderHeight;

                var dx = transformedX - x;
                var dy = transformedY - y;
                var distance = Math.Sqrt(dx * dx + dy * dy);

                var hitRadius = entity.Type switch
                {
                    MapEntityType.ActivePlayer or MapEntityType.SleepingPlayer => 10 / ZoomLevel,
                    MapEntityType.PatrolHelicopter or MapEntityType.Chinook => 20 / ZoomLevel,
                    MapEntityType.CargoShip => 25 / ZoomLevel,
                    _ => 8 / ZoomLevel
                };

                if (distance < hitRadius)
                    return entity;
            }

            return null;
        }

        private FrameworkElement BuildTooltipContent(MapEntity entity)
        {
            var panel = new StackPanel();

            // Entity name
            panel.Children.Add(new TextBlock
            {
                Text = entity.Label ?? entity.Type.ToString(),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            });

            // Status text
            var statusText = entity.Type switch
            {
                MapEntityType.ActivePlayer => entity.IsDead ? "💀 Dead" : "🟢 Online",
                MapEntityType.SleepingPlayer => "🟠 Sleeping",
                MapEntityType.PatrolHelicopter => "🚁 Patrol Helicopter",
                MapEntityType.Chinook => "🚁 CH47 Chinook",
                MapEntityType.CargoShip => "🚢 Cargo Ship",
                MapEntityType.Bradley => "🛡️ Bradley APC",
                MapEntityType.Airdrop => "📦 Supply Drop",
                MapEntityType.LockedCrate => "📦 Locked Crate",
                _ => entity.Type.ToString()
            };

            panel.Children.Add(new TextBlock
            {
                Text = statusText,
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170))
            });

            return panel;
        }

        #endregion
    }
}
