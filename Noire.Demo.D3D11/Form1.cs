﻿using System;
using System.Windows.Forms;
using Noire.Graphics.D3D11;
using Noire.Graphics.D3D11.FX;
using SharpDX;

namespace Noire.Demo.D3D11 {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeExtraControls();

            _app = D3DApp11.Create(label1);
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            ResizeEnd += Form1_ResizeEnd;
            SizeChanged += Form1_SizeChanged;
            FormClosed += Form1_FormClosed;
            Activated += Form1_Activated;
            Deactivate += Form1_Deactivate;
            label1.MouseDown += Form1_MouseDown;
            label1.MouseUp += Form1_MouseUp;
            label1.MouseMove += Form1_MouseMove;
            timer1.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            Text = $"{_app.Fps.ToString("0.00")} fps on {_app.DriverName}";
        }

        private void InitializeExtraControls() {
            var mnuMain = new ToolStripMenuItem("调整(&A)");
            menuStrip1.Items.Add(mnuMain);
            var mnuLights = new ToolStripMenuItem("灯光(&L)");
            mnuMain.DropDownItems.Add(mnuLights);
            for (var i = 0; i < BasicEffect11.MaxLights; ++i) {
                var m = new ToolStripMenuItem($"灯光 #{i}");
                m.Click += LightMenuItem_Click;
                m.Tag = i + 1;
                mnuLights.DropDownItems.Add(m);
            }
            (mnuLights.DropDownItems[0] as ToolStripMenuItem).Checked = true;
            timer1.Enabled = true;
            mnuMain.DropDownItems.Add(new ToolStripSeparator());
            var mnuToggleWireframe = new ToolStripMenuItem("线框模式(&W)");
            mnuMain.DropDownItems.Add(mnuToggleWireframe);
            mnuToggleWireframe.Click += ToggleWireframeRenderMode_Click;
            mnuToggleWireframe.CheckOnClick = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var camera = D3DApp11.I.Camera;
                var dx = MathUtil.DegreesToRadians(0.25f * (e.X - _lastMousePos.X));
                var dy = MathUtil.DegreesToRadians(0.25f * (e.Y - _lastMousePos.Y));

                camera.Pitch(dy);
                camera.Yaw(dx);
            }
            _lastMousePos = e.Location;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            var c = sender as Control;
            if (c != null) {
                c.Capture = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            _lastMousePos = e.Location;
            var c = sender as Control;
            if (c != null) {
                c.Capture = true;
            }
        }

        private void LightMenuItem_Click(object sender, EventArgs e) {
            var mnu = sender as ToolStripMenuItem;
            var parent = mnu.OwnerItem as ToolStripMenuItem;
            foreach (ToolStripMenuItem item in parent.DropDownItems) {
                item.Checked = item == mnu;
            }
            var s = _app.GetChildByType<ShadowScene>();
            //s.LightCount = (int)mnu.Tag;
        }

        private void ToggleWireframeRenderMode_Click(object sender, EventArgs e) {
            var item = sender as ToolStripMenuItem;
            var scene = _app.GetChildByType<ShadowScene>();
            if (scene != null) {
                scene.RenderInWireframe = item.Checked;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) {
                _lastWindowState = FormWindowState.Minimized;
            } else {
                if (WindowState != _lastWindowState) {
                    _app.ResetSurface(this);
                    _lastWindowState = WindowState;
                }
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e) {
            _app.IsPaused = true;
        }

        private void Form1_Activated(object sender, EventArgs e) {
            _app.IsPaused = false;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e) {
            if (WindowState != FormWindowState.Minimized) {
                _app?.ResetSurface(this);
            }
        }

        private void Form1_FormClosed(object sender, EventArgs e) {
            _app.Terminate();
            _app.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e) {
            _lastWindowState = WindowState;
            _app.Initialize();
            _app.RunAsync();

            var camera = _app.Camera;
            camera.Position = new Vector3(0, 5, -15);
            camera.LookAt(Vector3.Zero, Vector3.UnitY);

            var scene = new ShadowScene(_app, _app);
            scene.Initialize();
            scene.Name = "ShapesScene";
            _app.ChildComponents.Add(scene);
            var inputHandler = new InputHandler(_app, _app);
            inputHandler.Initialize();
            _app.ChildComponents.Add(inputHandler);
        }

        private readonly D3DApp11 _app;
        private System.Drawing.Point _lastMousePos;
        private FormWindowState _lastWindowState;

    }
}
