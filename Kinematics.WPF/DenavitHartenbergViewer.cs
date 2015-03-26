// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="DenavitHartenbergViewer.cs">
//   
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kinematics.WPF
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Accord.Math.Kinematics;

    using AForge.Math;

    using Color = System.Windows.Media.Color;
    using FontFamily = System.Windows.Media.FontFamily;
    using Point = System.Windows.Point;

    /// <summary>
    ///   Denavit Hartenberg Viewer.
    /// </summary>
    /// <remarks>
    ///   This class can be used to visualize a D-H model as bitmaps.
    /// </remarks>
    /// 
    public class DenavitHartenbergViewer
    {
        // Bitmaps (one for XY, one for YZ and one for XZ planes)
        /// <summary>
        /// </summary>
        WriteableBitmap xy;

        /// <summary>
        /// </summary>
        WriteableBitmap yz;

        /// <summary>
        /// </summary>
        WriteableBitmap xz;

        /// <summary>
        ///   Gets or sets the color of the links between joints
        /// </summary>
        /// 
        public Color LinksColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the joints
        /// </summary>
        /// 
        public Color JointsColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the last joint of a model
        /// </summary>
        /// 
        public Color EndJointColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the first joint of a model
        /// </summary>
        /// 
        public Color BaseJointColor { get; set; }

        /// <summary>
        ///   Gets or sets the color of the rendering surface background
        /// </summary>
        /// 
        public Color BackColor { get; set; }

        /// <summary>
        ///   Gets or sets the value to scale the drawing of the model to fit the window. Default is 1.
        /// </summary>
        /// 
        public float Scale { get; set; }

        /// <summary>
        ///   Gets or sets the radius of the joints circles. Default is 8.
        /// </summary>
        /// 
        public float JointRadius { get; set; }

        /// <summary>
        ///   Gets or sets the arrows indicating the axes on each drawing represented as a Rectangle object.
        /// </summary>
        /// 
        public Rectangle ArrowsBoundingBox { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenavitHartenbergViewer"/> class.
        /// </summary>
        /// <param name="width">
        /// Width of the drawing window
        /// </param>
        /// <param name="height">
        /// Height of the drawing window
        /// </param>
        public DenavitHartenbergViewer(int width, int height)
        {
            // Setting the default parameters
            this.BackColor = Colors.Black;
            this.LinksColor = Colors.Green;
            this.JointsColor = Colors.Red;
            this.BaseJointColor = Colors.Gold;
            this.EndJointColor = Colors.Blue;
            this.Scale = 1;
            this.JointRadius = 8;

            this.ArrowsBoundingBox = new Rectangle(10, height - 80, 50, 50);


            // Creating the bitmap and graphics
            this.xy = BitmapFactory.New(width, height);
            this.yz = BitmapFactory.New(width, height);
            this.xz = BitmapFactory.New(width, height);
        }

        /// <summary>
        ///   Image of the model viewed on the XY plane.
        /// </summary>
        /// 
        public BitmapSource PlaneXY
        {
            get { return this.xy; }
        }

        /// <summary>
        ///   Image of the model viewed on the YZ plane.
        /// </summary>
        /// 
        public BitmapSource PlaneYZ
        {
            get { return this.yz; }
        }

        /// <summary>
        ///   Image of the model viewed on the XZ plane.
        /// </summary>
        /// 
        public BitmapSource PlaneXZ
        {
            get { return this.xz; }
        }

        /// <summary>
        /// Makes a list of all the models contained on a
        ///   ModelCombinator. This function is recursive.
        /// </summary>
        /// <param name="model">
        /// ModelCombinator model in which to extract all the models. 
        /// </param>
        /// <param name="models">
        /// List of already extracted models. It accumulates all the 
        ///   models at each call of this function.
        /// </param>
        /// <returns>
        /// Returns a list of all the models contained in the 
        /// ModelCombinator 'model' plus all previously extracted models
        /// </returns>
        private List<DenavitHartenbergModel> GetAllModels(DenavitHartenbergNode model, 
            List<DenavitHartenbergModel> models)
        {
            // If it's the first call
            if (models == null)
            {
                // Create the models list
                models = new List<DenavitHartenbergModel>();
            }

            // Add the model contained in the ModelCombinator
            models.Add(model.Model);

            // For all the children of the ModelCombinator
            foreach (DenavitHartenbergNode child in model.Children)
            {
                // Execute recursively this function 
                models = this.GetAllModels(child, models);
            }

            // Return the models list
            return models;
        }

        /// <summary>
        /// Computes the three images of a list of ModelCombinator
        /// </summary>
        /// <param name="model">
        /// List of arguments of models to be drawn
        /// </param>
        /// <remarks>
        /// This function assumes that the models have already been calculated.
        /// </remarks>
        public void ComputeImages(params DenavitHartenbergNode[] model)
        {
            // List of models we will render
            List<DenavitHartenbergModel> models_to_render = new List<DenavitHartenbergModel>();

            // For each model on the argument list
            for (int i = 0; i < model.Length; i++)
            {
                // Add the models extracted by the GetAllModels function to the list
                models_to_render.AddRange(this.GetAllModels(model[i], null));
            }

            // Draw the extracted models
            this.ComputeImages(models_to_render.ToArray());
        }

        /// <summary>
        /// Computes the three images of a list of models
        /// </summary>
        /// <param name="model">
        /// List of arguments of models
        /// </param>
        public void ComputeImages(params DenavitHartenbergModel[] model)
        {
            // Clear the 3 images with the background color
            this.xy.Clear(this.BackColor);
            this.xz.Clear(this.BackColor);
            this.yz.Clear(this.BackColor);

            // Draw each model
            for (int i = 0; i < model.Length; i++)
            {
                DenavitHartenbergModel mdl = model[i];

                // Draw each link of the model
                Vector3 previous = mdl.Position;

                for (var j = 0; j < mdl.Joints.Count; j++)
                {
                    Vector3 current = mdl.Joints[j].Position;

                    // XY
                    this.xy.DrawLine( 
                        (int)Math.Round((this.xy.Width / 2) + previous.X * this.Scale), 
                        (int)Math.Round((this.xy.Height / 2) - previous.Y * this.Scale), 
                        (int)Math.Round((this.xy.Width / 2) + current.X * this.Scale), 
                        (int)Math.Round((this.xy.Height / 2) - current.Y * this.Scale), 
                        this.LinksColor);

                    // ZY
                    this.yz.DrawLine(
                        (int)Math.Round((this.yz.Width / 2) + previous.Z * this.Scale), 
                        (int)Math.Round((this.yz.Height / 2) - previous.Y * this.Scale), 
                        (int)Math.Round((this.yz.Width / 2) + current.Z * this.Scale), 
                        (int)Math.Round((this.yz.Height / 2) - current.Y * this.Scale), 
                        this.LinksColor);

                    // XZ
                    this.xz.DrawLine(
                        (int)Math.Round((this.xz.Width / 2) + previous.X * this.Scale), 
                        (int)Math.Round((this.xz.Height / 2) - previous.Z * this.Scale), 
                        (int)Math.Round((this.xz.Width / 2) + current.X * this.Scale), 
                        (int)Math.Round((this.xz.Height / 2) - current.Z * this.Scale), 
                        this.LinksColor);

                    previous = current;
                }

                Color pJoints;

                // Draw each joint
                for (var j = 0; j < mdl.Joints.Count + 1; j++)
                {
                    Vector3 current;

                    // Select the color of the joint
                    if (j == 0)
                    {
                        pJoints = this.BaseJointColor;
                        current = mdl.Position;
                    }
                    else
                    {
                        current = mdl.Joints[j - 1].Position;

                        if (j == mdl.Joints.Count)
                        {
                            pJoints = this.EndJointColor;
                        }
                        else
                        {
                            pJoints = this.JointsColor;
                        }
                    }

                    // XY
                    this.xy.FillEllipseCentered(
                        (int)Math.Round((this.xy.Width / 2) + current.X * this.Scale), 
                        (int)Math.Round((this.xy.Height / 2) - current.Y * this.Scale), 
                        (int)Math.Round(this.JointRadius), 
                        (int)Math.Round(this.JointRadius), 
                        pJoints);

                    // YZ
                    this.yz.FillEllipseCentered(
                        (int)Math.Round((this.yz.Width / 2) + current.Z * this.Scale), 
                        (int)Math.Round((this.yz.Height / 2) - current.Y * this.Scale), 
                        (int)Math.Round(this.JointRadius), 
                        (int)Math.Round(this.JointRadius), 
                        pJoints);

                    // XZ
                    this.xz.FillEllipseCentered(
                        (int)Math.Round((this.xz.Width / 2) + current.X * this.Scale), 
                        (int)Math.Round((this.xz.Height / 2) - current.Z * this.Scale), 
                        (int)Math.Round(this.JointRadius), 
                        (int)Math.Round(this.JointRadius), 
                        pJoints);
                }
            }

            // Draw the arrows on the windows
            this.DrawArrows(this.xy, "Y", "X");
            this.DrawArrows(this.yz, "Y", "Z");
            this.DrawArrows(this.xz, "Z", "X");


        }

        /// <summary>
        /// Method to draw arrows to indicate the axis.
        /// </summary>
        /// <param name="g">
        /// Graphics variable to use to draw.
        /// </param>
        /// <param name="topArrowText">
        /// Text to draw on the top of the arrow.
        /// </param>
        /// <param name="rightArrowText">
        /// Text to draw on the right arrow.
        /// </param>
        private void DrawArrows(WriteableBitmap g, string topArrowText, string rightArrowText)
        {
            var pAxes = Colors.White;
            var bText = Colors.White;

            // Draw the top arrow
            WriteText(g, topArrowText, bText, this.ArrowsBoundingBox.Left - 5, this.ArrowsBoundingBox.Top - 15);

            g.DrawLine(this.ArrowsBoundingBox.Left, this.ArrowsBoundingBox.Top, this.ArrowsBoundingBox.Left, this.ArrowsBoundingBox.Bottom, pAxes);

            g.DrawLine(this.ArrowsBoundingBox.Left, this.ArrowsBoundingBox.Top, this.ArrowsBoundingBox.Left - 5, this.ArrowsBoundingBox.Top + 10, pAxes);

            g.DrawLine(this.ArrowsBoundingBox.Left, this.ArrowsBoundingBox.Top, this.ArrowsBoundingBox.Left + 5, this.ArrowsBoundingBox.Top + 10, pAxes);

            // Draw the right arrow
            WriteText(g, rightArrowText, bText, this.ArrowsBoundingBox.Right - 10 + 12, this.ArrowsBoundingBox.Bottom - 6);

            g.DrawLine(this.ArrowsBoundingBox.Left, this.ArrowsBoundingBox.Bottom, this.ArrowsBoundingBox.Right, this.ArrowsBoundingBox.Bottom, pAxes);

            g.DrawLine(this.ArrowsBoundingBox.Right - 10, this.ArrowsBoundingBox.Bottom - 5, this.ArrowsBoundingBox.Right, this.ArrowsBoundingBox.Bottom, pAxes);

            g.DrawLine(this.ArrowsBoundingBox.Right - 10, this.ArrowsBoundingBox.Bottom + 5, this.ArrowsBoundingBox.Right, this.ArrowsBoundingBox.Bottom, pAxes);
        }

        private static void WriteText(WriteableBitmap g, string text, Color color, int x, int y)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                8.0,
                new SolidColorBrush(color));

            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawText(formattedText, new Point(1, 1));
            drawingContext.Close();

            var render = new RenderTargetBitmap(20, 10, 96, 96, PixelFormats.Pbgra32);
            render.Render(drawingVisual);
            var renderBitmap = new WriteableBitmap(render);

            g.Blit(new Rect(x, y, 20, 10), renderBitmap, new Rect(0, 0, 20, 10));
        }
    }
}
