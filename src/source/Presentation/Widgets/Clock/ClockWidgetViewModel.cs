// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Chronos.Presentation.ViewModel;

namespace Chronos.Presentation.Widgets
{
    /// <summary>
    /// Based on http://moonydesk.codeplex.com/
    /// </summary>
    public sealed class ClockWidgetViewModel
        : WidgetViewModel
    {
        #region · Fields ·

        private Point   degreeStartPoint;
        private Point   degreeCurrentPoint;
        private bool    isLargeArc;
        private int     dayOfYear;
        private int     year;
        private string  seconds;
        private string  minutes;
        private string  hours;
        private string  pmAm;
        private bool    hours24;
        private string  date;
        private string  dayOfWeek;
        private int     angle;

        #endregion

        #region · Properties ·

        public Point DegreeStartPoint
        {
            get { return this.degreeStartPoint; }
            set
            {
                if (this.degreeStartPoint != value)
                {
                    this.degreeStartPoint = value;

                    this.NotifyPropertyChanged(() => DegreeStartPoint);
                }
            }
        }

        public Point DegreeCurrentPoint
        {
            get { return this.degreeCurrentPoint; }
            set
            {
                if (this.degreeCurrentPoint != value)
                {
                    this.degreeCurrentPoint = value;

                    this.NotifyPropertyChanged(() => DegreeCurrentPoint);
                }
            }
        }

        public bool IsLargeArc
        {
            get { return this.isLargeArc; }
            set
            {
                if (this.isLargeArc != value)
                {
                    this.isLargeArc = value;

                    this.NotifyPropertyChanged(() => IsLargeArc);
                }
            }
        }

        public string Seconds
        {
            get { return this.seconds; }
            set
            {
                if (this.seconds != value)
                {
                    this.seconds = value;

                    this.NotifyPropertyChanged(() => Seconds);
                }
            }
        }

        public string Minutes
        {
            get { return this.minutes; }
            set
            {
                if (this.minutes != value)
                {
                    this.minutes = value;

                    this.NotifyPropertyChanged(() => Minutes);
                }
            }
        }

        public string Hours
        {
            get { return this.hours; }
            set
            {
                if (this.hours != value)
                {
                    this.hours = value;

                    this.NotifyPropertyChanged(() => Hours);
                }
            }
        }

        public string PmAm
        {
            get { return this.pmAm; }
            set
            {
                if (this.pmAm != value)
                {
                    this.pmAm = value;

                    this.NotifyPropertyChanged(() => PmAm);
                }
            }
        }

        public bool Hours24
        {
            get { return this.hours24; }
            set
            {
                if (this.hours24 != value)
                {
                    this.hours24 = value;

                    this.NotifyPropertyChanged(() => Hours24);
                }
            }
        }

        public string Date
        {
            get { return this.date; }
            set
            {
                if (this.date != value)
                {
                    this.date = value;

                    this.NotifyPropertyChanged(() => Date);
                }
            }
        }

        public string DayOfWeek
        {
            get { return this.dayOfWeek; }
            set
            {
                if (this.dayOfWeek != value)
                {
                    this.dayOfWeek = value;

                    this.NotifyPropertyChanged(() => DayOfWeek);
                }
            }
        }

        public int Angle
        {
            get { return this.angle; }
            set
            {
                if (this.angle != value)
                {
                    this.angle = value;

                    this.NotifyPropertyChanged(() => Angle);
                }
            }
        }

        public int DayOfYear
        {
            get { return this.dayOfYear; }
            set
            {
                if (this.dayOfYear != value)
                {
                    this.dayOfYear = value;

                    this.NotifyPropertyChanged(() => DayOfYear);
                }
            }
        }

        public int Year
        {
            get { return this.year; }
            set
            {
                if (this.year != value)
                {
                    this.year = value;

                    this.NotifyPropertyChanged(() => DayOfYear);
                }
            }
        }

        #endregion

        #region · Constructors ·

        public ClockWidgetViewModel()
            : base()
        {
            this.DayOfYear          = -1;
            this.Year               = -1;
            this.Angle              = -4;
            this.Hours24            = Properties.Settings.Default.hours24;
            this.DegreeStartPoint   = new Point(110, 10);
            this.DegreeCurrentPoint = new Point(110, 210);
        }

        #endregion

        #region · Methods ·

        public void Start()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval  = TimeSpan.FromSeconds(0.1);
            timer.Tick      += new EventHandler(OnTimerTick);
            timer.Start();

            this.SetCurTime();
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Set current time
        /// </summary>
        private void SetCurTime()
        {
            DateTime now = DateTime.Now;
            
            this.SetDeg((now.Second + now.Millisecond / 1000.0) * 6);
            this.Seconds = now.Second.ToString("00");
            this.Minutes = now.Minute.ToString("00");

            if (this.Hours24)
            {
                this.Hours  = now.Hour.ToString("00");
                this.PmAm   = String.Empty;
            }
            else
            {
                this.Hours = now.ToString("hh");
                this.PmAm = now.ToString("tt");
            }

            if (now.DayOfYear != this.DayOfYear || now.Year != this.Year)
            {
                this.Date       = now.ToString("d MMMM yyyy");
                this.DayOfWeek  = now.ToString("dddd");
                this.DayOfYear  = now.DayOfYear;
                this.Year       = now.Year;
            }
        }

        /// <summary>
        /// Set seconds arc degree
        /// </summary>
        /// <param name="degree"></param>
        private void SetDeg(double degree)
        {
            double offset   = this.DegreeStartPoint.X;
            double x        = Math.Cos((degree - 90) * Math.PI / 180) * 100.0 + offset;
            double y        = Math.Sin((degree - 90) * Math.PI / 180) * 100.0 + offset;
            
            this.DegreeCurrentPoint = new Point(x, y);
            this.IsLargeArc         = (degree > 180);
        }

        #endregion

        #region · Event Handlers ·

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.SetCurTime();
        }

        #endregion
    }
}
