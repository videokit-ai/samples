/* 
*   VideoKit
*   Copyright (c) 2024 NatML Inc. All Rights Reserved.
*/

namespace VideoKit.Examples {

    using UnityEngine;
    using Unity.Collections;
    using VideoKit.Clocks;

    public sealed class SlowMotionVideoSample : MonoBehaviour {

        #region --Inspector--
        public VideoKitCameraManager cameraManager;
        #endregion

    
        #region --Operations--
        private int previewWidth, previewHeight;
        private NativeArray<byte> previewData;
        private MediaRecorder recorder;
        private IClock clock;
        private bool recording;
        private readonly object recordingFence = new object();

        private void Start () {
            // Listen for pixel buffers
            cameraManager.OnPixelBuffer += OnPixelBuffer;
        }

        private void OnPixelBuffer (PixelBuffer pixelBuffer) {
            // Create preview buffer
            previewWidth = pixelBuffer.width;
            previewHeight = pixelBuffer.height;
            if (!previewData.IsCreated)
                previewData = new NativeArray<byte>(previewWidth * previewHeight * 4, Allocator.Persistent);
            // Convert to RGBA for recording
            using var previewBuffer = new PixelBuffer(
                previewWidth,
                previewHeight,
                PixelBuffer.Format.RGBA8888,
                previewData,
                timestamp: clock.timestamp
            );
            pixelBuffer.CopyTo(previewBuffer);
            // Encode
            lock (recordingFence)
                if (recording)
                    recorder.Append(previewBuffer);
        }

        private void OnDisable () {
            // Stop listening for pixel buffers
            cameraManager.OnPixelBuffer -= OnPixelBuffer;
            // Dispose preview data
            previewData.Dispose();
        }
        #endregion


        #region --UI handlers--

        public async void StartRecording () {
            // Start recording
            clock = new RealtimeClock();
            recorder = await MediaRecorder.Create(
                MediaRecorder.Format.MP4,
                previewWidth,
                previewHeight,
                frameRate: 240
            );
            lock (recordingFence)
                recording = true;
        }

        public async void StopRecording () {
            // Stop recording
            lock (recordingFence)
                recording = false;
            var asset = await recorder.FinishWriting();
            // Save to the camera roll
            await asset.SaveToCameraRoll();
        }
        #endregion
    }
}