using System;
using System.Diagnostics;
using System.Drawing;
using Overlay.NET.Common;
using Overlay.NET.Demo.Internals;
using Overlay.NET.Directx;
using Process.NET.Windows;

namespace Overlay.NET.Demo.Directx
{
    [RegisterPlugin("DirectXverlayDemo-1", "Jacob Kemple", "DirectXOverlayDemo", "0.0",
         "A basic demo of the DirectXoverlay.")]
    public class DirectxOverlayPluginExample : DirectXOverlayPlugin
    {
        readonly ISettings<DemoOverlaySettings> _settings = new SerializableSettings<DemoOverlaySettings>();
        readonly TickEngine _tickEngine = new TickEngine();
        int _displayFps;
        int _font;
        int _hugeFont;
        int _i;
        int _interiorBrush;
        int _redBrush;
        int _redOpacityBrush;
        float _rotation;
        Stopwatch _watch;

        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);

            // For demo, show how to use settings
            var current = _settings.Current;
            var type = GetType();

            current.UpdateRate = 1000/60;
            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            _settings.Save();
            _settings.Load();
            Console.Title = @"OverlayExample";

            OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
            _watch = Stopwatch.StartNew();

            _redBrush = OverlayWindow.Graphics.CreateBrush(0x7FFF0000);
            _redOpacityBrush = OverlayWindow.Graphics.CreateBrush(Color.FromArgb(80, 255, 0, 0));
            _interiorBrush = OverlayWindow.Graphics.CreateBrush(0x7FFFFF00);

            _font = OverlayWindow.Graphics.CreateFont("Arial", 20);
            _hugeFont = OverlayWindow.Graphics.CreateFont("Arial", 50, true);

            _rotation = 0.0f;
            _displayFps = 0;
            _i = 0;
            // Set up update interval and register events for the tick engine.
            _tickEngine.Interval = _settings.Current.UpdateRate.Milliseconds();
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        void OnTick(object sender, EventArgs e)
        {
            if (OverlayWindow.IsVisible)
            {
                OverlayWindow.SetBounds(TargetWindow.X, TargetWindow.Y, TargetWindow.Width, TargetWindow.Height);
                InternalRender();
            }
        }

        void OnPreTick(object sender, EventArgs e)
        {
            var activated = TargetWindow.IsActivated;
            var visible = OverlayWindow.IsVisible;

            // Ensure window is shown or hidden correctly prior to updating
            if (!activated && visible)
            {
                OverlayWindow.Hide();
            }

            else if (activated && !visible)
            {
                OverlayWindow.Show();
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        // ReSharper disable once RedundantOverriddenMember
        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Update()
        {
            _tickEngine.Pulse();
            base.Update();
        }

        protected void InternalRender()
        {
            OverlayWindow.Graphics.BeginScene();
            OverlayWindow.Graphics.ClearScene();

            //first row
            OverlayWindow.Graphics.DrawText("DrawBarH", _font, _redBrush, 50, 40);
            OverlayWindow.Graphics.DrawBarH(50, 70, 20, 100, 80, 2, _redBrush, _interiorBrush);

            OverlayWindow.Graphics.DrawText("DrawBarV", _font, _redBrush, 200, 40);
            OverlayWindow.Graphics.DrawBarV(200, 120, 100, 20, 80, 2, _redBrush, _interiorBrush);

            OverlayWindow.Graphics.DrawText("DrawBox2D", _font, _redBrush, 350, 40);
            OverlayWindow.Graphics.DrawBox2D(350, 70, 50, 100, 2, _redBrush, _redOpacityBrush);

            OverlayWindow.Graphics.DrawText("DrawBox3D", _font, _redBrush, 500, 40);
            OverlayWindow.Graphics.DrawBox3D(500, 80, 50, 100, 10, 2, _redBrush, _redOpacityBrush);

            OverlayWindow.Graphics.DrawText("DrawCircle3D", _font, _redBrush, 650, 40);
            OverlayWindow.Graphics.DrawCircle(700, 120, 35, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawEdge", _font, _redBrush, 800, 40);
            OverlayWindow.Graphics.DrawEdge(800, 70, 50, 100, 10, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawLine", _font, _redBrush, 950, 40);
            OverlayWindow.Graphics.DrawLine(950, 70, 1000, 200, 2, _redBrush);

            //second row
            OverlayWindow.Graphics.DrawText("DrawPlus", _font, _redBrush, 50, 250);
            OverlayWindow.Graphics.DrawPlus(70, 300, 15, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawRectangle", _font, _redBrush, 200, 250);
            OverlayWindow.Graphics.DrawRectangle(200, 300, 50, 100, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("DrawRectangle3D", _font, _redBrush, 350, 250);
            OverlayWindow.Graphics.DrawRectangle3D(350, 320, 50, 100, 10, 2, _redBrush);

            OverlayWindow.Graphics.DrawText("FillCircle", _font, _redBrush, 800, 250);
            OverlayWindow.Graphics.FillCircle(850, 350, 50, _redBrush);

            OverlayWindow.Graphics.DrawText("FillRectangle", _font, _redBrush, 950, 250);
            OverlayWindow.Graphics.FillRectangle(950, 300, 50, 100, _redBrush);

            _rotation += 0.03f; //related to speed

            if (_rotation > 50.0f) //size of the swastika
            {
                _rotation = -50.0f;
            }

            if (_watch.ElapsedMilliseconds > 1000)
            {
                _i = _displayFps;
                _displayFps = 0;
                _watch.Restart();
            }

            else
            {
                _displayFps++;
            }

            OverlayWindow.Graphics.DrawText("FPS: " + _i, _hugeFont, _redBrush, 400, 600, false);

            OverlayWindow.Graphics.EndScene();
        }

        public override void Dispose()
        {
            OverlayWindow.Dispose();
            base.Dispose();
        }
    }
}