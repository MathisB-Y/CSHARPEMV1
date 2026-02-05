 using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using Avalonia.Platform;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Shapes;

namespace CSHARPEMV1
{
    public partial class MainWindow : Window
    {
        //VARIABLES
        private TextBox _pseudoTextBox = null!;
        private string _pseudo = "";
        private string _selectedRace = "";
        private int _diceRollValue = 0;
        private string _selectedClass = "";
        private Grid _mainContainer = null!;
        private Grid _overlayContainer = null!;
        
        private StackPanel _raceStatsContainer = null!;
        private TextBlock _raceCompetenceText = null!;
        private Button _nextToClassButton = null!;
        private StackPanel _classStatsContainer = null!;
        private Button _nextToItemsButton = null!;
        private Border? _previouslySelectedCard;


        //DONNÉES
        private readonly Dictionary<string, (string Stats, string Competence)> _raceData = new()
        {
            { "Harpie", ("Force: 50\nDéfense: 50\nVitesse: 85\nPV: 550\nMagie: 10\nRégénération PV: 3% /sec (0% combat)", "Ne peut pas être ciblée pendant 5 secondes.") },
            { "Ogre", ("Force: 30\nDéfense: 70\nVitesse: 25\nPV: 955\nMagie: 10\nRégénération PV: 4% /sec (0% combat)", "Réduit sa vie de 150 PV mais augmente son armure de 30 (10s).") },
            { "Gobelin", ("Force: 60\nDéfense: 35\nVitesse: 105\nPV: 375\nMagie: 7\nRégénération PV: 3% /sec (0% combat)", "Se camoufle pendant 3.5 secondes.") },
            { "Elfe", ("Force: 50\nDéfense: 40\nVitesse: 80\nPV: 500\nMagie: 20\nRégénération PV: 2.5% /sec (0% combat)", "Crée une flaque magique (5 dégâts/sec).") },
            { "Vampire", ("Force: 30\nDéfense: 55\nVitesse: 90\nPV: 630\nMagie: 25\nRégénération PV: 5% /sec (0% combat)", "Vole 7% de la vie d'un ennemi.") },
            { "Humain", ("Force: 50\nDéfense: 50\nVitesse: 55\nPV: 500\nMagie: 10\nRégénération PV: 2.5% /sec (0% combat)", "Boost stats +5% pendant 10 secondes.") }
        };

        private readonly Dictionary<string, string> _classStats = new()
        {
            { "Assassin", "+ 15 Force" },
            { "Voleur", "+ 7 Vitesse et + 8 Force" },
            { "Guerrier", "+ 5 Force et + 10 Défense" },
            { "Mage", "+ 16 Magie" },
            { "Archer", "+ 10 Vitesse et + 1% Régénération PV" },
            { "Tank", "+ 15 Défense et + 150 PV" },
            { "Soutien", "+ 1.5% Régénération PV et + 7 Défense" },
            { "Moine", "+ 6 Magie et + 1.6% Régénération PV" },
            { "Nécromancien", "+ 5 Magie et + 175 PV" },
            { "Ninja", "+ 10 Vitesse et + 5 Force" },
            { "Voltigeur", "+ 13 Vitesse et + 125 PV" }
        };

        private readonly Dictionary<string, string> _itemsBaseStats = new()
        {
            { "Sabre Obscur", "+ 55 Force" },
            { "Bottes Célérité", "+ 55 Vitesse" },
            { "Bouclier Céleste", "+ 45 Défense et + 215 PV" },
            { "Casque Régénérateur", "+ 6% Régénération PV" },
            { "Sceptre Antique", "+ 50 Magie et + 5 Vitesse" },
            { "Cœur Métallique", "+ 750 PV" },
            { "Katana Kokushibo", "+ 25 Vitesse et + 30 Force" },
            { "Plastron Magma", "+ 40 Défense et + 15 Force" },
            { "Lunette ZX", "+ 20 Magie et + 2.5% Régénération PV" },
            { "Protège Bras du Navigateur", "+ 35 Défense et + 20 Magie" },
            { "Arc Ténébreux", "+ 26 Force et + 29 Magie" },
            { "Arbalète du Soleil", "+ 30 Magie et + 200 PV" },
            { "Gants Supersonique", "+ 40 Vitesse et + 15 Défense" },
            { "Clair de Lune", "+ 3% Régénération PV et + 150 PV" },
            { "Jambière Epineuse", "+ 30 Défense et + 25 Force" }
        };

        private readonly Dictionary<string, string> _itemImages = new()
        {
            { "Sabre Obscur", "sabreobscur.jpg" },
            { "Bottes Célérité", "bottescelerite.jpg" },
            { "Bouclier Céleste", "bouclierceleste.jpg" },
            { "Casque Régénérateur", "casqueregenerateur.jpg" },
            { "Sceptre Antique", "sceptreantique.jpg" },
            { "Cœur Métallique", "coeurmetallique.jpg" },
            { "Katana Kokushibo", "katanadekokushibo.jpg" },
            { "Plastron Magma", "plastronmagma.jpg" },
            { "Lunette ZX", "lunetteszx.jpg" },
            { "Protège Bras du Navigateur", "protegebras.jpg" },
            { "Arc Ténébreux", "arctenebreux.jpg" },
            { "Arbalète du Soleil", "arbaletedusoleil.jpg" },
            { "Gants Supersonique", "gantsupersonique.jpg" },
            { "Clair de Lune", "clairdelune.jpg" },
            { "Jambière Epineuse", "jambiereepineuse.jpg" }
        };

        private readonly Dictionary<string, double> _rarityMultipliers = new()
        {
            { "Commun", 1.0 },
            { "Rare", 1.15 },
            { "Épique", 1.35 },
            { "Légendaire", 1.65 }
        };

        private readonly Dictionary<string, Color> _rarityColors = new()
        {
            { "Commun", Colors.LightGray },
            { "Rare", Color.Parse("#3498db") },
            { "Épique", Color.Parse("#9b59b6") },
            { "Légendaire", Color.Parse("#f1c40f") }
        };

        private readonly Dictionary<string, List<string>> _blockedClasses = new()
        {
            { "Harpie", new List<string> { "Moine", "Ninja" } },
            { "Ogre", new List<string> { "Mage", "Voltigeur" } },
            { "Gobelin", new List<string> { "Tank", "Mage" } },
            { "Elfe", new List<string> { "Voleur", "Tank" } },
            { "Vampire", new List<string> { "Ninja", "Archer" } },
            { "Humain", new List<string> { "Moine", "Nécromancien" } }
        };

        private readonly Dictionary<string, string> _raceImages = new()
        {
            { "Harpie", "avares://CSHARPEMV1/Assets/harpie.jpg" },
            { "Ogre", "avares://CSHARPEMV1/Assets/ogre.jpg" },
            { "Gobelin", "avares://CSHARPEMV1/Assets/gobelin.jpg" },
            { "Elfe", "avares://CSHARPEMV1/Assets/elfe.jpg" },
            { "Vampire", "avares://CSHARPEMV1/Assets/vampire.jpg" },
            { "Humain", "avares://CSHARPEMV1/Assets/humain.jpg" }
        };

        private readonly Dictionary<string, string> _statIcons = new()
        {
            { "Force", "avares://CSHARPEMV1/Assets/force.png" },
            { "Défense", "avares://CSHARPEMV1/Assets/defense.png" },
            { "Vitesse", "avares://CSHARPEMV1/Assets/vitesse.png" },
            { "PV", "avares://CSHARPEMV1/Assets/pv.png" },
            { "Magie", "avares://CSHARPEMV1/Assets/magie.png" },
            { "Régénération PV", "avares://CSHARPEMV1/Assets/regeneration.png" }
        };

        private readonly string _backgroundImage = "avares://CSHARPEMV1/Assets/background.jpg";
        private readonly string[] _races = { "Harpie", "Ogre", "Gobelin", "Elfe", "Vampire", "Humain" };
        private readonly string[] _classes = { "Assassin", "Voleur", "Guerrier", "Mage", "Archer", "Tank", "Soutien", "Moine", "Nécromancien", "Ninja", "Voltigeur" };


        //INITIALISATION
        public MainWindow()
        {
            InitializeComponent();
            #if DEBUG
            this.AttachDevTools();
            #endif
            ShowWelcomeScreen();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupWindowStructure()
        {
            var rootGrid = new Grid();
            
            Image? backgroundImage = null;
            try
            {
                var uri = new Uri(_backgroundImage);
                var stream = AssetLoader.Open(uri);
                backgroundImage = new Image
                {
                    Source = new Bitmap(stream),
                    Stretch = Stretch.UniformToFill,
                    ZIndex = -10
                };
            }
            catch { rootGrid.Background = Brushes.Black; }

            if (backgroundImage != null) rootGrid.Children.Add(backgroundImage);

            _mainContainer = new Grid();
            rootGrid.Children.Add(_mainContainer);

            _overlayContainer = new Grid
            {
                Background = new SolidColorBrush(Colors.Black) { Opacity = 0.7 },
                IsVisible = false,
                ZIndex = 100
            };
            rootGrid.Children.Add(_overlayContainer);

            this.Content = rootGrid;
        }

       // ÉCRAN PSEUDO
private void ShowWelcomeScreen()
{
    SetupWindowStructure();

    var mainContainer = new Grid
    {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };

    try
    {
        var backgroundCharacters = new Image
        {
            Source = new Bitmap(AssetLoader.Open(new Uri("avares://CSHARPEMV1/Assets/RPG-Perso.png"))),
            Stretch = Stretch.UniformToFill,
            Opacity = 0.4,
            Width = 500,
            Height = 200,
        };
        mainContainer.Children.Add(backgroundCharacters);
    }
    catch
    {
    }

    var stackPanel = new StackPanel
    {
        Spacing = 50,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        MaxWidth = 950
    };

    try
    {
        var titleImage = new Image
        {
            Source = new Bitmap(AssetLoader.Open(new Uri("avares://CSHARPEMV1/Assets/choix-pseudo.png"))),
            Stretch = Stretch.Uniform,
            MaxWidth = 2000,
            MaxHeight = 500,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 30)
        };
        
        var titleBorder = new Border
        {
            Child = titleImage,
            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 30,
                OffsetX = 0,
                OffsetY = 8,
                Opacity = 0.9
            }
        };
        
        stackPanel.Children.Add(titleBorder);
    }
    catch
    {
        var title = new TextBlock
        {
            Text = "Créateur de Personnage RPG",
            FontSize = 68,
            FontWeight = FontWeight.Black,
            FontFamily = new FontFamily("Arial Black, Arial, sans-serif"),
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FFE873"), 0.0),
                    new GradientStop(Color.Parse("#FFD700"), 0.4),
                    new GradientStop(Color.Parse("#FFA500"), 0.7),
                    new GradientStop(Color.Parse("#FF8C00"), 1.0)
                }
            },
            Margin = new Thickness(0, 0, 0, 50),
            TextAlignment = TextAlignment.Center,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        stackPanel.Children.Add(title);
    }

    var textBoxContainer = new Border
    {
        Width = 500,
        Padding = new Thickness(4),
        CornerRadius = new CornerRadius(18),
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.Parse("#8B7355"), 0.0),
                new GradientStop(Color.Parse("#6B5442"), 1.0)
            }
        },
        HorizontalAlignment = HorizontalAlignment.Center,
        BoxShadow = BoxShadows.Parse("0 6 20 0 #000000")
    };

    _pseudoTextBox = new TextBox
    {
        Watermark = "Entrez votre pseudo ...",
        FontSize = 26,
        Padding = new Thickness(25, 18),
        CornerRadius = new CornerRadius(15),
        Background = new SolidColorBrush(Color.Parse("#1a1a1a")) { Opacity = 0.95 },
        Foreground = new SolidColorBrush(Color.Parse("#E0E0E0")),
        BorderThickness = new Thickness(0),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        FontWeight = FontWeight.Medium,
        TextAlignment = TextAlignment.Center
    };
    textBoxContainer.Child = _pseudoTextBox;
    stackPanel.Children.Add(textBoxContainer);

    var buttonBorder = new Border
    {
        Width = 450,
        Padding = new Thickness(4),
        CornerRadius = new CornerRadius(18),
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.Parse("#8B0000"), 0.0),
                new GradientStop(Color.Parse("#B22222"), 0.5),
                new GradientStop(Color.Parse("#8B0000"), 1.0)
            }
        },
        HorizontalAlignment = HorizontalAlignment.Center,
        BoxShadow = BoxShadows.Parse("0 8 25 0 #8B0000"),
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
    };

    var confirmButton = new Button
    {
        FontSize = 24,
        Padding = new Thickness(0, 18),
        CornerRadius = new CornerRadius(15),
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.Parse("#2a2a2a"), 0.0),
                new GradientStop(Color.Parse("#1a1a1a"), 1.0)
            }
        },
        Foreground = Brushes.White,
        BorderThickness = new Thickness(0),
        HorizontalAlignment = HorizontalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        FontWeight = FontWeight.Bold
    };

    var buttonContent = new StackPanel
    {
        Orientation = Orientation.Horizontal,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Spacing = 12
    };


    buttonContent.Children.Add(new TextBlock
    {
        Text = "COMMENCER L'AVENTURE",
        FontSize = 24,
        FontWeight = FontWeight.Bold,
        VerticalAlignment = VerticalAlignment.Center
    });


    confirmButton.Content = buttonContent;

    var scaleTransform = new ScaleTransform(1, 1);
    buttonBorder.RenderTransform = scaleTransform;
    buttonBorder.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

    buttonBorder.PointerEntered += (s, e) =>
    {
        scaleTransform.ScaleX = 1.05;
        scaleTransform.ScaleY = 1.05;
        buttonBorder.BoxShadow = BoxShadows.Parse("0 12 35 0 #B22222");
    };

    buttonBorder.PointerExited += (s, e) =>
    {
        scaleTransform.ScaleX = 1.0;
        scaleTransform.ScaleY = 1.0;
        buttonBorder.BoxShadow = BoxShadows.Parse("0 8 25 0 #8B0000");
    };

    confirmButton.Click += (sender, e) =>
    {
        _pseudo = string.IsNullOrWhiteSpace(_pseudoTextBox.Text) ? "Aventurier" : _pseudoTextBox.Text.Trim();
        ShowRaceSelectionScreen();
    };

    buttonBorder.Child = confirmButton;
    stackPanel.Children.Add(buttonBorder);

    var footerText = new TextBlock
    {
        Text = "Votre destinée vous attend",
        FontSize = 17,
        FontStyle = FontStyle.Italic,
        HorizontalAlignment = HorizontalAlignment.Center,
        Foreground = new SolidColorBrush(Color.Parse("#888888")),
        Margin = new Thickness(0, 20, 0, 0),
        Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 5,
            OffsetX = 1,
            OffsetY = 1,
            Opacity = 0.7
        }
    };
    stackPanel.Children.Add(footerText);

    mainContainer.Children.Add(stackPanel);
    _mainContainer.Children.Add(mainContainer);

    this.Title = "Créateur de Personnage RPG";
    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
}

        //ÉCRAN CHOIX RACE
        private void ShowRaceSelectionScreen()
        {
            _mainContainer.Children.Clear();

            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(400) });

            var leftPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 30, 0, 30)
            };

            var racesGrid = new Grid();
            racesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            racesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            racesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            racesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            racesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;
            int col = 0;
            foreach (var race in _races)
            {
                var card = CreateRaceCard(race);
                Grid.SetRow(card, row);
                Grid.SetColumn(card, col);
                racesGrid.Children.Add(card);

                col++;
                if (col == 3) { col = 0; row++; }
            }

            leftPanel.Children.Add(racesGrid);
            Grid.SetColumn(leftPanel, 0);
            mainGrid.Children.Add(leftPanel);

            var statsPanel = CreateRaceStatsPanel();
            Grid.SetColumn(statsPanel, 1);
            mainGrid.Children.Add(statsPanel);

            _mainContainer.Children.Add(mainGrid);
            this.Title = $"Choix de la Race - {_pseudo}";
        }

        private StackPanel CreateRaceCard(string race)
        {
            var cardPanel = new StackPanel { Spacing = 18, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(20) };

            var border = new Border
            {
                Width = 220, Height = 300, CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(Colors.Black) { Opacity = 0.6 },
                BorderBrush = new SolidColorBrush(Colors.Gray), BorderThickness = new Thickness(3),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
            };

            var imageContainer = new Border { CornerRadius = new CornerRadius(17), ClipToBounds = true };
            var image = new Image { Stretch = Stretch.UniformToFill };
            
            try { image.Source = new Bitmap(AssetLoader.Open(new Uri(_raceImages[race]))); }
            catch { imageContainer.Background = new SolidColorBrush(GetRaceColor(race)); }
            
            if (image.Source != null) imageContainer.Child = image;
            border.Child = imageContainer;
            cardPanel.Children.Add(border);

            var nameText = new TextBlock { Text = race, FontSize = 22, FontWeight = FontWeight.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center };
            cardPanel.Children.Add(nameText);

            var scaleTransform = new ScaleTransform(1, 1);
            border.RenderTransform = scaleTransform;

            border.PointerEntered += (s, e) => { scaleTransform.ScaleX = 1.05; scaleTransform.ScaleY = 1.05; border.BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")); };
            border.PointerExited += (s, e) => {
                if (_previouslySelectedCard != border) { scaleTransform.ScaleX = 1.0; scaleTransform.ScaleY = 1.0; border.BorderBrush = new SolidColorBrush(Colors.Gray); }
            };
            border.PointerPressed += (s, e) => {
                _selectedRace = race;
                UpdateRaceStatsDisplay(race);
                if (_previouslySelectedCard != null && _previouslySelectedCard != border) {
                    ((ScaleTransform)_previouslySelectedCard.RenderTransform!).ScaleX = 1.0;
                    ((ScaleTransform)_previouslySelectedCard.RenderTransform!).ScaleY = 1.0;
                    _previouslySelectedCard.BorderBrush = new SolidColorBrush(Colors.Gray);
                }
                border.BorderBrush = new SolidColorBrush(Color.Parse("#2ecc71"));
                _previouslySelectedCard = border;
            };

            return cardPanel;
        }

        private Border CreateRaceStatsPanel()
        {
            var border = new Border
            {
                Width = 380, Margin = new Thickness(20), Background = new SolidColorBrush(Colors.Black) { Opacity = 0.85 },
                Padding = new Thickness(25), CornerRadius = new CornerRadius(20), VerticalAlignment = VerticalAlignment.Center
            };
            var panel = new StackPanel { Spacing = 20 };
            
            panel.Children.Add(new TextBlock { Text = "Stats de la race", FontSize = 28, FontWeight = FontWeight.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center });
            
            _raceStatsContainer = new StackPanel { Spacing = 12 };
            _raceStatsContainer.Children.Add(new TextBlock { Text = "Aucune sélection", FontSize = 18, Foreground = Brushes.LightGray, HorizontalAlignment = HorizontalAlignment.Center });
            panel.Children.Add(_raceStatsContainer);

            _raceCompetenceText = new TextBlock { Text = "", FontSize = 18, Foreground = new SolidColorBrush(Color.Parse("#e74c3c")), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 15, 0, 0) };
            panel.Children.Add(_raceCompetenceText);

            _nextToClassButton = new Button
            {
                Content = "Suivant → Choix Classe", FontSize = 20, Padding = new Thickness(0, 12), CornerRadius = new CornerRadius(12),
                Background = new SolidColorBrush(Color.Parse("#95a5a6")), Foreground = Brushes.White, IsEnabled = false, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center
            };
            _nextToClassButton.Click += (s, e) => { _previouslySelectedCard = null; ShowClassSelectionScreen(); };
            panel.Children.Add(_nextToClassButton);

            border.Child = panel;
            return border;
        }

        private void UpdateRaceStatsDisplay(string race)
        {
            var data = _raceData[race];
            _raceStatsContainer.Children.Clear();
            foreach (var stat in data.Stats.Split('\n'))
            {
                var parts = stat.Split(':');
                if (parts.Length == 2) _raceStatsContainer.Children.Add(CreateStatRow(parts[0].Trim(), parts[1].Trim()));
            }
            _raceCompetenceText.Text = $"★ {data.Competence}";
            _nextToClassButton.IsEnabled = true;
            _nextToClassButton.Background = new SolidColorBrush(Color.Parse("#2ecc71"));
        }

        //ÉCRAN CHOIX CLASSE
        private void ShowClassSelectionScreen()
        {
            _mainContainer.Children.Clear();
            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(400) });

            var scrollViewer = new ScrollViewer { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(20, 30, 20, 30) };
            var classesPanel = new WrapPanel { MaxWidth = 850, HorizontalAlignment = HorizontalAlignment.Center, Orientation = Orientation.Horizontal };

            var blockedClasses = _blockedClasses.ContainsKey(_selectedRace) ? _blockedClasses[_selectedRace] : new List<string>();

            foreach (var className in _classes)
            {
                var card = CreateClassCard(className, blockedClasses.Contains(className));
                classesPanel.Children.Add(card);
            }
            scrollViewer.Content = classesPanel;
            Grid.SetColumn(scrollViewer, 0);
            mainGrid.Children.Add(scrollViewer);

            var statsPanel = CreateClassStatsPanel();
            Grid.SetColumn(statsPanel, 1);
            mainGrid.Children.Add(statsPanel);

            _mainContainer.Children.Add(mainGrid);
            this.Title = $"Choix de la Classe - {_pseudo} ({_selectedRace})";
        }

        private StackPanel CreateClassCard(string className, bool isBlocked)
        {
            var cardPanel = new StackPanel { Spacing = 10, Margin = new Thickness(20), Opacity = isBlocked ? 0.4 : 1.0 };
            var border = new Border
            {
                Width = 200, Height = 120, CornerRadius = new CornerRadius(15),
                Background = new SolidColorBrush(isBlocked ? Colors.DarkRed : Colors.Black) { Opacity = 0.7 },
                BorderBrush = new SolidColorBrush(isBlocked ? Colors.Red : Colors.Gray), BorderThickness = new Thickness(3),
                Cursor = new Avalonia.Input.Cursor(isBlocked ? Avalonia.Input.StandardCursorType.No : Avalonia.Input.StandardCursorType.Hand),
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
            };

            var content = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 5 };
            content.Children.Add(new TextBlock { Text = className, FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center });
            if (isBlocked) content.Children.Add(new TextBlock { Text = "BLOQUÉ", FontSize = 14, FontWeight = FontWeight.Bold, Foreground = Brushes.Red, HorizontalAlignment = HorizontalAlignment.Center });

            border.Child = content;
            cardPanel.Children.Add(border);

            if (!isBlocked)
            {
                var scale = new ScaleTransform(1, 1);
                border.RenderTransform = scale;
                border.PointerEntered += (s, e) => { scale.ScaleX = 1.05; scale.ScaleY = 1.05; border.BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")); };
                border.PointerExited += (s, e) => { if (_previouslySelectedCard != border) { scale.ScaleX = 1; scale.ScaleY = 1; border.BorderBrush = new SolidColorBrush(Colors.Gray); } };
                border.PointerPressed += (s, e) => {
                    _selectedClass = className;
                    UpdateClassStatsDisplay(className);
                    if (_previouslySelectedCard != null && _previouslySelectedCard != border) { ((ScaleTransform)_previouslySelectedCard.RenderTransform!).ScaleX = 1; ((ScaleTransform)_previouslySelectedCard.RenderTransform!).ScaleY = 1; _previouslySelectedCard.BorderBrush = new SolidColorBrush(Colors.Gray); }
                    border.BorderBrush = new SolidColorBrush(Color.Parse("#2ecc71"));
                    _previouslySelectedCard = border;
                };
            }
            return cardPanel;
        }

        private Border CreateClassStatsPanel()
        {
            var border = new Border { Width = 380, Margin = new Thickness(20), Background = new SolidColorBrush(Colors.Black) { Opacity = 0.85 }, Padding = new Thickness(25), CornerRadius = new CornerRadius(20), VerticalAlignment = VerticalAlignment.Center };
            var panel = new StackPanel { Spacing = 20 };

            var backButton = new Button { Content = "← Race", FontSize = 16, Padding = new Thickness(10, 5), CornerRadius = new CornerRadius(8), Background = new SolidColorBrush(Colors.Gray), Foreground = Brushes.White };
            backButton.Click += (s, e) => ShowRaceSelectionScreen();
            panel.Children.Add(backButton);

            panel.Children.Add(new TextBlock { Text = "Stats de Classe", FontSize = 28, FontWeight = FontWeight.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center });
            panel.Children.Add(new TextBlock { Text = $"Race: {_selectedRace}", FontSize = 16, Foreground = Brushes.Cyan, HorizontalAlignment = HorizontalAlignment.Center });

            _classStatsContainer = new StackPanel { Spacing = 12, MinHeight = 100 };
            _classStatsContainer.Children.Add(new TextBlock { Text = "Sélectionnez une classe", FontSize = 18, Foreground = Brushes.LightGray, HorizontalAlignment = HorizontalAlignment.Center });
            panel.Children.Add(_classStatsContainer);

            _nextToItemsButton = new Button
            {
                Content = "Suivant → Items", FontSize = 20, Padding = new Thickness(0, 12), CornerRadius = new CornerRadius(12),
                Background = new SolidColorBrush(Color.Parse("#95a5a6")), Foreground = Brushes.White, IsEnabled = false, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Center
            };
            
            _nextToItemsButton.Click += OnNextToItemsClick;
            
            panel.Children.Add(_nextToItemsButton);
            border.Child = panel;
            return border;
        }

        private void UpdateClassStatsDisplay(string className)
        {
            _classStatsContainer.Children.Clear();
            foreach (var part in _classStats[className].Split(new[] { " et " }, StringSplitOptions.RemoveEmptyEntries))
            {
                _classStatsContainer.Children.Add(CreateClassStatRow(part.Trim()));
            }
            _nextToItemsButton.IsEnabled = true;
            _nextToItemsButton.Background = new SolidColorBrush(Color.Parse("#2ecc71"));
        }

        //POP-UP CONFIRMATION
        private void OnNextToItemsClick(object? sender, RoutedEventArgs e)
        {
            _overlayContainer.Children.Clear();
            _overlayContainer.IsVisible = true;

            var popupBorder = new Border
            {
                Width = 400,
                Padding = new Thickness(30),
                CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(Color.Parse("#2c3e50")),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BoxShadow = new BoxShadows(new BoxShadow { Blur = 20, Color = Colors.Black, OffsetX = 0, OffsetY = 10 })
            };

            var stack = new StackPanel { Spacing = 20 };

            stack.Children.Add(new TextBlock
            {
                Text = "ATTENTION",
                Foreground = Brushes.Red,
                FontSize = 28,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            stack.Children.Add(new TextBlock
            {
                Text = "Vous ne pourrez plus retourner en arrière pour changer de Race ou de Classe après cette étape.",
                Foreground = Brushes.White,
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            });

            var btnContainer = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 20, Margin = new Thickness(0, 10, 0, 0) };

            var btnCancel = new Button { Content = "Annuler", Background = Brushes.Gray, Foreground = Brushes.White, CornerRadius = new CornerRadius(8) };
            btnCancel.Click += (s, args) => { _overlayContainer.IsVisible = false; };

            var btnConfirm = new Button { Content = "Confirmer", Background = new SolidColorBrush(Color.Parse("#c0392b")), Foreground = Brushes.White, FontWeight = FontWeight.Bold, CornerRadius = new CornerRadius(8) };
            btnConfirm.Click += (s, args) =>
            {
                _overlayContainer.IsVisible = false;
                ShowItemsScreen();
            };

            btnContainer.Children.Add(btnCancel);
            btnContainer.Children.Add(btnConfirm);
            stack.Children.Add(btnContainer);

            popupBorder.Child = stack;
            _overlayContainer.Children.Add(popupBorder);
        }

        //ÉCRAN ITEMS (ROULETTE ANIMÉE)
        
        private List<(string itemName, string rarity, string stats)> _selectedItems = new();
        
        private void ShowItemsScreen()
        {
            _mainContainer.Children.Clear();

            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var header = new StackPanel { Margin = new Thickness(0, 40, 0, 20), HorizontalAlignment = HorizontalAlignment.Center };
            header.Children.Add(new TextBlock 
            { 
                Text = "🎰 Tirage des Équipements 🎰", 
                FontSize = 36, 
                FontWeight = FontWeight.Bold, 
                Foreground = Brushes.White, 
                HorizontalAlignment = HorizontalAlignment.Center 
            });
            header.Children.Add(new TextBlock 
            { 
                Text = "Le destin tourne la roue...", 
                FontSize = 18, 
                Foreground = Brushes.LightGray, 
                HorizontalAlignment = HorizontalAlignment.Center, 
                FontStyle = FontStyle.Italic 
            });
            mainGrid.Children.Add(header);

            var roulettesContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 50
            };

            Grid.SetRow(roulettesContainer, 1);
            mainGrid.Children.Add(roulettesContainer);
            _mainContainer.Children.Add(mainGrid);

            this.Title = $"Équipement - {_pseudo}";

            Task.Run(async () =>
            {
                var random = new Random();
                
                var selectedItemNames = _itemsBaseStats.Keys.OrderBy(x => random.Next()).Take(2).ToList();
                
                string rarity1 = GetRandomRarityByDropRate(random);
                string rarity2 = GetRandomRarityByDropRate(random);

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    roulettesContainer.Children.Add(CreateRouletteWheel(selectedItemNames[0], rarity1, 1));
                });

                await Task.Delay(800);
                
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    roulettesContainer.Children.Add(CreateRouletteWheel(selectedItemNames[1], rarity2, 2));
                });
                
                await Task.Delay(8000);
                
                double multiplier1 = _rarityMultipliers[rarity1];
                double multiplier2 = _rarityMultipliers[rarity2];
                string stats1 = CalculateItemStats(_itemsBaseStats[selectedItemNames[0]], multiplier1);
                string stats2 = CalculateItemStats(_itemsBaseStats[selectedItemNames[1]], multiplier2);
                
                _selectedItems = new List<(string, string, string)>
                {
                    (selectedItemNames[0], rarity1, stats1),
                    (selectedItemNames[1], rarity2, stats2)
                };
                
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ShowDiceRollScreen();
                });
            });
        }

        private Border CreateRouletteWheel(string finalItem, string finalRarity, int wheelNumber)
        {
            var random = new Random();
            var allItems = _itemsBaseStats.Keys.ToList();
            
            var mainBorder = new Border
            {
                Width = 400,
                Height = 550,
                CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(Colors.Black) { Opacity = 0.9 },
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(3),
                Padding = new Thickness(20)
            };

            var stack = new StackPanel { Spacing = 20 };

            stack.Children.Add(new TextBlock
            {
                Text = $"🎲 ÉQUIPEMENT {wheelNumber} 🎲",
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var rouletteContainer = new Grid
            {
                Height = 300,
                ClipToBounds = true
            };

            var indicator = new Border
            {
                Width = 380,
                Height = 80,
                BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")),
                BorderThickness = new Thickness(4),
                CornerRadius = new CornerRadius(10),
                Background = new SolidColorBrush(Colors.Black) { Opacity = 0.3 },
                VerticalAlignment = VerticalAlignment.Center,
                ZIndex = 10
            };
            rouletteContainer.Children.Add(indicator);

            var scrollPanel = new StackPanel
            {
                Spacing = 10,
                VerticalAlignment = VerticalAlignment.Top
            };

            var itemSequence = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                itemSequence.Add(allItems[random.Next(allItems.Count)]);
            }
            itemSequence.Insert(7, finalItem);

            foreach (var itemName in itemSequence)
            {
                var itemCard = new Border
                {
                    Width = 360,
                    Height = 70,
                    CornerRadius = new CornerRadius(10),
                    Background = new SolidColorBrush(Colors.DarkSlateGray) { Opacity = 0.6 },
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(2)
                };

                var itemContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Spacing = 15,
                    Margin = new Thickness(15, 0, 0, 0)
                };

                var miniImage = new Border
                {
                    Width = 50,
                    Height = 50,
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true
                };
                
                var img = new Image { Stretch = Stretch.UniformToFill };
                try
                {
                    string fileName = _itemImages.ContainsKey(itemName) ? _itemImages[itemName] : "default.jpg";
                    var uri = new Uri($"avares://CSHARPEMV1/Assets/{fileName}");
                    img.Source = new Bitmap(AssetLoader.Open(uri));
                    miniImage.Child = img;
                }
                catch
                {
                    miniImage.Background = new SolidColorBrush(Colors.DarkSlateGray);
                }

                itemContent.Children.Add(miniImage);
                itemContent.Children.Add(new TextBlock
                {
                    Text = itemName,
                    FontSize = 18,
                    FontWeight = FontWeight.Bold,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center
                });

                itemCard.Child = itemContent;
                scrollPanel.Children.Add(itemCard);
            }

            rouletteContainer.Children.Add(scrollPanel);
            stack.Children.Add(rouletteContainer);

            var resultPanel = new StackPanel
            {
                Spacing = 10,
                Opacity = 0
            };

            var rarityText = new TextBlock
            {
                Text = "CALCUL...",
                FontSize = 22,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            resultPanel.Children.Add(rarityText);

            var statsText = new TextBlock
            {
                Text = "",
                FontSize = 18,
                Foreground = Brushes.LightGreen,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeight.Bold,
                TextWrapping = TextWrapping.Wrap
            };
            resultPanel.Children.Add(statsText);

            stack.Children.Add(resultPanel);
            mainBorder.Child = stack;

            AnimateRoulette(scrollPanel, resultPanel, rarityText, statsText, finalItem, finalRarity, wheelNumber);

            return mainBorder;
        }

        private async void AnimateRoulette(
            StackPanel scrollPanel, 
            StackPanel resultPanel, 
            TextBlock rarityText, 
            TextBlock statsText, 
            string finalItem, 
            string finalRarity,
            int wheelNumber)
        {
            var random = new Random();
            
            double startY = 0;
            double targetY = -450;
            
            double currentY = startY;
            int duration = 3000 + (wheelNumber * 500);
            int steps = 60;
            double delay = duration / steps;

            for (int i = 0; i <= steps; i++)
            {
                double progress = (double)i / steps;
                double easedProgress = 1 - Math.Pow(1 - progress, 3);
                
                currentY = startY + (targetY - startY) * easedProgress;
                
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    scrollPanel.Margin = new Thickness(0, currentY, 0, 0);
                });

                await Task.Delay((int)delay);
            }

            await Task.Delay(800);

            double multiplier = _rarityMultipliers[finalRarity];
            Color rarityColor = _rarityColors[finalRarity];
            string baseStats = _itemsBaseStats[finalItem];
            string finalStats = CalculateItemStats(baseStats, multiplier);

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                rarityText.Text = $"✨ {finalRarity.ToUpper()} ✨";
                rarityText.Foreground = new SolidColorBrush(rarityColor);
                
                statsText.Text = finalStats;
                statsText.Foreground = new SolidColorBrush(rarityColor);

                resultPanel.Opacity = 1;
            });
        }

//ÉCRAN TIRAGE DE DÉ
        
private void UpdateDiceFace(Canvas canvas, int value)
{
    canvas.Children.Clear();
    
    void AddDot(double x, double y)
    {
        var dot = new Avalonia.Controls.Shapes.Ellipse
        {
            Width = 28,
            Height = 28,
            Fill = new RadialGradientBrush
            {
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#ffffff"), 0),
                    new GradientStop(Color.Parse("#f1c40f"), 1)
                }
            }
        };
        Canvas.SetLeft(dot, x);
        Canvas.SetTop(dot, y);
        canvas.Children.Add(dot);
    }
    
    double centerX = 76;
    double centerY = 76;
    double offset = 50;
    
    switch (value)
    {
        case 1:
            AddDot(centerX, centerY);
            break;
        case 2:
            AddDot(centerX - offset, centerY - offset);
            AddDot(centerX + offset, centerY + offset);
            break;
        case 3:
            AddDot(centerX - offset, centerY - offset); 
            AddDot(centerX, centerY);                    
            AddDot(centerX + offset, centerY + offset); 
            break;
        case 4:
            AddDot(centerX - offset, centerY - offset); 
            AddDot(centerX + offset, centerY - offset); 
            AddDot(centerX - offset, centerY + offset); 
            AddDot(centerX + offset, centerY + offset); 
            break;
        case 5:
            AddDot(centerX - offset, centerY - offset); 
            AddDot(centerX + offset, centerY - offset); 
            AddDot(centerX, centerY);                    
            AddDot(centerX - offset, centerY + offset); 
            AddDot(centerX + offset, centerY + offset); 
            break;
        case 6:
            AddDot(centerX - offset, centerY - offset); 
            AddDot(centerX + offset, centerY - offset); 
            AddDot(centerX - offset, centerY);          
            AddDot(centerX + offset, centerY);          
            AddDot(centerX - offset, centerY + offset); 
            AddDot(centerX + offset, centerY + offset); 
            break;
    }
}

private void ShowDiceRollScreen()
{
    _mainContainer.Children.Clear();

    var mainGrid = new Grid();
    mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

    var header = new StackPanel { Margin = new Thickness(0, 40, 0, 20), HorizontalAlignment = HorizontalAlignment.Center };
    header.Children.Add(new TextBlock 
    { 
        Text = "Tirage du Dé", 
        FontSize = 36, 
        FontWeight = FontWeight.Bold, 
        Foreground = Brushes.White, 
        HorizontalAlignment = HorizontalAlignment.Center 
    });
    header.Children.Add(new TextBlock 
    { 
        Text = "Lancez le dé pour obtenir un bonus de stats !", 
        FontSize = 18, 
        Foreground = Brushes.LightGray, 
        HorizontalAlignment = HorizontalAlignment.Center, 
        FontStyle = FontStyle.Italic 
    });
    mainGrid.Children.Add(header);

    var contentPanel = new StackPanel
    {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Spacing = 40
    };

    var diceContainer = new Border
    {
        Width = 250,
        Height = 250,
        Background = Brushes.Transparent,
        ClipToBounds = false
    };

    var diceCanvas = new Canvas
    {
        Width = 200,
        Height = 200,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
    };

    var diceFace = new Border
    {
        Width = 180,
        Height = 180,
        CornerRadius = new CornerRadius(20),
        Background = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.Parse("#1a1a1a"), 0),
                new GradientStop(Color.Parse("#2d2d2d"), 1)
            }
        },
        BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")),
        BorderThickness = new Thickness(4),
        BoxShadow = BoxShadows.Parse("0 10 40 0 #f1c40f"),
        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
    };

    var diceDotsPanel = new Canvas
    {
        Width = 180,
        Height = 180
    };
    diceFace.Child = diceDotsPanel;

    Canvas.SetLeft(diceFace, 10);
    Canvas.SetTop(diceFace, 10);
    diceCanvas.Children.Add(diceFace);
    
    var questionMark = new TextBlock
    {
        Text = "?",
        FontSize = 120,
        FontWeight = FontWeight.Bold,
        Foreground = new SolidColorBrush(Color.Parse("#f1c40f")),
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };
    diceDotsPanel.Children.Add(questionMark);
    Canvas.SetLeft(questionMark, 40);
    Canvas.SetTop(questionMark, 20);

    diceContainer.Child = diceCanvas;
    contentPanel.Children.Add(diceContainer);

    var rollButton = new Button
    {
        Content = "🎲 LANCER LE DÉ",
        FontSize = 24,
        Padding = new Thickness(40, 15),
        CornerRadius = new CornerRadius(12),
        Background = new SolidColorBrush(Color.Parse("#e74c3c")),
        Foreground = Brushes.White,
        FontWeight = FontWeight.Bold,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    var resultPanel = new StackPanel
    {
        Spacing = 20,
        HorizontalAlignment = HorizontalAlignment.Center,
        Opacity = 0
    };

    var resultText = new TextBlock
    {
        Text = "",
        FontSize = 28,
        FontWeight = FontWeight.Bold,
        Foreground = new SolidColorBrush(Color.Parse("#2ecc71")),
        HorizontalAlignment = HorizontalAlignment.Center
    };
    resultPanel.Children.Add(resultText);

    var instructionText = new TextBlock
    {
        Text = "Choisissez l'attribut à améliorer :",
        FontSize = 20,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center
    };
    resultPanel.Children.Add(instructionText);

    var attributesPanel = new WrapPanel
    {
        HorizontalAlignment = HorizontalAlignment.Center,
        Orientation = Orientation.Horizontal
    };

    string[] attributes = { "Force", "Défense", "Vitesse", "Magie" };
    foreach (var attr in attributes)
    {
        var attrButton = new Button
        {
            Content = attr,
            FontSize = 20,
            Padding = new Thickness(25, 12),
            Margin = new Thickness(10),
            CornerRadius = new CornerRadius(10),
            Background = new SolidColorBrush(Color.Parse("#3498db")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold
        };
        attrButton.Click += (s, e) => ShowFinalSummary(attr);
        attributesPanel.Children.Add(attrButton);
    }
    resultPanel.Children.Add(attributesPanel);

    contentPanel.Children.Add(rollButton);
    contentPanel.Children.Add(resultPanel);

    Grid.SetRow(contentPanel, 1);
    mainGrid.Children.Add(contentPanel);
    _mainContainer.Children.Add(mainGrid);

    this.Title = $"Tirage du Dé - {_pseudo}";

    rollButton.Click += async (s, e) =>
    {
        rollButton.IsEnabled = false;
        rollButton.Opacity = 0.5;
        
        var random = new Random();
        _diceRollValue = random.Next(1, 7);
        
        var rotateTransform = new RotateTransform();
        diceFace.RenderTransform = rotateTransform;
        
        for (int i = 0; i < 30; i++)
        {
            double angle = i * 36;
            rotateTransform.Angle = angle;
            
            int tempValue = random.Next(1, 7);
            UpdateDiceFace(diceDotsPanel, tempValue);
            
            double scale = 1.0 + Math.Sin(angle * Math.PI / 180) * 0.2;
            var scaleTransform = new ScaleTransform(scale, scale);
            diceCanvas.RenderTransform = scaleTransform;
            
            await Task.Delay(50);
        }
        
        for (int i = 0; i < 20; i++)
        {
            double angle = (30 * 36) + (i * 18);
            rotateTransform.Angle = angle;
            
            int tempValue = random.Next(1, 7);
            UpdateDiceFace(diceDotsPanel, tempValue);
            
            double scale = 1.0 + Math.Sin(angle * Math.PI / 180) * 0.15;
            var scaleTransform = new ScaleTransform(scale, scale);
            diceCanvas.RenderTransform = scaleTransform;
            
            await Task.Delay(70 + i * 10);
        }
        
        rotateTransform.Angle = 0;
        diceCanvas.RenderTransform = new ScaleTransform(1, 1);
        UpdateDiceFace(diceDotsPanel, _diceRollValue);
        
        for (int i = 0; i < 3; i++)
        {
            var bounceScale = new ScaleTransform(1.15, 1.15);
            diceCanvas.RenderTransform = bounceScale;
            await Task.Delay(100);
            
            bounceScale = new ScaleTransform(0.95, 0.95);
            diceCanvas.RenderTransform = bounceScale;
            await Task.Delay(100);
        }
        
        diceCanvas.RenderTransform = new ScaleTransform(1, 1);
        
        diceFace.BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f"));
        diceFace.BoxShadow = BoxShadows.Parse("0 10 60 0 #f1c40f");
        
        await Task.Delay(500);

        resultText.Text = $"Vous avez obtenu : +{_diceRollValue}";
        resultPanel.Opacity = 1;
    };
}

private Dictionary<string, double> CalculateFinalStats(string selectedAttribute)
{
    var raceData = _raceData[_selectedRace];
    var stats = new Dictionary<string, double>();
    
    foreach (var line in raceData.Stats.Split('\n'))
    {
        var parts = line.Split(':');
        if (parts.Length == 2)
        {
            string statName = parts[0].Trim();
            string statValue = parts[1].Trim();
            
            if (statName == "Régénération PV")
            {
                var match = Regex.Match(statValue, @"(\d+(?:\.\d+)?)%");
                if (match.Success)
                {
                    stats[statName] = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            else
            {
                var match = Regex.Match(statValue, @"(\d+)");
                if (match.Success)
                {
                    stats[statName] = double.Parse(match.Groups[1].Value);
                }
            }
        }
    }
    
    var classBonus = _classStats[_selectedClass];
    foreach (var bonus in classBonus.Split(new[] { " et " }, StringSplitOptions.RemoveEmptyEntries))
    {
        var match = Regex.Match(bonus.Trim(), @"\+\s*(\d+(?:\.\d+)?)\s*(%?)\s*(.+)");
        if (match.Success)
        {
            double value = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            bool isPercent = match.Groups[2].Value == "%";
            string statName = match.Groups[3].Value.Trim();
            
            if (stats.ContainsKey(statName))
            {
                stats[statName] += value;
            }
        }
    }
    
    foreach (var item in _selectedItems)
    {
        foreach (var bonus in item.stats.Split(new[] { " et ", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var match = Regex.Match(bonus.Trim(), @"\+\s*(\d+(?:\.\d+)?)\s*(%?)\s*(.+)");
            if (match.Success)
            {
                double value = double.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                bool isPercent = match.Groups[2].Value == "%";
                string statName = match.Groups[3].Value.Trim();
                
                if (stats.ContainsKey(statName))
                {
                    stats[statName] += value;
                }
            }
        }
    }
    
    if (stats.ContainsKey(selectedAttribute))
    {
        stats[selectedAttribute] += _diceRollValue;
    }
    
    return stats;
}

private void ShowFinalSummary(string selectedAttribute)
{
    _mainContainer.Children.Clear();

    var finalStats = CalculateFinalStats(selectedAttribute);

    var scrollViewer = new ScrollViewer();
    var mainPanel = new StackPanel
    {
        Margin = new Thickness(50),
        Spacing = 30,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    var title = new TextBlock
    {
        Text = $"Personnage de {_pseudo}",
        FontSize = 42,
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center,
        Margin = new Thickness(0, 20, 0, 30)
    };
    mainPanel.Children.Add(title);

    var infoPanel = new StackPanel { Spacing = 10, HorizontalAlignment = HorizontalAlignment.Center };
    infoPanel.Children.Add(new TextBlock
    {
        Text = $"Race : {_selectedRace}",
        FontSize = 24,
        Foreground = Brushes.Cyan,
        HorizontalAlignment = HorizontalAlignment.Center
    });
    infoPanel.Children.Add(new TextBlock
    {
        Text = $"Classe : {_selectedClass}",
        FontSize = 24,
        Foreground = Brushes.Cyan,
        HorizontalAlignment = HorizontalAlignment.Center
    });
    mainPanel.Children.Add(infoPanel);


    //STATS FINALES
    var statsTitle = new TextBlock
    {
        Text = "📊 Statistiques Finales",
        FontSize = 32,
        FontWeight = FontWeight.Bold,
        Foreground = new SolidColorBrush(Color.Parse("#f1c40f")),
        HorizontalAlignment = HorizontalAlignment.Center,
        Margin = new Thickness(0, 30, 0, 20)
    };
    mainPanel.Children.Add(statsTitle);

    var raceImageBorder = new Border
    {
        Width = 200,
        Height = 280,
        CornerRadius = new CornerRadius(15),
        ClipToBounds = true,
        BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")),
        BorderThickness = new Thickness(4),
        HorizontalAlignment = HorizontalAlignment.Center,
        Margin = new Thickness(0, 0, 0, 20)
    };

    var raceImage = new Image { Stretch = Stretch.UniformToFill };
    try
    {
        raceImage.Source = new Bitmap(AssetLoader.Open(new Uri(_raceImages[_selectedRace])));
        raceImageBorder.Child = raceImage;
    }
    catch
    {
        raceImageBorder.Background = new SolidColorBrush(GetRaceColor(_selectedRace));
    }

    mainPanel.Children.Add(raceImageBorder);

    var statsBorder = new Border
    {
        Width = 600,
        Padding = new Thickness(30),
        CornerRadius = new CornerRadius(20),
        Background = new SolidColorBrush(Colors.Black) { Opacity = 0.8 },
        BorderBrush = new SolidColorBrush(Color.Parse("#f1c40f")),
        BorderThickness = new Thickness(3)
    };

    var statsPanel = new StackPanel { Spacing = 15 };

    string[] statOrder = { "Force", "Défense", "Vitesse", "PV", "Magie", "Régénération PV" };
    foreach (var statName in statOrder)
    {
        if (finalStats.ContainsKey(statName))
        {
            var statRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 15,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            if (_statIcons.ContainsKey(statName))
            {
                try
                {
                    var icon = new Image
                    {
                        Source = new Bitmap(AssetLoader.Open(new Uri(_statIcons[statName]))),
                        Width = 32,
                        Height = 32,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    statRow.Children.Add(icon);
                }
                catch { }
            }

            var nameText = new TextBlock
            {
                Text = $"{statName}:",
                FontSize = 22,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Width = 180,
                VerticalAlignment = VerticalAlignment.Center
            };
            statRow.Children.Add(nameText);

            string valueStr = statName == "Régénération PV" 
                ? $"{Math.Round(finalStats[statName], 1)}% /sec"
                : Math.Round(finalStats[statName]).ToString();

            var valueText = new TextBlock
            {
                Text = valueStr,
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                Foreground = new SolidColorBrush(Color.Parse("#2ecc71")),
                VerticalAlignment = VerticalAlignment.Center
            };
            statRow.Children.Add(valueText);

            if (statName == selectedAttribute)
            {
                var diceIcon = new TextBlock
                {
                    Text = $"🎲 +{_diceRollValue}",
                    FontSize = 18,
                    FontWeight = FontWeight.Bold,
                    Foreground = new SolidColorBrush(Color.Parse("#f1c40f")),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                statRow.Children.Add(diceIcon);
            }

            statsPanel.Children.Add(statRow);
        }
    }

    statsBorder.Child = statsPanel;
    mainPanel.Children.Add(statsBorder);

    var raceData = _raceData[_selectedRace];
    var competenceTitle = new TextBlock
    {
        Text = "⚡ Compétence Raciale",
        FontSize = 28,
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center,
        Margin = new Thickness(0, 20, 0, 10)
    };
    mainPanel.Children.Add(competenceTitle);

    var competenceBorder = new Border
    {
        Width = 500,
        Padding = new Thickness(20),
        CornerRadius = new CornerRadius(15),
        Background = new SolidColorBrush(Colors.Black) { Opacity = 0.7 },
        BorderBrush = new SolidColorBrush(Color.Parse("#e74c3c")),
        BorderThickness = new Thickness(3)
    };

    var competenceText = new TextBlock
    {
        Text = raceData.Competence,
        FontSize = 18,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center,
        TextWrapping = TextWrapping.Wrap,
        TextAlignment = TextAlignment.Center
    };
    competenceBorder.Child = competenceText;
    mainPanel.Children.Add(competenceBorder);

    var itemsTitle = new TextBlock
    {
        Text = "🎒 Équipements Obtenus",
        FontSize = 28,
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.White,
        HorizontalAlignment = HorizontalAlignment.Center,
        Margin = new Thickness(0, 20, 0, 10)
    };
    mainPanel.Children.Add(itemsTitle);

    var itemsPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 30 };
    foreach (var item in _selectedItems)
    {
        var itemBorder = new Border
        {
            Width = 280,
            Padding = new Thickness(15),
            CornerRadius = new CornerRadius(15),
            Background = new SolidColorBrush(Colors.Black) { Opacity = 0.7 },
            BorderBrush = new SolidColorBrush(_rarityColors[item.rarity]),
            BorderThickness = new Thickness(3)
        };

        var itemStack = new StackPanel { Spacing = 10 };
        
        if (_itemImages.ContainsKey(item.itemName))
        {
            try
            {
                string fileName = _itemImages[item.itemName];
                var uri = new Uri($"avares://CSHARPEMV1/Assets/{fileName}");
                var itemImage = new Image
                {
                    Source = new Bitmap(AssetLoader.Open(uri)),
                    Width = 100,
                    Height = 100,
                    Stretch = Stretch.UniformToFill,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                var imageBorder = new Border
                {
                    Child = itemImage,
                    CornerRadius = new CornerRadius(10),
                    ClipToBounds = true
                };
                itemStack.Children.Add(imageBorder);
            }
            catch { }
        }
        
        itemStack.Children.Add(new TextBlock
        {
            Text = item.itemName,
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center
        });
        itemStack.Children.Add(new TextBlock
        {
            Text = item.rarity.ToUpper(),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(_rarityColors[item.rarity]),
            HorizontalAlignment = HorizontalAlignment.Center
        });
        itemStack.Children.Add(new TextBlock
        {
            Text = item.stats,
            FontSize = 16,
            Foreground = Brushes.LightGreen,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center
        });

        itemBorder.Child = itemStack;
        itemsPanel.Children.Add(itemBorder);
    }
    mainPanel.Children.Add(itemsPanel);

    var buttonsPanel = new StackPanel
    {
        Orientation = Orientation.Horizontal,
        HorizontalAlignment = HorizontalAlignment.Center,
        Spacing = 20,
        Margin = new Thickness(0, 30, 0, 30)
    };

    var restartButton = new Button
    {
        Content = "🔄 NOUVEAU PERSONNAGE",
        FontSize = 20,
        Padding = new Thickness(30, 15),
        CornerRadius = new CornerRadius(12),
        Background = new SolidColorBrush(Color.Parse("#3498db")),
        Foreground = Brushes.White,
        FontWeight = FontWeight.Bold
    };
    restartButton.Click += (s, e) =>
    {
        _selectedItems.Clear();
        _diceRollValue = 0;
        ShowWelcomeScreen();
    };
    buttonsPanel.Children.Add(restartButton);

    var finishButton = new Button
    {
        Content = "🎉 TERMINER",
        FontSize = 20,
        Padding = new Thickness(30, 15),
        CornerRadius = new CornerRadius(12),
        Background = new SolidColorBrush(Color.Parse("#2ecc71")),
        Foreground = Brushes.White,
        FontWeight = FontWeight.Bold
    };
    finishButton.Click += (s, e) =>
    {
        this.Close();
    };
    buttonsPanel.Children.Add(finishButton);

    mainPanel.Children.Add(buttonsPanel);

    scrollViewer.Content = mainPanel;
    _mainContainer.Children.Add(scrollViewer);

    this.Title = $"Récapitulatif - {_pseudo}";
}

        //UTILITAIRES
        
        private string GetRandomRarityByDropRate(Random random)
        {
            int roll = random.Next(100); 
            
            if (roll < 40) return "Commun";       
            if (roll < 75) return "Rare";          
            if (roll < 90) return "Épique";       
            return "Légendaire";                    
        }
        private string CalculateItemStats(string baseStats, double multiplier)
        {
            string pattern = @"(\d+(?:\.\d+)?)";

            string result = Regex.Replace(baseStats, pattern, match =>
            {
                if (double.TryParse(match.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double value))
                {
                    double newValue = value * multiplier;
                    
                    if (baseStats.Contains("%") && value < 20) 
                        return Math.Round(newValue, 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                        return Math.Round(newValue).ToString();
                }
                return match.Value;
            });

            return result;
        }

        private StackPanel CreateStatRow(string name, string value)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
            if (_statIcons.ContainsKey(name))
            {
                try { row.Children.Add(new Image { Source = new Bitmap(AssetLoader.Open(new Uri(_statIcons[name]))), Width = 24, Height = 24 }); } catch { }
            }
            row.Children.Add(new TextBlock { Text = $"{name}: {value}", FontSize = 18, Foreground = Brushes.LightGray });
            return row;
        }

        private StackPanel CreateClassStatRow(string text)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Center };
            foreach (var k in _statIcons.Keys)
            {
                if (text.Contains(k))
                {
                    try { row.Children.Add(new Image { Source = new Bitmap(AssetLoader.Open(new Uri(_statIcons[k]))), Width = 20, Height = 20 }); } catch { }
                    break;
                }
            }
            row.Children.Add(new TextBlock { Text = text, FontSize = 18, Foreground = new SolidColorBrush(Color.Parse("#2ecc71")), FontWeight = FontWeight.Bold });
            return row;
        }

        private Color GetRaceColor(string race)
        {
            return race switch
            {
                "Harpie" => Color.Parse("#9b59b6"),
                "Ogre" => Color.Parse("#27ae60"),
                "Gobelin" => Color.Parse("#f39c12"),
                "Elfe" => Color.Parse("#3498db"),
                "Vampire" => Color.Parse("#c0392b"),
                "Humain" => Color.Parse("#7f8c8d"),
                _ => Colors.DarkGray
            };
        }
    }
}