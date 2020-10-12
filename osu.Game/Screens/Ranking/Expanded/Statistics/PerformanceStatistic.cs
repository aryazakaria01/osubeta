﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Scoring;

namespace osu.Game.Screens.Ranking.Expanded.Statistics
{
    public class PerformanceStatistic : CounterStatistic
    {
        private readonly ScoreInfo score;

        private readonly Bindable<int> performance = new Bindable<int>();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public PerformanceStatistic(ScoreInfo score)
            : base("PP", 0)
        {
            this.score = score;
        }

        [BackgroundDependencyLoader]
        private void load(ScorePerformanceManager performanceManager)
        {
            if (score.PP.HasValue)
            {
                setPerformanceValue(score.PP.Value);
            }
            else
            {
                performanceManager.CalculatePerformanceAsync(score, cancellationTokenSource.Token)
                                  .ContinueWith(t => Schedule(() => setPerformanceValue(t.Result)), cancellationTokenSource.Token);
            }
        }

        private void setPerformanceValue(double? pp)
        {
            if (pp.HasValue)
                performance.Value = (int)Math.Round(pp.Value, MidpointRounding.AwayFromZero);
        }

        public override void Appear()
        {
            base.Appear();
            Counter.Current.BindTo(performance);
        }

        protected override void Dispose(bool isDisposing)
        {
            cancellationTokenSource?.Cancel();
            base.Dispose(isDisposing);
        }
    }
}
