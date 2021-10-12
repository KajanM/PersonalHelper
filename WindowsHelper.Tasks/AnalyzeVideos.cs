using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FFMpegCore;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;

namespace WindowsHelper.Tasks
{
    public static class AnalyzeVideos
    {
        public static IEnumerable<IGrouping<FieldsToDetermineIfDemuxingIsPossible, IMediaAnalysis>> Execute(AnalyzeVideosOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            var mediaInfos = new List<IMediaAnalysis>();

            foreach (var video in directory.GetVideos())
            {
                try
                {
                    var mediaInfo = FFProbe.Analyse(video.FullName);
                    mediaInfos.Add(mediaInfo);
                    // Log.Information("{@MediaInfo}", mediaInfo);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while analyzing {Path}", video.FullName);
                }
            }

            var groupedData = mediaInfos.GroupBy(info => new FieldsToDetermineIfDemuxingIsPossible
            {
                FormatName = info.Format.FormatName,
                FrameRate = info.PrimaryVideoStream.FrameRate,
                Height = info.PrimaryVideoStream.Height,
                Width = info.PrimaryVideoStream.Width,
            });
            
            foreach (var mediaAnalyses in groupedData)
            {
                Log.Information("Found {Count} videos under {@Key}", mediaAnalyses.Count(), mediaAnalyses.Key);
            }

            return groupedData;
        }

        public class FieldsToDetermineIfDemuxingIsPossible
        {
            public string FormatName { get; set; }
            public double FrameRate { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }

            protected bool Equals(FieldsToDetermineIfDemuxingIsPossible other)
            {
                return FormatName == other.FormatName &&
                       FrameRate.Equals(other.FrameRate) && Height == other.Height && Width == other.Width;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FieldsToDetermineIfDemuxingIsPossible)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(FormatName, FrameRate, Height, Width);
            }

            public static bool operator ==(FieldsToDetermineIfDemuxingIsPossible left, FieldsToDetermineIfDemuxingIsPossible right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(FieldsToDetermineIfDemuxingIsPossible left, FieldsToDetermineIfDemuxingIsPossible right)
            {
                return !Equals(left, right);
            }
        }
    }
}