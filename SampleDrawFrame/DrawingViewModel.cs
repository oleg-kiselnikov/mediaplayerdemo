using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SampleDrawFrame
{
    public class DrawingViewModel : IDrawingViewModel
    {
        private bool hanldeCollectionChanges = true;

        private readonly List<Curve> removedCurves = new List<Curve>();
        private readonly List<Curve> addedCurves = new List<Curve>();
        public ObservableCollection<Curve> Curves { get; private set; }

        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }

        public Size DrawingAreaSize { get; set; }

        public Color PenColor { get; set; }

        public int PenWidth { get; set; } 

        private readonly IMediaPlayer mediaPlayer;
        public DrawingViewModel(IMediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;

            Curves = new ObservableCollection<Curve>();
            Curves.CollectionChanged += Curves_CollectionChanged;

            UndoCommand = new RelayCommand(Undo, CanUndo);
            RedoCommand = new RelayCommand(Redo, CanRedo);
            ClearAllCommand = new RelayCommand(ClearAll, CanClearAll);

            mediaPlayer.BeforeRendering += (s, e) =>
            {
                if (DrawingAreaSize.Width < 1 || DrawingAreaSize.Height < 1) return;

                var penColor = System.Drawing.Color.FromArgb(PenColor.A, PenColor.R, PenColor.G, PenColor.B);
                var penWidth = PenWidth > 0 ? PenWidth : 1;


                using (var graphics = System.Drawing.Graphics.FromImage(e.Bitmap))
                {
                    foreach (var curve in Curves)
                    {
                        try
                        {
                            if (curve.Points.Count < 2)
                                continue;

                            if (curve.Pen == null)
                                curve.Pen = new System.Drawing.Pen(penColor, penWidth);

                            var points = curve.Points.Select(p =>
                            {
                                double x = e.Bitmap.Width * p.X / DrawingAreaSize.Width;
                                double y = e.Bitmap.Height * p.Y / DrawingAreaSize.Height;

                                return new System.Drawing.Point((int)x, (int)y);
                            }).ToArray();

                            graphics.DrawLines(curve.Pen, points);
                        }
                        catch { }
                    }
                }
            };
        }

        

        public bool CanUndo()
        {
            return addedCurves.Any();
        }

        public void Undo()
        {
            if (addedCurves.Any())
            {
                var curve = addedCurves[addedCurves.Count - 1];
                addedCurves.RemoveAt(addedCurves.Count - 1);
                hanldeCollectionChanges = false;
                try
                {
                    Curves.Remove(curve);
                    removedCurves.Add(curve);
                }
                finally
                {
                    hanldeCollectionChanges = true;
                }
            }
        }


        public bool CanRedo()
        {
            return removedCurves.Any();
        }

        public void Redo()
        {
            if (removedCurves.Any())
            {
                var curve = removedCurves[removedCurves.Count - 1];
                removedCurves.RemoveAt(removedCurves.Count - 1);
                hanldeCollectionChanges = false;
                try
                {
                    Curves.Add(curve);
                    addedCurves.Add(curve);
                }
                finally
                {
                    hanldeCollectionChanges = true;
                }
            }
        }


        public bool CanClearAll()
        {
            return addedCurves.Any();
        }

        public void ClearAll()
        {
            hanldeCollectionChanges = false;
            try
            {
                Curves.Clear();
                addedCurves.Clear();
                removedCurves.Clear();
            }
            finally
            {
                hanldeCollectionChanges = true;
            }
        }


        private void Curves_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!hanldeCollectionChanges)
                return;

            removedCurves.Clear();

            if (e.OldItems != null)
            {
                foreach (Curve curve in e.OldItems)
                    addedCurves.Remove(curve);
            }

            if (e.NewItems != null)
                foreach (Curve curve in e.NewItems)
                    addedCurves.Add(curve);

            mediaPlayer.ForseFrameRefreshOnPause();
        }
    }
}
