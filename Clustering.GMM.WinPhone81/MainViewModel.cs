// Copyright (c) 2015 Anders Gustafsson, Cureos AB.
// All rights reserved.

namespace Clustering.GMM
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using Accord.MachineLearning;
    using Accord.Math;
    using Accord.Statistics.Distributions.Multivariate;

    using Clustering.GMM.Annotations;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MainViewModel : INotifyPropertyChanged
    {
        #region FIELDS

        // Visually distinct colors used in the pie graphics
        private static readonly ColorSequenceCollection Colors = new ColorSequenceCollection();

        private PlotModel plotModel;

        private int k; // the number of clusters assumed present in the data

        private double[][] mixture; // the data points containing the mixture

        private KMeans kmeans;

        private bool canInitializeOrFit;

        #endregion

        #region CONSTRUCTORS

        public MainViewModel()
        {
            var model = new PlotModel
                            {
                                Title = "Normal (Gaussian) distributions",
                                PlotAreaBorderThickness = new OxyThickness(0.0),
                                LegendOrientation = LegendOrientation.Horizontal,
                                LegendBorderThickness = 1.0,
                                LegendPosition = LegendPosition.TopCenter,
                                LegendPlacement = LegendPlacement.Outside,
                                Background = OxyColors.WhiteSmoke
                            };
            model.Axes.Add(
                new LinearAxis
                    {
                        Title = "X",
                        Maximum = 10.0,
                        Minimum = -10.0,
                        IsAxisVisible = false,
                        Position = AxisPosition.Bottom
                    });
            model.Axes.Add(
                new LinearAxis
                    {
                        Title = "Y",
                        Maximum = 10.0,
                        Minimum = -10.0,
                        IsAxisVisible = false,
                        Position = AxisPosition.Left
                    });

            this.CreateDataCommand = new Command(this.DoCreateData);
            this.InitializeKMeansCommand = new Command(this.DoInitializeKMeans, () => this.canInitializeOrFit);
            this.FitGMMCommand = new Command(this.DoFitGMM, () => this.canInitializeOrFit);

            this.PlotModel = model;
            this.NumberOfClusters = 2;

            this.DoCreateData();
        }

        #endregion

        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region PROPERTIES

        public PlotModel PlotModel
        {
            get
            {
                return this.plotModel;
            }
            set
            {
                if (Equals(value, this.plotModel))
                {
                    return;
                }
                this.plotModel = value;
                this.OnPropertyChanged();
            }
        }

        public int NumberOfClusters
        {
            get
            {
                return this.k;
            }
            set
            {
                if (value == this.k)
                {
                    return;
                }
                this.k = value;
                this.OnPropertyChanged();

                this.canInitializeOrFit = false;
                this.ChangeCanExecute();
            }
        }

        public ICommand CreateDataCommand { get; private set; }

        public ICommand InitializeKMeansCommand { get; private set; }

        public ICommand FitGMMCommand { get; private set; }

        #endregion

        #region METHODS

        private static void CreateScatterplot(PlotModel model, ColorSequenceCollection colors, IEnumerable<double[]> graph, int n)
        {
            model.Series.Clear();

            // Add the curve for the mixture points
            var mixtureSeries = new ScatterSeries
            {
                Title = "Mixture",
                MarkerStrokeThickness = 0.0,
                MarkerFill = OxyColors.Gray,
                MarkerType = MarkerType.Diamond,
                MarkerSize = 2.0
            };
            mixtureSeries.Points.AddRange(graph.Select(t => new ScatterPoint(t[0], t[1])));
            model.Series.Add(mixtureSeries);

            for (var i = 0; i < n; i++)
            {
                // Add curves for the clusters to be detected
                var clusterSeries = new ScatterSeries
                {
                    Title = string.Format("D{0}", i + 1),
                    MarkerStrokeThickness = 0.0,
                    MarkerFill = colors[i],
                    MarkerType = MarkerType.Diamond,
                    MarkerSize = 2.0
                };
                model.Series.Add(clusterSeries);
            }

            model.InvalidatePlot(true);
        }

        private void DoCreateData()
        {
            // Generate data with n Gaussian distributions
            var data = new double[this.k][][];

            for (var i = 0; i < this.k; i++)
            {
                // Create random centroid to place the Gaussian distribution
                var mean = Matrix.Random(2, -6.0, +6.0);

                // Create random covariance matrix for the distribution
                var covariance = Accord.Statistics.Tools.RandomCovariance(2, -5, 5);

                // Create the Gaussian distribution
                var gaussian = new MultivariateNormalDistribution(mean, covariance);

                var samples = Tools.Random.Next(150, 250);
                data[i] = gaussian.Generate(samples);
            }

            // Join the generated data
            this.mixture = Matrix.Stack(data);

            // Update the scatter plot
            CreateScatterplot(this.PlotModel, Colors, this.mixture, this.k);

            // Forget previous initialization
            this.kmeans = null;

            this.canInitializeOrFit = true;
            this.ChangeCanExecute();
        }

        private void DoInitializeKMeans()
        {
            // Creates and computes a new 
            // K-Means clustering algorithm:
            this.kmeans = new KMeans(this.k);
            this.kmeans.Compute(this.mixture);

            // Classify all instances in mixture data
            var classifications = this.kmeans.Clusters.Nearest(this.mixture);

            // Draw the classifications
            this.UpdateGraph(classifications);
        }

        private void DoFitGMM()
        {
            // Create a new Gaussian Mixture Model
            var gmm = new GaussianMixtureModel(this.k);

            // If available, initialize with k-means
            if (this.kmeans != null)
            {
                gmm.Initialize(this.kmeans);
            }

            // Compute the model
            gmm.Compute(this.mixture);

            // Classify all instances in mixture data
            var classifications = gmm.Gaussians.Nearest(this.mixture);

            // Draw the classifications
            this.UpdateGraph(classifications);
        }

        private void ChangeCanExecute()
        {
            ((Command)this.InitializeKMeansCommand).ChangeCanExecute(this.InitializeKMeansCommand);
            ((Command)this.FitGMMCommand).ChangeCanExecute(this.FitGMMCommand);
        }

        private void UpdateGraph(IReadOnlyList<int> classifications)
        {
            // Paint the clusters accordingly
            for (var i = 0; i < this.k + 1; i++)
            {
                ((ScatterSeries)this.PlotModel.Series[i]).Points.Clear();
            }

            for (var j = 0; j < this.mixture.Length; j++)
            {
                var c = classifications[j];

                var series = (ScatterSeries)this.PlotModel.Series[c + 1];
                var point = this.mixture[j];
                series.Points.Add(new ScatterPoint(point[0], point[1]));
            }

            this.PlotModel.InvalidatePlot(true);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}