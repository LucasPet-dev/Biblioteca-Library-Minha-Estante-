using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace MinhaEstante.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        using var stream = AssetLoader.Open(new Uri("avares://MinhaEstante.Presentation/Assets/Logo.png"));
        Icon = new WindowIcon(new Bitmap(stream));
    }
}