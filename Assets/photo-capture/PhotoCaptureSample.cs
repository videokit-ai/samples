/* 
*   VideoKit
*   Copyright © 2025 Yusuf Olokoba. All Rights Reserved.
*/

namespace VideoKit.Samples {

    using UnityEngine;

    public sealed class PhotoCaptureSample : MonoBehaviour {

        public VideoKitCameraManager cameraManager;

        private void Reset () => cameraManager = FindFirstObjectByType<VideoKitCameraManager>();

        public void CapturePhoto () {

        }
    }
}