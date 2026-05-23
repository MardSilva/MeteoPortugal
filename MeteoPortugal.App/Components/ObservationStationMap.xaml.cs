using System.Collections;
using MeteoPortugal.App.Models;

namespace MeteoPortugal.App.Components;

public partial class ObservationStationMap : ContentView
{
    public static readonly BindableProperty StationsProperty = BindableProperty.Create(
        nameof(Stations),
        typeof(IEnumerable),
        typeof(ObservationStationMap),
        null,
        propertyChanged: OnStationsChanged);

    private readonly ObservationStationDrawable drawable = new();

    public ObservationStationMap()
    {
        InitializeComponent();
        MapCanvas.Drawable = drawable;
    }

    public IEnumerable? Stations
    {
        get => (IEnumerable?)GetValue(StationsProperty);
        set => SetValue(StationsProperty, value);
    }

    private static void OnStationsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var map = (ObservationStationMap)bindable;
        map.drawable.Stations = (newValue as IEnumerable)?.OfType<ObservationStation>().ToList() ?? [];
        map.MapCanvas.Invalidate();
    }

    private sealed class ObservationStationDrawable : IDrawable
    {
        private const double MinLatitude = 36.8;
        private const double MaxLatitude = 42.4;
        private const double MinLongitude = -9.7;
        private const double MaxLongitude = -6.0;

        public IReadOnlyList<ObservationStation> Stations { get; set; } = [];

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromArgb("#F4F8FB");
            canvas.FillRoundedRectangle(dirtyRect, 8);

            var mapRect = new RectF(
                dirtyRect.X + 22,
                dirtyRect.Y + 18,
                dirtyRect.Width - 44,
                dirtyRect.Height - 36);

            DrawPortugalShape(canvas, mapRect);

            foreach (var station in Stations)
            {
                var point = Project(station, mapRect);
                var radius = station.DistanceKm < 35 ? 7 : 5;

                canvas.FillColor = Color.FromArgb(station.DistanceKm < 35 ? "#FDB515" : "#0F6CBD");
                canvas.FillCircle(point.X, point.Y, radius);
                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize = 2;
                canvas.DrawCircle(point.X, point.Y, radius);
            }

            canvas.FontColor = Color.FromArgb("#5B6773");
            canvas.FontSize = 12;
            canvas.DrawString("Estações IPMA próximas", dirtyRect.X + 16, dirtyRect.Bottom - 26, HorizontalAlignment.Left);
        }

        private static void DrawPortugalShape(ICanvas canvas, RectF rect)
        {
            var path = new PathF();
            path.MoveTo(rect.X + rect.Width * 0.48f, rect.Y);
            path.LineTo(rect.X + rect.Width * 0.63f, rect.Y + rect.Height * 0.12f);
            path.LineTo(rect.X + rect.Width * 0.58f, rect.Y + rect.Height * 0.30f);
            path.LineTo(rect.X + rect.Width * 0.66f, rect.Y + rect.Height * 0.46f);
            path.LineTo(rect.X + rect.Width * 0.55f, rect.Y + rect.Height * 0.66f);
            path.LineTo(rect.X + rect.Width * 0.60f, rect.Y + rect.Height * 0.88f);
            path.LineTo(rect.X + rect.Width * 0.45f, rect.Bottom);
            path.LineTo(rect.X + rect.Width * 0.33f, rect.Y + rect.Height * 0.82f);
            path.LineTo(rect.X + rect.Width * 0.39f, rect.Y + rect.Height * 0.58f);
            path.LineTo(rect.X + rect.Width * 0.31f, rect.Y + rect.Height * 0.38f);
            path.LineTo(rect.X + rect.Width * 0.40f, rect.Y + rect.Height * 0.16f);
            path.Close();

            canvas.FillColor = Color.FromArgb("#DDEEFF");
            canvas.FillPath(path);
            canvas.StrokeColor = Color.FromArgb("#9FC7E8");
            canvas.StrokeSize = 2;
            canvas.DrawPath(path);
        }

        private static PointF Project(ObservationStation station, RectF rect)
        {
            var xRatio = (station.Longitude - MinLongitude) / (MaxLongitude - MinLongitude);
            var yRatio = (MaxLatitude - station.Latitude) / (MaxLatitude - MinLatitude);

            return new PointF(
                rect.X + (float)(xRatio * rect.Width),
                rect.Y + (float)(yRatio * rect.Height));
        }
    }
}
