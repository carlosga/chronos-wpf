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
        private Point _degreeStartPoint;
        private Point _degreeCurrentPoint;
        private bool _isLargeArc;
        private int _dayOfYear;
        private int _year;
        private string _seconds;
        private string _minutes;
        private string _hours;
        private string _pmAm;
        private bool _hours24;
        private string _date;
        private string _dayOfWeek;
        private int _angle;

        public Point DegreeStartPoint
        {
            get { return _degreeStartPoint; }
            set
            {
                if (_degreeStartPoint != value)
                {
                    _degreeStartPoint = value;

                    this.NotifyPropertyChanged(() => DegreeStartPoint);
                }
            }
        }

        public Point DegreeCurrentPoint
        {
            get { return _degreeCurrentPoint; }
            set
            {
                if (_degreeCurrentPoint != value)
                {
                    _degreeCurrentPoint = value;

                    this.NotifyPropertyChanged(() => DegreeCurrentPoint);
                }
            }
        }

        public bool IsLargeArc
        {
            get { return _isLargeArc; }
            set
            {
                if (_isLargeArc != value)
                {
                    _isLargeArc = value;

                    this.NotifyPropertyChanged(() => IsLargeArc);
                }
            }
        }

        public string Seconds
        {
            get { return _seconds; }
            set
            {
                if (_seconds != value)
                {
                    _seconds = value;

                    this.NotifyPropertyChanged(() => Seconds);
                }
            }
        }

        public string Minutes
        {
            get { return _minutes; }
            set
            {
                if (_minutes != value)
                {
                    _minutes = value;

                    this.NotifyPropertyChanged(() => Minutes);
                }
            }
        }

        public string Hours
        {
            get { return _hours; }
            set
            {
                if (_hours != value)
                {
                    _hours = value;

                    this.NotifyPropertyChanged(() => Hours);
                }
            }
        }

        public string PmAm
        {
            get { return _pmAm; }
            set
            {
                if (_pmAm != value)
                {
                    _pmAm = value;

                    this.NotifyPropertyChanged(() => PmAm);
                }
            }
        }

        public bool Hours24
        {
            get { return _hours24; }
            set
            {
                if (_hours24 != value)
                {
                    _hours24 = value;

                    this.NotifyPropertyChanged(() => Hours24);
                }
            }
        }

        public string Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;

                    this.NotifyPropertyChanged(() => Date);
                }
            }
        }

        public string DayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                if (_dayOfWeek != value)
                {
                    _dayOfWeek = value;

                    this.NotifyPropertyChanged(() => DayOfWeek);
                }
            }
        }

        public int Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;

                    this.NotifyPropertyChanged(() => Angle);
                }
            }
        }

        public int DayOfYear
        {
            get { return _dayOfYear; }
            set
            {
                if (_dayOfYear != value)
                {
                    _dayOfYear = value;

                    this.NotifyPropertyChanged(() => DayOfYear);
                }
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                if (_year != value)
                {
                    _year = value;

                    this.NotifyPropertyChanged(() => DayOfYear);
                }
            }
        }

        public ClockWidgetViewModel()
            : base()
        {
            this.DayOfYear = -1;
            this.Year = -1;
            this.Angle = -4;
            this.Hours24 = Properties.Settings.Default.hours24;
            this.DegreeStartPoint = new Point(110, 10);
            this.DegreeCurrentPoint = new Point(110, 210);
        }

        public void Start()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(OnTimerTick);
            timer.Start();

            this.SetCurTime();
        }

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
                this.Hours = now.Hour.ToString("00");
                this.PmAm = String.Empty;
            }
            else
            {
                this.Hours = now.ToString("hh");
                this.PmAm = now.ToString("tt");
            }

            if (now.DayOfYear != this.DayOfYear || now.Year != this.Year)
            {
                this.Date = now.ToString("d MMMM yyyy");
                this.DayOfWeek = now.ToString("dddd");
                this.DayOfYear = now.DayOfYear;
                this.Year = now.Year;
            }
        }

        /// <summary>
        /// Set seconds arc degree
        /// </summary>
        /// <param name="degree"></param>
        private void SetDeg(double degree)
        {
            double offset = this.DegreeStartPoint.X;
            double x = Math.Cos((degree - 90) * Math.PI / 180) * 100.0 + offset;
            double y = Math.Sin((degree - 90) * Math.PI / 180) * 100.0 + offset;

            this.DegreeCurrentPoint = new Point(x, y);
            this.IsLargeArc = (degree > 180);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.SetCurTime();
        }
    }
}
