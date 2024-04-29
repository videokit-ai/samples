/* 
*   VideoKit
*   Copyright (c) 2024 NatML Inc. All Rights Reserved.
*/

namespace VideoKit.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Video;
    using Unity.Collections;
    using VideoKit.Clocks;
    using UnityEngine.UI;

    public sealed class WatermarkVideoSample : MonoBehaviour {

        [Header(@"Watermarking")]
        public Texture2D watermark;
        public RectInt rect;

        [Header(@"Playback")]
        public VideoPlayer videoPlayer;
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
    
        private async void Start () { // INCOMPLETE
            // Watermark video
            var video = await MediaAsset.FromStreamingAssets(@"video.mp4");
            var result = await WatermarkVideo(video, watermark, rect);
            // Playback result
            videoPlayer.url = result.path;
            videoPlayer.Prepare();
        }

        private static async Task<MediaAsset> WatermarkVideo ( // INCOMPLETE // Watermark pls
            MediaAsset video,
            Texture2D watermark,
            RectInt rect
        ) {
            // Create destination recorder
            var recorder = await MediaRecorder.Create(
                MediaRecorder.Format.MP4,
                video.width,
                video.height,
                video.frameRate
            );
            var clock = new FixedClock(video.frameRate);
            using var frameData = new NativeArray<byte>(video.width * video.height * 4, Allocator.Persistent);
            // Read frames in the video
            foreach (var pixelBuffer in video.Read<PixelBuffer>()) {
                // Convert to `RGBA8888`
                using var rgbaBuffer = new PixelBuffer(
                    video.width,
                    video.height,
                    PixelBuffer.Format.RGBA8888,
                    frameData
                );
                pixelBuffer.CopyTo(rgbaBuffer);
                // Add watermark
                
                // Append to the recorder
                recorder.Append(rgbaBuffer);
            }
            // Finish writing
            var result = await recorder.FinishWriting();
            // Return
            return result;
        }
    }
}