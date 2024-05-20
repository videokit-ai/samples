/* 
*   VideoKit
*   Copyright (c) 2024 NatML Inc. All Rights Reserved.
*/

namespace VideoKit.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Video;
    using Unity.Collections;
    using UnityEngine.UI;

    public sealed class WatermarkVideoSample : MonoBehaviour {

        [Header(@"Watermarking")]
        public Texture2D watermark;
        public Vector2Int position;

        [Header(@"Playback")]
        public VideoPlayer videoPlayer;
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
    
        private async void Start () {
            // Watermark video
            var video = await MediaAsset.FromStreamingAssets(@"4088192-hd_1920_1080_25fps.mp4");
            var result = await WatermarkVideo(video, watermark, position);
            Debug.Log(result.path);
            // Prepare
            videoPlayer.url = result.path;
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                await Task.Yield();
            // Display
            rawImage.texture = videoPlayer.texture;
            aspectFitter.aspectRatio = (float)videoPlayer.width / videoPlayer.height;
            // Playback
            videoPlayer.Play();
        }

        private static async Task<MediaAsset> WatermarkVideo (
            MediaAsset video,
            Texture2D watermark,
            Vector2Int position
        ) {
            // Create watermark buffer
            using var watermarkBuffer = new PixelBuffer(watermark);
            // Create destination recorder
            var recorder = await MediaRecorder.Create(
                MediaRecorder.Format.MP4,
                video.width,
                video.height,
                video.frameRate
            );
            using var rgbaData = new NativeArray<byte>(video.width * video.height * 4, Allocator.Persistent);
            // Read frames in the video
            foreach (var pixelBuffer in video.Read<PixelBuffer>()) {
                // Convert to `RGBA8888`
                using var rgbaBuffer = new PixelBuffer(
                    video.width,
                    video.height,
                    PixelBuffer.Format.RGBA8888,
                    rgbaData,
                    timestamp: pixelBuffer.timestamp
                );
                pixelBuffer.CopyTo(rgbaBuffer);
                // Add watermark
                using var watermarkRegion = rgbaBuffer.Region(position.x, position.y, watermark.width, watermark.height);
                watermarkRegion.CopyTo(watermarkBuffer, watermarkRegion);
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