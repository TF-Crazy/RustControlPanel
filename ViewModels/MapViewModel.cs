// ════════════════════════════════════════════════════════════════════
// MapViewModel.cs - Map page view model with filtering and sorting
// ════════════════════════════════════════════════════════════════════

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using RustControlPanel.Core.Utils;
using RustControlPanel.Models;

namespace RustControlPanel.ViewModels
{
    /// <summary>
    /// ViewModel for the Map page with player list, filtering, and sorting.
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        #region Fields

        private MapInfo _mapInfo = new();
        private ObservableCollection<MapEntity> _entities = new();
        private ObservableCollection<PlayerDetails> _players = new();
        private PlayerDetails? _selectedPlayer;
        private string _searchText = string.Empty;
        private int _sortMode = 0;
        private double _zoomLevel = 1.0;
        private bool _showGrid = true;
        private bool _showMonuments = true;
        private bool _showMonumentLabels = true;

        #endregion

        #region Properties

        /// <summary>
        /// Map information (image, monuments, world size).
        /// </summary>
        public MapInfo MapInfo
        {
            get => _mapInfo;
            set => SetProperty(ref _mapInfo, value);
        }

        /// <summary>
        /// All map entities (players, heli, cargo, etc.).
        /// </summary>
        public ObservableCollection<MapEntity> Entities
        {
            get => _entities;
            set => SetProperty(ref _entities, value);
        }

        /// <summary>
        /// All players.
        /// </summary>
        public ObservableCollection<PlayerDetails> Players
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged(nameof(Players));
                SetupFilters();
                UpdatePlayerCounts();
            }
        }

        /// <summary>
        /// Filtered and sorted players view.
        /// </summary>
        public ICollectionView? FilteredPlayers { get; private set; }

        /// <summary>
        /// Selected player.
        /// </summary>
        public PlayerDetails? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                SetProperty(ref _selectedPlayer, value);
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }

        /// <summary>
        /// Search text for filtering players.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilteredPlayers?.Refresh();
            }
        }

        /// <summary>
        /// Sort mode (0=Name, 1=Health, 2=Online, 3=Ping, 4=Team).
        /// </summary>
        public int SortMode
        {
            get => _sortMode;
            set
            {
                SetProperty(ref _sortMode, value);
                ApplySort();
            }
        }

        /// <summary>
        /// Map zoom level.
        /// </summary>
        public double ZoomLevel
        {
            get => _zoomLevel;
            set => SetProperty(ref _zoomLevel, Math.Clamp(value, 0.5, 4.0));
        }

        /// <summary>
        /// Show grid on map.
        /// </summary>
        public bool ShowGrid
        {
            get => _showGrid;
            set => SetProperty(ref _showGrid, value);
        }

        /// <summary>
        /// Show monuments on map.
        /// </summary>
        public bool ShowMonuments
        {
            get => _showMonuments;
            set => SetProperty(ref _showMonuments, value);
        }

        /// <summary>
        /// Show monument labels on map.
        /// </summary>
        public bool ShowMonumentLabels
        {
            get => _showMonumentLabels;
            set => SetProperty(ref _showMonumentLabels, value);
        }

        /// <summary>
        /// Number of active (online) players.
        /// </summary>
        public int ActivePlayersCount => Players.Count(p => p.IsOnline && !p.IsDead);

        /// <summary>
        /// Number of sleeping players.
        /// </summary>
        public int SleepingPlayersCount => Players.Count(p => !p.IsOnline || p.IsDead);

        #endregion

        #region Commands

        /// <summary>
        /// Command to zoom in.
        /// </summary>
        public ICommand ZoomInCommand { get; }

        /// <summary>
        /// Command to zoom out.
        /// </summary>
        public ICommand ZoomOutCommand { get; }

        /// <summary>
        /// Command to reset view.
        /// </summary>
        public ICommand ResetViewCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new MapViewModel.
        /// </summary>
        public MapViewModel()
        {
            // Initialize commands
            ZoomInCommand = new RelayCommand(ExecuteZoomIn);
            ZoomOutCommand = new RelayCommand(ExecuteZoomOut);
            ResetViewCommand = new RelayCommand(ExecuteResetView);

            // Setup filters
            SetupFilters();

            Logger.Instance.Debug("MapViewModel created");
        }

        #endregion

        #region Filtering & Sorting

        /// <summary>
        /// Sets up the filtered players collection view.
        /// </summary>
        private void SetupFilters()
        {
            FilteredPlayers = CollectionViewSource.GetDefaultView(Players);
            FilteredPlayers.Filter = FilterPlayers;
            ApplySort();
            OnPropertyChanged(nameof(FilteredPlayers));
        }

        /// <summary>
        /// Filter predicate for players.
        /// </summary>
        private bool FilterPlayers(object obj)
        {
            if (obj is not PlayerDetails player) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            return player.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Applies the selected sort mode to the players list.
        /// </summary>
        private void ApplySort()
        {
            if (FilteredPlayers == null) return;

            FilteredPlayers.SortDescriptions.Clear();

            switch (SortMode)
            {
                case 0: // Par nom
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.DisplayName), ListSortDirection.Ascending));
                    break;

                case 1: // Par santé (faible → élevé)
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.Health), ListSortDirection.Ascending));
                    break;

                case 2: // En ligne d'abord
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.IsOnline), ListSortDirection.Descending));
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.DisplayName), ListSortDirection.Ascending));
                    break;

                case 3: // Par ping (faible → élevé)
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.Ping), ListSortDirection.Ascending));
                    break;

                case 4: // Par team (grouper teammates)
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.TeamId), ListSortDirection.Ascending));
                    FilteredPlayers.SortDescriptions.Add(
                        new SortDescription(nameof(PlayerDetails.DisplayName), ListSortDirection.Ascending));
                    break;
            }
        }

        /// <summary>
        /// Updates player count properties.
        /// </summary>
        public void UpdatePlayerCounts()
        {
            OnPropertyChanged(nameof(ActivePlayersCount));
            OnPropertyChanged(nameof(SleepingPlayersCount));
        }

        #endregion

        #region Command Implementations

        private void ExecuteZoomIn(object? parameter)
        {
            ZoomLevel *= 1.2;
        }

        private void ExecuteZoomOut(object? parameter)
        {
            ZoomLevel /= 1.2;
        }

        private void ExecuteResetView(object? parameter)
        {
            ZoomLevel = 1.0;
        }

        #endregion
    }
}
